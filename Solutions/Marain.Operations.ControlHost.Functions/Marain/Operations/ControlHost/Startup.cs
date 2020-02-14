// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.ControlHost.Startup))]

namespace Marain.Operations.ControlHost
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

            // Workaround for https://github.com/menes-dotnet/Menes/issues/34
            // Menes currently dependson IConfigurationRoot being available through DI for external service URL
            // resolution to work. Azure Functions does not make it directly available - it only registers
            // IConfiguration. But since the object in question happens to implement IConfigurationRoot too,
            // we can just reexpose the same object for this service type.
            services.AddSingleton(sp => (IConfigurationRoot)sp.GetRequiredService<IConfiguration>());

            services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenantCloudBlobContainerFactoryOptions").Get<TenantCloudBlobContainerFactoryOptions>());

            services.AddTenantedOperationsControlApi(config => config.Documents.AddSwaggerEndpoint());
        }
    }
}
