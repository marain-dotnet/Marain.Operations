<#

.EXAMLE

.\deploy.ps1 `
	-ArtifactStorageResourceGroupName "end.operations.dev" `
	-ArtifactStorageAccountName "endoperationsdevar" `
	-ResourceGroupName "end.operations.dev" `
	-StatusFunctionsAppName "endoperationsstatusdev" `
	-ControlFunctionsAppName "endoperationscontroldev" `
	-OperationsStorageAccountName "endoperationsdev" `
	-ResourceGroupLocation "uksouth" `
	-KeyVaultName "endoperationsdev" `
	-OperationsStorageAccountSecretName "operationsstorage" `
	-DiagnosticsStorageAccountName "endoperationsauditdev" `
	-AppInsightsInstanceName "endoperationsdev" `
	-ControlAadClientId f870406e-6890-4ebc-998a-6cca50f0fc99 `
	-StatusAadClientId 65c11237-75ba-4024-a7f5-73580c5b1e9b
#>

[CmdletBinding(DefaultParametersetName='None')] 
param(
	[string] $ArtifactStorageResourceGroupName,
	[string] $ArtifactStorageAccountName,

	[string] $StatusFunctionsAppName,
	[string] $ControlFunctionsAppName,
	[switch] $StatusFunctionAnonymousAccessEnabled,
	[switch] $ControlFunctionAnonymousAccessEnabled,
	[switch] $OperationsStorageAccountAlreadyExists,
	[string] $OperationsStorageAccountName,
	[string] $ResourceGroupName,
	[string] $ResourceGroupLocation,

	[switch] $KeyVaultAlreadyExists,
	[string] $KeyVaultName,
	[string] $OperationsStorageAccountSecretName,

	[string] $DiagnosticsStorageAccountName,

	[switch] $AppInsightsAlreadyExists,
	[string] $AppInsightsInstanceName,

	[string] $ControlAadClientId,
	[string] $StatusAadClientId,

	[string] $OperationsMarainServiceTenantId,
	[string] $OperationsMarainServiceTenantDisplayName,

	[string] $ArtifactStagingDirectory = ".",
	[string] $ArtifactStorageContainerName = "stageartifacts"
)

Begin{
	# Setup options and variables
	$ErrorActionPreference = 'Stop'
	Set-Location $PSScriptRoot

	$ArtifactStagingDirectory = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory))
}

Process{
	if ($SkipDeployment) {
		Write-Host "`nSkipping deployment steps due to SkipDeployment parameter being present"
	} else {
		
		# Create resource group and artifact storage account
		Write-Host "`nStep 1: Creating resource group $ArtifactStorageResourceGroupName and artifact storage account $ArtifactStorageAccountName" -ForegroundColor Green
		try {
			.\Scripts\Create-StorageAccount.ps1 `
				-ResourceGroupName $ArtifactStorageResourceGroupName `
				-ResourceGroupLocation $ResourceGroupLocation `
				-StorageAccountName $ArtifactStorageAccountName
		}
		catch{
			throw $_
		}

		# Deploy main ARM template
		Write-Host "`nStep 2: Deploying main resources template"  -ForegroundColor Green
		try{
			$parameters = New-Object -TypeName Hashtable

			$parameters["statusFunctionsAppName"] = $StatusFunctionsAppName
			$parameters["controlFunctionsAppName"] = $ControlFunctionsAppName
			$parameters["statusFunctionAnonymousAccessEnabled"] = $StatusFunctionAnonymousAccessEnabled.IsPresent
			$parameters["controlFunctionAnonymousAccessEnabled"] = $ControlFunctionAnonymousAccessEnabled.IsPresent
			$parameters["operationsStorageAccountAlreadyExists"] = $OperationsStorageAccountAlreadyExists.IsPresent
			$parameters["operationsStorageAccountName"] = $OperationsStorageAccountName
			$parameters["keyVaultAlreadyExists"] = $KeyVaultAlreadyExists.IsPresent
			$parameters["keyVaultName"] = $KeyVaultName
			$parameters["operationsStorageAccountSecretName"] = $OperationsStorageAccountSecretName
			$parameters["appInsightsAlreadyExists"] = $AppInsightsAlreadyExists.IsPresent
			$parameters["appInsightsInstanceName"] = $AppInsightsInstanceName
			$parameters["artifactStagingDirectory"] = $ArtifactStagingDirectory
			$parameters["artifactStorageContainerName"] = $ArtifactStorageContainerName
			$parameters["controlAadClientId"] = $ControlAadClientId
			$parameters["statusAadClientId"] = $StatusAadClientId
			$parameters["marainServiceTenantId"] = $OperationsMarainServiceTenantId
			$parameters["marainServiceTenantDisplayName"] = $OperationsMarainServiceTenantDisplayName
			$parameters["diagnosticsStorageAccountName"] = $DiagnosticsStorageAccountName
			$parameters["isDeveloperEnvironment"] = $false

			$TemplateFilePath = [System.IO.Path]::Combine($ArtifactStagingDirectory, "deploy.json")

			$str = $parameters | Out-String
			Write-Host $str

			Write-Host $ArtifactStagingDirectory

			$deploymentResult = .\Deploy-AzureResourceGroup.ps1 `
				-UploadArtifacts `
				-ResourceGroupLocation $ResourceGroupLocation `
				-ResourceGroupName $ResourceGroupName `
				-StorageAccountName $ArtifactStorageAccountName `
				-ArtifactStagingDirectory $ArtifactStagingDirectory `
				-StorageContainerName $ArtifactStorageContainerName `
				-TemplateParameters $parameters `
				-TemplateFile $TemplateFilePath
		}
		catch{
			throw $_
		}
	}
}


End{
	Write-Host -ForegroundColor Green "`n######################################################################`n"
	Write-Host -ForegroundColor Green "Deployment finished"
	Write-Host -ForegroundColor Green "`n######################################################################`n"
}

