param(
    [string]$LedgerPath = 'docs\features\sts1-events\v20-final-gate-overlay.csv',
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

function Get-GateIdSetDetail {
    param([AllowEmptyCollection()][AllowNull()][string[]]$GateIds)

    if ($null -eq $GateIds -or $GateIds.Count -eq 0) {
        return ''
    }

    return (@($GateIds | Sort-Object { [int]($_ -replace '^O', '') }) -join ',')
}

$resolvedLedger = Resolve-RepoPath $LedgerPath
Add-Check -Name 'v20_overlay_file_exists' -Passed (Test-Path -LiteralPath $resolvedLedger) -Detail "overlay not found: $resolvedLedger"

if (-not (Test-Path -LiteralPath $resolvedLedger)) {
    foreach ($check in $checks) {
        Write-Output "$($check.Name) status=$(if ($check.Passed) { 'pass' } else { 'fail' })"
    }

    Write-Output "checks=$($checks.Count)"
    Write-Output "mismatches=$($mismatches.Count)"
    if ($FailOnMismatch) {
        exit 1
    }

    exit 0
}

$rows = @(Import-Csv -LiteralPath $resolvedLedger)
$expectedIds = @(76..84 | ForEach-Object { "O$_" })
$actualIds = @($rows | Select-Object -ExpandProperty gate_id)
$allowedStatuses = @(
    'documentation-in-progress',
    'current-pending',
    'static-pass'
)

$expectedStatuses = @{
    O76 = 'documentation-in-progress'
    O77 = 'documentation-in-progress'
    O78 = 'documentation-in-progress'
    O79 = 'documentation-in-progress'
    O80 = 'documentation-in-progress'
    O81 = 'current-pending'
    O82 = 'static-pass'
    O83 = 'static-pass'
    O84 = 'static-pass'
}

$rowNeedles = @(
    [pscustomobject]@{ GateId = 'O76'; Field = 'requirement'; Needle = 'current-validation updated' },
    [pscustomobject]@{ GateId = 'O77'; Field = 'requirement'; Needle = 'status-board updated' },
    [pscustomobject]@{ GateId = 'O78'; Field = 'requirement'; Needle = 'monthly review updated' },
    [pscustomobject]@{ GateId = 'O79'; Field = 'requirement'; Needle = 'handoff docs updated' },
    [pscustomobject]@{ GateId = 'O80'; Field = 'requirement'; Needle = 'owner actions listed' },
    [pscustomobject]@{ GateId = 'O81'; Field = 'requirement'; Needle = 'no unsupported commit/push' },
    [pscustomobject]@{ GateId = 'O82'; Field = 'requirement'; Needle = 'release-ready claim absent unless gates pass' },
    [pscustomobject]@{ GateId = 'O83'; Field = 'requirement'; Needle = 'final summary states blocked gates honestly' },
    [pscustomobject]@{ GateId = 'O84'; Field = 'requirement'; Needle = 'next-run start point lists unresolved gates only' },
    [pscustomobject]@{ GateId = 'O81'; Field = 'evidence'; Needle = 'Coordination pause forbids commit/push from this thread' },
    [pscustomobject]@{ GateId = 'O82'; Field = 'evidence'; Needle = 'release/live ready no' },
    [pscustomobject]@{ GateId = 'O83'; Field = 'evidence'; Needle = 'hard-stop-blocker-report-v20-coordination-pause-20260617.md' },
    [pscustomobject]@{ GateId = 'O84'; Field = 'evidence'; Needle = 'hard-stop-blocker-report-v20-coordination-pause-20260617.md' },
    [pscustomobject]@{ GateId = 'O84'; Field = 'next_action'; Needle = 'post-pause validation' }
)

Add-Check -Name 'v20_overlay_gate_count' -Passed ($rows.Count -eq $expectedIds.Count) -Detail "expected $($expectedIds.Count) rows but found $($rows.Count)"
Add-Check -Name 'v20_overlay_exact_gate_ids' -Passed ((Get-GateIdSetDetail $actualIds) -eq (Get-GateIdSetDetail $expectedIds)) -Detail "expected $(Get-GateIdSetDetail $expectedIds) but found $(Get-GateIdSetDetail $actualIds)"
Add-Check -Name 'v20_overlay_single_group' -Passed (@($rows | Where-Object { $_.gate_group -ne 'O76-O84' }).Count -eq 0) -Detail 'all v20 overlay rows must use O76-O84 gate group'
Add-Check -Name 'v20_overlay_allowed_statuses' -Passed (@($rows | Where-Object { $allowedStatuses -notcontains $_.current_status }).Count -eq 0) -Detail 'unexpected current_status value in overlay'
Add-Check -Name 'v20_overlay_no_runtime_completion_status' -Passed (@($rows | Where-Object { $_.current_status -in @('current-pass', 'manual-verified', 'release-ready') }).Count -eq 0) -Detail 'final docs overlay must not claim runtime or release completion'

foreach ($entry in $expectedStatuses.GetEnumerator()) {
    $row = $rows | Where-Object { $_.gate_id -eq $entry.Key } | Select-Object -First 1
    Add-Check -Name "v20_overlay_status_$($entry.Key)" -Passed ($null -ne $row -and $row.current_status -eq $entry.Value) -Detail "expected $($entry.Key) status $($entry.Value)"
}

foreach ($needle in $rowNeedles) {
    $row = $rows | Where-Object { $_.gate_id -eq $needle.GateId } | Select-Object -First 1
    $value = if ($null -eq $row) { '' } else { "$($row.($needle.Field))" }
    Add-Check -Name "v20_overlay_$($needle.GateId)_$($needle.Field)" -Passed ($value.Contains($needle.Needle)) -Detail "expected $($needle.Field) to contain '$($needle.Needle)'"
}

foreach ($check in $checks) {
    Write-Output "$($check.Name) status=$(if ($check.Passed) { 'pass' } else { 'fail' })"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

if ($OutFile) {
    $resolvedOutFile = if ([System.IO.Path]::IsPathRooted($OutFile)) {
        [System.IO.Path]::GetFullPath($OutFile)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutFile))
    }

    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    [pscustomobject]@{
        LedgerPath = $resolvedLedger
        Checks = $checks
        Mismatches = $mismatches
    } | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
