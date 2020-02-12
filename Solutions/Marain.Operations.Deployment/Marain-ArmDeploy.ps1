<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    $TemplateParameters = @{
        appName="operations"
        controlFunctionEasyAuthAadClientId=$ServiceDeploymentContext.Variables["OperationsControlAppId"]
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("operations")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    $ServiceDeploymentContext.Variables["KeyVaultName"] = $DeploymentResult.Outputs.keyVaultName.Value
    $ServiceDeploymentContext.Variables["StatusFunctionServicePrincipalId"] = $DeploymentResult.Outputs.controlFunctionServicePrincipalId.Value
    $ServiceDeploymentContext.Variables["ControlFunctionServicePrincipalId"] = $DeploymentResult.Outputs.statusFunctionServicePrincipalId.Value
}