// <copyright file="OperationsStatusApiAndTasksBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.OpenApi;
    using Marain.Operations.Storage;
    using Marain.Operations.Tasks;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings for the integration tests for <see cref="OperationsStatusOpenApiService"/> and
    /// <see cref="OperationsStatusTasks"/>.
    /// </summary>
    [Binding]
    public static class OperationsStatusApiAndTasksBindings
    {
        /// <summary>
        /// Configures the DI container before tests start.
        /// </summary>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@operationsStatus", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddSingleton<FakeOperationsRepository>();
                    serviceCollection.AddSingleton<IOperationsRepository>(s => s.GetRequiredService<FakeOperationsRepository>());
                    serviceCollection.AddSingleton<ITenantProvider, FakeTenantProvider>();
                    serviceCollection.AddOperationsStatusApi();
                });
        }
    }
}
