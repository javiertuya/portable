#!/bin/bash
# Prepare and publish JavaToCSharp as a nuget global tool
# Configure the API_KEY before
API_KEY="xxxxxxxxxxxxxxxxxxxxxx"

# Get the project, adds required tags and prepare nuget package. Using a forked version
git clone --depth=1 https://github.com/javiertuya/JavaToCSharp.git JavaToCSharp-temp --branch master-fork
sed -i "s|</Version>|</Version><PackAsTool>true</PackAsTool><PackageOutputPath>./nupkg</PackageOutputPath>|g" JavaToCSharp-temp/JavaToCSharpCli/JavaToCSharpCli.csproj
dotnet pack --configuration Release JavaToCSharp-temp/JavaToCSharpCli/JavaToCSharpCli.csproj

# Publish to the github package manager
dotnet nuget add source --username javiertuya --password "${API_KEY}" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/javiertuya/index.json"
dotnet nuget push JavaToCSharp-temp/JavaToCSharpCli/nupkg/*.nupkg  --api-key "${API_KEY}" --source "github"
# Do not forget set this package as public in package settings (if not, it will require custom permissions)

# Install and verify
dotnet tool install JavaToCSharpCli --global
# dotnet tool update JavaToCSharpCli --global
dotnet tool list --global
JavaToCSharpCli --help
# ant convert
