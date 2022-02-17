// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.ControlHost.Startup))]

namespace Marain.Operations.ControlHost
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <inheritdoc/>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IServiceCollection services = builder.Services;
            IConfiguration config = builder.GetContext().Configuration;

            services.AddApplicationInsightsInstrumentationTelemetry();
            services.AddLogging();

            // TODO: we really shouldn't need this. Menes.Hosting turns out to have a dependency on
            // IConfigurationRoot. Yuck.
            services.AddSingleton((IConfigurationRoot)config);

            string azureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"];
            services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
            services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

            services.AddTenantedOperationsControlApiWithOpenApiActionResultHosting(config => config.Documents.AddSwaggerEndpoint());
        }
    }
}