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

        [Then("then the request succeeds")]
        public void ThenThenTheRequestSucceeds()
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
