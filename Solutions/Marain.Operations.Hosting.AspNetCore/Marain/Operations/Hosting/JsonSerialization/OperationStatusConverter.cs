// <copyright file="OperationStatusConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Hosting.JsonSerialization;

using System;

using Marain.Operations.Domain;

using Newtonsoft.Json.Converters;

/// <summary>
/// Provides non-camel-cased conversion for <see cref="OperationStatus"/>.
/// </summary>
/// <remarks>
/// <para>
/// We mostly use camel casing for JSON serialization. However, historically the operation
/// status has been reported with Pascal casing. This converter provides that without needing
/// us to add any Json.NET-specific annotations in the Marain.Operations.Abstractions library.
/// </para>
/// </remarks>
internal class OperationStatusConverter : StringEnumConverter
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => objectType == typeof(OperationStatus);
}