// <copyright file="ApiBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.Configuration;
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

        [BeforeFeature(Order = BindingSequence.FunctionStartup)]
        public static async Task StartApis(FeatureContext featureContext)
        {
            FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
            FunctionConfiguration functionConfiguration = FunctionsBindings.GetFunctionConfiguration(featureContext);

            IConfiguration configuration = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IConfiguration>();

            // TODO: do we need to update Corvus.Testing.AzureFunctions to accept IEnumerable<KeyValuePair<string, string?>
            // since that's what the nullable-aware version of Microsoft.Extensions.Configuration seems to have opted for?
            functionConfiguration.CopyToEnvironmentVariables(
                configuration
                .AsEnumerable()
                .Cast<KeyValuePair<string, string>>());
            functionConfiguration.EnvironmentVariables.Add("ExternalServices:OperationsStatus", StatusApiBaseUrl);

            await Task.WhenAll(
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.ControlHost.Functions",
                    ControlApiPort,
                    "net7.0",
                    "csharp",
                    configuration: functionConfiguration),
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.StatusHost.Functions",
                    StatusApiPort,
                    "net7.0",
                    "csharp",
                    configuration: functionConfiguration));
        }

        [AfterScenario]
        public static void WriteOutput(FeatureContext featureContext)
        {
            ILogger logger = featureContext.Get<ILogger>();
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