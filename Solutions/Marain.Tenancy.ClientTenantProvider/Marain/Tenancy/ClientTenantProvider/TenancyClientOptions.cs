// <copyright file="TenancyClientOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Tenancy.ClientTenantProvider;

using System;

/// <summary>
/// Dup of class from Marain.Tenancy.Client. TODO: work out where this really goes.
/// </summary>
/// <param name="TenancyServiceBaseUri">
/// The base URL of the tenancy service.
/// </param>
/// <param name="ResourceIdForMsiAuthentication">
/// The resource ID to use when asking the Managed Identity system for a token
/// with which to communicate with the tenancy service. This is typically the App ID of the
/// application created for securing access to the tenancy service. In test scenarios, this
/// can be set to null to disable authentication.
/// </param>
public record TenancyClientOptions(
    Uri TenancyServiceBaseUri,
    string? ResourceIdForMsiAuthentication = null);