// <copyright file="OperationsTestStorageSetup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs;

using Azure.Storage.Blobs;

using Corvus.Storage.Azure.BlobStorage;
using Corvus.Storage.Azure.BlobStorage.Tenancy;
using Corvus.Tenancy;
using Corvus.Testing.SpecFlow;

using Marain.Operations.Storage.Blob;
using Marain.TenantManagement.Configuration;
using Marain.TenantManagement.EnrollmentConfiguration;
using Marain.TenantManagement.Testing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TechTalk.SpecFlow;

/// <summary>
/// Code shared across <c>Marain.Operations.Specs</c> and <c>Marain.Operations.OpenApi.Specs</c> for
/// setting up blob storage.
/// </summary>
public static class OperationsTestStorageSetup
{
    /// <summary>
    /// Builds an <see cref="EnrollmentConfigurationEntry"/> for Claims with unique container
    /// names and creates those containers.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <param name="serviceProvider">DI service provider.</param>
    /// <returns>
    /// A task producing the new <see cref="EnrollmentConfigurationEntry"/>.
    /// </returns>
    public static async Task<EnrollmentConfigurationEntry> CreateEnrollmentConfigurationAndEnsureContainersExistForEnrollmentAsync(
        FeatureContext featureContext,
        IServiceProvider serviceProvider)
    {
        EnrollmentConfigurationEntry enrollmentConfiguration = CreateOperationsConfig(featureContext);
        IBlobContainerSourceFromDynamicConfiguration blobContainerSource = serviceProvider.GetRequiredService<IBlobContainerSourceFromDynamicConfiguration>();
        var operationsContainerConfig = (BlobStorageConfigurationItem)enrollmentConfiguration.ConfigurationItems[OperationsRepository.OperationsV3ConfigKey];
        BlobContainerClient operationsContainer = await blobContainerSource.GetStorageContextAsync(operationsContainerConfig.Configuration);
        await operationsContainer.CreateIfNotExistsAsync();

        return enrollmentConfiguration;
    }

    /// <summary>
    /// Tears down blob containers created for the transient tenant's Operations enrollment.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <returns>
    /// A task that completes once the container has been torn down.
    /// </returns>
    public static async Task TearDownBlobContainersAsync(FeatureContext featureContext)
    {
        ITenant transientClientTenant = TransientTenantManager.GetInstance(featureContext).PrimaryTransientClient;
        IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);

        if (transientClientTenant != null && serviceProvider != null)
        {
            IBlobContainerSourceFromDynamicConfiguration containerSource = serviceProvider.GetRequiredService<IBlobContainerSourceFromDynamicConfiguration>();

            await featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                BlobContainerClient operationsContainer = await containerSource
                    .GetBlobContainerClientFromTenantAsync(transientClientTenant, OperationsRepository.OperationsV3ConfigKey)
                    .ConfigureAwait(false);
                await operationsContainer.DeleteIfExistsAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Builds an <see cref="EnrollmentConfigurationEntry"/> for Operations, with a unique
    /// container name.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <returns>
    /// An <see cref="EnrollmentConfigurationEntry"/> suitable for enrolling a tenant in Operations.
    /// </returns>
    private static EnrollmentConfigurationEntry CreateOperationsConfig(FeatureContext featureContext)
    {
        // We need each test run to have a distinct container. We want these test-generated
        // containers to be easily recognized in storage accounts, so we don't just want to use
        // GUIDs.
        string testRunId = DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff");

        IConfiguration configuration = ContainerBindings
            .GetServiceProvider(featureContext)
            .GetRequiredService<IConfiguration>();

        // Can't create a logger using the generic type of this class because it's static, so we'll do it using
        // the feature context instead.
        ILogger<FeatureContext> logger = ContainerBindings
            .GetServiceProvider(featureContext)
            .GetRequiredService<ILogger<FeatureContext>>();

        BlobContainerConfiguration operationsStoreStorageConfiguration =
            configuration.GetSection("TestBlobStorageConfiguration").Get<BlobContainerConfiguration>()
            ?? new BlobContainerConfiguration();

        operationsStoreStorageConfiguration.Container = $"specs-operations-{testRunId}";

        if (string.IsNullOrEmpty(operationsStoreStorageConfiguration.AccountName))
        {
            logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
        }

        return new EnrollmentConfigurationEntry(
            new Dictionary<string, ConfigurationItem>
            {
                {
                    OperationsRepository.OperationsV3ConfigKey,
                    new BlobStorageConfigurationItem
                    {
                        Configuration = operationsStoreStorageConfiguration,
                    }
                },
            },
            null);
    }
}