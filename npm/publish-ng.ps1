$env:NODE_AUTH_TOKEN="access_token"

npm config set //registry.npmjs.org/:_authToken $env:NODE_AUTH_TOKEN

$commands = @(
  "cd ng-packs",
  "npm install",
  "npm run build-module",

  "cd dist\nankingcigar\ng.core",
  "npm publish --access public",

  "cd ..\ng.translate",
  "npm publish --access public",

  "cd ..\ng-zorro-antd",
  "npm publish --access public",

  "cd ..\ng-monaco-editor",
  "npm publish --access public",

  "cd ..\ng-wang-editor",
  "npm publish --access public"
)

foreach ($command in $commands) {

  $timer = [System.Diagnostics.Stopwatch]::StartNew()

  Write-Host ""
  Write-Host "=================================================="
  Write-Host $command
  Write-Host "=================================================="

  Invoke-Expression $command

  if ($LASTEXITCODE -ne 0 -and $command -notlike "cd *") {
    Write-Host ""
    Write-Host "FAILED: $command"
    exit 1
  }

  $timer.Stop()

  Write-Host ""
  Write-Host "Finished in $($timer.Elapsed)"
}