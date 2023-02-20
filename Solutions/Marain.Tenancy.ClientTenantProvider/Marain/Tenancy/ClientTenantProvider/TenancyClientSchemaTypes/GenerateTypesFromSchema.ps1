$SchemaPath = Join-Path -Resolve $PSScriptRoot ..\..\..\..\..\..\..\Marain.Tenancy\Solutions\Marain.Tenancy.OpenApi.Service\Marain\Tenancy\OpenApi\TenancyServices.yaml

$SchemaYaml = Get-Content -Path $SchemaPath | ConvertFrom-Yaml
$SchemaYaml | ConvertTo-Json -Depth 10 | Set-Content TenancyServices.json
$SchemaTypes = "Tenant", "ChildTenants", "UpdateTenantJsonPatchArray"

foreach ($SchemaType in $SchemaTypes) {
    generatejsonschematypes `
        TenancyServices.json `
        --rootNamespace Marain.Tenancy.ClientTenantProvider.TenancyClientSchemaTypes --rootPath "#/components/schemas/$SchemaType" --outputPath .
}