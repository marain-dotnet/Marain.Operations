// <copyright file="OperationsControlApiAndTasksBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using Corvus.Testing.SpecFlow;

    using Marain.Operations.OpenApi;
    using Marain.Operations.Tasks;
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
                    // TBD: better way to pass the config?
                    IConfigurationRoot config = featureContext.Get<IConfigurationRoot>();
                    serviceCollection.AddOperationsControlApiWithOpenApiActionResultHosting(
                        config.GetSection("ExternalServices"));
                });
        }
    }
}