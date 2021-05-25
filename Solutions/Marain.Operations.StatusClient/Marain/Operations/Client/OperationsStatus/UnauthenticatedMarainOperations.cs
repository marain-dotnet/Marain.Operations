﻿// <copyright file="OperationsControlClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Client.OperationsStatus
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// Operations Status API client for use in scenarios where authentication is not required.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In scenarios in which inter-service communication is secured at a networking level, it
    /// might be unnecessary to authenticate requests. The base proxy type supports this but only
    /// through protected constructors. This type makes a suitable constructor available publicly.
    /// </para>
    /// </remarks>
    internal class UnauthenticatedMarainOperations : MarainOperations
    {
        /// <summary>
        /// Create an <see cref="UnauthenticatedMarainOperations"/>.
        /// </summary>
        /// <param name="baseUri">The base URI of the Operations control service.</param>
        /// <param name="handlers">Optional request processing handlers.</param>
        public UnauthenticatedMarainOperations(Uri baseUri, params DelegatingHandler[] handlers)
            : base(baseUri, handlers)
        {
        }
    }
}
