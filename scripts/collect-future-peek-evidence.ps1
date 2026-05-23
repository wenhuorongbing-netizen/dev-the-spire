param(
    [string]$EvidenceDir,

    [switch]$Launch,

    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$previewToolsScript = Join-Path $PSScriptRoot 'collect-preview-tools-evidence.ps1'
if (-not (Test-Path -LiteralPath $previewToolsScript -PathType Leaf)) {
    throw "Missing preview tools evidence helper: $previewToolsScript"
}

Write-Warning 'collect-future-peek-evidence.ps1 is a compatibility wrapper. Use collect-preview-tools-evidence.ps1 for the integrated Spire Plus preview tools.'

$arguments = @{}
if ($EvidenceDir) {
    $arguments['EvidenceDir'] = $EvidenceDir
}

if ($Launch) {
    $arguments['Launch'] = $true
}

if ($NoLaunch) {
    $arguments['NoLaunch'] = $true
}

& $previewToolsScript @arguments
exit $LASTEXITCODE
