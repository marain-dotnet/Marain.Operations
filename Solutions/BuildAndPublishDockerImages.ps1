docker build --no-cache -t idgatmos2.azurecr.io/marain.operations.control:latest --build-arg PROJECT_FOLDER=Marain.Operations.ControlHost.Functions .
docker build --no-cache -t idgatmos2.azurecr.io/marain.operations.status:latest --build-arg PROJECT_FOLDER=Marain.Operations.StatusHost.Functions .

docker push idgatmos2.azurecr.io/marain.operations.control
docker push idgatmos2.azurecr.io/marain.operations.status