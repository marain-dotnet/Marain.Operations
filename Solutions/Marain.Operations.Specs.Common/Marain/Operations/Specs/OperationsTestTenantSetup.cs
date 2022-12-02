// <copyright file="OperationsTestTenantSetup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs;

using System;
using System.Threading.Tasks;

using Corvus.Tenancy;
using Corvus.Testing.SpecFlow;

using Marain.TenantManagement.EnrollmentConfiguration;
using Marain.TenantManagement.Testing;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

/// <summary>
/// Code shared across <c>Marain.Operations.Specs</c> and <c>Marain.Operations.OpenApi.Specs</c> for
/// setting up blob storage.
/// </summary>
public static  class OperationsTestTenantSetup
{
    /// <summary>
    /// Creates test client and service tenants and enrolls the client in the service, creating
    /// suitable containers in blob storage.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <returns>
    /// A task that produces a the id of the transient client tenant.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The service tenant is based on the real service manifest, but we create it with a new
    /// identifier, so that each test invocation creates its own distinct service tenant. (This
    /// prevents problems if tests are parallelized.)
    /// </para>
    /// </remarks>
    public static async Task<OperationsServiceTestTenants> CreateTestTenantAndEnrollInClaimsAsync(FeatureContext featureContext)
    {
        IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
        ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
        var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
        await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

        // Create a transient service tenant for testing purposes.
        ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
            typeof(OperationsTestTenantSetup).Assembly,
            "ServiceManifests.OperationsServiceManifest.jsonc").ConfigureAwait(false);

        // Starting with Marain.TenantManagement v3, we must set up the container during enrollment. (With
        // older versions, the container would be created dynamically.)
        EnrollmentConfigurationEntry enrollmentConfiguration = await OperationsTestStorageSetup
            .CreateEnrollmentConfigurationAndEnsureContainersExistForEnrollmentAsync(featureContext, serviceProvider)
            .ConfigureAwait(false);

        // Now we need to construct a transient client tenant for the test, and enroll it in the new
        // transient service.
        ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);
        await transientTenantManager.AddEnrollmentAsync(
            transientClientTenant.Id,
            transientServiceTenant.Id,
            enrollmentConfiguration).ConfigureAwait(false);

        // TODO: Temporary hack to work around the fact that the transient tenant manager no longer holds the latest
        // version of the tenants it's tracking; see https://github.com/marain-dotnet/Marain.TenantManagement/issues/28
        transientTenantManager.PrimaryTransientClient = await tenantProvider.GetTenantAsync(transientClientTenant.Id).ConfigureAwait(false);

        return new OperationsServiceTestTenants(
                transientServiceTenant,
                transientClientTenant);
    }
}