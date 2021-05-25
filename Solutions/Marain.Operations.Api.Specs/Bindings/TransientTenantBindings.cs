﻿// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Marain.Operations.Storage.Blob;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to manage creation and deletion of tenants for test features.
    /// </summary>
    [Binding]
    public static class TransientTenantBindings
    {
        public const string TransientServiceTenantIdFeatureContextKey = "TransientServiceTenantId";

        /// <summary>
        /// Creates a new <see cref="ITenant"/> for the current feature, adding a test <see cref="CosmosConfiguration"/>
        /// to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The newly created tenant is added to the <see cref="FeatureContext"/>. Access it via the helper methods
        /// <see cref="GetTransientTenant(FeatureContext)"/> or <see cref="GetTransientTenantId(FeatureContext)"/>.
        /// </remarks>
        [BeforeFeature(Order = BindingSequence.TransientTenantSetup)]
        public static async Task SetupTransientTenant(FeatureContext featureContext)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantProvider>();
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            // Create a transient service tenant for testing purposes.
            ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantBindings).Assembly,
                "Marain.Operations.Api.Specs.ServiceManifests.OperationsServiceManifest.jsonc").ConfigureAwait(false);

            // Now update the service Id in our configuration and in the function configuration
            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientServiceTenant);

            // Now we need to construct a transient client tenant for the test, and enroll it in the new
            // transient service.
            ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);

            await transientTenantManager.AddEnrollmentAsync(
                transientClientTenant.Id,
                transientServiceTenant.Id,
                GetOperationsConfig(featureContext)).ConfigureAwait(false);

            // TODO: Temporary hack to work around the fact that the transient tenant manager no longer holds the latest
            // version of the tenants it's tracking; see https://github.com/marain-dotnet/Marain.TenantManagement/issues/28
            transientTenantManager.PrimaryTransientClient = await tenantProvider.GetTenantAsync(transientClientTenant.Id).ConfigureAwait(false);
        }

        [AfterFeature]
        public static async Task TearDownTenants(FeatureContext featureContext)
        {
            var tenantManager = TransientTenantManager.GetInstance(featureContext);

            await featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                ITenantCloudBlobContainerFactory cloudBlobContainerFactory = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantCloudBlobContainerFactory>();
                CloudBlobContainer testContainer = await cloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenantManager.PrimaryTransientClient, OperationsRepository.ContainerDefinition).ConfigureAwait(false);
                await testContainer.DeleteIfExistsAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(() => tenantManager.CleanupAsync()).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the transient tenant created for the current feature from the supplied <see cref="FeatureContext"/>,
        /// or null if there is none.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>The <see cref="ITenant"/>.</returns>
        public static ITenant GetTransientTenant(this FeatureContext context)
        {
            return TransientTenantManager.GetInstance(context).PrimaryTransientClient;
        }

        /// <summary>
        /// Retrieves the Id of the transient tenant created for the current feature from the supplied feature context.
        /// <see cref="FeatureContext"/>.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>The Id of the <see cref="ITenant"/>.</returns>
        /// <exception cref="ArgumentNullException">There is no current tenant.</exception>
        public static string GetTransientTenantId(this FeatureContext context)
        {
            return context.GetTransientTenant().Id;
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

        private static EnrollmentConfigurationItem[] GetOperationsConfig(FeatureContext featureContext)
        {
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            // Can't create a logger using the generic type of this class because it's static, so we'll do it using
            // the feature context instead.
            ILogger<FeatureContext> logger = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<ILogger<FeatureContext>>();

            BlobStorageConfiguration blobStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>()
                ?? new BlobStorageConfiguration();

            if (string.IsNullOrEmpty(blobStorageConfiguration.AccountName))
            {
                logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
            }

            return new EnrollmentConfigurationItem[]
            {
                new EnrollmentBlobStorageConfigurationItem
                {
                    Key = "operationsStore",
                    Configuration = blobStorageConfiguration,
                },
            };
        }
    }
}
