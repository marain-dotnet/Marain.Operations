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
    public readonly partial struct LinksEntity
    {
        public readonly partial struct AdditionalPropertiesEntity
        {
            /// <summary>
            /// A type generated from a JsonSchema specification.
            /// </summary>
            public readonly partial struct LinkArray
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
                            if (level >= ValidationLevel.Detailed)
                            {
                                result = result.WithResult(isValid: false, $"9.3.2.3. additionalProperties - additional property \"{property.Name}\" is not permitted.");
                            }
                            else if (level >= ValidationLevel.Basic)
                            {
                                result = result.WithResult(isValid: false, "9.3.2.3. additionalProperties - additional properties are not permitted.");
                            }
                            else
                            {
                                return result.WithResult(isValid: false);
                            }
                        }

                        propertyCount++;
                    }

                    return result;
                }
            }
        }
    }
}