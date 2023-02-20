//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System.Collections.Immutable;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes;
/// <summary>
/// A type generated from a JsonSchema specification.
/// </summary>
public readonly partial struct ChildTenants
{
    /// <summary>
    /// Conversion from <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource"/>.
    /// </summary>
    /// <param name = "value">The value from which to convert.</param>
    public static implicit operator ChildTenants(Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return value.ValueKind switch
        {
            JsonValueKind.Object => new((ImmutableDictionary<JsonPropertyName, JsonAny>)value),
            _ => Undefined
        };
    }

    /// <summary>
    /// Conversion to <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource"/>.
    /// </summary>
    /// <param name = "value">The value from which to convert.</param>
    public static implicit operator Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource(ChildTenants value)
    {
        if ((value.backing & Backing.JsonElement) != 0)
        {
            return new(value.AsJsonElement);
        }

        if ((value.backing & Backing.Object) != 0)
        {
            return new(value.objectBacking);
        }

        return Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.Undefined;
    }
}