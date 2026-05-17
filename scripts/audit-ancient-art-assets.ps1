[CmdletBinding()]
param(
    [string]$ManifestPath = "docs/features/ancient-expansion-v2.2/art-asset-manifest.json",
    [string]$ExportPresetsPath = "export_presets.cfg",
    [string]$OutFile,
    [switch]$FailOnMissingFinal,
    [switch]$FailOnHashMismatch,
    [switch]$FailOnMissingExport,
    [switch]$FailOnInvalidGenerationMode
)

$ErrorActionPreference = "Stop"

function Get-RepoRoot {
    return [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
}

function Resolve-RepoPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,
        [Parameter(Mandatory = $true)]
        [string]$RelativePath,
        [switch]$MustExist
    )

    if ([System.IO.Path]::IsPathRooted($RelativePath)) {
        throw "Expected a repository-relative path, got absolute path: $RelativePath"
    }

    $root = [System.IO.Path]::GetFullPath($RepoRoot)
    $candidate = [System.IO.Path]::GetFullPath((Join-Path $root $RelativePath))
    if (!$candidate.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Path escapes repository root: $RelativePath"
    }

    if ($MustExist -and !(Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Missing required file: $RelativePath"
    }

    return $candidate
}

function Get-StatusCounts {
    param([object[]]$Assets)

    $counts = [ordered]@{}
    foreach ($status in @(
        "final_generated",
        "user_supplied",
        "source_local_background",
        "source_local_generated",
        "source_derived_temporary",
        "generic_temporary",
        "missing"
    )) {
        $counts[$status] = 0
    }

    foreach ($asset in $Assets) {
        $status = [string]$asset.source_status
        if (!$counts.Contains($status)) {
            $counts[$status] = 0
        }

        $counts[$status] += 1
    }

    return [pscustomobject]$counts
}

function Get-ExportedResourcePaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,
        [Parameter(Mandatory = $true)]
        [string]$RelativePath
    )

    $exportFullPath = Resolve-RepoPath -RepoRoot $RepoRoot -RelativePath $RelativePath -MustExist
    $exportPreset = Get-Content -Raw -LiteralPath $exportFullPath
    $match = [regex]::Match(
        $exportPreset,
        'export_files=PackedStringArray\((?<files>.*?)\)',
        [System.Text.RegularExpressions.RegexOptions]::Singleline)
    if (!$match.Success) {
        throw "Could not find export_files in $RelativePath"
    }

    $paths = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($fileMatch in [regex]::Matches($match.Groups["files"].Value, '"(?<path>[^"]+)"')) {
        [void]$paths.Add($fileMatch.Groups["path"].Value)
    }

    return $paths
}

function Test-RequiresExportCoverage {
    param([string]$TargetPath)

    foreach ($prefix in @(
        "EZMicroBalance/images/ancients/",
        "EZMicroBalance/images/events/",
        "EZMicroBalance/images/encounters/",
        "EZMicroBalance/images/monsters/",
        "EZMicroBalance/scenes/",
        "EZMicroBalance/images/powers/",
        "EZMicroBalance/images/card_portraits/",
        "EZMicroBalance/images/relics/",
        "EZMicroBalance/images/ascension/"
    )) {
        if ($TargetPath.StartsWith($prefix, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

$repoRoot = Get-RepoRoot
$manifestFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $ManifestPath -MustExist
$exportedResourcePaths = Get-ExportedResourcePaths -RepoRoot $repoRoot -RelativePath $ExportPresetsPath
$manifest = Get-Content -Raw -LiteralPath $manifestFullPath | ConvertFrom-Json
$assets = @($manifest.assets)

$missingTargets = New-Object System.Collections.Generic.List[object]
$hashMismatches = New-Object System.Collections.Generic.List[object]
$missingExports = New-Object System.Collections.Generic.List[object]
$invalidGenerationModes = New-Object System.Collections.Generic.List[object]
$hashRecords = New-Object System.Collections.Generic.List[object]
$requiredGenerationMode = if ($manifest.required_generation_mode) { [string]$manifest.required_generation_mode } else { "GPTimage2" }

foreach ($asset in $assets) {
    $targetPath = [string]$asset.target_path
    $sourceStatus = [string]$asset.source_status
    $targetFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $targetPath
    $exists = Test-Path -LiteralPath $targetFullPath -PathType Leaf

    if ($sourceStatus -eq "final_generated" -and [string]$asset.generation_mode -ne $requiredGenerationMode) {
        $invalidGenerationModes.Add([pscustomobject]@{
            id = $asset.id
            target_path = $targetPath
            expected_generation_mode = $requiredGenerationMode
            actual_generation_mode = [string]$asset.generation_mode
        })
    }

    if ($sourceStatus -ne "missing" -and !$exists) {
        $missingTargets.Add([pscustomobject]@{
            id = $asset.id
            target_path = $targetPath
            source_status = $sourceStatus
        })
        continue
    }

    if ($sourceStatus -ne "missing" -and (Test-RequiresExportCoverage -TargetPath $targetPath)) {
        $exportPath = "res://$targetPath"
        if (!$exportedResourcePaths.Contains($exportPath)) {
            $missingExports.Add([pscustomobject]@{
                id = $asset.id
                target_path = $targetPath
                export_path = $exportPath
                source_status = $sourceStatus
            })
        }
    }

    if (!$exists) {
        continue
    }

    $actualHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $targetFullPath).Hash
    $hashRecords.Add([pscustomobject]@{
        id = $asset.id
        target_path = $targetPath
        sha256 = $actualHash
    })

    if ($asset.sha256 -and $actualHash -ne [string]$asset.sha256) {
        $hashMismatches.Add([pscustomobject]@{
            id = $asset.id
            target_path = $targetPath
            expected_sha256 = [string]$asset.sha256
            actual_sha256 = $actualHash
        })
    }
}

$uniqueHashRecords = $hashRecords | Sort-Object target_path -Unique
$duplicateGroups = @(
    $uniqueHashRecords |
        Group-Object sha256 |
        Where-Object { $_.Count -gt 1 } |
        ForEach-Object {
            [pscustomobject]@{
                sha256 = $_.Name
                target_paths = @($_.Group | Sort-Object target_path | ForEach-Object { $_.target_path })
            }
        }
)

$temporaryOrMissing = @(
    $assets |
        Where-Object { $_.source_status -in @("generic_temporary", "source_derived_temporary", "missing") } |
        Sort-Object id |
        ForEach-Object {
            [pscustomobject]@{
                id = $_.id
                role = $_.role
                target_path = $_.target_path
                source_status = $_.source_status
                prompt_id = $_.prompt_id
                final_required_before_release = [bool]$_.final_required_before_release
            }
        }
)

$missingFinal = @(
    $assets |
        Where-Object {
            [bool]$_.final_required_before_release -and
            $_.source_status -in @("generic_temporary", "source_derived_temporary", "missing")
        } |
        Sort-Object id |
        ForEach-Object {
            [pscustomobject]@{
                id = $_.id
                role = $_.role
                target_path = $_.target_path
                source_status = $_.source_status
                prompt_id = $_.prompt_id
            }
        }
)

$sourceStatusCounts = Get-StatusCounts -Assets $assets
$missingTargetItems = $missingTargets.ToArray()
$hashMismatchItems = $hashMismatches.ToArray()
$missingExportItems = $missingExports.ToArray()
$invalidGenerationModeItems = $invalidGenerationModes.ToArray()

$summary = @{
    manifest_path = $ManifestPath
    export_presets_path = $ExportPresetsPath
    required_generation_mode = $requiredGenerationMode
    asset_count = $assets.Count
    source_status_counts = $sourceStatusCounts
    missing_target_count = $missingTargets.Count
    hash_mismatch_count = $hashMismatches.Count
    missing_export_count = $missingExports.Count
    invalid_generation_mode_count = $invalidGenerationModes.Count
    duplicate_group_count = $duplicateGroups.Count
    temporary_or_missing_count = $temporaryOrMissing.Count
    missing_final_count = $missingFinal.Count
    missing_targets = @($missingTargetItems)
    hash_mismatches = @($hashMismatchItems)
    missing_exports = @($missingExportItems)
    invalid_generation_modes = @($invalidGenerationModeItems)
    duplicate_groups = @($duplicateGroups)
    temporary_or_missing = @($temporaryOrMissing)
    missing_final = @($missingFinal)
}

$json = $summary | ConvertTo-Json -Depth 8

if ($OutFile) {
    $outFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $OutFile
    $outDirectory = Split-Path -Parent $outFullPath
    if (!(Test-Path -LiteralPath $outDirectory -PathType Container)) {
        throw "Output directory does not exist: $outDirectory"
    }

    Set-Content -LiteralPath $outFullPath -Value $json -Encoding UTF8
}

$json

if ($FailOnHashMismatch -and $hashMismatches.Count -gt 0) {
    exit 1
}

if ($FailOnMissingExport -and $missingExports.Count -gt 0) {
    exit 1
}

if ($FailOnInvalidGenerationMode -and $invalidGenerationModes.Count -gt 0) {
    exit 1
}

if ($FailOnMissingFinal -and ($missingFinal.Count -gt 0 -or $missingTargets.Count -gt 0)) {
    exit 1
}

exit 0
