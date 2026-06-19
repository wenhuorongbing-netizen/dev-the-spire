param(
    [string]$EvidenceDir,

    [int]$Iteration = 0,

    [string]$IterationDir,

    [string]$LogPath,

    [string]$AuditPath,

    [string]$OutFile,

    [switch]$FailOnBlockingFinding
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$sts1EnabledModeLogVerifierScript = Join-Path $PSScriptRoot 'check-sts1-enabled-mode-runtime-log.ps1'

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Test-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return $null -ne $Object -and $Object.PSObject.Properties.Name -contains $Name
}

function Get-JsonValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $DefaultValue = $null
    )

    if (Test-JsonProperty -Object $Object -Name $Name) {
        return $Object.$Name
    }

    return $DefaultValue
}

function ConvertTo-IntOrDefault {
    param(
        [AllowNull()]$Value,
        [int]$DefaultValue = 0
    )

    if ($null -eq $Value) {
        return $DefaultValue
    }

    try {
        return [int]$Value
    } catch {
        return $DefaultValue
    }
}

function ConvertTo-LongOrDefault {
    param(
        [AllowNull()]$Value,
        [long]$DefaultValue = 0
    )

    if ($null -eq $Value) {
        return $DefaultValue
    }

    try {
        return [long]$Value
    } catch {
        return $DefaultValue
    }
}

function ConvertTo-BoolOrDefault {
    param(
        [AllowNull()]$Value,
        [bool]$DefaultValue = $false
    )

    if ($null -eq $Value) {
        return $DefaultValue
    }

    if ($Value -is [bool]) {
        return [bool]$Value
    }

    return $DefaultValue
}

function Get-JsonIntValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [int]$DefaultValue = 0
    )

    return ConvertTo-IntOrDefault -Value (Get-JsonValue -Object $Object -Name $Name -DefaultValue $DefaultValue) -DefaultValue $DefaultValue
}

function Get-JsonLongValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [long]$DefaultValue = 0
    )

    return ConvertTo-LongOrDefault -Value (Get-JsonValue -Object $Object -Name $Name -DefaultValue $DefaultValue) -DefaultValue $DefaultValue
}

function Get-JsonBoolValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [bool]$DefaultValue = $false
    )

    return ConvertTo-BoolOrDefault -Value (Get-JsonValue -Object $Object -Name $Name -DefaultValue $DefaultValue) -DefaultValue $DefaultValue
}

function Test-JsonBoolProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return (Test-JsonProperty -Object $Object -Name $Name) -and
        $null -ne $Object.$Name -and
        $Object.$Name -is [bool]
}

function Test-JsonArrayProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return (Test-JsonProperty -Object $Object -Name $Name) -and
        $null -ne $Object.$Name -and
        $Object.$Name -is [System.Array]
}

function Get-JsonArrayValues {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $items = [System.Collections.Generic.List[object]]::new()
    if (-not (Test-JsonProperty -Object $Object -Name $Name)) {
        return
    }

    $value = $Object.$Name
    if ($null -eq $value) {
        return
    }

    if ($value -is [System.Array]) {
        foreach ($item in $value) {
            $items.Add($item) | Out-Null
        }
    } else {
        $items.Add($value) | Out-Null
    }

    foreach ($item in $items) {
        Write-Output -NoEnumerate $item
    }
}

function ConvertTo-StringArray {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return @()
    }

    return @($Value | ForEach-Object { [string]$_ })
}

function Get-NormalizedAncientIdTokens {
    param([AllowNull()]$Value)

    return @(ConvertTo-StringArray -Value $Value |
        ForEach-Object { $_ -split ',' } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { $_.Trim().ToUpperInvariant() })
}

function Test-StringArrayEquals {
    param(
        [Alias('Left')][AllowNull()]$Actual,
        [Alias('Right')][AllowNull()]$Expected
    )

    $actualArray = @(ConvertTo-StringArray -Value $Actual)
    $expectedArray = @(ConvertTo-StringArray -Value $Expected)
    if ($actualArray.Count -ne $expectedArray.Count) {
        return $false
    }

    for ($index = 0; $index -lt $actualArray.Count; $index++) {
        if (-not [string]::Equals($actualArray[$index], $expectedArray[$index], [System.StringComparison]::Ordinal)) {
            return $false
        }
    }

    return $true
}

function Get-FailureCodeGroupCount {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string[]]$FailureCodes
    )

    $count = 0
    foreach ($item in @($Items)) {
        $itemCodes = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $item -Name 'FailureReasonCodes' -DefaultValue @()))
        foreach ($failureCode in $FailureCodes) {
            if ($itemCodes -contains $failureCode) {
                $count++
                break
            }
        }
    }

    return $count
}

function Get-FailureReasonCounts {
    param([AllowNull()]$Items)

    $counts = [ordered]@{}
    foreach ($item in @($Items)) {
        $itemCodes = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $item -Name 'FailureReasonCodes' -DefaultValue @()))
        foreach ($code in $itemCodes) {
            if ([string]::IsNullOrWhiteSpace($code)) {
                continue
            }

            if (-not $counts.Contains($code)) {
                $counts[$code] = 0
            }

            $counts[$code]++
        }
    }

    return $counts
}

function Test-CountMapMatches {
    param(
        [AllowNull()]$ActualCountMap,
        [Parameter(Mandatory = $true)]$ExpectedCounts
    )

    if ($null -eq $ActualCountMap) {
        return $false
    }

    $actualProperties = @($ActualCountMap.PSObject.Properties)
    $expectedKeys = @($ExpectedCounts.Keys)
    if ($actualProperties.Count -ne $expectedKeys.Count) {
        return $false
    }

    foreach ($key in $expectedKeys) {
        $actualProperty = $ActualCountMap.PSObject.Properties[$key]
        if ($null -eq $actualProperty -or
            (ConvertTo-IntOrDefault -Value $actualProperty.Value -DefaultValue -1) -ne
            (ConvertTo-IntOrDefault -Value $ExpectedCounts[$key] -DefaultValue -2)) {
            return $false
        }
    }

    return $true
}

function Get-RuntimeMonkeySummaryMismatchDetails {
    param([AllowNull()]$Summary)

    $details = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Summary -or -not (Test-JsonProperty -Object $Summary -Name 'Results')) {
        return @()
    }

    $summaryResults = @($Summary.Results)
    $failedSummaryResults = @($summaryResults | Where-Object { -not (Get-JsonBoolValue -Object $_ -Name 'Passed' -DefaultValue $false) })
    $summaryRequestedIterations = Get-JsonIntValue -Object $Summary -Name 'RequestedIterations' -DefaultValue 0
    $summaryCompletedIterations = Get-JsonIntValue -Object $Summary -Name 'CompletedIterations' -DefaultValue 0
    $expectedSummaryPassed = $summaryResults.Count -gt 0 -and $failedSummaryResults.Count -eq 0 -and $summaryCompletedIterations -eq $summaryRequestedIterations
    $summaryPassed = (Get-JsonBoolValue -Object $Summary -Name 'Passed' -DefaultValue $false)
    if ($summaryPassed -ne $expectedSummaryPassed) {
        $details.Add("Passed expected=$expectedSummaryPassed actual=$summaryPassed") | Out-Null
    }

    $summaryFailedIterations = Get-JsonIntValue -Object $Summary -Name 'FailedIterations' -DefaultValue -1
    if ($summaryFailedIterations -ne $failedSummaryResults.Count) {
        $details.Add("FailedIterations expected=$($failedSummaryResults.Count) actual=$summaryFailedIterations") | Out-Null
    }

    $summaryFailedIterationIds = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $Summary -Name 'FailedIterationIds' -DefaultValue @()))
    $expectedFailedIterationIds = @($failedSummaryResults | ForEach-Object { [string](Get-JsonIntValue -Object $_ -Name 'Iteration' -DefaultValue 0) })
    if (-not (Test-StringArrayEquals -Actual $summaryFailedIterationIds -Expected $expectedFailedIterationIds)) {
        $details.Add("FailedIterationIds expected=$($expectedFailedIterationIds -join ',') actual=$($summaryFailedIterationIds -join ',')") | Out-Null
    }

    $failureReasonCounts = Get-JsonValue -Object $Summary -Name 'FailureReasonCounts' -DefaultValue $null
    if (-not (Test-CountMapMatches -ActualCountMap $failureReasonCounts -ExpectedCounts (Get-FailureReasonCounts -Items $summaryResults))) {
        $details.Add('FailureReasonCounts mismatch') | Out-Null
    }

    $summaryCounterChecks = @(
        [pscustomobject]@{ Name = 'ProcessExitCount'; Codes = @('game_process_exited') },
        [pscustomobject]@{ Name = 'MainWindowMissingCount'; Codes = @('main_window_missing') },
        [pscustomobject]@{ Name = 'LiveSessionBindingMissingCount'; Codes = @(
                'live_session_prepare_output_missing',
                'live_session_launch_metadata_missing',
                'live_session_pid_attribution_missing',
                'live_session_pid_attribution_failed',
                'game_process_start_time_unbound',
                'game_process_path_missing',
                'game_process_id_mismatch',
                'game_process_start_time_mismatch',
                'game_process_path_mismatch',
                'live_session_session_state_missing',
                'live_session_restore_state_missing'
            ) },
        [pscustomobject]@{ Name = 'LiveSessionRestoreItemCountMismatchCount'; Codes = @('restore_item_count_mismatch') },
        [pscustomobject]@{ Name = 'LiveSessionPreservedCurrentRunManifestMissingCount'; Codes = @('preserved_current_runs_manifest_missing') },
        [pscustomobject]@{ Name = 'LiveSessionRestoreLeakCount'; Codes = @('post_restore_process_leak') },
        [pscustomobject]@{ Name = 'LiveSessionRestoreHashMismatchCount'; Codes = @('restore_settings_hash_mismatch') },
        [pscustomobject]@{ Name = 'LiveSessionSelectedProcessNotStoppedCount'; Codes = @('selected_game_process_not_stopped') },
        [pscustomobject]@{ Name = 'GodotLogBeforeMissingCount'; Codes = @('godot_log_before_missing') },
        [pscustomobject]@{ Name = 'CurrentIterationLogMissingCount'; Codes = @('current_iteration_log_missing') },
        [pscustomobject]@{ Name = 'UnresponsiveIterationCount'; Codes = @('process_unresponsive') },
        [pscustomobject]@{ Name = 'StaleProcessObservedCount'; Codes = @('stale_process_observed') },
        [pscustomobject]@{ Name = 'LogStallIterationCount'; Codes = @('startup_log_stalled', 'runtime_log_stalled') },
        [pscustomobject]@{ Name = 'CommandAckMissingCount'; Codes = @('command_ack_missing') }
    )

    foreach ($counterCheck in $summaryCounterChecks) {
        $actualCounter = Get-JsonIntValue -Object $Summary -Name ([string]$counterCheck.Name) -DefaultValue -1
        $expectedCounter = Get-FailureCodeGroupCount -Items $summaryResults -FailureCodes ([string[]]$counterCheck.Codes)
        if ($actualCounter -ne $expectedCounter) {
            $details.Add("$($counterCheck.Name) expected=$expectedCounter actual=$actualCounter") | Out-Null
        }
    }

    return @($details.ToArray())
}

function Get-RuntimeMonkeySummaryResultMismatchDetails {
    param(
        [AllowNull()]$Result,
        [AllowNull()]$SummaryResult
    )

    $details = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Result -or $null -eq $SummaryResult) {
        return @()
    }

    $stringFields = @(
        'Scenario',
        'CommandSelectionMode',
        'Command',
        'CommandFilePath',
        'CommandFileSha256',
        'RuntimeProbeSamplesPath',
        'RuntimeProbeSamplesSha256',
        'LiveSessionSessionStatePath',
        'LiveSessionSessionStateSha256',
        'LiveSessionRestoreStatePath',
        'LiveSessionRestoreStateSha256',
        'ScenarioTag',
        'OwnerArea',
        'CommandAckPattern'
    )

    foreach ($fieldName in $stringFields) {
        $resultValue = [string](Get-JsonValue -Object $Result -Name $fieldName -DefaultValue '')
        $summaryValue = [string](Get-JsonValue -Object $SummaryResult -Name $fieldName -DefaultValue '')
        if (-not [string]::Equals($resultValue, $summaryValue, [System.StringComparison]::Ordinal)) {
            $details.Add("$fieldName expected='$resultValue' actual='$summaryValue'") | Out-Null
        }
    }

    foreach ($fieldName in @('Passed', 'CommandAckRequired', 'CommandAckObserved')) {
        $resultValue = (Get-JsonBoolValue -Object $Result -Name $fieldName -DefaultValue $false)
        $summaryValue = (Get-JsonBoolValue -Object $SummaryResult -Name $fieldName -DefaultValue $false)
        if ($resultValue -ne $summaryValue) {
            $details.Add("$fieldName expected=$resultValue actual=$summaryValue") | Out-Null
        }
    }

    foreach ($fieldName in @('FailureReasonCodes', 'HangSignals')) {
        $resultValues = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $Result -Name $fieldName -DefaultValue @()))
        $summaryValues = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $SummaryResult -Name $fieldName -DefaultValue @()))
        if (-not (Test-StringArrayEquals -Actual $summaryValues -Expected $resultValues)) {
            $details.Add("$fieldName expected=$($resultValues -join ',') actual=$($summaryValues -join ',')") | Out-Null
        }
    }

    return @($details.ToArray())
}

function Get-RuntimeMonkeySummaryPlanMismatchDetails {
    param(
        [AllowNull()]$Summary,
        [AllowNull()]$Plan
    )

    $details = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Summary -or $null -eq $Plan -or -not (Test-JsonProperty -Object $Plan -Name 'PlannedCommands')) {
        return @()
    }

    $stringFields = @(
        'Scenario',
        'CommandSelectionMode',
        'Sts1EventMode',
        'ExpectedPackageVersion',
        'ExpectedGameVersion',
        'ExpectedRitsuLibVersion',
        'ExpectedRitsuCompatBranch'
    )

    foreach ($fieldName in $stringFields) {
        $planValue = [string](Get-JsonValue -Object $Plan -Name $fieldName -DefaultValue '')
        $summaryValue = [string](Get-JsonValue -Object $Summary -Name $fieldName -DefaultValue '')
        if (-not [string]::Equals($summaryValue, $planValue, [System.StringComparison]::Ordinal)) {
            $details.Add("$fieldName expected='$planValue' actual='$summaryValue'") | Out-Null
        }
    }

    $planPatchCount = Get-JsonIntValue -Object $Plan -Name 'ExpectedPatchCount' -DefaultValue 0
    $summaryPatchCount = Get-JsonIntValue -Object $Summary -Name 'ExpectedPatchCount' -DefaultValue 0
    if ($summaryPatchCount -ne $planPatchCount) {
        $details.Add("ExpectedPatchCount expected='$planPatchCount' actual='$summaryPatchCount'") | Out-Null
    }

    return @($details.ToArray())
}

function Get-AutoSlaySummaryPlanMismatchDetails {
    param(
        [AllowNull()]$Summary,
        [AllowNull()]$Plan
    )

    $details = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Summary -or $null -eq $Plan) {
        return @()
    }

    foreach ($fieldName in @('RunnerKind', 'Sts1EventMode', 'PackageVersion', 'GameVersion', 'RitsuLibVersion', 'RitsuCompatBranch')) {
        $planHasField = Test-JsonProperty -Object $Plan -Name $fieldName
        $summaryHasField = Test-JsonProperty -Object $Summary -Name $fieldName
        $planValue = [string](Get-JsonValue -Object $Plan -Name $fieldName -DefaultValue '')
        $summaryValue = [string](Get-JsonValue -Object $Summary -Name $fieldName -DefaultValue '')
        if (-not $planHasField -or -not $summaryHasField -or -not [string]::Equals($summaryValue, $planValue, [System.StringComparison]::Ordinal)) {
            $details.Add("$fieldName expected='$planValue' actual='$summaryValue'") | Out-Null
        }
    }

    $planHasPatchCount = Test-JsonProperty -Object $Plan -Name 'ExpectedPatchCount'
    $summaryHasPatchCount = Test-JsonProperty -Object $Summary -Name 'ExpectedPatchCount'
    $planPatchCount = Get-JsonIntValue -Object $Plan -Name 'ExpectedPatchCount' -DefaultValue 0
    $summaryPatchCount = Get-JsonIntValue -Object $Summary -Name 'ExpectedPatchCount' -DefaultValue 0
    if (-not $planHasPatchCount -or -not $summaryHasPatchCount -or $summaryPatchCount -ne $planPatchCount) {
        $details.Add("ExpectedPatchCount expected='$planPatchCount' actual='$summaryPatchCount'") | Out-Null
    }

    $planExpectedAncientIds = @(Get-NormalizedAncientIdTokens -Value (Get-JsonValue -Object $Plan -Name 'ExpectedAncientIds' -DefaultValue @()) | Sort-Object -Unique)
    $summaryExpectedAncientIds = @(Get-NormalizedAncientIdTokens -Value (Get-JsonValue -Object $Summary -Name 'ExpectedAncientIds' -DefaultValue @()) | Sort-Object -Unique)
    $missingExpectedAncientIds = @($planExpectedAncientIds | Where-Object { $summaryExpectedAncientIds -notcontains $_ })
    $unexpectedExpectedAncientIds = @($summaryExpectedAncientIds | Where-Object { $planExpectedAncientIds -notcontains $_ })
    if (-not (Test-JsonProperty -Object $Plan -Name 'ExpectedAncientIds') -or
        -not (Test-JsonProperty -Object $Summary -Name 'ExpectedAncientIds') -or
        $missingExpectedAncientIds.Count -gt 0 -or
        $unexpectedExpectedAncientIds.Count -gt 0) {
        $details.Add("ExpectedAncientIds missing='$($missingExpectedAncientIds -join ',')' unexpected='$($unexpectedExpectedAncientIds -join ',')'") | Out-Null
    }

    return @($details.ToArray())
}

function Get-RuntimeMonkeyPlanResultMismatchDetails {
    param(
        [AllowNull()]$Plan,
        [AllowNull()]$Result
    )

    $details = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Plan -or $null -eq $Result -or -not (Test-JsonProperty -Object $Plan -Name 'PlannedCommands')) {
        return @()
    }

    $iteration = Get-JsonIntValue -Object $Result -Name 'Iteration' -DefaultValue 0
    $planScenario = [string](Get-JsonValue -Object $Plan -Name 'Scenario' -DefaultValue '')
    $resultScenario = [string](Get-JsonValue -Object $Result -Name 'Scenario' -DefaultValue '')
    if (-not [string]::Equals($resultScenario, $planScenario, [System.StringComparison]::Ordinal)) {
        $details.Add("Scenario expected='$planScenario' actual='$resultScenario'") | Out-Null
    }

    $planCommandSelectionMode = [string](Get-JsonValue -Object $Plan -Name 'CommandSelectionMode' -DefaultValue '')
    $resultCommandSelectionMode = [string](Get-JsonValue -Object $Result -Name 'CommandSelectionMode' -DefaultValue '')
    if (-not [string]::Equals($resultCommandSelectionMode, $planCommandSelectionMode, [System.StringComparison]::Ordinal)) {
        $details.Add("CommandSelectionMode expected='$planCommandSelectionMode' actual='$resultCommandSelectionMode'") | Out-Null
    }

    $plannedCommands = @(Get-JsonArrayValues -Object $Plan -Name 'PlannedCommands')
    $plannedMatches = @($plannedCommands | Where-Object { (Get-JsonIntValue -Object $_ -Name 'Iteration' -DefaultValue 0) -eq $iteration })
    if ($iteration -le 0) {
        $details.Add("Iteration expected=positive actual=$iteration") | Out-Null
        return @($details.ToArray())
    }

    if ($plannedMatches.Count -eq 0) {
        $details.Add("PlannedCommands missing Iteration=$iteration") | Out-Null
        return @($details.ToArray())
    }

    if ($plannedMatches.Count -gt 1) {
        $details.Add("PlannedCommands duplicate Iteration=$iteration count=$($plannedMatches.Count)") | Out-Null
        return @($details.ToArray())
    }

    $plannedCommand = $plannedMatches[0]
    foreach ($fieldName in @('Command', 'CommandSelectionMode', 'ScenarioTag', 'OwnerArea', 'CommandAckPattern')) {
        $expected = [string](Get-JsonValue -Object $plannedCommand -Name $fieldName -DefaultValue '')
        $actual = [string](Get-JsonValue -Object $Result -Name $fieldName -DefaultValue '')
        if (-not [string]::Equals($actual, $expected, [System.StringComparison]::Ordinal)) {
            $details.Add("$fieldName expected='$expected' actual='$actual'") | Out-Null
        }
    }

    $expectedCommandIndex = Get-JsonIntValue -Object $plannedCommand -Name 'CommandIndex' -DefaultValue -1
    $actualCommandIndex = Get-JsonIntValue -Object $Result -Name 'CommandIndex' -DefaultValue -2
    if ($actualCommandIndex -ne $expectedCommandIndex) {
        $details.Add("CommandIndex expected=$expectedCommandIndex actual=$actualCommandIndex") | Out-Null
    }

    return @($details.ToArray())
}

function Read-JsonOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $null
    }

    try {
        return Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        return $null
    }
}

function Test-JsonFileParses {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $false
    }

    $json = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    if ([string]::IsNullOrWhiteSpace($json)) {
        return $false
    }

    try {
        [void]($json | ConvertFrom-Json)
        return $true
    } catch {
        return $false
    }
}

function Read-TextAfterByteOffset {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    $stream = [System.IO.File]::Open($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    try {
        [void]$stream.Seek($Offset, [System.IO.SeekOrigin]::Begin)
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

function Normalize-LogSliceForComparison {
    param([AllowNull()][string]$Text)

    if ($null -eq $Text) {
        return ''
    }

    $normalized = $Text
    if ($normalized.Length -gt 0 -and $normalized[0] -eq [char]0xFEFF) {
        $normalized = $normalized.Substring(1)
    }

    return $normalized -replace "[`r`n]+$", ''
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

function Get-FirstJsonString {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string[]]$Names
    )

    foreach ($name in $Names) {
        $value = [string](Get-JsonValue -Object $Object -Name $name -DefaultValue '')
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            return $value
        }
    }

    return ''
}

function Resolve-AnalysisPath {
    param(
        [Parameter(Mandatory = $true)][string]$BaseDir,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        if ([System.IO.Path]::IsPathRooted($Path)) {
            return [System.IO.Path]::GetFullPath($Path)
        }

        $underBase = [System.IO.Path]::GetFullPath((Join-Path $BaseDir $Path))
        if (Test-Path -LiteralPath $underBase -PathType Leaf) {
            return $underBase
        }

        $parent = [System.IO.Directory]::GetParent($BaseDir)
        if ($null -ne $parent) {
            $underParent = [System.IO.Path]::GetFullPath((Join-Path $parent.FullName $Path))
            if (Test-Path -LiteralPath $underParent -PathType Leaf) {
                return $underParent
            }
        }

        return $underBase
    } catch {
        return ''
    }
}

function ConvertTo-NormalizedPathOrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        return [System.IO.Path]::GetFullPath($Path)
    } catch {
        return ''
    }
}

function Test-PathInsideDirectory {
    param(
        [AllowEmptyString()][string]$Path,
        [AllowEmptyString()][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path) -or [string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $fullPath = [System.IO.Path]::GetFullPath($Path)
        $fullDirectory = [System.IO.Path]::GetFullPath($Directory)
        if (-not $fullDirectory.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
            $fullDirectory += [System.IO.Path]::DirectorySeparatorChar
        }

        return $fullPath.StartsWith($fullDirectory, [System.StringComparison]::OrdinalIgnoreCase)
    } catch {
        return $false
    }
}

function Test-BytePrefix {
    param(
        [AllowEmptyCollection()]
        [Parameter(Mandatory = $true)][byte[]]$Prefix,
        [AllowEmptyCollection()]
        [Parameter(Mandatory = $true)][byte[]]$Content
    )

    if ($Prefix.Length -gt $Content.Length) {
        return $false
    }

    for ($i = 0; $i -lt $Prefix.Length; $i++) {
        if ($Prefix[$i] -ne $Content[$i]) {
            return $false
        }
    }

    return $true
}

function Test-CurrentSliceFromBeforeAfter {
    param(
        [Parameter(Mandatory = $true)][string]$BeforePath,
        [Parameter(Mandatory = $true)][string]$AfterPath,
        [Parameter(Mandatory = $true)][string]$CurrentPath
    )

    $result = [ordered]@{
        PrefixMatches = $false
        SliceMatches = $false
        Detail = ''
    }

    try {
        $beforeBytes = [System.IO.File]::ReadAllBytes($BeforePath)
        $afterBytes = [System.IO.File]::ReadAllBytes($AfterPath)
        $currentBytes = [System.IO.File]::ReadAllBytes($CurrentPath)
        $result.PrefixMatches = Test-BytePrefix -Prefix $beforeBytes -Content $afterBytes
        if (-not $result.PrefixMatches) {
            $result.Detail = 'godot.log.after-launch does not have godot.log.before as a byte prefix'
            return [pscustomobject]$result
        }

        $sliceLength = $afterBytes.Length - $beforeBytes.Length
        if ($currentBytes.Length -ne $sliceLength) {
            $result.Detail = "current slice length $($currentBytes.Length) does not match after-before length $sliceLength"
            return [pscustomobject]$result
        }

        for ($i = 0; $i -lt $sliceLength; $i++) {
            if ($currentBytes[$i] -ne $afterBytes[$beforeBytes.Length + $i]) {
                $result.Detail = "current slice differs from after-launch at byte $i after the before-log prefix"
                return [pscustomobject]$result
            }
        }

        $result.SliceMatches = $true
        $result.Detail = 'godot.log.current-iteration matches godot.log.after-launch after the godot.log.before byte prefix'
        return [pscustomobject]$result
    } catch {
        $result.Detail = $_.Exception.Message
        return [pscustomobject]$result
    }
}

function Test-OrderedTextSequence {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string[]]$Needles
    )

    $offset = 0
    foreach ($needle in $Needles) {
        if ([string]::IsNullOrWhiteSpace($needle)) {
            return $false
        }

        $index = $Text.IndexOf($needle, $offset, [System.StringComparison]::OrdinalIgnoreCase)
        if ($index -lt 0) {
            return $false
        }

        $offset = $index + $needle.Length
    }

    return $true
}

function Test-TextContains {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    if ([string]::IsNullOrWhiteSpace($Needle)) {
        return $false
    }

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Test-AllJsonPropertiesPresent {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name) -or $null -eq $item.$Name) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AllJsonPropertiesRetained {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name)) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AnyJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ((Get-JsonBoolValue -Object $item -Name $Name -DefaultValue $false)) {
            return $true
        }
    }

    return $false
}

function Test-NoJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ((Test-JsonProperty -Object $item -Name $Name) -and ($null -eq $item.$Name -or $item.$Name -isnot [bool] -or [bool]$item.$Name)) {
            return $false
        }
    }

    return $true
}

function Test-NoJsonPropertyFalse {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ((Test-JsonProperty -Object $item -Name $Name) -and ($null -eq $item.$Name -or $item.$Name -isnot [bool] -or -not [bool]$item.$Name)) {
            return $false
        }
    }

    return $true
}

function Test-AnyJsonPropertyStringEquals {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Value
    )

    foreach ($item in @($Items)) {
        if ([string]::Equals([string](Get-JsonValue -Object $item -Name $Name -DefaultValue ''), $Value, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function ConvertTo-DateTimeOffsetParseResult {
    param([AllowEmptyString()][string]$Text)

    [System.DateTimeOffset]$value = [System.DateTimeOffset]::MinValue
    $styles = [System.Globalization.DateTimeStyles]::AssumeUniversal -bor [System.Globalization.DateTimeStyles]::AdjustToUniversal
    $parsed = (-not [string]::IsNullOrWhiteSpace($Text)) -and [System.DateTimeOffset]::TryParse($Text, [System.Globalization.CultureInfo]::InvariantCulture, $styles, [ref]$value)

    [pscustomobject]@{
        Parsed = $parsed
        Value = $value
    }
}

function Get-UnhealthyObservationFields {
    param(
        [AllowNull()]$Observation,
        [Parameter(Mandatory = $true)][string[]]$RequiredTrueFields,
        [Parameter(Mandatory = $true)][string[]]$RequiredFalseFields,
        [string]$ZeroCountField = ''
    )

    $failures = [System.Collections.Generic.List[string]]::new()
    if ($null -eq $Observation) {
        $failures.Add('missing') | Out-Null
        return @($failures.ToArray())
    }

    foreach ($field in $RequiredTrueFields) {
        if (-not (Get-JsonBoolValue -Object $Observation -Name $field -DefaultValue $false)) {
            $failures.Add($field) | Out-Null
        }
    }

    foreach ($field in $RequiredFalseFields) {
        if ((Get-JsonBoolValue -Object $Observation -Name $field -DefaultValue $true)) {
            $failures.Add($field) | Out-Null
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($ZeroCountField)) {
        if ((Get-JsonIntValue -Object $Observation -Name $ZeroCountField -DefaultValue -1) -ne 0) {
            $failures.Add($ZeroCountField) | Out-Null
        }
    }

    return @($failures.ToArray())
}

function Add-Finding {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.List[object]]$Findings,
        [Parameter(Mandatory = $true)][string]$Signal,
        [Parameter(Mandatory = $true)][string]$Severity,
        [Parameter(Mandatory = $true)][string]$OwnerArea,
        [Parameter(Mandatory = $true)][string]$Rationale,
        [Parameter(Mandatory = $true)][string]$NextStep,
        [ValidateSet('low', 'medium', 'high')]
        [string]$Confidence = 'medium',
        [string[]]$EvidenceFiles = @()
    )

    $Findings.Add([pscustomobject]@{
        Signal = $Signal
        Severity = $Severity
        OwnerArea = $OwnerArea
        Rationale = $Rationale
        NextStep = $NextStep
        Confidence = $Confidence
        EvidenceFiles = @($EvidenceFiles)
    }) | Out-Null
}

function Get-OwnerAreaFromText {
    param(
        [AllowEmptyString()][string]$Text,
        [AllowEmptyString()][string]$Command
    )

    $combined = "$Command`n$Text"

    if ($Text -match '(?i)\b(TypeLoadException|MissingMethodException|MissingFieldException|BaseLib patch failure|Creature\.get_ShowsInfiniteHp|runtime expectation|source drift|package drift|BaseLib\.Patches)\b|(?i)(?:\[ERROR\]\s+\[BaseLib\]|BaseLib.*(?:HarmonyException|Patching exception|patch(?:ing)? exception|failed))') {
        return 'PackageRuntimeDrift'
    }

    if ($Text -match '(?i)\b(Golden Idol|Big Fish|The Cleric|AdditiveBatch1|CanaryOnly|registered-event|Registered act event|Registered shared event|sts1-mode-log-check)\b') {
        return 'Sts1Events'
    }

    if ($Text -match '(?i)\b(Crystal Sphere|Transform Preview|Future Peek|PreviewTransform|PreviewCrystalSphere|Spire Plus\] Preview|prediction_prepared_multiplayer_ui_only|coop_local_ui_preview_enabled|Transform prediction|Crystal Sphere peek)\b') {
        return 'PreviewTools'
    }

    if ($Text -match '(?i)\b(coop|co-op|multiplayer|net=multi|coop_gameplay_disabled|coop_combat_hook_disabled|ALLOW_UNVERIFIED_COOP)\b') {
        return 'MultiplayerPolicy'
    }

    if ($combined -match '(?i)\b(Vakuu|Sere Talon)\b' -and
        $combined -match '(?i)\b(fight_started|child_combat_room_entered|parent_event_resume_success|fallback_map_exit|ParentEventId|prefinished|black.?screen|fade)\b') {
        return 'Ancients.Vakuu.ChildCombatResume'
    }

    if ($combined -match '(?i)\b(Vakuu|Sere Talon)\b' -and
        $combined -match '(?i)\b(fight_option_shown|force.?fight|confirm fight|fight.?option)\b') {
        return 'Ancients.Vakuu.FightOptionSetup'
    }

    if ($combined -match '(?i)\b(Morvi|Forbidden Loan|Red Ink|Open Book|Overdue Library|Blueprint Proof|Misprint)\b' -and
        $combined -match '(?i)\b(card.?play|borrowed|sealed.?card|freeze|hang|combat.?state|weak.?table)\b') {
        return 'Ancients.Morvi.CardPlayState'
    }

    if ($combined -match '(?i)\b(Lotha|Death Reprieve|Single Sentence|Mirror Hall|Deferred Verdict|Martyr)\b' -and
        $combined -match '(?i)\b(card.?play|ShouldPlay|ModifyCardPlayCount|extra.?play|phase|freeze|hang|combat.?state)\b') {
        return 'Ancients.Lotha.CardPlayState'
    }

    if ($combined -match '(?i)\b(Urda|Root Sight|Root Eyes|Seed Bank|Seedbed|Planting)\b' -and
        $combined -match '(?i)\b(map|save|load|hover|click|preview|entry|commit|queue|state|codec|deck.?mirror)\b') {
        return 'Ancients.Urda.MapSaveState'
    }

    if ($combined -match '(?i)\b(Rootblight|Blight Sprout|Seedbed)\b' -and
        $combined -match '(?i)\b(combat.?end|growth|downgrade|split|pending|hold|marker|save|load)\b') {
        return 'Ascension11To20.Rootblight'
    }

    if ($combined -match '(?i)\b(Vakuu|Sere Talon|Contract|Blood Debt|Stolen Vault|ParentEvent|broken lock)\b') {
        return 'Ancients.Vakuu'
    }

    if ($combined -match '(?i)\b(Morvi|Forbidden Loan|Red Ink|Debt Settlement|Open Book|Overdue Library|Blueprint Proof)\b') {
        return 'Ancients.Morvi'
    }

    if ($combined -match '(?i)\b(Lotha|Death Reprieve|Single Sentence|Mirror Hall|Deferred Verdict|Martyr)\b') {
        return 'Ancients.Lotha'
    }

    if ($combined -match '(?i)\b(Urda|Root Sight|Root Eyes|Seed Bank|Seedbed|Planting|Elite Root|Rooted Route|Trial Branch)\b') {
        return 'Ancients.Urda'
    }

    if ($combined -match '(?i)\b(StS1|Golden Idol|Big Fish|The Cleric|AdditiveBatch1|CanaryOnly)\b') {
        return 'Sts1Events'
    }

    if ($combined -match '(?i)\b(Ascension|Rootblight|Blight Sprout|Firemark|Banner|Boss Seal|Branded Form|Time Sand|Residual Sample)\b') {
        return 'Ascension11To20'
    }

    if ($combined -match '(?i)\b(Crystal Sphere|Transform Preview|Future Peek|PreviewTransform|PreviewCrystalSphere|Spire Plus\] Preview|prediction_prepared_multiplayer_ui_only|coop_local_ui_preview_enabled|Transform prediction|Crystal Sphere peek)\b') {
        return 'PreviewTools'
    }

    if ($combined -match '(?i)\b(coop|co-op|multiplayer|net=multi|coop_gameplay_disabled|coop_combat_hook_disabled)\b') {
        return 'MultiplayerPolicy'
    }

    return 'Runtime.Unknown'
}

function Get-AuditOwnerText {
    param(
        [AllowEmptyString()][string]$LogText,
        [AllowEmptyString()][string]$AuditName
    )

    if ([string]::IsNullOrWhiteSpace($LogText)) {
        return $AuditName
    }

    $ownerRelevantLines = @($LogText -split "`r?`n" | Where-Object {
        $_ -match '(?i)(ERROR|exception|TypeLoadException|MissingMethodException|MissingFieldException|BaseLib patch failure|Creature\.get_ShowsInfiniteHp|runtime expectation|source drift|package drift|StS1|Sts1|Golden Idol|Big Fish|The Cleric|AdditiveBatch1|CanaryOnly|registered-event|Registered act event|Registered shared event|Crystal Sphere|Transform Preview|Future Peek|PreviewTransform|PreviewCrystalSphere|Spire Plus\] Preview|prediction_prepared_multiplayer_ui_only|coop_local_ui_preview_enabled|Transform prediction|Crystal Sphere peek|coop|co-op|multiplayer|ALLOW_UNVERIFIED_COOP)'
    } | Select-Object -First 200)

    if ($ownerRelevantLines.Count -eq 0) {
        return $AuditName
    }

    return "$AuditName`n$($ownerRelevantLines -join "`n")"
}

function Resolve-OwnerArea {
    param(
        [AllowEmptyString()][string]$PlannedOwnerArea,
        [AllowEmptyString()][string]$LogOwnerArea,
        [AllowEmptyString()][string]$CommandOwnerArea,
        [switch]$PreferLog
    )

    $ownerCandidates = if ($PreferLog) {
        @($LogOwnerArea, $PlannedOwnerArea, $CommandOwnerArea)
    } else {
        @($PlannedOwnerArea, $CommandOwnerArea, $LogOwnerArea)
    }

    foreach ($owner in $ownerCandidates) {
        if (-not [string]::IsNullOrWhiteSpace($owner) -and $owner -ne 'Runtime.Unknown') {
            return $owner
        }
    }

    return 'Runtime.Unknown'
}

function Get-NextStepForOwner {
    param(
        [Parameter(Mandatory = $true)][string]$OwnerArea,
        [Parameter(Mandatory = $true)][string]$Signal
    )

    switch ($OwnerArea) {
        'Ancients.Vakuu.FightOptionSetup' {
            return 'Inspect Vakuu force-fight gate arming, fight-option visibility, evidence logging, and event UI setup before treating the packet as child-combat proof.'
        }
        'Ancients.Vakuu.ChildCombatResume' {
            return 'Inspect Vakuu parent event node cleanup, direct room stack transition, no-normal-reward resume, ParentEventId restore, fallback map exit, and prefinished heal-skip logs.'
        }
        'Ancients.Morvi.CardPlayState' {
            return 'Inspect Misprint extra-play, Forbidden Loan borrowed markers/cost, Red Ink, Open Book sealed-card restore, and combat-state weak-table ownership.'
        }
        'Ancients.Lotha.CardPlayState' {
            return 'Inspect ShouldPlay, ModifyCardPlayCount, extra-play canary decisions, Single Sentence caps, Mirror branches, and Death Reprieve phase restore.'
        }
        'Ancients.Urda.MapSaveState' {
            return 'Inspect Root Sight preview/entry commit, map UI patches, Seed Bank extraction, Seedbed queue/state, and state codec/deck mirror.'
        }
        'Ascension11To20.Rootblight' {
            return 'Inspect RootDeck combat lifecycle, pending downgrades, Seedbed hold markers, Blight Sprout growth exclusions, and save/load normalization.'
        }
        'Ancients.Vakuu' {
            return 'Inspect Vakuu child-combat transition, parent event cleanup, no-reward resume, death/failure path, and active-fight save-load logs before changing source.'
        }
        'Ancients.Morvi' {
            return 'Inspect Morvi borrowed-card markers, debt settlement, Red Ink/Open Book combat state, and save mirror paths around the failing turn.'
        }
        'Ancients.Lotha' {
            return 'Inspect Lotha Death Reprieve phase mirror, combat-state flags, card-play dispatch, and lethal-path logs around the failing action.'
        }
        'Ancients.Urda' {
            return 'Inspect Root Sight map marker state, Seed Bank extraction, Seedbed queue, Root Eyes hover/click, and save-load restoration for the marked run.'
        }
        'Ascension11To20' {
            return 'Inspect Ascension map/combat owner split, Rootblight lifecycle, Firemark/Banner marker logs, and boss dedicated ability source for the exact level.'
        }
        'Sts1Events' {
            return 'Run the retained StS1 log verifier against the copied log, then compare mode, registration count, and event class set against current source expectations.'
        }
        'PreviewTools' {
            return 'Inspect preview-tool local UI-only guards, RNG fork use, and co-op fail-open behavior; do not mutate rewards or real RNG while debugging.'
        }
        'MultiplayerPolicy' {
            return 'Confirm whether fail-closed co-op logs are expected; only use explicit SPIREPLUS_ALLOW_UNVERIFIED_COOP_* gates for focused two-client debugging.'
        }
        default {
            if ($Signal -match 'package|expectation|TypeLoad|MissingMethod') {
                return 'Check installed package parity, RitsuLib/BaseLib compatibility, and current game API targets before changing gameplay source.'
            }

            return 'Start from iteration-result.json, runtime-probe-samples.json, godot.log.after-launch, and godot-log-audit.json; narrow to the first failing signal.'
        }
    }
}

function Get-AuditHits {
    param([AllowEmptyCollection()][object[]]$AuditItems)

    $hits = [System.Collections.Generic.List[object]]::new()
    foreach ($item in @($AuditItems)) {
        foreach ($hit in (Get-JsonArrayValues -Object $item -Name 'SignatureHits')) {
            $count = Get-JsonIntValue -Object $hit -Name 'Count' -DefaultValue 0
            if ($count -gt 0) {
                $hits.Add([pscustomobject]@{
                    Name = [string](Get-JsonValue -Object $hit -Name 'Name' -DefaultValue '')
                    Count = $count
                }) | Out-Null
            }
        }
    }

    return @($hits.ToArray())
}

function Get-BaseLibPatchFailureDetails {
    param([AllowEmptyString()][string]$LogText)

    $details = [System.Collections.Generic.List[object]]::new()
    if ([string]::IsNullOrWhiteSpace($LogText)) {
        return @($details.ToArray())
    }

    $lines = @($LogText -split "`r?`n")
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = [string]$lines[$i]
        if ($line -notmatch '(?i)^\s*\[ERROR\]\s+\[BaseLib\].*HarmonyException|^\s*\[ERROR\]\s+\[BaseLib\].*Patching exception') {
            continue
        }

        $block = [System.Collections.Generic.List[string]]::new()
        $block.Add($line.Trim()) | Out-Null
        for ($j = $i + 1; $j -lt $lines.Count; $j++) {
            $nextLine = [string]$lines[$j]
            if ($nextLine -match '^\s*\[(?:INFO|WARN|ERROR)\]' -and $nextLine -notmatch '^\s*\[ERROR\]\s+\[BaseLib\]') {
                break
            }

            $block.Add($nextLine.TrimEnd()) | Out-Null
            if ($block.Count -ge 24) {
                break
            }
        }

        $blockText = ($block -join "`n")
        $targetMethod = ''
        $patchMethod = ''
        $failureKind = 'BaseLib patch failure'
        $summary = $block[0]

        if ($blockText -match '(?m)Patching exception in method (?<target>.+)$') {
            $targetMethod = $Matches['target'].Trim()
            if ([string]::Equals($targetMethod, 'null', [System.StringComparison]::OrdinalIgnoreCase)) {
                $targetMethod = ''
            }
        }
        if ($blockText -match '(?m)Undefined target method for patch method (?<patch>.+)$') {
            $failureKind = 'Undefined target method'
            $patchMethod = $Matches['patch'].Trim()
            $summary = "Undefined target method for patch method $patchMethod"
        } elseif ($blockText -match '(?m)Failed to find match:') {
            $failureKind = 'Instruction matcher failed'
            $summary = if ([string]::IsNullOrWhiteSpace($targetMethod)) { 'Failed to find match' } else { "Failed to find match in $targetMethod" }
        }

        $snippet = @($block | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -First 8)
        $details.Add([pscustomobject]@{
            FailureKind = $failureKind
            TargetMethod = $targetMethod
            PatchMethod = $patchMethod
            Summary = $summary
            Snippet = @($snippet)
        }) | Out-Null
    }

    if ($LogText -match '(?im)^\s*\[INFO\]\s+\[BaseLib\]\s+Applied\s+(?<applied>\d+)\s+patches\s+successfully,\s+(?<failed>\d+)\s+failed') {
        $details.Add([pscustomobject]@{
            FailureKind = 'Patch summary'
            TargetMethod = ''
            PatchMethod = ''
            Summary = "BaseLib applied $($Matches['applied']) patches successfully, $($Matches['failed']) failed"
            Snippet = @($Matches[0].Trim())
        }) | Out-Null
    }

    return @($details.ToArray())
}

function ConvertTo-AuditSummary {
    param([AllowNull()]$Audit)

    $items = @($Audit)
    $dirtyItems = 0
    $hitCount = 0
    $itemPaths = [System.Collections.Generic.List[string]]::new()
    $itemLengths = [System.Collections.Generic.List[long]]::new()
    $itemSha256s = [System.Collections.Generic.List[string]]::new()

    foreach ($item in $items) {
        if (-not (Get-JsonBoolValue -Object $item -Name 'Clean' -DefaultValue $false)) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Path') -and -not [string]::IsNullOrWhiteSpace([string]$item.Path)) {
            $itemPaths.Add([System.IO.Path]::GetFullPath([string]$item.Path)) | Out-Null
        }

        if (Test-JsonProperty -Object $item -Name 'Length') {
            $itemLengths.Add((ConvertTo-LongOrDefault -Value $item.Length -DefaultValue -1)) | Out-Null
        }

        if ((Test-JsonProperty -Object $item -Name 'Sha256') -and -not [string]::IsNullOrWhiteSpace([string]$item.Sha256)) {
            $itemSha256s.Add(([string]$item.Sha256).ToLowerInvariant()) | Out-Null
        }

        foreach ($hit in (Get-JsonArrayValues -Object $item -Name 'SignatureHits')) {
            if (Test-JsonProperty -Object $hit -Name 'Count') {
                $hitCount += ConvertTo-IntOrDefault -Value $hit.Count -DefaultValue 0
            }
        }
    }

    return [pscustomobject]@{
        Items = $items.Count
        ItemPaths = @($itemPaths)
        ItemLengths = @($itemLengths)
        ItemSha256s = @($itemSha256s)
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0)
    }
}

function Invoke-RecomputedAudit {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $auditJson = (& $logAuditScript -Path $LogPath | Out-String)
    if ([string]::IsNullOrWhiteSpace($auditJson)) {
        throw "audit-godot-log.ps1 returned empty output for $LogPath"
    }

    return $auditJson | ConvertFrom-Json
}

function Get-CheckSignatureArray {
    param([AllowNull()]$Items)

    if ($null -eq $Items) {
        return @()
    }

    return @($Items | ForEach-Object {
        $name = [string](Get-JsonValue -Object $_ -Name 'Name' -DefaultValue '')
        $passed = (Get-JsonBoolValue -Object $_ -Name 'Passed' -DefaultValue $false)
        $detail = [string](Get-JsonValue -Object $_ -Name 'Detail' -DefaultValue '')
        "${name}|${passed}|${detail}"
    })
}

function Invoke-RecomputedSts1ModeLogCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Mode,
        [Parameter(Mandatory = $true)][string]$LogPath,
        [Parameter(Mandatory = $true)][string]$AuditPath,
        [AllowEmptyString()][string]$EffectiveExpectedPackageVersion,
        [AllowEmptyString()][string]$EffectiveExpectedGameVersion,
        [AllowEmptyString()][string]$EffectiveExpectedRitsuLibVersion,
        [AllowEmptyString()][string]$EffectiveExpectedRitsuCompatBranch
    )

    $outFile = Join-Path ([System.IO.Path]::GetTempPath()) "spireplus-analyzer-sts1-mode-log-check-$([System.Guid]::NewGuid().ToString('N')).json"
    try {
        $verifierParams = @{
            Mode = $Mode
            LogPath = $LogPath
            AuditPath = $AuditPath
            OutFile = $outFile
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedPackageVersion)) {
            $verifierParams['ExpectedPackageVersion'] = $EffectiveExpectedPackageVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedGameVersion)) {
            $verifierParams['ExpectedGameVersion'] = $EffectiveExpectedGameVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedRitsuLibVersion)) {
            $verifierParams['ExpectedRitsuLibVersion'] = $EffectiveExpectedRitsuLibVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedRitsuCompatBranch)) {
            $verifierParams['ExpectedRitsuCompatBranch'] = $EffectiveExpectedRitsuCompatBranch
        }

        $verifierOutput = @(& $sts1EnabledModeLogVerifierScript @verifierParams 2>&1)
        if (-not (Test-Path -LiteralPath $outFile -PathType Leaf)) {
            throw "check-sts1-enabled-mode-runtime-log.ps1 did not write a recomputed report. Output: $($verifierOutput -join [Environment]::NewLine)"
        }

        return Get-Content -LiteralPath $outFile -Raw -Encoding UTF8 | ConvertFrom-Json
    } finally {
        if (Test-Path -LiteralPath $outFile -PathType Leaf) {
            Remove-Item -LiteralPath $outFile -Force
        }
    }
}

function Test-HarnessOwnerArea {
    param([AllowEmptyString()][string]$OwnerArea)

    if ([string]::IsNullOrWhiteSpace($OwnerArea)) {
        return $false
    }

    return $OwnerArea -match '^(RuntimeHarness|RuntimeStartup|RuntimeCrash|RuntimeLogAudit|DevConsoleHarness|LiveSessionRestore)$'
}

function Analyze-Iteration {
    param(
        [Parameter(Mandatory = $true)][string]$Directory,
        [AllowNull()]$SummaryResult,
        [string]$ResultFileName = 'iteration-result.json',
        [int]$DefaultIteration = 0,
        [bool]$RunResultPathInsideEvidenceDir = $true,
        [string]$ExpectedRunnerKind = '',
        [string[]]$SummaryFailedIterationIdsInvalidDetails = @(),
        [string[]]$SummaryMismatchDetails = @(),
        [bool]$RequireRuntimeMonkeyPlanBinding = $false,
        [AllowNull()]$Summary = $null,
        [AllowNull()]$Plan = $null
    )

    $resultPath = Join-Path $Directory $ResultFileName
    $result = Read-JsonOrNull -Path $resultPath
    $iterationResultMissing = $null -eq $result
    if ($null -eq $result -and $null -ne $SummaryResult) {
        $result = $SummaryResult
    }

    $runnerKind = if ($result) { [string](Get-JsonValue -Object $result -Name 'RunnerKind' -DefaultValue '') } else { '' }
    if ([string]::IsNullOrWhiteSpace($runnerKind) -and [string]::Equals($ResultFileName, 'direct-smoke-summary.json', [System.StringComparison]::OrdinalIgnoreCase)) {
        $runnerKind = 'DirectSmoke'
    }
    $observedRunnerKind = $runnerKind
    $runnerKindMatchesExpectedTarget = $true
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRunnerKind) -and
        -not [string]::Equals($runnerKind, $ExpectedRunnerKind, [System.StringComparison]::Ordinal)) {
        $runnerKindMatchesExpectedTarget = $false
        $runnerKind = $ExpectedRunnerKind
    }

    $isGameNativeAutoSlay = [string]::Equals($runnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)
    $isDirectSmoke = [string]::Equals($runnerKind, 'DirectSmoke', [System.StringComparison]::Ordinal)
    $seed = if ($result) { [string](Get-JsonValue -Object $result -Name 'Seed' -DefaultValue '') } else { '' }
    $eventKind = if ($result) { [string](Get-JsonValue -Object $result -Name 'EventKind' -DefaultValue '') } else { '' }
    $ancientId = if ($result) { [string](Get-JsonValue -Object $result -Name 'AncientId' -DefaultValue '') } else { '' }
    $invocation = if ($result) { [string](Get-JsonValue -Object $result -Name 'Invocation' -DefaultValue '') } else { '' }
    $command = if ($result) { [string](Get-JsonValue -Object $result -Name 'Command' -DefaultValue '') } else { '' }
    if ([string]::IsNullOrWhiteSpace($command) -and -not [string]::IsNullOrWhiteSpace($invocation)) {
        $command = $invocation
    }
    if ([string]::IsNullOrWhiteSpace($eventKind) -and $SummaryResult) {
        $eventKind = [string](Get-JsonValue -Object $SummaryResult -Name 'EventKind' -DefaultValue '')
    }
    if ([string]::IsNullOrWhiteSpace($ancientId) -and $SummaryResult) {
        $ancientId = [string](Get-JsonValue -Object $SummaryResult -Name 'AncientId' -DefaultValue '')
    }
    $resultOwnerArea = if ($result) { [string](Get-JsonValue -Object $result -Name 'OwnerArea' -DefaultValue '') } else { '' }
    $scenarioTag = if ($result) { [string](Get-JsonValue -Object $result -Name 'ScenarioTag' -DefaultValue '') } else { '' }
    if ([string]::IsNullOrWhiteSpace($scenarioTag) -and $isGameNativeAutoSlay) {
        $scenarioTag = 'game-native-autoslay'
    } elseif ([string]::IsNullOrWhiteSpace($scenarioTag) -and $isDirectSmoke) {
        $scenarioTag = 'direct-smoke'
    }

    $analysisPlan = $Plan
    if ($null -eq $analysisPlan) {
        $planParent = [System.IO.Directory]::GetParent($Directory)
        if ($null -ne $planParent) {
            $planName = if ($isGameNativeAutoSlay) { 'autoslay-plan.json' } else { 'monkey-plan.json' }
            $analysisPlan = Read-JsonOrNull -Path (Join-Path $planParent.FullName $planName)
        }
    }
    $analysisSummary = $Summary
    if ($null -eq $analysisSummary -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        $summaryParent = [System.IO.Directory]::GetParent($Directory)
        if ($null -ne $summaryParent) {
            $analysisSummary = Read-JsonOrNull -Path (Join-Path $summaryParent.FullName 'monkey-summary.json')
        }
    }

    $expectedSts1Mode = Get-FirstJsonString -Object $analysisPlan -Names @('Sts1EventMode')
    $effectiveExpectedPackageVersion = Get-FirstJsonString -Object $analysisPlan -Names @('ExpectedPackageVersion', 'PackageVersion')
    $effectiveExpectedGameVersion = Get-FirstJsonString -Object $analysisPlan -Names @('ExpectedGameVersion', 'GameVersion')
    $effectiveExpectedRitsuLibVersion = Get-FirstJsonString -Object $analysisPlan -Names @('ExpectedRitsuLibVersion', 'RitsuLibVersion')
    $effectiveExpectedRitsuCompatBranch = Get-FirstJsonString -Object $analysisPlan -Names @('ExpectedRitsuCompatBranch', 'RitsuCompatBranch')

    $canonicalBeforeLogCandidate = Join-Path $Directory 'godot.log.before'
    $canonicalFullLogCandidate = Join-Path $Directory 'godot.log.after-launch'
    $canonicalCurrentIterationLogCandidate = Join-Path $Directory 'godot.log.current-iteration'
    $canonicalProbeSamplesCandidate = Join-Path $Directory 'runtime-probe-samples.json'
    $canonicalSessionStateCandidate = Join-Path $Directory 'session-state.json'
    $canonicalRestoreStateCandidate = Join-Path $Directory 'restore-state.json'
    $beforeLogCandidate = $canonicalBeforeLogCandidate
    $fullLogCandidate = $canonicalFullLogCandidate
    $currentIterationLogCandidate = $canonicalCurrentIterationLogCandidate
    $auditCandidate = Join-Path $Directory 'godot-log-audit.json'
    $probeSamplesCandidate = $canonicalProbeSamplesCandidate
    $sessionStateCandidate = $canonicalSessionStateCandidate
    $restoreStateCandidate = $canonicalRestoreStateCandidate
    $sts1ModeCandidate = Join-Path $Directory 'sts1-mode-log-check.json'
    if ($result) {
        $beforeLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogBeforePath' -DefaultValue 'godot.log.before'))
        $fullLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogAfterLaunchPath' -DefaultValue 'godot.log.after-launch'))
        $currentIterationLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogCurrentIterationPath' -DefaultValue 'godot.log.current-iteration'))
        $probeSamplesCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'RuntimeProbeSamplesPath' -DefaultValue 'runtime-probe-samples.json'))
        $sessionStateCandidate = if (Test-JsonProperty -Object $result -Name 'LiveSessionSessionStatePath') {
            Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'LiveSessionSessionStatePath' -DefaultValue ''))
        } else {
            ''
        }
        $restoreStateCandidate = if (Test-JsonProperty -Object $result -Name 'LiveSessionRestoreStatePath') {
            Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'LiveSessionRestoreStatePath' -DefaultValue ''))
        } else {
            ''
        }
    }
    if ($isGameNativeAutoSlay -and $result) {
        $auditCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogAuditPath' -DefaultValue 'godot-log-audit.json'))
        $sts1ModeCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'Sts1ModeLogCheckPath' -DefaultValue 'sts1-mode-log-check.json'))
    }

    $logCandidate = if (-not [string]::IsNullOrWhiteSpace($currentIterationLogCandidate) -and (Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf)) {
        $currentIterationLogCandidate
    } else {
        $fullLogCandidate
    }

    $autoSlayLogCandidate = if ($isGameNativeAutoSlay -and $result) {
        Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'AutoSlayLogPath' -DefaultValue 'autoslay.log'))
    } else {
        Join-Path $Directory 'autoslay.log'
    }
    $findings = [System.Collections.Generic.List[object]]::new()
    $candidateEvidenceFiles = @(
        $resultPath,
        $beforeLogCandidate,
        $currentIterationLogCandidate,
        $fullLogCandidate,
        $auditCandidate,
        $probeSamplesCandidate,
        $sessionStateCandidate,
        $restoreStateCandidate,
        $sts1ModeCandidate,
        $autoSlayLogCandidate
    )
    $evidenceFiles = @($candidateEvidenceFiles | Where-Object {
        -not [string]::IsNullOrWhiteSpace([string]$_) -and (Test-Path -LiteralPath $_ -PathType Leaf)
    })
    $runtimeMonkeyRunArtifactsTrustedForOwner = $true
    $runtimeMonkeyProbeArtifactTrustedForOwner = $true
    $autoSlayRunArtifactsTrustedForOwner = -not $isGameNativeAutoSlay
    $autoSlayProbeArtifactTrustedForOwner = -not $isGameNativeAutoSlay
    $autoSlayAuditArtifactTrustedForOwner = -not $isGameNativeAutoSlay
    $autoSlaySts1ModeArtifactTrustedForOwner = -not $isGameNativeAutoSlay
    $autoSlaySidecarPathTrustedForOwner = -not $isGameNativeAutoSlay
    $sts1ModeLogCheckTrustedForOwner = $true
    if (-not $runnerKindMatchesExpectedTarget) {
        $signal = if ([string]::Equals($ExpectedRunnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)) {
            'autoslay_run_result_runner_kind_mismatch'
        } else {
            'analysis_target_runner_kind_mismatch'
        }
        Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Evidence target was selected as '$ExpectedRunnerKind' but the retained run result reported RunnerKind='$observedRunnerKind'." -NextStep 'Regenerate or reject the packet; summary target type and retained per-run result type must agree before owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    $summaryFailedIterationIdsInvalidDetailArray = @($SummaryFailedIterationIdsInvalidDetails)
    $summaryFailedIterationIdsInvalidDetailCount = ($SummaryFailedIterationIdsInvalidDetails | Measure-Object).Count
    if ($summaryFailedIterationIdsInvalidDetailCount -gt 0 -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_summary_failed_iteration_ids_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "monkey-summary.json FailedIterationIds contains invalid target entries: $($summaryFailedIterationIdsInvalidDetailArray -join '; ')." -NextStep 'Regenerate or reject monkey-summary.json; failed iteration ids must be positive integers before summary-directed owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    $summaryMismatchDetailArray = @($SummaryMismatchDetails)
    $summaryMismatchDetailCount = ($summaryMismatchDetailArray | Measure-Object).Count
    if ($summaryMismatchDetailCount -gt 0 -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_summary_counter_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "monkey-summary.json top-level counters do not match Results[] aggregation: $($summaryMismatchDetailArray -join '; ')." -NextStep 'Regenerate or reject monkey-summary.json; top-level summary counters must match Results[] before owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    $summaryResultMismatchDetails = if ($null -ne $SummaryResult -and -not $iterationResultMissing -and $result -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        @(Get-RuntimeMonkeySummaryResultMismatchDetails -Result $result -SummaryResult $SummaryResult)
    } else {
        @()
    }
    if (($summaryResultMismatchDetails | Measure-Object).Count -gt 0) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_summary_result_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "monkey-summary.json Results[] row does not match iteration-result.json: $($summaryResultMismatchDetails -join '; ')." -NextStep 'Regenerate or reject monkey-summary.json; summary Results rows must match canonical iteration-result.json before owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    if ($RequireRuntimeMonkeyPlanBinding -and $result -and -not $iterationResultMissing -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke -and
        ($null -eq $analysisPlan -or -not (Test-JsonProperty -Object $analysisPlan -Name 'PlannedCommands'))) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_plan_missing_or_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey batch evidence did not retain a parseable monkey-plan.json with PlannedCommands.' -NextStep 'Regenerate or reject the packet; batch owner routing requires retained monkey-plan.json PlannedCommands binding.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    $summaryPlanMismatchDetails = if ($null -ne $analysisSummary -and $result -and -not $iterationResultMissing -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        @(Get-RuntimeMonkeySummaryPlanMismatchDetails -Summary $analysisSummary -Plan $analysisPlan)
    } else {
        @()
    }
    if (($summaryPlanMismatchDetails | Measure-Object).Count -gt 0) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_summary_plan_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "monkey-summary.json batch metadata does not match monkey-plan.json: $($summaryPlanMismatchDetails -join '; ')." -NextStep 'Regenerate or reject the packet; summary batch metadata must match retained monkey-plan.json before owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }
    $planResultMismatchDetails = if ($result -and -not $iterationResultMissing -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        @(Get-RuntimeMonkeyPlanResultMismatchDetails -Plan $analysisPlan -Result $result)
    } else {
        @()
    }
    if (($planResultMismatchDetails | Measure-Object).Count -gt 0) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        Add-Finding -Findings $findings -Signal 'runtime_monkey_plan_result_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "monkey-plan.json PlannedCommands row does not match iteration-result.json: $($planResultMismatchDetails -join '; ')." -NextStep 'Regenerate or reject the packet; planned command metadata must match canonical iteration-result.json before owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($result -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        $runtimeMonkeyRequiredArtifacts = @(
            [pscustomobject]@{ Label = 'godot.log.before'; OutsideSignal = 'runtime_monkey_before_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_before_log_not_retained_file'; Path = $beforeLogCandidate; CanonicalPath = $canonicalBeforeLogCandidate; NextStep = 'Retain godot.log.before as the standard file in the iteration directory before using runtime-monkey log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.after-launch'; OutsideSignal = 'runtime_monkey_after_launch_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_after_launch_log_not_retained_file'; Path = $fullLogCandidate; CanonicalPath = $canonicalFullLogCandidate; NextStep = 'Retain godot.log.after-launch as the standard file in the iteration directory before using runtime-monkey log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.current-iteration'; OutsideSignal = 'runtime_monkey_current_iteration_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_current_iteration_log_not_retained_file'; Path = $currentIterationLogCandidate; CanonicalPath = $canonicalCurrentIterationLogCandidate; NextStep = 'Retain godot.log.current-iteration as the standard file in the iteration directory before using runtime-monkey log lines for owner routing.' },
            [pscustomobject]@{ Label = 'runtime-probe-samples.json'; FieldName = 'RuntimeProbeSamplesPath'; HashField = 'RuntimeProbeSamplesSha256'; MissingPathSignal = 'runtime_monkey_runtime_probe_samples_path_missing'; MissingFileSignal = 'runtime_monkey_runtime_probe_samples_missing'; MissingHashSignal = 'runtime_monkey_runtime_probe_samples_hash_missing'; HashMismatchSignal = 'runtime_monkey_runtime_probe_samples_hash_mismatch'; OutsideSignal = 'runtime_monkey_runtime_probe_samples_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_runtime_probe_samples_not_retained_file'; Path = $probeSamplesCandidate; CanonicalPath = $canonicalProbeSamplesCandidate; NextStep = 'Retain runtime-probe-samples.json as the standard file in the iteration directory before using runtime-monkey probe telemetry for triage.' },
            [pscustomobject]@{ Label = 'session-state.json'; FieldName = 'LiveSessionSessionStatePath'; HashField = 'LiveSessionSessionStateSha256'; MissingPathSignal = 'runtime_monkey_session_state_path_missing'; MissingFileSignal = 'runtime_monkey_session_state_missing'; MissingHashSignal = 'runtime_monkey_session_state_hash_missing'; HashMismatchSignal = 'runtime_monkey_session_state_hash_mismatch'; OutsideSignal = 'runtime_monkey_session_state_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_session_state_not_retained_file'; Path = $sessionStateCandidate; CanonicalPath = $canonicalSessionStateCandidate; NextStep = 'Retain session-state.json as the standard file in the iteration directory before trusting live-session restore transaction evidence.' },
            [pscustomobject]@{ Label = 'restore-state.json'; FieldName = 'LiveSessionRestoreStatePath'; HashField = 'LiveSessionRestoreStateSha256'; MissingPathSignal = 'runtime_monkey_restore_state_path_missing'; MissingFileSignal = 'runtime_monkey_restore_state_missing'; MissingHashSignal = 'runtime_monkey_restore_state_hash_missing'; HashMismatchSignal = 'runtime_monkey_restore_state_hash_mismatch'; OutsideSignal = 'runtime_monkey_restore_state_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_restore_state_not_retained_file'; Path = $restoreStateCandidate; CanonicalPath = $canonicalRestoreStateCandidate; NextStep = 'Retain restore-state.json as the standard file in the iteration directory before trusting live-session restore transaction evidence.' }
        )

        foreach ($artifact in $runtimeMonkeyRequiredArtifacts) {
            $artifactPath = [string]$artifact.Path
            $artifactFieldName = if (Test-JsonProperty -Object $artifact -Name 'FieldName') { [string]$artifact.FieldName } else { '' }
            $artifactHashField = if (Test-JsonProperty -Object $artifact -Name 'HashField') { [string]$artifact.HashField } else { '' }
            $artifactIsRuntimeProbeSamples = [string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)
            if (-not [string]::IsNullOrWhiteSpace($artifactFieldName) -and
                (-not (Test-JsonProperty -Object $result -Name $artifactFieldName) -or [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $result -Name $artifactFieldName -DefaultValue '')))) {
                $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                if ($artifactIsRuntimeProbeSamples) {
                    $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.MissingPathSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey result JSON did not retain $artifactFieldName for $($artifact.Label)." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }
            if ([string]::IsNullOrWhiteSpace($artifactPath)) {
                $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                if ($artifactIsRuntimeProbeSamples) {
                    $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                }

                continue
            }

            $artifactInsideIteration = Test-PathInsideDirectory -Path $artifactPath -Directory $Directory
            $artifactFullPath = ConvertTo-NormalizedPathOrEmpty -Path $artifactPath
            $canonicalFullPath = ConvertTo-NormalizedPathOrEmpty -Path ([string]$artifact.CanonicalPath)
            $artifactMatchesCanonical = -not [string]::IsNullOrWhiteSpace($artifactFullPath) -and
                -not [string]::IsNullOrWhiteSpace($canonicalFullPath) -and
                [System.StringComparer]::OrdinalIgnoreCase.Equals($artifactFullPath, $canonicalFullPath)
            if (-not $artifactInsideIteration -or -not $artifactMatchesCanonical) {
                $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                if ($artifactIsRuntimeProbeSamples) {
                    $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                }

                if (-not $artifactInsideIteration) {
                    Add-Finding -Findings $findings -Signal ([string]$artifact.OutsideSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $($artifact.Label) resolved outside the per-iteration evidence directory." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                } else {
                    Add-Finding -Findings $findings -Signal ([string]$artifact.NonCanonicalSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $($artifact.Label) did not resolve to the retained standard iteration file." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
            if (-not [string]::IsNullOrWhiteSpace($artifactHashField)) {
                $artifactFileExists = Test-Path -LiteralPath $artifactPath -PathType Leaf
                if (-not $artifactFileExists) {
                    $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    if ($artifactIsRuntimeProbeSamples) {
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal ([string]$artifact.MissingFileSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $($artifact.Label) path did not point to an existing retained file." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                    continue
                }

                $recordedArtifactSha256 = [string](Get-JsonValue -Object $result -Name $artifactHashField -DefaultValue '')
                if (-not (Test-JsonProperty -Object $result -Name $artifactHashField) -or -not (Test-Sha256Text -Value $recordedArtifactSha256)) {
                    $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    if ($artifactIsRuntimeProbeSamples) {
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal ([string]$artifact.MissingHashSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey result JSON did not retain a valid $artifactHashField for $($artifact.Label)." -NextStep 'Record SHA256 bindings for retained runtime monkey artifacts before trusting probe, restore, or gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    continue
                }

                $actualArtifactSha256 = Get-FileSha256OrEmpty -Path $artifactPath
                if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($recordedArtifactSha256, $actualArtifactSha256)) {
                    $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    if ($artifactIsRuntimeProbeSamples) {
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal ([string]$artifact.HashMismatchSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $artifactHashField does not match retained $($artifact.Label); recorded=$recordedArtifactSha256 actual=$actualArtifactSha256." -NextStep 'Regenerate or reject the packet; do not route ownership from runtime monkey artifacts whose retained hashes have drifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
        }
    }
    if ($isGameNativeAutoSlay -and $result) {
        $autoSlayRunArtifactsTrustedForOwner = $RunResultPathInsideEvidenceDir -and -not $iterationResultMissing -and $runnerKindMatchesExpectedTarget
        $autoSlayProbeArtifactTrustedForOwner = $true
        $autoSlayAuditArtifactTrustedForOwner = $true
        $autoSlaySts1ModeArtifactTrustedForOwner = $true
        $autoSlaySidecarPathTrustedForOwner = $true
        $autoSlaySummaryPlanMismatchDetails = @(Get-AutoSlaySummaryPlanMismatchDetails -Summary $analysisSummary -Plan $analysisPlan)
        if (($autoSlaySummaryPlanMismatchDetails | Measure-Object).Count -gt 0) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            Add-Finding -Findings $findings -Signal 'autoslay_summary_plan_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay autoslay-summary.json batch metadata does not match autoslay-plan.json: $($autoSlaySummaryPlanMismatchDetails -join '; ')." -NextStep 'Regenerate or reject the packet; AutoSlay summary batch metadata must match retained autoslay-plan.json before owner routing is trusted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if (-not $RunResultPathInsideEvidenceDir) {
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_path_outside_evidence_dir' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay autoslay-summary.json RunResultPath resolved outside the retained evidence directory.' -NextStep 'Retain each run-result.json under the AutoSlay evidence root before analyzing per-seed artifacts or routing source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if ($null -ne $SummaryResult) {
            $summaryRunResultSha256 = [string](Get-JsonValue -Object $SummaryResult -Name 'RunResultSha256' -DefaultValue '')
            if (-not (Test-JsonProperty -Object $SummaryResult -Name 'RunResultSha256') -or -not (Test-Sha256Text -Value $summaryRunResultSha256)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'autoslay_run_result_summary_hash_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay autoslay-summary.json did not retain a valid RunResultSha256 for the per-seed run-result.json.' -NextStep 'Record RunResultSha256 in autoslay-summary.json before trusting run-result.json for source ownership routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } elseif ((Test-Path -LiteralPath $resultPath -PathType Leaf) -and -not [System.StringComparer]::OrdinalIgnoreCase.Equals((Get-FileSha256OrEmpty -Path $resultPath), $summaryRunResultSha256)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'autoslay_run_result_summary_hash_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay autoslay-summary.json RunResultSha256 does not match the retained run-result.json bytes.' -NextStep 'Regenerate or reject the packet; summary rows must hash-bind the exact retained run-result.json before owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }

            if (-not $iterationResultMissing) {
                if (Test-JsonProperty -Object $SummaryResult -Name 'Passed') {
                    $summaryPassedIsBool = Test-JsonBoolProperty -Object $SummaryResult -Name 'Passed'
                    $resultPassedIsBool = Test-JsonBoolProperty -Object $result -Name 'Passed'
                    if (-not $summaryPassedIsBool -or -not $resultPassedIsBool) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_passed_boolean_malformed' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay Passed fields must be native JSON booleans; summary=$summaryPassedIsBool result=$resultPassedIsBool." -NextStep 'Regenerate or reject the packet; string/null pass-fail fields cannot be used for owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    } else {
                        $summaryPassed = (Get-JsonBoolValue -Object $SummaryResult -Name 'Passed' -DefaultValue $false)
                        $resultPassed = (Get-JsonBoolValue -Object $result -Name 'Passed' -DefaultValue $false)
                        if ($summaryPassed -ne $resultPassed) {
                            $autoSlayRunArtifactsTrustedForOwner = $false
                            Add-Finding -Findings $findings -Signal 'autoslay_summary_passed_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay autoslay-summary.json Runs[] Passed disagrees with the retained run-result.json.' -NextStep 'Regenerate or reject the packet; summary pass/fail state must match the retained run-result.json before owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                        }
                    }
                }

                $summarySignalComparisons = @(
                    [pscustomobject]@{ FieldName = 'FailureReasonCodes'; Signal = 'autoslay_summary_failure_reason_codes_mismatch'; Rationale = 'GameNativeAutoSlay autoslay-summary.json Runs[] FailureReasonCodes disagrees with the retained run-result.json.' },
                    [pscustomobject]@{ FieldName = 'HangSignals'; Signal = 'autoslay_summary_hang_signals_mismatch'; Rationale = 'GameNativeAutoSlay autoslay-summary.json Runs[] HangSignals disagrees with the retained run-result.json.' }
                )
                foreach ($comparison in $summarySignalComparisons) {
                    $fieldName = [string]$comparison.FieldName
                    if ((Test-JsonProperty -Object $SummaryResult -Name $fieldName) -or (Test-JsonProperty -Object $result -Name $fieldName)) {
                        $summarySignalsAreArray = Test-JsonArrayProperty -Object $SummaryResult -Name $fieldName
                        $resultSignalsAreArray = Test-JsonArrayProperty -Object $result -Name $fieldName
                        if (-not $summarySignalsAreArray -or -not $resultSignalsAreArray) {
                            $autoSlayRunArtifactsTrustedForOwner = $false
                            Add-Finding -Findings $findings -Signal 'autoslay_signal_array_malformed' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $fieldName fields must be native JSON arrays; summary=$summarySignalsAreArray result=$resultSignalsAreArray." -NextStep 'Regenerate or reject the packet; scalar/null failure and hang signal fields cannot be used for owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                            continue
                        }

                        $summarySignals = Get-JsonArrayValues -Object $SummaryResult -Name $fieldName
                        $resultSignals = Get-JsonArrayValues -Object $result -Name $fieldName
                        if (-not (Test-StringArrayEquals -Actual @($summarySignals) -Expected @($resultSignals))) {
                            $autoSlayRunArtifactsTrustedForOwner = $false
                            Add-Finding -Findings $findings -Signal ([string]$comparison.Signal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale ([string]$comparison.Rationale) -NextStep 'Regenerate or reject the packet; summary failure and hang signals must match the retained run-result.json before owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                        }
                    }
                }
            }
        }

        $autoSlayRequiredArtifacts = @(
            [pscustomobject]@{ Label = 'godot.log.before'; FieldName = 'GodotLogBeforePath'; Signal = 'autoslay_before_log_outside_run_dir'; MissingSignal = 'autoslay_before_log_path_missing'; NonCanonicalSignal = 'autoslay_before_log_not_retained_file'; Path = $beforeLogCandidate; CanonicalPath = $canonicalBeforeLogCandidate; NextStep = 'Retain godot.log.before beside run-result.json before using AutoSlay log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.after-launch'; FieldName = 'GodotLogAfterLaunchPath'; Signal = 'autoslay_after_launch_log_outside_run_dir'; MissingSignal = 'autoslay_after_launch_log_path_missing'; NonCanonicalSignal = 'autoslay_after_launch_log_not_retained_file'; Path = $fullLogCandidate; CanonicalPath = $canonicalFullLogCandidate; NextStep = 'Retain godot.log.after-launch beside run-result.json before using AutoSlay log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.current-iteration'; FieldName = 'GodotLogCurrentIterationPath'; Signal = 'autoslay_current_iteration_log_outside_run_dir'; MissingSignal = 'autoslay_current_iteration_log_path_missing'; NonCanonicalSignal = 'autoslay_current_iteration_log_not_retained_file'; Path = $currentIterationLogCandidate; CanonicalPath = $canonicalCurrentIterationLogCandidate; NextStep = 'Retain godot.log.current-iteration beside run-result.json before using AutoSlay log lines for owner routing.' },
            [pscustomobject]@{ Label = 'runtime-probe-samples.json'; FieldName = 'RuntimeProbeSamplesPath'; HashField = 'RuntimeProbeSamplesSha256'; Signal = 'autoslay_runtime_probe_samples_outside_run_dir'; MissingSignal = 'autoslay_runtime_probe_samples_path_missing'; MissingHashSignal = 'autoslay_runtime_probe_samples_hash_missing'; HashMismatchSignal = 'autoslay_runtime_probe_samples_hash_mismatch'; NonCanonicalSignal = 'autoslay_runtime_probe_samples_not_retained_file'; Path = $probeSamplesCandidate; CanonicalPath = $canonicalProbeSamplesCandidate; NextStep = 'Retain runtime-probe-samples.json beside run-result.json before using AutoSlay probe telemetry for triage.' },
            [pscustomobject]@{ Label = 'godot-log-audit.json'; FieldName = 'GodotLogAuditPath'; Signal = 'autoslay_godot_log_audit_outside_run_dir'; MissingSignal = 'autoslay_godot_log_audit_path_missing'; NonCanonicalSignal = 'autoslay_godot_log_audit_not_retained_file'; Path = $auditCandidate; CanonicalPath = Join-Path $Directory 'godot-log-audit.json'; NextStep = 'Retain godot-log-audit.json beside run-result.json before using audit signatures for owner routing.' },
            [pscustomobject]@{ Label = 'sts1-mode-log-check.json'; FieldName = 'Sts1ModeLogCheckPath'; Signal = 'autoslay_sts1_mode_log_check_outside_run_dir'; MissingSignal = 'autoslay_sts1_mode_log_check_path_missing'; NonCanonicalSignal = 'autoslay_sts1_mode_log_check_not_retained_file'; Path = $sts1ModeCandidate; CanonicalPath = Join-Path $Directory 'sts1-mode-log-check.json'; NextStep = 'Retain sts1-mode-log-check.json beside run-result.json before using StS1 mode evidence for owner routing.' },
            [pscustomobject]@{ Label = 'autoslay.log'; FieldName = 'AutoSlayLogPath'; Signal = 'autoslay_sidecar_log_outside_run_dir'; MissingSignal = 'autoslay_sidecar_log_path_missing'; NonCanonicalSignal = 'autoslay_sidecar_log_not_retained_file'; Path = $autoSlayLogCandidate; CanonicalPath = Join-Path $Directory 'autoslay.log'; NextStep = 'Retain autoslay.log beside run-result.json before using sidecar log lines for owner routing.' }
        )

        foreach ($artifact in $autoSlayRequiredArtifacts) {
            $artifactPath = [string]$artifact.Path
            $artifactFieldRetained = Test-JsonProperty -Object $result -Name ([string]$artifact.FieldName)
            $artifactHashField = if (Test-JsonProperty -Object $artifact -Name 'HashField') { [string]$artifact.HashField } else { '' }
            $artifactIsRuntimeProbeSamples = [string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)
            if (-not $artifactFieldRetained) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayProbeArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'godot-log-audit.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'sts1-mode-log-check.json', [System.StringComparison]::Ordinal)) {
                    $autoSlaySts1ModeArtifactTrustedForOwner = $false
                    $sts1ModeLogCheckTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.MissingSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json did not retain $($artifact.FieldName) for $($artifact.Label)." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }

            if ([string]::IsNullOrWhiteSpace($artifactPath)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayProbeArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'godot-log-audit.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'sts1-mode-log-check.json', [System.StringComparison]::Ordinal)) {
                    $autoSlaySts1ModeArtifactTrustedForOwner = $false
                    $sts1ModeLogCheckTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.MissingSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $($artifact.FieldName) was empty, blank, or malformed for $($artifact.Label)." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }

            $artifactExists = $false
            try {
                $artifactExists = Test-Path -LiteralPath $artifactPath -PathType Leaf
            } catch {
                $artifactExists = $false
            }

            if (-not $artifactExists) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayProbeArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'godot-log-audit.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'sts1-mode-log-check.json', [System.StringComparison]::Ordinal)) {
                    $autoSlaySts1ModeArtifactTrustedForOwner = $false
                    $sts1ModeLogCheckTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.MissingSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $($artifact.FieldName) did not point to a retained $($artifact.Label) file on disk." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }

            if (-not (Test-PathInsideDirectory -Path $artifactPath -Directory $Directory)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayProbeArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'godot-log-audit.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'sts1-mode-log-check.json', [System.StringComparison]::Ordinal)) {
                    $autoSlaySts1ModeArtifactTrustedForOwner = $false
                    $sts1ModeLogCheckTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.Signal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $($artifact.Label) resolved outside the per-seed run evidence directory." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }

            $artifactFullPath = ConvertTo-NormalizedPathOrEmpty -Path $artifactPath
            $canonicalFullPath = ConvertTo-NormalizedPathOrEmpty -Path ([string]$artifact.CanonicalPath)
            $artifactMatchesCanonical = -not [string]::IsNullOrWhiteSpace($artifactFullPath) -and
                -not [string]::IsNullOrWhiteSpace($canonicalFullPath) -and
                [System.StringComparer]::OrdinalIgnoreCase.Equals($artifactFullPath, $canonicalFullPath)
            if (-not $artifactMatchesCanonical) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayProbeArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'godot-log-audit.json', [System.StringComparison]::Ordinal)) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'sts1-mode-log-check.json', [System.StringComparison]::Ordinal)) {
                    $autoSlaySts1ModeArtifactTrustedForOwner = $false
                    $sts1ModeLogCheckTrustedForOwner = $false
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.NonCanonicalSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $($artifact.Label) did not resolve to the retained standard per-seed file." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
            }

            if (-not [string]::IsNullOrWhiteSpace($artifactHashField)) {
                $recordedArtifactSha256 = [string](Get-JsonValue -Object $result -Name $artifactHashField -DefaultValue '')
                if (-not (Test-JsonProperty -Object $result -Name $artifactHashField) -or -not (Test-Sha256Text -Value $recordedArtifactSha256)) {
                    $autoSlayRunArtifactsTrustedForOwner = $false
                    if ($artifactIsRuntimeProbeSamples) {
                        $autoSlayProbeArtifactTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal ([string]$artifact.MissingHashSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json did not retain a valid $artifactHashField for $($artifact.Label)." -NextStep 'Record SHA256 bindings for retained AutoSlay artifacts before trusting probe telemetry or gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    continue
                }

                if ($artifactIsRuntimeProbeSamples -and $null -ne $SummaryResult) {
                    $summaryArtifactSha256 = [string](Get-JsonValue -Object $SummaryResult -Name $artifactHashField -DefaultValue '')
                    if (-not (Test-JsonProperty -Object $SummaryResult -Name $artifactHashField) -or -not (Test-Sha256Text -Value $summaryArtifactSha256)) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false

                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_samples_summary_hash_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay autoslay-summary.json did not retain a valid $artifactHashField for $($artifact.Label)." -NextStep 'Record RuntimeProbeSamplesSha256 in both run-result.json and autoslay-summary.json before trusting AutoSlay probe telemetry.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                        continue
                    }

                    if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($summaryArtifactSha256, $recordedArtifactSha256)) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false

                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_samples_summary_hash_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay autoslay-summary.json $artifactHashField does not match run-result.json; summary=$summaryArtifactSha256 result=$recordedArtifactSha256." -NextStep 'Regenerate or reject the packet; summary and run-result probe hashes must bind to the same retained runtime-probe-samples.json.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                        continue
                    }
                }

                $actualArtifactSha256 = Get-FileSha256OrEmpty -Path $artifactPath
                if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($recordedArtifactSha256, $actualArtifactSha256)) {
                    $autoSlayRunArtifactsTrustedForOwner = $false
                    if ($artifactIsRuntimeProbeSamples) {
                        $autoSlayProbeArtifactTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal ([string]$artifact.HashMismatchSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $artifactHashField does not match retained $($artifact.Label); recorded=$recordedArtifactSha256 actual=$actualArtifactSha256." -NextStep 'Regenerate or reject the packet; do not route ownership from AutoSlay probe telemetry whose retained hash has drifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
        }
    }
    $currentIterationLogExists = -not [string]::IsNullOrWhiteSpace($currentIterationLogCandidate) -and (Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf)
    $beforeLogExists = -not [string]::IsNullOrWhiteSpace($beforeLogCandidate) -and (Test-Path -LiteralPath $beforeLogCandidate -PathType Leaf)
    $fullLogExists = -not [string]::IsNullOrWhiteSpace($fullLogCandidate) -and (Test-Path -LiteralPath $fullLogCandidate -PathType Leaf)
    $autoSlayLogExists = -not [string]::IsNullOrWhiteSpace($autoSlayLogCandidate) -and (Test-Path -LiteralPath $autoSlayLogCandidate -PathType Leaf)
    $autoSlayLogText = if ($autoSlayLogExists) { Get-Content -LiteralPath $autoSlayLogCandidate -Raw -Encoding UTF8 } else { '' }
    $autoSlaySidecarTrustedForOwner = -not $isGameNativeAutoSlay
    if ($isGameNativeAutoSlay) {
        $autoSlaySidecarTrustedForOwner = $autoSlayLogExists -and $autoSlaySidecarPathTrustedForOwner
        if ($autoSlayLogExists -and $autoSlaySidecarPathTrustedForOwner) {
            $recordedAutoSlayLogSha256 = if ($result) { [string](Get-JsonValue -Object $result -Name 'AutoSlayLogSha256' -DefaultValue '') } else { '' }
            if ([string]::IsNullOrWhiteSpace($recordedAutoSlayLogSha256)) {
                $autoSlaySidecarTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'autoslay_sidecar_log_hash_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay run-result.json did not retain AutoSlayLogSha256 for the sidecar log.' -NextStep 'Record AutoSlayLogSha256 in run-result.json before using sidecar lines for owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } else {
                $actualAutoSlayLogSha256 = Get-FileSha256OrEmpty -Path $autoSlayLogCandidate
                if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($recordedAutoSlayLogSha256, $actualAutoSlayLogSha256)) {
                    $autoSlaySidecarTrustedForOwner = $false
                    Add-Finding -Findings $findings -Signal 'autoslay_sidecar_log_hash_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay AutoSlayLogSha256 does not match the retained sidecar log; recorded=$recordedAutoSlayLogSha256 actual=$actualAutoSlayLogSha256." -NextStep 'Regenerate or reject the packet; do not route ownership from sidecar log text whose retained hash has drifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
        }
    }
    $logText = ''
    $logTextTrustedForOwner = $false
    if ($result -and $currentIterationLogExists) {
        if (-not ($beforeLogExists -and $fullLogExists)) {
            Add-Finding `
                -Findings $findings `
                -Signal 'current_iteration_log_before_after_binding_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'Evidence has godot.log.current-iteration without both godot.log.before and godot.log.after-launch, so the retained current slice may be stale or hand-assembled.' `
                -NextStep 'Fix before/after/current log retention or rerun the packet after validation lanes are unpaused; do not route ownership from an unbound current-iteration slice.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            $sliceBinding = Test-CurrentSliceFromBeforeAfter -BeforePath $beforeLogCandidate -AfterPath $fullLogCandidate -CurrentPath $currentIterationLogCandidate
            $logText = [System.IO.File]::ReadAllText($currentIterationLogCandidate)
            $offsetMatchesBeforeLength = $isGameNativeAutoSlay -or $isDirectSmoke
            if (-not $isGameNativeAutoSlay -and -not $isDirectSmoke -and -not (Test-JsonProperty -Object $result -Name 'LogScanOffsetBytes')) {
                Add-Finding `
                    -Findings $findings `
                    -Signal 'current_iteration_log_offset_binding_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'godot.log.current-iteration exists without LogScanOffsetBytes, so the retained current slice may be stale or hand-assembled.' `
                    -NextStep 'Fix current-iteration log offset binding or rerun the packet after validation lanes are unpaused; do not route ownership from an unbound current-iteration slice.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            } elseif (-not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
                $logScanOffset = Get-JsonLongValue -Object $result -Name 'LogScanOffsetBytes' -DefaultValue -1
                $beforeLogLength = [long](Get-Item -LiteralPath $beforeLogCandidate).Length
                $fullLogLength = [long](Get-Item -LiteralPath $fullLogCandidate).Length
                $offsetMatchesBeforeLength = $logScanOffset -eq $beforeLogLength
                if ($logScanOffset -lt 0 -or $logScanOffset -gt $fullLogLength) {
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'current_iteration_log_scan_offset_invalid' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "LogScanOffsetBytes is outside godot.log.after-launch; offset=$logScanOffset, length=$fullLogLength." `
                        -NextStep 'Fix current-iteration log slicing or evidence retention before routing this runtime failure to gameplay source.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                } elseif (-not $offsetMatchesBeforeLength) {
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'current_iteration_log_scan_offset_before_length_mismatch' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "LogScanOffsetBytes must equal retained godot.log.before length; offset=$logScanOffset, beforeLength=$beforeLogLength." `
                        -NextStep 'Regenerate the packet with before/after/current log binding before using current-iteration logs for owner routing.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                }
            }

            $godotLogMetadataMatches = $true
            if ($isGameNativeAutoSlay -or (-not $isDirectSmoke)) {
                $runtimeMonkeyMetadataMissingSignal = 'runtime_monkey_godot_log_metadata_missing'
                $runtimeMonkeyMetadataMismatchSignal = 'runtime_monkey_godot_log_metadata_mismatch'
                $autoSlayMetadataMissingSignal = 'autoslay_godot_log_metadata_missing'
                $autoSlayMetadataMismatchSignal = 'autoslay_godot_log_metadata_mismatch'
                $metadataMissingSignal = if ($isGameNativeAutoSlay) { $autoSlayMetadataMissingSignal } else { $runtimeMonkeyMetadataMissingSignal }
                $metadataMismatchSignal = if ($isGameNativeAutoSlay) { $autoSlayMetadataMismatchSignal } else { $runtimeMonkeyMetadataMismatchSignal }
                $metadataRunnerLabel = if ($isGameNativeAutoSlay) { 'GameNativeAutoSlay' } else { 'Runtime monkey' }
                $missingLogMetadata = [System.Collections.Generic.List[string]]::new()
                $mismatchedLogMetadata = [System.Collections.Generic.List[string]]::new()
                $logMetadataChecks = @(
                    [pscustomobject]@{ Label = 'GodotLogBefore'; Path = $beforeLogCandidate; LengthField = 'GodotLogBeforeLengthBytes'; ShaField = 'GodotLogBeforeSha256' },
                    [pscustomobject]@{ Label = 'GodotLogAfterLaunch'; Path = $fullLogCandidate; LengthField = 'GodotLogAfterLaunchLengthBytes'; ShaField = 'GodotLogAfterLaunchSha256' },
                    [pscustomobject]@{ Label = 'GodotLogCurrentIteration'; Path = $currentIterationLogCandidate; LengthField = 'GodotLogCurrentIterationLengthBytes'; ShaField = 'GodotLogCurrentIterationSha256' }
                )

                foreach ($metadataCheck in $logMetadataChecks) {
                    $recordedLength = Get-JsonLongValue -Object $result -Name $metadataCheck.LengthField -DefaultValue -1
                    $recordedSha256 = [string](Get-JsonValue -Object $result -Name $metadataCheck.ShaField -DefaultValue '')
                    $metadataPath = [string]$metadataCheck.Path
                    $metadataPathExists = -not [string]::IsNullOrWhiteSpace($metadataPath) -and (Test-Path -LiteralPath $metadataPath -PathType Leaf)
                    if (-not (Test-JsonProperty -Object $result -Name $metadataCheck.LengthField) -or $recordedLength -lt 0) {
                        $missingLogMetadata.Add($metadataCheck.LengthField) | Out-Null
                    } elseif (-not $metadataPathExists) {
                        $mismatchedLogMetadata.Add("$($metadataCheck.LengthField): retained file missing") | Out-Null
                    } else {
                        $actualLength = [long](Get-Item -LiteralPath $metadataPath).Length
                        if ($recordedLength -ne $actualLength) {
                            $mismatchedLogMetadata.Add("$($metadataCheck.LengthField): recorded=$recordedLength actual=$actualLength") | Out-Null
                        }
                    }

                    if ([string]::IsNullOrWhiteSpace($recordedSha256)) {
                        $missingLogMetadata.Add($metadataCheck.ShaField) | Out-Null
                    } else {
                        $actualSha256 = if ($metadataPathExists) { Get-FileSha256OrEmpty -Path $metadataPath } else { '' }
                        if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($recordedSha256, $actualSha256)) {
                            $mismatchedLogMetadata.Add("$($metadataCheck.ShaField): recorded=$recordedSha256 actual=$actualSha256") | Out-Null
                        }
                    }
                }

                if ($missingLogMetadata.Count -gt 0) {
                    $godotLogMetadataMatches = $false
                    if (-not $isGameNativeAutoSlay) {
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal $metadataMissingSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "$metadataRunnerLabel result JSON is missing retained log metadata: $($missingLogMetadata -join ', ')." -NextStep 'Record before/after/current Godot log length and SHA256 fields before routing evidence to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }

                if ($mismatchedLogMetadata.Count -gt 0) {
                    $godotLogMetadataMatches = $false
                    if (-not $isGameNativeAutoSlay) {
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    }

                    Add-Finding -Findings $findings -Signal $metadataMismatchSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "$metadataRunnerLabel result JSON log metadata does not match retained files: $($mismatchedLogMetadata -join '; ')." -NextStep 'Regenerate or reject the packet; do not route ownership from log files whose retained byte metadata has drifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }

            $logTextTrustedForOwner = [bool]$sliceBinding.SliceMatches -and $offsetMatchesBeforeLength -and $godotLogMetadataMatches -and $autoSlayRunArtifactsTrustedForOwner -and $runtimeMonkeyRunArtifactsTrustedForOwner
            if (-not [bool]$sliceBinding.SliceMatches) {
                $nextStep = if ($isGameNativeAutoSlay) {
                    'Use only byte-bound current-iteration slices for AutoSlay source routing, then fix evidence retention before trusting packet evidence.'
                } else {
                    'Use only byte-bound current-iteration slices for source routing, then fix current-iteration log retention before trusting packet evidence.'
                }
                Add-Finding `
                    -Findings $findings `
                    -Signal 'current_iteration_log_slice_mismatch' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale $sliceBinding.Detail `
                    -NextStep $nextStep `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }
        }
    } elseif (-not [string]::IsNullOrWhiteSpace($logCandidate) -and (Test-Path -LiteralPath $logCandidate -PathType Leaf)) {
        $logText = Get-Content -LiteralPath $logCandidate -Raw -Encoding UTF8
    }

    if ($result -and -not $currentIterationLogExists -and $fullLogExists) {
        $logText = Get-Content -LiteralPath $fullLogCandidate -Raw -Encoding UTF8
    }

    if ($isGameNativeAutoSlay) {
        if ([string]::IsNullOrWhiteSpace($seed)) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_seed_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay run evidence must retain the exact seed before the run can be reproduced or triaged.' `
                -NextStep 'Fix AutoSlay run-result retention so each run-result.json and autoslay-summary.json row records Seed.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if (-not [string]::Equals($eventKind, 'Ancient', [System.StringComparison]::Ordinal)) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_event_kind_not_ancient' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale "GameNativeAutoSlay run evidence must record EventKind='Ancient'; found '$eventKind'." `
                -NextStep 'Retain EventKind from the game-native AutoSlay event-room handler before treating this packet as Ancient traversal evidence.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if ([string]::IsNullOrWhiteSpace($ancientId)) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_ancient_id_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay run evidence must retain the AncientId selected by the event-room handler.' `
                -NextStep 'Fix AutoSlay run-result and summary retention so every Ancient event run records the concrete AncientId.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if (-not (Test-OrderedTextSequence -Text $invocation -Needles @('AutoSlayer.Start(seed, logFile)'))) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_invocation_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay run evidence did not retain the launcher or mod-hook invocation that calls AutoSlayer.Start(seed, logFile).' `
                -NextStep 'Retain the exact launcher/mod-hook invocation before treating this packet as game-native AutoSlay evidence.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        $startTimestampText = if ($result) { [string](Get-JsonValue -Object $result -Name 'StartTimestamp' -DefaultValue '') } else { '' }
        $endTimestampText = if ($result) { [string](Get-JsonValue -Object $result -Name 'EndTimestamp' -DefaultValue '') } else { '' }
        $startTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $startTimestampText
        $endTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $endTimestampText
        if (-not [bool]$startTimestampParse.Parsed) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_start_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json must retain a parseable StartTimestamp; found '$startTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp retention before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if (-not [bool]$endTimestampParse.Parsed) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_end_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json must retain a parseable EndTimestamp; found '$endTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp retention before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if ([bool]$startTimestampParse.Parsed -and [bool]$endTimestampParse.Parsed -and $startTimestampParse.Value -gt $endTimestampParse.Value) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_timestamp_order_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json has StartTimestamp later than EndTimestamp; start='$startTimestampText' end='$endTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp capture before using duration or ownership routing from this packet.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if ([string]::IsNullOrWhiteSpace($probeSamplesCandidate) -or -not (Test-Path -LiteralPath $probeSamplesCandidate -PathType Leaf)) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $autoSlayProbeArtifactTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_runtime_probe_samples_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay evidence did not retain runtime-probe-samples.json for this seed.' `
                -NextStep 'Fix AutoSlay process/window/log sampling retention before routing this packet to gameplay source.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } elseif ($isGameNativeAutoSlay -and -not $autoSlayProbeArtifactTrustedForOwner) {
            # The containment finding above is enough; do not classify probe health from a shared/root artifact.
        } else {
            try {
                $probeSamplesParsed = Get-Content -LiteralPath $probeSamplesCandidate -Raw -Encoding UTF8 | ConvertFrom-Json
                $probeSamples = @($probeSamplesParsed)
                $requiredProbeFields = @(
                    'Phase',
                    'SampledAt',
                    'LogExists',
                    'LogLengthBytes',
                    'ProcessId',
                    'ProcessObserved',
                    'MainWindowObserved',
                    'HungWindow',
                    'Responding',
                    'ProcessStartTimeUtc',
                    'ProcessPath',
                    'ExpectedGameProcessId',
                    'ExpectedGameProcessStartTimeUtc',
                    'ExpectedGameProcessPath',
                    'ProcessIdMatchesExpected',
                    'ProcessStartTimeMatchesExpected',
                    'ProcessPathMatchesExpected',
                    'ProcessIdentityMatchesExpected',
                    'StaleProcessCount',
                    'CurrentProcessCount',
                    'UnknownStartTimeProcessCount',
                    'AmbiguousCurrentProcessCount')
                $requiredRetainedProbeFields = @(
                    'LogLastWriteTimeUtc')

                if ($probeSamples.Count -eq 0) {
                    $autoSlayRunArtifactsTrustedForOwner = $false
                    $autoSlayProbeArtifactTrustedForOwner = $false
                    $logTextTrustedForOwner = $false
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'autoslay_runtime_probe_samples_empty' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale 'GameNativeAutoSlay runtime-probe-samples.json has no process/window/log samples.' `
                        -NextStep 'Retain the sampled process/window/log timeline before classifying gameplay source.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                } elseif (@($requiredProbeFields | Where-Object { -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name $_) }).Count -gt 0 -or
                    @($requiredRetainedProbeFields | Where-Object { -not (Test-AllJsonPropertiesRetained -Items $probeSamples -Name $_) }).Count -gt 0) {
                    $missingProbeFields = @(
                        @($requiredProbeFields | Where-Object {
                            -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name $_)
                        })
                        @($requiredRetainedProbeFields | Where-Object {
                            -not (Test-AllJsonPropertiesRetained -Items $probeSamples -Name $_)
                        })
                    )
                    $autoSlayRunArtifactsTrustedForOwner = $false
                    $autoSlayProbeArtifactTrustedForOwner = $false
                    $logTextTrustedForOwner = $false
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'autoslay_runtime_probe_samples_incomplete' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "GameNativeAutoSlay runtime-probe-samples.json is missing required fields: $($missingProbeFields -join ', ')." `
                        -NextStep 'Record Phase, SampledAt, LogExists, LogLengthBytes, LogLastWriteTimeUtc, ProcessId, ProcessObserved, MainWindowObserved, HungWindow, Responding, StaleProcessCount, CurrentProcessCount, UnknownStartTimeProcessCount, and AmbiguousCurrentProcessCount for every probe sample.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                } else {
                    $invalidTimestampProbeSamples = @($probeSamples | Where-Object {
                        $sampledAtParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'SampledAt' -DefaultValue ''))
                        $logExists = (Get-JsonBoolValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        $logLastWriteParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'LogLastWriteTimeUtc' -DefaultValue ''))
                        (-not [bool]$sampledAtParse.Parsed) -or
                            ($logExists -and -not [bool]$logLastWriteParse.Parsed) -or
                            ($logExists -and [bool]$sampledAtParse.Parsed -and [bool]$logLastWriteParse.Parsed -and $logLastWriteParse.Value -gt $sampledAtParse.Value)
                    })
                    if ($invalidTimestampProbeSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay runtime-probe-samples.json has invalid probe timestamps; invalidCount=$($invalidTimestampProbeSamples.Count)." -NextStep 'Regenerate runtime-probe-samples.json with parseable SampledAt values, parseable LogLastWriteTimeUtc values when LogExists=true, and no log write time later than the sample timestamp.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyStringEquals -Items $probeSamples -Name 'Phase' -Value 'main-menu')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_main_menu_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never retained a main-menu phase sample.' -NextStep 'Fix AutoSlay probe sampling so startup and runtime phases are both represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyStringEquals -Items $probeSamples -Name 'Phase' -Value 'runtime')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_runtime_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never retained a runtime phase sample.' -NextStep 'Fix AutoSlay probe sampling so startup and runtime phases are both represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'ProcessObserved')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never observed the SlayTheSpire2 process.' -NextStep 'Fix process selection before routing this AutoSlay packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'MainWindowObserved')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_main_window_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never observed the main game window.' -NextStep 'Fix process/window binding before treating the packet as runtime gameplay proof.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-NoJsonPropertyTrue -Items $probeSamples -Name 'HungWindow')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_hung_window' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples report a hung game window.' -NextStep 'Inspect the retained runtime probe timeline and current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-NoJsonPropertyFalse -Items $probeSamples -Name 'Responding')) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_not_responding' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples include Responding=false.' -NextStep 'Inspect the retained runtime probe timeline and current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $staleProcessSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
                    if ($staleProcessSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_stale_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw stale SlayTheSpire2 processes, so shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing clients and recapture the packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $unknownStartTimeSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
                    if ($unknownStartTimeSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_unknown_start_time_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw SlayTheSpire2 processes with unreadable StartTime, so current-run attribution is ambiguous.' -NextStep 'Recapture with no unreadable SlayTheSpire2 processes before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $ambiguousCurrentProcessSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
                    if ($ambiguousCurrentProcessSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_ambiguous_current_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw multiple current SlayTheSpire2 processes, so shared log and PID evidence are ambiguous.' -NextStep 'Close overlapping clients and recapture the AutoSlay packet after the validation pause is lifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $currentProcessCountSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
                    if ($currentProcessCountSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_current_process_count_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples did not consistently bind to exactly one current SlayTheSpire2 process.' -NextStep 'Fix process selection and contamination rejection before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $observedProcessIds = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonIntValue -Object $_ -Name 'ProcessId' -DefaultValue 0 } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProcessStartTimes = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProcessPaths = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedExpectedProcessIds = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonIntValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0 } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedExpectedProcessStartTimes = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedExpectedProcessPaths = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $identityMismatchProbeSamples = @($probeSamples | Where-Object {
                        (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                        (-not (Get-JsonBoolValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
                    })
                    if ($observedProcessIds.Count -ne 1) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_identity_unstable' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime probe samples must bind to exactly one positive process id; observed count=$($observedProcessIds.Count)." -NextStep 'Fix AutoSlay process selection and stale-process rejection before trusting this packet.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    } elseif ($result) {
                        $resultProcessId = Get-JsonIntValue -Object $result -Name 'ProcessId' -DefaultValue 0
                        $resultProcessStartTimeParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $result -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                        $resultProcessStartTime = if ([bool]$resultProcessStartTimeParse.Parsed) { $resultProcessStartTimeParse.Value.ToString('o') } else { '' }
                        $resultProcessPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $result -Name 'ProcessPath' -DefaultValue ''))
                        $identityDefects = [System.Collections.Generic.List[string]]::new()
                        if ($resultProcessId -le 0) { $identityDefects.Add('run-result ProcessId missing') | Out-Null }
                        if (-not [bool]$resultProcessStartTimeParse.Parsed) { $identityDefects.Add('run-result ProcessStartTimeUtc missing or invalid') | Out-Null }
                        if ([string]::IsNullOrWhiteSpace($resultProcessPath)) { $identityDefects.Add('run-result ProcessPath missing') | Out-Null }
                        if ($observedProcessIds.Count -ne 1 -or $observedProcessIds[0] -ne $resultProcessId) { $identityDefects.Add("probe ProcessId values=$($observedProcessIds -join ',') result=$resultProcessId") | Out-Null }
                        if ($observedProcessStartTimes.Count -ne 1 -or -not [string]::Equals([string]$observedProcessStartTimes[0], $resultProcessStartTime, [System.StringComparison]::Ordinal)) { $identityDefects.Add("probe ProcessStartTimeUtc values=$($observedProcessStartTimes -join ',') result=$resultProcessStartTime") | Out-Null }
                        if ($observedProcessPaths.Count -ne 1 -or -not [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$observedProcessPaths[0], $resultProcessPath)) { $identityDefects.Add("probe ProcessPath values=$($observedProcessPaths -join ',') result=$resultProcessPath") | Out-Null }
                        if ($observedExpectedProcessIds.Count -ne 1 -or $observedExpectedProcessIds[0] -ne $resultProcessId) { $identityDefects.Add("probe ExpectedGameProcessId values=$($observedExpectedProcessIds -join ',') result=$resultProcessId") | Out-Null }
                        if ($observedExpectedProcessStartTimes.Count -ne 1 -or -not [string]::Equals([string]$observedExpectedProcessStartTimes[0], $resultProcessStartTime, [System.StringComparison]::Ordinal)) { $identityDefects.Add("probe ExpectedGameProcessStartTimeUtc values=$($observedExpectedProcessStartTimes -join ',') result=$resultProcessStartTime") | Out-Null }
                        if ($observedExpectedProcessPaths.Count -ne 1 -or -not [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$observedExpectedProcessPaths[0], $resultProcessPath)) { $identityDefects.Add("probe ExpectedGameProcessPath values=$($observedExpectedProcessPaths -join ',') result=$resultProcessPath") | Out-Null }
                        if ($identityMismatchProbeSamples.Count -gt 0) { $identityDefects.Add("ProcessIdentityMatchesExpected false count=$($identityMismatchProbeSamples.Count)") | Out-Null }
                        if ($identityDefects.Count -gt 0) {
                            $autoSlayRunArtifactsTrustedForOwner = $false
                            $autoSlayProbeArtifactTrustedForOwner = $false
                            $logTextTrustedForOwner = $false
                            Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_identity_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay runtime-probe-samples.json does not bind to run-result.json process identity: $($identityDefects -join '; ')." -NextStep 'Regenerate the AutoSlay packet with probe samples from the launched game process before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                        }
                    }

                    $probeRuntimeObservation = if ($result) { Get-JsonValue -Object $result -Name 'RuntimeObservation' -DefaultValue $null } else { $null }
                    $runtimeObservationLogGrew = $null -ne $probeRuntimeObservation -and (Get-JsonBoolValue -Object $probeRuntimeObservation -Name 'LogGrew' -DefaultValue $false)
                    $runtimeObservationInitialLogLength = if ($null -ne $probeRuntimeObservation) { Get-JsonLongValue -Object $probeRuntimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1 } else { -1L }
                    $runtimeObservationFinalLogLength = if ($null -ne $probeRuntimeObservation) { Get-JsonLongValue -Object $probeRuntimeObservation -Name 'LogFinalLengthBytes' -DefaultValue -1 } else { -1L }
                    $runtimeProbeLogLengths = @($probeSamples |
                        Where-Object {
                            [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'runtime', [System.StringComparison]::Ordinal) -and
                            (Get-JsonBoolValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        } |
                        ForEach-Object { Get-JsonLongValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1 } |
                        Where-Object { $_ -ge 0 })
                    $runtimeProbeMaxLogLength = if ($runtimeProbeLogLengths.Count -gt 0) {
                        [long](@($runtimeProbeLogLengths | Sort-Object -Descending)[0])
                    } else {
                        -1L
                    }
                    if ($runtimeObservationLogGrew -and
                        ($runtimeObservationInitialLogLength -lt 0 -or
                            $runtimeObservationFinalLogLength -le $runtimeObservationInitialLogLength -or
                            $runtimeProbeMaxLogLength -le $runtimeObservationInitialLogLength)) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_log_growth_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "RuntimeObservation.LogGrew=true is not backed by retained runtime sample LogLengthBytes; initial=$runtimeObservationInitialLogLength final=$runtimeObservationFinalLogLength maxRuntimeSample=$runtimeProbeMaxLogLength." -NextStep 'Regenerate the AutoSlay packet with runtime probe samples whose log-length timeline proves the runtime log growth claim.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }
                }
            } catch {
                $autoSlayRunArtifactsTrustedForOwner = $false
                $autoSlayProbeArtifactTrustedForOwner = $false
                $logTextTrustedForOwner = $false
                Add-Finding `
                    -Findings $findings `
                    -Signal 'autoslay_runtime_probe_samples_invalid' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale "GameNativeAutoSlay runtime-probe-samples.json could not be parsed or classified: $($_.Exception.Message)" `
                    -NextStep 'Regenerate runtime-probe-samples.json from structured probe telemetry before classifying gameplay source.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }
        }

        $mainMenuObservation = if ($result) { Get-JsonValue -Object $result -Name 'MainMenuObservation' -DefaultValue $null } else { $null }
        $mainMenuObservationFailures = @(Get-UnhealthyObservationFields `
            -Observation $mainMenuObservation `
            -RequiredTrueFields @('Passed', 'MainMenuReached', 'ProcessObserved', 'LogObserved') `
            -RequiredFalseFields @('ProcessExitedAfterObservation', 'HungWindowDetected', 'StaleProcessObserved', 'NoLogGrowthTimeoutExceeded') `
            -ZeroCountField 'MaxStaleProcessCount')
        if ($mainMenuObservationFailures.Count -gt 0) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $autoSlayProbeArtifactTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            $mainMenuSignal = if ($mainMenuObservationFailures -contains 'missing') { 'autoslay_main_menu_observation_missing' } else { 'autoslay_main_menu_observation_unhealthy' }
            Add-Finding -Findings $findings -Signal $mainMenuSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay MainMenuObservation is not clean: $($mainMenuObservationFailures -join ', ')." -NextStep 'Fix main-menu process/window/log observation before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        $runtimeObservation = if ($result) { Get-JsonValue -Object $result -Name 'RuntimeObservation' -DefaultValue $null } else { $null }
        $runtimeObservationFailures = @(Get-UnhealthyObservationFields `
            -Observation $runtimeObservation `
            -RequiredTrueFields @('Passed', 'ProcessObserved', 'LogObserved', 'LogGrew') `
            -RequiredFalseFields @('ProcessExitedAfterObservation', 'HungWindowDetected', 'StaleProcessObserved', 'NoLogGrowthTimeoutExceeded') `
            -ZeroCountField 'MaxStaleProcessCount')
        if ($runtimeObservationFailures.Count -gt 0) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $autoSlayProbeArtifactTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            $runtimeSignal = if ($runtimeObservationFailures -contains 'missing') { 'autoslay_runtime_observation_missing' } else { 'autoslay_runtime_observation_unhealthy' }
            Add-Finding -Findings $findings -Signal $runtimeSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay RuntimeObservation is not clean: $($runtimeObservationFailures -join ', ')." -NextStep 'Fix runtime process/window/log observation before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if (-not $autoSlayLogExists) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $autoSlaySidecarTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_sidecar_log_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay evidence did not retain the AutoSlay sidecar log for this seed.' `
                -NextStep 'Fix AutoSlay log retention before classifying gameplay source; the sidecar log is required to prove event-room traversal.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            $eventSequence = @(
                "Starting run with seed=$seed",
                'Entering Event room',
                'Detected Ancient event, clicking through dialogue',
                'Selecting event option:'
            )
            $completionMarker = "Run completed successfully with seed=$seed"
            $failureMarker = "Run failed with seed=$seed"

            if (-not [string]::IsNullOrWhiteSpace($seed) -and -not (Test-OrderedTextSequence -Text $autoSlayLogText -Needles $eventSequence)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                $autoSlaySidecarTrustedForOwner = $false
                $logTextTrustedForOwner = $false
                Add-Finding `
                    -Findings $findings `
                    -Signal 'autoslay_sidecar_event_sequence_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'The AutoSlay sidecar log does not contain the ordered seed start, event-room entry, Ancient dialogue, and option-selection markers.' `
                    -NextStep 'Rerun with a seed/launcher path that reaches an Ancient event room, or fix AutoSlay event-room logging before using this packet as gameplay evidence.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }

            if (-not [string]::IsNullOrWhiteSpace($ancientId) -and -not (Test-TextContains -Text $autoSlayLogText -Needle $ancientId)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                $autoSlaySidecarTrustedForOwner = $false
                $logTextTrustedForOwner = $false
                Add-Finding `
                    -Findings $findings `
                    -Signal 'autoslay_sidecar_ancient_id_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale "The AutoSlay sidecar log does not contain AncientId '$ancientId'." `
                    -NextStep 'Fix AutoSlay event-room logging or rerun with a retained sidecar log that names the Ancient event actually traversed.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }

            if (-not [string]::IsNullOrWhiteSpace($seed) -and -not (Test-TextContains -Text $autoSlayLogText -Needle $completionMarker) -and -not (Test-TextContains -Text $autoSlayLogText -Needle $failureMarker)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                $autoSlaySidecarTrustedForOwner = $false
                $logTextTrustedForOwner = $false
                Add-Finding `
                    -Findings $findings `
                    -Signal 'autoslay_completion_or_failure_marker_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'The AutoSlay sidecar log has no completion or failure marker for the retained seed.' `
                    -NextStep 'Fix AutoSlay termination logging before using the sidecar log to classify this run.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }

            if (-not [string]::IsNullOrWhiteSpace($seed) -and (Test-TextContains -Text $autoSlayLogText -Needle $failureMarker)) {
                $autoSlayRunArtifactsTrustedForOwner = $false
                $autoSlaySidecarTrustedForOwner = $false
                $logTextTrustedForOwner = $false
                Add-Finding `
                    -Findings $findings `
                    -Signal 'autoslay_run_failed_marker' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'The AutoSlay sidecar log explicitly records a failed run for the retained seed.' `
                    -NextStep 'Inspect the trusted current-iteration log and sidecar lines around the failure marker, then reroute to gameplay source only after the packet bindings are clean.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }
        }

        if ($logTextTrustedForOwner -and -not [string]::IsNullOrWhiteSpace($seed) -and -not (Test-OrderedTextSequence -Text $logText -Needles @("Starting run with seed=$seed", 'Entering Event room', 'Detected Ancient event, clicking through dialogue', 'Selecting event option:'))) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_current_log_event_sequence_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'The byte-bound current-iteration Godot log does not contain the ordered AutoSlay event traversal markers.' `
                -NextStep 'Fix Godot/current-slice logging or rerun the AutoSlay packet; do not use sidecar-only traversal as game-native proof.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if ($logTextTrustedForOwner -and -not [string]::IsNullOrWhiteSpace($ancientId) -and -not (Test-TextContains -Text $logText -Needle $ancientId)) {
            $autoSlayRunArtifactsTrustedForOwner = $false
            $logTextTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_current_log_ancient_id_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale "The byte-bound current-iteration Godot log does not contain AncientId '$ancientId'." `
                -NextStep 'Fix current-slice capture or AutoSlay event-room logging before treating the run as game-native Ancient traversal proof.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }
    }

    $isRuntimeMonkeyResult = -not $isGameNativeAutoSlay -and
        $result -and
        (Test-JsonProperty -Object $result -Name 'HangProbeSchemaVersion')
    $runtimeMonkeyProbeEvidenceInvalid = $false
    if ($isRuntimeMonkeyResult) {
        if ([string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $result -Name 'RuntimeProbeSamplesPath' -DefaultValue ''))) {
            $runtimeMonkeyProbeEvidenceInvalid = $true
            Add-Finding `
                -Findings $findings `
                -Signal 'runtime_monkey_probe_samples_path_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'Runtime monkey iteration-result.json did not retain RuntimeProbeSamplesPath, so process/window samples are not bound to the result artifact.' `
                -NextStep 'Fix RuntimeProbeSamplesPath retention and rerun the packet after validation lanes are unpaused.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if ([string]::IsNullOrWhiteSpace($probeSamplesCandidate) -or -not (Test-Path -LiteralPath $probeSamplesCandidate -PathType Leaf)) {
            $runtimeMonkeyProbeEvidenceInvalid = $true
            Add-Finding `
                -Findings $findings `
                -Signal 'runtime_monkey_probe_samples_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'Runtime monkey evidence did not retain runtime-probe-samples.json, so process/window sampling cannot be tied to the observation windows.' `
                -NextStep 'Fix runtime-probe-samples.json retention and rerun the packet after validation lanes are unpaused.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } elseif (-not $runtimeMonkeyProbeArtifactTrustedForOwner) {
            $runtimeMonkeyProbeEvidenceInvalid = $true
            # The containment finding above is enough; do not classify probe health from a shared/root artifact.
        } else {
            try {
                $probeSamplesParsed = Get-Content -LiteralPath $probeSamplesCandidate -Raw -Encoding UTF8 | ConvertFrom-Json
                $probeSamples = @($probeSamplesParsed)
                $requiredProbeFields = @(
                    'Phase',
                    'SampledAt',
                    'LogExists',
                    'LogLengthBytes',
                    'ProcessId',
                    'ProcessObserved',
                    'MainWindowObserved',
                    'HungWindow',
                    'Responding',
                    'StaleProcessCount',
                    'CurrentProcessCount',
                    'UnknownStartTimeProcessCount',
                    'AmbiguousCurrentProcessCount')
                $requiredRetainedProbeFields = @('LogLastWriteTimeUtc')

                if ($probeSamples.Count -eq 0) {
                    $runtimeMonkeyProbeEvidenceInvalid = $true
                    Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_samples_empty' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey runtime-probe-samples.json has no process/window/log samples.' -NextStep 'Retain the sampled process/window/log timeline before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } else {
                    $missingProbeFields = @(
                        @($requiredProbeFields | Where-Object {
                            -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name $_)
                        })
                        @($requiredRetainedProbeFields | Where-Object {
                            -not (Test-AllJsonPropertiesRetained -Items $probeSamples -Name $_)
                        })
                    )
                    if ($missingProbeFields.Count -gt 0) {
                    $runtimeMonkeyProbeEvidenceInvalid = $true
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'runtime_monkey_probe_samples_incomplete' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "Runtime monkey runtime-probe-samples.json is missing required fields: $($missingProbeFields -join ', ')." `
                        -NextStep 'Record Phase, timestamp, log telemetry, process identity, window state, responsiveness, and process-count fields for every runtime monkey probe sample.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                    } else {
                    $startupMainMenuProbeSamples = @($probeSamples | Where-Object {
                        [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'StartupMainMenu', [System.StringComparison]::Ordinal)
                    })
                    $postCommandRuntimeProbeSamples = @($probeSamples | Where-Object {
                        [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'PostCommandRuntime', [System.StringComparison]::Ordinal)
                    })
                    $unknownRuntimeProbePhaseSamples = @($probeSamples | Where-Object {
                        $phase = [string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue '')
                        -not ([string]::Equals($phase, 'StartupMainMenu', [System.StringComparison]::Ordinal) -or
                            [string]::Equals($phase, 'PostCommandRuntime', [System.StringComparison]::Ordinal))
                    })
                    $mainMenuObservation = Get-JsonValue -Object $result -Name 'MainMenuObservation' -DefaultValue $null
                    $runtimeObservation = Get-JsonValue -Object $result -Name 'RuntimeObservation' -DefaultValue $null
                    $mainMenuObservationSampleCount = if ($null -ne $mainMenuObservation) { Get-JsonIntValue -Object $mainMenuObservation -Name 'Samples' -DefaultValue -1 } else { -1 }
                    $runtimeObservationSampleCount = if ($null -ne $runtimeObservation) { Get-JsonIntValue -Object $runtimeObservation -Name 'Samples' -DefaultValue -1 } else { -1 }

                    if ($unknownRuntimeProbePhaseSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_unknown_phase' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey probe samples include phase values outside StartupMainMenu/PostCommandRuntime; unknownCount=$($unknownRuntimeProbePhaseSamples.Count)." -NextStep 'Fix runtime probe phase labeling before using the packet for owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if ($startupMainMenuProbeSamples.Count -eq 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_startup_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples never retained a StartupMainMenu sample.' -NextStep 'Fix main-menu probe sampling so startup and runtime windows are both represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if ($postCommandRuntimeProbeSamples.Count -eq 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_runtime_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples never retained a PostCommandRuntime sample.' -NextStep 'Fix runtime probe sampling so post-command or idle runtime health is represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if ($mainMenuObservationSampleCount -lt 0 -or $startupMainMenuProbeSamples.Count -ne $mainMenuObservationSampleCount) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_startup_sample_count_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "StartupMainMenu probe count does not match MainMenuObservation.Samples; expected=$mainMenuObservationSampleCount actual=$($startupMainMenuProbeSamples.Count)." -NextStep 'Regenerate the packet with retained startup probe samples that bind to MainMenuObservation.Samples.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if ($runtimeObservationSampleCount -lt 0 -or $postCommandRuntimeProbeSamples.Count -ne $runtimeObservationSampleCount) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_runtime_sample_count_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "PostCommandRuntime probe count does not match RuntimeObservation.Samples; expected=$runtimeObservationSampleCount actual=$($postCommandRuntimeProbeSamples.Count)." -NextStep 'Regenerate the packet with retained runtime probe samples that bind to RuntimeObservation.Samples.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $invalidTimestampProbeSamples = @($probeSamples | Where-Object {
                        $sampledAtParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'SampledAt' -DefaultValue ''))
                        $logExists = (Get-JsonBoolValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        $logLastWriteParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'LogLastWriteTimeUtc' -DefaultValue ''))
                        (-not [bool]$sampledAtParse.Parsed) -or
                            ($logExists -and -not [bool]$logLastWriteParse.Parsed) -or
                            ($logExists -and [bool]$sampledAtParse.Parsed -and [bool]$logLastWriteParse.Parsed -and $logLastWriteParse.Value -gt $sampledAtParse.Value)
                    })
                    if ($invalidTimestampProbeSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey runtime-probe-samples.json has invalid probe timestamps; invalidCount=$($invalidTimestampProbeSamples.Count)." -NextStep 'Regenerate runtime-probe-samples.json with parseable SampledAt values, parseable LogLastWriteTimeUtc values when LogExists=true, and no log write time later than the sample timestamp.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $staleProcessSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
                    if ($staleProcessSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_stale_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw stale SlayTheSpire2 processes, so shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing game clients and recapture the packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $unknownStartTimeSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
                    if ($unknownStartTimeSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_unknown_start_time_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw SlayTheSpire2 processes with unreadable StartTime, so current-run attribution is ambiguous.' -NextStep 'Recapture with no unreadable SlayTheSpire2 processes before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $ambiguousCurrentProcessSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
                    if ($ambiguousCurrentProcessSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_ambiguous_current_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw multiple current SlayTheSpire2 processes, so shared log and PID evidence are ambiguous.' -NextStep 'Close overlapping clients and recapture the runtime monkey packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $currentProcessCountSamples = @($probeSamples | Where-Object { (Get-JsonIntValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
                    if ($currentProcessCountSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_current_process_count_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples did not consistently bind to exactly one current SlayTheSpire2 process.' -NextStep 'Fix process selection and contamination rejection before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $resultGameProcessId = Get-JsonIntValue -Object $result -Name 'GameProcessId' -DefaultValue 0
                    $resultGameProcessStartParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $result -Name 'GameProcessStartTimeUtc' -DefaultValue ''))
                    $resultGameProcessStartTime = if ([bool]$resultGameProcessStartParse.Parsed) { $resultGameProcessStartParse.Value.ToString('o') } else { '' }
                    $resultGameProcessPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $result -Name 'GameProcessPath' -DefaultValue ''))
                    $resultLiveSessionSelectedGameProcessId = Get-JsonIntValue -Object $result -Name 'LiveSessionSelectedGameProcessId' -DefaultValue 0
                    $resultLiveSessionStartParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $result -Name 'LiveSessionSelectedGameProcessStartTimeUtc' -DefaultValue ''))
                    $resultLiveSessionStartTime = if ([bool]$resultLiveSessionStartParse.Parsed) { $resultLiveSessionStartParse.Value.ToString('o') } else { '' }
                    $resultLiveSessionPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $result -Name 'LiveSessionSelectedGameProcessPath' -DefaultValue ''))
                    $observedProbeProcessIds = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonIntValue -Object $_ -Name 'ProcessId' -DefaultValue 0 } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProbeStartTimes = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbePaths = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbeExpectedProcessIds = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonIntValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0 } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProbeExpectedStartTimes = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbeExpectedPaths = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $identityMismatchProbeSamples = @($probeSamples | Where-Object {
                        (Get-JsonBoolValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                        (-not (Get-JsonBoolValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                            -not (Get-JsonBoolValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
                    })
                    $identityDefects = [System.Collections.Generic.List[string]]::new()
                    if ($resultGameProcessId -le 0) { $identityDefects.Add('result GameProcessId missing') | Out-Null }
                    if ([string]::IsNullOrWhiteSpace($resultGameProcessStartTime)) { $identityDefects.Add('result GameProcessStartTimeUtc missing') | Out-Null }
                    if ([string]::IsNullOrWhiteSpace($resultGameProcessPath)) { $identityDefects.Add('result GameProcessPath missing') | Out-Null }
                    if ($observedProbeProcessIds.Count -ne 1 -or $observedProbeProcessIds[0] -ne $resultGameProcessId) { $identityDefects.Add("probe ProcessId values=$($observedProbeProcessIds -join ',') result=$resultGameProcessId") | Out-Null }
                    if ($observedProbeStartTimes.Count -ne 1 -or -not [string]::Equals([string]$observedProbeStartTimes[0], $resultGameProcessStartTime, [System.StringComparison]::Ordinal)) { $identityDefects.Add("probe ProcessStartTimeUtc values=$($observedProbeStartTimes -join ',') result=$resultGameProcessStartTime") | Out-Null }
                    if ($observedProbePaths.Count -ne 1 -or -not [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$observedProbePaths[0], $resultGameProcessPath)) { $identityDefects.Add("probe ProcessPath values=$($observedProbePaths -join ',') result=$resultGameProcessPath") | Out-Null }
                    if ($resultLiveSessionSelectedGameProcessId -gt 0 -and ($observedProbeExpectedProcessIds.Count -ne 1 -or $observedProbeExpectedProcessIds[0] -ne $resultLiveSessionSelectedGameProcessId)) { $identityDefects.Add("probe ExpectedGameProcessId values=$($observedProbeExpectedProcessIds -join ',') liveSession=$resultLiveSessionSelectedGameProcessId") | Out-Null }
                    if (-not [string]::IsNullOrWhiteSpace($resultLiveSessionStartTime) -and ($observedProbeExpectedStartTimes.Count -ne 1 -or -not [string]::Equals([string]$observedProbeExpectedStartTimes[0], $resultLiveSessionStartTime, [System.StringComparison]::Ordinal))) { $identityDefects.Add("probe ExpectedGameProcessStartTimeUtc values=$($observedProbeExpectedStartTimes -join ',') liveSession=$resultLiveSessionStartTime") | Out-Null }
                    if (-not [string]::IsNullOrWhiteSpace($resultLiveSessionPath) -and ($observedProbeExpectedPaths.Count -ne 1 -or -not [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$observedProbeExpectedPaths[0], $resultLiveSessionPath))) { $identityDefects.Add("probe ExpectedGameProcessPath values=$($observedProbeExpectedPaths -join ',') liveSession=$resultLiveSessionPath") | Out-Null }
                    if ($identityMismatchProbeSamples.Count -gt 0) { $identityDefects.Add("ProcessIdentityMatchesExpected false count=$($identityMismatchProbeSamples.Count)") | Out-Null }
                    if ($identityDefects.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_process_identity_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey runtime-probe-samples.json does not bind to iteration-result.json/live-session process identity: $($identityDefects -join '; ')." -NextStep 'Regenerate the packet with probe samples from the live-session-selected game process before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $runtimeObservationLogGrowthRequired = if ($null -ne $runtimeObservation) { (Get-JsonBoolValue -Object $runtimeObservation -Name 'RuntimeLogGrowthRequired' -DefaultValue $false) } else { $false }
                    $runtimeObservationLogGrew = if ($null -ne $runtimeObservation) { (Get-JsonBoolValue -Object $runtimeObservation -Name 'LogGrew' -DefaultValue $false) } else { $false }
                    $runtimeObservationInitialLogLength = if ($null -ne $runtimeObservation) { Get-JsonLongValue -Object $runtimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1 } else { -1L }
                    $postCommandRuntimeProbeLogLengths = @($postCommandRuntimeProbeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'LogExists' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonLongValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1 } |
                        Where-Object { $_ -ge 0 })
                    $probeSampleLogLengths = @($probeSamples |
                        Where-Object { (Get-JsonBoolValue -Object $_ -Name 'LogExists' -DefaultValue $false) } |
                        ForEach-Object { Get-JsonLongValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1 } |
                        Where-Object { $_ -ge 0 })
                    $postCommandRuntimeProbeMaxLogLength = if ($postCommandRuntimeProbeLogLengths.Count -gt 0) {
                        [long](@($postCommandRuntimeProbeLogLengths | Sort-Object -Descending)[0])
                    } else {
                        -1L
                    }
                    $probeSampleMaxLogLength = if ($probeSampleLogLengths.Count -gt 0) {
                        [long](@($probeSampleLogLengths | Sort-Object -Descending)[0])
                    } else {
                        -1L
                    }
                    if ($runtimeObservationLogGrowthRequired -and $runtimeObservationLogGrew -and
                        ($runtimeObservationInitialLogLength -lt 0 -or $postCommandRuntimeProbeMaxLogLength -le $runtimeObservationInitialLogLength)) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_runtime_log_growth_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "RuntimeObservation.LogGrew=true is not backed by retained PostCommandRuntime sample LogLengthBytes; initial=$runtimeObservationInitialLogLength maxRuntimeSample=$postCommandRuntimeProbeMaxLogLength." -NextStep 'Regenerate the packet with runtime probe samples whose log-length timeline proves the post-command log growth claim.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }
                    $recordedAfterLaunchLogLength = if ($result) { Get-JsonLongValue -Object $result -Name 'GodotLogAfterLaunchLengthBytes' -DefaultValue -1 } else { -1L }
                    $retainedAfterLaunchLogLength = if ($fullLogExists) { [long](Get-Item -LiteralPath $fullLogCandidate).Length } else { -1L }
                    $probeLengthExceedsRecordedAfterLaunch = $probeSampleMaxLogLength -ge 0 -and
                        $recordedAfterLaunchLogLength -ge 0 -and
                        $probeSampleMaxLogLength -gt $recordedAfterLaunchLogLength
                    $probeLengthExceedsRetainedAfterLaunch = $probeSampleMaxLogLength -ge 0 -and
                        $retainedAfterLaunchLogLength -ge 0 -and
                        $probeSampleMaxLogLength -gt $retainedAfterLaunchLogLength
                    if ($probeLengthExceedsRecordedAfterLaunch -or $probeLengthExceedsRetainedAfterLaunch) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_log_length_exceeds_retained_after_launch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey probe sample LogLengthBytes exceeds retained after-launch log bytes; recordedAfterLaunch=$recordedAfterLaunchLogLength retainedAfterLaunch=$retainedAfterLaunchLogLength maxProbeSample=$probeSampleMaxLogLength." -NextStep 'Regenerate or reject the packet; probe log-length telemetry must stay within the retained godot.log.after-launch byte ceiling before source ownership routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }
                    }
                }
            } catch {
                $runtimeMonkeyProbeEvidenceInvalid = $true
                Add-Finding `
                    -Findings $findings `
                    -Signal 'runtime_monkey_probe_samples_invalid' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale "Runtime monkey runtime-probe-samples.json could not be parsed or classified: $($_.Exception.Message)" `
                    -NextStep 'Regenerate runtime-probe-samples.json from structured probe telemetry before classifying gameplay source.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }
        }
    }

    if ($isRuntimeMonkeyResult -and $runtimeMonkeyProbeEvidenceInvalid) {
        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
        $logTextTrustedForOwner = $false
    }

    $ownerLogText = if ($logTextTrustedForOwner -and $isGameNativeAutoSlay -and $autoSlaySidecarTrustedForOwner) {
        "$logText`n$autoSlayLogText"
    } elseif ($logTextTrustedForOwner -and $isGameNativeAutoSlay) {
        $logText
    } elseif ($logTextTrustedForOwner) {
        $logText
    } else {
        ''
    }
    $logOwnerArea = Get-OwnerAreaFromText -Text $ownerLogText -Command ''
    $commandOwnerArea = Get-OwnerAreaFromText -Text '' -Command $command
    $baseLibPatchFailures = if ($logTextTrustedForOwner) {
        @(Get-BaseLibPatchFailureDetails -LogText $logText)
    } else {
        @()
    }

    $auditExists = -not [string]::IsNullOrWhiteSpace($auditCandidate) -and (Test-Path -LiteralPath $auditCandidate -PathType Leaf)
    $auditJsonValid = (-not $auditExists) -or (Test-JsonFileParses -Path $auditCandidate)
    $auditData = if ($auditExists -and $auditJsonValid) { Read-JsonOrNull -Path $auditCandidate } else { $null }
    $auditSummary = if ($null -ne $auditData) { ConvertTo-AuditSummary -Audit $auditData } else { $null }
    $auditTrustedForOwner = $false
    $auditHits = [System.Collections.Generic.List[object]]::new()
    $failureCodes = @(if ($result) { Get-JsonArrayValues -Object $result -Name 'FailureReasonCodes' } else { @() })
    $hangSignals = @(if ($result) { Get-JsonArrayValues -Object $result -Name 'HangSignals' } else { @() })

    if ($result -and -not $currentIterationLogExists -and -not (@($failureCodes) -contains 'current_iteration_log_missing')) {
        Add-Finding `
            -Findings $findings `
            -Signal 'current_iteration_log_missing' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale 'The launched run did not retain a current-iteration log slice, so full godot.log content cannot be trusted for owner routing.' `
            -NextStep 'Fix current-iteration log slicing or rerun the packet after validation lanes are unpaused; do not route ownership from the full log.' `
            -Confidence 'high' `
            -EvidenceFiles $evidenceFiles
    }

    if ($isDirectSmoke -and $result) {
        $directSmokePassed = (Get-JsonBoolValue -Object $result -Name 'Passed' -DefaultValue $false)
        $directSmokeAuditClean = (Get-JsonBoolValue -Object $result -Name 'AuditClean' -DefaultValue $false)
        $directSmokeModeVerifierMismatches = Get-JsonIntValue -Object $result -Name 'ModeVerifierMismatches' -DefaultValue 0
        $directSmokePacketVerifierMismatches = Get-JsonIntValue -Object $result -Name 'PacketVerifierMismatches' -DefaultValue 0
        $directSmokeFailedOrDirty = (-not $directSmokePassed) -or
            (-not $directSmokeAuditClean) -or
            $directSmokeModeVerifierMismatches -gt 0 -or
            $directSmokePacketVerifierMismatches -gt 0

        if ($directSmokeFailedOrDirty -and -not $currentIterationLogExists) {
            Add-Finding `
                -Findings $findings `
                -Signal 'direct_smoke_current_iteration_log_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'Failed DirectSmoke evidence did not retain godot.log.current-iteration, so the summary cannot be bound to the log slice that failed.' `
                -NextStep 'Retain godot.log.current-iteration for failed direct smokes before assigning package or gameplay ownership.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if ($directSmokeFailedOrDirty -and -not $auditExists) {
            Add-Finding `
                -Findings $findings `
                -Signal 'direct_smoke_godot_log_audit_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'Failed DirectSmoke evidence did not retain godot-log-audit.json, so audit dirtiness cannot be recomputed or routed safely.' `
                -NextStep 'Retain godot-log-audit.json generated from godot.log.current-iteration before using DirectSmoke evidence for package/runtime diagnosis.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }

        if ($directSmokeModeVerifierMismatches -gt 0 -or $directSmokePacketVerifierMismatches -gt 0) {
            Add-Finding `
                -Findings $findings `
                -Signal 'direct_smoke_verifier_mismatch' `
                -Severity 'blocking' `
                -OwnerArea 'PackageRuntimeDrift' `
                -Rationale "DirectSmoke verifier mismatch counts are nonzero; modeMismatches=$directSmokeModeVerifierMismatches packetMismatches=$directSmokePacketVerifierMismatches." `
                -NextStep 'Inspect the retained direct smoke verifier reports and package/runtime markers before treating the failure as gameplay source behavior.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }
    }

    if ($iterationResultMissing) {
        $missingResultRationale = if ($null -ne $SummaryResult) {
            "$ResultFileName is missing or could not be parsed. Summary JSON provided a fallback row for routing, but it is not the canonical per-run evidence artifact."
        } else {
            "$ResultFileName is missing or could not be parsed, and summary JSON did not provide a usable run result."
        }

        Add-Finding `
            -Findings $findings `
            -Signal 'iteration_result_missing_or_invalid' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale $missingResultRationale `
            -NextStep 'Fix evidence retention or rerun the packet after validation lanes are unpaused; do not classify gameplay behavior from an incomplete iteration/run packet.' `
            -Confidence 'high' `
            -EvidenceFiles @($resultPath, $logCandidate, $auditCandidate, $probeSamplesCandidate, $sts1ModeCandidate)
    }

    if ($auditExists -and -not $auditJsonValid) {
        if ($isGameNativeAutoSlay) {
            $autoSlayAuditArtifactTrustedForOwner = $false
        }

        Add-Finding `
            -Findings $findings `
            -Signal 'godot_log_audit_json_invalid' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale 'godot-log-audit.json is missing valid JSON, so audit signature evidence cannot be trusted.' `
            -NextStep 'Fix audit evidence retention or rerun the packet after validation lanes are unpaused; do not treat an invalid audit artifact as a clean runtime log.' `
            -Confidence 'high' `
            -EvidenceFiles $evidenceFiles
    }

    if ($auditExists -and $auditJsonValid) {
        if (-not $currentIterationLogExists) {
            if ($isGameNativeAutoSlay) {
                $autoSlayAuditArtifactTrustedForOwner = $false
            }

            Add-Finding `
                -Findings $findings `
                -Signal 'godot_log_audit_current_iteration_log_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'godot-log-audit.json exists without a retained godot.log.current-iteration slice, so audit hits may belong to stale or unrelated log content.' `
                -NextStep 'Regenerate the packet with current-iteration slicing before using audit signatures for owner routing.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } elseif ($null -eq $auditSummary) {
            if ($isGameNativeAutoSlay) {
                $autoSlayAuditArtifactTrustedForOwner = $false
            }

            Add-Finding `
                -Findings $findings `
                -Signal 'godot_log_audit_json_invalid' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'godot-log-audit.json parsed once but could not be converted to audit items, so audit signature evidence cannot be trusted.' `
                -NextStep 'Fix audit evidence retention or rerun the packet after validation lanes are unpaused.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            $expectedAuditPath = [System.IO.Path]::GetFullPath($currentIterationLogCandidate)
            $expectedAuditLength = [long](Get-Item -LiteralPath $currentIterationLogCandidate).Length
            $expectedAuditSha256 = Get-FileSha256OrEmpty -Path $currentIterationLogCandidate
            $auditItemPaths = @($auditSummary.ItemPaths)
            $auditItemLengths = @($auditSummary.ItemLengths)
            $auditItemSha256s = @($auditSummary.ItemSha256s)
            $auditMetadataMatches =
                $auditItemPaths.Count -eq 1 -and
                [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemPaths[0], $expectedAuditPath) -and
                $auditItemLengths.Count -eq 1 -and
                $auditItemLengths[0] -eq $expectedAuditLength -and
                $auditItemSha256s.Count -eq 1 -and
                [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], $expectedAuditSha256)

            if (-not $auditMetadataMatches) {
                if ($isGameNativeAutoSlay) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }

                Add-Finding `
                    -Findings $findings `
                    -Signal 'godot_log_audit_current_iteration_binding_mismatch' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'godot-log-audit.json Path, Length, or Sha256 does not bind to the retained godot.log.current-iteration slice.' `
                    -NextStep 'Use only the packet checker recomputed audit or rerun the packet; do not route ownership from stale audit JSON.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            } elseif (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
                if ($isGameNativeAutoSlay) {
                    $autoSlayAuditArtifactTrustedForOwner = $false
                }

                Add-Finding `
                    -Findings $findings `
                    -Signal 'godot_log_audit_recompute_script_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale "The analyzer could not find audit-godot-log.ps1 at $logAuditScript, so retained audit signatures cannot be recomputed." `
                    -NextStep 'Restore the canonical audit script before classifying runtime evidence.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            } else {
                try {
                    $recomputedAudit = Invoke-RecomputedAudit -LogPath $currentIterationLogCandidate
                    $recomputedAuditSummary = ConvertTo-AuditSummary -Audit $recomputedAudit
                    $recomputedAuditSha256s = @($recomputedAuditSummary.ItemSha256s)
                    $auditMatchesRecomputed =
                        $auditSummary.DirtyItems -eq $recomputedAuditSummary.DirtyItems -and
                        $auditSummary.SignatureHitCount -eq $recomputedAuditSummary.SignatureHitCount -and
                        $auditItemSha256s.Count -eq 1 -and
                        $recomputedAuditSha256s.Count -eq 1 -and
                        [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], [string]$recomputedAuditSha256s[0])

                    if ($auditMatchesRecomputed) {
                        if ($autoSlayRunArtifactsTrustedForOwner -and $autoSlayAuditArtifactTrustedForOwner -and $runtimeMonkeyRunArtifactsTrustedForOwner) {
                            $auditTrustedForOwner = $true
                            $auditHits.Clear()
                            foreach ($hit in @(Get-AuditHits -AuditItems @($recomputedAudit))) {
                                $auditHits.Add($hit) | Out-Null
                            }
                        }
                    } else {
                        if ($isGameNativeAutoSlay) {
                            $autoSlayAuditArtifactTrustedForOwner = $false
                        }

                        Add-Finding `
                            -Findings $findings `
                            -Signal 'godot_log_audit_recomputed_mismatch' `
                            -Severity 'blocking' `
                            -OwnerArea 'RuntimeHarness' `
                            -Rationale "Retained audit signature counts do not match a fresh audit of godot.log.current-iteration; retained dirty=$($auditSummary.DirtyItems), retained hits=$($auditSummary.SignatureHitCount), recomputed dirty=$($recomputedAuditSummary.DirtyItems), recomputed hits=$($recomputedAuditSummary.SignatureHitCount)." `
                            -NextStep 'Treat the retained audit JSON as stale or hand-edited; rerun the packet or regenerate the audit from the current-iteration log before owner routing.' `
                            -Confidence 'high' `
                            -EvidenceFiles $evidenceFiles
                        }
                } catch {
                    if ($isGameNativeAutoSlay) {
                        $autoSlayAuditArtifactTrustedForOwner = $false
                    }

                    Add-Finding `
                        -Findings $findings `
                        -Signal 'godot_log_audit_recompute_failed' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "The analyzer could not recompute godot-log audit from the retained current-iteration slice: $($_.Exception.Message)" `
                        -NextStep 'Fix the current-iteration log or audit script before using audit signatures for source routing.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                }
            }
        }
    }

    $retainedSignalItems = [System.Collections.Generic.List[object]]::new()
    foreach ($signalItem in @($hangSignals)) {
        $retainedSignalItems.Add($signalItem) | Out-Null
    }
    foreach ($signalItem in @($failureCodes)) {
        $retainedSignalItems.Add($signalItem) | Out-Null
    }
    $retainedSignals = @($retainedSignalItems.ToArray()) | Select-Object -Unique
    $addRetainedSignalFindings = {
        param([bool]$AutoSlayEvidenceInvalidForOwner)

        foreach ($signal in $retainedSignals) {
            if ([string]::IsNullOrWhiteSpace([string]$signal)) {
                continue
            }

            switch ([string]$signal) {
            'game_process_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained probe did not observe SlayTheSpire2 during the sampled window.' -NextStep 'Check live-session launch output, process samples, Steam propagation, and whether another client was already running.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_exited' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeCrash' -Rationale 'The game process disappeared after being observed.' -NextStep 'Inspect the tail of godot.log.after-launch, Windows crash artifacts if available, and package/API compatibility markers.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'main_menu_timeout' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeStartup' -Rationale 'The main-menu marker was not observed before timeout.' -NextStep 'Compare runtime-probe-samples.json against godot.log growth, then rerun a smaller live packet with screenshots after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'startup_log_stalled' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeStartup' -Rationale 'godot.log stopped growing before main menu.' -NextStep 'Inspect the last retained log lines and probe timestamps; check package/API drift before touching gameplay code.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'runtime_log_stalled' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'godot.log stopped growing during runtime observation.' -NextStep 'Inspect RuntimeObservation, runtime-probe-samples.json, and the current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'process_unresponsive' {
                if ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner -or $autoSlayEvidenceInvalidForOwner) {
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The window was reported hung or not responding, but retained run/probe evidence is missing, invalid, or not byte-bound to trusted files.' -NextStep 'Fix runtime artifact retention and probe/sample binding before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } else {
                    $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea $owner -Rationale 'The window was reported hung or not responding during observation.' -NextStep (Get-NextStepForOwner -OwnerArea $owner -Signal $signal) -Confidence 'medium' -EvidenceFiles $evidenceFiles
                }
            }
            'stale_process_observed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained probe observed a SlayTheSpire2 process that started before this iteration; shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing game clients, rerun the packet after validation lanes are unpaused, and do not route ownership from this iteration log.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_prepare_output_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The iteration did not retain the live-session prepare output needed to bind launcher setup to runtime evidence.' -NextStep 'Fix prepare-output.json retention and rerun the packet; do not route ownership from unbound runtime evidence.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_launch_metadata_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The iteration is missing Steam launch metadata, so launcher setup cannot be verified against runtime evidence.' -NextStep 'Fix live-session launch metadata retention before classifying gameplay behavior.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_pid_attribution_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The live-session packet predates or lacks selected-game PID attribution fields.' -NextStep 'Regenerate the packet with the current live-session helper so SlayTheSpire2 PID/start/path identity is retained.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_pid_attribution_failed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The live-session helper could not select exactly one newly launched SlayTheSpire2 process.' -NextStep 'Inspect prepare-output.json candidates, close stale game clients, and rerun after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_start_time_unbound' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The observed game process start time was not proven to occur at or after the live-session launch request.' -NextStep 'Fix process start-time retention and live-session binding before assigning source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_path_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The iteration did not retain the executable path for the observed SlayTheSpire2 process.' -NextStep 'Fix process path retention in runtime probes before classifying gameplay behavior.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_id_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The observed runtime process id does not match the live-session selected game process id.' -NextStep 'Treat the packet as contaminated or stale; inspect prepare-output.json and runtime-probe-samples.json before rerunning.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_start_time_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The observed runtime process start time does not match the live-session selected game process start time.' -NextStep 'Treat PID reuse or stale process contamination as the leading cause; rerun only after process identity probes are clean.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'game_process_path_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The observed runtime executable path does not match the live-session selected game process path.' -NextStep 'Verify the launched executable and process probe selection before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'command_ack_missing' {
                if ($isRuntimeMonkeyResult -and ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner -or -not $runtimeMonkeyProbeArtifactTrustedForOwner)) {
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The command acknowledgement was absent, but runtime monkey run/probe evidence is missing, invalid, or not byte-bound to retained files.' -NextStep 'Fix runtime monkey artifact retention and probe/sample binding before assigning command-handler ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } elseif ($autoSlayEvidenceInvalidForOwner) {
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The command acknowledgement was absent, but AutoSlay run/probe/traversal evidence is missing, invalid, or not byte-bound to trusted files.' -NextStep 'Fix AutoSlay artifact retention, probe/sample binding, and event traversal proof before assigning command-handler ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } else {
                    $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea $owner -Rationale 'The command was sent but the expected source-backed acknowledgement line was absent.' -NextStep 'Verify foreground/DevConsole input delivery first; if input landed, inspect the target command handler and its preconditions.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
                }
            }
            'command_send_failed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'DevConsoleHarness' -Rationale 'The SendKeys DevConsole helper failed before runtime behavior could be trusted.' -NextStep 'Use window preflight and command-output JSON; do not classify this as gameplay failure until command delivery is proven.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'godot_log_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The launched run did not retain a copied godot.log.' -NextStep 'Fix evidence retention before investigating gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'current_iteration_log_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The launched run did not retain a current-iteration log slice, so stale appended log content cannot be excluded.' -NextStep 'Fix current-iteration log slicing before investigating gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'main_window_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The run reached the main-menu marker without observing a main game window.' -NextStep 'Check launch/window focus and process selection before treating the run as a gameplay failure.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'log_audit_failed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeLogAudit' -Rationale 'audit-godot-log reported release-blocking signatures.' -NextStep 'Use the specific audit signature findings in this report to choose owner area.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'runtime_expectation_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'PackageRuntimeDrift' -Rationale 'Expected package/game/RitsuLib/patch markers did not match the copied log.' -NextStep 'Run installed package/tooling preflight and compare root manifest, installed manifest, RitsuLib variant, and copied log version markers.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'sts1_mode_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'iteration-result.json retained an StS1 mismatch signal, but source ownership requires an analyzer-side StS1 verifier recomputation against the retained current log and audit.' -NextStep 'Use the recomputed sts1_mode_log_check_mismatch finding, if present, for StS1 source ownership; otherwise regenerate the packet before assigning feature ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'restore_failed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The live-session helper failed to restore settings/mods/current runs.' -NextStep 'Inspect restore-state/session-state and fix restore safety before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_session_state_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The iteration did not retain a hash-bound session-state.json, so restore inputs cannot be audited.' -NextStep 'Fix session-state.json retention and SHA256 binding before trusting restore success or routing gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'live_session_restore_state_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The iteration did not retain a schema-versioned, hash-bound restore-state.json, so restore outputs cannot be audited.' -NextStep 'Fix restore-state.json retention and SHA256 binding before trusting restore success or routing gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'post_restore_process_leak' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The restore transaction left SlayTheSpire2 or Godot processes running after restore.' -NextStep 'Inspect restore-state.json post-restore process ids, close leaked processes, and fix restore cleanup before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'restore_item_count_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The restore transaction restored fewer or more mod/current-run items than the moved lists retained in session-state.json.' -NextStep 'Compare session-state.json MovedMods/MovedCurrentRuns against restore-state.json restored counts and fix skipped item restoration before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'preserved_current_runs_manifest_missing' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The restore transaction preserved test-created current-run files but did not retain a hash-bound manifest for them.' -NextStep 'Fix PreservedNewCurrentRunsManifestPath/Sha256 binding in restore-state.json before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'selected_game_process_not_stopped' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The restore transaction did not prove it stopped the selected game process for the launched iteration.' -NextStep 'Bind StopGameOnRestore to the selected process id in restore-state.json before accepting runtime monkey cleanup as complete.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'restore_settings_hash_mismatch' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The restore transaction settings hashes do not match the retained pre-prepare backups.' -NextStep 'Compare session-state.json hashes with restore-state.json hashes and fix settings backup restoration before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            default {
                if ($isRuntimeMonkeyResult -and ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner -or -not $runtimeMonkeyProbeArtifactTrustedForOwner)) {
                    Add-Finding -Findings $findings -Signal ([string]$signal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'iteration-result.json retained an unclassified failure code, but runtime monkey run/probe evidence is missing, invalid, or not byte-bound to retained files.' -NextStep 'Fix runtime monkey artifact retention and probe/sample binding before assigning feature ownership from retained failure codes.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } elseif ($autoSlayEvidenceInvalidForOwner) {
                    Add-Finding -Findings $findings -Signal ([string]$signal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'run-result.json retained an unclassified failure code, but AutoSlay run/probe/traversal evidence is missing, invalid, or not byte-bound to trusted files.' -NextStep 'Fix AutoSlay artifact retention, probe/sample binding, and event traversal proof before assigning feature ownership from retained failure codes.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                } else {
                    $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
                    Add-Finding -Findings $findings -Signal ([string]$signal) -Severity 'blocking' -OwnerArea $owner -Rationale 'Unclassified retained failure code from iteration-result.json.' -NextStep (Get-NextStepForOwner -OwnerArea $owner -Signal ([string]$signal)) -Confidence 'low' -EvidenceFiles $evidenceFiles
                }
            }
            }
        }
    }

    $blockingFindingsSoFar = @($findings | Where-Object { [string]$_.Severity -eq 'blocking' }).Count
    if ($result -and
        -not (Get-JsonBoolValue -Object $result -Name 'Passed' -DefaultValue $false) -and
        @($failureCodes).Count -eq 0 -and
        @($hangSignals).Count -eq 0 -and
        $auditHits.Count -eq 0 -and
        $blockingFindingsSoFar -eq 0) {
        Add-Finding `
            -Findings $findings `
            -Signal 'iteration_failed_without_failure_signal' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale "$ResultFileName says the iteration failed, but it retained no FailureReasonCodes, HangSignals, or audit hits to explain the failure." `
            -NextStep 'Fix runner evidence retention or derive the missing failure code from failed booleans before classifying gameplay source.' `
            -Confidence 'high' `
            -EvidenceFiles $evidenceFiles
    }

    foreach ($hit in $auditHits) {
        $name = [string]$hit.Name
        $auditOwnerText = Get-AuditOwnerText -LogText $ownerLogText -AuditName $name
        $auditLogOwnerArea = Get-OwnerAreaFromText -Text $auditOwnerText -Command ''
        $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $auditLogOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
        $next = Get-NextStepForOwner -OwnerArea $owner -Signal $name

        if ($name -match 'TypeLoadException|MissingMethodException|BaseLib patch failure|Creature\.get_ShowsInfiniteHp|BaseLib\.Patches') {
            $owner = 'PackageRuntimeDrift'
            $next = 'Treat this as installed-game/BaseLib/RitsuLib API drift first; compare current game source/API targets and package build before gameplay fixes.'
        } elseif ($name -match 'Spire Plus error/exception') {
            $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $auditLogOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
            $next = Get-NextStepForOwner -OwnerArea $owner -Signal $name
        } elseif ($name -match 'Godot ERROR line') {
            $next = 'Inspect nearby ERROR lines in godot.log.after-launch; ignore only documented third-party manifest noise already filtered by audit-godot-log.'
        }

        Add-Finding `
            -Findings $findings `
            -Signal "audit:$name" `
            -Severity 'blocking' `
            -OwnerArea $owner `
            -Rationale "godot-log audit hit count $($hit.Count) for '$name'." `
            -NextStep $next `
            -Confidence 'high' `
            -EvidenceFiles $evidenceFiles
    }

    $sts1ModeReportExpectedForOwner = $result -and
        -not $isDirectSmoke -and
        -not $isGameNativeAutoSlay -and
        -not [string]::IsNullOrWhiteSpace($expectedSts1Mode)
    $sts1ModeReportExists = -not [string]::IsNullOrWhiteSpace($sts1ModeCandidate) -and
        (Test-Path -LiteralPath $sts1ModeCandidate -PathType Leaf)
    if ($sts1ModeReportExpectedForOwner -and -not $sts1ModeReportExists) {
        $sts1ModeLogCheckTrustedForOwner = $false
        Add-Finding `
            -Findings $findings `
            -Signal 'sts1_mode_log_check_missing' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale 'The retained run plan expects StS1 mode evidence, but sts1-mode-log-check.json is missing from the iteration evidence.' `
            -NextStep 'Retain sts1-mode-log-check.json generated from godot.log.current-iteration and godot-log-audit.json before assigning StS1 ownership.' `
            -Confidence 'high' `
            -EvidenceFiles $evidenceFiles
    }

    if ((-not $isGameNativeAutoSlay -or $autoSlaySts1ModeArtifactTrustedForOwner) -and
        -not [string]::IsNullOrWhiteSpace($sts1ModeCandidate) -and
        $sts1ModeReportExists) {
        $sts1Report = Read-JsonOrNull -Path $sts1ModeCandidate
        $sts1ModeReportTrustedForOwner = $true
        if ($null -eq $sts1Report) {
            $sts1ModeReportTrustedForOwner = $false
            Add-Finding `
                -Findings $findings `
                -Signal 'sts1_mode_log_check_json_invalid' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'The retained StS1 mode verifier report is missing, empty, or invalid JSON.' `
                -NextStep 'Regenerate sts1-mode-log-check.json from the retained godot.log.current-iteration and godot-log-audit.json before assigning StS1 ownership.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            $expectedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path $currentIterationLogCandidate
            $expectedSts1LogLength = if ($currentIterationLogExists) { [long](Get-Item -LiteralPath $currentIterationLogCandidate).Length } else { -1L }
            $expectedSts1LogSha256 = Get-FileSha256OrEmpty -Path $currentIterationLogCandidate
            $sts1Mode = [string](Get-JsonValue -Object $sts1Report -Name 'Mode' -DefaultValue '')
            $sts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $sts1Report -Name 'LogPath' -DefaultValue ''))
            $sts1LogLengthValue = Get-JsonValue -Object $sts1Report -Name 'LogLength' -DefaultValue $null
            $sts1LogSha256 = [string](Get-JsonValue -Object $sts1Report -Name 'LogSha256' -DefaultValue '')
            $sts1LogLengthMatches = $false
            if ($null -ne $sts1LogLengthValue) {
                try {
                    $sts1LogLengthMatches = (ConvertTo-LongOrDefault -Value $sts1LogLengthValue -DefaultValue -1) -eq $expectedSts1LogLength
                } catch {
                    $sts1LogLengthMatches = $false
                }
            }

            $sts1Mismatches = @(ConvertTo-StringArray -Value @(Get-JsonArrayValues -Object $sts1Report -Name 'Mismatches'))
            $sts1Checks = @(Get-JsonArrayValues -Object $sts1Report -Name 'Checks')
            $sts1CheckSignatures = @(Get-CheckSignatureArray -Items $sts1Checks)

            if ([string]::IsNullOrWhiteSpace($expectedSts1Mode)) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_plan_binding_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained StS1 mode report exists, but the analyzer could not bind it to a retained Sts1EventMode in monkey-plan.json or autoslay-plan.json.' -NextStep 'Retain the run plan with Sts1EventMode and expected package/game/Ritsu targets before routing StS1 mode failures.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } elseif (-not [string]::Equals($sts1Mode, $expectedSts1Mode, [System.StringComparison]::Ordinal)) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_plan_binding_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "The retained StS1 mode report Mode '$sts1Mode' does not match the plan Sts1EventMode '$expectedSts1Mode'." -NextStep 'Regenerate the StS1 mode report from the retained current-iteration log using the mode recorded in the run plan.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }

            if (-not $currentIterationLogExists) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_current_iteration_log_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained StS1 mode report exists, but godot.log.current-iteration is missing so it cannot be recomputed or byte-bound.' -NextStep 'Retain godot.log.current-iteration beside the StS1 mode report before assigning StS1 ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } elseif ([string]::IsNullOrWhiteSpace($sts1LogPath) -or
                [string]::IsNullOrWhiteSpace($expectedSts1LogPath) -or
                -not [System.StringComparer]::OrdinalIgnoreCase.Equals($sts1LogPath, $expectedSts1LogPath) -or
                -not $sts1LogLengthMatches -or
                [string]::IsNullOrWhiteSpace($sts1LogSha256) -or
                -not [System.StringComparer]::OrdinalIgnoreCase.Equals($sts1LogSha256, $expectedSts1LogSha256)) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_current_iteration_binding_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained StS1 mode report LogPath, LogLength, or LogSha256 does not bind to godot.log.current-iteration.' -NextStep 'Regenerate sts1-mode-log-check.json from the retained current-iteration log before using it for owner routing.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }

            if (-not $auditExists) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_audit_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained StS1 mode report exists, but godot-log-audit.json is missing so the verifier cannot be recomputed.' -NextStep 'Retain godot-log-audit.json beside the current-iteration log and regenerate the StS1 mode report.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } elseif (-not (Test-Path -LiteralPath $sts1EnabledModeLogVerifierScript -PathType Leaf)) {
                $sts1ModeReportTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_recompute_script_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "The StS1 mode verifier script is missing: $sts1EnabledModeLogVerifierScript" -NextStep 'Restore check-sts1-enabled-mode-runtime-log.ps1 before analyzing retained StS1 mode reports.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            } elseif (-not [string]::IsNullOrWhiteSpace($expectedSts1Mode) -and $currentIterationLogExists) {
                try {
                    $recomputedSts1Report = Invoke-RecomputedSts1ModeLogCheck `
                        -Mode $expectedSts1Mode `
                        -LogPath $currentIterationLogCandidate `
                        -AuditPath $auditCandidate `
                        -EffectiveExpectedPackageVersion $effectiveExpectedPackageVersion `
                        -EffectiveExpectedGameVersion $effectiveExpectedGameVersion `
                        -EffectiveExpectedRitsuLibVersion $effectiveExpectedRitsuLibVersion `
                        -EffectiveExpectedRitsuCompatBranch $effectiveExpectedRitsuCompatBranch
                    $recomputedSts1Mode = [string](Get-JsonValue -Object $recomputedSts1Report -Name 'Mode' -DefaultValue '')
                    $recomputedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $recomputedSts1Report -Name 'LogPath' -DefaultValue ''))
                    $recomputedSts1LogLengthValue = Get-JsonValue -Object $recomputedSts1Report -Name 'LogLength' -DefaultValue $null
                    $recomputedSts1LogSha256 = [string](Get-JsonValue -Object $recomputedSts1Report -Name 'LogSha256' -DefaultValue '')
                    $recomputedSts1LogLengthMatches = $false
                    if ($null -ne $recomputedSts1LogLengthValue) {
                        try {
                            $recomputedSts1LogLengthMatches = (ConvertTo-LongOrDefault -Value $recomputedSts1LogLengthValue -DefaultValue -1) -eq $expectedSts1LogLength
                        } catch {
                            $recomputedSts1LogLengthMatches = $false
                        }
                    }

                    $recomputedSts1Mismatches = @(ConvertTo-StringArray -Value @(Get-JsonArrayValues -Object $recomputedSts1Report -Name 'Mismatches'))
                    $recomputedSts1Checks = @(Get-JsonArrayValues -Object $recomputedSts1Report -Name 'Checks')
                    $recomputedSts1FailedChecks = @($recomputedSts1Checks | Where-Object {
                        -not (Get-JsonBoolValue -Object $_ -Name 'Passed' -DefaultValue $false)
                    })
                    $recomputedSts1CheckSignatures = @(Get-CheckSignatureArray -Items $recomputedSts1Checks)

                    if ($recomputedSts1Mode -ne $expectedSts1Mode -or
                        [string]::IsNullOrWhiteSpace($recomputedSts1LogPath) -or
                        -not [System.StringComparer]::OrdinalIgnoreCase.Equals($recomputedSts1LogPath, $expectedSts1LogPath) -or
                        -not $recomputedSts1LogLengthMatches -or
                        -not [System.StringComparer]::OrdinalIgnoreCase.Equals($recomputedSts1LogSha256, $expectedSts1LogSha256)) {
                        $sts1ModeReportTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_recomputed_binding_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The freshly recomputed StS1 mode report did not bind back to the retained current-iteration log.' -NextStep 'Inspect check-sts1-enabled-mode-runtime-log.ps1 and retained log/audit paths before routing StS1 ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-StringArrayEquals -Actual $sts1Mismatches -Expected $recomputedSts1Mismatches) -or
                        -not (Test-StringArrayEquals -Actual $sts1CheckSignatures -Expected $recomputedSts1CheckSignatures)) {
                        $sts1ModeReportTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_recomputed_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained StS1 mode report Mismatches or Checks do not match a fresh recomputation from godot.log.current-iteration.' -NextStep 'Treat the retained StS1 report as stale or hand-edited; regenerate the packet before assigning source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if ($sts1ModeReportTrustedForOwner -and
                        $auditTrustedForOwner -and
                        ($recomputedSts1Mismatches.Count -gt 0 -or $recomputedSts1FailedChecks.Count -gt 0)) {
                        Add-Finding `
                            -Findings $findings `
                            -Signal 'sts1_mode_log_check_mismatch' `
                            -Severity 'blocking' `
                            -OwnerArea 'Sts1Events' `
                            -Rationale "A fresh StS1 mode verifier recomputation contains $($recomputedSts1Mismatches.Count) mismatches and $($recomputedSts1FailedChecks.Count) failed checks." `
                            -NextStep 'Classify this as environment propagation if the log shows Off/default mode; otherwise inspect registration count, class set, and tuple expectations.' `
                            -Confidence 'high' `
                            -EvidenceFiles $evidenceFiles
                    }
                } catch {
                    $sts1ModeReportTrustedForOwner = $false
                    Add-Finding -Findings $findings -Signal 'sts1_mode_log_check_recompute_failed' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Failed to recompute the StS1 mode verifier report: $($_.Exception.Message)" -NextStep 'Fix the retained current log, audit, or StS1 verifier inputs before assigning StS1 ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
        }

        if (-not $sts1ModeReportTrustedForOwner) {
            $sts1ModeLogCheckTrustedForOwner = $false
            if ($isGameNativeAutoSlay) {
                $autoSlaySts1ModeArtifactTrustedForOwner = $false
            }
        }
    }

    $autoSlayEvidenceInvalidForOwner = $isGameNativeAutoSlay -and (
        -not $autoSlayRunArtifactsTrustedForOwner -or
        -not $autoSlayProbeArtifactTrustedForOwner -or
        -not $autoSlaySidecarTrustedForOwner -or
        -not $autoSlayAuditArtifactTrustedForOwner -or
        -not $autoSlaySts1ModeArtifactTrustedForOwner -or
        -not $logTextTrustedForOwner)
    & $addRetainedSignalFindings -AutoSlayEvidenceInvalidForOwner $autoSlayEvidenceInvalidForOwner

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_gameplay_disabled|coop_combat_hook_disabled') {
        Add-Finding -Findings $findings -Signal 'coop_fail_closed_observed' -Severity 'info' -OwnerArea 'MultiplayerPolicy' -Rationale 'The log shows co-op gameplay/combat hooks failing closed.' -NextStep 'Treat as expected only when no explicit SPIREPLUS_ALLOW_UNVERIFIED_COOP_* debug gate was intended.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($logTextTrustedForOwner -and $logText -match '(?i)\bcoop_[a-z0-9_]*override_enabled\b') {
        $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
        Add-Finding -Findings $findings -Signal 'coop_override_enabled_runtime_failure' -Severity 'blocking' -OwnerArea $owner -Rationale 'A co-op unsafe/debug override appears near a runtime failure.' -NextStep 'Treat this as deliberate unsafe two-client debugging; route by feature text and preserve both host/client logs.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
    }

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_local_ui_preview_enabled|prediction_prepared_multiplayer_ui_only') {
        Add-Finding -Findings $findings -Signal 'coop_preview_ui_only_observed' -Severity 'info' -OwnerArea 'PreviewTools' -Rationale 'The log shows preview tools running as local UI only in multiplayer.' -NextStep 'This supports preview-tool co-op policy, but still does not prove two-client behavior without live evidence.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    $commandText = [string]$command
    if ([regex]::IsMatch($commandText, '(?i)spireplus_test_ancient\s+VAKUU') -and (@($hangSignals).Count -gt 0 -or @($failureCodes).Count -gt 0)) {
        if ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner -or $autoSlayEvidenceInvalidForOwner) {
            Add-Finding -Findings $findings -Signal 'vakuu_command_failed_or_hung' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The failing iteration targeted Vakuu through a live-test command, but runtime or AutoSlay run/probe/traversal evidence is missing, invalid, or not byte-bound to retained files.' -NextStep 'Fix runtime monkey or AutoSlay artifact retention and probe/sample/traversal binding before assigning Vakuu source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        } else {
            $vakuuOwner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea
            Add-Finding -Findings $findings -Signal 'vakuu_command_failed_or_hung' -Severity 'blocking' -OwnerArea $vakuuOwner -Rationale 'The failing iteration targeted Vakuu through the live-test command.' -NextStep (Get-NextStepForOwner -OwnerArea $vakuuOwner -Signal 'vakuu_command_failed_or_hung') -Confidence 'medium' -EvidenceFiles $evidenceFiles
        }
    }

    $signalItems = [System.Collections.Generic.List[object]]::new()
    foreach ($signalItem in @($failureCodes)) {
        $signalItems.Add($signalItem) | Out-Null
    }
    foreach ($signalItem in @($hangSignals)) {
        $signalItems.Add($signalItem) | Out-Null
    }
    foreach ($hit in @($auditHits.ToArray())) {
        $signalItems.Add("audit:$([string](Get-JsonValue -Object $hit -Name 'Name' -DefaultValue ''))") | Out-Null
    }
    foreach ($finding in @($findings)) {
        $signalItems.Add((Get-JsonValue -Object $finding -Name 'Signal' -DefaultValue '')) | Out-Null
    }
    $signals = @($signalItems.ToArray()) | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } | Select-Object -Unique

    [pscustomobject]@{
        IterationDir = $Directory
        Iteration = if ($result) { Get-JsonIntValue -Object $result -Name 'Iteration' -DefaultValue $DefaultIteration } else { $DefaultIteration }
        Seed = $seed
        RunnerKind = $runnerKind
        EventKind = $eventKind
        AncientId = $ancientId
        Passed = if ($result) { (Get-JsonBoolValue -Object $result -Name 'Passed' -DefaultValue $false) } else { $false }
        Command = $command
        ScenarioTag = $scenarioTag
        OwnerAreaHint = $resultOwnerArea
        OwnerAreaFromLog = $logOwnerArea
        LogTextTrustedForOwner = $logTextTrustedForOwner
        RuntimeMonkeyRunArtifactsTrustedForOwner = $runtimeMonkeyRunArtifactsTrustedForOwner
        RuntimeMonkeyProbeArtifactTrustedForOwner = $runtimeMonkeyProbeArtifactTrustedForOwner
        AutoSlaySidecarTrustedForOwner = $autoSlaySidecarTrustedForOwner
        AutoSlayRunArtifactsTrustedForOwner = $autoSlayRunArtifactsTrustedForOwner
        AutoSlayProbeArtifactTrustedForOwner = $autoSlayProbeArtifactTrustedForOwner
        AutoSlayAuditArtifactTrustedForOwner = $autoSlayAuditArtifactTrustedForOwner
        AutoSlaySts1ModeArtifactTrustedForOwner = $autoSlaySts1ModeArtifactTrustedForOwner
        Sts1ModeLogCheckTrustedForOwner = $sts1ModeLogCheckTrustedForOwner
        OwnerAreaFromCommand = $commandOwnerArea
        Signals = @($signals)
        EvidenceFiles = @($evidenceFiles)
        FailureReasonCodes = @($failureCodes)
        HangSignals = @($hangSignals)
        AuditTrustedForOwner = $auditTrustedForOwner
        AuditHits = @($auditHits.ToArray())
        BaseLibPatchFailures = @($baseLibPatchFailures)
        Findings = @($findings)
    }
}

$analysisTargets = @()
$summary = $null
$summaryResultsByIteration = @{}
$summaryMismatchDetails = @()
$evidenceFull = ''

if ($IterationDir) {
    $resolvedIterationDir = Resolve-RepoPath -Path $IterationDir
    $resultFileName = if (Test-Path -LiteralPath (Join-Path $resolvedIterationDir 'run-result.json') -PathType Leaf) {
        'run-result.json'
    } else {
        'iteration-result.json'
    }
    $defaultIteration = if ($resolvedIterationDir -match '(?:iteration|run)-(\d+)$') { [int]$Matches[1] } else { $Iteration }
    $analysisTargets = @([pscustomobject]@{
        Directory = $resolvedIterationDir
        SummaryResult = $null
        ResultFileName = $resultFileName
        DefaultIteration = $defaultIteration
    })
} elseif ($EvidenceDir) {
    $evidenceFull = Resolve-RepoPath -Path $EvidenceDir
    $summaryPath = Join-Path $evidenceFull 'monkey-summary.json'
    $autoSlaySummaryPath = Join-Path $evidenceFull 'autoslay-summary.json'
    $directSmokeSummaryPath = Join-Path $evidenceFull 'direct-smoke-summary.json'
    $autoSlaySummary = Read-JsonOrNull -Path $autoSlaySummaryPath
    $summary = Read-JsonOrNull -Path $summaryPath
    $directSmokeSummary = Read-JsonOrNull -Path $directSmokeSummaryPath
    if ($autoSlaySummary -and
        [string]::Equals([string](Get-JsonValue -Object $autoSlaySummary -Name 'RunnerKind' -DefaultValue ''), 'GameNativeAutoSlay', [System.StringComparison]::Ordinal) -and
        (Test-JsonProperty -Object $autoSlaySummary -Name 'Runs') -and
        @($autoSlaySummary.Runs).Count -gt 0) {
        $runIndex = 0
        foreach ($run in @($autoSlaySummary.Runs)) {
            $runIndex++
            if ($Iteration -gt 0 -and $runIndex -ne $Iteration) {
                continue
            }

            if (-not (Test-JsonProperty -Object $run -Name 'RunnerKind')) {
                $run | Add-Member -MemberType NoteProperty -Name 'RunnerKind' -Value 'GameNativeAutoSlay' -Force
            }

            $runResultPath = Resolve-AnalysisPath -BaseDir $evidenceFull -Path ([string](Get-JsonValue -Object $run -Name 'RunResultPath' -DefaultValue ('run-{0:D4}/run-result.json' -f $runIndex)))
            $runResultPathInsideEvidenceDir = Test-PathInsideDirectory -Path $runResultPath -Directory $evidenceFull
            $runDirectory = if ($runResultPathInsideEvidenceDir) { [System.IO.Path]::GetDirectoryName($runResultPath) } else { Join-Path $evidenceFull ('run-{0:D4}' -f $runIndex) }
            if ([string]::IsNullOrWhiteSpace($runDirectory)) {
                $runDirectory = Join-Path $evidenceFull ('run-{0:D4}' -f $runIndex)
            }
            $runResultFileName = if ($runResultPathInsideEvidenceDir) { [System.IO.Path]::GetFileName($runResultPath) } else { 'run-result.json' }

            $analysisTargets += [pscustomobject]@{
                Directory = $runDirectory
                SummaryResult = $run
                ResultFileName = $runResultFileName
                DefaultIteration = $runIndex
                RunResultPathInsideEvidenceDir = $runResultPathInsideEvidenceDir
                ExpectedRunnerKind = 'GameNativeAutoSlay'
                Summary = $autoSlaySummary
            }
        }
    } else {
        if ($summary -and (Test-JsonProperty -Object $summary -Name 'Results')) {
            $summaryMismatchDetails = @(Get-RuntimeMonkeySummaryMismatchDetails -Summary $summary)
            foreach ($result in @($summary.Results)) {
                $summaryResultsByIteration[(Get-JsonIntValue -Object $result -Name 'Iteration' -DefaultValue 0)] = $result
            }
        }

        $iterationDirs = @()
        $failedIterationIdsInvalidDetails = @()
        if ($Iteration -gt 0) {
            $iterationDirs = @(Join-Path $evidenceFull ('iteration-{0:D4}' -f $Iteration))
        } elseif ($summary -and (Test-JsonProperty -Object $summary -Name 'FailedIterationIds') -and ($summary.FailedIterationIds | Measure-Object).Count -gt 0) {
            $failedIterationIds = [System.Collections.Generic.List[int]]::new()
            $failedIterationIdValues = @($summary.FailedIterationIds)
            $failedIterationIdValueCount = ($failedIterationIdValues | Measure-Object).Count
            for ($failedIterationIdIndex = 0; $failedIterationIdIndex -lt $failedIterationIdValueCount; $failedIterationIdIndex++) {
                $rawFailedIterationId = $failedIterationIdValues[$failedIterationIdIndex]
                $failedIterationId = ConvertTo-IntOrDefault -Value $rawFailedIterationId -DefaultValue 0
                if ($failedIterationId -le 0) {
                    $failedIterationIdsInvalidDetails = @($failedIterationIdsInvalidDetails) + "index=$failedIterationIdIndex value='$rawFailedIterationId'"
                } else {
                    $failedIterationIds.Add($failedIterationId) | Out-Null
                }
            }

            if (($failedIterationIdsInvalidDetails | Measure-Object).Count -gt 0) {
                $iterationDirs = @(Get-ChildItem -LiteralPath $evidenceFull -Directory -Filter 'iteration-*' -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object { $_.FullName })
                if (($iterationDirs | Measure-Object).Count -eq 0) {
                    $iterationDirs = @($evidenceFull)
                }
            } else {
                $iterationDirs = @($failedIterationIds.ToArray() | Sort-Object -Unique | ForEach-Object {
                    Join-Path $evidenceFull ('iteration-{0:D4}' -f $_)
                })
            }
        } else {
            $iterationDirs = @(Get-ChildItem -LiteralPath $evidenceFull -Directory -Filter 'iteration-*' -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object { $_.FullName })
        }

        if (($iterationDirs | Measure-Object).Count -eq 0 -and
            $Iteration -le 0 -and
            $directSmokeSummary) {
            $analysisTargets += [pscustomobject]@{
                Directory = $evidenceFull
                SummaryResult = $null
                ResultFileName = 'direct-smoke-summary.json'
                DefaultIteration = 0
            }
        }

        foreach ($dir in $iterationDirs) {
            $iterationNumber = if ($dir -match 'iteration-(\d+)$') { [int]$Matches[1] } else { 0 }
            $summaryResult = if ($summaryResultsByIteration.ContainsKey($iterationNumber)) { $summaryResultsByIteration[$iterationNumber] } else { $null }
            $analysisTargets += [pscustomobject]@{
                Directory = $dir
                SummaryResult = $summaryResult
                ResultFileName = 'iteration-result.json'
                DefaultIteration = $iterationNumber
                SummaryFailedIterationIdsInvalidDetails = @($failedIterationIdsInvalidDetails)
                SummaryMismatchDetails = @($summaryMismatchDetails)
                RequireRuntimeMonkeyPlanBinding = $null -ne $summary
                Summary = $summary
            }
        }
    }
} elseif ($LogPath -or $AuditPath) {
    $tempDir = Join-Path $env:TEMP ('spire-plus-runtime-analysis-' + [guid]::NewGuid().ToString('N'))
    [void][System.IO.Directory]::CreateDirectory($tempDir)
    if ($LogPath) {
        Copy-Item -LiteralPath (Resolve-RepoPath -Path $LogPath) -Destination (Join-Path $tempDir 'godot.log.after-launch') -Force
    }
    if ($AuditPath) {
        Copy-Item -LiteralPath (Resolve-RepoPath -Path $AuditPath) -Destination (Join-Path $tempDir 'godot-log-audit.json') -Force
    }

    $analysisTargets = @([pscustomobject]@{
        Directory = $tempDir
        SummaryResult = $null
        ResultFileName = 'iteration-result.json'
        DefaultIteration = 0
    })
} else {
    throw 'Pass -EvidenceDir, -IterationDir, or -LogPath/-AuditPath.'
}

$iterationReports = foreach ($target in $analysisTargets) {
    $runResultPathInsideEvidenceDir = if ($null -ne $target.PSObject.Properties['RunResultPathInsideEvidenceDir']) { [bool]$target.RunResultPathInsideEvidenceDir } else { $true }
    $expectedRunnerKind = if ($null -ne $target.PSObject.Properties['ExpectedRunnerKind']) { [string]$target.ExpectedRunnerKind } else { '' }
    $summaryFailedIterationIdsInvalidDetails = if ($null -ne $target.PSObject.Properties['SummaryFailedIterationIdsInvalidDetails']) { @($target.SummaryFailedIterationIdsInvalidDetails) } else { @() }
    $summaryMismatchDetails = if ($null -ne $target.PSObject.Properties['SummaryMismatchDetails']) { @($target.SummaryMismatchDetails) } else { @() }
    $requireRuntimeMonkeyPlanBinding = if ($null -ne $target.PSObject.Properties['RequireRuntimeMonkeyPlanBinding']) { [bool]$target.RequireRuntimeMonkeyPlanBinding } else { $false }
    $analysisSummary = if ($null -ne $target.PSObject.Properties['Summary']) { $target.Summary } else { $null }
    try {
        Analyze-Iteration -Directory $target.Directory -SummaryResult $target.SummaryResult -ResultFileName $target.ResultFileName -DefaultIteration $target.DefaultIteration -RunResultPathInsideEvidenceDir $runResultPathInsideEvidenceDir -ExpectedRunnerKind $expectedRunnerKind -SummaryFailedIterationIdsInvalidDetails $summaryFailedIterationIdsInvalidDetails -SummaryMismatchDetails $summaryMismatchDetails -RequireRuntimeMonkeyPlanBinding $requireRuntimeMonkeyPlanBinding -Summary $analysisSummary
    } catch {
        $scriptStack = if ([string]::IsNullOrWhiteSpace([string]$_.ScriptStackTrace)) { '<no script stack>' } else { [string]$_.ScriptStackTrace }
        throw "Analyze-Iteration failed for '$($target.Directory)' result '$($target.ResultFileName)': $($_.Exception.Message)`n$scriptStack"
    }
}

$allFindings = @($iterationReports | ForEach-Object { @($_.Findings) })
$blockingFindings = @($allFindings | Where-Object { [string]$_.Severity -eq 'blocking' })
$harnessBlockingFindings = @($blockingFindings | Where-Object { Test-HarnessOwnerArea -OwnerArea ([string]$_.OwnerArea) })
$packageBlockingFindings = @($blockingFindings | Where-Object { [string]$_.OwnerArea -eq 'PackageRuntimeDrift' })
$gameplayBlockingFindings = @($blockingFindings | Where-Object {
    -not (Test-HarnessOwnerArea -OwnerArea ([string]$_.OwnerArea)) -and [string]$_.OwnerArea -ne 'PackageRuntimeDrift'
})
$triageDisposition = if (@($harnessBlockingFindings).Count -gt 0) {
    'HarnessEvidenceInvalid'
} elseif (@($packageBlockingFindings).Count -gt 0) {
    'PackageRuntimeDrift'
} elseif (@($gameplayBlockingFindings).Count -gt 0) {
    'GameplayOwnerAction'
} else {
    'NoBlockingFindings'
}
$recommendedNextActions = @($blockingFindings |
    ForEach-Object { [string]$_.NextStep } |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
    Select-Object -Unique |
    Select-Object -First 10)
$ownerAreas = @($allFindings | ForEach-Object { $_.OwnerArea } | Where-Object { $_ } | Select-Object -Unique)

$report = [pscustomobject]@{
    SchemaVersion = 1
    CreatedAt = (Get-Date).ToString('o')
    EvidenceDir = $evidenceFull
    AnalyzedIterationCount = @($iterationReports).Count
    BlockingFindingCount = @($blockingFindings).Count
    TriageDisposition = $triageDisposition
    HarnessBlockingFindingCount = @($harnessBlockingFindings).Count
    PackageBlockingFindingCount = @($packageBlockingFindings).Count
    GameplayBlockingFindingCount = @($gameplayBlockingFindings).Count
    OwnerAreas = @($ownerAreas)
    RecommendedNextActions = @($recommendedNextActions)
    HarnessBlockingFindings = @($harnessBlockingFindings)
    PackageBlockingFindings = @($packageBlockingFindings)
    GameplayBlockingFindings = @($gameplayBlockingFindings)
    Iterations = @($iterationReports)
}

foreach ($iterationReport in @($iterationReports)) {
    Write-Output "iteration=$($iterationReport.Iteration) scenario=$($iterationReport.ScenarioTag) owner_hint=$($iterationReport.OwnerAreaHint) owner_log=$($iterationReport.OwnerAreaFromLog) owner_command=$($iterationReport.OwnerAreaFromCommand) passed=$($iterationReport.Passed) findings=$(@($iterationReport.Findings).Count) command='$($iterationReport.Command)'"
    foreach ($finding in @($iterationReport.Findings)) {
        Write-Output "finding severity=$($finding.Severity) confidence=$($finding.Confidence) owner=$($finding.OwnerArea) signal=$($finding.Signal) next='$($finding.NextStep)'"
    }
}

Write-Output "analyzed_iterations=$(@($iterationReports).Count)"
Write-Output "blocking_findings=$(@($blockingFindings).Count)"
Write-Output "triage_disposition=$triageDisposition harness_blockers=$(@($harnessBlockingFindings).Count) package_blockers=$(@($packageBlockingFindings).Count) gameplay_blockers=$(@($gameplayBlockingFindings).Count)"
Write-Output "owner_areas=$($ownerAreas -join ',')"

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath -Path $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnBlockingFinding -and @($blockingFindings).Count -gt 0) {
    exit 1
}
