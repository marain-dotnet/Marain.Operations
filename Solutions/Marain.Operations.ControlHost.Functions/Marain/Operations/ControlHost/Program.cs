// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Marain.Operations.ControlHost;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(
    (HostBuilderContext context, IFunctionsWorkerApplicationBuilder builder) =>
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = context.Configuration;

        Startup.Configure(services, configuration);
    })
    .Build();

host.Run();