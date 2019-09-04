// <copyright file="OperationsStatusApiAndTasksBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using Corvus.SpecFlow.Extensions;
    using Marain.Operations.Storage;
    using Marain.Operations.Tasks;
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
        [BeforeFeature("@status", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddLogging();

                    serviceCollection.AddSingleton<IOpenApiDocumentProvider, OpenApiDocumentProvider>();

                    serviceCollection.AddTransient<OperationsStatusOpenApiService>();
                    serviceCollection.AddTransient<IOperationsStatusTasks, OperationsStatusTasks>();

                    var repository = new FakeOperationsRepository();
                    serviceCollection.AddSingleton<IOperationsRepository>(repository);
                    serviceCollection.AddSingleton(repository);
                });
        }
    }
}
