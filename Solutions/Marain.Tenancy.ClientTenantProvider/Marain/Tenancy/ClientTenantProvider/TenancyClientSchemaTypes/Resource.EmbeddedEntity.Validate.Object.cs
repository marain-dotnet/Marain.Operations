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
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Corvus.Json;
using Corvus.Json.Internal;

namespace Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes;
public readonly partial struct Resource
{
    /// <summary>
    /// A type generated from a JsonSchema specification.
    /// </summary>
    public readonly partial struct EmbeddedEntity
    {
        private ValidationContext ValidateObject(JsonValueKind valueKind, in ValidationContext validationContext, ValidationLevel level)
        {
            ValidationContext result = validationContext;
            if (valueKind != JsonValueKind.Object)
            {
                return result;
            }

            int propertyCount = 0;
            foreach (JsonObjectProperty property in this.EnumerateObject())
            {
                if (!result.HasEvaluatedLocalProperty(propertyCount))
                {
                    result = property.ValueAs<Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity>().Validate(result, level);
                    if (level == ValidationLevel.Flag && !result.IsValid)
                    {
                        return result;
                    }

                    result = result.WithLocalProperty(propertyCount);
                }

                propertyCount++;
            }

            return result;
        }
    }
}