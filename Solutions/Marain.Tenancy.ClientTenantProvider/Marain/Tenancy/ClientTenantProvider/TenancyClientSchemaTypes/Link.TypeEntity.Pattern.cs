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
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Corvus.Json;
using Corvus.Json.Internal;

namespace Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes;
public readonly partial struct Link
{
    /// <summary>
    /// A type generated from a JsonSchema specification.
    /// </summary>
    public readonly partial struct TypeEntity
    {
        private static Regex __CorvusPatternExpression => new Regex("^(application|audio|example|image|message|model|multipart|text|video)\\/[a-zA-Z0-9!#\\$&\\.\\+-\\^_]{1,127}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    }
}