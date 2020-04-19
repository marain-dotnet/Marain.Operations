// <copyright file="MarainServiceConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services
{
    using System;

    /// <summary>
    /// Standard configuration that all Marain services will have.
    /// </summary>
    /// <remarks>
    /// This class does not force initial values for its properties to be supplied via the constructor because it's expected
    /// to be used with <c>IConfiguration.GetSection(string)</c>.
    /// </remarks>
    public class MarainServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the Id of the Service Tenant corresponding to the Marain Service.
        /// </summary>
#nullable disable annotations
        public string ServiceTenantId { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets or sets the display name of the Marain service.
        /// </summary>
#nullable disable annotations
        public string ServiceDisplayName { get; set; }
#nullable restore annotations
    }
}
