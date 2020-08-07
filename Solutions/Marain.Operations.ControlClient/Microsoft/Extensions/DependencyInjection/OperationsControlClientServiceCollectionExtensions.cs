// <copyright file="OperationsControlClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Operations.Client.OperationsControl;
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
        /// <param name="getOptions">A callback that can be used to retrieve options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddOperationsControlClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainOperationsControlClientOptions> getOptions)
        {
            services.AddSingleton(sp =>
            {
                MarainOperationsControlClientOptions options = getOptions(sp);

                if (options is null || options.OperationsControlServiceBaseUri == null)
                {
                    throw new InvalidOperationException("Cannot instantiate the Operations Control client without supplying the BaseUri for the service.");
                }

                return BuildOperationsControlClient(sp, options.OperationsControlServiceBaseUri, options.ResourceIdForMsiAuthentication);
            });

            return services;
        }

        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="options">Options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddOperationsControlClient(
            this IServiceCollection services,
            MarainOperationsControlClientOptions options)
        {
            return services.AddOperationsControlClient(_ => options);
        }

        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="baseUri">The base URI of the Operations control service.</param>
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        private static IMarainOperationsControl BuildOperationsControlClient(
            IServiceProvider serviceProvider,
            Uri baseUri,
            string resourceIdForMsiAuthentication = null)
        {
            return resourceIdForMsiAuthentication == null
                ? new UnauthenticatedMarainOperationsControl(baseUri)
                : new MarainOperationsControl(
                        baseUri,
                        new TokenCredentials(
                            new ServiceIdentityTokenProvider(
                                serviceProvider.GetRequiredService<IServiceIdentityTokenSource>(),
                                resourceIdForMsiAuthentication)));
        }
    }
}
