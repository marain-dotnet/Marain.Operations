param(
	[string]$ScriptPath,
	[string]$prefix,
	[hashtable]$secretVariables
)

Set-Location $PSScriptRoot

$rawParams = @{}
$params = @{}
# Add all environment variables to the parameters hashtable
if ($prefix)
{
	(Get-ChildItem env:) | Where-Object {$_.Name.StartsWith($prefix, "InvariantCultureIgnoreCase") } | Foreach-Object { $rawParams[$_.Name.Substring($prefix.Length)] = $_.Value }
} 
else {
	(Get-ChildItem env:) | Foreach-Object { $rawParams[$_.Name] = $_.Value }
}

# Add all secret variables to the parameters hashtable
if ($secretVariables)
{
	$secretVariables.GetEnumerator() | Foreach-Object { $rawParams[$_.Name] = (convertto-securestring $_.Value -asplaintext -force) }
}

# Strip the parameters hashtable down to the required set of parameters for the script
$scriptParameters = (Get-Command $ScriptPath).Parameters.GetEnumerator() | Select Key
$rawParams.GetEnumerator() | Foreach-Object { if ($scriptParameters.Key -contains $_.Name) { $params.Add($_.Name, $_.Value) } }

Write-Host ($params | Out-String)

& $ScriptPath @params