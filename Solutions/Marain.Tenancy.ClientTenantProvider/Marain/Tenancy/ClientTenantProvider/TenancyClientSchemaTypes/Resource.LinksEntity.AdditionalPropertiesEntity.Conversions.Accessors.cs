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
    public readonly partial struct LinksEntity
    {
        /// <summary>
        /// A type generated from a JsonSchema specification.
        /// </summary>
        public readonly partial struct AdditionalPropertiesEntity
        {
            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link"/>.
            /// </summary>
            public Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link AsLink
            {
                get
                {
                    return (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link)this;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a valid <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link"/>.
            /// </summary>
            public bool IsLink
            {
                get
                {
                    return ((Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link)this).IsValid();
                }
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link"/>.
            /// </summary>
            /// <param name = "result">The result of the conversion.</param>
            /// <returns><c>True</c> if the conversion was valid.</returns>
            public bool TryGetAsLink(out Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link result)
            {
                result = (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Link)this;
                return result.IsValid();
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray"/>.
            /// </summary>
            public Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray AsLinkArray
            {
                get
                {
                    return (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray)this;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a valid <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray"/>.
            /// </summary>
            public bool IsLinkArray
            {
                get
                {
                    return ((Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray)this).IsValid();
                }
            }

            /// <summary>
            /// Gets the value as a <see cref = "Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray"/>.
            /// </summary>
            /// <param name = "result">The result of the conversion.</param>
            /// <returns><c>True</c> if the conversion was valid.</returns>
            public bool TryGetAsLinkArray(out Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray result)
            {
                result = (Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes.Resource.LinksEntity.AdditionalPropertiesEntity.LinkArray)this;
                return result.IsValid();
            }
        }
    }
}