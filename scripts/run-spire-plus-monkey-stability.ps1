param(
    [ValidateRange(1, 10000)]
    [int]$Iterations = 1000,

    [string]$EvidenceRoot,

    [string]$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2',

    [string]$SteamExe = 'E:\Steam\steam.exe',

    [string]$SteamUserId,

    [ValidateSet('', 'eng', 'zhs')]
    [string]$Language = '',

    [ValidateSet('Off', 'CanaryOnly', 'AdditiveBatch1')]
    [string]$Sts1EventMode = 'Off',

    [ValidateSet('AncientUiSmoke', 'AncientUiPlusVakuuFight', 'VakuuFightSmoke', 'StartupOnly')]
    [string]$Scenario = 'AncientUiSmoke',

    [ValidateSet('RoundRobin', 'Random')]
    [string]$CommandSelectionMode = 'RoundRobin',

    [string]$CommandCorpusFile,

    [string[]]$CommandCorpus = @(),

    [ValidateRange(30, 1800)]
    [int]$MainMenuTimeoutSeconds = 180,

    [ValidateRange(1, 60)]
    [int]$ObservationIntervalSeconds = 2,

    [ValidateRange(1, 30)]
    [int]$UnresponsiveSampleThreshold = 3,

    [ValidateRange(30, 1800)]
    [int]$NoLogGrowthTimeoutSeconds = 90,

    [ValidateRange(5, 1800)]
    [int]$PostCommandSeconds = 20,

    [string]$ExpectedPackageVersion = '',

    [string]$ExpectedGameVersion = '',

    [string]$ExpectedRitsuLibVersion = '',

    [string]$ExpectedRitsuCompatBranch = '',

    [ValidateRange(1, 1000)]
    [int]$ExpectedPatchCount = 25,

    [int]$RandomSeed = 1729,

    [switch]$Launch,

    [switch]$MoveOtherMods,

    [switch]$MoveCurrentRuns,

    [switch]$NoDevConsoleCommands,

    [switch]$FailOnFirstFailure
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$liveSessionScript = Join-Path $PSScriptRoot 'spire-plus-live-session.ps1'
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$consoleCommandScript = Join-Path $PSScriptRoot 'send-spire-dev-console-command.ps1'
$sts1ModeVerifierScript = Join-Path $PSScriptRoot 'check-sts1-enabled-mode-runtime-log.ps1'
$godotLogPath = Join-Path $env:APPDATA 'SlayTheSpire2\logs\godot.log'
$hangProbeSchemaVersion = 1

if (-not ('SpirePlusRuntimeMonkeyNative' -as [type])) {
    Add-Type @"
using System;
using System.Runtime.InteropServices;

public static class SpirePlusRuntimeMonkeyNative {
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsHungAppWindow(IntPtr hWnd);
}
"@
}

function New-DirectoryIfMissing {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Save-Json {
    param(
        [Parameter(Mandatory = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $parent = Split-Path -Parent $Path
    if ($parent) {
        New-DirectoryIfMissing -Path $parent
    }

    $InputObject | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Get-ResolvedFullPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Read-JsonOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $null
    }

    return Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
}

function Normalize-VersionWithoutPrefix {
    param([AllowEmptyString()][string]$Version)

    if ([string]::IsNullOrWhiteSpace($Version)) {
        return ''
    }

    $normalized = $Version.Trim()
    if ($normalized.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $normalized.Substring(1)
    }

    return $normalized
}

function Test-LogContains {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $false
    }

    return [regex]::IsMatch([System.IO.File]::ReadAllText($Path), $Pattern)
}

function Get-LogTextAfterOffset {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    $stream = [System.IO.File]::Open($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    try {
        $safeOffset = $Offset
        if ($safeOffset -lt 0 -or $safeOffset -gt $stream.Length) {
            $safeOffset = 0L
        }

        [void]$stream.Seek($safeOffset, [System.IO.SeekOrigin]::Begin)
        $reader = [System.IO.StreamReader]::new($stream, [System.Text.Encoding]::UTF8, $true, 4096, $true)
        try {
            return $reader.ReadToEnd()
        } finally {
            $reader.Dispose()
        }
    } finally {
        $stream.Dispose()
    }
}

function Test-LogContainsAfterOffset {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Pattern,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    $text = Get-LogTextAfterOffset -Path $Path -Offset $Offset
    if ([string]::IsNullOrEmpty($text)) {
        return $false
    }

    return [regex]::IsMatch($text, $Pattern)
}

function Get-LogSnapshot {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return [pscustomobject]@{
            Exists = $false
            Length = 0L
            LastWriteTimeUtc = $null
        }
    }

    $item = Get-Item -LiteralPath $Path
    return [pscustomobject]@{
        Exists = $true
        Length = [long]$item.Length
        LastWriteTimeUtc = $item.LastWriteTimeUtc.ToString('o')
    }
}

function Get-SpireProcessSnapshot {
    param([datetime]$MinimumStartTimeUtc = [datetime]::MinValue)

    $processes = @(Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue)
    $currentProcesses = [System.Collections.Generic.List[object]]::new()
    $staleProcessCount = 0
    $earliestStaleProcessStartTimeUtc = $null

    foreach ($candidate in $processes) {
        $candidateStartTimeUtc = $null
        try {
            $candidateStartTimeUtc = $candidate.StartTime.ToUniversalTime()
        } catch {
            $candidateStartTimeUtc = $null
        }

        if ($candidateStartTimeUtc -and $candidateStartTimeUtc -lt $MinimumStartTimeUtc) {
            $staleProcessCount++
            if ($null -eq $earliestStaleProcessStartTimeUtc -or $candidateStartTimeUtc -lt $earliestStaleProcessStartTimeUtc) {
                $earliestStaleProcessStartTimeUtc = $candidateStartTimeUtc
            }

            continue
        }

        $currentProcesses.Add($candidate) | Out-Null
    }

    $process = @($currentProcesses | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1)
    if ($process.Count -eq 0) {
        $process = @($currentProcesses | Select-Object -First 1)
    }

    if ($process.Count -eq 0) {
        return [pscustomobject]@{
            Observed = $false
            ProcessName = ''
            Id = 0
            MainWindowHandle = 0
            MainWindowTitle = ''
            Responding = $null
            HungWindow = $false
            Error = ''
            MinimumStartTimeUtc = $MinimumStartTimeUtc.ToString('o')
            StaleProcessCount = $staleProcessCount
            EarliestStaleProcessStartTimeUtc = if ($earliestStaleProcessStartTimeUtc) { $earliestStaleProcessStartTimeUtc.ToString('o') } else { $null }
        }
    }

    $selected = $process[0]
    $handle = [IntPtr]$selected.MainWindowHandle
    $responding = $null
    $hungWindow = $false
    $error = ''
    $startTimeUtc = $null

    try {
        $startTimeUtc = $selected.StartTime.ToUniversalTime().ToString('o')
    } catch {
        $startTimeUtc = $null
    }

    if ($handle -ne [IntPtr]::Zero) {
        try {
            $responding = [bool]$selected.Responding
        } catch {
            $error = $_.Exception.Message
        }

        try {
            $hungWindow = [SpirePlusRuntimeMonkeyNative]::IsHungAppWindow($handle)
        } catch {
            if ($error) {
                $error = "$error; $($_.Exception.Message)"
            } else {
                $error = $_.Exception.Message
            }
        }
    }

    return [pscustomobject]@{
        Observed = $true
        ProcessName = $selected.ProcessName
        Id = $selected.Id
        StartTimeUtc = $startTimeUtc
        MainWindowHandle = [int64]$selected.MainWindowHandle
        MainWindowTitle = $selected.MainWindowTitle
        Responding = $responding
        HungWindow = $hungWindow
        Error = $error
        MinimumStartTimeUtc = $MinimumStartTimeUtc.ToString('o')
        StaleProcessCount = $staleProcessCount
        EarliestStaleProcessStartTimeUtc = if ($earliestStaleProcessStartTimeUtc) { $earliestStaleProcessStartTimeUtc.ToString('o') } else { $null }
    }
}

function Add-ProbeSample {
    param(
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$Samples,
        [Parameter(Mandatory = $true)][string]$Phase,
        [Parameter(Mandatory = $true)]$LogSnapshot,
        [Parameter(Mandatory = $true)]$ProcessSnapshot
    )

    $Samples.Add([pscustomobject]@{
        Phase = $Phase
        SampledAt = (Get-Date).ToString('o')
        LogExists = [bool]$LogSnapshot.Exists
        LogLengthBytes = [long]$LogSnapshot.Length
        LogLastWriteTimeUtc = $LogSnapshot.LastWriteTimeUtc
        ProcessObserved = [bool]$ProcessSnapshot.Observed
        ProcessName = $ProcessSnapshot.ProcessName
        ProcessId = [int]$ProcessSnapshot.Id
        MinimumProcessStartTimeUtc = $ProcessSnapshot.MinimumStartTimeUtc
        StaleProcessCount = [int]$ProcessSnapshot.StaleProcessCount
        EarliestStaleProcessStartTimeUtc = $ProcessSnapshot.EarliestStaleProcessStartTimeUtc
        MainWindowObserved = [int64]$ProcessSnapshot.MainWindowHandle -ne 0
        MainWindowTitle = $ProcessSnapshot.MainWindowTitle
        Responding = $ProcessSnapshot.Responding
        HungWindow = [bool]$ProcessSnapshot.HungWindow
    }) | Out-Null
}

function Wait-ForMainMenuLog {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][int]$TimeoutSeconds,
        [Parameter(Mandatory = $true)][int]$NoGrowthTimeoutSeconds,
        [Parameter(Mandatory = $true)][int]$IntervalSeconds,
        [Parameter(Mandatory = $true)][int]$UnresponsiveSampleThreshold,
        [Parameter(Mandatory = $true)][long]$BaselineLogLengthBytes,
        [Parameter(Mandatory = $true)][datetime]$MinimumProcessStartTimeUtc,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$ProbeSamples
    )

    $startedAt = Get-Date
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $pattern = '\[Startup\] Time to main menu'
    $initialLog = Get-LogSnapshot -Path $Path
    $lastLog = $initialLog
    $initialLogResetObserved = $initialLog.Exists -and [long]$initialLog.Length -lt $BaselineLogLengthBytes
    $scanOffset = if ($initialLog.Exists -and -not $initialLogResetObserved) { $BaselineLogLengthBytes } else { 0L }
    $lastGrowthAt = if ($initialLog.Exists) { Get-Date } else { $startedAt }
    $lastLength = if ($initialLog.Exists) { [long]$initialLog.Length } else { -1L }
    $logResetObserved = $initialLogResetObserved
    $maxNoGrowthSeconds = 0
    $sampleCount = 0
    $processObserved = $false
    $processExitedAfterObservation = $false
    $hungWindowDetected = $false
    $consecutiveUnresponsiveSamples = 0
    $maxConsecutiveUnresponsiveSamples = 0
    $noLogGrowthTimeoutExceeded = $false
    $lastProcess = $null
    $failureReason = ''

    do {
        $sampleCount++
        $lastLog = Get-LogSnapshot -Path $Path
        if ($lastLog.Exists -and ([long]$lastLog.Length -gt $lastLength -or $lastLength -lt 0)) {
            $lastLength = [long]$lastLog.Length
            $lastGrowthAt = Get-Date
        } elseif ($lastLog.Exists -and [long]$lastLog.Length -lt $lastLength) {
            $logResetObserved = $true
            $lastLength = [long]$lastLog.Length
            $scanOffset = 0L
            $lastGrowthAt = Get-Date
        }

        $lastProcess = Get-SpireProcessSnapshot -MinimumStartTimeUtc $MinimumProcessStartTimeUtc
        Add-ProbeSample -Samples $ProbeSamples -Phase 'StartupMainMenu' -LogSnapshot $lastLog -ProcessSnapshot $lastProcess
        if ($lastProcess.Observed) {
            $processObserved = $true
        } elseif ($processObserved) {
            $processExitedAfterObservation = $true
            $failureReason = 'SlayTheSpire2 process disappeared before main menu.'
            break
        }

        $sampleUnresponsive = $lastProcess.HungWindow -or ($null -ne $lastProcess.Responding -and -not [bool]$lastProcess.Responding)
        if ($sampleUnresponsive) {
            $consecutiveUnresponsiveSamples++
            $maxConsecutiveUnresponsiveSamples = [Math]::Max($maxConsecutiveUnresponsiveSamples, $consecutiveUnresponsiveSamples)
        } else {
            $consecutiveUnresponsiveSamples = 0
        }

        if ($consecutiveUnresponsiveSamples -ge $UnresponsiveSampleThreshold) {
            $hungWindowDetected = $true
            $failureReason = "SlayTheSpire2 window reported hung/not responding for $consecutiveUnresponsiveSamples consecutive samples before main menu."
            break
        }

        $noGrowthSeconds = [int][Math]::Floor(((Get-Date) - $lastGrowthAt).TotalSeconds)
        $maxNoGrowthSeconds = [Math]::Max($maxNoGrowthSeconds, $noGrowthSeconds)
        if ($noGrowthSeconds -ge $NoGrowthTimeoutSeconds) {
            $noLogGrowthTimeoutExceeded = $true
            $failureReason = "godot.log did not grow for $noGrowthSeconds seconds before main menu."
            break
        }

        if (Test-LogContainsAfterOffset -Path $Path -Pattern $pattern -Offset $scanOffset) {
            return [pscustomobject]@{
                MainMenuReached = $true
                Passed = $true
                FailureReason = ''
                StartedAt = $startedAt.ToString('o')
                FinishedAt = (Get-Date).ToString('o')
                TimeoutSeconds = $TimeoutSeconds
                NoLogGrowthTimeoutSeconds = $NoGrowthTimeoutSeconds
                ObservationIntervalSeconds = $IntervalSeconds
                Samples = $sampleCount
                ElapsedSeconds = [Math]::Round(((Get-Date) - $startedAt).TotalSeconds, 3)
                MainMenuDetectedAt = (Get-Date).ToString('o')
                ProcessObserved = $processObserved
                ProcessExitedAfterObservation = $processExitedAfterObservation
                HungWindowDetected = $hungWindowDetected
                NoLogGrowthTimeoutExceeded = $noLogGrowthTimeoutExceeded
                LogObserved = [bool]$lastLog.Exists
                LogInitialLengthBytes = [long]$initialLog.Length
                BaselineLogLengthBytes = $BaselineLogLengthBytes
                LogScanOffsetBytes = $scanOffset
                LogFinalLength = [long]$lastLog.Length
                LogLastWriteTimeUtc = $lastLog.LastWriteTimeUtc
                LastLogGrowthAt = $lastGrowthAt.ToString('o')
                LogResetObserved = $logResetObserved
                MaxNoLogGrowthSeconds = $maxNoGrowthSeconds
                MaxConsecutiveUnresponsiveSamples = $maxConsecutiveUnresponsiveSamples
                LastProcess = $lastProcess
                MinimumProcessStartTimeUtc = $MinimumProcessStartTimeUtc.ToString('o')
            }
        }

        Start-Sleep -Seconds $IntervalSeconds
    } while ((Get-Date) -lt $deadline)

    if (-not $failureReason) {
        $failureReason = 'main menu log line missing before timeout.'
    }

    return [pscustomobject]@{
        MainMenuReached = $false
        Passed = $false
        FailureReason = $failureReason
        StartedAt = $startedAt.ToString('o')
        FinishedAt = (Get-Date).ToString('o')
        TimeoutSeconds = $TimeoutSeconds
        NoLogGrowthTimeoutSeconds = $NoGrowthTimeoutSeconds
        ObservationIntervalSeconds = $IntervalSeconds
        Samples = $sampleCount
        ElapsedSeconds = [Math]::Round(((Get-Date) - $startedAt).TotalSeconds, 3)
        MainMenuDetectedAt = $null
        ProcessObserved = $processObserved
        ProcessExitedAfterObservation = $processExitedAfterObservation
        HungWindowDetected = $hungWindowDetected
        NoLogGrowthTimeoutExceeded = $noLogGrowthTimeoutExceeded
        LogObserved = [bool]$lastLog.Exists
        LogInitialLengthBytes = [long]$initialLog.Length
        BaselineLogLengthBytes = $BaselineLogLengthBytes
        LogScanOffsetBytes = $scanOffset
        LogFinalLength = if ($lastLog) { [long]$lastLog.Length } else { 0L }
        LogLastWriteTimeUtc = if ($lastLog) { $lastLog.LastWriteTimeUtc } else { $null }
        LastLogGrowthAt = $lastGrowthAt.ToString('o')
        LogResetObserved = $logResetObserved
        MaxNoLogGrowthSeconds = $maxNoGrowthSeconds
        MaxConsecutiveUnresponsiveSamples = $maxConsecutiveUnresponsiveSamples
        LastProcess = $lastProcess
        MinimumProcessStartTimeUtc = $MinimumProcessStartTimeUtc.ToString('o')
    }
}

function Watch-RuntimeHealth {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][int]$DurationSeconds,
        [Parameter(Mandatory = $true)][int]$IntervalSeconds,
        [Parameter(Mandatory = $true)][int]$UnresponsiveSampleThreshold,
        [Parameter(Mandatory = $true)][datetime]$MinimumProcessStartTimeUtc,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$ProbeSamples
    )

    $startedAt = Get-Date
    $deadline = (Get-Date).AddSeconds($DurationSeconds)
    $initialLog = Get-LogSnapshot -Path $Path
    $lastLog = $initialLog
    $lastLength = [long]$initialLog.Length
    $lastGrowthAt = if ($initialLog.Exists) { Get-Date } else { $null }
    $logResetObserved = $false
    $maxNoGrowthSeconds = 0
    $sampleCount = 0
    $processObserved = $false
    $processExitedAfterObservation = $false
    $hungWindowDetected = $false
    $consecutiveUnresponsiveSamples = 0
    $maxConsecutiveUnresponsiveSamples = 0
    $lastProcess = $null
    $failureReason = ''

    do {
        $sampleCount++
        $lastProcess = Get-SpireProcessSnapshot -MinimumStartTimeUtc $MinimumProcessStartTimeUtc
        $lastLog = Get-LogSnapshot -Path $Path
        Add-ProbeSample -Samples $ProbeSamples -Phase 'PostCommandRuntime' -LogSnapshot $lastLog -ProcessSnapshot $lastProcess
        if ($lastProcess.Observed) {
            $processObserved = $true
        } elseif ($processObserved) {
            $processExitedAfterObservation = $true
            $failureReason = 'SlayTheSpire2 process disappeared during runtime observation.'
            break
        }

        $sampleUnresponsive = $lastProcess.HungWindow -or ($null -ne $lastProcess.Responding -and -not [bool]$lastProcess.Responding)
        if ($sampleUnresponsive) {
            $consecutiveUnresponsiveSamples++
            $maxConsecutiveUnresponsiveSamples = [Math]::Max($maxConsecutiveUnresponsiveSamples, $consecutiveUnresponsiveSamples)
        } else {
            $consecutiveUnresponsiveSamples = 0
        }

        if ($consecutiveUnresponsiveSamples -ge $UnresponsiveSampleThreshold) {
            $hungWindowDetected = $true
            $failureReason = "SlayTheSpire2 window reported hung/not responding for $consecutiveUnresponsiveSamples consecutive samples during runtime observation."
            break
        }

        if ($lastLog.Exists -and [long]$lastLog.Length -gt $lastLength) {
            $lastLength = [long]$lastLog.Length
            $lastGrowthAt = Get-Date
        } elseif ($lastLog.Exists -and [long]$lastLog.Length -lt $lastLength) {
            $logResetObserved = $true
            $lastLength = [long]$lastLog.Length
            $lastGrowthAt = Get-Date
        }

        if ($lastGrowthAt) {
            $noGrowthSeconds = [int][Math]::Floor(((Get-Date) - $lastGrowthAt).TotalSeconds)
            $maxNoGrowthSeconds = [Math]::Max($maxNoGrowthSeconds, $noGrowthSeconds)
        }

        Start-Sleep -Seconds $IntervalSeconds
    } while ((Get-Date) -lt $deadline)

    $logGrew = $lastLog.Exists -and [long]$lastLog.Length -gt [long]$initialLog.Length
    $passed = $processObserved -and -not $processExitedAfterObservation -and -not $hungWindowDetected
    if (-not $failureReason -and -not $processObserved) {
        $failureReason = 'SlayTheSpire2 process was not observed during runtime observation.'
    }

    return [pscustomobject]@{
        Passed = $passed
        FailureReason = if ($passed) { '' } else { $failureReason }
        StartedAt = $startedAt.ToString('o')
        FinishedAt = (Get-Date).ToString('o')
        DurationSeconds = $DurationSeconds
        ObservationIntervalSeconds = $IntervalSeconds
        Samples = $sampleCount
        ElapsedSeconds = [Math]::Round(((Get-Date) - $startedAt).TotalSeconds, 3)
        ProcessObserved = $processObserved
        ProcessExitedAfterObservation = $processExitedAfterObservation
        HungWindowDetected = $hungWindowDetected
        LogObserved = [bool]$lastLog.Exists
        LogInitialLengthBytes = [long]$initialLog.Length
        LogFinalLengthBytes = [long]$lastLog.Length
        LogGrew = $logGrew
        LastLogGrowthAt = if ($lastGrowthAt) { $lastGrowthAt.ToString('o') } else { $null }
        LogResetObserved = $logResetObserved
        MaxNoLogGrowthSeconds = $maxNoGrowthSeconds
        MaxConsecutiveUnresponsiveSamples = $maxConsecutiveUnresponsiveSamples
        LastProcess = $lastProcess
        MinimumProcessStartTimeUtc = $MinimumProcessStartTimeUtc.ToString('o')
    }
}

function Get-CommandAckPattern {
    param([AllowEmptyString()][string]$Command)

    if ([string]::IsNullOrWhiteSpace($Command)) {
        return ''
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        return "\[Spire Plus\] Starting unsaved live-test run for $([regex]::Escape($target)) Ancient UI evidence\."
    }

    return ''
}

function Get-ScenarioCommandCorpus {
    param([Parameter(Mandatory = $true)][string]$ScenarioName)

    switch ($ScenarioName) {
        'StartupOnly' {
            return @()
        }
        'VakuuFightSmoke' {
            return @('spireplus_test_ancient VAKUU confirm fight')
        }
        'AncientUiPlusVakuuFight' {
            return @(
                'spireplus_test_ancient URDA confirm',
                'spireplus_test_ancient MORVI confirm',
                'spireplus_test_ancient LOTHA confirm',
                'spireplus_test_ancient VAKUU confirm',
                'spireplus_test_ancient VAKUU confirm fight'
            )
        }
        default {
            return @(
                'spireplus_test_ancient URDA confirm',
                'spireplus_test_ancient MORVI confirm',
                'spireplus_test_ancient LOTHA confirm',
                'spireplus_test_ancient VAKUU confirm'
            )
        }
    }
}

function Get-CommandOwnerArea {
    param([AllowEmptyString()][string]$Command)

    if ([string]::IsNullOrWhiteSpace($Command)) {
        return 'RuntimeStartup'
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b(.*)$') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        $tail = $Matches[2]
        if ($target -eq 'VAKUU' -and $tail -match '(?i)\bfight\b') {
            return 'Ancients.Vakuu.ChildCombatResume'
        }

        switch ($target) {
            'URDA' { return 'Ancients.Urda.MapSaveState' }
            'MORVI' { return 'Ancients.Morvi.CardPlayState' }
            'LOTHA' { return 'Ancients.Lotha.CardPlayState' }
            'VAKUU' { return 'Ancients.Vakuu' }
        }
    }

    return 'Runtime.Unknown'
}

function Get-CommandScenarioTag {
    param([AllowEmptyString()][string]$Command)

    if ([string]::IsNullOrWhiteSpace($Command)) {
        return 'startup'
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b(.*)$') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        if ($target -eq 'VAKUU' -and $Matches[2] -match '(?i)\bfight\b') {
            return 'vakuu-fight'
        }

        return "ancient-ui-$($target.ToLowerInvariant())"
    }

    return 'custom-command'
}

function Test-CommandAck {
    param(
        [Parameter(Mandatory = $true)][string]$LogPath,
        [AllowEmptyString()][string]$Command
    )

    $pattern = Get-CommandAckPattern -Command $Command
    if ([string]::IsNullOrWhiteSpace($pattern)) {
        return [pscustomobject]@{
            Required = $false
            Observed = $true
            Pattern = ''
        }
    }

    $text = [System.IO.File]::ReadAllText($LogPath)
    return [pscustomobject]@{
        Required = $true
        Observed = [regex]::IsMatch($text, $pattern)
        Pattern = $pattern
    }
}

function Add-FailureCode {
    param(
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[string]]$Codes,
        [AllowEmptyString()][string]$Code
    )

    if (-not [string]::IsNullOrWhiteSpace($Code)) {
        [void]$Codes.Add($Code)
    }
}

function Get-FailureReasonCodes {
    param([Parameter(Mandatory = $true)]$Result)

    $codes = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $main = $Result.MainMenuObservation
    $runtime = $Result.RuntimeObservation

    if ($main) {
        if (-not [bool]$main.ProcessObserved) { Add-FailureCode -Codes $codes -Code 'game_process_missing' }
        if ([bool]$main.ProcessExitedAfterObservation) { Add-FailureCode -Codes $codes -Code 'game_process_exited' }
        if ([bool]$main.HungWindowDetected) { Add-FailureCode -Codes $codes -Code 'process_unresponsive' }
        if ([bool]$main.NoLogGrowthTimeoutExceeded) { Add-FailureCode -Codes $codes -Code 'startup_log_stalled' }
        if (-not [bool]$main.MainMenuReached) { Add-FailureCode -Codes $codes -Code 'main_menu_timeout' }
    } elseif (-not [bool]$Result.MainMenuReached) {
        Add-FailureCode -Codes $codes -Code 'main_menu_timeout'
    }

    if ($runtime) {
        if (-not [bool]$runtime.ProcessObserved) { Add-FailureCode -Codes $codes -Code 'game_process_missing' }
        if ([bool]$runtime.ProcessExitedAfterObservation) { Add-FailureCode -Codes $codes -Code 'game_process_exited' }
        if ([bool]$runtime.HungWindowDetected) { Add-FailureCode -Codes $codes -Code 'process_unresponsive' }
    }

    if ($Result.CommandAckRequired -and -not $Result.CommandAckObserved) {
        Add-FailureCode -Codes $codes -Code 'command_ack_missing'
    }

    if (-not [bool]$Result.LogCopied) { Add-FailureCode -Codes $codes -Code 'godot_log_missing' }
    if (-not [bool]$Result.AuditClean) { Add-FailureCode -Codes $codes -Code 'log_audit_failed' }
    if (-not [bool]$Result.ExpectationPassed) { Add-FailureCode -Codes $codes -Code 'runtime_expectation_mismatch' }
    if (-not [bool]$Result.Sts1ModeVerifierPassed) { Add-FailureCode -Codes $codes -Code 'sts1_mode_mismatch' }
    if (-not [bool]$Result.RestoreSucceeded) { Add-FailureCode -Codes $codes -Code 'restore_failed' }
    if (-not $devConsoleCommandsDisabled -and -not [string]::IsNullOrWhiteSpace([string]$Result.Command) -and -not [bool]$Result.ConsoleCommandSent) {
        Add-FailureCode -Codes $codes -Code 'command_send_failed'
    }

    return @($codes)
}

function Get-HangSignals {
    param([Parameter(Mandatory = $true)][string[]]$FailureReasonCodes)

    $hangCodes = @(
        'game_process_missing',
        'game_process_exited',
        'main_menu_timeout',
        'startup_log_stalled',
        'process_unresponsive',
        'command_ack_missing'
    )

    return @($FailureReasonCodes | Where-Object { $hangCodes -contains $_ })
}

function Get-ValueCounts {
    param(
        [AllowEmptyCollection()]$Items,
        [Parameter(Mandatory = $true)][string]$PropertyName
    )

    $counts = [ordered]@{}
    foreach ($item in @($Items)) {
        $value = ''
        if ($null -ne $item -and $item.PSObject.Properties.Name -contains $PropertyName) {
            $value = [string]$item.$PropertyName
        }

        if ([string]::IsNullOrWhiteSpace($value)) {
            $value = '<none>'
        }

        if (-not $counts.Contains($value)) {
            $counts[$value] = 0
        }

        $counts[$value]++
    }

    return $counts
}

function Copy-CurrentGodotLog {
    param(
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if (-not (Test-Path -LiteralPath $godotLogPath -PathType Leaf)) {
        return $false
    }

    Copy-Item -LiteralPath $godotLogPath -Destination $Destination -Force
    return $true
}

function Invoke-LogAudit {
    param(
        [Parameter(Mandatory = $true)][string]$LogPath,
        [Parameter(Mandatory = $true)][string]$OutFile
    )

    & $logAuditScript -Path $LogPath -OutFile $OutFile | Out-Null
    $audit = Get-Content -LiteralPath $OutFile -Raw -Encoding UTF8 | ConvertFrom-Json
    if ($audit -is [array]) {
        return [bool]$audit[0].Clean
    }

    return [bool]$audit.Clean
}

function Invoke-Sts1ModeVerifier {
    param(
        [Parameter(Mandatory = $true)][string]$LogPath,
        [Parameter(Mandatory = $true)][string]$AuditPath,
        [Parameter(Mandatory = $true)][string]$OutFile,
        [Parameter(Mandatory = $true)][string]$TextOutFile
    )

    $args = @(
        '-Mode', $Sts1EventMode,
        '-LogPath', $LogPath,
        '-AuditPath', $AuditPath,
        '-ExpectedPackageVersion', $ExpectedPackageVersion,
        '-ExpectedRitsuCompatBranch', $ExpectedRitsuCompatBranch,
        '-ExpectedRitsuLibVersion', $ExpectedRitsuLibVersion,
        '-ExpectedGameVersion', $ExpectedGameVersion,
        '-OutFile', $OutFile
    )

    & $sts1ModeVerifierScript @args | Out-File -LiteralPath $TextOutFile -Encoding UTF8
    $report = Get-Content -LiteralPath $OutFile -Raw -Encoding UTF8 | ConvertFrom-Json
    return [pscustomobject]@{
        Passed = @($report.Mismatches).Count -eq 0
        Checks = @($report.Checks)
        Mismatches = @($report.Mismatches)
    }
}

function Add-ExpectationCheck {
    param(
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$Checks,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [Parameter(Mandatory = $true)][string]$Detail
    )

    $Checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null
}

function Test-LogExpectations {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $checks = [System.Collections.Generic.List[object]]::new()
    $text = [System.IO.File]::ReadAllText($LogPath)

    if ($ExpectedPackageVersion) {
        Add-ExpectationCheck `
            -Checks $checks `
            -Name 'expected_package_version' `
            -Passed ($text.IndexOf($ExpectedPackageVersion, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) `
            -Detail "expected package version '$ExpectedPackageVersion'"
    }

    if ($ExpectedGameVersion) {
        $gameVersion = $ExpectedGameVersion.Trim()
        $gameVersionWithPrefix = if ($gameVersion.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) { $gameVersion } else { "v$gameVersion" }
        $gamePattern = "(?im)\b(?:release|Host Version|Release Version):?\s*=\s*{0}\b|\bHost Version:\s*{0}\b|\bRelease Version:\s*{0}\b" -f [regex]::Escape($gameVersionWithPrefix)
        Add-ExpectationCheck `
            -Checks $checks `
            -Name 'expected_game_version' `
            -Passed ([regex]::IsMatch($text, $gamePattern)) `
            -Detail "expected game version '$gameVersionWithPrefix' in copied log"
    }

    if ($ExpectedRitsuLibVersion) {
        $version = $ExpectedRitsuLibVersion.TrimStart('v')
        $ritsuPattern = "(?im)\bRitsuLib\s+{0}\s+bootstrap starting\b|\bRitsuLib Version:\s*{0}\s+\[compat branch:" -f [regex]::Escape($version)
        Add-ExpectationCheck `
            -Checks $checks `
            -Name 'expected_ritsulib_version' `
            -Passed ([regex]::IsMatch($text, $ritsuPattern)) `
            -Detail "expected RitsuLib version '$version'"
    }

    if ($ExpectedRitsuCompatBranch) {
        $branchPattern = "(?im)\[compat branch:\s*{0}\]|picked variant\s+{0}\b" -f [regex]::Escape($ExpectedRitsuCompatBranch)
        Add-ExpectationCheck `
            -Checks $checks `
            -Name 'expected_ritsu_compat_branch' `
            -Passed ([regex]::IsMatch($text, $branchPattern)) `
            -Detail "expected RitsuLib compat branch '$ExpectedRitsuCompatBranch'"
    }

    $patchSummary = "Patch application complete: $ExpectedPatchCount applied, 0 ignored, 0 failed, $ExpectedPatchCount total"
    $registeredSummary = "ModPatcher applied $ExpectedPatchCount patches ($ExpectedPatchCount registered)"
    Add-ExpectationCheck `
        -Checks $checks `
        -Name 'expected_spire_plus_patch_count' `
        -Passed (($text.IndexOf($patchSummary, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) -and
            ($text.IndexOf($registeredSummary, [System.StringComparison]::OrdinalIgnoreCase) -ge 0)) `
        -Detail "expected '$patchSummary' and '$registeredSummary'"

    $failed = @($checks | Where-Object { -not $_.Passed })
    return [pscustomobject]@{
        Passed = $failed.Count -eq 0
        Checks = @($checks)
        Mismatches = @($failed | ForEach-Object { "$($_.Name): $($_.Detail)" })
    }
}

function Invoke-LiveSessionRestore {
    param([Parameter(Mandatory = $true)][string]$EvidenceDir)

    & $liveSessionScript `
        -Mode Restore `
        -EvidenceDir $EvidenceDir `
        -GameRoot $GameRoot `
        -SteamExe $SteamExe `
        -StopGameOnRestore `
        -PreserveNewCurrentRunsOnRestore | Out-Null
}

if (-not (Test-Path -LiteralPath $liveSessionScript -PathType Leaf)) {
    throw "Missing live session helper: $liveSessionScript"
}

if (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
    throw "Missing log audit helper: $logAuditScript"
}

$commandCorpusSource = if ($PSBoundParameters.ContainsKey('CommandCorpus')) { 'parameter' } else { "scenario:$Scenario" }
if ($CommandCorpusFile) {
    $commandCorpusPath = Get-ResolvedFullPath -Path $CommandCorpusFile
    if (-not (Test-Path -LiteralPath $commandCorpusPath -PathType Leaf)) {
        throw "Command corpus file not found: $commandCorpusPath"
    }

    $CommandCorpus = @(
        Get-Content -LiteralPath $commandCorpusPath -Encoding UTF8 |
            ForEach-Object { $_.Trim() } |
            Where-Object { $_ -and -not $_.StartsWith('#', [System.StringComparison]::Ordinal) }
    )
    $commandCorpusSource = "file:$commandCorpusPath"
} elseif (-not $PSBoundParameters.ContainsKey('CommandCorpus')) {
    $CommandCorpus = @(Get-ScenarioCommandCorpus -ScenarioName $Scenario)
}

$CommandCorpus = @($CommandCorpus | ForEach-Object { ([string]$_).Trim() } | Where-Object { $_ })
$devConsoleCommandsDisabled = [bool]$NoDevConsoleCommands -or $CommandCorpus.Count -eq 0

if (-not $devConsoleCommandsDisabled -and -not (Test-Path -LiteralPath $consoleCommandScript -PathType Leaf)) {
    throw "Missing console command helper: $consoleCommandScript"
}

if (-not (Test-Path -LiteralPath $sts1ModeVerifierScript -PathType Leaf)) {
    throw "Missing StS1 mode verifier helper: $sts1ModeVerifierScript"
}

if (-not $EvidenceRoot) {
    New-DirectoryIfMissing -Path $runtimeRoot
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $EvidenceRoot = Join-Path $runtimeRoot "monkey-stability-$stamp"
}

$evidenceFull = Get-ResolvedFullPath -Path $EvidenceRoot
New-DirectoryIfMissing -Path $evidenceFull

if (-not $ExpectedPackageVersion) {
    $rootManifest = Read-JsonOrNull -Path (Join-Path $repoRoot 'EZMicroBalance.json')
    if ($rootManifest -and $rootManifest.version) {
        $ExpectedPackageVersion = [string]$rootManifest.version
    }
}

if (-not $ExpectedGameVersion) {
    $gameReleaseInfo = Read-JsonOrNull -Path (Join-Path $GameRoot 'release_info.json')
    if ($gameReleaseInfo -and $gameReleaseInfo.version) {
        $ExpectedGameVersion = Normalize-VersionWithoutPrefix -Version ([string]$gameReleaseInfo.version)
    }
}

if (-not $ExpectedRitsuLibVersion) {
    $ritsuManifest = Read-JsonOrNull -Path (Join-Path $GameRoot 'mods\STS2-RitsuLib\mod_manifest.json')
    if ($ritsuManifest -and $ritsuManifest.version) {
        $ExpectedRitsuLibVersion = Normalize-VersionWithoutPrefix -Version ([string]$ritsuManifest.version)
    }
}

if (-not $ExpectedRitsuCompatBranch) {
    $variantConfig = Read-JsonOrNull -Path (Join-Path $GameRoot 'mods\STS2-RitsuLib\ritsulib-variants.json')
    if ($variantConfig -and $variantConfig.variants -and $ExpectedGameVersion) {
        $expectedGameWithoutPrefix = Normalize-VersionWithoutPrefix -Version $ExpectedGameVersion
        $variant = @($variantConfig.variants | Where-Object {
            [string]$_.compatTarget -eq $expectedGameWithoutPrefix
        } | Select-Object -First 1)
        if ($variant.Count -gt 0) {
            $ExpectedRitsuCompatBranch = [string]$variant[0].compatTarget
        }
    }
}

$random = [System.Random]::new($RandomSeed)
$plannedCommands = for ($i = 1; $i -le $Iterations; $i++) {
    $command = ''
    $commandIndex = -1
    if (-not $devConsoleCommandsDisabled -and $CommandCorpus.Count -gt 0) {
        if ($CommandSelectionMode -eq 'Random') {
            $commandIndex = $random.Next(0, $CommandCorpus.Count)
        } else {
            $commandIndex = ($i - 1) % $CommandCorpus.Count
        }

        $command = $CommandCorpus[$commandIndex]
    }

    [pscustomobject]@{
        Iteration = $i
        Command = $command
        CommandIndex = $commandIndex
        CommandSelectionMode = $CommandSelectionMode
        ScenarioTag = Get-CommandScenarioTag -Command $command
        OwnerArea = Get-CommandOwnerArea -Command $command
        CommandAckPattern = Get-CommandAckPattern -Command $command
    }
}

$plannedCommandCounts = Get-ValueCounts -Items $plannedCommands -PropertyName 'Command'
$plannedScenarioTagCounts = Get-ValueCounts -Items $plannedCommands -PropertyName 'ScenarioTag'
$plannedOwnerAreaCounts = Get-ValueCounts -Items $plannedCommands -PropertyName 'OwnerArea'
$plannedVakuuFightIterationCount = @($plannedCommands | Where-Object { [string]$_.ScenarioTag -eq 'vakuu-fight' }).Count

$plan = [ordered]@{
    HangProbeSchemaVersion = $hangProbeSchemaVersion
    CreatedAt = (Get-Date).ToString('o')
    RepoRoot = $repoRoot
    EvidenceRoot = $evidenceFull
    Iterations = $Iterations
    Launch = [bool]$Launch
    GameRoot = $GameRoot
    SteamExe = $SteamExe
    SteamUserId = $SteamUserId
    Language = $Language
    Sts1EventMode = $Sts1EventMode
    Scenario = $Scenario
    CommandSelectionMode = $CommandSelectionMode
    CommandCorpusSource = $commandCorpusSource
    MainMenuTimeoutSeconds = $MainMenuTimeoutSeconds
    ObservationIntervalSeconds = $ObservationIntervalSeconds
    UnresponsiveSampleThreshold = $UnresponsiveSampleThreshold
    NoLogGrowthTimeoutSeconds = $NoLogGrowthTimeoutSeconds
    PostCommandSeconds = $PostCommandSeconds
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedGameVersion = $ExpectedGameVersion
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    ExpectedPatchCount = $ExpectedPatchCount
    RandomSeed = $RandomSeed
    MoveOtherMods = [bool]$MoveOtherMods
    MoveCurrentRuns = [bool]$MoveCurrentRuns
    NoDevConsoleCommands = [bool]$NoDevConsoleCommands
    EffectiveNoDevConsoleCommands = [bool]$devConsoleCommandsDisabled
    ProcessProbe = [ordered]@{
        ProcessName = 'SlayTheSpire2'
        UsesRespondingProperty = $true
        UsesIsHungAppWindow = $true
        UnresponsiveSampleThreshold = $UnresponsiveSampleThreshold
        FailsOnlyAfterConsecutiveUnresponsiveSamples = $true
        StartupFailsOnProcessExit = $true
        RuntimeFailsOnProcessExit = $true
        RuntimeFailsOnHungWindow = $true
    }
    LogGrowthProbe = [ordered]@{
        StartupFailsOnNoGrowth = $true
        StartupNoGrowthTimeoutSeconds = $NoLogGrowthTimeoutSeconds
        RuntimeLogGrowthIsTelemetryOnly = $true
        DetectsLogResetOrTruncation = $true
    }
    CommandCorpus = @($CommandCorpus)
    PlannedCommandCounts = $plannedCommandCounts
    PlannedScenarioTagCounts = $plannedScenarioTagCounts
    PlannedOwnerAreaCounts = $plannedOwnerAreaCounts
    PlannedVakuuFightIterationCount = $plannedVakuuFightIterationCount
    CommandScenarioMatrix = @($CommandCorpus | ForEach-Object {
        [pscustomobject]@{
            Command = $_
            ScenarioTag = Get-CommandScenarioTag -Command $_
            OwnerArea = Get-CommandOwnerArea -Command $_
            CommandAckPattern = Get-CommandAckPattern -Command $_
        }
    })
    CommandAckPatterns = @($plannedCommands | Where-Object { $_.CommandAckPattern } | ForEach-Object {
        [pscustomobject]@{
            Command = $_.Command
            ScenarioTag = $_.ScenarioTag
            OwnerArea = $_.OwnerArea
            Pattern = $_.CommandAckPattern
        }
    })
    PlannedCommands = @($plannedCommands)
    FailureCriteria = @(
        'main menu log line missing before timeout',
        'SlayTheSpire2 process disappears before or after main menu',
        'SlayTheSpire2 window reports not responding or hung for the configured consecutive-sample threshold',
        'godot.log stops growing before main menu for the configured no-growth timeout',
        'required DevConsole command acknowledgement line is absent from godot.log',
        'godot.log missing or empty',
        'audit-godot-log reports release-blocking signature hits',
        'expected package/game/RitsuLib/patch-count markers are absent from godot.log',
        'StS1 mode verifier reports that actual godot.log mode/package/game/Ritsu shape does not match this run',
        'live-session restore fails',
        'DevConsole command send fails when enabled'
    )
}

Save-Json -InputObject $plan -Path (Join-Path $evidenceFull 'monkey-plan.json')
($CommandCorpus -join [Environment]::NewLine) | Set-Content -LiteralPath (Join-Path $evidenceFull 'command-corpus.txt') -Encoding UTF8

if (-not $Launch) {
    [pscustomobject]@{
        Status = 'planned'
        EvidenceRoot = $evidenceFull
        Iterations = $Iterations
        Scenario = $Scenario
        CommandSelectionMode = $CommandSelectionMode
        CommandCorpusSource = $commandCorpusSource
        EffectiveNoDevConsoleCommands = [bool]$devConsoleCommandsDisabled
        Message = 'Dry-run only. Re-run with -Launch to start Steam sessions.'
    } | ConvertTo-Json -Depth 5
    exit 0
}

$previousSts1Mode = [string]$env:SPIREPLUS_STS1_EVENT_MODE
try {
    if ($Sts1EventMode -eq 'Off') {
        Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
    } else {
        $env:SPIREPLUS_STS1_EVENT_MODE = $Sts1EventMode
    }

    $results = [System.Collections.Generic.List[object]]::new()
    foreach ($planned in $plannedCommands) {
        $iterationDir = Join-Path $evidenceFull ('iteration-{0:D4}' -f [int]$planned.Iteration)
        New-DirectoryIfMissing -Path $iterationDir
        $commandPath = Join-Path $iterationDir 'command.txt'
        [string]$planned.Command | Set-Content -LiteralPath $commandPath -Encoding UTF8
        $probeSamples = [System.Collections.Generic.List[object]]::new()

        $result = [ordered]@{
            HangProbeSchemaVersion = $hangProbeSchemaVersion
            Iteration = [int]$planned.Iteration
            StartedAt = (Get-Date).ToString('o')
            EvidenceDir = $iterationDir
            Command = [string]$planned.Command
            CommandIndex = [int]$planned.CommandIndex
            CommandSelectionMode = [string]$planned.CommandSelectionMode
            Scenario = $Scenario
            ScenarioTag = [string]$planned.ScenarioTag
            OwnerArea = [string]$planned.OwnerArea
            CommandAckPattern = [string]$planned.CommandAckPattern
            CommandAckRequired = -not [string]::IsNullOrWhiteSpace([string]$planned.CommandAckPattern)
            CommandAckObserved = [string]::IsNullOrWhiteSpace([string]$planned.CommandAckPattern)
            RuntimeProbeSamplesPath = Join-Path $iterationDir 'runtime-probe-samples.json'
            GameProcessId = 0
            GameProcessStartTimeUtc = $null
            MainWindowObserved = $false
            MainMenuDetectedAt = $null
            MainMenuElapsedSeconds = 0
            PreLaunchLogLengthBytes = 0L
            PreLaunchLogLastWriteTimeUtc = $null
            MinimumProcessStartTimeUtc = $null
            LogInitialLengthBytes = 0L
            LogFinalLengthBytes = 0L
            LastLogGrowthAt = $null
            MaxSecondsWithoutLogGrowth = 0
            MaxConsecutiveUnresponsiveSamples = 0
            StartupLogProbePassed = $false
            PostCommandLogProbePassed = $false
            ResponsivenessProbePassed = $false
            HangSignals = @()
            FailureReasonCodes = @()
            MainMenuReached = $false
            MainMenuObservationPassed = $false
            MainMenuObservation = $null
            RuntimeObservationPassed = $false
            RuntimeObservation = $null
            LogCopied = $false
            AuditClean = $false
            ExpectationPassed = $false
            ExpectationChecks = @()
            Sts1ModeVerifierPassed = $false
            Sts1ModeVerifierChecks = @()
            Sts1ModeVerifierMismatches = @()
            ConsoleCommandSent = $false
            RestoreSucceeded = $false
            Passed = $false
            Error = ''
        }

        try {
            $minimumProcessStartTimeUtc = (Get-Date).ToUniversalTime()
            $preLaunchLog = Get-LogSnapshot -Path $godotLogPath
            $result.PreLaunchLogLengthBytes = [long]$preLaunchLog.Length
            $result.PreLaunchLogLastWriteTimeUtc = $preLaunchLog.LastWriteTimeUtc
            $result.MinimumProcessStartTimeUtc = $minimumProcessStartTimeUtc.ToString('o')

            $prepareArgs = @(
                '-Mode', 'Prepare',
                '-EvidenceDir', $iterationDir,
                '-GameRoot', $GameRoot,
                '-SteamExe', $SteamExe,
                '-Launch'
            )
            if ($SteamUserId) {
                $prepareArgs += @('-SteamUserId', $SteamUserId)
            }
            if ($Language) {
                $prepareArgs += @('-Language', $Language)
            }
            if ($MoveOtherMods) {
                $prepareArgs += '-MoveOtherMods'
            }
            if ($MoveCurrentRuns) {
                $prepareArgs += '-MoveCurrentRuns'
            }

            & $liveSessionScript @prepareArgs | Out-File -LiteralPath (Join-Path $iterationDir 'prepare-output.json') -Encoding UTF8
            $mainMenuObservation = Wait-ForMainMenuLog `
                -Path $godotLogPath `
                -TimeoutSeconds $MainMenuTimeoutSeconds `
                -NoGrowthTimeoutSeconds $NoLogGrowthTimeoutSeconds `
                -IntervalSeconds $ObservationIntervalSeconds `
                -UnresponsiveSampleThreshold $UnresponsiveSampleThreshold `
                -BaselineLogLengthBytes ([long]$result.PreLaunchLogLengthBytes) `
                -MinimumProcessStartTimeUtc $minimumProcessStartTimeUtc `
                -ProbeSamples $probeSamples
            $result.MainMenuObservation = $mainMenuObservation
            $result.MainMenuReached = [bool]$mainMenuObservation.MainMenuReached
            $result.MainMenuObservationPassed = [bool]$mainMenuObservation.Passed
            $result.StartupLogProbePassed = -not [bool]$mainMenuObservation.NoLogGrowthTimeoutExceeded
            $result.MainMenuDetectedAt = $mainMenuObservation.MainMenuDetectedAt
            $result.MainMenuElapsedSeconds = [double]$mainMenuObservation.ElapsedSeconds
            $result.LogInitialLengthBytes = [long]$mainMenuObservation.LogInitialLengthBytes
            $result.LogFinalLengthBytes = [long]$mainMenuObservation.LogFinalLength
            $result.LastLogGrowthAt = $mainMenuObservation.LastLogGrowthAt
            $result.MaxSecondsWithoutLogGrowth = [int]$mainMenuObservation.MaxNoLogGrowthSeconds
            $result.MaxConsecutiveUnresponsiveSamples = [int]$mainMenuObservation.MaxConsecutiveUnresponsiveSamples
            if ($mainMenuObservation.LastProcess -and $mainMenuObservation.LastProcess.Observed) {
                $result.GameProcessId = [int]$mainMenuObservation.LastProcess.Id
                $result.GameProcessStartTimeUtc = $mainMenuObservation.LastProcess.StartTimeUtc
                $result.MainWindowObserved = [int64]$mainMenuObservation.LastProcess.MainWindowHandle -ne 0
            }

            if ($result.MainMenuReached -and -not $devConsoleCommandsDisabled -and -not [string]::IsNullOrWhiteSpace([string]$planned.Command)) {
                & $consoleCommandScript -Command ([string]$planned.Command) |
                    Out-File -LiteralPath (Join-Path $iterationDir 'console-command-output.json') -Encoding UTF8
                $result.ConsoleCommandSent = $true
                $runtimeObservation = Watch-RuntimeHealth `
                    -Path $godotLogPath `
                    -DurationSeconds $PostCommandSeconds `
                    -IntervalSeconds $ObservationIntervalSeconds `
                    -UnresponsiveSampleThreshold $UnresponsiveSampleThreshold `
                    -MinimumProcessStartTimeUtc $minimumProcessStartTimeUtc `
                    -ProbeSamples $probeSamples
                $result.RuntimeObservation = $runtimeObservation
                $result.RuntimeObservationPassed = [bool]$runtimeObservation.Passed
                $result.PostCommandLogProbePassed = [bool]$runtimeObservation.LogObserved
                $result.MaxSecondsWithoutLogGrowth = [Math]::Max([int]$result.MaxSecondsWithoutLogGrowth, [int]$runtimeObservation.MaxNoLogGrowthSeconds)
                $result.MaxConsecutiveUnresponsiveSamples = [Math]::Max([int]$result.MaxConsecutiveUnresponsiveSamples, [int]$runtimeObservation.MaxConsecutiveUnresponsiveSamples)
                if ($runtimeObservation.LastLogGrowthAt) {
                    $result.LastLogGrowthAt = $runtimeObservation.LastLogGrowthAt
                }
            } elseif ($PostCommandSeconds -gt 0) {
                $runtimeObservation = Watch-RuntimeHealth `
                    -Path $godotLogPath `
                    -DurationSeconds ([Math]::Min($PostCommandSeconds, 10)) `
                    -IntervalSeconds $ObservationIntervalSeconds `
                    -UnresponsiveSampleThreshold $UnresponsiveSampleThreshold `
                    -MinimumProcessStartTimeUtc $minimumProcessStartTimeUtc `
                    -ProbeSamples $probeSamples
                $result.RuntimeObservation = $runtimeObservation
                $result.RuntimeObservationPassed = [bool]$runtimeObservation.Passed
                $result.PostCommandLogProbePassed = [bool]$runtimeObservation.LogObserved
                $result.MaxSecondsWithoutLogGrowth = [Math]::Max([int]$result.MaxSecondsWithoutLogGrowth, [int]$runtimeObservation.MaxNoLogGrowthSeconds)
                $result.MaxConsecutiveUnresponsiveSamples = [Math]::Max([int]$result.MaxConsecutiveUnresponsiveSamples, [int]$runtimeObservation.MaxConsecutiveUnresponsiveSamples)
                if ($runtimeObservation.LastLogGrowthAt) {
                    $result.LastLogGrowthAt = $runtimeObservation.LastLogGrowthAt
                }
            } else {
                $result.RuntimeObservationPassed = $true
                $result.PostCommandLogProbePassed = $true
            }

            $result.ResponsivenessProbePassed = $result.MainMenuObservationPassed -and $result.RuntimeObservationPassed

            $launchLog = Join-Path $iterationDir 'godot.log.after-launch'
            $result.LogCopied = Copy-CurrentGodotLog -Destination $launchLog
            if ($result.LogCopied) {
                $commandAck = Test-CommandAck -LogPath $launchLog -Command ([string]$planned.Command)
                $result.CommandAckRequired = [bool]$commandAck.Required
                $result.CommandAckObserved = [bool]$commandAck.Observed
                $result.CommandAckPattern = [string]$commandAck.Pattern

                $auditPath = Join-Path $iterationDir 'godot-log-audit.json'
                $result.AuditClean = Invoke-LogAudit -LogPath $launchLog -OutFile $auditPath
                $expectations = Test-LogExpectations -LogPath $launchLog
                $result.ExpectationPassed = [bool]$expectations.Passed
                $result.ExpectationChecks = @($expectations.Checks)
                $modeCheck = Invoke-Sts1ModeVerifier `
                    -LogPath $launchLog `
                    -AuditPath $auditPath `
                    -OutFile (Join-Path $iterationDir 'sts1-mode-log-check.json') `
                    -TextOutFile (Join-Path $iterationDir 'sts1-mode-log-check.txt')
                $result.Sts1ModeVerifierPassed = [bool]$modeCheck.Passed
                $result.Sts1ModeVerifierChecks = @($modeCheck.Checks)
                $result.Sts1ModeVerifierMismatches = @($modeCheck.Mismatches)
                $copiedLog = Get-LogSnapshot -Path $launchLog
                $result.LogFinalLengthBytes = [long]$copiedLog.Length
            }
        } catch {
            $result.Error = $_.Exception.Message
        } finally {
            try {
                Invoke-LiveSessionRestore -EvidenceDir $iterationDir
                $result.RestoreSucceeded = $true
            } catch {
                $restoreError = $_.Exception.Message
                if ($result.Error) {
                    $result.Error = "$($result.Error); restore failed: $restoreError"
                } else {
                    $result.Error = "restore failed: $restoreError"
                }
            }
        }

        $result.FinishedAt = (Get-Date).ToString('o')
        $failureCodes = Get-FailureReasonCodes -Result ([pscustomobject]$result)
        $hangSignals = Get-HangSignals -FailureReasonCodes $failureCodes
        $result.FailureReasonCodes = @($failureCodes)
        $result.HangSignals = @($hangSignals)

        Save-Json -InputObject @($probeSamples) -Path $result.RuntimeProbeSamplesPath

        $result.Passed = $result.MainMenuReached -and $result.MainMenuObservationPassed -and $result.RuntimeObservationPassed -and $result.CommandAckObserved -and $result.LogCopied -and $result.AuditClean -and $result.ExpectationPassed -and $result.Sts1ModeVerifierPassed -and $result.RestoreSucceeded -and
            ($devConsoleCommandsDisabled -or [string]::IsNullOrWhiteSpace([string]$planned.Command) -or $result.ConsoleCommandSent)

        Save-Json -InputObject $result -Path (Join-Path $iterationDir 'iteration-result.json')
        $results.Add([pscustomobject]$result) | Out-Null

        if (-not $result.Passed -and $FailOnFirstFailure) {
            break
        }
    }

    $failed = @($results | Where-Object { -not $_.Passed })
    $allFailureCodes = @($results | ForEach-Object { @($_.FailureReasonCodes) } | Where-Object { $_ })
    $failureReasonCounts = [ordered]@{}
    foreach ($code in $allFailureCodes) {
        if (-not $failureReasonCounts.Contains($code)) {
            $failureReasonCounts[$code] = 0
        }

        $failureReasonCounts[$code]++
    }

    $commandCounts = Get-ValueCounts -Items $results -PropertyName 'Command'
    $scenarioTagCounts = Get-ValueCounts -Items $results -PropertyName 'ScenarioTag'
    $ownerAreaCounts = Get-ValueCounts -Items $results -PropertyName 'OwnerArea'

    $summary = [ordered]@{
        HangProbeSchemaVersion = $hangProbeSchemaVersion
        FinishedAt = (Get-Date).ToString('o')
        EvidenceRoot = $evidenceFull
        RequestedIterations = $Iterations
        CompletedIterations = $results.Count
        Passed = $results.Count -gt 0 -and $failed.Count -eq 0 -and $results.Count -eq $Iterations
        FailedIterations = $failed.Count
        FailedIterationIds = @($failed | ForEach-Object { [int]$_.Iteration })
        FailureReasonCounts = $failureReasonCounts
        ProcessExitCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'game_process_exited' }).Count
        UnresponsiveIterationCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'process_unresponsive' }).Count
        LogStallIterationCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'startup_log_stalled' }).Count
        CommandAckMissingCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'command_ack_missing' }).Count
        CommandCounts = $commandCounts
        ScenarioTagCounts = $scenarioTagCounts
        OwnerAreaCounts = $ownerAreaCounts
        VakuuFightIterationCount = @($results | Where-Object { [string]$_.ScenarioTag -eq 'vakuu-fight' }).Count
        MaxMainMenuElapsedSeconds = if ($results.Count -gt 0) { [double](($results | Measure-Object -Property MainMenuElapsedSeconds -Maximum).Maximum) } else { 0 }
        MaxSecondsWithoutLogGrowth = if ($results.Count -gt 0) { [int](($results | Measure-Object -Property MaxSecondsWithoutLogGrowth -Maximum).Maximum) } else { 0 }
        MaxConsecutiveUnresponsiveSamples = if ($results.Count -gt 0) { [int](($results | Measure-Object -Property MaxConsecutiveUnresponsiveSamples -Maximum).Maximum) } else { 0 }
        Results = @($results)
    }

    Save-Json -InputObject $summary -Path (Join-Path $evidenceFull 'monkey-summary.json')
    $summary | ConvertTo-Json -Depth 20

    if ($failed.Count -gt 0) {
        exit 1
    }
} finally {
    if ([string]::IsNullOrEmpty($previousSts1Mode)) {
        Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
    } else {
        $env:SPIREPLUS_STS1_EVENT_MODE = $previousSts1Mode
    }
}
