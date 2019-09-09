#
# Delete-AzureAdApplications.ps1
#

Param(
	[string] $TenantId = "0f621c67-98a0-4ed5-b5bd-31a35be41e29",
    [string] $ClientName = "mar",
    [string] $AppName = "operations",
	[string] $EnvironmentName = "dev",
	[string] $Suffix = "AAD"
)

Set-AzureRmContext -Tenant $TenantId

$aadName = "*" + $Suffix

$aadApps = Get-AzureRmADApplication | where { $_.DisplayName -match "$ClientName$AppName.*$EnvironmentName$Suffix"}

Write-Host "`nThe following Azure AD applications will be deleted:`n"
$aadApps | % { Write-Host $_.DisplayName }

$reply = Read-Host -Prompt "`nAre you sure you want to delete these Azure AD applications? [y/n]"
if ($reply -match "[yY]") { 
    Write-Host "`nDeleting Azure AD applications..."

    $aadApps | Remove-AzureRmADApplication -Verbose -Force

    Write-Host "Deleted Azure AD applications..."
}
else {
    Write-Host "Deletion request cancelled."
}