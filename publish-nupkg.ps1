# Recursively remove bin, obj, ClientBin, Generated_Code folders
Get-ChildItem -Path . -Directory -Recurse -Include 'bin','obj','ClientBin','Generated_Code' | 
ForEach-Object {
    Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Removed: $($_.FullName)"
}

# Build and pack in Release configuration
dotnet build --configuration Release
dotnet pack --configuration Release

# NuGet push parameters
$nugeturl = "https://api.nuget.org/v3/index.json"
$pkey = "yourpkey"  # Replace with your actual API key

# Push all .nupkg files
Get-ChildItem -Path . -Recurse -Filter *.nupkg | 
ForEach-Object {
    Write-Host "Pushing $($_.Name) to NuGet..."
    dotnet nuget push $_.FullName --api-key $pkey --source $nugeturl
}

# Optional pause (remove if not needed)
# pause