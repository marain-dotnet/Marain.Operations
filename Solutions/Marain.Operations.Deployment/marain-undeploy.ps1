<#

.EXAMLE

.\deploy.ps1 `
	-Prefix "mar" `
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

$ResourceGroupName = $Prefix + "." + $AppName + "." + $Suffix
$ArtifactStorageResourceGroupName = $ResourceGroupName + "." + "artifacts"
$ResourceGroupLocation = "uksouth"

Write-Host "`nThe following Azure AD applications will be deleted:`n"
Write-Host "`t$ArtifactStorageResourceGroupName"
Write-Host "`t$ResourceGroupName"

$reply = Read-Host -Prompt "`nAre you sure you want to delete these Azure AD applications? [y/n]"
if ($reply -match "[yY]") { 
	Write-Host "Deleting resource groups"
	Remove-AzureRmResourceGroup `
		-Name $ArtifactStorageResourceGroupName `
		-Force `
		-AsJob

	Remove-AzureRmResourceGroup `
		-Name $ResourceGroupName `
		-Force `
		-AsJob

	Write-Host "Deletion jobs started. Remember to clear down AAD applications using .\Scripts\Delete-AzureAdApplications.ps1"
}
else {
	Write-Host "Deletion request cancelled"
}