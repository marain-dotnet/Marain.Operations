// <copyright file="ContainerSetupBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings;

using System.Collections.Generic;
using System.Threading.Tasks;

using BoDi;

using Corvus.Configuration;
using Corvus.Tenancy;
using Corvus.Testing.SpecFlow;

using Marain.Operations.Hosting.JsonSerialization;
using Marain.Operations.Storage;
using Marain.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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

                serviceCollection.AddInMemoryTenantProvider();

                serviceCollection.AddJsonNetSerializerSettingsProvider();
                serviceCollection.AddJsonNetPropertyBag();
                serviceCollection.AddJsonNetCultureInfoConverter();
                serviceCollection.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                serviceCollection.AddSingleton<JsonConverter>(new OperationStatusConverter());
                serviceCollection.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

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

                serviceCollection.AddSingleton(config);
                serviceCollection.AddSingleton<IConfiguration>(config);

                serviceCollection.AddMarainTenantManagementForBlobStorage();
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