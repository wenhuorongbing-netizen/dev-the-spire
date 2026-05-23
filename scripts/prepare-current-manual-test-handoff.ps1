param(
    [string]$EvidenceRoot,

    [switch]$SkipPendingVerifier
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'

function New-DirectoryIfMissing {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Assert-PathInside {
    param(
        [Parameter(Mandatory = $true)][string]$Child,
        [Parameter(Mandatory = $true)][string]$Parent,
        [Parameter(Mandatory = $true)][string]$Label
    )

    $childFull = [System.IO.Path]::GetFullPath($Child).TrimEnd('\', '/')
    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\', '/')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    if ($childFull.Equals($parentFull, $comparison)) {
        return
    }

    if (-not $childFull.StartsWith($parentFull + '\', $comparison)) {
        throw "$Label path is outside expected root. Path: $childFull Root: $parentFull"
    }
}

function Get-HandoffRoot {
    param([string]$RequestedPath)

    New-DirectoryIfMissing -Path $runtimeRoot

    if ([string]::IsNullOrWhiteSpace($RequestedPath)) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "manual-test-handoff-$stamp"))
    }

    $candidate = if ([System.IO.Path]::IsPathRooted($RequestedPath)) {
        $RequestedPath
    } else {
        Join-Path $repoRoot $RequestedPath
    }

    return [System.IO.Path]::GetFullPath($candidate)
}

function Get-PowerShellExecutable {
    $processPath = (Get-Process -Id $PID).Path
    if ($processPath -and (Test-Path -LiteralPath $processPath)) {
        return $processPath
    }

    if ($PSVersionTable.PSEdition -eq 'Core') {
        return 'pwsh'
    }

    return 'powershell.exe'
}

function Invoke-RepoScript {
    param(
        [Parameter(Mandatory = $true)][string]$ScriptName,
        [Parameter(Mandatory = $true)][string[]]$ArgumentList
    )

    $scriptPath = Join-Path $PSScriptRoot $ScriptName
    if (-not (Test-Path -LiteralPath $scriptPath -PathType Leaf)) {
        throw "Missing helper script: $scriptPath"
    }

    $powerShellExe = Get-PowerShellExecutable
    $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $scriptPath) + $ArgumentList
    $output = & $powerShellExe @childArgs 2>&1
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode) {
        $exitCode = 0
    }

    return [pscustomobject]@{
        Script = $scriptPath
        ExitCode = [int]$exitCode
        Output = @($output | ForEach-Object { $_.ToString() })
    }
}

function Assert-Success {
    param([Parameter(Mandatory = $true)]$Result)

    if ($Result.Output.Count -gt 0) {
        $Result.Output | Write-Output
    }

    if ($Result.ExitCode -ne 0) {
        throw "$($Result.Script) failed with exit code $($Result.ExitCode)."
    }
}

$handoffRoot = Get-HandoffRoot -RequestedPath $EvidenceRoot
Assert-PathInside -Child $handoffRoot -Parent $runtimeRoot -Label 'EvidenceRoot'
New-DirectoryIfMissing -Path $handoffRoot

$summary = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    HandoffRoot = $handoffRoot
    NoLaunch = $true
    PendingVerifierChecked = -not [bool]$SkipPendingVerifier
    Sections = [ordered]@{}
    Notice = 'No game was launched. These folders are templates only; live rows remain pending until filled with screenshots, logs, and notes.'
}

Assert-Success (Invoke-RepoScript -ScriptName 'collect-release-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'release')
    ))
$summary.Sections['release'] = Join-Path $handoffRoot 'release'

Assert-Success (Invoke-RepoScript -ScriptName 'collect-vakuu-fight-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'vakuu')
    ))
$summary.Sections['vakuu'] = Join-Path $handoffRoot 'vakuu'

Assert-Success (Invoke-RepoScript -ScriptName 'collect-preview-tools-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'preview-tools')
    ))
$summary.Sections['preview-tools'] = Join-Path $handoffRoot 'preview-tools'

Assert-Success (Invoke-RepoScript -ScriptName 'collect-coop-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'coop')
    ))
$summary.Sections['coop'] = Join-Path $handoffRoot 'coop'

$ancientUiRoot = Join-Path $handoffRoot 'ancient-ui'
foreach ($ancient in @('URDA', 'MORVI', 'LOTHA', 'VAKUU')) {
    Assert-Success (Invoke-RepoScript -ScriptName 'collect-ancient-ui-evidence.ps1' -ArgumentList @(
            '-Mode', 'Prepare',
            '-Ancient', $ancient,
            '-EvidenceDir', (Join-Path $ancientUiRoot $ancient),
            '-NoPreflight'
        ))
}

Assert-Success (Invoke-RepoScript -ScriptName 'collect-ancient-ui-evidence.ps1' -ArgumentList @(
        '-Mode', 'Prepare',
        '-Ancient', 'VAKUU',
        '-ForceVakuuFight',
        '-EvidenceDir', (Join-Path $ancientUiRoot 'VAKUU-FIGHT'),
        '-NoPreflight'
    ))
$summary.Sections['ancient-ui'] = $ancientUiRoot

if (-not $SkipPendingVerifier) {
    $verifyResult = Invoke-RepoScript -ScriptName 'verify-spire-plus-release-evidence.ps1' -ArgumentList @(
        '-EvidenceRoot', (Join-Path $handoffRoot 'release'),
        '-ManifestPath', (Join-Path $handoffRoot 'release\release-evidence-manifest.json')
    )

    if ($verifyResult.ExitCode -eq 0) {
        throw 'Pending release evidence unexpectedly passed verification.'
    }

    $outputText = $verifyResult.Output -join [Environment]::NewLine
    if ($outputText -notmatch 'FailureCount' -or $outputText -notmatch 'pending') {
        throw 'Pending release evidence failed for an unexpected reason; expected pending-row failures.'
    }

    $summary.PendingVerifierExitCode = $verifyResult.ExitCode
    $summary.PendingVerifierExpectedFailure = $true
}

$summaryPath = Join-Path $handoffRoot 'handoff-summary.json'
$summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

$readme = @(
    '# Spire Plus Manual Test Handoff',
    '',
    "Created: $($summary.CreatedAt)",
    '',
    'This folder was generated without launching the game. It is a template set for manual testing, not proof that any live row passed.',
    '',
    '## Folders',
    '',
    '- `release/`: verifier-readable release evidence manifest and row folders.',
    '- `ancient-ui/`: focused clicked-UI capture plans for Urda, Morvi, Lotha, Vakuu normal, and Vakuu force-fight.',
    '- `vakuu/`: focused hidden Vakuu fight gameplay/save-load rows.',
    '- `preview-tools/`: Crystal Sphere and transform preview rows.',
    '- `coop/`: two-client co-op rows.',
    '',
    'Run `scripts\verify-spire-plus-release-evidence.ps1` only after the release rows have live screenshots, logs, and notes. Pending rows are expected to fail closed.'
) -join [Environment]::NewLine
$readme | Set-Content -LiteralPath (Join-Path $handoffRoot 'README.md') -Encoding UTF8

Write-Output "Prepared complete manual-test handoff under $handoffRoot"
Write-Output "Summary: $summaryPath"
Write-Output 'No game was launched. All live rows remain pending.'
