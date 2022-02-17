// <copyright file="MarainOperationsControlClientOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Client.OperationsControl
{
    using System;

    /// <summary>
    /// Settings for configuring the <see cref="MarainOperationsControl"/> service.
    /// </summary>
    public class MarainOperationsControlClientOptions
    {
        /// <summary>
        /// Gets or sets the base URL of the operations control service.
        /// </summary>
        public Uri OperationsControlServiceBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the resource ID to use when asking the Managed Identity system for a token with which to
        /// communicate with the service. This is typically the App ID of the application created for securing access
        /// to the operations control service.
        /// </summary>
        /// <remarks>
        /// If this is null, no attempt will be made to secure communication with the operations control
        /// service. This may be appropriate for local development scenarios.
        /// </remarks>
        public string ResourceIdForMsiAuthentication { get; set; }
    }
}