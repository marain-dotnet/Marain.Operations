// <copyright file="OperationsUsingClientSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Testing.SpecFlow;
    using Marain.Operations.Client.OperationsStatus;
    using Marain.Operations.Client.OperationsStatus.Models;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OperationsUsingClientSteps : Steps
    {
        [When("I use the operations status client to get the operation with Id '(.*)'")]
        public async Task WhenIUseTheOperationsStatusClientToGetTheOperationWithId(Guid id)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.FeatureContext);
            IMarainOperations client = serviceProvider.GetRequiredService<IMarainOperations>();
            var transientTenantManager = TransientTenantManager.GetInstance(this.FeatureContext);

            try
            {
                Operation operation = await client.GetOperationByIdAsync(
                    transientTenantManager.PrimaryTransientClient.Id,
                    id).ConfigureAwait(false);

                this.ScenarioContext.Set(operation);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Then("an exception of type '(.*)' is thrown")]
        public void ThenAnExceptionOfTypeHttpOperationExceptionIsThrown(string expectedExceptionTypeName)
        {
            if (!this.ScenarioContext.TryGetValue(out Exception exception))
            {
                Assert.Fail($"Expected an exception of type '{expectedExceptionTypeName}' to have been thrown, but none was.");
            }

            Assert.AreEqual(expectedExceptionTypeName, exception.GetType().Name);
        }
    }
}
