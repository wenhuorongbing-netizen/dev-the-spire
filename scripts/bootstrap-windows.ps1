[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$GameRoot,

    [Parameter(Mandatory = $true)]
    [string]$GodotExePath,

    [switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'

function Fail($Message) {
    Write-Error $Message
    exit 1
}

Write-Host 'EzDailyContent Windows bootstrap'
Write-Host 'This script validates local tools, creates Directory.Build.props from the example if needed, and runs build/publish.'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Fail 'dotnet was not found on PATH. Install the .NET 9 SDK first.'
}

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Fail 'git was not found on PATH. Install Git first.'
}

$resolvedGameRoot = Resolve-Path -LiteralPath $GameRoot -ErrorAction SilentlyContinue
if (-not $resolvedGameRoot) {
    Fail "GameRoot does not exist: $GameRoot"
}

$resolvedGodot = Resolve-Path -LiteralPath $GodotExePath -ErrorAction SilentlyContinue
if (-not $resolvedGodot) {
    Fail "GodotExePath does not exist: $GodotExePath"
}

$examplePath = Join-Path (Get-Location) 'Directory.Build.props.example'
$propsPath = Join-Path (Get-Location) 'Directory.Build.props'

if (-not (Test-Path -LiteralPath $examplePath)) {
    Fail 'Directory.Build.props.example was not found.'
}

if (-not (Test-Path -LiteralPath $propsPath)) {
    Copy-Item -LiteralPath $examplePath -Destination $propsPath
    Write-Host 'Created Directory.Build.props from Directory.Build.props.example.'
} else {
    Write-Host 'Directory.Build.props already exists; updating local paths only.'
}

$godotValue = $resolvedGodot.Path.Replace('\', '/')
$sts2Value = $resolvedGameRoot.Path.Replace('\', '/')
$props = Get-Content -Raw -LiteralPath $propsPath
$props = [regex]::Replace($props, '<GodotPath>.*?</GodotPath>', "<GodotPath>$godotValue</GodotPath>")
$props = [regex]::Replace($props, '<Sts2Path>.*?</Sts2Path>', "<Sts2Path>$sts2Value</Sts2Path>")
Set-Content -LiteralPath $propsPath -Value $props -Encoding UTF8

$baseLibPath = Join-Path $resolvedGameRoot.Path 'mods\BaseLib'
Write-Host "Expected BaseLib path: $baseLibPath"

$baseLibFiles = @('BaseLib.json', 'BaseLib.dll', 'BaseLib.pck')
$missingBaseLib = @()
foreach ($file in $baseLibFiles) {
    $candidate = Join-Path $baseLibPath $file
    if (-not (Test-Path -LiteralPath $candidate)) {
        $missingBaseLib += $file
    }
}

if ($missingBaseLib.Count -gt 0) {
    Write-Warning "BaseLib appears incomplete. Missing: $($missingBaseLib -join ', ')"
    Write-Warning 'Install BaseLib v3.1.0 under <GameRoot>\mods\BaseLib before game verification.'
} else {
    Write-Host 'BaseLib runtime files found.'
}

Write-Host 'Running dotnet build...'
dotnet build
if ($LASTEXITCODE -ne 0) {
    Fail 'dotnet build failed. Publish was skipped.'
}

if (-not $SkipPublish) {
    Write-Host 'Running dotnet publish...'
    dotnet publish
    if ($LASTEXITCODE -ne 0) {
        Fail 'dotnet publish failed.'
    }
} else {
    Write-Host 'SkipPublish was set; dotnet publish was not run.'
}

Write-Host 'Bootstrap completed.'
Write-Host 'Launch Slay the Spire 2, open Settings -> Mod Settings, and confirm BaseLib plus EzDailyContent appear and are enabled.'
