param(
    [switch]$FailOnLocalizationMissing,
    [string]$OutFile
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$steps = [System.Collections.Generic.List[object]]::new()
$failures = [System.Collections.Generic.List[string]]::new()

function Invoke-StaticStep {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$ScriptName,
        [hashtable]$Parameters = @{}
    )

    $scriptPath = Join-Path $PSScriptRoot $ScriptName
    if (-not (Test-Path -LiteralPath $scriptPath)) {
        $failures.Add("${Name}: script not found: $scriptPath") | Out-Null
        $steps.Add([pscustomobject]@{
            Name = $Name
            ExitCode = 127
            Output = @()
        }) | Out-Null
        return
    }

    Write-Output "== $Name =="
    $global:LASTEXITCODE = 0
    $output = @(& $scriptPath @Parameters 2>&1)
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode) {
        $exitCode = 0
    }

    foreach ($line in $output) {
        Write-Output $line
    }

    $steps.Add([pscustomobject]@{
        Name = $Name
        ExitCode = $exitCode
        Output = $output
    }) | Out-Null

    if ($exitCode -ne 0) {
        $failures.Add("${Name}: exit code $exitCode") | Out-Null
    }
}

Invoke-StaticStep -Name 'registry-shape' -ScriptName 'check-sts1-event-registry-shape.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'enabled-log-canary-expected-shape' -ScriptName 'check-sts1-enabled-mode-runtime-log.ps1' -Parameters @{ Mode = 'CanaryOnly'; PrintExpected = $true }
Invoke-StaticStep -Name 'enabled-log-additive-batch1-expected-shape' -ScriptName 'check-sts1-enabled-mode-runtime-log.ps1' -Parameters @{ Mode = 'AdditiveBatch1'; PrintExpected = $true }
Invoke-StaticStep -Name 'current-doc-claims' -ScriptName 'check-sts1-event-current-doc-claims.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'static-file-hygiene' -ScriptName 'check-sts1-static-file-hygiene.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'v19-gate-ledger' -ScriptName 'check-sts1-v19-gate-ledger.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'v20-final-gate-overlay' -ScriptName 'check-sts1-v20-final-gate-overlay.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'v19-subagent-coverage' -ScriptName 'check-sts1-v19-subagent-coverage.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'event-spec-registration-notes' -ScriptName 'check-sts1-event-spec-registration-notes.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'feature-gates' -ScriptName 'check-sts1-event-feature-gates.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'parity-blockers' -ScriptName 'check-sts1-event-parity-blockers.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'asset-safety' -ScriptName 'check-sts1-event-asset-safety.ps1' -Parameters @{ FailOnMismatch = $true }
Invoke-StaticStep -Name 'multiplayer-shape' -ScriptName 'check-sts1-event-multiplayer-shape.ps1' -Parameters @{ FailOnMismatch = $true }

$localizationParameters = if ($FailOnLocalizationMissing) { @{ FailOnMissing = $true } } else { @{} }
Invoke-StaticStep -Name 'localization-source-keys' -ScriptName 'check-sts1-localization-source-keys.ps1' -Parameters $localizationParameters
Invoke-StaticStep -Name 'localization-gap-baseline' -ScriptName 'check-sts1-localization-gap-baseline.ps1' -Parameters @{ FailOnMismatch = $true }

$localizationStep = $steps | Where-Object { $_.Name -eq 'localization-source-keys' } | Select-Object -First 1
$missingLocalizationLine = @($localizationStep.Output | Where-Object { "$_" -like 'missing_source_referenced_keys=*' } | Select-Object -First 1)
$missingLocalizationCount = $null
if ($missingLocalizationLine.Count -gt 0 -and "$($missingLocalizationLine[0])" -match '^missing_source_referenced_keys=(\d+)$') {
    $missingLocalizationCount = [int]$Matches[1]
}

if (-not $FailOnLocalizationMissing -and $missingLocalizationCount -gt 0) {
    Write-Output "known_localization_gap=$missingLocalizationCount"
    Write-Output 'localization_gap_status=known-gap-not-failing'
}

$report = [pscustomobject]@{
    Steps = $steps
    Failures = $failures
    MissingLocalizationKeys = $missingLocalizationCount
    FailOnLocalizationMissing = [bool]$FailOnLocalizationMissing
}

Write-Output "static_suite_steps=$($steps.Count)"
Write-Output "static_suite_failures=$($failures.Count)"

foreach ($failure in $failures) {
    Write-Output "failure $failure"
}

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

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($failures.Count -gt 0) {
    exit 1
}
