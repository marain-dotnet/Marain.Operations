// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.StatusHost.Startup))]

namespace Marain.Operations.StatusHost
{
    using Corvus.Azure.Storage.Tenancy;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddLogging(logging =>
            {
#if DEBUG
                // Ensure you enable the required logging level in host.json
                // e.g:
                //
                // "logging": {
                //    "fileLoggingMode": "debugOnly",
                //    "logLevel": {
                //
                //    // For all functions
                //    "Function": "Debug",
                //
                //    // Default settings, e.g. for host
                //    "default": "Debug"
                // }
                logging.AddConsole();
#endif
            });

            services.AddSingleton(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                return new TenantCloudBlobContainerFactoryOptions
                {
                    AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
                };
            });
            services.AddMarainServiceConfiguration();

            services.AddTenantedOperationsStatusApi(config => config.Documents.AddSwaggerEndpoint());
        }
    }
}
