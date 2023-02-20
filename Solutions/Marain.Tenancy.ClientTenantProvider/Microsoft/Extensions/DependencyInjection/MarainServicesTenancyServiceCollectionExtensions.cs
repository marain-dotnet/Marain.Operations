// <copyright file="MarainServicesTenancyServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Temporary standin for the class of the same name in Marain.Services.Tenancy while
/// we port to System.Text.Json.
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
    public static IServiceCollection AddMarainServicesTenancySystemTextJson(this IServiceCollection serviceCollection)
    {
        ////if (serviceCollection.Any(s => typeof(IMarainServicesTenancy).IsAssignableFrom(s.ServiceType)))
        ////{
        ////    return serviceCollection;
        ////}

        ////serviceCollection.AddSingleton<TenancyClient>();
        ////serviceCollection.AddSingleton<ITenantProvider>(sp => sp.GetRequiredService<TenancyClient>());

        return serviceCollection;
    }
}