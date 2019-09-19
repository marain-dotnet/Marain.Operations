
# This requires the OperationsControl function to be running locally on its default port of 7078
$tmp = (New-TemporaryFile).FullName
iwr http://localhost:7078/api/swagger -o $tmp

$OutputFolder = Join-Path $PSScriptRoot "Marain\Operations\Client\OperationsControl"


# If you do not have autorest, install it with:
#   npm install -g autorest
# Ensure it is up to date with
#   autorest --latest
autorest --input-file=$tmp --csharp --output-folder=$OutputFolder --namespace=Marain.Operations.Client.OperationsControl --add-credentials