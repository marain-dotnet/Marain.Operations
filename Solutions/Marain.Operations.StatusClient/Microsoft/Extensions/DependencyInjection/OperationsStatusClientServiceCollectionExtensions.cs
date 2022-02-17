// <copyright file="OperationsStatusClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Marain.Operations.Client.OperationsStatus;

    /// <summary>
    /// DI initialization for clients of the Operations control service.
    /// </summary>
    public static class OperationsStatusClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="getBaseUri">A callback that can be used to retrieve the base Uri for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddOperationsStatusClient(
            this IServiceCollection services,
            Func<IServiceProvider, Uri> getBaseUri)
        {
            services.AddSingleton(sp =>
            {
                Uri baseUri = getBaseUri(sp);

                if (!baseUri.IsAbsoluteUri)
                {
                    throw new InvalidOperationException(
                        $"Invalid base Uri '{baseUri}' provided. Please ensure that the supplied value is an absolute Uri.");
                }

                return BuildOperationsStatusClient(baseUri);
            });

            return services;
        }

        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUri">The base Uri for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddOperationsStatusClient(
            this IServiceCollection services,
            Uri baseUri)
        {
            return services.AddOperationsStatusClient(_ => baseUri);
        }

        /// <summary>
        /// Adds the Operations control client to a service collection.
        /// </summary>
        /// <param name="baseUri">The base URI of the Operations control service.</param>
        /// <returns>The modified service collection.</returns>
        private static IMarainOperations BuildOperationsStatusClient(Uri baseUri)
        {
            return new UnauthenticatedMarainOperations(baseUri);
        }
    }
}