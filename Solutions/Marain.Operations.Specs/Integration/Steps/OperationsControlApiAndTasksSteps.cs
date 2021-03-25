// <copyright file="OperationsControlApiAndTasksSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Testing.SpecFlow;
    using Marain.Operations.OpenApi;
    using Marain.TenantManagement.Testing;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsControlApiAndTasksSteps
    {
        private readonly OperationsControlOpenApiService service;
        private readonly TransientTenantManager transientTenantManager;
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;

        public OperationsControlApiAndTasksSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.service = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<OperationsControlOpenApiService>();
            this.transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [When(@"I call OperationsControlOpenApiService\.CreateOperation with id '(.*)'")]
        public async Task WhenICallOperationsControlOpenApiService_CreateOperationWithId(Guid operationId)
        {
            OpenApiResult result = await this.service.CreateOperation(
                this.transientTenantManager.PrimaryTransientClient.Id,
                operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationFailed with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationFailedWithId(Guid operationId)
        {
            OpenApiResult result = await this.service.SetOperationFailed(
                this.transientTenantManager.PrimaryTransientClient.Id,
                operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationRunning with id '(.*)' and percentComplete of (.*)")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationRunningWithIdAndPercentCompleteOf(
            Guid operationId, int percentComplete)
        {
            OpenApiResult result = await this.service.SetOperationRunning(
                this.transientTenantManager.PrimaryTransientClient.Id,
                operationId,
                percentComplete).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [When(@"I call OperationsStatusOpenApiService\.SetOperationSucceeded with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_SetOperationSucceededWithIdAsync(Guid operationId)
        {
            OpenApiResult result = await this.service.SetOperationSucceeded(
                this.transientTenantManager.PrimaryTransientClient.Id,
                operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }
    }
}
