// <copyright file="ContainerSetupBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BoDi;

    using Corvus.Configuration;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.Operations.Storage;
    using Marain.Services;
    using Marain.Tenancy.ClientTenantProvider;

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

                    // Marain Tenancy testing support.
                    serviceCollection.AddInMemoryTenantProvider();

                    serviceCollection.AddTestNameProvider();
                    serviceCollection.AddMarainServiceConfiguration();
                    serviceCollection.AddMarainServicesTenancy();

                    serviceCollection.AddAzureBlobStorageClientSourceFromDynamicConfiguration();

                    serviceCollection.AddSingleton<FakeOperationsRepository>();
                    serviceCollection.AddSingleton<IOperationsRepository>(s => s.GetRequiredService<FakeOperationsRepository>());

                    var configData = new Dictionary<string, string>
                    {
                        { "ExternalServices:OperationsStatus", "http://operationsstatus.example.com/" },
                    };

                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddConfigurationForTest("appsettings.json", configData);
                    IConfigurationRoot config = configBuilder.Build();
                    featureContext.Set(config);

                    serviceCollection.AddSingleton(config);
                    serviceCollection.AddSingleton<IConfiguration>(config);

                    serviceCollection.AddMarainTenantManagementForBlobStorage();

                    string azureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"]!;
                    serviceCollection.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
                    TenancyClientOptions tenancyConfiguration = config.GetSection("TenancyClient").Get<TenancyClientOptions>()!;
                    serviceCollection.AddSingleton(tenancyConfiguration);
                });
        }

        [BeforeFeature(Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static Task TaskSetupOperationsControlTenants(FeatureContext featureContext, IObjectContainer objectContainer)
        {
            return SetupTenants(featureContext, objectContainer);
        }

        [BeforeScenario]
        public static void SetupScenario(FeatureContext featureContext)
        {
            FakeOperationsRepository repository = ContainerBindings.GetServiceProvider(featureContext).GetService<FakeOperationsRepository>()!;
            repository.Reset();
        }

        public static async Task SetupTenants(FeatureContext featureContext, IObjectContainer objectContainer)
        {
            OperationsServiceTestTenants transientTenants = await OperationsTestTenantSetup.CreateTestTenantAndEnrollInClaimsAsync(featureContext);

            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientTenants.TransientServiceTenant);

            objectContainer.RegisterInstanceAs(transientTenants);
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