// <copyright file="TenancyClient.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Tenancy.ClientTenantProvider;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;

using Corvus.Identity.ClientAuthentication;
using Corvus.Json;
using Corvus.Json.PropertyBag;
using Corvus.Json.Serialization;
using Corvus.Tenancy;
using Corvus.Tenancy.Exceptions;

using Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes;

using Microsoft.Extensions.ObjectPool;

/// <summary>
/// Implements methods required by Marain services to validate and work with tenants.
/// </summary>
/// <remarks>
/// <para>
/// This is an experiment to see what we might want the code generation for JSON Schema-driven
/// client libraries to look like.
/// </para>
/// <para>
/// This depends on the types in <c>TenancyClientSchemaTypes</c>. To generate these, install the
/// <c>Corvus.Json.JsonSchema.TypeGeneratorTool</c> tool. We used this version:
/// https://www.nuget.org/packages/Corvus.Json.JsonSchema.TypeGeneratorTool/1.0.3
/// running either:
/// </para>
/// <code>
/// dotnet tool install --global Corvus.Json.JsonSchema.TypeGeneratorTool --version 1.0.3
/// </code>
/// <para>
/// or, if you already have an older version installed, this:</para>
/// <code>
/// dotnet tool update --global Corvus.Json.JsonSchema.TypeGeneratorTool --version 1.0.3
/// </code>
/// <para>
/// Next, at a command prompt set the current directory to this project's <c>TenancyClientSchemaTypes</c>
/// folder.</para>
/// </remarks>
public class TenancyClient : ITenantStore
{
    private static readonly ObjectPool<ArrayPoolBufferWriter<byte>> ArrayPoolWriterPool =
        new DefaultObjectPoolProvider().Create<ArrayPoolBufferWriter<byte>>();

    // TODO: consider IHttpClientFactory, and whether we want to configure the client auth
    // via that.
    private static readonly HttpClient Http = new();
    private readonly IServiceIdentityAccessTokenSource serviceIdentityAccessTokenSource;
    private readonly IJsonPropertyBagFactory propertyBagFactory;
    private readonly Uri tenancyServiceBaseUrl;
    private readonly string[]? scopes;
    private readonly JsonSerializerOptions serializerOptions;

    /// <summary>
    /// Creates a <see cref="TenancyClient"/>.
    /// </summary>
    /// <param name="serviceIdentityAccessTokenSource">
    /// Provides access tokens representing the service identity.
    /// </param>
    /// <param name="options">Configuration.</param>
    /// <param name="propertyBagFactory">Property bag factory for root tenant.</param>
    /// <param name="serializerOptionsProvider">Provides JSON serialization settings.</param>
    public TenancyClient(
        IServiceIdentityAccessTokenSource serviceIdentityAccessTokenSource,
        TenancyClientOptions options,
        IJsonPropertyBagFactory propertyBagFactory,
        IJsonSerializerOptionsProvider serializerOptionsProvider)
    {
        this.serviceIdentityAccessTokenSource = serviceIdentityAccessTokenSource;
        this.propertyBagFactory = propertyBagFactory;
        this.tenancyServiceBaseUrl = options.TenancyServiceBaseUri;
        this.serializerOptions = serializerOptionsProvider.Instance;

        if (options.ResourceIdForMsiAuthentication is not null)
        {
            this.scopes = new[] { $"{options.ResourceIdForMsiAuthentication}/.default" };
        }

        this.Root = new RootTenant(propertyBagFactory);
    }

    /// <inheritdoc/>
    public RootTenant Root { get; } // TODO: this isn't good. Shouldn't need to be a RootTenant. Do we even need this?

    /// <inheritdoc/>
    public Task<ITenant> CreateChildTenantAsync(string parentTenantId, string name)
    {
        return this.CreateChildTenantAsync(parentTenantId, name, null);
    }

    /// <inheritdoc/>
    public Task<ITenant> CreateWellKnownChildTenantAsync(string parentTenantId, Guid wellKnownChildTenantGuid, string name)
    {
        return this.CreateChildTenantAsync(parentTenantId, name, wellKnownChildTenantGuid);
    }

    /// <inheritdoc/>
    public async Task DeleteTenantAsync(string tenantId)
    {
        Uri url = new(
            this.tenancyServiceBaseUrl,
            $"/{tenantId.GetParentId()}/marain/tenant/children/{tenantId}");
        HttpResponseMessage response = await this.SendHttpMessageAsync(
            url, static url => new HttpRequestMessage(HttpMethod.Delete, url))
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new TenantNotFoundException();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new InvalidOperationException();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    /// <inheritdoc/>
    public Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string? continuationToken = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ITenant> GetTenantAsync(string tenantId, string? eTag = null)
    {
        return this.GetTenantByUrlAsync(new Uri(this.tenancyServiceBaseUrl, $"/{tenantId}/marain/tenant"));
    }

    /// <inheritdoc/>
    public async Task<ITenant> UpdateTenantAsync(
        string tenantId,
        string? name = null,
        IEnumerable<KeyValuePair<string, object>>? propertiesToSetOrAdd = null,
        IEnumerable<string>? propertiesToRemove = null)
    {
        UpdateTenantJsonPatchArray patches = name is null
            ? UpdateTenantJsonPatchArray.EmptyArray
            : UpdateTenantJsonPatchArray.FromItems(UpdateTenantJsonPatchEntry.Create(
                UpdateTenantJsonPatchEntry.OpEntity.EnumValues.Add,
                "/name"u8,
                name));

        if (propertiesToSetOrAdd is not null)
        {
            patches = patches.AddRange(
                propertiesToSetOrAdd.Select(kv =>
                    UpdateTenantJsonPatchEntry.Create(
                        UpdateTenantJsonPatchEntry.OpEntity.EnumValues.Add,
                        "/properties/" + kv.Key, // TODO: not the most efficient approach!
                        UpdateTenantJsonPatchEntry.ValueEntity.FromJson(JsonSerializer.SerializeToElement(kv.Value, kv.Value.GetType(), this.serializerOptions)))));
        }

        if (propertiesToRemove is not null)
        {
            patches = patches.AddRange(
                propertiesToRemove.Select(key =>
                    UpdateTenantJsonPatchEntry.Create(
                        UpdateTenantJsonPatchEntry.OpEntity.EnumValues.Remove,
                        "/properties/" + key))); // TODO: not the most efficient approach!
        }

        ArrayPoolBufferWriter<byte>? json = null;
        try
        {
            json = ArrayPoolWriterPool.Get();
            using (Utf8JsonWriter jw = new(json))
            {
                patches.WriteTo(jw);
            }

            Uri url = new(this.tenancyServiceBaseUrl, $"/{tenantId}/marain/tenant");

            // TODO: check whether we're allowed to reuse ReadOnlyMemoryContent across multiple
            // retries like this, or it's like HttpRequestMessage in that we have to construct
            // a new one for each attempt.
            var content = new ReadOnlyMemoryContent(json.WrittenMemory);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json", "charset=utf-8");
            HttpResponseMessage response = await this.SendHttpMessageAsync(
                (url, content),
                static args =>
                    new HttpRequestMessage(HttpMethod.Patch, args.url)
                    {
                        Content = args.content,
                    })
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await this.GetTenantFromResponseAsync(response).ConfigureAwait(false);
        }
        finally
        {
            if (json is not null)
            {
                json.Clear();
                ArrayPoolWriterPool.Return(json);
            }
        }
    }

    private async Task<ITenant> CreateChildTenantAsync(string parentTenantId, string name, Guid? wellKnownChildTenantGuid)
    {
        Uri url = new(
            this.tenancyServiceBaseUrl,
            $"/{parentTenantId}/marain/tenant/children?tenantName={name}{(wellKnownChildTenantGuid is Guid g ? $"wellKnownChildTenantGuid={g}" : string.Empty)}");
        HttpResponseMessage response = await this.SendHttpMessageAsync(
            url,
            static url => new HttpRequestMessage(HttpMethod.Post, url))
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new TenantNotFoundException();
        }

        // TODO: How do we determine a duplicate Id?
        // See https://github.com/marain-dotnet/Marain.Tenancy/issues/237
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new TenantConflictException();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        if (response.Headers.Location is not Uri tenantUrl)
        {
            throw new InvalidOperationException("POST did not return Location header for newly created tenant");
        }

        if (!tenantUrl.IsAbsoluteUri)
        {
            tenantUrl = new Uri(this.tenancyServiceBaseUrl, tenantUrl);
        }

        return await this.GetTenantByUrlAsync(tenantUrl);
    }

    private async Task<ITenant> GetTenantByUrlAsync(Uri url, string? eTag = null)
    {
        HttpResponseMessage response = await this.SendHttpMessageAsync(
            (url, eTag),
            static args =>
            {
                HttpRequestMessage req = new(HttpMethod.Get, args.url);
                if (args.eTag is not null)
                {
                    req.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(args.eTag));
                }

                return req;
            }).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new TenantNotFoundException();
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            throw new TenantNotModifiedException();
        }

        response.EnsureSuccessStatusCode();

        return await this.GetTenantFromResponseAsync(response).ConfigureAwait(false);
    }

    private async ValueTask<ITenant> GetTenantFromResponseAsync(HttpResponseMessage response)
    {
        // The property bag factory clones the JsonElement we pass, meaning it's not expecting us
        // to keep the document around. That's why it's OK for us to dispose this once we're done
        // with it at the end of this method.
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            .ConfigureAwait(false);

        TenancyClientSchemaTypes.Tenant.AnyOf1Entity tenantJson = new TenancyClientSchemaTypes.Tenant(jsonDocument.RootElement).AsAnyOf1Entity;
        IPropertyBag propertyBag = this.propertyBagFactory.Create(tenantJson.Properties.AsJsonElement);

        return new Corvus.Tenancy.Tenant(tenantJson.Id, tenantJson.Name, propertyBag);
    }

    private async Task<HttpResponseMessage> SendHttpMessageAsync<TContext>(
        TContext context,
        Func<TContext, HttpRequestMessage> makeRequest)
    {
        HttpRequestMessage request = makeRequest(context);
        if (this.scopes is not null)
        {
            AccessTokenDetail tokenDetail = await this.serviceIdentityAccessTokenSource.GetAccessTokenAsync(
                new AccessTokenRequest(this.scopes)).ConfigureAwait(false);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenDetail.AccessToken);
        }

        HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

        // TODO: retry and this.serviceIdentityAccessTokenSource.GetReplacementForFailedAccessTokenAsync
        // if we get a 401.
        return response;
    }
}