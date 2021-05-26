// <copyright file="OperationsUsingClientSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Steps
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Testing.SpecFlow;
    using Marain.Operations.Api.Specs.Bindings;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Marain.Operations.Client.OperationsStatus;
    using Marain.Operations.Client.OperationsStatus.Models;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsUsingClientSteps : Steps
    {
        public const string LastResponseContextKey = "LastResponse";

        [When("I use the operations status client to get the operation with the Id called '(.*)'")]
        public Task WhenIUseTheOperationsStatusClientToGetTheOperationWithTheIdCalled(string operationIdName)
        {
            Guid operationId = this.ScenarioContext.Get<Guid>(operationIdName);

            return this.WhenIUseTheOperationsStatusClientToGetTheOperationWithId(operationId);
        }

        [When("I use the operations status client to get the operation with Id '(.*)'")]
        public Task WhenIUseTheOperationsStatusClientToGetTheOperationWithId(Guid id)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperations client = serviceProvider.GetRequiredService<IMarainOperations>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    Client.OperationsStatus.Models.Operation? operation = await client.GetOperationByIdAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        id).ConfigureAwait(false);

                    this.ScenarioContext.Set(operation, LastResponseContextKey);
                });
        }

        [Given("I generate a new operation Id and call it '(.*)'")]
        public void GivenIGenerateANewOperationIdAndCallIt(string newOperationIdName)
        {
            this.ScenarioContext.Set(Guid.NewGuid(), newOperationIdName);
        }

        [Given("I use the operations control client to create an operation with the Id called '(.*)'")]
        [When("I use the operations control client to create an operation with the Id called '(.*)'")]
        public Task WhenIUseTheOperationsControlClientToCreateAnOperationWithTheIdCalled(string operationIdName)
        {
            Guid operationId = this.ScenarioContext.Get<Guid>(operationIdName);

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperationsControl client = serviceProvider.GetRequiredService<IMarainOperationsControl>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    CreateOperationHeaders result = await client.CreateOperationAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        operationId).ConfigureAwait(false);

                    this.ScenarioContext.Set(result);
                });
        }

        [Given("I use the operations control client to create an operation")]
        [When("I use the operations control client to create an operation")]
        public Task WhenIUseTheOperationsControlClientToCreateAnOperation(Table table)
        {
            string idName = table.Rows[0]["IdName"];
            Guid id = this.ScenarioContext.Get<Guid>(idName);
            string resourceLocation = table.Rows[0]["ResourceLocation"];
            long expireAfter = long.Parse(table.Rows[0]["ExpireAfter"]);
            string body = table.Rows[0]["Body"];

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperationsControl client = serviceProvider.GetRequiredService<IMarainOperationsControl>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    CreateOperationHeaders result = await client.CreateOperationAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        id,
                        resourceLocation,
                        expireAfter,
                        body).ConfigureAwait(false);

                    this.ScenarioContext.Set(result);
                });
        }

        [Given("I use the operations control client to set the status of the operation with Id called '(.*)' to Running")]
        public Task GivenIUseTheOperationsControlClientToSetTheStatusOfTheOperationWithIdCalledToRunning(string operationIdName)
        {
            return this.GivenIUseTheOperationsControlClientToSetTheStatusOfTheOperationWithIdCalledToRunningAndThePercentageCompleteTo(operationIdName, null);
        }

        [Given("I use the operations control client to set the status of the operation with Id called '(.*)' to Running and the percentage complete to (.*)")]
        public Task GivenIUseTheOperationsControlClientToSetTheStatusOfTheOperationWithIdCalledToRunningAndThePercentageCompleteTo(string operationIdName, int? percentComplete)
        {
            Guid id = this.ScenarioContext.Get<Guid>(operationIdName);

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperationsControl client = serviceProvider.GetRequiredService<IMarainOperationsControl>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    ProblemDetails? result = await client.SetOperationRunningAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        id,
                        percentComplete).ConfigureAwait(false);

                    this.ScenarioContext.Set(result);
                });
        }

        [Given("I use the operations control client to set the status of the operation with Id called '(.*)' to Succeeded")]
        public Task GivenIUseTheOperationsControlClientToSetTheStatusOfTheOperationWithIdCalledToSucceeded(string operationIdName)
        {
            Guid id = this.ScenarioContext.Get<Guid>(operationIdName);

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperationsControl client = serviceProvider.GetRequiredService<IMarainOperationsControl>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    ProblemDetails? result = await client.SetOperationSucceededAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        id).ConfigureAwait(false);

                    this.ScenarioContext.Set(result);
                });
        }

        [Given("I use the operations control client to set the status of the operation with Id called '(.*)' to Failed")]
        public Task GivenIUseTheOperationsControlClientToSetTheStatusOfTheOperationWithIdCalledToFailed(string operationIdName)
        {
            Guid id = this.ScenarioContext.Get<Guid>(operationIdName);

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperationsControl client = serviceProvider.GetRequiredService<IMarainOperationsControl>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            return Exceptions.ExecuteAndStoreExceptionAsync(
                this.ScenarioContext,
                async () =>
                {
                    ProblemDetails? result = await client.SetOperationFailedAsync(
                        transientTenantManager.PrimaryTransientClient.Id,
                        id).ConfigureAwait(false);

                    this.ScenarioContext.Set(result);
                });
        }

        [Given("the request succeeds")]
        [When("the request succeeds")]
        [Then("the request succeeds")]
        public void ThenTheRequestSucceeds()
        {
            Exceptions.AssertNoLastException(this.ScenarioContext);
        }

        [Then("the create operation response contains the location in the operations status Api for the operation with the Id called '(.*)'")]
        public void ThenTheCreateOperationResponseContainsTheLocationInTheOperationsStatusApiForTheOperationWithTheIdCalled(string operationIdName)
        {
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);
            Guid operationId = this.ScenarioContext.Get<Guid>(operationIdName);

            CreateOperationHeaders response = this.ScenarioContext.Get<CreateOperationHeaders>();
            var actualUri = new Uri(response.Location);
            var expectedUri = new Uri(new Uri(ApiBindings.StatusApiBaseUrl), $"/{transientTenantManager.PrimaryTransientClient.Id}/api/operations/{operationId}");

            Assert.AreEqual(expectedUri, actualUri);
        }

        [Then("the get operation response is null")]
        public void ThenTheGetOperationResponseIsNull()
        {
            Client.OperationsStatus.Models.Operation? response =
                this.ScenarioContext.Get<Client.OperationsStatus.Models.Operation?>(LastResponseContextKey);

            Assert.IsNull(response);
        }

        [Then("the retrieved operation has the status '(.*)'")]
        public void ThenTheRetrievedOperationHasTheStatus(string expectedStatus)
        {
            Client.OperationsStatus.Models.Operation result =
                this.ScenarioContext.Get<Client.OperationsStatus.Models.Operation>(LastResponseContextKey);

            Assert.AreEqual(expectedStatus, result.Status);
        }

        [Then("the retrieved operation has the resource location '(.*)'")]
        public void ThenTheRetrievedOperationHasTheResourceLocation(string expectedResourceLocation)
        {
            Client.OperationsStatus.Models.Operation result =
                this.ScenarioContext.Get<Client.OperationsStatus.Models.Operation>(LastResponseContextKey);

            Assert.AreEqual(expectedResourceLocation, result.ResourceLocation);
        }

        [Then("the retrieved operation is (.*) percent complete")]
        public void ThenTheRetrievedOperationIsPercentComplete(int expectedPercentComplete)
        {
            Client.OperationsStatus.Models.Operation result =
                this.ScenarioContext.Get<Client.OperationsStatus.Models.Operation>(LastResponseContextKey);

            Assert.AreEqual(expectedPercentComplete, result.PercentComplete);
        }

        [Then("the retrieved operation has percent complete set to null")]
        public void ThenTheRetrievedOperationHasPercentCompleteSetToNull()
        {
            Client.OperationsStatus.Models.Operation result =
                this.ScenarioContext.Get<Client.OperationsStatus.Models.Operation>(LastResponseContextKey);

            Assert.IsNull(result.PercentComplete);
        }

        [Then("an exception of type '(.*)' is thrown")]
        public void ThenAnExceptionOfTypeHttpOperationExceptionIsThrown(string expectedExceptionTypeName)
        {
            Exceptions.AssertLastExceptionWasOfType(this.ScenarioContext, expectedExceptionTypeName);
        }

        [Then(@"an exception of type HttpOperationException with a '(.*)' status is thrown")]
        public void ThenAnExceptionOfTypeWithAStatusCodeIsThrown(HttpStatusCode expectedStatus)
        {
            Exceptions.AssertLastExceptionWasOfType(this.ScenarioContext, "HttpOperationException");

            var exception = Exceptions.GetLastException(this.ScenarioContext) as HttpOperationException;
            Assert.AreEqual(expectedStatus, exception!.Response.StatusCode);
        }
    }
}
