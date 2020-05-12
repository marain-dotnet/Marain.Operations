<#
This is called during Marain.Instance infrastructure deployment prior to the Marain-ArmDeploy.ps
script. It is our opportunity to perform initialization that needs to complete before any Azure
resources are created.

We create the Azure AD Application that the Operations Control function will use to authenticate incoming
requests. (Currently, this application is used with Azure Easy Auth, but the service could also
use it directly.)

#>

# Marain.Instance expects us to define just this one function.
Function MarainDeployment([MarainServiceDeploymentContext] $ServiceDeploymentContext) {

    # Don't think we need an app for status, since it is open by design
    #$StatusApp = $ServiceDeploymentContext.DefineAzureAdAppForAppService("status")
    $ControlApp = $ServiceDeploymentContext.DefineAzureAdAppForAppService("control")

    $ControllerAppRoleId = "77d9c620-a258-4f0b-945c-a7128e82f3ec"
    $ControlApp.EnsureAppRolesContain(
        $ControllerAppRoleId,
        "Operations controller",
        "Able to create and modify operations",
        "OperationsController",
        ("User", "Application"))

    # ensure the service tenancy exists
    $serviceManifest = Join-Path $PSScriptRoot "..\ServiceManifests\OperationsServiceManifest.jsonc" -Resolve
    & $ServiceDeploymentContext.InstanceContext.MarainCliPath create-service $serviceManifest
    if ($LASTEXITCODE -ne 0) {
        # TODO: Ignore error when service tenant already exists
        Write-Error "Error whilst trying to register the Operations service tenant: ExitCode=$LASTEXITCODE"
    }
}