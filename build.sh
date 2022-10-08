#/bin/bash

# Restore
dotnet restore

# Build
dotnet build

# Publish Web
dotnet publish -c Release -o ./bin/