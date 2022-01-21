// <copyright file="OperationsRepositoryServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    using Marain.Operations.OpenApi;
    using Marain.Operations.Storage;
    using Marain.Operations.Storage.Blob;
    using Marain.Tenancy.Client;

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
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenancyBlobContainerOperationsRepository(
            this IServiceCollection services)
        {
            services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());
            services.AddBlobContainerV2ToV3Transition();
            services.AddAzureBlobStorageClientSourceFromDynamicConfiguration();

            services.AddSingleton<IOperationsRepository, OperationsRepository>();

            return services;
        }

        /// <summary>
        /// Add the tenanted operations status api when hosting in direct ASP.NET Core pipeline
        /// mode.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsStatusApiWithAspNetPipelineHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            services.AddOperationsStatusApiWithAspNetPipelineHosting(configureHost);
            services.AddTenancyBlobContainerOperationsRepository();
            return services;
        }

        /// <summary>
        /// Add the tenanted operations status api when hosting in Action Result mode (e.g., in
        /// Azure Functions).
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsStatusApiWithOpenApiActionResultHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            services.AddOperationsStatusApiWithOpenApiActionResultHosting(configureHost);
            services.AddTenancyBlobContainerOperationsRepository();
            return services;
        }

        /// <summary>
        /// Add the tenanted operations control api when hosting in direct ASP.NET Core pipeline
        /// mode.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsControlApiWithAspNetPipelineHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            // TODO: Work out exactly why it's necessary to call the methods in this order. Switching the order
            // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
            // obvious from an initial scan through exactly why this is.
            services.AddOperationsControlApiWithAspNetPipelineHosting(configureHost);
            services.AddTenancyBlobContainerOperationsRepository();
            return services;
        }

        /// <summary>
        /// Add the tenanted operations control api when hosting in Action Result mode (e.g., in
        /// Azure Functions).
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">The optional action to configure the host.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddTenantedOperationsControlApiWithOpenApiActionResultHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            // TODO: Work out exactly why it's necessary to call the methods in this order. Switching the order
            // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
            // obvious from an initial scan through exactly why this is.
            services.AddOperationsControlApiWithOpenApiActionResultHosting(configureHost);
            services.AddTenancyBlobContainerOperationsRepository();
            return services;
        }
    }
}
