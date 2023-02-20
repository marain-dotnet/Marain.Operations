// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.ControlHost;

using System;

using Corvus.Tenancy;

using Marain.Tenancy.ClientTenantProvider;

using Microsoft.ApplicationInsights;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Startup code for the Function.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Di initialization.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration.</param>
    public static void Configure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(new TelemetryClient(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration()));
        services.AddApplicationInsightsInstrumentationTelemetry();
        services.AddLogging();

        // TODO: we really shouldn't need this. Menes.Hosting turns out to have a dependency on
        // IConfigurationRoot. Yuck.
        services.AddSingleton((IConfigurationRoot)configuration);

        string azureServicesAuthConnectionString = configuration["AzureServicesAuthConnectionString"] ?? string.Empty;
        services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);

        // TODO: do we still need this?
        services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

        // TODO: feels like these should go somewhere common...but possibly in the client library?
        TenancyClientOptions tenancyClientOptions = configuration.GetSection("TenancyClient").Get<TenancyClientOptions>()
            ?? throw new InvalidOperationException("TenancyClient settings are missing");
        ////services.AddSingleton(configuration.GetSection("TenancyClient").Get<TenancyClientOptions>()
        ////    ?? throw new InvalidOperationException("TenancyClient settings are missing"));
        services.AddSingleton<ITenantProvider, TenancyClient>();

        services.AddTenantedOperationsControlApiWithIsolatedAzureFunctionsHosting(
            configuration.GetSection("ExternalServices"),
            tenancyClientOptions,
            config => config.Documents.AddSwaggerEndpoint());
    }
}