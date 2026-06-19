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

    [switch]$RequireCurrentSourceSnapshot,

    [switch]$RequireCleanGdreExport,

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
$sourceWorkspaceCheckerScript = Join-Path $PSScriptRoot 'check-local-godot-source-workspace.ps1'
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

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
}

function Test-Sha256Text {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[A-Fa-f0-9]{64}$'
}

function Get-FileBytesAfterOffset {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return [byte[]]::new(0)
    }

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($Offset -lt 0 -or $Offset -gt $bytes.Length) {
        return [byte[]]::new(0)
    }

    $sliceLength = $bytes.Length - [int]$Offset
    $slice = [byte[]]::new($sliceLength)
    if ($sliceLength -gt 0) {
        [System.Array]::Copy($bytes, [int]$Offset, $slice, 0, $sliceLength)
    }

    return $slice
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
    param(
        [datetime]$MinimumStartTimeUtc = [datetime]::MinValue,
        [int]$ExpectedProcessId = 0,
        [AllowEmptyString()][string]$ExpectedProcessStartTimeUtc = '',
        [AllowEmptyString()][string]$ExpectedProcessPath = ''
    )

    $processes = @(Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue)
    $currentProcesses = [System.Collections.Generic.List[object]]::new()
    $staleProcessCount = 0
    $unknownStartTimeProcessCount = 0
    $earliestStaleProcessStartTimeUtc = $null
    $hasExpectedProcessId = $ExpectedProcessId -gt 0
    $expectedProcessPathFull = ''
    if (-not [string]::IsNullOrWhiteSpace($ExpectedProcessPath)) {
        try {
            $expectedProcessPathFull = [System.IO.Path]::GetFullPath($ExpectedProcessPath)
        } catch {
            $expectedProcessPathFull = $ExpectedProcessPath
        }
    }

    foreach ($candidate in $processes) {
        $candidateStartTimeUtc = $null
        try {
            $candidateStartTimeUtc = $candidate.StartTime.ToUniversalTime()
        } catch {
            $candidateStartTimeUtc = $null
        }

        if ($null -eq $candidateStartTimeUtc) {
            $staleProcessCount++
            $unknownStartTimeProcessCount++
            continue
        }

        if ($candidateStartTimeUtc -lt $MinimumStartTimeUtc) {
            $staleProcessCount++
            if ($null -eq $earliestStaleProcessStartTimeUtc -or $candidateStartTimeUtc -lt $earliestStaleProcessStartTimeUtc) {
                $earliestStaleProcessStartTimeUtc = $candidateStartTimeUtc
            }

            continue
        }

        $currentProcesses.Add($candidate) | Out-Null
    }

    $currentProcessCount = $currentProcesses.Count
    if ($currentProcessCount -gt 1) {
        return [pscustomobject]@{
            Observed = $false
            ProcessName = ''
            Id = 0
            StartTimeUtc = $null
            ProcessPath = ''
            ExpectedProcessId = $ExpectedProcessId
            ExpectedProcessStartTimeUtc = $ExpectedProcessStartTimeUtc
            ExpectedProcessPath = $ExpectedProcessPath
            ProcessIdMatchesExpected = -not $hasExpectedProcessId
            ProcessStartTimeMatchesExpected = [string]::IsNullOrWhiteSpace($ExpectedProcessStartTimeUtc)
            ProcessPathMatchesExpected = [string]::IsNullOrWhiteSpace($ExpectedProcessPath)
            ProcessIdentityMatchesExpected = $false
            MainWindowHandle = 0
            MainWindowTitle = ''
            Responding = $false
            HungWindow = $false
            Error = "Observed $currentProcessCount current SlayTheSpire2 process(es); shared godot.log cannot be attributed to one launched process."
            MinimumStartTimeUtc = $MinimumStartTimeUtc.ToString('o')
            StaleProcessCount = $staleProcessCount + $currentProcessCount
            CurrentProcessCount = $currentProcessCount
            UnknownStartTimeProcessCount = $unknownStartTimeProcessCount
            AmbiguousCurrentProcessCount = $currentProcessCount
            EarliestStaleProcessStartTimeUtc = if ($earliestStaleProcessStartTimeUtc) { $earliestStaleProcessStartTimeUtc.ToString('o') } else { $null }
        }
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
            StartTimeUtc = $null
            ProcessPath = ''
            ExpectedProcessId = $ExpectedProcessId
            ExpectedProcessStartTimeUtc = $ExpectedProcessStartTimeUtc
            ExpectedProcessPath = $ExpectedProcessPath
            ProcessIdMatchesExpected = -not $hasExpectedProcessId
            ProcessStartTimeMatchesExpected = [string]::IsNullOrWhiteSpace($ExpectedProcessStartTimeUtc)
            ProcessPathMatchesExpected = [string]::IsNullOrWhiteSpace($ExpectedProcessPath)
            ProcessIdentityMatchesExpected = -not $hasExpectedProcessId -and [string]::IsNullOrWhiteSpace($ExpectedProcessStartTimeUtc) -and [string]::IsNullOrWhiteSpace($ExpectedProcessPath)
            MainWindowHandle = 0
            MainWindowTitle = ''
            Responding = $null
            HungWindow = $false
            Error = ''
            MinimumStartTimeUtc = $MinimumStartTimeUtc.ToString('o')
            StaleProcessCount = $staleProcessCount
            CurrentProcessCount = $currentProcessCount
            UnknownStartTimeProcessCount = $unknownStartTimeProcessCount
            AmbiguousCurrentProcessCount = 0
            EarliestStaleProcessStartTimeUtc = if ($earliestStaleProcessStartTimeUtc) { $earliestStaleProcessStartTimeUtc.ToString('o') } else { $null }
        }
    }

    $selected = $process[0]
    $handle = [IntPtr]$selected.MainWindowHandle
    $responding = $null
    $hungWindow = $false
    $error = ''
    $startTimeUtc = $null
    $processPath = ''

    try {
        $startTimeUtc = $selected.StartTime.ToUniversalTime().ToString('o')
    } catch {
        $startTimeUtc = $null
    }

    try {
        $processPath = [string]$selected.Path
    } catch {
        $processPath = ''
    }

    $processIdMatchesExpected = -not $hasExpectedProcessId -or [int]$selected.Id -eq $ExpectedProcessId
    $processStartTimeMatchesExpected = $true
    if (-not [string]::IsNullOrWhiteSpace($ExpectedProcessStartTimeUtc)) {
        $processStartTimeMatchesExpected = [string]::Equals($startTimeUtc, $ExpectedProcessStartTimeUtc, [System.StringComparison]::OrdinalIgnoreCase)
    }

    $processPathMatchesExpected = $true
    if (-not [string]::IsNullOrWhiteSpace($ExpectedProcessPath)) {
        $processPathFull = ''
        try {
            $processPathFull = [System.IO.Path]::GetFullPath($processPath)
        } catch {
            $processPathFull = $processPath
        }

        $processPathMatchesExpected = -not [string]::IsNullOrWhiteSpace($processPathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($processPathFull, $expectedProcessPathFull)
    }

    $processIdentityMatchesExpected = $processIdMatchesExpected -and $processStartTimeMatchesExpected -and $processPathMatchesExpected

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
        ProcessPath = $processPath
        ExpectedProcessId = $ExpectedProcessId
        ExpectedProcessStartTimeUtc = $ExpectedProcessStartTimeUtc
        ExpectedProcessPath = $ExpectedProcessPath
        ProcessIdMatchesExpected = $processIdMatchesExpected
        ProcessStartTimeMatchesExpected = $processStartTimeMatchesExpected
        ProcessPathMatchesExpected = $processPathMatchesExpected
        ProcessIdentityMatchesExpected = $processIdentityMatchesExpected
        MainWindowHandle = [int64]$selected.MainWindowHandle
        MainWindowTitle = $selected.MainWindowTitle
        Responding = $responding
        HungWindow = $hungWindow
        Error = $error
        MinimumStartTimeUtc = $MinimumStartTimeUtc.ToString('o')
        StaleProcessCount = $staleProcessCount
        CurrentProcessCount = $currentProcessCount
        UnknownStartTimeProcessCount = $unknownStartTimeProcessCount
        AmbiguousCurrentProcessCount = 0
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
        ProcessStartTimeUtc = $ProcessSnapshot.StartTimeUtc
        ProcessPath = $ProcessSnapshot.ProcessPath
        ExpectedGameProcessId = [int]$ProcessSnapshot.ExpectedProcessId
        ExpectedGameProcessStartTimeUtc = $ProcessSnapshot.ExpectedProcessStartTimeUtc
        ExpectedGameProcessPath = $ProcessSnapshot.ExpectedProcessPath
        ProcessIdMatchesExpected = [bool]$ProcessSnapshot.ProcessIdMatchesExpected
        ProcessStartTimeMatchesExpected = [bool]$ProcessSnapshot.ProcessStartTimeMatchesExpected
        ProcessPathMatchesExpected = [bool]$ProcessSnapshot.ProcessPathMatchesExpected
        ProcessIdentityMatchesExpected = [bool]$ProcessSnapshot.ProcessIdentityMatchesExpected
        MinimumProcessStartTimeUtc = $ProcessSnapshot.MinimumStartTimeUtc
        StaleProcessCount = [int]$ProcessSnapshot.StaleProcessCount
        CurrentProcessCount = [int]$ProcessSnapshot.CurrentProcessCount
        UnknownStartTimeProcessCount = [int]$ProcessSnapshot.UnknownStartTimeProcessCount
        AmbiguousCurrentProcessCount = [int]$ProcessSnapshot.AmbiguousCurrentProcessCount
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
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$ProbeSamples,
        [int]$ExpectedProcessId = 0,
        [AllowEmptyString()][string]$ExpectedProcessStartTimeUtc = '',
        [AllowEmptyString()][string]$ExpectedProcessPath = ''
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
    $staleProcessObserved = $false
    $maxStaleProcessCount = 0
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

        $lastProcess = Get-SpireProcessSnapshot `
            -MinimumStartTimeUtc $MinimumProcessStartTimeUtc `
            -ExpectedProcessId $ExpectedProcessId `
            -ExpectedProcessStartTimeUtc $ExpectedProcessStartTimeUtc `
            -ExpectedProcessPath $ExpectedProcessPath
        Add-ProbeSample -Samples $ProbeSamples -Phase 'StartupMainMenu' -LogSnapshot $lastLog -ProcessSnapshot $lastProcess
        $sampleStaleProcessCount = [int]$lastProcess.StaleProcessCount
        if ($sampleStaleProcessCount -gt 0) {
            $staleProcessObserved = $true
            $maxStaleProcessCount = [Math]::Max($maxStaleProcessCount, $sampleStaleProcessCount)
            $failureReason = "Observed $sampleStaleProcessCount pre-existing SlayTheSpire2 process(es); shared godot.log cannot be trusted for this iteration."
            break
        }

        if ($lastProcess.Observed) {
            $processObserved = $true
            if (-not [bool]$lastProcess.ProcessIdentityMatchesExpected) {
                $failureReason = 'Observed SlayTheSpire2 process identity did not match the live-session selected game process.'
                break
            }
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
                StaleProcessObserved = $staleProcessObserved
                MaxStaleProcessCount = $maxStaleProcessCount
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
        StaleProcessObserved = $staleProcessObserved
        MaxStaleProcessCount = $maxStaleProcessCount
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
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[object]]$ProbeSamples,
        [Parameter(Mandatory = $true)][bool]$RequireLogGrowth,
        [int]$ExpectedProcessId = 0,
        [AllowEmptyString()][string]$ExpectedProcessStartTimeUtc = '',
        [AllowEmptyString()][string]$ExpectedProcessPath = ''
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
    $staleProcessObserved = $false
    $maxStaleProcessCount = 0
    $lastProcess = $null
    $failureReason = ''

    do {
        $sampleCount++
        $lastProcess = Get-SpireProcessSnapshot `
            -MinimumStartTimeUtc $MinimumProcessStartTimeUtc `
            -ExpectedProcessId $ExpectedProcessId `
            -ExpectedProcessStartTimeUtc $ExpectedProcessStartTimeUtc `
            -ExpectedProcessPath $ExpectedProcessPath
        $lastLog = Get-LogSnapshot -Path $Path
        Add-ProbeSample -Samples $ProbeSamples -Phase 'PostCommandRuntime' -LogSnapshot $lastLog -ProcessSnapshot $lastProcess
        $sampleStaleProcessCount = [int]$lastProcess.StaleProcessCount
        if ($sampleStaleProcessCount -gt 0) {
            $staleProcessObserved = $true
            $maxStaleProcessCount = [Math]::Max($maxStaleProcessCount, $sampleStaleProcessCount)
            $failureReason = "Observed $sampleStaleProcessCount pre-existing SlayTheSpire2 process(es) during runtime observation."
            break
        }

        if ($lastProcess.Observed) {
            $processObserved = $true
            if (-not [bool]$lastProcess.ProcessIdentityMatchesExpected) {
                $failureReason = 'Observed SlayTheSpire2 process identity did not match the live-session selected game process during runtime observation.'
                break
            }
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
    $noLogGrowthTimeoutExceeded = $RequireLogGrowth -and -not $logGrew
    $logGrowthSatisfied = -not $RequireLogGrowth -or $logGrew
    $passed = $processObserved -and -not $processExitedAfterObservation -and -not $hungWindowDetected -and -not $staleProcessObserved -and $logGrowthSatisfied
    if (-not $failureReason -and -not $processObserved) {
        $failureReason = 'SlayTheSpire2 process was not observed during runtime observation.'
    } elseif (-not $failureReason -and $RequireLogGrowth -and -not $logGrew) {
        $failureReason = 'godot.log did not grow during runtime observation.'
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
        RuntimeLogGrowthRequired = $RequireLogGrowth
        NoLogGrowthTimeoutExceeded = $noLogGrowthTimeoutExceeded
        LogObserved = [bool]$lastLog.Exists
        LogInitialLengthBytes = [long]$initialLog.Length
        LogFinalLengthBytes = [long]$lastLog.Length
        LogGrew = $logGrew
        LastLogGrowthAt = if ($lastGrowthAt) { $lastGrowthAt.ToString('o') } else { $null }
        LogResetObserved = $logResetObserved
        MaxNoLogGrowthSeconds = $maxNoGrowthSeconds
        MaxConsecutiveUnresponsiveSamples = $maxConsecutiveUnresponsiveSamples
        StaleProcessObserved = $staleProcessObserved
        MaxStaleProcessCount = $maxStaleProcessCount
        LastProcess = $lastProcess
        MinimumProcessStartTimeUtc = $MinimumProcessStartTimeUtc.ToString('o')
    }
}

function Get-CommandAckPattern {
    param([AllowEmptyString()][string]$Command)

    if ([string]::IsNullOrWhiteSpace($Command)) {
        return ''
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+VAKUU\s+confirm\s+fight\b') {
        return '\[SPIREPLUS-EVIDENCE\]\s+VakuuFight\s+fight_option_shown\b'
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
            return 'Ancients.Vakuu.FightOptionSetup'
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
        $runtimeLogGrowthRequired = -not [string]::IsNullOrWhiteSpace([string]$Result.Command) -or [bool]$Result.CommandAckRequired
        if ($runtime.PSObject.Properties.Name -contains 'RuntimeLogGrowthRequired') {
            $runtimeLogGrowthRequired = [bool]$runtime.RuntimeLogGrowthRequired
        }
        if ([bool]$runtime.NoLogGrowthTimeoutExceeded -or ($runtimeLogGrowthRequired -and -not [bool]$runtime.LogGrew)) { Add-FailureCode -Codes $codes -Code 'runtime_log_stalled' }
    }

    if (($main -and [bool]$main.StaleProcessObserved) -or
        ($runtime -and [bool]$runtime.StaleProcessObserved) -or
        [bool]$Result.StaleProcessObserved -or
        [int]$Result.StaleProcessCount -gt 0) {
        Add-FailureCode -Codes $codes -Code 'stale_process_observed'
    }

    if ($Result.MainMenuReached -and -not [bool]$Result.MainWindowObserved) {
        Add-FailureCode -Codes $codes -Code 'main_window_missing'
    }

    if ([string]::IsNullOrWhiteSpace([string]$Result.LiveSessionPrepareOutputSha256) -or
        -not (Test-Path -LiteralPath ([string]$Result.LiveSessionPrepareOutputPath) -PathType Leaf)) {
        Add-FailureCode -Codes $codes -Code 'live_session_prepare_output_missing'
    }

    if ([int]$Result.LiveSessionLaunchedProcessId -le 0 -or
        [string]::IsNullOrWhiteSpace([string]$Result.LiveSessionLaunchedAt) -or
        [string]::IsNullOrWhiteSpace([string]$Result.LiveSessionLauncherKind)) {
        Add-FailureCode -Codes $codes -Code 'live_session_launch_metadata_missing'
    }

    if ([int]$Result.LiveSessionPidAttributionSchemaVersion -le 0) {
        Add-FailureCode -Codes $codes -Code 'live_session_pid_attribution_missing'
    }

    if (-not [bool]$Result.LiveSessionPidAttributionPassed -or
        [int]$Result.LiveSessionSelectedGameProcessId -le 0 -or
        [string]::IsNullOrWhiteSpace([string]$Result.LiveSessionSelectedGameProcessStartTimeUtc) -or
        [string]::IsNullOrWhiteSpace([string]$Result.LiveSessionSelectedGameProcessPath)) {
        Add-FailureCode -Codes $codes -Code 'live_session_pid_attribution_failed'
    }

    if ($Result.MainMenuReached -and -not [bool]$Result.GameProcessStartTimeAfterLiveSessionLaunch) {
        Add-FailureCode -Codes $codes -Code 'game_process_start_time_unbound'
    }

    if ($Result.MainMenuReached -and [string]::IsNullOrWhiteSpace([string]$Result.GameProcessPath)) {
        Add-FailureCode -Codes $codes -Code 'game_process_path_missing'
    }

    if ($Result.MainMenuReached -and -not [bool]$Result.GameProcessIdMatchesLiveSession) {
        Add-FailureCode -Codes $codes -Code 'game_process_id_mismatch'
    }

    if ($Result.MainMenuReached -and -not [bool]$Result.GameProcessStartTimeMatchesLiveSession) {
        Add-FailureCode -Codes $codes -Code 'game_process_start_time_mismatch'
    }

    if ($Result.MainMenuReached -and -not [bool]$Result.GameProcessPathMatchesLiveSession) {
        Add-FailureCode -Codes $codes -Code 'game_process_path_mismatch'
    }

    if ($Result.CommandAckRequired -and -not $Result.CommandAckObserved) {
        Add-FailureCode -Codes $codes -Code 'command_ack_missing'
    }

    if (-not [bool]$Result.GodotLogBeforeCopied) { Add-FailureCode -Codes $codes -Code 'godot_log_before_missing' }
    if (-not [bool]$Result.LogCopied) { Add-FailureCode -Codes $codes -Code 'godot_log_missing' }
    if (-not [bool]$Result.CurrentIterationLogCopied) { Add-FailureCode -Codes $codes -Code 'current_iteration_log_missing' }
    if (-not [bool]$Result.AuditClean) { Add-FailureCode -Codes $codes -Code 'log_audit_failed' }
    if (-not [bool]$Result.ExpectationPassed) { Add-FailureCode -Codes $codes -Code 'runtime_expectation_mismatch' }
    if (-not [bool]$Result.Sts1ModeVerifierPassed) { Add-FailureCode -Codes $codes -Code 'sts1_mode_mismatch' }
    if (-not [bool]$Result.RestoreSucceeded) { Add-FailureCode -Codes $codes -Code 'restore_failed' }
    if ([bool]$Result.RestoreSucceeded) {
        $sessionStatePath = [string]$Result.LiveSessionSessionStatePath
        $restoreStatePath = [string]$Result.LiveSessionRestoreStatePath
        if ([string]::IsNullOrWhiteSpace([string]$Result.LiveSessionSessionStateSha256) -or
            [string]::IsNullOrWhiteSpace($sessionStatePath) -or
            -not (Test-Path -LiteralPath $sessionStatePath -PathType Leaf)) {
            Add-FailureCode -Codes $codes -Code 'live_session_session_state_missing'
        }
        if ([string]::IsNullOrWhiteSpace([string]$Result.LiveSessionRestoreStateSha256) -or
            [string]::IsNullOrWhiteSpace($restoreStatePath) -or
            -not (Test-Path -LiteralPath $restoreStatePath -PathType Leaf) -or
            [int]$Result.LiveSessionRestoreSchemaVersion -le 0) {
            Add-FailureCode -Codes $codes -Code 'live_session_restore_state_missing'
        }
        if ([int]$Result.LiveSessionPostRestoreSlayProcessCount -gt 0 -or
            [int]$Result.LiveSessionPostRestoreGodotProcessCount -gt 0) {
            Add-FailureCode -Codes $codes -Code 'post_restore_process_leak'
        }
        if (-not [bool]$Result.LiveSessionRestoreItemCountsMatch) {
            Add-FailureCode -Codes $codes -Code 'restore_item_count_mismatch'
        }
        if ([int]$Result.LiveSessionPreservedNewCurrentRunCount -gt 0 -and
            -not [bool]$Result.LiveSessionPreservedNewCurrentRunsManifestBound) {
            Add-FailureCode -Codes $codes -Code 'preserved_current_runs_manifest_missing'
        }
        if ([bool]$Result.MainMenuReached -and
            [int]$Result.LiveSessionSelectedGameProcessId -gt 0 -and
            -not [bool]$Result.LiveSessionStoppedSelectedGameProcess) {
            Add-FailureCode -Codes $codes -Code 'selected_game_process_not_stopped'
        }
        if (-not [bool]$Result.LiveSessionSettingsRestoredFromBackup -or
            -not [bool]$Result.LiveSessionSettingsBackupRestoredFromBackup) {
            Add-FailureCode -Codes $codes -Code 'restore_settings_hash_mismatch'
        }
    }
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
        'runtime_log_stalled',
        'process_unresponsive',
        'live_session_prepare_output_missing',
        'live_session_launch_metadata_missing',
        'live_session_pid_attribution_missing',
        'live_session_pid_attribution_failed',
        'game_process_start_time_unbound',
        'game_process_path_missing',
        'game_process_id_mismatch',
        'game_process_start_time_mismatch',
        'game_process_path_mismatch',
        'godot_log_before_missing',
        'current_iteration_log_missing',
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

function Copy-BaselineGodotLog {
    param(
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if (Test-Path -LiteralPath $godotLogPath -PathType Leaf) {
        Copy-Item -LiteralPath $godotLogPath -Destination $Destination -Force
    } else {
        [System.IO.File]::WriteAllBytes($Destination, [byte[]]::new(0))
    }

    return Test-Path -LiteralPath $Destination -PathType Leaf
}

function Write-CurrentIterationLogSlice {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    if (-not (Test-Path -LiteralPath $Source -PathType Leaf)) {
        return $false
    }

    $bytes = Get-FileBytesAfterOffset -Path $Source -Offset $Offset
    [System.IO.File]::WriteAllBytes($Destination, $bytes)
    return (Test-Path -LiteralPath $Destination -PathType Leaf) -and (Get-Item -LiteralPath $Destination).Length -gt 0
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

function Update-LiveSessionRestoreStateFields {
    param([Parameter(Mandatory = $true)]$Result)

    $Result.LiveSessionSessionStateSha256 = Get-FileSha256OrEmpty -Path ([string]$Result.LiveSessionSessionStatePath)
    $Result.LiveSessionRestoreStateSha256 = Get-FileSha256OrEmpty -Path ([string]$Result.LiveSessionRestoreStatePath)

    $sessionState = Read-JsonOrNull -Path ([string]$Result.LiveSessionSessionStatePath)
    $restoreState = Read-JsonOrNull -Path ([string]$Result.LiveSessionRestoreStatePath)
    if ($null -eq $restoreState) {
        return
    }

    if ($restoreState.PSObject.Properties.Name -contains 'RestoreSchemaVersion' -and $null -ne $restoreState.RestoreSchemaVersion) {
        $Result.LiveSessionRestoreSchemaVersion = [int]$restoreState.RestoreSchemaVersion
    }
    if ($restoreState.PSObject.Properties.Name -contains 'StoppedSelectedGameProcess') {
        $Result.LiveSessionStoppedSelectedGameProcess = [bool]$restoreState.StoppedSelectedGameProcess
    }
    if ($restoreState.PSObject.Properties.Name -contains 'RestoredModCount' -and $null -ne $restoreState.RestoredModCount) {
        $Result.LiveSessionRestoredModCount = [int]$restoreState.RestoredModCount
    }
    if ($restoreState.PSObject.Properties.Name -contains 'RestoredCurrentRunCount' -and $null -ne $restoreState.RestoredCurrentRunCount) {
        $Result.LiveSessionRestoredCurrentRunCount = [int]$restoreState.RestoredCurrentRunCount
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PreservedNewCurrentRunCount' -and $null -ne $restoreState.PreservedNewCurrentRunCount) {
        $Result.LiveSessionPreservedNewCurrentRunCount = [int]$restoreState.PreservedNewCurrentRunCount
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PreservedNewCurrentRunsManifestPath') {
        $Result.LiveSessionPreservedNewCurrentRunsManifestPath = [string]$restoreState.PreservedNewCurrentRunsManifestPath
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PreservedNewCurrentRunsManifestSha256') {
        $Result.LiveSessionPreservedNewCurrentRunsManifestSha256 = [string]$restoreState.PreservedNewCurrentRunsManifestSha256
    }
    $preservedManifestPath = [string]$Result.LiveSessionPreservedNewCurrentRunsManifestPath
    $preservedManifestSha256 = [string]$Result.LiveSessionPreservedNewCurrentRunsManifestSha256
    $Result.LiveSessionPreservedNewCurrentRunsManifestBound = [int]$Result.LiveSessionPreservedNewCurrentRunCount -eq 0
    if ([int]$Result.LiveSessionPreservedNewCurrentRunCount -gt 0 -and
        -not [string]::IsNullOrWhiteSpace($preservedManifestPath) -and
        (Test-Path -LiteralPath $preservedManifestPath -PathType Leaf) -and
        (Test-Sha256Text -Value $preservedManifestSha256) -and
        [System.StringComparer]::OrdinalIgnoreCase.Equals($preservedManifestSha256, (Get-FileSha256OrEmpty -Path $preservedManifestPath))) {
        $Result.LiveSessionPreservedNewCurrentRunsManifestBound = $true
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PostRestoreSlayProcessCount' -and $null -ne $restoreState.PostRestoreSlayProcessCount) {
        $Result.LiveSessionPostRestoreSlayProcessCount = [int]$restoreState.PostRestoreSlayProcessCount
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PostRestoreSlayProcessIds') {
        $Result.LiveSessionPostRestoreSlayProcessIds = @($restoreState.PostRestoreSlayProcessIds)
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PostRestoreGodotProcessCount' -and $null -ne $restoreState.PostRestoreGodotProcessCount) {
        $Result.LiveSessionPostRestoreGodotProcessCount = [int]$restoreState.PostRestoreGodotProcessCount
    }
    if ($restoreState.PSObject.Properties.Name -contains 'PostRestoreGodotProcessIds') {
        $Result.LiveSessionPostRestoreGodotProcessIds = @($restoreState.PostRestoreGodotProcessIds)
    }
    if ($restoreState.PSObject.Properties.Name -contains 'SettingsHashAfterRestore') {
        $Result.LiveSessionSettingsHashAfterRestore = [string]$restoreState.SettingsHashAfterRestore
    }
    if ($restoreState.PSObject.Properties.Name -contains 'SettingsBackupHashAfterRestore') {
        $Result.LiveSessionSettingsBackupHashAfterRestore = [string]$restoreState.SettingsBackupHashAfterRestore
    }
    if ($restoreState.PSObject.Properties.Name -contains 'SettingsBackupExistsAfterRestore') {
        $Result.LiveSessionSettingsBackupExistsAfterRestoreRecorded = $true
        $Result.LiveSessionSettingsBackupExistsAfterRestore = [bool]$restoreState.SettingsBackupExistsAfterRestore
    }

    if ($null -ne $sessionState) {
        $movedMods = if ($sessionState.PSObject.Properties.Name -contains 'MovedMods') { @($sessionState.MovedMods) } else { @() }
        $movedCurrentRuns = if ($sessionState.PSObject.Properties.Name -contains 'MovedCurrentRuns') { @($sessionState.MovedCurrentRuns) } else { @() }
        $movedModCount = @($movedMods).Count
        $movedCurrentRunCount = @($movedCurrentRuns).Count
        $Result.LiveSessionMovedModCount = [int]$movedModCount
        $Result.LiveSessionMovedCurrentRunCount = [int]$movedCurrentRunCount
        $Result.LiveSessionRestoreItemCountsMatch =
            [int]$Result.LiveSessionRestoredModCount -eq [int]$movedModCount -and
            [int]$Result.LiveSessionRestoredCurrentRunCount -eq [int]$movedCurrentRunCount

        $settingsHashBefore = [string]$sessionState.SettingsHashBefore
        $settingsBackupHashBefore = [string]$sessionState.SettingsBackupHashBefore
        $settingsBackupExistedBefore = if ($sessionState.PSObject.Properties.Name -contains 'SettingsBackupExistedBefore') {
            [bool]$sessionState.SettingsBackupExistedBefore
        } else {
            -not [string]::IsNullOrWhiteSpace($settingsBackupHashBefore)
        }
        $Result.LiveSessionSettingsBackupExistedBefore = [bool]$settingsBackupExistedBefore
        $Result.LiveSessionSettingsRestoredFromBackup =
            (-not [string]::IsNullOrWhiteSpace($settingsHashBefore)) -and
            [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$Result.LiveSessionSettingsHashAfterRestore, $settingsHashBefore)
        $Result.LiveSessionSettingsBackupRestoredFromBackup = if ($settingsBackupExistedBefore) {
            (-not [string]::IsNullOrWhiteSpace($settingsBackupHashBefore)) -and
            [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$Result.LiveSessionSettingsBackupHashAfterRestore, $settingsBackupHashBefore)
        } else {
            [bool]$Result.LiveSessionSettingsBackupExistsAfterRestoreRecorded -and
            -not [bool]$Result.LiveSessionSettingsBackupExistsAfterRestore -and
            [string]::IsNullOrWhiteSpace([string]$Result.LiveSessionSettingsBackupHashAfterRestore)
        }
    }
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

if (-not (Test-Path -LiteralPath $sourceWorkspaceCheckerScript -PathType Leaf)) {
    throw "Missing source workspace checker helper: $sourceWorkspaceCheckerScript"
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

$sourceWorkspaceCheckPath = Join-Path $evidenceFull 'local-godot-source-workspace-check.json'
$sourceWorkspaceCheckOutputPath = Join-Path $evidenceFull 'local-godot-source-workspace-check.txt'
$sourceWorkspaceArgs = @{
    SourceRoot = (Join-Path $repoRoot 'source code')
    GameRoot = $GameRoot
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedGameVersion = $ExpectedGameVersion
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    OutFile = $sourceWorkspaceCheckPath
}
if ($RequireCurrentSourceSnapshot) {
    $sourceWorkspaceArgs.RequireCurrentSourceSnapshot = $true
}
if ($RequireCleanGdreExport) {
    $sourceWorkspaceArgs.RequireCleanGdreExport = $true
}
if ($RequireCurrentSourceSnapshot -or $RequireCleanGdreExport) {
    $sourceWorkspaceArgs.FailOnMismatch = $true
}

& $sourceWorkspaceCheckerScript @sourceWorkspaceArgs |
    Out-File -LiteralPath $sourceWorkspaceCheckOutputPath -Encoding UTF8

$sourceWorkspaceReport = Read-JsonOrNull -Path $sourceWorkspaceCheckPath
$sourceWorkspaceCheckHash = if (Test-Path -LiteralPath $sourceWorkspaceCheckPath -PathType Leaf) {
    (Get-FileHash -Algorithm SHA256 -LiteralPath $sourceWorkspaceCheckPath).Hash.ToLowerInvariant()
} else {
    ''
}
$sourceWorkspaceSummary = [ordered]@{
    Checked = $null -ne $sourceWorkspaceReport
    ReportPath = $sourceWorkspaceCheckPath
    OutputPath = $sourceWorkspaceCheckOutputPath
    ReportSha256 = $sourceWorkspaceCheckHash
    Passed = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.Passed } else { $false }
    SourceRoot = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.SourceRoot } else { '' }
    SourceVersion = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.Version } else { '' }
    SourceCommit = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.Commit } else { '' }
    SourceBranch = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.Branch } else { '' }
    SourceMainAssemblyHash = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.MainAssemblyHash } else { '' }
    InstalledGameVersion = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.Game.Version } else { '' }
    InstalledGameCommit = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.Game.Commit } else { '' }
    InstalledGameBranch = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.Game.Branch } else { '' }
    InstalledGameMainAssemblyHash = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.Game.MainAssemblyHash } else { '' }
    Disposition = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.Disposition } else { '' }
    MatchesInstalledGame = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.RecoveredSource.MatchesInstalledGame } else { $false }
    OriginPckPath = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RecoveredSource.OriginPckPath } else { '' }
    OriginMatchesInstalledGamePck = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.RecoveredSource.OriginMatchesInstalledGamePck } else { $false }
    RitsuLibVersion = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.Version } else { '' }
    RitsuLibCompatBranch = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.CompatBranch } else { '' }
    RitsuLibManifestPath = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.ManifestPath } else { '' }
    RitsuLibManifestSha256 = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.ManifestSha256 } else { '' }
    RitsuLibVariantsPath = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.VariantsPath } else { '' }
    RitsuLibVariantsSha256 = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.VariantsSha256 } else { '' }
    RitsuLibVariantDllPath = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.VariantDllPath } else { '' }
    RitsuLibVariantDllSha256 = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.VariantDllSha256 } else { '' }
    RitsuLibExpectedVariantDllSha256 = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.ExpectedVariantDllSha256 } else { '' }
    RitsuLibCompatTargetPath = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.CompatTargetPath } else { '' }
    RitsuLibCompatTargetText = if ($sourceWorkspaceReport) { [string]$sourceWorkspaceReport.RitsuLib.CompatTargetText } else { '' }
    RefreshSourceSnapshotBeforeCurrentApiClaims = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.EvidenceUsePolicy.RefreshSourceSnapshotBeforeCurrentApiClaims } else { $true }
    NotRuntimeProof = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.EvidenceUsePolicy.NotRuntimeProof } else { $true }
    AuthorizedSourceOriginVerified = if ($sourceWorkspaceReport) { [bool]$sourceWorkspaceReport.EvidenceUsePolicy.AuthorizedSourceOriginVerified } else { $false }
    RequireCurrentSourceSnapshot = [bool]$RequireCurrentSourceSnapshot
    RequireCleanGdreExport = [bool]$RequireCleanGdreExport
}

$random = [System.Random]::new($RandomSeed)
$runnerScriptPath = [System.IO.Path]::GetFullPath($PSCommandPath)
$runnerScriptSha256 = Get-FileSha256OrEmpty -Path $runnerScriptPath
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
$commandCorpusPath = Join-Path $evidenceFull 'command-corpus.txt'
$commandCorpusText = $CommandCorpus -join [Environment]::NewLine
Set-Content -LiteralPath $commandCorpusPath -Encoding UTF8 -NoNewline -Value $commandCorpusText
$commandCorpusSha256 = Get-FileSha256OrEmpty -Path $commandCorpusPath

$plan = [ordered]@{
    HangProbeSchemaVersion = $hangProbeSchemaVersion
    CreatedAt = (Get-Date).ToString('o')
    RepoRoot = $repoRoot
    EvidenceRoot = $evidenceFull
    RunnerScriptPath = $runnerScriptPath
    RunnerScriptSha256 = $runnerScriptSha256
    Iterations = $Iterations
    Launch = [bool]$Launch
    GameRoot = $GameRoot
    SteamExe = $SteamExe
    SteamUserId = $SteamUserId
    Language = $Language
    Sts1EventMode = $Sts1EventMode
    ReleaseEvidenceLogEnabled = $true
    Scenario = $Scenario
    CommandSelectionMode = $CommandSelectionMode
    CommandCorpusSource = $commandCorpusSource
    CommandCorpusPath = $commandCorpusPath
    CommandCorpusSha256 = $commandCorpusSha256
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
    SourceWorkspaceCheckPath = $sourceWorkspaceCheckPath
    SourceWorkspaceCheckOutputPath = $sourceWorkspaceCheckOutputPath
    SourceWorkspaceCheckSha256 = $sourceWorkspaceCheckHash
    SourceWorkspace = [pscustomobject]$sourceWorkspaceSummary
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
        RequiresMainWindowAfterMainMenu = $true
    }
    LogGrowthProbe = [ordered]@{
        StartupFailsOnNoGrowth = $true
        StartupNoGrowthTimeoutSeconds = $NoLogGrowthTimeoutSeconds
        RuntimeLogGrowthIsTelemetryOnly = $false
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
        'pre-existing SlayTheSpire2 process is observed before launch or during observation',
        'live-session selected game process PID/start/path attribution is missing or does not match runtime probes',
        'SlayTheSpire2 main window is not observed after main menu',
        'SlayTheSpire2 window reports not responding or hung for the configured consecutive-sample threshold',
        'godot.log stops growing before main menu for the configured no-growth timeout',
        'required DevConsole command acknowledgement line is absent from godot.log',
        'godot.log missing or empty',
        'audit-godot-log reports release-blocking signature hits',
        'expected package/game/RitsuLib/patch-count markers are absent from godot.log',
        'StS1 mode verifier reports that actual godot.log mode/package/game/Ritsu shape does not match this run',
        'live-session restore fails',
        'live-session session/restore transaction files are missing, stale, or hash-unbound',
        'live-session restore item counts do not match the moved item lists from session-state.json',
        'live-session restore leaves SlayTheSpire2 or Godot processes running',
        'live-session restore does not restore settings hashes from retained backups',
        'DevConsole command send fails when enabled'
    )
}

Save-Json -InputObject $plan -Path (Join-Path $evidenceFull 'monkey-plan.json')

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
$previousReleaseEvidenceLog = [string]$env:SPIREPLUS_RELEASE_EVIDENCE_LOG
try {
    if ($Sts1EventMode -eq 'Off') {
        Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
    } else {
        $env:SPIREPLUS_STS1_EVENT_MODE = $Sts1EventMode
    }
    $env:SPIREPLUS_RELEASE_EVIDENCE_LOG = '1'

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
            LiveSessionPrepareOutputPath = Join-Path $iterationDir 'prepare-output.json'
            LiveSessionPrepareOutputSha256 = ''
            LiveSessionSessionStatePath = Join-Path $iterationDir 'session-state.json'
            LiveSessionSessionStateSha256 = ''
            LiveSessionRestoreStatePath = Join-Path $iterationDir 'restore-state.json'
            LiveSessionRestoreStateSha256 = ''
            LiveSessionEvidenceDir = ''
            LiveSessionLauncherKind = ''
            LiveSessionSteamAppId = ''
            LiveSessionLaunchFilePath = ''
            LiveSessionLaunchArgumentList = @()
            LiveSessionLaunchedProcessId = 0
            LiveSessionLaunchedAt = $null
            LiveSessionLaunchReturnedAt = $null
            LiveSessionPidAttributionSchemaVersion = 0
            LiveSessionPidAttributionPassed = $false
            LiveSessionPidAttributionMethod = ''
            LiveSessionPidProbeStartedAtUtc = $null
            LiveSessionPidProbeFinishedAtUtc = $null
            LiveSessionPreLaunchSlayProcessCount = 0
            LiveSessionPreLaunchSlayProcessIds = @()
            LiveSessionSelectedGameProcessId = 0
            LiveSessionSelectedGameProcessStartTimeUtc = $null
            LiveSessionSelectedGameProcessPath = ''
            LiveSessionSelectedGameProcessParentProcessId = 0
            LiveSessionAttributionFailureReason = ''
            LiveSessionRestoreSchemaVersion = 0
            LiveSessionStoppedSelectedGameProcess = $false
            LiveSessionMovedModCount = -1
            LiveSessionMovedCurrentRunCount = -1
            LiveSessionRestoredModCount = -1
            LiveSessionRestoredCurrentRunCount = -1
            LiveSessionRestoreItemCountsMatch = $false
            LiveSessionPreservedNewCurrentRunCount = 0
            LiveSessionPreservedNewCurrentRunsManifestPath = ''
            LiveSessionPreservedNewCurrentRunsManifestSha256 = ''
            LiveSessionPreservedNewCurrentRunsManifestBound = $true
            LiveSessionPostRestoreSlayProcessCount = -1
            LiveSessionPostRestoreSlayProcessIds = @()
            LiveSessionPostRestoreGodotProcessCount = -1
            LiveSessionPostRestoreGodotProcessIds = @()
            LiveSessionSettingsHashAfterRestore = ''
            LiveSessionSettingsBackupHashAfterRestore = ''
            LiveSessionSettingsBackupExistedBefore = $false
            LiveSessionSettingsBackupExistsAfterRestoreRecorded = $false
            LiveSessionSettingsBackupExistsAfterRestore = $false
            LiveSessionSettingsRestoredFromBackup = $false
            LiveSessionSettingsBackupRestoredFromBackup = $false
            GameProcessStartTimeAfterLiveSessionLaunch = $false
            GameProcessIdMatchesLiveSession = $false
            GameProcessStartTimeMatchesLiveSession = $false
            GameProcessPathMatchesLiveSession = $false
            RuntimeProbeSamplesPath = Join-Path $iterationDir 'runtime-probe-samples.json'
            RuntimeProbeSamplesSha256 = ''
            GodotLogBeforePath = Join-Path $iterationDir 'godot.log.before'
            GodotLogAfterLaunchPath = Join-Path $iterationDir 'godot.log.after-launch'
            GodotLogCurrentIterationPath = Join-Path $iterationDir 'godot.log.current-iteration'
            CurrentIterationLogPath = Join-Path $iterationDir 'godot.log.current-iteration'
            GameProcessId = 0
            GameProcessStartTimeUtc = $null
            GameProcessPath = ''
            MainWindowObserved = $false
            MainMenuDetectedAt = $null
            MainMenuElapsedSeconds = 0
            GodotLogBeforeCopied = $false
            GodotLogBeforeLengthBytes = 0L
            GodotLogBeforeSha256 = ''
            GodotLogAfterLaunchLengthBytes = 0L
            GodotLogAfterLaunchSha256 = ''
            GodotLogCurrentIterationLengthBytes = 0L
            GodotLogCurrentIterationSha256 = ''
            PreLaunchLogLengthBytes = 0L
            PreLaunchLogLastWriteTimeUtc = $null
            MinimumProcessStartTimeUtc = $null
            LogInitialLengthBytes = 0L
            LogScanOffsetBytes = 0L
            LogFinalLengthBytes = 0L
            LastLogGrowthAt = $null
            MaxSecondsWithoutLogGrowth = 0
            MaxConsecutiveUnresponsiveSamples = 0
            StaleProcessObserved = $false
            StaleProcessCount = 0
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
            CurrentIterationLogCopied = $false
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
        $prepareStarted = $false

        try {
            $minimumProcessStartTimeUtc = (Get-Date).ToUniversalTime()
            $preExistingProcesses = @(Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue)
            if ($preExistingProcesses.Count -gt 0) {
                $result.StaleProcessObserved = $true
                $result.StaleProcessCount = $preExistingProcesses.Count
                $result.Error = "Observed $($preExistingProcesses.Count) pre-existing SlayTheSpire2 process(es) before launch; shared godot.log cannot be trusted for this iteration."
            } else {
                $result.GodotLogBeforeCopied = Copy-BaselineGodotLog -Destination ([string]$result.GodotLogBeforePath)
                $preLaunchLog = Get-LogSnapshot -Path ([string]$result.GodotLogBeforePath)
                $result.PreLaunchLogLengthBytes = [long]$preLaunchLog.Length
                $result.PreLaunchLogLastWriteTimeUtc = $preLaunchLog.LastWriteTimeUtc
                $result.GodotLogBeforeLengthBytes = [long]$preLaunchLog.Length
                $result.GodotLogBeforeSha256 = Get-FileSha256OrEmpty -Path ([string]$result.GodotLogBeforePath)
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

                $prepareStarted = $true
                $prepareOutputPath = [string]$result.LiveSessionPrepareOutputPath
                & $liveSessionScript @prepareArgs | Out-File -LiteralPath $prepareOutputPath -Encoding UTF8
                $result.LiveSessionPrepareOutputSha256 = Get-FileSha256OrEmpty -Path $prepareOutputPath
                $result.LiveSessionSessionStateSha256 = Get-FileSha256OrEmpty -Path ([string]$result.LiveSessionSessionStatePath)
                $prepareOutput = Read-JsonOrNull -Path $prepareOutputPath
                if ($prepareOutput) {
                    $result.LiveSessionEvidenceDir = [string]$prepareOutput.EvidenceDir
                    $result.LiveSessionLauncherKind = [string]$prepareOutput.LaunchKind
                    $result.LiveSessionSteamAppId = [string]$prepareOutput.SteamAppId
                    $result.LiveSessionLaunchFilePath = [string]$prepareOutput.LaunchFilePath
                    $result.LiveSessionLaunchArgumentList = @($prepareOutput.LaunchArgumentList)
                    if ($prepareOutput.PSObject.Properties.Name -contains 'LaunchedProcessId' -and $null -ne $prepareOutput.LaunchedProcessId) {
                        $result.LiveSessionLaunchedProcessId = [int]$prepareOutput.LaunchedProcessId
                    }

                    if ($prepareOutput.PSObject.Properties.Name -contains 'LaunchedAt') {
                        $result.LiveSessionLaunchedAt = $prepareOutput.LaunchedAt
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'LaunchReturnedAt') {
                        $result.LiveSessionLaunchReturnedAt = $prepareOutput.LaunchReturnedAt
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PidAttributionSchemaVersion' -and $null -ne $prepareOutput.PidAttributionSchemaVersion) {
                        $result.LiveSessionPidAttributionSchemaVersion = [int]$prepareOutput.PidAttributionSchemaVersion
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PidAttributionPassed') {
                        $result.LiveSessionPidAttributionPassed = [bool]$prepareOutput.PidAttributionPassed
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PidAttributionMethod') {
                        $result.LiveSessionPidAttributionMethod = [string]$prepareOutput.PidAttributionMethod
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PidProbeStartedAtUtc') {
                        $result.LiveSessionPidProbeStartedAtUtc = $prepareOutput.PidProbeStartedAtUtc
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PidProbeFinishedAtUtc') {
                        $result.LiveSessionPidProbeFinishedAtUtc = $prepareOutput.PidProbeFinishedAtUtc
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PreLaunchSlayProcessCount' -and $null -ne $prepareOutput.PreLaunchSlayProcessCount) {
                        $result.LiveSessionPreLaunchSlayProcessCount = [int]$prepareOutput.PreLaunchSlayProcessCount
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'PreLaunchSlayProcessIds') {
                        $result.LiveSessionPreLaunchSlayProcessIds = @($prepareOutput.PreLaunchSlayProcessIds)
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'SelectedGameProcessId' -and $null -ne $prepareOutput.SelectedGameProcessId) {
                        $result.LiveSessionSelectedGameProcessId = [int]$prepareOutput.SelectedGameProcessId
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'SelectedGameProcessStartTimeUtc') {
                        $result.LiveSessionSelectedGameProcessStartTimeUtc = $prepareOutput.SelectedGameProcessStartTimeUtc
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'SelectedGameProcessPath') {
                        $result.LiveSessionSelectedGameProcessPath = [string]$prepareOutput.SelectedGameProcessPath
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'SelectedGameProcessParentProcessId' -and $null -ne $prepareOutput.SelectedGameProcessParentProcessId) {
                        $result.LiveSessionSelectedGameProcessParentProcessId = [int]$prepareOutput.SelectedGameProcessParentProcessId
                    }
                    if ($prepareOutput.PSObject.Properties.Name -contains 'AttributionFailureReason') {
                        $result.LiveSessionAttributionFailureReason = [string]$prepareOutput.AttributionFailureReason
                    }
                }
                $mainMenuObservation = Wait-ForMainMenuLog `
                    -Path $godotLogPath `
                    -TimeoutSeconds $MainMenuTimeoutSeconds `
                    -NoGrowthTimeoutSeconds $NoLogGrowthTimeoutSeconds `
                    -IntervalSeconds $ObservationIntervalSeconds `
                    -UnresponsiveSampleThreshold $UnresponsiveSampleThreshold `
                    -BaselineLogLengthBytes ([long]$result.PreLaunchLogLengthBytes) `
                    -MinimumProcessStartTimeUtc $minimumProcessStartTimeUtc `
                    -ProbeSamples $probeSamples `
                    -ExpectedProcessId ([int]$result.LiveSessionSelectedGameProcessId) `
                    -ExpectedProcessStartTimeUtc ([string]$result.LiveSessionSelectedGameProcessStartTimeUtc) `
                    -ExpectedProcessPath ([string]$result.LiveSessionSelectedGameProcessPath)
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
                $result.StaleProcessObserved = [bool]$mainMenuObservation.StaleProcessObserved
                $result.StaleProcessCount = [Math]::Max([int]$result.StaleProcessCount, [int]$mainMenuObservation.MaxStaleProcessCount)
                if ($mainMenuObservation.LastProcess -and $mainMenuObservation.LastProcess.Observed) {
                    $result.GameProcessId = [int]$mainMenuObservation.LastProcess.Id
                    $result.GameProcessStartTimeUtc = $mainMenuObservation.LastProcess.StartTimeUtc
                    $result.GameProcessPath = [string]$mainMenuObservation.LastProcess.ProcessPath
                    $result.GameProcessIdMatchesLiveSession = [bool]$mainMenuObservation.LastProcess.ProcessIdMatchesExpected
                    $result.GameProcessStartTimeMatchesLiveSession = [bool]$mainMenuObservation.LastProcess.ProcessStartTimeMatchesExpected
                    $result.GameProcessPathMatchesLiveSession = [bool]$mainMenuObservation.LastProcess.ProcessPathMatchesExpected
                    $result.MainWindowObserved = [int64]$mainMenuObservation.LastProcess.MainWindowHandle -ne 0
                    if ($result.LiveSessionLaunchedAt -and $result.GameProcessStartTimeUtc) {
                        try {
                            $gameProcessStart = [datetime]::Parse([string]$result.GameProcessStartTimeUtc).ToUniversalTime()
                            $liveSessionLaunch = [datetime]::Parse([string]$result.LiveSessionLaunchedAt).ToUniversalTime()
                            $result.GameProcessStartTimeAfterLiveSessionLaunch = $gameProcessStart -ge $liveSessionLaunch
                        } catch {
                            $result.GameProcessStartTimeAfterLiveSessionLaunch = $false
                        }
                    }
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
                        -ProbeSamples $probeSamples `
                        -RequireLogGrowth $true `
                        -ExpectedProcessId ([int]$result.LiveSessionSelectedGameProcessId) `
                        -ExpectedProcessStartTimeUtc ([string]$result.LiveSessionSelectedGameProcessStartTimeUtc) `
                        -ExpectedProcessPath ([string]$result.LiveSessionSelectedGameProcessPath)
                    $result.RuntimeObservation = $runtimeObservation
                    $result.RuntimeObservationPassed = [bool]$runtimeObservation.Passed
                    $result.PostCommandLogProbePassed = [bool]$runtimeObservation.LogObserved -and (-not [bool]$runtimeObservation.RuntimeLogGrowthRequired -or ([bool]$runtimeObservation.LogGrew -and -not [bool]$runtimeObservation.NoLogGrowthTimeoutExceeded))
                    $result.MaxSecondsWithoutLogGrowth = [Math]::Max([int]$result.MaxSecondsWithoutLogGrowth, [int]$runtimeObservation.MaxNoLogGrowthSeconds)
                    $result.MaxConsecutiveUnresponsiveSamples = [Math]::Max([int]$result.MaxConsecutiveUnresponsiveSamples, [int]$runtimeObservation.MaxConsecutiveUnresponsiveSamples)
                    $result.StaleProcessObserved = [bool]$result.StaleProcessObserved -or [bool]$runtimeObservation.StaleProcessObserved
                    $result.StaleProcessCount = [Math]::Max([int]$result.StaleProcessCount, [int]$runtimeObservation.MaxStaleProcessCount)
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
                        -ProbeSamples $probeSamples `
                        -RequireLogGrowth $false `
                        -ExpectedProcessId ([int]$result.LiveSessionSelectedGameProcessId) `
                        -ExpectedProcessStartTimeUtc ([string]$result.LiveSessionSelectedGameProcessStartTimeUtc) `
                        -ExpectedProcessPath ([string]$result.LiveSessionSelectedGameProcessPath)
                    $result.RuntimeObservation = $runtimeObservation
                    $result.RuntimeObservationPassed = [bool]$runtimeObservation.Passed
                    $result.PostCommandLogProbePassed = [bool]$runtimeObservation.LogObserved -and (-not [bool]$runtimeObservation.RuntimeLogGrowthRequired -or ([bool]$runtimeObservation.LogGrew -and -not [bool]$runtimeObservation.NoLogGrowthTimeoutExceeded))
                    $result.MaxSecondsWithoutLogGrowth = [Math]::Max([int]$result.MaxSecondsWithoutLogGrowth, [int]$runtimeObservation.MaxNoLogGrowthSeconds)
                    $result.MaxConsecutiveUnresponsiveSamples = [Math]::Max([int]$result.MaxConsecutiveUnresponsiveSamples, [int]$runtimeObservation.MaxConsecutiveUnresponsiveSamples)
                    $result.StaleProcessObserved = [bool]$result.StaleProcessObserved -or [bool]$runtimeObservation.StaleProcessObserved
                    $result.StaleProcessCount = [Math]::Max([int]$result.StaleProcessCount, [int]$runtimeObservation.MaxStaleProcessCount)
                    if ($runtimeObservation.LastLogGrowthAt) {
                        $result.LastLogGrowthAt = $runtimeObservation.LastLogGrowthAt
                    }
                } else {
                    $result.RuntimeObservationPassed = $true
                    $result.PostCommandLogProbePassed = $true
                }

                $result.ResponsivenessProbePassed = $result.MainMenuObservationPassed -and $result.RuntimeObservationPassed

                $launchLog = [string]$result.GodotLogAfterLaunchPath
                $result.LogCopied = Copy-CurrentGodotLog -Destination $launchLog
                if ($result.LogCopied) {
                    $copiedLog = Get-LogSnapshot -Path $launchLog
                    $result.LogFinalLengthBytes = [long]$copiedLog.Length
                    $result.GodotLogAfterLaunchLengthBytes = [long]$copiedLog.Length
                    $result.GodotLogAfterLaunchSha256 = Get-FileSha256OrEmpty -Path $launchLog
                    $currentIterationLog = [string]$result.GodotLogCurrentIterationPath
                    $result.CurrentIterationLogPath = $currentIterationLog
                    $result.LogScanOffsetBytes = [long]$result.GodotLogBeforeLengthBytes
                    $result.CurrentIterationLogCopied = Write-CurrentIterationLogSlice `
                        -Source $launchLog `
                        -Destination $currentIterationLog `
                        -Offset ([long]$result.LogScanOffsetBytes)
                    if ($result.CurrentIterationLogCopied) {
                        $currentLogSnapshot = Get-LogSnapshot -Path $currentIterationLog
                        $result.GodotLogCurrentIterationLengthBytes = [long]$currentLogSnapshot.Length
                        $result.GodotLogCurrentIterationSha256 = Get-FileSha256OrEmpty -Path $currentIterationLog
                    }

                    $logForChecks = if ($result.CurrentIterationLogCopied) { $currentIterationLog } else { $launchLog }
                    $commandAck = Test-CommandAck -LogPath $logForChecks -Command ([string]$planned.Command)
                    $result.CommandAckRequired = [bool]$commandAck.Required
                    $result.CommandAckObserved = [bool]$commandAck.Observed
                    $result.CommandAckPattern = [string]$commandAck.Pattern

                    $auditPath = Join-Path $iterationDir 'godot-log-audit.json'
                    if ($result.CurrentIterationLogCopied) {
                        $result.AuditClean = Invoke-LogAudit -LogPath $currentIterationLog -OutFile $auditPath
                    } else {
                        $result.AuditClean = $false
                        Invoke-LogAudit -LogPath $launchLog -OutFile (Join-Path $iterationDir 'godot-log-after-launch-audit.json') | Out-Null
                    }
                    $expectations = Test-LogExpectations -LogPath $logForChecks
                    $result.ExpectationPassed = [bool]$expectations.Passed
                    $result.ExpectationChecks = @($expectations.Checks)
                    if ($result.CurrentIterationLogCopied) {
                        $modeCheck = Invoke-Sts1ModeVerifier `
                            -LogPath $currentIterationLog `
                            -AuditPath $auditPath `
                            -OutFile (Join-Path $iterationDir 'sts1-mode-log-check.json') `
                            -TextOutFile (Join-Path $iterationDir 'sts1-mode-log-check.txt')
                        $result.Sts1ModeVerifierPassed = [bool]$modeCheck.Passed
                        $result.Sts1ModeVerifierChecks = @($modeCheck.Checks)
                        $result.Sts1ModeVerifierMismatches = @($modeCheck.Mismatches)
                    }
                }
            }
        } catch {
            $result.Error = $_.Exception.Message
        } finally {
            if ($prepareStarted) {
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
                Update-LiveSessionRestoreStateFields -Result $result
            } else {
                if ([bool]$result.StaleProcessObserved -and [int]$result.StaleProcessCount -gt 0) {
                    $result.RestoreSucceeded = $true
                } else {
                    $result.RestoreSucceeded = $false
                }
            }
        }

        $result.FinishedAt = (Get-Date).ToString('o')
        $failureCodes = Get-FailureReasonCodes -Result ([pscustomobject]$result)
        $hangSignals = Get-HangSignals -FailureReasonCodes $failureCodes
        $result.FailureReasonCodes = @($failureCodes)
        $result.HangSignals = @($hangSignals)

        Save-Json -InputObject @($probeSamples) -Path $result.RuntimeProbeSamplesPath
        $result.RuntimeProbeSamplesSha256 = Get-FileSha256OrEmpty -Path ([string]$result.RuntimeProbeSamplesPath)

        $result.Passed = $result.MainMenuReached -and $result.MainMenuObservationPassed -and $result.RuntimeObservationPassed -and $result.MainWindowObserved -and -not [bool]$result.StaleProcessObserved -and [int]$result.StaleProcessCount -eq 0 -and $result.CommandAckObserved -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionPrepareOutputSha256) -and [int]$result.LiveSessionLaunchedProcessId -gt 0 -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionLaunchedAt) -and [bool]$result.LiveSessionPidAttributionPassed -and [int]$result.LiveSessionSelectedGameProcessId -gt 0 -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionSelectedGameProcessStartTimeUtc) -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionSelectedGameProcessPath) -and [bool]$result.GameProcessStartTimeAfterLiveSessionLaunch -and [bool]$result.GameProcessIdMatchesLiveSession -and [bool]$result.GameProcessStartTimeMatchesLiveSession -and [bool]$result.GameProcessPathMatchesLiveSession -and -not [string]::IsNullOrWhiteSpace([string]$result.GameProcessPath) -and $result.GodotLogBeforeCopied -and $result.LogCopied -and $result.CurrentIterationLogCopied -and $result.AuditClean -and $result.ExpectationPassed -and $result.Sts1ModeVerifierPassed -and $result.RestoreSucceeded -and -not [string]::IsNullOrWhiteSpace([string]$result.RuntimeProbeSamplesSha256) -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionSessionStateSha256) -and -not [string]::IsNullOrWhiteSpace([string]$result.LiveSessionRestoreStateSha256) -and [int]$result.LiveSessionRestoreSchemaVersion -gt 0 -and [bool]$result.LiveSessionStoppedSelectedGameProcess -and [bool]$result.LiveSessionRestoreItemCountsMatch -and ([int]$result.LiveSessionPreservedNewCurrentRunCount -eq 0 -or [bool]$result.LiveSessionPreservedNewCurrentRunsManifestBound) -and [int]$result.LiveSessionPostRestoreSlayProcessCount -eq 0 -and [int]$result.LiveSessionPostRestoreGodotProcessCount -eq 0 -and [bool]$result.LiveSessionSettingsRestoredFromBackup -and [bool]$result.LiveSessionSettingsBackupRestoredFromBackup -and
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
        MainWindowMissingCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'main_window_missing' }).Count
        LiveSessionBindingMissingCount = @($results | Where-Object {
                $codes = @($_.FailureReasonCodes)
                $codes -contains 'live_session_prepare_output_missing' -or
                $codes -contains 'live_session_launch_metadata_missing' -or
                $codes -contains 'live_session_pid_attribution_missing' -or
                $codes -contains 'live_session_pid_attribution_failed' -or
                $codes -contains 'game_process_start_time_unbound' -or
                $codes -contains 'game_process_path_missing' -or
                $codes -contains 'game_process_id_mismatch' -or
                $codes -contains 'game_process_start_time_mismatch' -or
                $codes -contains 'game_process_path_mismatch' -or
                $codes -contains 'live_session_session_state_missing' -or
                $codes -contains 'live_session_restore_state_missing'
            }).Count
        LiveSessionRestoreItemCountMismatchCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'restore_item_count_mismatch' }).Count
        LiveSessionPreservedCurrentRunManifestMissingCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'preserved_current_runs_manifest_missing' }).Count
        LiveSessionRestoreLeakCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'post_restore_process_leak' }).Count
        LiveSessionRestoreHashMismatchCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'restore_settings_hash_mismatch' }).Count
        LiveSessionSelectedProcessNotStoppedCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'selected_game_process_not_stopped' }).Count
        GodotLogBeforeMissingCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'godot_log_before_missing' }).Count
        CurrentIterationLogMissingCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'current_iteration_log_missing' }).Count
        UnresponsiveIterationCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'process_unresponsive' }).Count
        StaleProcessObservedCount = @($results | Where-Object { @($_.FailureReasonCodes) -contains 'stale_process_observed' }).Count
        LogStallIterationCount = @($results | Where-Object {
                $codes = @($_.FailureReasonCodes)
                $codes -contains 'startup_log_stalled' -or $codes -contains 'runtime_log_stalled'
            }).Count
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
    if ([string]::IsNullOrEmpty($previousReleaseEvidenceLog)) {
        Remove-Item Env:\SPIREPLUS_RELEASE_EVIDENCE_LOG -ErrorAction SilentlyContinue
    } else {
        $env:SPIREPLUS_RELEASE_EVIDENCE_LOG = $previousReleaseEvidenceLog
    }
}
