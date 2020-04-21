// <copyright file="OperationsControlApiAndTasksBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.OpenApi;
    using Marain.Operations.Tasks;
    using Marain.Services;
    using Marain.TenantManagement;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
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
                    serviceCollection.AddOperationsControlApi();
                });
        }

        [BeforeFeature("@operationsControl", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static Task TaskSetupOperationsControlTenants(FeatureContext featureContext)
        {
            return ContainerSetupBindings.SetupTenants(featureContext, "OperationsControlServiceManifest");
        }
    }
}
