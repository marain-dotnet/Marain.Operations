// <copyright file="OperationsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.OpenApi
{
    using System;
    using System.Linq;

    using Marain.Operations.Hosting.JsonSerialization;
    using Marain.Operations.Tasks;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods for configuring DI for the Operations Open API services.
    /// </summary>
    public static class OperationsServiceCollectionExtensions
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
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddOperationsControlApiWithAspNetPipelineHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddOperationsControlApiNonHostingTypeSpecific();

            services.AddOpenApiAspNetPipelineHosting<SimpleOpenApiContext>(config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
                configureHost?.Invoke(config);
            });

            return services;
        }

        /// <summary>
        /// Add services required by the Operations Control API when hosting in Action Result mode
        /// (e.g., in Azure Functions).
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddOperationsControlApiWithOpenApiActionResultHosting(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddOperationsControlApiNonHostingTypeSpecific();

            services.AddOpenApiActionResultHosting<SimpleOpenApiContext>(config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
                configureHost?.Invoke(config);
            });

            return services;
        }

        /// <summary>
        /// Adds services required for resolving URLs to external services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        internal static IServiceCollection AddExternalServicesForOperationsControlApi(this IServiceCollection services)
        {
            services.AddExternalServices(
                "ExternalServices",
                externalServices => externalServices.AddExternalServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>("OperationsStatus"));

            return services;
        }

        /// <summary>
        /// Adds services required to access tenant information.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        internal static IServiceCollection AddMarainTenancyServices(this IServiceCollection services)
        {
            // TODO: Work out exactly why it's necessary to call the methods in this order. Switching the order
            // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
            // obvious from an initial scan through exactly why this is.
            services.AddTenantProviderServiceClient(true);
            services.AddMarainServiceConfiguration();
            services.AddSingleton<JsonConverter>(new OperationStatusConverter());

            services.AddMarainServicesTenancy();

            return services;
        }

        private static IServiceCollection AddOperationsStatusApiNonHostingTypeSpecific(
            this IServiceCollection services)
        {
            services.AddLogging();

            services.AddSingleton<OperationsStatusOpenApiService>();
            services.AddSingleton<IOpenApiService, OperationsStatusOpenApiService>(s => s.GetRequiredService<OperationsStatusOpenApiService>());

            services.AddTransient<IOperationsStatusTasks, OperationsStatusTasks>();
            services.AddMarainTenancyServices();

            return services;
        }

        private static IServiceCollection AddOperationsControlApiNonHostingTypeSpecific(
            this IServiceCollection services)
        {
            services.AddLogging();

            services.AddSingleton<OperationsControlOpenApiService>();
            services.AddSingleton<IOpenApiService, OperationsControlOpenApiService>(s => s.GetRequiredService<OperationsControlOpenApiService>());
            services.AddTransient<IOperationsControlTasks, OperationsControlTasks>();

            services.AddExternalServicesForOperationsControlApi();
            services.AddMarainTenancyServices();

            return services;
        }
    }
}