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

Write-Host 'Spire Plus Windows bootstrap'
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

$ritsuLibPath = Join-Path $resolvedGameRoot.Path 'mods\STS2-RitsuLib'
Write-Host "Expected STS2-RitsuLib path: $ritsuLibPath"

$ritsuLibFiles = @(
    'mod_manifest.json',
    'STS2-RitsuLib.dll',
    'ritsulib-variants.manifest',
    'lib\0.107.1\STS2-RitsuLib.dll'
)
$missingRitsuLib = @()
foreach ($file in $ritsuLibFiles) {
    $candidate = Join-Path $ritsuLibPath $file
    if (-not (Test-Path -LiteralPath $candidate)) {
        $missingRitsuLib += $file
    }
}

if ($missingRitsuLib.Count -gt 0) {
    Write-Warning "STS2-RitsuLib appears incomplete. Missing: $($missingRitsuLib -join ', ')"
    Write-Warning 'Install STS2-RitsuLib v0.4.33 or newer under <GameRoot>\mods\STS2-RitsuLib before game verification.'
} else {
    Write-Host 'STS2-RitsuLib runtime files found.'
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
Write-Host 'Launch Slay the Spire 2, open Settings -> Mod Settings, and confirm STS2-RitsuLib and Spire Plus appear and are enabled.'
