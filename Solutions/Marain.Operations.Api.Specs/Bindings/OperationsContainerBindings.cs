﻿// <copyright file="OperationsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System;

    using Corvus.Configuration;
    using Corvus.Testing.SpecFlow;

    using Marain.Operations.Client.OperationsControl;
    using Marain.Tenancy.Client;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using TechTalk.SpecFlow;

    [Binding]
    public static class OperationsContainerBindings
    {
        [BeforeFeature(Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void PerFeatureContainerSetup(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddConfigurationForTest("appsettings.json");
                    IConfigurationRoot config = configBuilder.Build();
                    services.AddSingleton<IConfiguration>(config);

                    services.AddLogging(x => x.AddConsole());

                    services.AddJsonNetSerializerSettingsProvider();
                    services.AddJsonNetPropertyBag();
                    services.AddJsonNetCultureInfoConverter();
                    services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                    services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

                    services.AddAzureBlobStorageClientSourceFromDynamicConfiguration();

                    // Tenancy service client.
                    TenancyClientOptions tenancyConfiguration = config.GetSection("TenancyClient").Get<TenancyClientOptions>();

                    if (tenancyConfiguration?.TenancyServiceBaseUri is null)
                    {
                        throw new InvalidOperationException("Could not find a configuration value for TenancyClient:TenancyServiceBaseUri");
                    }

                    services.AddSingleton(tenancyConfiguration);

                    // Disable tenant caching - necessary because we create/update tenants as part of setup.
                    services.AddTenantProviderServiceClient(false);

                    // Token source, to provide authentication when accessing external services.
                    string azureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"];
                    services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
                    services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

                    // Marain tenancy management, required to create transient client/service tenants.
                    services.AddMarainTenantManagementForBlobStorage();
                });
        }

        [BeforeFeature("useClients", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void AddClientsToContainer(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    var controlClientOptions = new MarainOperationsControlClientOptions
                    {
                        OperationsControlServiceBaseUri = new Uri(ApiBindings.ControlApiBaseUrl),
                    };

                    services.AddOperationsControlClient(controlClientOptions);

                    services.AddOperationsStatusClient(new Uri(ApiBindings.StatusApiBaseUrl));
                });
        }
    }
}