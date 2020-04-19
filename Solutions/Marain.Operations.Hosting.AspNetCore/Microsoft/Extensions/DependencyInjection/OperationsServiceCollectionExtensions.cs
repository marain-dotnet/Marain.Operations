// <copyright file="OperationsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.OpenApi
{
    using System;
    using System.Linq;
    using Marain.Operations.Tasks;
    using Marain.Services.Tenancy.Exceptions;
    using Menes;
    using Menes.Exceptions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for configuring DI for the Operations Open API services.
    /// </summary>
    public static class OperationsServiceCollectionExtensions
    {
        /// <summary>
        /// Add services required by the Operations Status API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddOperationsStatusApi(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            if (services.Any(s => typeof(IOperationsStatusTasks).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddLogging();
            services.AddRootTenant();

            services.AddSingleton<OperationsStatusOpenApiService>();
            services.AddSingleton<IOpenApiService, OperationsStatusOpenApiService>(s => s.GetRequiredService<OperationsStatusOpenApiService>());

            services.AddTransient<IOperationsStatusTasks, OperationsStatusTasks>();
            services.AddOpenApiHttpRequestHosting<SimpleOpenApiContext>((config) =>
            {
                config.Exceptions.Map<TenantNotEnrolledForServiceException>(401);
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>();
                OperationsStatusOpenApiService.MapLinks(config.Links);
                configureHost?.Invoke(config);
            });

            AddMarainTenancyServices(services);

            return services;
        }

        /// <summary>
        /// Add services required by the Operations Status API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddOperationsControlApi(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration>? configureHost = null)
        {
            if (services.Any(s => typeof(IOperationsControlTasks).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddLogging();
            services.AddRootTenant();

            services.AddSingleton<OperationsControlOpenApiService>();
            services.AddSingleton<IOpenApiService, OperationsControlOpenApiService>(s => s.GetRequiredService<OperationsControlOpenApiService>());
            services.AddTransient<IOperationsControlTasks, OperationsControlTasks>();

            services.AddOpenApiHttpRequestHosting<SimpleOpenApiContext>(config =>
            {
                config.Exceptions.Map<TenantNotEnrolledForServiceException>(401);

                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<OperationsControlOpenApiService>();
                configureHost?.Invoke(config);
            });

            AddExternalServicesForOperationsControlApi(services);
            AddMarainTenancyServices(services);

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
            services.AddTenantProviderServiceClient();
            services.AddMarainServiceTenancyHelper();

            return services;
        }
    }
}
