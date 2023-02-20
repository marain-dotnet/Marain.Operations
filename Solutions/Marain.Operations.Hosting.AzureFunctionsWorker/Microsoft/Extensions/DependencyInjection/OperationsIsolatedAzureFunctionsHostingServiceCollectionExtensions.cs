// <copyright file="OperationsIsolatedAzureFunctionsHostingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Text.Json.Serialization;

using Marain.Operations.Hosting.JsonSerialization;
using Marain.Operations.OpenApi;
using Marain.Operations.Tasks;

using Menes;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Extension methods for configuring DI for the Operations Open API services.
/// </summary>
public static class OperationsIsolatedAzureFunctionsHostingServiceCollectionExtensions
{
    /// <summary>
    /// Add services required by the Operations Status API when hosting in an isolated Azure
    /// Function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsStatusApiWithIsolatedAzureFunctionsHosting(
        this IServiceCollection services,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsStatusTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsStatusApiNonHostingTypeSpecific();

        // TODO: Work if it's still necessary to call the methods in this order. Switching the order
        // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
        // obvious from an initial scan through exactly why this is.
        ////services.AddTenantProviderServiceClient(true);
        services.AddMarainServiceConfiguration();
        services.AddSingleton<JsonConverter>(new OperationStatusConverter());

        services.AddMarainServicesTenancySystemTextJson();

        services.AddOpenApiAzureFunctionsWorkerHosting<SimpleOpenApiContext>(
            (config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>();
                OperationsStatusOpenApiService.MapLinks(config.Links);
                configureHost?.Invoke(config);
            },
            OperationsOpenApiConfiguration.AddJsonConverters);

        return services;
    }

    /// <summary>
    /// Add services required by the Operations Control API when hosting in an isolated Azure
    /// Function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="statusExternalServiceConfigurationSection">
    /// The configuration section containing the <c>OperationsStatus</c> setting holding the
    /// base URL for the status service. (Conventionally, but not necessarily, a section called
    /// <c>ExternalServices</c> underneath the configuration root.)
    /// </param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsControlApiWithIsolatedAzureFunctionsHosting(
        this IServiceCollection services,
        IConfiguration statusExternalServiceConfigurationSection,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsControlApiNonHostingTypeSpecific(statusExternalServiceConfigurationSection);

        services.AddOpenApiAzureFunctionsWorkerHosting<SimpleOpenApiContext>(
            config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
                configureHost?.Invoke(config);
            },
            OperationsOpenApiConfiguration.AddJsonConverters);

        return services;
    }
}