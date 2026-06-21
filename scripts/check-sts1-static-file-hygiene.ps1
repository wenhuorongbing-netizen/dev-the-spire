param(
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

function ConvertTo-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith($repoRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($repoRoot.Length).TrimStart('\', '/')
    }

    return $fullPath
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

$explicitRelativePaths = @(
    'AGENTS.md',
    'PROJECT_STATE.md',
    'README.md',
    'docs\goal.md',
    'docs\doc-restructure-spec.md',
    'docs\restructure.md',
    'docs\PROJECT_MAP.md',
    'docs\architecture\patch-boundaries.md',
    'docs\BETA_COMPATIBILITY.md',
    'docs\REMOTE_DEVELOPMENT_SETUP.md',
    'docs\audits\v0.106-source-api-drift.md',
    'docs\README.md',
    'docs\platform-testing.md',
    'docs\test-plan.md',
    'docs\migration.md',
    'docs\goals\migration.md',
    'docs\goals\debug.md',
    'docs\goals\refactor.md',
    'docs\goals\sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md',
    'docs\integrations\ritsulib.md',
    'docs\features\ritsulib-migration\README.md',
    'docs\features\ritsulib-migration\monthly-dev-spec.md',
    'docs\features\ritsulib-migration\batch-4c-candidates.md',
    'docs\features\ritsulib-migration\runtime-hard-block-report-20260531.md',
    'docs\goals\m5-revision-l-runtime-hard-blocker.md',
    'docs\goals\m5-revision-l-runtime-smoke-plan.md',
    'docs\goals\m5-revision-l-final-report.md',
    'docs\goals\m5-revision-l-owner-review-packet.md',
    'docs\goals\m5-revision-l-dirty-ledger.md',
    'docs\goals\m5-revision-l-commit-slices.md',
    'docs\goals\m5-revision-l-warning-ledger.md',
    'docs\goals\m5-revision-m-final-report.md',
    'docs\goals\m5-revision-m-owner-review-packet.md',
    'docs\goals\m5-revision-m-runtime-drift-report.md',
    'docs\goals\m5-revision-m-patch-failure-ledger.md',
    'docs\goals\m5-revision-m-version-decision.md',
    'docs\goals\m5-revision-m-commit-slices.md',
    'docs\goals\warning-ledger.md',
    'docs\goals\overnight-run-ledger.md',
    'docs\goals\overnight-run-status.md',
    'docs\reviews\overnight-run-20260529.md',
    'docs\reviews\refactor-qa-20260602.md',
    'docs\reviews\refactor-qa-20260602-round2.md',
    'docs\features\ritsulib-migration\runtime-smoke-checklist.md',
    'docs\features\ritsulib-migration\next-overnight-run.md',
    'docs\test-ready-development-goal.md',
    'docs\dev-environment.md',
    'docs\issues.md',
    'docs\review.md',
    'docs\toreview.md',
    'docs\issues\ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md',
    'docs\reviews\current-validation.md',
    'docs\private-beta-verification-handoff.md',
    'docs\release-checklist.md',
    'docs\release-evidence-status.md',
    'docs\private-beta-release-completion-audit.md',
    'docs\test-ready-completion-audit.md',
    'docs\goals\event.md',
    'scripts\README.md',
    'scripts\check-spire-plus-autoslay-packet.ps1',
    'scripts\spire-plus-live-session.ps1'
)

$paths = [System.Collections.Generic.List[string]]::new()

foreach ($relativePath in $explicitRelativePaths) {
    $resolvedPath = Resolve-RepoPath $relativePath
    if (Test-Path -LiteralPath $resolvedPath) {
        $paths.Add($resolvedPath) | Out-Null
    }
}

$sts1DocsRoot = Resolve-RepoPath 'docs\features\sts1-events'
if (Test-Path -LiteralPath $sts1DocsRoot) {
    Get-ChildItem -LiteralPath $sts1DocsRoot -Recurse -File |
        Where-Object { $_.Extension -in @('.md', '.csv') } |
        ForEach-Object { $paths.Add($_.FullName) | Out-Null }
}

Get-ChildItem -LiteralPath (Resolve-RepoPath 'scripts') -Filter 'check-sts1*.ps1' -File |
    ForEach-Object { $paths.Add($_.FullName) | Out-Null }

$uniquePaths = @($paths | Sort-Object -Unique)
$trailingWhitespaceHits = [System.Collections.Generic.List[string]]::new()
$missingFinalNewlineFiles = [System.Collections.Generic.List[string]]::new()
$nulByteFiles = [System.Collections.Generic.List[string]]::new()
$replacementCharHits = [System.Collections.Generic.List[string]]::new()
$statusBoardPath = Resolve-RepoPath 'docs\features\sts1-events\status-board.md'
$statusBoardText = if (Test-Path -LiteralPath $statusBoardPath) { [System.IO.File]::ReadAllText($statusBoardPath) } else { '' }
$statusBoardExpectedChain = 'planned -> spec-drafted -> wiki-verified -> api-verified -> implemented -> compiled -> test-guarded -> asset-mapped -> loc-render-verified -> manual-verified -> save-load-verified'
$statusBoardUnicodeArrow = [string][char]0x2192
$statusBoardReplacementChar = [string][char]0xFFFD

foreach ($path in $uniquePaths) {
    $relativePath = ConvertTo-RepoRelativePath $path
    $lines = [System.IO.File]::ReadAllLines($path)

    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ([regex]::IsMatch($lines[$i], '[ \t]+$')) {
            $lineNumber = $i + 1
            $trailingWhitespaceHits.Add("${relativePath}:${lineNumber}") | Out-Null
        }

        if ($lines[$i].Contains($statusBoardReplacementChar)) {
            $lineNumber = $i + 1
            $replacementCharHits.Add("${relativePath}:${lineNumber}") | Out-Null
        }
    }

    $bytes = [System.IO.File]::ReadAllBytes($path)
    if ($bytes.Length -gt 0 -and $bytes[$bytes.Length - 1] -ne 10) {
        $missingFinalNewlineFiles.Add($relativePath) | Out-Null
    }

    if ([array]::IndexOf($bytes, [byte]0) -ge 0) {
        $nulByteFiles.Add($relativePath) | Out-Null
    }
}

Add-Check -Name 'sts1_hygiene_scanned_files_present' -Passed ($uniquePaths.Count -gt 0) -Detail "scanned $($uniquePaths.Count) files"
Add-Check -Name 'sts1_hygiene_scans_ritsu_migration_readme' -Passed ($uniquePaths -contains (Resolve-RepoPath 'docs\features\ritsulib-migration\README.md')) -Detail 'RitsuLib migration README must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_scans_ritsu_monthly_spec' -Passed ($uniquePaths -contains (Resolve-RepoPath 'docs\features\ritsulib-migration\monthly-dev-spec.md')) -Detail 'RitsuLib monthly dev spec must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_scans_ritsu_batch4c' -Passed ($uniquePaths -contains (Resolve-RepoPath 'docs\features\ritsulib-migration\batch-4c-candidates.md')) -Detail 'RitsuLib Batch 4c proposal must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_scans_ritsu_runtime_checklist' -Passed ($uniquePaths -contains (Resolve-RepoPath 'docs\features\ritsulib-migration\runtime-smoke-checklist.md')) -Detail 'RitsuLib runtime smoke checklist must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_scans_ritsu_next_overnight' -Passed ($uniquePaths -contains (Resolve-RepoPath 'docs\features\ritsulib-migration\next-overnight-run.md')) -Detail 'RitsuLib next overnight run plan must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_scans_autoslay_packet_verifier' -Passed ($uniquePaths -contains (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1')) -Detail 'AutoSlay packet verifier must be included in static-file hygiene scope'
Add-Check -Name 'sts1_hygiene_no_trailing_whitespace' -Passed ($trailingWhitespaceHits.Count -eq 0) -Detail "trailing whitespace hits: $($trailingWhitespaceHits -join ', ')"
Add-Check -Name 'sts1_hygiene_final_newline' -Passed ($missingFinalNewlineFiles.Count -eq 0) -Detail "missing final newline files: $($missingFinalNewlineFiles -join ', ')"
Add-Check -Name 'sts1_hygiene_no_nul_bytes' -Passed ($nulByteFiles.Count -eq 0) -Detail "NUL byte files: $($nulByteFiles -join ', ')"
Add-Check -Name 'sts1_hygiene_no_replacement_chars' -Passed ($replacementCharHits.Count -eq 0) -Detail "replacement character hits: $($replacementCharHits -join ', ')"
Add-Check -Name 'sts1_hygiene_status_board_status_chain_ascii' -Passed $statusBoardText.Contains($statusBoardExpectedChain) -Detail 'status-board allowed-status progression must use ASCII arrows for console-safe rendering'
Add-Check -Name 'sts1_hygiene_status_board_no_arrow_mojibake' -Passed (-not $statusBoardText.Contains($statusBoardUnicodeArrow) -and -not $statusBoardText.Contains($statusBoardReplacementChar)) -Detail 'status-board must not contain Unicode arrows or replacement characters'

$report = [pscustomobject]@{
    ScannedFiles = @($uniquePaths | ForEach-Object { ConvertTo-RepoRelativePath $_ })
    TrailingWhitespaceHits = $trailingWhitespaceHits
    MissingFinalNewlineFiles = $missingFinalNewlineFiles
    NulByteFiles = $nulByteFiles
    ReplacementCharHits = $replacementCharHits
    StatusBoardExpectedChain = $statusBoardExpectedChain
    Checks = $checks
    Mismatches = $mismatches
}

Write-Output "scanned_files=$($uniquePaths.Count)"
Write-Output "trailing_whitespace_hits=$($trailingWhitespaceHits.Count)"
Write-Output "missing_final_newline_files=$($missingFinalNewlineFiles.Count)"
Write-Output "nul_byte_files=$($nulByteFiles.Count)"
Write-Output "replacement_char_hits=$($replacementCharHits.Count)"

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
