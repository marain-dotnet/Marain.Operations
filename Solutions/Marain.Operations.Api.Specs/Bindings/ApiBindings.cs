// <copyright file="ApiBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings;

using System.Threading.Tasks;

using Corvus.Testing.AzureFunctions;
using Corvus.Testing.AzureFunctions.ReqnRoll;
using Corvus.Testing.SpecFlow;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Reqnroll;

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
        functionConfiguration.CopyToEnvironmentVariables(configuration.AsEnumerable());
        functionConfiguration.EnvironmentVariables.Add("ExternalServices:OperationsStatus", StatusApiBaseUrl);

        await Task.WhenAll(
            functionsController.StartFunctionsInstanceAsync(
                "Marain.Operations.ControlHost.Functions",
                ControlApiPort,
                "net6.0",
                configuration: functionConfiguration),
            functionsController.StartFunctionsInstanceAsync(
                "Marain.Operations.StatusHost.Functions",
                StatusApiPort,
                "net6.0",
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
    public static async Task StopApis(FeatureContext featureContext)
    {
        await featureContext.RunAndStoreExceptionsAsync(
            async () =>
            {
                FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
                await functionsController.TeardownFunctionsAsync();
            });
    }
}