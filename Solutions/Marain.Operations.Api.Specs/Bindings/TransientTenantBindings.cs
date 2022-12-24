// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System.Threading.Tasks;

    using BoDi;

    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Tenancy;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;

    using Marain.Operations.Specs;
    using Marain.TenantManagement.Testing;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to manage creation and deletion of tenants for test features.
    /// </summary>
    [Binding]
    public static class TransientTenantBindings
    {
        public const string TransientServiceTenantIdFeatureContextKey = "TransientServiceTenantId";

        /// <summary>
        /// Creates a new <see cref="ITenant"/> for the current feature, adding a test <see cref="BlobContainerConfiguration"/>
        /// to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <param name="objectContainer">The SpecFlow DI container.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The newly created client and service tenants are available to tests via DI through the
        /// <see cref="OperationsServiceTestTenants"/> type.
        /// </remarks>
        [BeforeFeature(Order = BindingSequence.TransientTenantSetup)]
        public static async Task SetupTransientTenant(FeatureContext featureContext, IObjectContainer objectContainer)
        {
            OperationsServiceTestTenants transientTenants = await OperationsTestTenantSetup.CreateTestTenantAndEnrollInClaimsAsync(featureContext);

            // Now update the service Id in our configuration and in the function configuration
            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientTenants.TransientServiceTenant);

            objectContainer.RegisterInstanceAs(transientTenants);
        }

        [AfterFeature]
        public static async Task TearDownTenants(FeatureContext featureContext)
        {
            await OperationsTestStorageSetup.TearDownBlobContainersAsync(featureContext);

            var tenantManager = TransientTenantManager.GetInstance(featureContext);
            await featureContext.RunAndStoreExceptionsAsync(() => tenantManager.CleanupAsync()).ConfigureAwait(false);
        }

        private static void UpdateServiceConfigurationWithTransientTenantId(
            FeatureContext featureContext,
            ITenant transientServiceTenant)
        {
            FunctionConfiguration functionConfiguration = FunctionsBindings.GetFunctionConfiguration(featureContext);

            functionConfiguration.EnvironmentVariables.Add(
                "MarainServiceConfiguration:ServiceTenantId",
                transientServiceTenant.Id);

            functionConfiguration.EnvironmentVariables.Add(
                "MarainServiceConfiguration:ServiceDisplayName",
                transientServiceTenant.Name);
        }
    }
}