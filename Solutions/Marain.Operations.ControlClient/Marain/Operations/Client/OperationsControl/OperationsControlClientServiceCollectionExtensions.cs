// <copyright file="OperationsControlClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Client.OperationsControl
{
    using System;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the Operations control service.
    /// </summary>
    public static class OperationsControlClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUri">The base URI of the Operations control service.</param>
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddOperationsControlClient(
            this IServiceCollection services,
            Uri baseUri,
            string resourceIdForMsiAuthentication = null)
        {
            return resourceIdForMsiAuthentication == null
                ? services.AddSingleton<IMarainOperationsControl>(new UnauthenticatedMarainOperationsControl(baseUri))
                : services.AddSingleton<IMarainOperationsControl>(sp =>
                    new MarainOperationsControl(
                        baseUri,
                        new TokenCredentials(
                            new ServiceIdentityTokenProvider(
                                sp.GetRequiredService<IServiceIdentityTokenSource>(),
                                resourceIdForMsiAuthentication))));
        }
    }
}
