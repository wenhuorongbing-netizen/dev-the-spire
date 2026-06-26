<#
.SYNOPSIS
    Headless game-native AutoSlay smoke runner for Spire Plus.

.DESCRIPTION
    Launches Slay the Spire 2 with the Spire Plus mod, arms the game-native
    AutoSlayer via the mod's env-gated driver (SPIREPLUS_ENABLE_AUTOSLAY=1), waits
    for the AutoSlay run to finish (autoslay.log completion/failure marker AND/OR
    game process exit), enforces a hard timeout, and captures structured evidence
    under .tools/runtime-evidence/autoslay-<yyyyMMdd-HHmmss>/.

    The mod hook lives in EZMicroBalanceCode/Core/Integrations/AutoSlay/:
    SpirePlusAutoSlayDriver subscribes to RitsuLib's MainMenuReadyEvent and calls
    new AutoSlayer().Start(seed, logFile). It is default-OFF and only arms when
    SPIREPLUS_ENABLE_AUTOSLAY is truthy, so this launcher is what turns it on.

    Reports PASS (run completed + exit 0 + clean log audit) or BLOCKED (exact
    failure + the autoslay.log/godot.log paths).

    NOTE: This is a smoke runner, not release proof. The full
    scripts/check-spire-plus-autoslay-packet.ps1 proof packet (multi-run per-seed
    dirs, runtime-probe PID attribution, ancient-id coverage) is a larger
    deliverable; this launcher emits the canonical top-level artifact shapes
    (autoslay-plan.json / autoslay-summary.json / run-result.json /
    autoslay-launcher-proof.json) so that checker can parse them.
#>
[CmdletBinding()]
param(
    [string]$Seed = 'SPIREPLUS-AUTOSLAY-0001',

    [bool]$Headless = $true,

    [string]$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2',

    [string]$SteamExe = 'E:\Steam\steam.exe',

    [string]$EvidenceRoot,

    [ValidateRange(1, 240)]
    [int]$TimeoutMinutes = 30,

    [switch]$DirectExe,

    [ValidateRange(1, 60)]
    [int]$PollSeconds = 3,

    [string]$ExpectedPackageVersion = '',

    [string]$ExpectedGameVersion = '',

    [string]$ExpectedRitsuLibVersion = '',

    [int]$ExpectedPatchCount = 0
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$steamAppId = '2868840'
$gameProcessName = 'SlayTheSpire2'
$gameExeLeaf = 'SlayTheSpire2.exe'
$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$godotLogPath = Join-Path $env:APPDATA 'SlayTheSpire2\logs\godot.log'
$runResultSchemaVersion = 1

# Markers emitted by AutoSlayLog (source code/src/Core/AutoSlay/Helpers/AutoSlayLog.cs).
$autoSlayStartMarker = "Starting run with seed=$Seed"
$autoSlayCompletedMarker = "Run completed successfully with seed=$Seed"
$autoSlayFailedMarker = "Run failed with seed=$Seed"

# The literal contract the AutoSlay packet checker keys on.
$autoSlayerInvocationLiteral = 'AutoSlayer.Start(seed, logFile)'
$hookId = 'spireplus-autoslay-mainmenu-ready'
$hookAssembly = 'EZMicroBalance'

function New-DirectoryIfMissing {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Get-ResolvedFullPath {
    param([Parameter(Mandatory = $true)][string]$Path)
    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Assert-PathInside {
    param(
        [Parameter(Mandatory = $true)][string]$Child,
        [Parameter(Mandatory = $true)][string]$Parent,
        [Parameter(Mandatory = $true)][string]$Label
    )
    $childFull = [System.IO.Path]::GetFullPath($Child).TrimEnd('\')
    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase
    if ($childFull.Equals($parentFull, $comparison)) { return }
    if (-not $childFull.StartsWith($parentFull + '\', $comparison)) {
        throw "$Label path is outside expected root. Path: $childFull Root: $parentFull"
    }
}

function Save-Json {
    param(
        [Parameter(Mandatory = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$Path
    )
    $parent = Split-Path -Parent $Path
    if ($parent) { New-DirectoryIfMissing -Path $parent }
    $InputObject | ConvertTo-Json -Depth 24 | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Read-JsonOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $null }
    try {
        return Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        return $null
    }
}

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)
    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }
    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
}

function Normalize-VersionWithoutPrefix {
    param([AllowEmptyString()][string]$Version)
    if ([string]::IsNullOrWhiteSpace($Version)) { return '' }
    $normalized = $Version.Trim()
    if ($normalized.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $normalized.Substring(1)
    }
    return $normalized
}

function Get-LogSnapshot {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return [pscustomobject]@{ Exists = $false; Length = 0L }
    }
    $item = Get-Item -LiteralPath $Path
    return [pscustomobject]@{ Exists = $true; Length = [long]$item.Length }
}

function Test-FileContains {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Needle
    )
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $false }
    try {
        $text = [System.IO.File]::ReadAllText($Path)
    } catch {
        # File may be locked mid-write; treat as not-yet-present this poll.
        return $false
    }
    return $text.IndexOf($Needle, [System.StringComparison]::Ordinal) -ge 0
}

function Get-LaunchedGameProcess {
    param(
        [Parameter(Mandatory = $true)][datetime]$MinimumStartTimeUtc,
        [int[]]$PreLaunchProcessIds = @()
    )
    $preLaunchIdSet = @{}
    foreach ($id in @($PreLaunchProcessIds)) {
        if ($id -gt 0) { $preLaunchIdSet[$id] = $true }
    }

    $eligible = [System.Collections.Generic.List[object]]::new()
    foreach ($candidate in @(Get-Process -Name $gameProcessName -ErrorAction SilentlyContinue)) {
        $startUtc = $null
        try { $startUtc = $candidate.StartTime.ToUniversalTime() } catch { $startUtc = $null }
        if ($null -eq $startUtc) { continue }
        if ($preLaunchIdSet.ContainsKey([int]$candidate.Id)) { continue }
        if ($startUtc -lt $MinimumStartTimeUtc) { continue }

        $procPath = ''
        try { $procPath = [string]$candidate.Path } catch { $procPath = '' }
        $eligible.Add([pscustomobject]@{
            Id = [int]$candidate.Id
            StartTimeUtc = $startUtc.ToString('o')
            Path = $procPath
        }) | Out-Null
    }

    return @($eligible)
}

# ---- Resolve inputs ----

$gameRootFull = Get-ResolvedFullPath -Path $GameRoot
$gameExePath = Join-Path $gameRootFull $gameExeLeaf

if (-not (Test-Path -LiteralPath $gameRootFull -PathType Container)) {
    throw "GameRoot not found: $gameRootFull"
}

$useDirectExe = [bool]$DirectExe
if (-not $useDirectExe -and -not (Test-Path -LiteralPath $SteamExe -PathType Leaf)) {
    Write-Warning "Steam executable not found ($SteamExe); falling back to direct-exe launch."
    $useDirectExe = $true
}
if ($useDirectExe -and -not (Test-Path -LiteralPath $gameExePath -PathType Leaf)) {
    throw "Direct-exe launch requested but game exe not found: $gameExePath"
}

if (-not $EvidenceRoot) { $EvidenceRoot = $runtimeRoot }
$evidenceRootFull = Get-ResolvedFullPath -Path $EvidenceRoot
Assert-PathInside -Child $evidenceRootFull -Parent $runtimeRoot -Label 'EvidenceRoot'
New-DirectoryIfMissing -Path $evidenceRootFull

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$evidenceDir = Join-Path $evidenceRootFull "autoslay-$stamp"
New-DirectoryIfMissing -Path $evidenceDir

$seedSlug = ($Seed -replace '[^A-Za-z0-9_.-]+', '_').Trim('_')
if ([string]::IsNullOrWhiteSpace($seedSlug)) { $seedSlug = 'seed' }
$runDirName = "run-$seedSlug"
$runDir = Join-Path $evidenceDir $runDirName
New-DirectoryIfMissing -Path $runDir

$autoSlayLogPath = Join-Path $runDir 'autoslay.log'
$runGodotLogBeforePath = Join-Path $runDir 'godot.log.before'
$runGodotLogPath = Join-Path $runDir 'godot.log'
$runResultPath = Join-Path $runDir 'run-result.json'
$godotLogAuditPath = Join-Path $runDir 'godot-log-audit.json'
$launcherProofPath = Join-Path $runDir 'autoslay-launcher-proof.json'
$planPath = Join-Path $evidenceDir 'autoslay-plan.json'
$summaryPath = Join-Path $evidenceDir 'autoslay-summary.json'

# ---- Resolve versions for metadata (best-effort) ----

if (-not $ExpectedPackageVersion) {
    $rootManifest = Read-JsonOrNull -Path (Join-Path $repoRoot 'EZMicroBalance.json')
    if ($rootManifest -and $rootManifest.version) { $ExpectedPackageVersion = [string]$rootManifest.version }
}
if (-not $ExpectedGameVersion) {
    $gameReleaseInfo = Read-JsonOrNull -Path (Join-Path $gameRootFull 'release_info.json')
    if ($gameReleaseInfo -and $gameReleaseInfo.version) {
        $ExpectedGameVersion = Normalize-VersionWithoutPrefix -Version ([string]$gameReleaseInfo.version)
    }
}
if (-not $ExpectedRitsuLibVersion) {
    $ritsuManifest = Read-JsonOrNull -Path (Join-Path $gameRootFull 'mods\STS2-RitsuLib\mod_manifest.json')
    if ($ritsuManifest -and $ritsuManifest.version) {
        $ExpectedRitsuLibVersion = Normalize-VersionWithoutPrefix -Version ([string]$ritsuManifest.version)
    }
}

$launcherPathSelf = $PSCommandPath
$launcherSha256 = Get-FileSha256OrEmpty -Path $launcherPathSelf

# ---- Build launch command ----

if ($useDirectExe) {
    $launchFile = $gameExePath
    $launchArgs = @()
    if ($Headless) { $launchArgs += '--headless' }
    $launchArgs += @('autoslay', '--seed', $Seed, '--log-file', $autoSlayLogPath)
    $launchKind = 'DirectExe'
} else {
    $launchFile = $SteamExe
    # Args after the appId forward to the game.
    $launchArgs = @('-applaunch', $steamAppId)
    if ($Headless) { $launchArgs += '--headless' }
    $launchArgs += @('autoslay', '--seed', $Seed, '--log-file', $autoSlayLogPath)
    $launchKind = 'SteamAppLaunch'
}

# The exact env gate the mod hook reads.
$childEnv = [ordered]@{
    SPIREPLUS_ENABLE_AUTOSLAY = '1'
    SPIREPLUS_AUTOSLAY_SEED = $Seed
    SPIREPLUS_AUTOSLAY_LOG = $autoSlayLogPath
}

# Capture godot.log baseline before launch.
if (Test-Path -LiteralPath $godotLogPath -PathType Leaf) {
    Copy-Item -LiteralPath $godotLogPath -Destination $runGodotLogBeforePath -Force
}
$godotLogBefore = Get-LogSnapshot -Path $godotLogPath

# ---- Launcher proof (records the exact hook command before launch) ----

$invocationCommand = "$launchFile $($launchArgs -join ' ')"
$launcherProof = [ordered]@{
    SchemaVersion = 1
    HookId = $hookId
    HookAssembly = $hookAssembly
    RunnerKind = 'GameNativeAutoSlay'
    LaunchKind = $launchKind
    Headless = [bool]$Headless
    Seed = $Seed
    SteamAppId = if ($launchKind -eq 'SteamAppLaunch') { $steamAppId } else { '' }
    LaunchFilePath = $launchFile
    LaunchArgumentList = @($launchArgs)
    InvocationCommand = $invocationCommand
    EnvironmentGate = $childEnv
    # The mod-side hook the launcher's env gate ultimately drives:
    Invocation = "SpirePlusAutoSlayDriver: RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent> => new $autoSlayerInvocationLiteral"
    AutoSlayLogPath = $autoSlayLogPath
    GodotLogPath = $godotLogPath
    CreatedAt = (Get-Date).ToString('o')
}
Save-Json -InputObject $launcherProof -Path $launcherProofPath

# ---- Plan (top-level, checker-shaped) ----

$plan = [ordered]@{
    SchemaVersion = 1
    RunnerKind = 'GameNativeAutoSlay'
    Invocation = "Mod hook SpirePlusAutoSlayDriver calls new $autoSlayerInvocationLiteral on RitsuLib MainMenuReadyEvent (env-gated by SPIREPLUS_ENABLE_AUTOSLAY)."
    InvocationCommand = "$invocationCommand  ==> mod hook calls new $autoSlayerInvocationLiteral"
    LauncherKind = 'PowerShellAutoSlayLauncher'
    LauncherPath = $launcherProofPath
    LauncherSha256 = (Get-FileSha256OrEmpty -Path $launcherProofPath)
    HookId = $hookId
    HookAssembly = $hookAssembly
    Seeds = @($Seed)
    Sts1EventMode = [string]$env:SPIREPLUS_STS1_EVENT_MODE
    ExpectedPatchCount = $ExpectedPatchCount
    PackageVersion = $ExpectedPackageVersion
    GameVersion = $ExpectedGameVersion
    RitsuLibVersion = $ExpectedRitsuLibVersion
    LaunchKind = $launchKind
    Headless = [bool]$Headless
    CreatedAt = (Get-Date).ToString('o')
}
Save-Json -InputObject $plan -Path $planPath

# ---- Launch ----

Write-Output "AutoSlay launcher starting."
Write-Output "  evidence_dir = $evidenceDir"
Write-Output "  run_dir      = $runDir"
Write-Output "  launch_kind  = $launchKind"
Write-Output "  command      = $invocationCommand"
Write-Output "  env_gate     = SPIREPLUS_ENABLE_AUTOSLAY=1; SPIREPLUS_AUTOSLAY_SEED=$Seed"

$preLaunchProcs = @(Get-Process -Name $gameProcessName -ErrorAction SilentlyContinue)
$preLaunchProcIds = @($preLaunchProcs | ForEach-Object { [int]$_.Id })
if ($preLaunchProcs.Count -gt 0) {
    Write-Warning "Found $($preLaunchProcs.Count) pre-existing $gameProcessName process(es). The shared godot.log may be ambiguous; consider closing them first."
}

# Set env for the child process only (restored in finally).
$priorEnable = $env:SPIREPLUS_ENABLE_AUTOSLAY
$priorSeed = $env:SPIREPLUS_AUTOSLAY_SEED
$priorLog = $env:SPIREPLUS_AUTOSLAY_LOG

$result = [ordered]@{
    SchemaVersion = $runResultSchemaVersion
    Launch = $true
    RunnerKind = 'GameNativeAutoSlay'
    Invocation = "Mod hook calls new $autoSlayerInvocationLiteral via RitsuLib MainMenuReadyEvent."
    InvocationCommand = "$invocationCommand  ==> mod hook calls new $autoSlayerInvocationLiteral"
    LauncherKind = 'PowerShellAutoSlayLauncher'
    LauncherPath = $launcherProofPath
    LauncherSha256 = (Get-FileSha256OrEmpty -Path $launcherProofPath)
    HookId = $hookId
    HookAssembly = $hookAssembly
    Seed = $Seed
    EventKind = 'GameNativeAutoSlay'
    AncientId = ''
    Passed = $false
    ExitCode = -999
    FailureReasonCodes = @()
    HangSignals = @()
    FloorsReached = $null
    StartTimestamp = (Get-Date).ToString('o')
    FinishTimestamp = $null
    ElapsedSeconds = $null
    TimeoutMinutes = $TimeoutMinutes
    LaunchKind = $launchKind
    Headless = [bool]$Headless
    ProcessId = 0
    ProcessStartTimeUtc = $null
    ProcessPath = ''
    AutoSlayLogPath = $autoSlayLogPath
    GodotLogPath = $runGodotLogPath
    GodotLogBeforePath = $runGodotLogBeforePath
    GodotLogAuditPath = $godotLogAuditPath
    AutoSlayCompletedMarkerObserved = $false
    AutoSlayFailedMarkerObserved = $false
    AutoSlayStartMarkerObserved = $false
    PackageVersion = $ExpectedPackageVersion
    GameVersion = $ExpectedGameVersion
    RitsuLibVersion = $ExpectedRitsuLibVersion
}

$failureReasons = [System.Collections.Generic.List[string]]::new()
$startedAtUtc = (Get-Date).ToUniversalTime()
$launchedProcessFromStart = $null

try {
    $env:SPIREPLUS_ENABLE_AUTOSLAY = '1'
    $env:SPIREPLUS_AUTOSLAY_SEED = $Seed
    $env:SPIREPLUS_AUTOSLAY_LOG = $autoSlayLogPath

    $launchRequestedAtUtc = (Get-Date).ToUniversalTime()
    $startedProc = Start-Process -FilePath $launchFile -ArgumentList $launchArgs -PassThru
    $result['LaunchRequestedAtUtc'] = $launchRequestedAtUtc.ToString('o')

    # For direct-exe, the started process IS the game. For Steam, it is the steam
    # broker; the real game is found by start-time below.
    if ($launchKind -eq 'DirectExe') {
        $launchedProcessFromStart = $startedProc
    }

    $deadline = (Get-Date).AddMinutes($TimeoutMinutes)
    $selectedGameProcess = $null
    $completedMarkerSeen = $false
    $failedMarkerSeen = $false
    $gameExitObserved = $false
    $ambiguousProcess = $false

    while ((Get-Date) -lt $deadline) {
        # Marker detection (authoritative completion signal).
        if (Test-FileContains -Path $autoSlayLogPath -Needle $autoSlayCompletedMarker) {
            $completedMarkerSeen = $true
            break
        }
        if (Test-FileContains -Path $autoSlayLogPath -Needle $autoSlayFailedMarker) {
            $failedMarkerSeen = $true
            break
        }

        # Process attribution (only relevant for exit detection / metadata).
        if ($null -eq $selectedGameProcess) {
            $candidates = Get-LaunchedGameProcess -MinimumStartTimeUtc $launchRequestedAtUtc -PreLaunchProcessIds $preLaunchProcIds
            if ($candidates.Count -eq 1) {
                $selectedGameProcess = $candidates[0]
            } elseif ($candidates.Count -gt 1) {
                $ambiguousProcess = $true
            }
        }

        if ($null -ne $selectedGameProcess) {
            $stillRunning = @(Get-Process -Id ([int]$selectedGameProcess.Id) -ErrorAction SilentlyContinue)
            if ($stillRunning.Count -eq 0) {
                $gameExitObserved = $true
                # Give the log a moment to flush its final marker after exit.
                Start-Sleep -Seconds 1
                if (Test-FileContains -Path $autoSlayLogPath -Needle $autoSlayCompletedMarker) { $completedMarkerSeen = $true }
                if (Test-FileContains -Path $autoSlayLogPath -Needle $autoSlayFailedMarker) { $failedMarkerSeen = $true }
                break
            }
        }

        Start-Sleep -Seconds $PollSeconds
    }

    $finishedAtUtc = (Get-Date).ToUniversalTime()
    $result['FinishTimestamp'] = (Get-Date).ToString('o')
    $result['ElapsedSeconds'] = [Math]::Round(($finishedAtUtc - $startedAtUtc).TotalSeconds, 2)
    $result['AutoSlayStartMarkerObserved'] = Test-FileContains -Path $autoSlayLogPath -Needle $autoSlayStartMarker
    $result['AutoSlayCompletedMarkerObserved'] = $completedMarkerSeen
    $result['AutoSlayFailedMarkerObserved'] = $failedMarkerSeen

    if ($null -ne $selectedGameProcess) {
        $result['ProcessId'] = [int]$selectedGameProcess.Id
        $result['ProcessStartTimeUtc'] = $selectedGameProcess.StartTimeUtc
        $result['ProcessPath'] = [string]$selectedGameProcess.Path
    } elseif ($null -ne $launchedProcessFromStart) {
        $result['ProcessId'] = [int]$launchedProcessFromStart.Id
    }

    if ($ambiguousProcess -and $null -eq $selectedGameProcess) {
        $failureReasons.Add('AmbiguousGameProcessAttribution') | Out-Null
    }

    # Determine exit code.
    $exitCode = -999
    if ($gameExitObserved -and $null -ne $launchedProcessFromStart) {
        try { $exitCode = [int]$launchedProcessFromStart.ExitCode } catch { $exitCode = -999 }
    }
    # If we only have markers (Steam path), infer a clean exit when completed.
    if ($exitCode -eq -999) {
        if ($completedMarkerSeen) { $exitCode = 0 }
        elseif ($failedMarkerSeen) { $exitCode = 1 }
    }
    $result['ExitCode'] = $exitCode

    # Capture floors reached from autoslay.log (best-effort).
    if (Test-Path -LiteralPath $autoSlayLogPath -PathType Leaf) {
        try {
            $logText = [System.IO.File]::ReadAllText($autoSlayLogPath)
            $floorMatches = [regex]::Matches($logText, 'Floor (\d+)')
            if ($floorMatches.Count -gt 0) {
                $maxFloor = ($floorMatches | ForEach-Object { [int]$_.Groups[1].Value } | Measure-Object -Maximum).Maximum
                $result['FloorsReached'] = [int]$maxFloor
            }
        } catch { }
    }

    # Outcome classification.
    if (-not $completedMarkerSeen -and -not $failedMarkerSeen) {
        if ((Get-Date).ToUniversalTime() -ge $deadline.ToUniversalTime()) {
            $failureReasons.Add('HardTimeoutExceeded') | Out-Null
            $result['HangSignals'] = @('NoCompletionMarkerBeforeTimeout')
        } else {
            $failureReasons.Add('GameExitedWithoutCompletionMarker') | Out-Null
        }
    }
    if ($failedMarkerSeen) {
        $failureReasons.Add('AutoSlayRunFailed') | Out-Null
    }
    if (-not $result['AutoSlayStartMarkerObserved']) {
        $failureReasons.Add('AutoSlayStartMarkerMissing') | Out-Null
    }

    # On timeout, try to stop the launched game so it does not linger.
    if (-not $completedMarkerSeen -and -not $gameExitObserved) {
        if ($null -ne $selectedGameProcess) {
            Stop-Process -Id ([int]$selectedGameProcess.Id) -Force -ErrorAction SilentlyContinue
        } elseif ($null -ne $launchedProcessFromStart) {
            Stop-Process -Id ([int]$launchedProcessFromStart.Id) -Force -ErrorAction SilentlyContinue
        }
    }
}
finally {
    $env:SPIREPLUS_ENABLE_AUTOSLAY = $priorEnable
    $env:SPIREPLUS_AUTOSLAY_SEED = $priorSeed
    $env:SPIREPLUS_AUTOSLAY_LOG = $priorLog
}

# ---- Capture evidence ----

if (Test-Path -LiteralPath $godotLogPath -PathType Leaf) {
    Copy-Item -LiteralPath $godotLogPath -Destination $runGodotLogPath -Force
}

# Run the shared log audit over the captured godot.log. audit-godot-log.ps1 emits
# a JSON array of per-file objects, each with a 'Clean' boolean and SignatureHits[].
$auditClean = $false
$auditObject = $null
if (Test-Path -LiteralPath $runGodotLogPath -PathType Leaf) {
    try {
        & $logAuditScript -Path $runGodotLogPath -OutFile $godotLogAuditPath | Out-Null
        $auditObject = Read-JsonOrNull -Path $godotLogAuditPath
        if ($null -ne $auditObject) {
            $auditEntries = @($auditObject)
            if ($auditEntries.Count -gt 0) {
                $uncleanEntries = @($auditEntries | Where-Object {
                    -not ($_.PSObject.Properties.Name -contains 'Clean' -and [bool]$_.Clean)
                })
                $auditClean = ($uncleanEntries.Count -eq 0)
            }
        }
    } catch {
        $failureReasons.Add('GodotLogAuditError') | Out-Null
    }
} else {
    $failureReasons.Add('GodotLogNotCaptured') | Out-Null
}

if (-not $auditClean) {
    $failureReasons.Add('GodotLogAuditNotClean') | Out-Null
}

$completed = [bool]$result['AutoSlayCompletedMarkerObserved']
$exitOk = ([int]$result['ExitCode'] -eq 0)
$passed = $completed -and $exitOk -and $auditClean -and ($failureReasons.Count -eq 0)

$result['Passed'] = $passed
$result['FailureReasonCodes'] = @($failureReasons | Select-Object -Unique)
Save-Json -InputObject $result -Path $runResultPath

$result['RunResultSha256'] = (Get-FileSha256OrEmpty -Path $runResultPath)
Save-Json -InputObject $result -Path $runResultPath

# ---- Summary (top-level, checker-shaped) ----

$summary = [ordered]@{
    SchemaVersion = 1
    Passed = $passed
    TotalRuns = 1
    FailedRuns = if ($passed) { 0 } else { 1 }
    ExpectedPatchCount = $ExpectedPatchCount
    PackageVersion = $ExpectedPackageVersion
    GameVersion = $ExpectedGameVersion
    RitsuLibVersion = $ExpectedRitsuLibVersion
    Runs = @(
        [ordered]@{
            Seed = $Seed
            EventKind = 'GameNativeAutoSlay'
            AncientId = ''
            Passed = $passed
            ExitCode = [int]$result['ExitCode']
            FailureReasonCodes = @($result['FailureReasonCodes'])
            HangSignals = @($result['HangSignals'])
            RunResultPath = $runResultPath
            RunResultSha256 = (Get-FileSha256OrEmpty -Path $runResultPath)
            AutoSlayLogPath = $autoSlayLogPath
            GodotLogBeforePath = $runGodotLogBeforePath
            GodotLogAfterLaunchPath = $runGodotLogPath
            GodotLogAuditPath = $godotLogAuditPath
            FloorsReached = $result['FloorsReached']
        }
    )
    CreatedAt = (Get-Date).ToString('o')
}
Save-Json -InputObject $summary -Path $summaryPath

# ---- Report ----

Write-Output ''
if ($passed) {
    Write-Output "RESULT: PASS"
    Write-Output "  AutoSlay run completed (seed=$Seed), exit=$($result['ExitCode']), log audit clean."
    Write-Output "  floors_reached = $($result['FloorsReached'])"
} else {
    Write-Output "RESULT: BLOCKED"
    Write-Output "  failure_reasons = $((@($result['FailureReasonCodes']) -join ', '))"
    Write-Output "  completed_marker = $($result['AutoSlayCompletedMarkerObserved']); failed_marker = $($result['AutoSlayFailedMarkerObserved']); exit = $($result['ExitCode'])"
}
Write-Output "  autoslay_log = $autoSlayLogPath"
Write-Output "  godot_log    = $runGodotLogPath"
Write-Output "  run_result   = $runResultPath"
Write-Output "  plan         = $planPath"
Write-Output "  summary      = $summaryPath"
Write-Output "  launcher_proof = $launcherProofPath"

if ($passed) { exit 0 } else { exit 1 }
