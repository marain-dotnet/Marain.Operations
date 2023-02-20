// <copyright file="OperationStatusConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Hosting.JsonSerialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Marain.Operations.Domain;

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
    internal class OperationStatusConverter : JsonConverter<OperationStatus>
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType) => objectType == typeof(OperationStatus);

        /// <inheritdoc/>
        public override OperationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected string value for OperationStatus.");
            }

            string? value = reader.GetString();
            if (!Enum.TryParse(value, out OperationStatus result))
            {
                throw new JsonException($"Unable to parse '{value}' as OperationStatus.");
            }

            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, OperationStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}