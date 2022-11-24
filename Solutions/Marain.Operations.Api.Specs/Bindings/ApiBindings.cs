// <copyright file="ApiBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Tracing;

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
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.ControlHost.Functions",
                    ControlApiPort,
                    "net6.0",
                    configuration: functionConfiguration),
                functionsController.StartFunctionsInstance(
                    "Marain.Operations.StatusHost.Functions",
                    StatusApiPort,
                    "net6.0",
                    configuration: functionConfiguration));
        }

        [AfterScenario]
        public static void WriteOutput(FeatureContext featureContext)
        {
            ////ILogger logger = featureContext.Get<ILogger>();
            ////ILogger logger = new TraceListenerLogger(featureContext.Get<ITraceListener>());
            ILogger logger = new SynchronousLogger();
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

        /// <summary>
        /// The ConsoleLogger dumps everything to a queue to avoid blocking callers, and processes the results
        /// on a separate thread, but because of how SpecFlow (or possibly NUnit) captures console output, the
        /// effect is that we lose all such output.
        /// </summary>
        private class SynchronousLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return new Scope();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                ////this.traceListener.WriteTestOutput($"{logLevel}: {formatter?.Invoke(state, exception) ?? "(no formatter)"}");
                Console.WriteLine($"{logLevel}: {formatter?.Invoke(state, exception) ?? "(no formatter)"}");
            }

            private class Scope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}