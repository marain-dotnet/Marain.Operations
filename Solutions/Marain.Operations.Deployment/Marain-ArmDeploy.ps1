<#
This is called during Marain.Instance infrastructure deployment after the Marain-PreDeploy.ps
script. It is our opportunity to create Azure resources.
#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    # TODO: make this discoverable
    $serviceTenantId = '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858'
    $serviceTenantDisplayName = 'Operations v1'

    [MarainAppService]$TenancyService = $ServiceDeploymentContext.InstanceContext.GetCommonAppService("Marain.Tenancy")

    $OperationsControlAppId = $ServiceDeploymentContext.GetAppId("control")
    $TemplateParameters = @{
        appName="operations"
        controlFunctionEasyAuthAadClientId=$OperationsControlAppId
        tenancyServiceResourceIdForMsiAuthentication=$TenancyService.AuthAppId
        tenancyServiceBaseUri=$TenancyService.BaseUrl
        appInsightsInstrumentationKey=$ServiceDeploymentContext.InstanceContext.ApplicationInsightsInstrumentationKey
        marainServiceTenantId=$serviceTenantId
        marainServiceTenantDisplayName=$serviceTenantDisplayName
    }
    $InstanceResourceGroupName = $InstanceDeploymentContext.MakeResourceGroupName("operations")
    $DeploymentResult = $ServiceDeploymentContext.InstanceContext.DeployArmTemplate(
        $PSScriptRoot,
        "deploy.json",
        $TemplateParameters,
        $InstanceResourceGroupName)

    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.controlFunctionServicePrincipalId.Value, "status", $null)
    $ServiceDeploymentContext.SetAppServiceDetails($DeploymentResult.Outputs.statusFunctionServicePrincipalId.Value, "control", $null)
}