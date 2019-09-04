// <copyright file="OperationsRepositoryServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Storage.Blob
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            services.SetRootTenantDefaultStorageConfiguration(rootTenantDefaultConfiguration);
            services.AddSingleton<IOperationsRepository, OperationsRepository>();

            return services;
        }
    }
}
