// <copyright file="OperationsStatusApiAndTasksSteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable CS1591 // Elements should be documented
#pragma warning disable SA1600 // Elements should be documented

namespace Marain.Operations.Specs.Integration.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Marain.Operations.OpenApi;
    using Marain.TenantManagement.Testing;
    using Menes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsStatusApiAndTasksSteps
    {
        private readonly IServiceProvider serviceProvider;
        private readonly FakeOperationsRepository repository;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "It can be useful when debugging tests to be able to see the host. It's also important that we ask DI for it, to ensure that all normal initialiation has occurred")]
        private readonly IOpenApiHost host;

        private readonly ITenantProvider tenantProvider;
        private readonly ScenarioContext scenarioContext;
        private readonly TransientTenantManager transientTenantManager;

        public OperationsStatusApiAndTasksSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            this.repository = this.serviceProvider.GetRequiredService<FakeOperationsRepository>();
            this.host = this.serviceProvider.GetRequiredService<IOpenApiHost<HttpRequest, IActionResult>>();
            this.tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            this.scenarioContext = scenarioContext;
            this.transientTenantManager = TransientTenantManager.GetInstance(featureContext);
        }

        [Given("There is an operation in the store with id '(.*)' and a status of '(.*)'")]
        public async Task GivenThereIsAnOperationInTheStoreWithIdAndAStatusOf(Guid operationId, string status)
        {
            var op = new Operation(
                operationId,
                createdDateTime: DateTimeOffset.UtcNow,
                lastActionDateTime: DateTimeOffset.UtcNow,
                Enum.Parse<OperationStatus>(status),
                this.transientTenantManager.PrimaryTransientClient.Id);

            await this.repository.PersistAsync(this.transientTenantManager.PrimaryTransientClient, op).ConfigureAwait(false);
        }

        [Given("There is a running operation in the store with id '(.*)' and a percentComplete of (.*)")]
        public async Task GivenThereIsAnOperationInTheStoreWithIdAndAStatusOfAndAPercentCompleteOf(Guid operationId, int percentComplete)
        {
            var op = new Operation(
                operationId,
                createdDateTime: DateTimeOffset.UtcNow,
                lastActionDateTime: DateTimeOffset.UtcNow,
                OperationStatus.Running,
                this.transientTenantManager.PrimaryTransientClient.Id)
            {
                PercentComplete = percentComplete,
            };

            await this.repository.PersistAsync(this.transientTenantManager.PrimaryTransientClient, op).ConfigureAwait(false);
        }

        [When(@"I call OperationsStatusOpenApiService\.GetOperationById with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_GetOperationByIdWithId(Guid operationId)
        {
            OperationsStatusOpenApiService service = this.serviceProvider.GetRequiredService<OperationsStatusOpenApiService>();

            OpenApiResult result = await service.GetOperationById(this.transientTenantManager.PrimaryTransientClient.Id, operationId).ConfigureAwait(false);

            this.scenarioContext.Set(result);
        }

        [Then("the operation status in the result should be '(.*)'")]
        public void ThenTheOperationStatusInTheResultShouldBe(string statusText)
        {
            var expectedStatus = (OperationStatus)Enum.Parse(typeof(OperationStatus), statusText);

            var result = (Operation)this.scenarioContext.Get<OpenApiResult>().Results["application/json"];
            Assert.AreEqual(expectedStatus, result.Status);
        }

        [Then("the percentComplete in the result should be (.*)")]
        public void ThenThePercentCompleteInTheResultShouldBe(int percentComplete)
        {
            var result = (Operation)this.scenarioContext.Get<OpenApiResult>().Results["application/json"];
            Assert.AreEqual(percentComplete, result.PercentComplete);
        }
    }
}
