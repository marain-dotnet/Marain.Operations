// <copyright file="CommonOperationsApiAndTaskSteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class CommonOperationsApiAndTaskSteps
    {
        private readonly FakeOperationsRepository repository;
        private readonly ITenantProvider tenantProvider;
        private readonly ScenarioContext scenarioContext;

        public CommonOperationsApiAndTaskSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.repository = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<FakeOperationsRepository>();
            this.tenantProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantProvider>();
            this.scenarioContext = scenarioContext;
        }

        [Given("There is no operation in the store with id '(.*)'")]
        public async Task GivenThereIsNoOperationInTheStoreWithId(Guid operationId)
        {
            Operation? op = await this.repository.GetAsync(this.tenantProvider.Root, operationId).ConfigureAwait(false);

            Assert.IsNull(op);
        }

        [Then("the status of the operation in the store with id '(.*)' should be '(.*)'")]
        public async Task ThenTheStatusOfTheOperationInTheStoreWithIdShouldBe(Guid operationId, string statusText)
        {
            var expectedStatus = (OperationStatus)Enum.Parse(typeof(OperationStatus), statusText);

            Operation? op = await this.repository.GetAsync(this.tenantProvider.Root, operationId).ConfigureAwait(false);

            Assert.AreEqual(expectedStatus, op?.Status);
        }

        [Then("the percentComplete of the operation in the store with id '(.*)' should be (.*)")]
        public async Task ThenThePercentCompleteOfTheOperationInTheStoreWithIdShouldBeAsync(
            Guid operationId,
            int percentComplete)
        {
            Operation? op = await this.repository.GetAsync(this.tenantProvider.Root, operationId).ConfigureAwait(false);

            Assert.AreEqual(percentComplete, op?.PercentComplete);
        }

        [Then("the result status should be (.*)")]
        public void ThenTheResultStatusShouldBe(int statusCode)
        {
            Assert.AreEqual(statusCode, this.scenarioContext.Get<OpenApiResult>().StatusCode);
        }

        [Then("the '(.*)' property in the result should be '(.*)'")]
        public void ThenThePropertyInTheResultShouldBe(string propertyName, string propertyValue)
        {
            OpenApiResult result = this.scenarioContext.Get<OpenApiResult>();

            Assert.IsTrue(
                result.Results.TryGetValue(propertyName, out object? value),
                $"Property '{propertyName}' not found in result");
            if (value is string stringValue)
            {
                Assert.AreEqual(propertyValue, stringValue);
            }
            else
            {
                Assert.Fail($"Property '{propertyName}' should be a string, but is of type {value!.GetType().FullName}");
            }
        }
    }
}
