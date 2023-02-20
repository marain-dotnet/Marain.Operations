//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Corvus.Json;

namespace Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes;
public readonly partial struct Resource
{
    public readonly partial struct EmbeddedEntity
    {
        /// <summary>
        /// A type generated from a JsonSchema specification.
        /// </summary>
        public readonly partial struct AdditionalPropertiesEntity
        {
            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource"/>.
            /// </summary>
            public Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource AsResource
            {
                get
                {
                    return (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource)this;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a valid <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource"/>.
            /// </summary>
            public bool IsResource
            {
                get
                {
                    return ((Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource)this).IsValid();
                }
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource"/>.
            /// </summary>
            /// <param name = "result">The result of the conversion.</param>
            /// <returns><c>True</c> if the conversion was valid.</returns>
            public bool TryGetAsResource(out Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource result)
            {
                result = (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource)this;
                return result.IsValid();
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray"/>.
            /// </summary>
            public Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray AsResourceArray
            {
                get
                {
                    return (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray)this;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a valid <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray"/>.
            /// </summary>
            public bool IsResourceArray
            {
                get
                {
                    return ((Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray)this).IsValid();
                }
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray"/>.
            /// </summary>
            /// <param name = "result">The result of the conversion.</param>
            /// <returns><c>True</c> if the conversion was valid.</returns>
            public bool TryGetAsResourceArray(out Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray result)
            {
                result = (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.EmbeddedEntity.AdditionalPropertiesEntity.ResourceArray)this;
                return result.IsValid();
            }
        }
    }
}