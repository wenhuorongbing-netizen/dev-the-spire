param(
    [string]$AssetManifestCsvPath = 'manifests\asset_manifest.csv',
    [string]$AssetManifestDocPath = 'docs\features\sts1-events\asset-manifest.md',
    [string]$AssetsDocPath = 'docs\features\sts1-events\assets.md',
    [string]$GitIgnorePath = '.gitignore',
    [string]$ImageRoot = 'EZMicroBalance\images\events',
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Required file not found: $resolved"
        exit 1
    }

    return [System.IO.File]::ReadAllText($resolved)
}

function Add-Check {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [Parameter(Mandatory = $true)][string]$Detail
    )

    $checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null

    if (-not $Passed) {
        $mismatches.Add("${Name}: $Detail") | Out-Null
    }
}

function Add-ContainsCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    Add-Check -Name $Name -Passed ($Text.Contains($Needle)) -Detail "requires '$Needle'"
}

function Add-SetCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)]$Actual,
        [Parameter(Mandatory = $true)]$Expected
    )

    $actualJoined = (@($Actual) | Sort-Object -Unique) -join ','
    $expectedJoined = (@($Expected) | Sort-Object -Unique) -join ','
    Add-Check -Name $Name -Passed ($actualJoined -eq $expectedJoined) -Detail "expected '$expectedJoined', found '$actualJoined'"
}

$resolvedManifest = Resolve-RepoPath $AssetManifestCsvPath
$resolvedImageRoot = Resolve-RepoPath $ImageRoot
if (-not (Test-Path -LiteralPath $resolvedManifest)) {
    Write-Error "Asset manifest CSV not found: $resolvedManifest"
    exit 1
}

if (-not (Test-Path -LiteralPath $resolvedImageRoot)) {
    Write-Error "Image root not found: $resolvedImageRoot"
    exit 1
}

$manifestRows = @(Import-Csv -LiteralPath $resolvedManifest)
$manifestIds = @($manifestRows | Select-Object -ExpandProperty sts2_entry)
$assetDoc = Read-RepoText $AssetManifestDocPath
$assetsDoc = Read-RepoText $AssetsDocPath
$gitIgnore = Read-RepoText $GitIgnorePath

$docIds = @([regex]::Matches($assetDoc, '(?m)^\|\s*\d+\s*\|\s*(sts1_[^|\s]+)\s*\|') | ForEach-Object { $_.Groups[1].Value })

Add-Check -Name 'asset_manifest_csv_rows' -Passed ($manifestRows.Count -eq 48) -Detail "expected 48 asset rows, found $($manifestRows.Count)"
Add-Check -Name 'asset_manifest_doc_rows' -Passed ($docIds.Count -eq 48) -Detail "expected 48 asset doc rows, found $($docIds.Count)"
Add-SetCheck -Name 'asset_manifest_doc_ids_match_csv' -Actual $docIds -Expected $manifestIds
Add-Check -Name 'asset_manifest_required_all_true' -Passed (@($manifestRows | Where-Object { $_.required -ne 'true' }).Count -eq 0) -Detail 'all active asset manifest rows must be required=true'
Add-Check -Name 'asset_manifest_dest_names_are_sts1_png' -Passed (@($manifestRows | Where-Object { $_.dest_filename -notmatch '^sts1_[a-z0-9_]+\.png$' }).Count -eq 0) -Detail 'all destination filenames must be sts1_*.png'
Add-Check -Name 'asset_manifest_source_names_are_png' -Passed (@($manifestRows | Where-Object { $_.source_filename -notmatch '\.png$' }).Count -eq 0) -Detail 'all source filenames must be PNG'
Add-Check -Name 'asset_manifest_includes_purifier' -Passed ($manifestIds -contains 'sts1_purifier') -Detail 'Purifier portrait target must be tracked in asset manifest'
Add-Check -Name 'asset_manifest_includes_golden_shrine' -Passed ($manifestIds -contains 'sts1_golden_shrine') -Detail 'Golden Shrine portrait target must be tracked in asset manifest'
Add-Check -Name 'asset_manifest_excludes_neow_special_stub' -Passed ($manifestIds -notcontains 'sts1_neow') -Detail 'Neow is a special stub, not an unknown-room portrait target'
Add-Check -Name 'asset_manifest_excludes_combat_start_special_stub' -Passed ($manifestIds -notcontains 'sts1_combat_start') -Detail 'Combat Start is a special stub, not an unknown-room portrait target'

Add-ContainsCheck -Name 'asset_doc_images_available_zero' -Text $assetDoc -Needle '| Images available | 0 |'
Add-ContainsCheck -Name 'asset_doc_permission_unconfirmed' -Text $assetDoc -Needle '| Redistribution permission | Not confirmed for StS1 original art |'
Add-ContainsCheck -Name 'asset_doc_o12_blocked' -Text $assetDoc -Needle 'O12 verdict: blocked.'
Add-ContainsCheck -Name 'asset_doc_local_extraction_local_qa_only' -Text $assetDoc -Needle 'Local extraction is local QA evidence only.'
Add-ContainsCheck -Name 'asset_doc_no_package_without_permission' -Text $assetDoc -Needle 'Extracted original StS1 images must not be included in a tester package, release package, handoff bundle, tracked file, or public artifact unless redistribution permission is confirmed and documented.'
Add-ContainsCheck -Name 'assets_doc_original_art_not_committed' -Text $assetsDoc -Needle 'Original StS1 art is not committed'
Add-ContainsCheck -Name 'assets_doc_output_sts1_png_gitignored' -Text $assetsDoc -Needle 'The default output directory is gitignored.'
Add-ContainsCheck -Name 'assets_doc_local_extraction_local_qa_only' -Text $assetsDoc -Needle 'local extraction is local QA evidence only'
Add-ContainsCheck -Name 'assets_doc_no_handoff_without_permission' -Text $assetsDoc -Needle 'Extracted original StS1 images must not be included in a tester package, release package, handoff bundle, tracked file, or public artifact unless redistribution permission is confirmed and documented.'
Add-ContainsCheck -Name 'assets_doc_validation_not_render_gate_proof' -Text $assetsDoc -Needle 'Passing this script proves only that local extracted files exist for the current machine.'
Add-ContainsCheck -Name 'assets_doc_placeholder_non_parity' -Text $assetsDoc -Needle 'Runtime screenshots can document this as a non-parity placeholder, but they cannot be described as StS1 art parity.'
Add-ContainsCheck -Name 'gitignore_ignores_extracted_sts1_pngs' -Text $gitIgnore -Needle '/EZMicroBalance/images/events/sts1_*.png'
Add-ContainsCheck -Name 'gitignore_ignores_extracted_sts1_dir' -Text $gitIgnore -Needle '/EZMicroBalance/images/events/sts1/'

$trackedSts1Images = @(Get-ChildItem -LiteralPath $resolvedImageRoot -File -Recurse |
    Where-Object { $_.Name -match '^sts1_.*\.(png|jpg|jpeg|webp)$' })
$ancientPortraits = @(Get-ChildItem -LiteralPath $resolvedImageRoot -File |
    Where-Object { $_.Name -match '^ezmb_(lotha|morvi|urda)\.png$' })

Add-Check -Name 'tracked_sts1_event_images_zero' -Passed ($trackedSts1Images.Count -eq 0) -Detail "expected 0 tracked sts1_* event images, found $($trackedSts1Images.Count)"
Add-Check -Name 'ancient_portraits_present_count' -Passed ($ancientPortraits.Count -eq 3) -Detail "expected 3 Ancient portraits in images/events, found $($ancientPortraits.Count)"

$report = [pscustomobject]@{
    Checks = $checks
    Mismatches = $mismatches
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
