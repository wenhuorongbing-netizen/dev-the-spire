param(
    [string]$EvidenceDir,

    [switch]$Launch,

    [switch]$NoLaunch,

    [switch]$MoveOtherMods,

    [switch]$MoveCurrentRuns
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'spire-plus-package-evidence.ps1')
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$liveSessionScript = Join-Path $PSScriptRoot 'spire-plus-live-session.ps1'

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

function Get-EvidenceFullPath {
    param([string]$RequestedPath)

    New-DirectoryIfMissing -Path $runtimeRoot

    if ([string]::IsNullOrWhiteSpace($RequestedPath)) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "vakuu-fight-evidence-$stamp"))
    }

    $candidate = if ([System.IO.Path]::IsPathRooted($RequestedPath)) {
        $RequestedPath
    } else {
        Join-Path $repoRoot $RequestedPath
    }

    return [System.IO.Path]::GetFullPath($candidate)
}

function Save-Json {
    param(
        [Parameter(Mandatory = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $InputObject | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Format-DisplayToken {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value -match '[\s`"]') {
        return '"' + ($Value -replace '"', '`"') + '"'
    }

    return $Value
}

function Format-DisplayCommand {
    param([Parameter(Mandatory = $true)][string[]]$Tokens)

    return (($Tokens | ForEach-Object { Format-DisplayToken -Value $_ }) -join ' ')
}

function Get-HashRow {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $fullPath = Join-Path $repoRoot $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        return [ordered]@{
            Path = $RelativePath
            Exists = $false
            Sha256 = $null
            Length = $null
        }
    }

    $item = Get-Item -LiteralPath $fullPath
    return [ordered]@{
        Path = $RelativePath
        Exists = $true
        Sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash
        Length = $item.Length
    }
}

function Set-ProcessEnvironment {
    param([Parameter(Mandatory = $true)]$Variables)

    $previous = @{}
    foreach ($entry in $Variables.GetEnumerator()) {
        $previous[$entry.Key] = [Environment]::GetEnvironmentVariable($entry.Key, 'Process')
        [Environment]::SetEnvironmentVariable($entry.Key, [string]$entry.Value, 'Process')
    }

    return $previous
}

function Restore-ProcessEnvironment {
    param([Parameter(Mandatory = $true)]$PreviousValues)

    foreach ($entry in $PreviousValues.GetEnumerator()) {
        [Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, 'Process')
    }
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

function Invoke-PowerShellFile {
    param(
        [Parameter(Mandatory = $true)][string]$ScriptPath,
        [Parameter(Mandatory = $true)][string[]]$ArgumentList
    )

    $powerShellExe = Get-PowerShellExecutable
    $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $ScriptPath) + $ArgumentList
    $output = & $powerShellExe @childArgs 2>&1
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode) {
        $exitCode = 0
    }

    return [pscustomobject]@{
        ExitCode = [int]$exitCode
        Output = @($output | ForEach-Object { $_.ToString() })
    }
}

if ($Launch -and $NoLaunch) {
    throw 'Pass only one of -Launch or -NoLaunch.'
}

$evidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir
Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'
New-DirectoryIfMissing -Path $evidenceFull
foreach ($subdir in @('screenshots', 'notes', 'save-load', 'victory', 'failure-death')) {
    New-DirectoryIfMissing -Path (Join-Path $evidenceFull $subdir)
}

$forceEnvironment = [ordered]@{
    SPIREPLUS_FORCE_ANCIENT = 'VAKUU'
    EZMB_FORCE_ANCIENT = 'VAKUU'
    SPIREPLUS_FORCE_VAKUU_FIGHT = '1'
    EZMB_FORCE_VAKUU_FIGHT = '1'
    SPIREPLUS_ENABLE_VAKUU_FIGHT = '1'
    EZMB_ENABLE_VAKUU_FIGHT = '1'
    SPIREPLUS_RELEASE_EVIDENCE_LOG = '1'
    EZMB_RELEASE_EVIDENCE_LOG = '1'
}

$selfTokens = @('.\scripts\collect-vakuu-fight-evidence.ps1')
if ($EvidenceDir) { $selfTokens += @('-EvidenceDir', $evidenceFull) }
if ($Launch) { $selfTokens += '-Launch' }
if ($NoLaunch) { $selfTokens += '-NoLaunch' }
if ($MoveOtherMods) { $selfTokens += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $selfTokens += '-MoveCurrentRuns' }

$manualRows = @(
    [ordered]@{
        Id = 'vakuu-fight-start'
        Feature = 'Vakuu fight'
        Status = 'pending'
        RequiredEvidence = @('screenshots/01-fight-start.png', 'godot.log', 'godot-log-audit.json')
        Notes = 'Confirm the dedicated Vakuu monster and encounter scene appear, not a placeholder fight.'
    },
    [ordered]@{
        Id = 'vakuu-contract-turns'
        Feature = 'Vakuu fight'
        Status = 'pending'
        RequiredEvidence = @('screenshots/02-contract-choice.png', 'notes/contract-turns.md')
        Notes = 'Confirm Contract choices appear on the expected turns and do not softlock the hand.'
    },
    [ordered]@{
        Id = 'vakuu-locks-blood-debt'
        Feature = 'Vakuu fight'
        Status = 'pending'
        RequiredEvidence = @('screenshots/03-locks-debt.png', 'notes/locks-blood-debt.md')
        Notes = 'Record broken locks, Blood Debt, Gold/HP settlement, and lethal-hit lock counting.'
    },
    [ordered]@{
        Id = 'vakuu-victory-no-black-screen'
        Feature = 'Vakuu fight'
        Status = 'pending'
        RequiredEvidence = @('victory/01-victory-return.png', 'victory/no-black-screen.md', 'godot.log', 'godot-log-audit.json')
        Notes = 'Confirm victory returns to a usable screen and does not black-screen or softlock.'
    },
    [ordered]@{
        Id = 'vakuu-non-vakuu-rewards'
        Feature = 'Vakuu rewards'
        Status = 'pending'
        RequiredEvidence = @('victory/02-reward-choices.png', 'notes/reward-choices.md')
        Notes = 'Confirm victory offers non-Vakuu Ancient rewards and no normal combat card reward.'
    },
    [ordered]@{
        Id = 'vakuu-failure-death'
        Feature = 'Vakuu fight'
        Status = 'pending'
        RequiredEvidence = @('failure-death/failure-or-death.png', 'failure-death/result-note.md', 'godot.log', 'godot-log-audit.json')
        Notes = 'Record whether loss/death exits cleanly and whether the run state remains coherent.'
    },
    [ordered]@{
        Id = 'vakuu-active-save-load'
        Feature = 'Vakuu save-load'
        Status = 'pending'
        RequiredEvidence = @('save-load/before-active-save.png', 'save-load/after-active-load.png', 'save-load/active-save-load.md', 'godot.log', 'godot-log-audit.json')
        Notes = 'Save during active child combat, reload, and confirm parent/event/combat state.'
    },
    [ordered]@{
        Id = 'vakuu-prefinished-save-load'
        Feature = 'Vakuu save-load'
        Status = 'pending'
        RequiredEvidence = @('save-load/before-prefinished-save.png', 'save-load/after-prefinished-load.png', 'save-load/prefinished-save-load.md', 'godot.log', 'godot-log-audit.json')
        Notes = 'Save around the post-combat reward/return path and confirm no duplicate Ancient heal or stale parent event.'
    }
)

$gitEvidence = Get-SpirePlusGitEvidence -RepoRoot $repoRoot
$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'vakuu-fight-evidence'
    RepositoryRoot = $repoRoot
    GitHead = $gitEvidence.Head
    GitStatusShort = $gitEvidence.StatusShort
    GitBranchStatus = $gitEvidence.BranchStatus
    GitUpstream = $gitEvidence.Upstream
    GitUpstreamHead = $gitEvidence.UpstreamHead
    GitPushedHead = $gitEvidence.PushedHead
    GitHeadMatchesUpstream = $gitEvidence.HeadMatchesUpstream
    Git = $gitEvidence
    LaunchRequested = [bool]$Launch
    NoLaunch = -not [bool]$Launch
    ForceEnvironment = $forceEnvironment
    PreferredDevConsoleCommand = 'spireplus_test_ancient VAKUU confirm fight'
    Requirement = 'Vakuu victory/no-black-screen, failure/death, active save-load, prefinished save-load, and reward return proof.'
}

$packageHashes = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Files = @(
        Get-HashRow -RelativePath 'EZMicroBalance.json'
        foreach ($artifactPath in (Get-SpirePlusPackageArtifactRelativePaths -RepoRoot $repoRoot)) {
            Get-HashRow -RelativePath $artifactPath
        }
    )
}

$instructions = @(
    '# Vakuu Fight Evidence',
    '',
    'Status: pending',
    '',
    'This helper prepares a focused evidence folder for the hidden Vakuu fight. It does not prove the fight by itself.',
    '',
    'Recommended DevConsole command after launch:',
    '',
    '    spireplus_test_ancient VAKUU confirm fight',
    '',
    'Required checks:',
    '- dedicated Vakuu monster and scene appear',
    '- Contract turns and Stolen Vault locks behave as designed',
    '- Blood Debt settlement is visible',
    '- victory return has no black screen or softlock',
    '- victory rewards are non-Vakuu Ancient rewards, not normal combat rewards',
    '- failure/death path exits coherently',
    '- active combat save/load works or is recorded as failing',
    '- prefinished reward/return save/load works or is recorded as failing',
    '',
    'Use SPIREPLUS_RELEASE_EVIDENCE_LOG=1 logs. EZMB_RELEASE_EVIDENCE_LOG=1 remains a legacy alias. Do not mark any row passed from this template alone.'
) -join [Environment]::NewLine

Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $evidenceFull 'command.txt') -Encoding UTF8
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')
Save-Json -InputObject ([ordered]@{ Rows = $manualRows }) -Path (Join-Path $evidenceFull 'manual-rows-template.json')
Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    RequiredFiles = @('godot.log', 'godot-log-audit.json')
    BlockingPatterns = @('ERROR', 'Exception', '[SPIREPLUS-EVIDENCE]', '[EZMB-EVIDENCE]')
    Notes = 'Fill with audit output after copying live logs. This template is not a pass marker.'
}) -Path (Join-Path $evidenceFull 'log-audit-template.json')
$instructions | Set-Content -LiteralPath (Join-Path $evidenceFull 'manual-instructions.md') -Encoding UTF8

if (-not $Launch) {
    Write-Output "Prepared Vakuu fight evidence templates under $evidenceFull."
    Write-Output 'No game was launched. Vakuu fight live rows remain pending.'
    exit 0
}

if (-not (Test-Path -LiteralPath $liveSessionScript)) {
    throw "Missing live-session helper: $liveSessionScript"
}

$launchArgs = @('-Mode', 'Prepare', '-EvidenceDir', $evidenceFull)
if ($MoveOtherMods) { $launchArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $launchArgs += '-MoveCurrentRuns' }
$launchArgs += '-Launch'

$previousEnvironment = Set-ProcessEnvironment -Variables $forceEnvironment
try {
    $result = Invoke-PowerShellFile -ScriptPath $liveSessionScript -ArgumentList $launchArgs
} finally {
    Restore-ProcessEnvironment -PreviousValues $previousEnvironment
}

if ($result.ExitCode -ne 0) {
    throw "Live-session prepare failed with exit code $($result.ExitCode): $($result.Output -join [Environment]::NewLine)"
}

Write-Output "Live session launched. Fill Vakuu fight evidence under $evidenceFull."
