// <copyright file="MarainTenancyTestServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.Tenancy;
    using Marain.TenantManagement.Testing;

    /// <summary>
    /// Extension methods for configuring DI for Marain Tenancy testing support.
    /// </summary>
    public static class MarainTenancyTestServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the in-memory tenant provider to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddInMemoryTenantProvider(this IServiceCollection serviceCollection)
        {
            // Add directly and via the interface - some testing code may wish to work with the provider directly because
            // it provides some helpful methods to shortcut tenant lookup.
            serviceCollection.AddSingleton<InMemoryTenantProvider>();
            serviceCollection.AddSingleton<ITenantProvider>(sp => sp.GetRequiredService<InMemoryTenantProvider>());

            return serviceCollection;
        }
    }
}
