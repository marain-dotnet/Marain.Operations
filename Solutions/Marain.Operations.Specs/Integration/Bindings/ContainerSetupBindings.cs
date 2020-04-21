// <copyright file="ContainerSetupBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Configuration;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.Storage;
    using Marain.Services;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    [Binding]
    public static class ContainerSetupBindings
    {
        /// <summary>
        /// Configures the DI container before tests start.
        /// </summary>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature(Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddLogging(config =>
                    {
                        config.SetMinimumLevel(LogLevel.Debug);
                        config.AddConsole();
                    });

                    // We need to call AddRootTenant first otherwise its content factory initialisation
                    // won't register the types correctly.
                    // TODO - create a GitHub issue for this.
                    serviceCollection.AddRootTenant();
                    serviceCollection.AddInMemoryTenantProvider();

                    serviceCollection.AddJsonSerializerSettings();
                    serviceCollection.AddTestNameProvider();
                    serviceCollection.AddMarainServiceConfiguration();
                    serviceCollection.AddMarainServicesTenancy();

                    serviceCollection.AddSingleton<FakeOperationsRepository>();
                    serviceCollection.AddSingleton<IOperationsRepository>(s => s.GetRequiredService<FakeOperationsRepository>());
                    
                    var configData = new Dictionary<string, string>
                    {
                        { "ExternalServices:OperationsStatus", "http://operationsstatus.example.com/" },
                    };

                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddTestConfiguration("appsettings.json", configData);
                    IConfigurationRoot config = configBuilder.Build();

                    serviceCollection.AddSingleton(config);
                    serviceCollection.AddSingleton<IConfiguration>(config);
                });
        }

        [BeforeScenario]
        public static void SetupScenario(FeatureContext featureContext)
        {
            FakeOperationsRepository repository = ContainerBindings.GetServiceProvider(featureContext).GetService<FakeOperationsRepository>();
            repository?.Reset();
        }

        public static async Task SetupTenants(FeatureContext featureContext, string manifestName)
        {
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            var tenantHelper = TransientTenantManager.GetInstance(featureContext);

            ITenant transientServiceTenant = await tenantHelper.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(OperationsControlApiAndTasksBindings).Assembly,
                $"Marain.Operations.Specs.Integration.ServiceManifests.{manifestName}.jsonc").ConfigureAwait(false);

            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientServiceTenant);

            ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);

            await transientTenantManager.AddEnrollmentAsync(
                transientClientTenant.Id,
                transientServiceTenant.Id,
                GetTestOperationsConfig()).ConfigureAwait(false);
        }

        private static EnrollmentConfigurationItem[] GetTestOperationsConfig()
        {
            // Config is needed for the enrollment but doesn't actually matter in practice because
            // we're using a fake store for the specs.
            return new EnrollmentConfigurationItem[]
{
                new EnrollmentBlobStorageConfigurationItem
                {
                    Key = "operationsStore",
                    Configuration = new BlobStorageConfiguration(),
                }
            };
        }

        private static void UpdateServiceConfigurationWithTransientTenantId(
            FeatureContext featureContext,
            ITenant transientServiceTenant)
        {
            MarainServiceConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<MarainServiceConfiguration>();

            configuration.ServiceTenantId = transientServiceTenant.Id;
            configuration.ServiceDisplayName = transientServiceTenant.Name;
        }
    }
}
