<#
This is called during Marain.Instance infrastructure deployment after the Marain-ArmDeploy.ps
script. It is our opportunity to do any deployment work that needs to happen after Azure resources
have been deployed.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    Write-Host 'Grant the functions access to the KV'

    #$InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("operations")
    #$KeyVaultName = $ServiceDeploymentContext.Variables["KeyVaultName"]
    #$StatusPrincipalId = $ServiceDeploymentContext.Variables["StatusFunctionServicePrincipalId"]
    #$ControlPrincipalId = $ServiceDeploymentContext.Variables["ControlFunctionServicePrincipalId"]

    # Apparently we're doing this through the template
    #Set-AzKeyVaultAccessPolicy `
    #    -VaultName $KeyVaultName `
    #    -ResourceGroupName $InstanceResourceGroupName `
    #    -ObjectId $StatusPrincipalId `
    #    -PermissionsToSecrets Get
    #
    #Set-AzKeyVaultAccessPolicy `
    #    -VaultName $KeyVaultName `
    #    -ResourceGroupName $InstanceResourceGroupName `
    #    -ObjectId $ControlPrincipalId `
    #    -PermissionsToSecrets Get

    Write-Host 'Uploading function code packages'

    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Operations.ControlHost.zip",
        $ServiceDeploymentContext.AppName + "status"
    )
    $ServiceDeploymentContext.UploadReleaseAssetAsAppServiceSitePackage(
        "Marain.Operations.StatusHost.zip",
        $ServiceDeploymentContext.AppName + "control"
    )

}