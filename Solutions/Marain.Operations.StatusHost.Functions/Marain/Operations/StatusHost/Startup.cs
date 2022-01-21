// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.StatusHost.Startup))]

namespace Marain.Operations.StatusHost
{
    using Corvus.Azure.Storage.Tenancy;

    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

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

            string azureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"];
            services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
            services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

            ////services.AddSingleton(sp =>
            ////{
            ////    IConfiguration config = sp.GetRequiredService<IConfiguration>();
            ////    return new TenantCloudBlobContainerFactoryOptions
            ////    {
            ////        AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
            ////    };
            ////});
            ////services.AddMarainServiceConfiguration();

            services.AddTenantedOperationsStatusApiWithOpenApiActionResultHosting(config => config.Documents.AddSwaggerEndpoint());
        }
    }
}
