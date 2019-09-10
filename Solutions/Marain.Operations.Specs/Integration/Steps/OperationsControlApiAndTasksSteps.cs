// <copyright file="OperationsControlApiAndTasksSteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.OpenApi;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsControlApiAndTasksSteps
    {
        private readonly OperationsControlOpenApiService service;
        private readonly ScenarioContext scenarioContext;

        public OperationsControlApiAndTasksSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.service = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<OperationsControlOpenApiService>();
            this.scenarioContext = scenarioContext;
        }

        [When(@"I call OperationsControlOpenApiService\.CreateOperation with id '(.*)'")]
        public async Task WhenICallOperationsControlOpenApiService_CreateOperationWithId(Guid operationId)
        {
            OpenApiResult result = await this.service.CreateOperation(RootTenant.RootTenantId, operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationFailed with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationFailedWithId(Guid operationId)
        {
            OpenApiResult result = await this.service.SetOperationFailed(RootTenant.RootTenantId, operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationRunning with id '(.*)' and percentComplete of (.*)")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationRunningWithIdAndPercentCompleteOf(
            Guid operationId, int percentComplete)
        {
            OpenApiResult result = await this.service.SetOperationRunning(
                RootTenant.RootTenantId,
                operationId,
                percentComplete).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationSucceeded with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationSucceededWithIdAsync(Guid operationId)
        {
            OpenApiResult result = await this.service.SetOperationSucceeded(RootTenant.RootTenantId, operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }
    }
}
