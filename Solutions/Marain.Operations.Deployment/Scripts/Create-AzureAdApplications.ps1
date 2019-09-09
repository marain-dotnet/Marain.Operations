<#
.EXAMPLE
.\Create-AzureAdApplications.ps1 `
    -TenantId "0f621c67-98a0-4ed5-b5bd-31a35be41e29" `
    -ClientName "mar" `
    -AppName "operations" `
	-EnvironmentName "dev" `
	-Suffix "AAD"
#>
param(
	[string] $TenantId = "0f621c67-98a0-4ed5-b5bd-31a35be41e29",
    [string] $ClientName = "mar",
    [string] $AppName = "operations",
	[string] $EnvironmentName = "dev",
	[string] $Suffix = "AAD"
)

Begin {

	function ConfigureAdApplication($adAppName, $replyUrls, $defaultDomain, $appRoles)
	{
		$uri = "http://$defaultDomain/$adAppName"

		# Create web AD application

		Write-Host "Looking for existing application for web app..."
		$app = Get-AzureADApplication -Filter "DisplayName eq '$adAppName'"

		if ($app) {
			Write-Host "Application for web app already exists. Updating..."
			Set-AzureADApplication -ObjectId $app.ObjectId -IdentifierUris $uri -ReplyUrls $replyUrls -Oauth2AllowImplicitFlow $true -Homepage $uri -AppRoles $appRoles -GroupMembershipClaims "All"
		}
		else {
			Write-Host "Creating new application for web app..."
			$newApp = New-AzureADApplication -DisplayName $adAppName -IdentifierUris $uri -ReplyUrls $replyUrls -Oauth2AllowImplicitFlow $true -Homepage $uri -AppRoles $appRoles -GroupMembershipClaims "All"

			# Azure AD takes a while to propagate a new App around its storage systems,
			# so we need to give it a few seconds to avoid errors that will otherwise
			# occasionally occur when we try to add the service principal for this app.
			Write-Host "Allowing time for new application to propagate through Azure AD..."
			Start-Sleep 15
		}

		$app = Get-AzureADApplication -Filter "DisplayName eq '$adAppName'"

		$appId = $app.AppID

        $sp = Get-AzureADServicePrincipal -Filter "AppId eq '$appId'"
		if ($sp){
			Write-Host "Service principal for web app already exists. Updating..."
			Set-AzureADServicePrincipal -ObjectId $sp.ObjectId -AppRoleAssignmentRequired $true
		}
		else {
			Write-Host "Creating new service principal for web app..."
			$newSp = New-AzureADServicePrincipal -AppId $appId -AppRoleAssignmentRequired $true
		}

		Write-Host "Successfully set application and service principal."

		return $appId
	}

	function CreateMappings($DefaultDomain)
	{
		$defaultReplyUrl = "https://" + $ClientName + $AppName + $EnvironmentName + ".azurewebsites.net/*"

		$emptyRoles = New-Object System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.AppRole]

		$operationsStatusAppName = $ClientName + "operationsstatus" + $EnvironmentName + $Suffix
		$operationsControlAppName = $ClientName + "operationscontrol" + $EnvironmentName + $Suffix
	
		$mappings = @(
			@{AdAppName= $operationsStatusAppName; ReplyUrls=$defaultReplyUrl; AppRoles= $emptyRoles; DefaultDomain =$DefaultDomain}
			@{AdAppName= $operationsControlAppName; ReplyUrls=$defaultReplyUrl; AppRoles= $emptyRoles; DefaultDomain =$DefaultDomain}
		)

		return $mappings
	}
}

Process {

	$ErrorActionPreference = 'Stop'

	if (!(Get-Module -ListAvailable AzureAD)) {
		Write-Host "Installing/updating AzureAD module..."
		Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force -Scope CurrentUser
		Install-Module AzureAD -Scope CurrentUser -Force
	}

	Write-Host "Connecting to Azure AD..."
	Write-Host
	Connect-AzureAD -TenantId $TenantId

	$tenantDetail = Get-AzureADTenantDetail
	$defaultDomain = $tenantDetail.VerifiedDomains[0].Name

	$results = @{}
	
	$mappings = CreateMappings($defaultDomain)

	foreach($entry in $mappings){

		Write-Host "Creating AD application for" $entry.AdAppName
		$AppId = ConfigureAdApplication @entry

		$results.Add($entry.AdAppName, $AppId)
	}


	# Write out application IDs
	Write-Host
	Write-Host "Applications IDs:"
	Write-Host ($results | Out-String)
}