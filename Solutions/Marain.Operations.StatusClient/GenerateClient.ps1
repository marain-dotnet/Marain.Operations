﻿# Generates the code for this project
#
# All the code in this project is generated by downloading a Swagger file, and running the AutoRest
# tool on the results. If the service is changed, you can run this script to regenerate the code.

# This requires the OperationsStatus function to be running locally on its default port of 7077
$tmp = (New-TemporaryFile).FullName
iwr http://localhost:7077/api/swagger -o $tmp


# If you do not have autorest, install it with:
#   npm install -g autorest
# Ensure it is up to date with
#   autorest --latest
$OutputFolder = Join-Path $PSScriptRoot "Marain\Operations\Client\OperationsStatus"
autorest --input-file=$tmp --csharp --output-folder=$OutputFolder --namespace=Marain.Operations.Client.OperationsStatus --add-credentials