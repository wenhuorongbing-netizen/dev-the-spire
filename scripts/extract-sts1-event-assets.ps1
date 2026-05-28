<#
.SYNOPSIS
    Extracts Slay the Spire 1 event portraits from a local StS1 installation.

.DESCRIPTION
    Reads the asset manifest and copies event portrait PNGs from the StS1
    install directory to the mod's images folder. Files are renamed to match
    StS2 event entry names.

    Original StS1 art is NOT committed to the repository. The output directory
    should be gitignored.

.PARAMETER Sts1Path
    Path to the local Slay the Spire 1 installation.
    Example: "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire"

.PARAMETER OutputPath
    Output directory for extracted assets.
    Default: "EZMicroBalance/images/events"

.EXAMPLE
    .\extract-sts1-event-assets.ps1 -Sts1Path "D:\Steam\steamapps\common\Slay the Spire"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Sts1Path,

    [Parameter(Mandatory = $false)]
    [string]$OutputPath = "EZMicroBalance/images/events"
)

$ErrorActionPreference = "Stop"

$manifestPath = Join-Path $PSScriptRoot "..\manifests\asset_manifest.csv"
if (-not (Test-Path $manifestPath)) {
    Write-Error "Asset manifest not found at: $manifestPath"
    exit 1
}

$manifest = Import-Csv $manifestPath
$sourceBase = Join-Path $Sts1Path "resources\images\events"

if (-not (Test-Path $sourceBase)) {
    Write-Error "StS1 events directory not found at: $sourceBase"
    exit 1
}

if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

$extracted = 0
$skipped = 0
$missing = 0

foreach ($entry in $manifest) {
    $sourceFile = Join-Path $sourceBase $entry.source_filename
    $destFile = Join-Path $OutputPath $entry.dest_filename

    if (Test-Path $sourceFile) {
        if (Test-Path $destFile) {
            Write-Host "  SKIP (exists): $($entry.dest_filename)" -ForegroundColor Yellow
            $skipped++
        } else {
            Copy-Item $sourceFile $destFile
            Write-Host "  OK: $($entry.source_filename) -> $($entry.dest_filename)" -ForegroundColor Green
            $extracted++
        }
    } else {
        if ($entry.required -eq "true") {
            Write-Warning "  MISSING (required): $($entry.source_filename)"
            $missing++
        } else {
            Write-Host "  SKIP (optional, not found): $($entry.source_filename)" -ForegroundColor DarkGray
        }
    }
}

Write-Host ""
Write-Host "Extraction complete:" -ForegroundColor Cyan
Write-Host "  Extracted: $extracted"
Write-Host "  Skipped:   $skipped"
Write-Host "  Missing:   $missing"

if ($missing -gt 0) {
    Write-Warning "$missing required assets are missing. Events will use default portraits."
    exit 2
}
