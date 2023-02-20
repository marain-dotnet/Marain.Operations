// <copyright file="OperationsAspNetHostingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

using Marain.Operations.Hosting.JsonSerialization;
using Marain.Operations.OpenApi;
using Marain.Operations.Tasks;

using Menes;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Extension methods for configuring DI for the Operations Open API services.
/// </summary>
public static class OperationsAspNetHostingServiceCollectionExtensions
{
    /// <summary>
    /// Add services required by the Operations Status API when hosting in direct ASP.NET Core
    /// pipeline mode.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsStatusApiWithAspNetPipelineHosting(
        this IServiceCollection services,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsStatusTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsStatusApiNonHostingTypeSpecific();

        services.AddOpenApiAspNetPipelineHosting<SimpleOpenApiContext>((config) =>
        {
            config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>();
            OperationsStatusOpenApiService.MapLinks(config.Links);
            configureHost?.Invoke(config);
        });

        return services;
    }

    /// <summary>
    /// Add services required by the Operations Status API when hosting in Action Result mode
    /// (e.g., in Azure Functions).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsStatusApiWithOpenApiActionResultHosting(
        this IServiceCollection services,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsStatusTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsStatusApiNonHostingTypeSpecific();

        services.AddOpenApiActionResultHosting<SimpleOpenApiContext>((config) =>
        {
            config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>();
            OperationsStatusOpenApiService.MapLinks(config.Links);
            configureHost?.Invoke(config);
        });

        return services;
    }

    /// <summary>
    /// Add services required by the Operations Control API when hosting in direct ASP.NET Core
    /// pipeline mode.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="statusExternalServiceConfigurationSection">
    /// The configuration section containing the <c>OperationsStatus</c> setting holding the
    /// base URL for the status service. (Conventionally, but not necessarily, a section called
    /// <c>ExternalServices</c> underneath the configuration root.)
    /// </param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsControlApiWithAspNetPipelineHosting(
        this IServiceCollection services,
        IConfiguration statusExternalServiceConfigurationSection,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsControlApiNonHostingTypeSpecific(statusExternalServiceConfigurationSection);

        services.AddOpenApiAspNetPipelineHosting<SimpleOpenApiContext>(
            config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
                configureHost?.Invoke(config);
            },
            OperationsOpenApiConfiguration.AddJsonConverters);

        return services;
    }

    /// <summary>
    /// Add services required by the Operations Control API when hosting in Action Result mode
    /// (e.g., in Azure Functions).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="statusExternalServiceConfigurationSection">
    /// The configuration section containing the <c>OperationsStatus</c> setting holding the
    /// base URL for the status service. (Conventionally, but not necessarily, a section called
    /// <c>ExternalServices</c> underneath the configuration root.)
    /// </param>
    /// <param name="configureHost">Optional callback for additional host configuration.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsControlApiWithOpenApiActionResultHosting(
        this IServiceCollection services,
        IConfiguration statusExternalServiceConfigurationSection,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
        {
            return services;
        }

        services.AddOperationsControlApiNonHostingTypeSpecific(statusExternalServiceConfigurationSection);

        services.AddOpenApiActionResultHosting<SimpleOpenApiContext>(config =>
        {
            config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
            configureHost?.Invoke(config);
        });

        return services;
    }
}