start for /d /r . %%d in (bin,obj, ClientBin,Generated_Code) do @if exist "%%d" rd /s /q "%%d"
dotnet build --configuration Release
dotnet pack --configuration Release

ECHO off
set nugeturl=https://api.nuget.org/v3/index.json
set pkey=yourpkey
SETlOCAL enabledelayedexpansion

SET FIND_DIR= %cd%

for /R %FIND_DIR% %%f in (*.nupkg) do (
    dotnet nuget push %%f --api-key %pkey% --source %nugeturl%
)
pause