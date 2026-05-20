#!/bin/bash
set -e

echo "=> Running database migrations inside the container..."
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --configuration Release

echo "=> Database ready! Starting Electro-PM Web API..."
exec dotnet watch run --project API/API.csproj --configuration Debug