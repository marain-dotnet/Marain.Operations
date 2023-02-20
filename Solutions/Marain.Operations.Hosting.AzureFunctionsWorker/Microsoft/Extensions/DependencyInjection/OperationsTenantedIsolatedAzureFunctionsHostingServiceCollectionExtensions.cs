// <copyright file="OperationsTenantedIsolatedAzureFunctionsHostingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using System;

using Marain.Tenancy.ClientTenantProvider;

using Menes;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Configuration of services for using the operations repository implemented on top of the
/// tenancy repository.
/// </summary>
public static class OperationsTenantedIsolatedAzureFunctionsHostingServiceCollectionExtensions
{
    /// <summary>
    /// Add the tenanted operations status api when hosting in direct ASP.NET Core pipeline
    /// mode.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="tenancyClientOptions">Tenancy client configuration.</param>
    /// <param name="configureHost">The optional action to configure the host.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddTenantedOperationsStatusApiWithIsolatedAzureFunctionsHosting(
        this IServiceCollection services,
        TenancyClientOptions tenancyClientOptions,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        services.AddOperationsStatusApiWithIsolatedAzureFunctionsHosting(configureHost);
        services.AddTenancyBlobContainerOperationsRepository(tenancyClientOptions);
        return services;
    }

    /// <summary>
    /// Add the tenanted operations control api when hosting in direct ASP.NET Core pipeline
    /// mode.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="statusExternalServiceConfigurationSection">
    /// The configuration section containing the <c>OperationsStatus</c> setting holding the
    /// base URL for the status service. (Conventionally, but not necessarily, a section called
    /// <c>ExternalServices</c> underneath the configuration root.)
    /// </param>
    /// <param name="tenancyClientOptions">Tenancy client configuration.</param>
    /// <param name="configureHost">The optional action to configure the host.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddTenantedOperationsControlApiWithIsolatedAzureFunctionsHosting(
        this IServiceCollection services,
        IConfiguration statusExternalServiceConfigurationSection,
        TenancyClientOptions tenancyClientOptions,
        Action<IOpenApiHostConfiguration>? configureHost = null)
    {
        // TODO: Work out exactly why it's necessary to call the methods in this order. Switching the order
        // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
        // obvious from an initial scan through exactly why this is.
        services.AddOperationsControlApiWithIsolatedAzureFunctionsHosting(
            statusExternalServiceConfigurationSection, configureHost);
        services.AddTenancyBlobContainerOperationsRepository(tenancyClientOptions);
        return services;
    }
}