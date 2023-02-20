// <copyright file="OperationsAzureStorageRepositoryServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.Configuration;

using System;
using System.Linq;
using System.Text.Json.Serialization;

using Corvus.Json.Serialization;
using Corvus.Storage.Azure.BlobStorage.Tenancy;

using Marain.Operations.Storage;
using Marain.Operations.Storage.Blob;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration of services for using the operations repository implemented on top of the
/// tenancy repository.
/// </summary>
public static class OperationsAzureStorageRepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Enable the tenancy repository based operations repository.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddBlobContainerOperationsRepository(
        this IServiceCollection services)
    {
        if (!services.Any(s => s.ServiceType == typeof(IOperationsRepository)))
        {
            // The repository has its own serialization requirements that should not influence or
            // be influenced by whatever the application is doing. Corvus supplies what we need,
            // but makes it accessible only via DI, so we create a local container just to get
            // the local serializer configuration we need.
            // Note that these happen to be the same as the serializer settings we pass to Menes
            ServiceCollection serializerServices = new();
            serializerServices.AddJsonSerializerOptionsProvider();

            // Historically, we've used the old Corvus DateTimeOffset serialization that writes out
            // a JSON object with both an ISO8601 date-time and a Unix epoch time. This was just
            // because that's what the old Newtonsoft-based Corvus serialization support set up.
            // We continue to use it to ensure that newer versions of Marain.Operations can work
            // with blob stores originally created under older ones.
            serializerServices.AddJsonDateTimeOffsetToIso8601AndUnixTimeConverter();

            // Older versions of Marain.Operations have always written out the status property
            // using PascalCasing. This has the effect of configuring all enumerations to use
            // PascalCasing, which is fine, given that we're only using these settings for the
            // repository.
            serializerServices.AddSingleton<JsonConverter>(new JsonStringEnumConverter());

            IServiceProvider serializerServiceProvider = serializerServices.BuildServiceProvider();

            services.AddSingleton<IOperationsRepository>(sp => new OperationsRepository(
                sp.GetRequiredService<IBlobContainerSourceWithTenantLegacyTransition>(),
                serializerServiceProvider.GetRequiredService<IJsonSerializerOptionsProvider>().Instance));
        }

        return services;
    }
}