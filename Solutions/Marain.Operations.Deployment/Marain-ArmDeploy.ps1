<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    [MarainAppService]$TenancyService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy")

    $OperationsControlAppId = $ServiceDeploymentContext.GetAppId("control")
    $TemplateParameters = @{
        appName="operations"
        controlFunctionEasyAuthAadClientId=$OperationsControlAppId
        tenancyServiceResourceIdForMsiAuthentication=$TenancyService.AuthAppId
        tenancyServiceBaseUri=$TenancyService.BaseUrl
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("operations")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    #$ServiceDeploymentContext.Variables["KeyVaultName"] = $DeploymentResult.Outputs.keyVaultName.Value
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.controlFunctionServicePrincipalId.Value, "status", $null)
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.statusFunctionServicePrincipalId.Value, "control", $null)
}