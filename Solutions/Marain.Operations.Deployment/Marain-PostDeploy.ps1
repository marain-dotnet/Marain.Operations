﻿<#
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

}