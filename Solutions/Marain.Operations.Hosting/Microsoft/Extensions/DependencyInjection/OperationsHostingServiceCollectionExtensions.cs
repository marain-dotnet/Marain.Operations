// <copyright file="OperationsHostingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

using Marain.Operations.Domain;
using Marain.Operations.Hosting.JsonSerialization;
using Marain.Operations.OpenApi;
using Marain.Operations.Tasks;

using Menes;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Extension methods for configuring non-hosting-technology-specific aspects of DI for the
/// Operations Open API services.
/// </summary>
public static class OperationsHostingServiceCollectionExtensions
{
    /// <summary>
    /// Add services required when hosting the Operations Control API regardless of the hosting
    /// technology used.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="statusExternalServiceConfigurationSection">
    /// The configuration section containing the <c>OperationsStatus</c> setting holding the
    /// base URL for the status service. (Conventionally, but not necessarily, a section called
    /// <c>ExternalServices</c> underneath the configuration root.)
    /// </param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsControlApiNonHostingTypeSpecific(
        this IServiceCollection services,
        IConfiguration statusExternalServiceConfigurationSection)
    {
        services.AddLogging();

        services.AddSingleton<OperationsControlOpenApiService>();
        services.AddSingleton<IOpenApiService, OperationsControlOpenApiService>(s => s.GetRequiredService<OperationsControlOpenApiService>());
        services.AddTransient<IOperationsControlTasks, OperationsControlTasks>();

        services.AddExternalServicesForOperationsControlApi(statusExternalServiceConfigurationSection);
        services.AddMarainTenancyServices();

        return services;
    }

    /// <summary>
    /// Add services required when hosting the Operations Status API regardless of the hosting
    /// technology used.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsStatusApiNonHostingTypeSpecific(
        this IServiceCollection services)
    {
        services.AddLogging();

        services.AddSingleton<OperationsStatusOpenApiService>();
        services.AddSingleton<IOpenApiService, OperationsStatusOpenApiService>(s => s.GetRequiredService<OperationsStatusOpenApiService>());

        services.AddTransient<IOperationsStatusTasks, OperationsStatusTasks>();
        services.AddMarainTenancyServices();

        return services;
    }

    /// <summary>
    /// Add JSON serialization services required when serializing or deserializing representations
    /// of Operations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    public static IServiceCollection AddOperationsDocumentJsonConverters(this IServiceCollection services)
    {
        // The Open API specification for the status service indicates that the createdDateTime and
        // lastActionDateTime properties will both use our slightly odd date time representation
        // which uses a JSON object with a dateTimeOffset property formatted as a date-time and a
        // unixTime property formatted as an int64. This format originated to support sortability
        // in Cosmos DB, early versions of which couldn't sort directly on a string-formatted
        // timestamp. This is now essentially a deprecated format in the world of Corvus and
        // Marain, but because the Operations Status API has always used this format at its public
        // web API, we need to continue to use it. (It will also be used internally in blob stores
        // for Operations services that had previously been built up using older versions of
        // Marain.Operations.) So we include the DateTimeOffset converter that uses this format.
        // The System.Text.Json flavour of Corvus JSON serialization support does not add this
        // automatically, because new projects generally won't want it.
        return services
            .AddJsonSerializerOptionsProvider()
            .AddJsonDateTimeOffsetToIso8601AndUnixTimeConverter()
            .AddPascalCaseConverterForEnums(typeof(OperationStatus));
    }

    /// <summary>
    /// Adds services required for resolving URLs to external services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">
    /// The configuration section from which base URL settings will be read.
    /// </param>
    /// <returns>The service collection, to enable chaining.</returns>
    internal static IServiceCollection AddExternalServicesForOperationsControlApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddExternalServices(
            configuration,
            externalServices => externalServices.AddExternalServiceWithEmbeddedDefinition<OperationsStatusOpenApiService>("OperationsStatus"));

        return services;
    }

    /// <summary>
    /// Adds services required to access tenant information.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, to enable chaining.</returns>
    internal static IServiceCollection AddMarainTenancyServices(this IServiceCollection services)
    {
        // TODO: Work out exactly why it's necessary to call the methods in this order. Switching the order
        // results in an attempt to register the Tenant content type with the ContentFactory twice, but it wasn't
        // obvious from an initial scan through exactly why this is.
        ////services.AddTenantProviderServiceClient(true);
        services.AddMarainServiceConfiguration();
        services.AddSingleton<JsonConverter>(new OperationStatusConverter());

        services.AddMarainServicesTenancy();

        return services;
    }
}