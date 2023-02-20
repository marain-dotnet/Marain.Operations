// <copyright file="OperationsTenantedHostingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using Marain.Tenancy.ClientTenantProvider;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Configuration of services for using the operations repository implemented on top of the
/// tenancy repository.
/// </summary>
public static class OperationsTenantedHostingServiceCollectionExtensions
{
    /// <summary>
    /// Enable the tenancy repository based operations repository.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="tenancyClientOptions">Tenancy client configuration.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddTenancyBlobContainerOperationsRepository(
        this IServiceCollection services,
        TenancyClientOptions tenancyClientOptions)
    {
        services.AddSingleton(tenancyClientOptions);
        services.AddBlobContainerV2ToV3Transition();
        services.AddAzureBlobStorageClientSourceFromDynamicConfiguration();

        services.AddBlobContainerOperationsRepository();

        return services;
    }
}