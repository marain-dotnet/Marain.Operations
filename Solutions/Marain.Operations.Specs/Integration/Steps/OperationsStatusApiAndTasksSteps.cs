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
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsStatusApiAndTasksSteps
    {
        private readonly IServiceProvider serviceProvider;
        private readonly FakeOperationsRepository repository;
        private readonly ITenantProvider tenantProvider;
        private readonly ScenarioContext scenarioContext;

        public OperationsStatusApiAndTasksSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            this.repository = this.serviceProvider.GetRequiredService<FakeOperationsRepository>();
            this.tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            this.OperationsTenant = this.tenantProvider.Root;
            this.TenantId = this.OperationsTenant.Id;
            this.scenarioContext = scenarioContext;
        }

        private ITenant OperationsTenant { get; }

        private string TenantId { get; }

        [Given("There is an operation in the store with id '(.*)' and a status of '(.*)'")]
        public async Task GivenThereIsAnOperationInTheStoreWithIdAndAStatusOf(Guid operationId, string status)
        {
            var op = new Operation
            {
                CreatedDateTime = DateTimeOffset.UtcNow,
                Id = operationId,
                LastActionDateTime = DateTimeOffset.UtcNow,
                Status = (OperationStatus)Enum.Parse(typeof(OperationStatus), status),
                TenantId = TenantId,
            };

            await this.repository.PersistAsync(this.OperationsTenant, op).ConfigureAwait(false);
        }

        [Given("There is a running operation in the store with id '(.*)' and a percentComplete of (.*)")]
        public async Task GivenThereIsAnOperationInTheStoreWithIdAndAStatusOfAndAPercentCompleteOf(Guid operationId, int percentComplete)
        {
            var op = new Operation
            {
                CreatedDateTime = DateTimeOffset.UtcNow,
                Id = operationId,
                LastActionDateTime = DateTimeOffset.UtcNow,
                Status = OperationStatus.Running,
                PercentComplete = percentComplete,
                TenantId = TenantId,
            };

            await this.repository.PersistAsync(this.OperationsTenant, op).ConfigureAwait(false);
        }

        [When(@"I call OperationsStatusOpenApiService\.GetOperationById with id '(.*)'")]
        public async Task WhenICallOperationsStatusOpenApiService_GetOperationByIdWithId(Guid operationId)
        {
            OperationsStatusOpenApiService service = this.serviceProvider.GetRequiredService<OperationsStatusOpenApiService>();

            OpenApiResult result = await service.GetOperationById(RootTenant.RootTenantId, operationId).ConfigureAwait(false);

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
