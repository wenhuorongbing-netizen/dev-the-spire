param(
    [string]$GameRoot = $env:STS2_PATH,
    [switch]$NoRefreshFromInstalled
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')
$manifestPath = Join-Path $repoRoot 'EZMicroBalance.json'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json

if ($manifest.id -ne 'EZMicroBalance') {
    throw "Expected stable manifest id EZMicroBalance, found '$($manifest.id)'."
}

if ($manifest.name -ne 'Spire Plus') {
    throw "Expected player-facing name Spire Plus, found '$($manifest.name)'."
}

if ([string]::IsNullOrWhiteSpace($GameRoot)) {
    $GameRoot = 'D:\Steam\steamapps\common\Slay the Spire 2'
}

$publishRoot = Join-Path $repoRoot 'publish'
$stagingRoot = Join-Path $publishRoot 'package-staging'
$stagingModDir = Join-Path $stagingRoot 'EZMicroBalance'
$versionedRoot = Join-Path $publishRoot "SpirePlus-$($manifest.version)"
$versionedModDir = Join-Path $versionedRoot 'EZMicroBalance'
$zipPath = Join-Path $publishRoot "SpirePlus-$($manifest.version).zip"
$legacyZipPath = Join-Path $publishRoot "EZMicroBalance-$($manifest.version).zip"
$installedModDir = Join-Path $GameRoot 'mods\EZMicroBalance'

function Assert-UnderPath {
    param(
        [Parameter(Mandatory)] [string]$Candidate,
        [Parameter(Mandatory)] [string]$Parent
    )

    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\') + '\'
    $candidateFull = [System.IO.Path]::GetFullPath($Candidate)
    if (-not $candidateFull.StartsWith($parentFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify path outside expected parent. Candidate: $candidateFull Parent: $parentFull"
    }
}

New-Item -ItemType Directory -Force -Path $publishRoot, $stagingModDir | Out-Null

if (-not $NoRefreshFromInstalled) {
    if (-not (Test-Path -LiteralPath $installedModDir)) {
        throw "Installed mod directory not found: $installedModDir. Run dotnet publish first or pass -NoRefreshFromInstalled."
    }

    foreach ($fileName in @('EZMicroBalance.dll', 'EZMicroBalance.json', 'EZMicroBalance.pck')) {
        $source = Join-Path $installedModDir $fileName
        if (-not (Test-Path -LiteralPath $source)) {
            throw "Installed artifact missing: $source"
        }

        Copy-Item -LiteralPath $source -Destination (Join-Path $stagingModDir $fileName) -Force
    }
}

$readmePath = Join-Path $stagingModDir 'README_INSTALL.txt'
if (-not (Test-Path -LiteralPath $readmePath)) {
    @"
Spire Plus private beta package

Archive name: SpirePlus-$($manifest.version).zip
Manifest id: EZMicroBalance

Install by placing this EZMicroBalance folder under the Slay the Spire 2 mods folder.
Requires BaseLib v3.1.2 installed under the same mods folder as BaseLib.
"@ | Set-Content -LiteralPath $readmePath -Encoding UTF8
}

foreach ($target in @($versionedRoot, $zipPath, $legacyZipPath)) {
    Assert-UnderPath -Candidate $target -Parent $publishRoot
    if (Test-Path -LiteralPath $target) {
        Remove-Item -LiteralPath $target -Recurse -Force
    }
}

New-Item -ItemType Directory -Force -Path $versionedRoot | Out-Null
Copy-Item -LiteralPath $stagingModDir -Destination $versionedRoot -Recurse -Force

Compress-Archive -LiteralPath $versionedModDir -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "Created $zipPath"
