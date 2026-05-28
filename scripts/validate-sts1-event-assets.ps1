<#
.SYNOPSIS
    Validates that all required StS1 event assets exist in the mod images directory.

.DESCRIPTION
    Checks the asset manifest and verifies that all required event portraits
    exist in the output directory. Reports missing or invalid assets.

.EXAMPLE
    .\validate-sts1-event-assets.ps1
#>

param(
    [Parameter(Mandatory = $false)]
    [string]$ImagePath = "EZMicroBalance/images/events"
)

$ErrorActionPreference = "Stop"

$manifestPath = Join-Path $PSScriptRoot "..\manifests\asset_manifest.csv"
if (-not (Test-Path $manifestPath)) {
    Write-Error "Asset manifest not found at: $manifestPath"
    exit 1
}

$manifest = Import-Csv $manifestPath

$ok = 0
$missing = 0
$errors = @()

foreach ($entry in $manifest) {
    $file = Join-Path $ImagePath $entry.dest_filename

    if (Test-Path $file) {
        $size = (Get-Item $file).Length
        if ($size -gt 0) {
            Write-Host "  OK: $($entry.dest_filename) ($size bytes)" -ForegroundColor Green
            $ok++
        } else {
            $errors += "EMPTY: $($entry.dest_filename)"
            $missing++
        }
    } else {
        if ($entry.required -eq "true") {
            $errors += "MISSING: $($entry.dest_filename)"
            $missing++
        } else {
            Write-Host "  SKIP (optional): $($entry.dest_filename)" -ForegroundColor DarkGray
        }
    }
}

Write-Host ""
Write-Host "Validation complete:" -ForegroundColor Cyan
Write-Host "  OK:      $ok"
Write-Host "  Missing: $missing"

if ($errors.Count -gt 0) {
    Write-Host ""
    Write-Host "Issues:" -ForegroundColor Red
    foreach ($err in $errors) {
        Write-Host "  $err" -ForegroundColor Red
    }
    exit 1
}

Write-Host "All required assets present." -ForegroundColor Green
