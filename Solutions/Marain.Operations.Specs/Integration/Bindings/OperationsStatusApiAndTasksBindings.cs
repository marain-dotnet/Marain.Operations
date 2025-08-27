// <copyright file="OperationsStatusApiAndTasksBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1021 // Use expression-bodied lambda - doesn't improve readability in this file

namespace Marain.Operations.Specs.Integration.Bindings;

using Corvus.Testing.SpecFlow;

using Marain.Operations.OpenApi;
using Marain.Operations.Tasks;

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
                serviceCollection.AddOperationsStatusApiWithOpenApiActionResultHosting();
            });
    }
}