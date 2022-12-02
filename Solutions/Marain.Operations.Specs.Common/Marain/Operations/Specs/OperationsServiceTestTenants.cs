// <copyright file="OperationsServiceTestTenants.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs;

using Corvus.Tenancy;

/// <summary>
/// Makes transient tenants available to tests.
/// </summary>
/// <param name="TransientServiceTenant">
/// Gets the tenant used for the service in this test.
/// </param>
/// <param name="TransientClientTenant">
/// Gets the tenant of the client in this test.
/// </param>
public record OperationsServiceTestTenants(ITenant TransientServiceTenant, ITenant TransientClientTenant);