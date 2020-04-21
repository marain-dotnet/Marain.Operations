// <copyright file="MarainServicesTenancyServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Marain.Services;
    using Marain.Services.Tenancy;
    using Marain.Services.Tenancy.Internal;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// DI configuration helpers for Marain service tenancy helpers.
    /// </summary>
    public static class MarainServicesTenancyServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Marain service tenancy helper to the service collection. Also adds Marain Tenant Management features,
        /// which this depends on. An implementation of ITenantProvider is also required (the implementation from
        /// Marain.Tenancy.ClientTenantProvider is the most likely candidate).
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddMarainServicesTenancy(this IServiceCollection serviceCollection)
        {
            if (serviceCollection.Any(s => typeof(IMarainServicesTenancy).IsAssignableFrom(s.ServiceType)))
            {
                return serviceCollection;
            }

            serviceCollection.AddMarainTenantManagement();

            serviceCollection.AddSingleton<MarainServicesTenancy>();
            serviceCollection.AddSingleton<IMarainServicesTenancy>(sp => sp.GetRequiredService<MarainServicesTenancy>());

            return serviceCollection;
        }

        /// <summary>
        /// Adds Marain service configuration to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <param name="configurationSectionName">The name of the configuration section in config.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddMarainServiceConfiguration(
            this IServiceCollection serviceCollection,
            string configurationSectionName = "MarainServiceConfiguration")
        {
            serviceCollection.AddSingleton(
                sp => sp.GetRequiredService<IConfiguration>().GetSection(configurationSectionName).Get<MarainServiceConfiguration>());

            return serviceCollection;
        }
    }
}
