param(
    [Parameter(Mandatory = $true)]
    [string]$EvidenceDir,

    [int]$ExpectedIterations = 0,

    [string]$ExpectedPackageVersion,

    [int]$ExpectedPatchCount = 0,

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

function Read-JsonOrNull {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$CheckName
    )

    try {
        return [System.IO.File]::ReadAllText($Path) | ConvertFrom-Json
    } catch {
        Add-Check -Name $CheckName -Passed $false -Detail "invalid JSON in $Path`: $($_.Exception.Message)"
        return $null
    }
}

function Contains-Text {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    if ([string]::IsNullOrWhiteSpace($Needle)) {
        return $false
    }

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Get-PatchCountLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][int]$ExpectedCount
    )

    if ($ExpectedCount -le 0) {
        return 0
    }

    $count = [regex]::Escape([string]$ExpectedCount)
    $patchSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+\[Patcher - SpirePlus\]\s+Patch application complete:\s+$count\s+applied,\s+0\s+ignored,\s+0\s+failed,\s+$count\s+total\s*$"
    $registeredSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+ModPatcher applied\s+$count\s+patches\s+\($count\s+registered\)\.?\s*$"
    if ([regex]::IsMatch($Text, $patchSummary) -and [regex]::IsMatch($Text, $registeredSummary)) {
        return 1
    }

    return 0
}

function Read-AuditSummary {
    param([Parameter(Mandatory = $true)][string]$Path)

    $json = [System.IO.File]::ReadAllText($Path)
    $items = @($json | ConvertFrom-Json)
    $dirtyItems = 0
    $hitCount = 0

    foreach ($item in $items) {
        if (-not (Test-JsonProperty -Object $item -Name 'Clean') -or -not [bool]$item.Clean) {
            $dirtyItems++
        }

        if (-not (Test-JsonProperty -Object $item -Name 'SignatureHits')) {
            continue
        }

        foreach ($hit in @($item.SignatureHits)) {
            if (Test-JsonProperty -Object $hit -Name 'Count') {
                $hitCount += [int]$hit.Count
            }
        }
    }

    return [pscustomobject]@{
        Path = $Path
        Items = $items.Count
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0)
    }
}

function Resolve-EvidenceFile {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    return [System.IO.Path]::GetFullPath((Join-Path $resolvedEvidenceDir $RelativePath))
}

function Resolve-ChildOrAbsolutePath {
    param(
        [Parameter(Mandatory = $true)][string]$BaseDir,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $BaseDir $Path))
}

function Test-PathUnderDirectory {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Directory
    )

    $directoryFull = [System.IO.Path]::GetFullPath($Directory).TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $pathFull = [System.IO.Path]::GetFullPath($Path)
    return $pathFull.StartsWith($directoryFull, [System.StringComparison]::OrdinalIgnoreCase)
}

function Get-ArrayCount {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return 0
    }

    return @($Value).Count
}

function Test-AnyJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
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
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
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
        if ((Test-JsonProperty -Object $item -Name $Name) -and $null -ne $item.$Name -and -not [bool]$item.$Name) {
            return $false
        }
    }

    return $true
}

function Get-ValueCounts {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$PropertyName
    )

    $counts = [ordered]@{}
    foreach ($item in @($Items)) {
        $value = [string](Get-JsonValue -Object $item -Name $PropertyName -DefaultValue '')
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

function Get-CountMapTotal {
    param([AllowNull()]$CountMap)

    if ($null -eq $CountMap) {
        return 0
    }

    $total = 0
    foreach ($property in @($CountMap.PSObject.Properties)) {
        $total += [int]$property.Value
    }

    return $total
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
        if ($null -eq $actualProperty -or [int]$actualProperty.Value -ne [int]$ExpectedCounts[$key]) {
            return $false
        }
    }

    return $true
}

$resolvedEvidenceDir = Resolve-RepoPath $EvidenceDir
if (-not (Test-Path -LiteralPath $resolvedEvidenceDir -PathType Container)) {
    Write-Error "Evidence directory not found: $resolvedEvidenceDir"
    exit 1
}

$planPath = Resolve-EvidenceFile 'monkey-plan.json'
$summaryPath = Resolve-EvidenceFile 'monkey-summary.json'

Write-Output "evidence_dir=$resolvedEvidenceDir"

$planExists = Test-Path -LiteralPath $planPath -PathType Leaf
$summaryExists = Test-Path -LiteralPath $summaryPath -PathType Leaf
Add-Check -Name 'monkey_plan_exists' -Passed $planExists -Detail 'requires monkey-plan.json'
Add-Check -Name 'monkey_summary_exists' -Passed $summaryExists -Detail 'requires monkey-summary.json from a launched monkey run'

$plan = $null
$summary = $null
if ($planExists) {
    $plan = Read-JsonOrNull -Path $planPath -CheckName 'monkey_plan_json_valid'
    if ($null -ne $plan) {
        Add-Check -Name 'plan_json_valid' -Passed $true -Detail 'monkey-plan.json parsed'
    }
}

if ($summaryExists) {
    $summary = Read-JsonOrNull -Path $summaryPath -CheckName 'monkey_summary_json_valid'
    if ($null -ne $summary) {
        Add-Check -Name 'summary_json_valid' -Passed $true -Detail 'monkey-summary.json parsed'
    }
}

$planIterations = if ($null -ne $plan) { [int](Get-JsonValue -Object $plan -Name 'Iterations' -DefaultValue 0) } else { 0 }
$planUnresponsiveSampleThreshold = if ($null -ne $plan) { [int](Get-JsonValue -Object $plan -Name 'UnresponsiveSampleThreshold' -DefaultValue 0) } else { 0 }
$summaryRequestedIterations = if ($null -ne $summary) { [int](Get-JsonValue -Object $summary -Name 'RequestedIterations' -DefaultValue 0) } else { 0 }
$summaryCompletedIterations = if ($null -ne $summary) { [int](Get-JsonValue -Object $summary -Name 'CompletedIterations' -DefaultValue 0) } else { 0 }
$expectedIterationCount = $ExpectedIterations
$planScenario = if ($null -ne $plan) { [string](Get-JsonValue -Object $plan -Name 'Scenario' -DefaultValue '') } else { '' }
$planCommandSelectionMode = if ($null -ne $plan) { [string](Get-JsonValue -Object $plan -Name 'CommandSelectionMode' -DefaultValue '') } else { '' }
$planPlannedCommands = @(
    if ($null -ne $plan -and (Test-JsonProperty -Object $plan -Name 'PlannedCommands')) {
        $plan.PlannedCommands
    }
)
if ($expectedIterationCount -le 0 -and $planIterations -gt 0) {
    $expectedIterationCount = $planIterations
}

if ($expectedIterationCount -le 0 -and $summaryRequestedIterations -gt 0) {
    $expectedIterationCount = $summaryRequestedIterations
}

if ($null -ne $plan) {
    Add-Check -Name 'plan_hang_probe_schema_version' -Passed ([int](Get-JsonValue -Object $plan -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'HangProbeSchemaVersion must be 1'
    Add-Check -Name 'plan_launch_true' -Passed ([bool](Get-JsonValue -Object $plan -Name 'Launch' -DefaultValue $false)) -Detail 'monkey packets require a launched run; dry-run plans have no runtime evidence'
    Add-Check -Name 'plan_iterations_positive' -Passed ($planIterations -gt 0) -Detail "plan Iterations must be positive; found $planIterations"
    Add-Check -Name 'plan_scenario_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'Scenario' -DefaultValue ''))) -Detail 'Scenario must identify the planned risk lane'
    Add-Check -Name 'plan_command_selection_mode_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'CommandSelectionMode' -DefaultValue ''))) -Detail 'CommandSelectionMode must be retained'
    Add-Check -Name 'plan_command_corpus_source_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'CommandCorpusSource' -DefaultValue ''))) -Detail 'CommandCorpusSource must be retained'
    Add-Check -Name 'plan_observation_interval_positive' -Passed ([int](Get-JsonValue -Object $plan -Name 'ObservationIntervalSeconds' -DefaultValue 0) -gt 0) -Detail 'ObservationIntervalSeconds must be present and positive'
    Add-Check -Name 'plan_unresponsive_sample_threshold_positive' -Passed ($planUnresponsiveSampleThreshold -gt 0) -Detail 'UnresponsiveSampleThreshold must be present and positive'
    Add-Check -Name 'plan_no_log_growth_timeout_positive' -Passed ([int](Get-JsonValue -Object $plan -Name 'NoLogGrowthTimeoutSeconds' -DefaultValue 0) -gt 0) -Detail 'NoLogGrowthTimeoutSeconds must be present and positive'
    Add-Check -Name 'plan_process_probe_present' -Passed (Test-JsonProperty -Object $plan -Name 'ProcessProbe') -Detail 'ProcessProbe must describe process/window checks'
    Add-Check -Name 'plan_log_growth_probe_present' -Passed (Test-JsonProperty -Object $plan -Name 'LogGrowthProbe') -Detail 'LogGrowthProbe must describe log-growth checks'
    Add-Check -Name 'plan_command_scenario_matrix_present' -Passed (Test-JsonProperty -Object $plan -Name 'CommandScenarioMatrix') -Detail 'CommandScenarioMatrix must map commands to owner areas'
    Add-Check -Name 'plan_command_ack_patterns_present' -Passed (Test-JsonProperty -Object $plan -Name 'CommandAckPatterns') -Detail 'CommandAckPatterns must be retained even when empty'
    Add-Check -Name 'plan_planned_commands_count_matches_iterations' -Passed ($planPlannedCommands.Count -eq $planIterations) -Detail "PlannedCommands must match plan Iterations; planned=$($planPlannedCommands.Count), iterations=$planIterations"

    $plannedScenarioTagCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'ScenarioTag'
    $plannedOwnerAreaCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'OwnerArea'
    $plannedCommandCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'Command'
    Add-Check -Name 'plan_planned_scenario_tag_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedScenarioTagCounts' -DefaultValue $null) -ExpectedCounts $plannedScenarioTagCounts) -Detail 'PlannedScenarioTagCounts must match PlannedCommands'
    Add-Check -Name 'plan_planned_owner_area_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedOwnerAreaCounts' -DefaultValue $null) -ExpectedCounts $plannedOwnerAreaCounts) -Detail 'PlannedOwnerAreaCounts must match PlannedCommands'
    Add-Check -Name 'plan_planned_command_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedCommandCounts' -DefaultValue $null) -ExpectedCounts $plannedCommandCounts) -Detail 'PlannedCommandCounts must match PlannedCommands'

    $plannedVakuuFightCount = @($planPlannedCommands | Where-Object { [string](Get-JsonValue -Object $_ -Name 'ScenarioTag' -DefaultValue '') -eq 'vakuu-fight' }).Count
    Add-Check -Name 'plan_vakuu_fight_planned_count_matches' -Passed ([int](Get-JsonValue -Object $plan -Name 'PlannedVakuuFightIterationCount' -DefaultValue -1) -eq $plannedVakuuFightCount) -Detail "PlannedVakuuFightIterationCount must match PlannedCommands; expected $plannedVakuuFightCount"
    if ($planScenario -eq 'VakuuFightSmoke') {
        Add-Check -Name 'plan_vakuu_fight_smoke_all_iterations_are_fight' -Passed ($plannedVakuuFightCount -eq $planIterations) -Detail "VakuuFightSmoke must plan only vakuu-fight iterations; found $plannedVakuuFightCount of $planIterations"
    } elseif ($planScenario -eq 'AncientUiPlusVakuuFight') {
        Add-Check -Name 'plan_ancient_ui_plus_vakuu_fight_includes_fight' -Passed ($plannedVakuuFightCount -gt 0) -Detail 'AncientUiPlusVakuuFight must include at least one vakuu-fight iteration'
        if ($planCommandSelectionMode -eq 'RoundRobin' -and $planIterations -eq 1000) {
            Add-Check -Name 'plan_ancient_ui_plus_vakuu_fight_1000_balanced' -Passed ($plannedVakuuFightCount -eq 200) -Detail "1000-iteration AncientUiPlusVakuuFight RoundRobin plan must include exactly 200 vakuu-fight iterations; found $plannedVakuuFightCount"
        }
    }

    if ($ExpectedIterations -gt 0) {
        Add-Check -Name 'expected_iterations_match_plan' -Passed ($planIterations -eq $ExpectedIterations) -Detail "expected $ExpectedIterations iterations in plan, found $planIterations"
    }
}

if ($null -ne $summary) {
    $summaryResults = @()
    if (Test-JsonProperty -Object $summary -Name 'Results') {
        $summaryResults = @($summary.Results)
    }

    Add-Check -Name 'summary_hang_probe_schema_version' -Passed ([int](Get-JsonValue -Object $summary -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'summary HangProbeSchemaVersion must be 1'
    Add-Check -Name 'summary_passed' -Passed ([bool](Get-JsonValue -Object $summary -Name 'Passed' -DefaultValue $false)) -Detail 'monkey-summary.json Passed must be true'
    Add-Check -Name 'summary_failed_iterations_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'FailedIterations' -DefaultValue -1) -eq 0) -Detail "FailedIterations must be 0; found $(Get-JsonValue -Object $summary -Name 'FailedIterations' -DefaultValue 'missing')"
    $failedIterationIdsCount = Get-ArrayCount -Value (Get-JsonValue -Object $summary -Name 'FailedIterationIds' -DefaultValue @())
    Add-Check -Name 'summary_failed_iteration_ids_empty' -Passed ($failedIterationIdsCount -eq 0) -Detail 'FailedIterationIds must be empty for a clean packet'
    Add-Check -Name 'summary_process_exit_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'ProcessExitCount' -DefaultValue -1) -eq 0) -Detail 'ProcessExitCount must be 0'
    Add-Check -Name 'summary_main_window_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'MainWindowMissingCount' -DefaultValue -1) -eq 0) -Detail 'MainWindowMissingCount must be 0'
    Add-Check -Name 'summary_unresponsive_iteration_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'UnresponsiveIterationCount' -DefaultValue -1) -eq 0) -Detail 'UnresponsiveIterationCount must be 0'
    Add-Check -Name 'summary_log_stall_iteration_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'LogStallIterationCount' -DefaultValue -1) -eq 0) -Detail 'LogStallIterationCount must be 0'
    Add-Check -Name 'summary_command_ack_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'CommandAckMissingCount' -DefaultValue -1) -eq 0) -Detail 'CommandAckMissingCount must be 0'
    Add-Check -Name 'summary_max_consecutive_unresponsive_recorded' -Passed ([int](Get-JsonValue -Object $summary -Name 'MaxConsecutiveUnresponsiveSamples' -DefaultValue -1) -ge 0) -Detail 'MaxConsecutiveUnresponsiveSamples must be recorded'
    $resultScenarioTagCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'ScenarioTag'
    $resultOwnerAreaCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'OwnerArea'
    $resultCommandCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'Command'
    Add-Check -Name 'summary_scenario_tag_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'ScenarioTagCounts' -DefaultValue $null) -ExpectedCounts $resultScenarioTagCounts) -Detail 'ScenarioTagCounts must match summary Results'
    Add-Check -Name 'summary_owner_area_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'OwnerAreaCounts' -DefaultValue $null) -ExpectedCounts $resultOwnerAreaCounts) -Detail 'OwnerAreaCounts must match summary Results'
    Add-Check -Name 'summary_command_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'CommandCounts' -DefaultValue $null) -ExpectedCounts $resultCommandCounts) -Detail 'CommandCounts must match summary Results'
    $summaryVakuuFightCount = @($summaryResults | Where-Object { [string](Get-JsonValue -Object $_ -Name 'ScenarioTag' -DefaultValue '') -eq 'vakuu-fight' }).Count
    Add-Check -Name 'summary_vakuu_fight_iteration_count_matches_results' -Passed ([int](Get-JsonValue -Object $summary -Name 'VakuuFightIterationCount' -DefaultValue -1) -eq $summaryVakuuFightCount) -Detail "VakuuFightIterationCount must match summary Results; expected $summaryVakuuFightCount"
    $failureReasonCountsValue = Get-JsonValue -Object $summary -Name 'FailureReasonCounts' -DefaultValue $null
    $failureReasonCountProperties = @()
    if ($null -ne $failureReasonCountsValue) {
        $failureReasonCountProperties = @($failureReasonCountsValue.PSObject.Properties)
    }

    Add-Check -Name 'summary_failure_reason_counts_empty' -Passed ($failureReasonCountProperties.Count -eq 0) -Detail 'FailureReasonCounts must be empty for a clean packet'

    if ($expectedIterationCount -gt 0) {
        Add-Check -Name 'summary_requested_iterations_match_expected' -Passed ($summaryRequestedIterations -eq $expectedIterationCount) -Detail "summary RequestedIterations expected $expectedIterationCount, found $summaryRequestedIterations"
        Add-Check -Name 'summary_completed_iterations_match_expected' -Passed ($summaryCompletedIterations -eq $expectedIterationCount) -Detail "summary CompletedIterations expected $expectedIterationCount, found $summaryCompletedIterations"
        Add-Check -Name 'summary_result_count_matches_expected' -Passed ($summaryResults.Count -eq $expectedIterationCount) -Detail "summary Results expected $expectedIterationCount entries, found $($summaryResults.Count)"
    }

    $failedSummaryResults = @($summaryResults | Where-Object { -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false) })
    Add-Check -Name 'summary_results_all_passed' -Passed ($failedSummaryResults.Count -eq 0) -Detail "summary Results contains $($failedSummaryResults.Count) failed entries"
}

$iterationDirectories = @(Get-ChildItem -LiteralPath $resolvedEvidenceDir -Directory -Filter 'iteration-*' | Sort-Object -Property Name)
if ($expectedIterationCount -gt 0) {
    Add-Check -Name 'iteration_directory_count_matches_expected' -Passed ($iterationDirectories.Count -eq $expectedIterationCount) -Detail "expected $expectedIterationCount iteration-* directories, found $($iterationDirectories.Count)"
} else {
    Add-Check -Name 'expected_iterations_resolved' -Passed $false -Detail 'could not resolve expected iteration count from -ExpectedIterations, monkey-plan.json, or monkey-summary.json'
}

for ($iteration = 1; $iteration -le $expectedIterationCount; $iteration++) {
    $iterationName = 'iteration-{0:D4}' -f $iteration
    $iterationDir = Resolve-EvidenceFile $iterationName
    $iterationDirExists = Test-Path -LiteralPath $iterationDir -PathType Container
    Add-Check -Name "${iterationName}_directory_exists" -Passed $iterationDirExists -Detail "requires $iterationName directory"

    if (-not $iterationDirExists) {
        continue
    }

    $resultPath = Join-Path $iterationDir 'iteration-result.json'
    $logPath = Join-Path $iterationDir 'godot.log.after-launch'
    $auditPath = Join-Path $iterationDir 'godot-log-audit.json'
    $sts1ModeCheckPath = Join-Path $iterationDir 'sts1-mode-log-check.json'
    $resultExists = Test-Path -LiteralPath $resultPath -PathType Leaf
    $logExists = Test-Path -LiteralPath $logPath -PathType Leaf
    $auditExists = Test-Path -LiteralPath $auditPath -PathType Leaf
    $sts1ModeCheckExists = Test-Path -LiteralPath $sts1ModeCheckPath -PathType Leaf

    Add-Check -Name "${iterationName}_iteration_result_exists" -Passed $resultExists -Detail 'requires iteration-result.json'
    Add-Check -Name "${iterationName}_godot_log_exists" -Passed $logExists -Detail 'requires godot.log.after-launch'
    Add-Check -Name "${iterationName}_audit_json_exists" -Passed $auditExists -Detail 'requires godot-log-audit.json'
    Add-Check -Name "${iterationName}_sts1_mode_log_check_exists" -Passed $sts1ModeCheckExists -Detail 'requires retained sts1-mode-log-check.json'

    $iterationResult = $null
    if ($resultExists) {
        $iterationResult = Read-JsonOrNull -Path $resultPath -CheckName "${iterationName}_iteration_result_json_valid"
        if ($null -ne $iterationResult) {
            Add-Check -Name "${iterationName}_iteration_result_json_valid" -Passed $true -Detail 'iteration-result.json parsed'
            Add-Check -Name "${iterationName}_hang_probe_schema_version" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'iteration HangProbeSchemaVersion must be 1'
            Add-Check -Name "${iterationName}_scenario_tag_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'ScenarioTag' -DefaultValue ''))) -Detail 'ScenarioTag must be retained for triage'
            Add-Check -Name "${iterationName}_owner_area_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'OwnerArea' -DefaultValue ''))) -Detail 'OwnerArea must be retained for triage'
            Add-Check -Name "${iterationName}_command_selection_mode_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'CommandSelectionMode' -DefaultValue ''))) -Detail 'CommandSelectionMode must be retained for triage'
            Add-Check -Name "${iterationName}_main_menu_reached" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainMenuReached' -DefaultValue $false)) -Detail 'MainMenuReached must be true; false means main-menu timeout or launch failure'
            Add-Check -Name "${iterationName}_main_menu_observation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainMenuObservationPassed' -DefaultValue $false)) -Detail 'MainMenuObservationPassed must be true'
            Add-Check -Name "${iterationName}_runtime_observation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'RuntimeObservationPassed' -DefaultValue $false)) -Detail 'RuntimeObservationPassed must be true'
            Add-Check -Name "${iterationName}_startup_log_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'StartupLogProbePassed' -DefaultValue $false)) -Detail 'StartupLogProbePassed must be true'
            Add-Check -Name "${iterationName}_post_command_log_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'PostCommandLogProbePassed' -DefaultValue $false)) -Detail 'PostCommandLogProbePassed must be true'
            Add-Check -Name "${iterationName}_responsiveness_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'ResponsivenessProbePassed' -DefaultValue $false)) -Detail 'ResponsivenessProbePassed must be true'
            Add-Check -Name "${iterationName}_command_ack_observed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckObserved' -DefaultValue $false)) -Detail 'required command acknowledgement must be observed when applicable'
            $failureReasonCodesCount = Get-ArrayCount -Value (Get-JsonValue -Object $iterationResult -Name 'FailureReasonCodes' -DefaultValue @())
            $hangSignalsCount = Get-ArrayCount -Value (Get-JsonValue -Object $iterationResult -Name 'HangSignals' -DefaultValue @())
            Add-Check -Name "${iterationName}_failure_reason_codes_empty" -Passed ($failureReasonCodesCount -eq 0) -Detail 'FailureReasonCodes must be empty for a clean packet'
            Add-Check -Name "${iterationName}_hang_signals_empty" -Passed ($hangSignalsCount -eq 0) -Detail 'HangSignals must be empty for a clean packet'
            Add-Check -Name "${iterationName}_game_process_id_positive" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'GameProcessId' -DefaultValue 0) -gt 0) -Detail 'GameProcessId must identify SlayTheSpire2'
            Add-Check -Name "${iterationName}_main_window_observed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainWindowObserved' -DefaultValue $false)) -Detail 'MainWindowObserved must be true'
            Add-Check -Name "${iterationName}_main_menu_elapsed_recorded" -Passed ([double](Get-JsonValue -Object $iterationResult -Name 'MainMenuElapsedSeconds' -DefaultValue 0) -gt 0) -Detail 'MainMenuElapsedSeconds must be positive'
            Add-Check -Name "${iterationName}_max_no_log_growth_recorded" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'MaxSecondsWithoutLogGrowth' -DefaultValue -1) -ge 0) -Detail 'MaxSecondsWithoutLogGrowth must be recorded'
            $iterationMaxUnresponsive = [int](Get-JsonValue -Object $iterationResult -Name 'MaxConsecutiveUnresponsiveSamples' -DefaultValue -1)
            Add-Check -Name "${iterationName}_max_consecutive_unresponsive_recorded" -Passed ($iterationMaxUnresponsive -ge 0) -Detail 'MaxConsecutiveUnresponsiveSamples must be recorded'
            if ($planUnresponsiveSampleThreshold -gt 0) {
                Add-Check -Name "${iterationName}_max_consecutive_unresponsive_below_threshold" -Passed ($iterationMaxUnresponsive -lt $planUnresponsiveSampleThreshold) -Detail "MaxConsecutiveUnresponsiveSamples must stay below threshold $planUnresponsiveSampleThreshold; found $iterationMaxUnresponsive"
            }
            Add-Check -Name "${iterationName}_result_log_copied" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LogCopied' -DefaultValue $false)) -Detail 'LogCopied must be true'
            Add-Check -Name "${iterationName}_result_audit_clean" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'AuditClean' -DefaultValue $false)) -Detail 'AuditClean must be true'
            Add-Check -Name "${iterationName}_result_expectation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'ExpectationPassed' -DefaultValue $false)) -Detail 'ExpectationPassed must be true'
            Add-Check -Name "${iterationName}_result_sts1_mode_verifier_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'Sts1ModeVerifierPassed' -DefaultValue $false)) -Detail 'Sts1ModeVerifierPassed must be true'
            Add-Check -Name "${iterationName}_restore_succeeded" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'RestoreSucceeded' -DefaultValue $false)) -Detail 'RestoreSucceeded must be true'
            Add-Check -Name "${iterationName}_iteration_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'Passed' -DefaultValue $false)) -Detail 'Passed must be true'

            $probeSamplesPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'RuntimeProbeSamplesPath' -DefaultValue ''))
            $probeSamplesExist = $probeSamplesPath -and (Test-Path -LiteralPath $probeSamplesPath -PathType Leaf)
            $probeSamplesUnderIteration = $probeSamplesPath -and (Test-PathUnderDirectory -Path $probeSamplesPath -Directory $iterationDir)
            $probeSamplesLeafExpected = $probeSamplesPath -and ([System.IO.Path]::GetFileName($probeSamplesPath) -eq 'runtime-probe-samples.json')
            Add-Check -Name "${iterationName}_runtime_probe_samples_under_iteration_dir" -Passed $probeSamplesUnderIteration -Detail 'RuntimeProbeSamplesPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_runtime_probe_samples_leaf_expected" -Passed $probeSamplesLeafExpected -Detail 'RuntimeProbeSamplesPath must end with runtime-probe-samples.json'
            Add-Check -Name "${iterationName}_runtime_probe_samples_exist" -Passed $probeSamplesExist -Detail 'requires retained runtime-probe-samples.json'
            if ($probeSamplesExist) {
                try {
                    $probeSamplesJson = [System.IO.File]::ReadAllText($probeSamplesPath)
                    $probeSamplesParsed = $probeSamplesJson | ConvertFrom-Json
                    $probeSamples = @($probeSamplesParsed)
                    Add-Check -Name "${iterationName}_runtime_probe_samples_non_empty" -Passed ($probeSamples.Count -gt 0) -Detail 'runtime-probe-samples.json must contain samples'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_observed" -Passed (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'ProcessObserved') -Detail 'at least one probe sample must observe SlayTheSpire2'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_hung_window" -Passed (Test-NoJsonPropertyTrue -Items $probeSamples -Name 'HungWindow') -Detail 'probe samples must not report hung windows'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_not_responding" -Passed (Test-NoJsonPropertyFalse -Items $probeSamples -Name 'Responding') -Detail 'probe samples must not report Responding=false'
                } catch {
                    Add-Check -Name "${iterationName}_runtime_probe_samples_json_valid" -Passed $false -Detail "invalid probe samples JSON in $probeSamplesPath`: $($_.Exception.Message)"
                }
            }

            $mainMenuObservation = Get-JsonValue -Object $iterationResult -Name 'MainMenuObservation' -DefaultValue $null
            Add-Check -Name "${iterationName}_main_menu_observation_exists" -Passed ($null -ne $mainMenuObservation) -Detail 'requires MainMenuObservation telemetry'
            if ($null -ne $mainMenuObservation) {
                Add-Check -Name "${iterationName}_main_menu_observation_main_menu_reached" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'MainMenuReached' -DefaultValue $false)) -Detail 'MainMenuObservation.MainMenuReached must be true'
                Add-Check -Name "${iterationName}_main_menu_observation_process_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.ProcessObserved must be true'
                Add-Check -Name "${iterationName}_main_menu_observation_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'godot.log must not stall before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_log_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.LogObserved must be true'
            }

            $runtimeObservation = Get-JsonValue -Object $iterationResult -Name 'RuntimeObservation' -DefaultValue $null
            Add-Check -Name "${iterationName}_runtime_observation_exists" -Passed ($null -ne $runtimeObservation) -Detail 'requires RuntimeObservation telemetry'
            if ($null -ne $runtimeObservation) {
                Add-Check -Name "${iterationName}_runtime_observation_passed_detail" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'Passed' -DefaultValue $false)) -Detail 'RuntimeObservation.Passed must be true'
                Add-Check -Name "${iterationName}_runtime_observation_process_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.ProcessObserved must be true'
                Add-Check -Name "${iterationName}_runtime_observation_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear during runtime observation'
                Add-Check -Name "${iterationName}_runtime_observation_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung during runtime observation'
                Add-Check -Name "${iterationName}_runtime_observation_log_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.LogObserved must be true'
            }
        }
    }

    $logText = ''
    if ($logExists) {
        $logItem = Get-Item -LiteralPath $logPath
        $logText = [System.IO.File]::ReadAllText($logPath)
        Add-Check -Name "${iterationName}_godot_log_non_empty" -Passed ($logItem.Length -gt 0 -and $logText.Length -gt 0) -Detail 'godot.log.after-launch must be non-empty'
        Add-Check -Name "${iterationName}_main_menu_log_line_present" -Passed ([regex]::IsMatch($logText, '\[Startup\] Time to main menu')) -Detail 'expected [Startup] Time to main menu in copied log'

        if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
            Add-Check -Name "${iterationName}_expected_package_version_in_log" -Passed (Contains-Text -Text $logText -Needle $ExpectedPackageVersion) -Detail "expected package version '$ExpectedPackageVersion' in copied log"
        }

        if ($ExpectedPatchCount -gt 0) {
            $patchHits = Get-PatchCountLineHits -Text $logText -ExpectedCount $ExpectedPatchCount
            Add-Check -Name "${iterationName}_expected_patch_count_in_log" -Passed ($patchHits -gt 0) -Detail "expected Spire Plus patch-count markers for $ExpectedPatchCount applied and $ExpectedPatchCount registered patches in copied log"
        }
    }

    if ($auditExists) {
        try {
            $auditSummary = Read-AuditSummary -Path $auditPath
            Add-Check -Name "${iterationName}_audit_clean" -Passed ([bool]$auditSummary.Clean) -Detail "audit must have zero dirty items and zero signature hits; dirty=$($auditSummary.DirtyItems), hits=$($auditSummary.SignatureHitCount)"
        } catch {
            Add-Check -Name "${iterationName}_audit_json_valid" -Passed $false -Detail "invalid audit JSON in $auditPath`: $($_.Exception.Message)"
        }
    }

    if ($sts1ModeCheckExists) {
        $sts1ModeCheck = Read-JsonOrNull -Path $sts1ModeCheckPath -CheckName "${iterationName}_sts1_mode_log_check_json_valid"
        if ($null -ne $sts1ModeCheck) {
            Add-Check -Name "${iterationName}_sts1_mode_log_check_json_valid" -Passed $true -Detail 'sts1-mode-log-check.json parsed'
            $sts1Mismatches = @((Get-JsonValue -Object $sts1ModeCheck -Name 'Mismatches' -DefaultValue @()))
            $sts1FailedChecks = @((Get-JsonValue -Object $sts1ModeCheck -Name 'Checks' -DefaultValue @()) | Where-Object {
                -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false)
            })
            Add-Check -Name "${iterationName}_sts1_mode_log_check_mismatches_empty" -Passed ($sts1Mismatches.Count -eq 0) -Detail "sts1-mode-log-check.json must have zero mismatches; found $($sts1Mismatches.Count)"
            Add-Check -Name "${iterationName}_sts1_mode_log_check_all_checks_passed" -Passed ($sts1FailedChecks.Count -eq 0) -Detail "sts1-mode-log-check.json contains $($sts1FailedChecks.Count) failed checks"
        }
    }
}

$report = [pscustomobject]@{
    EvidenceDir = $resolvedEvidenceDir
    ExpectedIterations = $expectedIterationCount
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedPatchCount = $ExpectedPatchCount
    Checks = $checks
    Mismatches = $mismatches
}

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

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
