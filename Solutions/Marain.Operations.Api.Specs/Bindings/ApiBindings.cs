// <copyright file="ApiBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    [Binding]
    public static class ApiBindings
    {
        public const int StatusApiPort = 7077;

        public const int ControlApiPort = 7078;

        public static readonly string StatusApiBaseUrl = $"http://localhost:{StatusApiPort}";

        public static readonly string ControlApiBaseUrl = $"http://localhost:{ControlApiPort}";

        [BeforeScenario(Order = BindingSequence.FunctionStartup)]
        public static Task StartApis(FeatureContext featureContext)
        {
            FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
            FunctionConfiguration functionConfiguration = FunctionsBindings.GetFunctionConfiguration(featureContext);

            return Task.WhenAll(
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.ControlHost.Functions",
                    ControlApiPort,
                    "netcoreapp3.1",
                    configuration: functionConfiguration),
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.StatusHost.Functions",
                    StatusApiPort,
                    "netcoreapp3.1",
                    configuration: functionConfiguration));
        }

        [AfterScenario]
        public static void WriteOutput(FeatureContext featureContext)
        {
            ILogger<FunctionsController> logger = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ILogger<FunctionsController>>();
            FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
            logger.LogAllAndClear(functionsController.GetFunctionsOutput());
        }

        [AfterFeature]
        public static void StopApis(FeatureContext featureContext)
        {
            featureContext.RunAndStoreExceptions(
                () =>
                {
                    FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
                    functionsController.TeardownFunctions();
                });
        }
    }
}
