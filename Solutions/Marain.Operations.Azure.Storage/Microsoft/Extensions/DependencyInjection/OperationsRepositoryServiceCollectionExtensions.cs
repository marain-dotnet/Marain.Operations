// <copyright file="OperationsRepositoryServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Marain.Operations.OpenApi;
    using Marain.Operations.Storage;
    using Marain.Operations.Storage.Blob;
    using Menes;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Configuration of services for using the operations repository implemented on top of the
    /// tenancy repository.
    /// </summary>
    public static class OperationsRepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// Enable the tenancy repository based operations repository.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootTenantDefaultConfiguration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenancyBlobContainerOperationsRepository(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration)
        {
            services.AddTenantProviderBlobStore();
            services.AddTenantCloudBlobContainerFactory(rootTenantDefaultConfiguration);
            services.AddSingleton<IOperationsRepository, OperationsRepository>();

            return services;
        }

        /// <summary>
        /// Add the tenanted operations status api.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootTenantDefaultConfiguration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsStatusApi(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            services.AddTenancyBlobContainerOperationsRepository(rootTenantDefaultConfiguration);
            services.AddOperationsStatusApi(configureHost);
            return services;
        }

        /// <summary>
        /// Add the tenanted operations control api.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootTenantDefaultConfiguration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsControlApi(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            services.AddTenancyBlobContainerOperationsRepository(rootTenantDefaultConfiguration);
            services.AddOperationsControlApi(configureHost);
            return services;
        }
    }
}
