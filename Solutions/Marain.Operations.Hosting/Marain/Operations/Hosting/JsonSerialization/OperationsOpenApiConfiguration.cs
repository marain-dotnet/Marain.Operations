// <copyright file="OperationsOpenApiConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Hosting.JsonSerialization;

using System;
using System.Text.Json.Serialization;

using Corvus.Extensions;

using Menes;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Menes.
/// </summary>
public static class OperationsOpenApiConfiguration
{
    /// <summary>
    /// Add JSON converters required for working with representations of Operations to Menes.
    /// </summary>
    /// <param name="openApiConfig">The Menes configuration.</param>
    public static void AddJsonConverters(IOpenApiConfiguration openApiConfig)
    {
        // Menes uses its own private serializer configuration - it won't use whatever we've
        // registered in DI because it doesn't know whether those settings are suitable for its
        // use.
        // So it keeps settings in IOpenApiConfiguration.SerializerOptions. This is used for all
        // Menes converters, and also for the HalDocumentFactory. That affects the tenancy service
        // because it uses HAL documents in its mappers.
        ServiceCollection serializerServices = new();
        serializerServices.AddOperationsDocumentJsonConverters();
        IServiceProvider serializerSp = serializerServices.BuildServiceProvider();

        openApiConfig.SerializerOptions.Converters.AddRange(serializerSp.GetServices<JsonConverter>());
    }
}