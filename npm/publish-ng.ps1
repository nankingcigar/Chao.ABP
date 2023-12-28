
$commands = (
  "cd ng-packs",
  "npm install",
  "npm run build-module",
  "cd dist\nankingcigar\ng.core",
  "npm publish",
  "cd ..\ng.translate",
  "npm publish",
  "cd ..\ng-zorro-antd",
  "npm publish",
  "cd ..\ng-monaco-editor",
  "npm publish",
  "cd ..\ng-wang-editor",
  "npm publish"
)

foreach ($command in $commands) { 
  $timer = [System.Diagnostics.Stopwatch]::StartNew()
  Write-Host $command
  Invoke-Expression $command
  if ($LASTEXITCODE -ne '0' -And $command -notlike '*cd *') {
    Write-Host ("Process failed! " + $command)
  }
  $timer.Stop()
  $total = $timer.Elapsed
  Write-Output "-------------------------"
  Write-Output "$command command took $total (Hours:Minutes:Seconds.Milliseconds)"
  Write-Output "-------------------------"
}
