<#
This is called during Marain.Instance infrastructure deployment after the Marain-ArmDeploy.ps
script. It is our opportunity to do any deployment work that needs to happen after Azure resources
have been deployed.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    $ServiceDeploymentContext.MakeAppServiceCommonService("Marain.Tenancy.Operations.Control", "control")

    Write-Host 'Uploading function code packages'

    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Operations.StatusHost.zip",
        $ServiceDeploymentContext.AppName + "status"
    )
    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Operations.ControlHost.zip",
        $ServiceDeploymentContext.AppName + "control"
    )

    # ensure the service tenancy exists
    Write-Host "Ensuring Operations service tenant..."
    $serviceManifest = Join-Path $PSScriptRoot "ServiceManifests\OperationsServiceManifest.jsonc" -Resolve
    try {
        $cliOutput = & $ServiceDeploymentContext.InstanceContext.MarainCliPath create-service $serviceManifest
        if ( $LASTEXITCODE -ne 0 -and -not ($cliOutput -imatch 'service tenant.*already exists') ) {
            Write-Error "Error whilst trying to register the Operations service tenant: ExitCode=$LASTEXITCODE`n$cliOutput"
        }
    }
    catch {
        throw $_
    }
}