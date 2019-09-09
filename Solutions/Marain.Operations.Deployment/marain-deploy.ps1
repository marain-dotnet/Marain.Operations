<#

.EXAMLE

.\deploy.ps1 `
	-Prefix "end" `
	-AppName "operations" `
	-Environment "dev" `
	-OperationsStatusHostFunctionsMsDeployPackagePath "..\Marain.Operations.Functions\bin\Debug\package\Marain.Operations.Functions.zip"
#>

[CmdletBinding(DefaultParametersetName='None')] 
param(
	[string] $Prefix = "mar",
	[string] $AppName = "operations",
	[string] $Suffix = "dev",
	[string] $ResourceGroupLocation = "uksouth"
)

Process{
	$ResourceGroupName = $Prefix + "." + $AppName + "." + $Suffix
	$ArtifactStorageResourceGroupName = $ResourceGroupName + "." + "artifacts"
	$ArtifactStorageAccountName = $Prefix + $AppName + $Suffix + "ar"
	$DefaultName = $Prefix + $AppName + $Suffix
	$StatusFunctionsAppName = $Prefix + $AppName + "status" + $Suffix
	$ControlFunctionsAppName = $Prefix + $AppName + "control" + $Suffix
	$DiagnosticsStorageAccountName = $Prefix + $AppName + "audit" + $Suffix
	$ResourceGroupLocation = "uksouth"

	.\deploy.ps1 `
        -ArtifactStorageResourceGroupName $ArtifactStorageResourceGroupName `
        -ArtifactStorageAccountName $ArtifactStorageAccountName `
		-StatusFunctionsAppName $StatusFunctionsAppName `
		-ControlFunctionsAppName $ControlFunctionsAppName `
		-StatusFunctionAnonymousAccessEnabled `
		-OperationsStorageAccountName $DefaultName `
		-ResourceGroupName $ResourceGroupName `
		-ResourceGroupLocation $ResourceGroupLocation `
		-KeyVaultName $DefaultName `
		-OperationsStorageAccountSecretName "operationsstorage" `
		-AppInsightsInstanceName $DefaultName `
		-ControlAadClientId "04f4d62e-5e78-479e-a834-27038f3393e0" `
		-StatusAadClientId "bfacd6b5-77ff-454a-977c-9e12f3f92029" `
		-DiagnosticsStorageAccountName $DiagnosticsStorageAccountName
}


<#
Applications IDs:

Name                           Value                               
----                           -----                               
maroperationscontroldevAAD     04f4d62e-5e78-479e-a834-27038f3393e0
maroperationsstatusdevAAD      bfacd6b5-77ff-454a-977c-9e12f3f92029
#>