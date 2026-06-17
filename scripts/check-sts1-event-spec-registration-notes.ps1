param(
    [string]$SpecRoot = 'docs\features\sts1-events\event-specs',
    [int]$ExpectedSpecCount = 50,
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$mismatches = [System.Collections.Generic.List[string]]::new()
$filesWithOneRegistrationNote = 0
$staleTermHits = [System.Collections.Generic.List[object]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

$resolvedSpecRoot = Resolve-RepoPath $SpecRoot
if (-not (Test-Path -LiteralPath $resolvedSpecRoot)) {
    Write-Error "StS1 event spec root not found: $resolvedSpecRoot"
    exit 1
}

$specFiles = @(Get-ChildItem -LiteralPath $resolvedSpecRoot -File -Filter '*.md' |
    Where-Object { $_.Name -ne 'README.md' } |
    Sort-Object Name)

if ($specFiles.Count -ne $ExpectedSpecCount) {
    $mismatches.Add("Expected $ExpectedSpecCount per-event spec files, found $($specFiles.Count)") | Out-Null
}

$stalePattern = 'RegisterActEvent|RegisterSharedEvent|Act1Model|Act2Model|Act3Model|ModEventTemplate'
$currentRegistrationPattern = 'Sts1EventRegistrationService|content\.(ActEvent|SharedEvent)<|special stub only|Compile-excluded and not registered'

foreach ($file in $specFiles) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $registrationMatches = [regex]::Matches($text, '(?m)^[-*]\s+\*\*Registration:\*\*.*$')

    if ($registrationMatches.Count -eq 1) {
        $filesWithOneRegistrationNote++
        $line = $registrationMatches[0].Value
        if ($line -notmatch $currentRegistrationPattern) {
            $mismatches.Add("$($file.Name) registration note does not reference current registration authority or explicit non-registration") | Out-Null
        }
    } else {
        $mismatches.Add("$($file.Name) has $($registrationMatches.Count) canonical registration notes; expected 1") | Out-Null
    }

    foreach ($match in [regex]::Matches($text, $stalePattern)) {
        $staleTermHits.Add([pscustomobject]@{
            File = $file.Name
            Term = $match.Value
        }) | Out-Null
    }
}

if ($staleTermHits.Count -gt 0) {
    $mismatches.Add("Found $($staleTermHits.Count) stale registration/API terms in event specs") | Out-Null
}

$report = [pscustomobject]@{
    SpecFiles = $specFiles.Count
    FilesWithOneRegistrationNote = $filesWithOneRegistrationNote
    StaleTermHits = $staleTermHits
    Mismatches = $mismatches
}

Write-Output "spec_files=$($specFiles.Count)"
Write-Output "files_with_one_registration_note=$filesWithOneRegistrationNote"
Write-Output "stale_term_hits=$($staleTermHits.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($hit in $staleTermHits) {
    Write-Output "stale $($hit.File): $($hit.Term)"
}

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
