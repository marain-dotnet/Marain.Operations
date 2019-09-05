// <copyright file="OperationsControlApiAndTasksBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using System.Collections.Generic;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.OpenApi;
    using Marain.Operations.Storage;
    using Marain.Operations.Tasks;
    using Menes;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings for the integration tests for <see cref="OperationsStatusOpenApiService"/> and
    /// <see cref="OperationsStatusTasks"/>.
    /// </summary>
    [Binding]
    public static class OperationsControlApiAndTasksBindings
    {
        /// <summary>
        /// Configures the DI container before tests start.
        /// </summary>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@operationsControl", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddLogging();

                    serviceCollection.AddSingleton<IOpenApiDocumentProvider, OpenApiDocumentProvider>();
                    serviceCollection.AddSingleton<ITenantProvider, FakeTenantProvider>();

                    serviceCollection.AddTransient<OperationsControlOpenApiService>();
                    serviceCollection.AddExternalServicesForOperationsControlApi();
                    serviceCollection.AddTransient<IOperationsControlTasks, OperationsControlTasks>();
                    var repository = new FakeOperationsRepository();
                    serviceCollection.AddSingleton<IOperationsRepository>(repository);
                    serviceCollection.AddSingleton(repository);

                    var configData = new Dictionary<string, string>
                    {
                        { "ExternalServices:OperationsStatus", "http://operationsstatus.example.com/" },
                    };
                    IConfigurationRoot config = new ConfigurationBuilder()
                        .AddInMemoryCollection(configData)
                        .Build();
                    serviceCollection.AddSingleton(config);
                });
        }

        [BeforeScenario]
        public static void SetupScenario(FeatureContext featureContext)
        {
            FakeOperationsRepository repository = ContainerBindings.GetServiceProvider(featureContext).GetService<FakeOperationsRepository>();
            repository?.Reset();
        }
    }
}
