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

function Get-JsonArrayValues {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $items = [System.Collections.Generic.List[object]]::new()
    if (-not (Test-JsonProperty -Object $Object -Name $Name)) {
        return ,$items
    }

    $value = $Object.$Name
    if ($null -eq $value) {
        return ,$items
    }

    if ($value -is [System.Array]) {
        foreach ($item in $value) {
            $items.Add($item) | Out-Null
        }
    } else {
        $items.Add($value) | Out-Null
    }

    return ,$items
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

    if ($Text -match '(?i)\b(TypeLoadException|MissingMethodException|MissingFieldException|BaseLib patch failure|Creature\.get_ShowsInfiniteHp|runtime expectation|source drift|package drift)\b') {
        return 'PackageRuntimeDrift'
    }

    if ($Text -match '(?i)\b(StS1|Sts1|Golden Idol|Big Fish|The Cleric|AdditiveBatch1|CanaryOnly|registered-event|Registered act event|Registered shared event|sts1-mode-log-check)\b') {
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
    param([string]$Path)

    $hits = [System.Collections.Generic.List[object]]::new()
    $audit = Read-JsonOrNull -Path $Path
    if ($null -eq $audit) {
        return ,$hits
    }

    foreach ($item in @($audit)) {
        foreach ($hit in (Get-JsonArrayValues -Object $item -Name 'SignatureHits')) {
            if ([int](Get-JsonValue -Object $hit -Name 'Count' -DefaultValue 0) -gt 0) {
                $hits.Add([pscustomobject]@{
                    Name = [string](Get-JsonValue -Object $hit -Name 'Name' -DefaultValue '')
                    Count = [int](Get-JsonValue -Object $hit -Name 'Count' -DefaultValue 0)
                }) | Out-Null
            }
        }
    }

    return ,$hits
}

function Analyze-Iteration {
    param(
        [Parameter(Mandatory = $true)][string]$Directory,
        [AllowNull()]$SummaryResult
    )

    $resultPath = Join-Path $Directory 'iteration-result.json'
    $fullLogCandidate = Join-Path $Directory 'godot.log.after-launch'
    $currentIterationLogCandidate = Join-Path $Directory 'godot.log.current-iteration'
    $logCandidate = if (Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf) {
        $currentIterationLogCandidate
    } else {
        $fullLogCandidate
    }
    $auditCandidate = Join-Path $Directory 'godot-log-audit.json'
    $probeSamplesCandidate = Join-Path $Directory 'runtime-probe-samples.json'
    $sts1ModeCandidate = Join-Path $Directory 'sts1-mode-log-check.json'
    $result = Read-JsonOrNull -Path $resultPath
    $iterationResultMissing = $null -eq $result
    if ($null -eq $result -and $null -ne $SummaryResult) {
        $result = $SummaryResult
    }

    $command = if ($result) { [string](Get-JsonValue -Object $result -Name 'Command' -DefaultValue '') } else { '' }
    $resultOwnerArea = if ($result) { [string](Get-JsonValue -Object $result -Name 'OwnerArea' -DefaultValue '') } else { '' }
    $scenarioTag = if ($result) { [string](Get-JsonValue -Object $result -Name 'ScenarioTag' -DefaultValue '') } else { '' }
    $findings = [System.Collections.Generic.List[object]]::new()
    $candidateEvidenceFiles = @(
        $resultPath,
        $currentIterationLogCandidate,
        $fullLogCandidate,
        $auditCandidate,
        $probeSamplesCandidate,
        $sts1ModeCandidate
    )
    $evidenceFiles = @($candidateEvidenceFiles | Where-Object {
        Test-Path -LiteralPath $_ -PathType Leaf
    })
    $logText = ''
    $currentIterationLogExists = Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf
    $fullLogExists = Test-Path -LiteralPath $fullLogCandidate -PathType Leaf
    if (Test-Path -LiteralPath $logCandidate -PathType Leaf) {
        $logText = Get-Content -LiteralPath $logCandidate -Raw -Encoding UTF8
    }

    if ($result -and $currentIterationLogExists -and $fullLogExists -and (Test-JsonProperty -Object $result -Name 'LogScanOffsetBytes')) {
        $logScanOffset = [long](Get-JsonValue -Object $result -Name 'LogScanOffsetBytes' -DefaultValue -1)
        $fullLogLength = [long](Get-Item -LiteralPath $fullLogCandidate).Length
        if ($logScanOffset -lt 0 -or $logScanOffset -gt $fullLogLength) {
            $logText = ''
            Add-Finding `
                -Findings $findings `
                -Signal 'current_iteration_log_scan_offset_invalid' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale "LogScanOffsetBytes is outside godot.log.after-launch; offset=$logScanOffset, length=$fullLogLength." `
                -NextStep 'Fix current-iteration log slicing or evidence retention before routing this runtime failure to gameplay source.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            $expectedCurrentIterationLogText = Read-TextAfterByteOffset -Path $fullLogCandidate -Offset $logScanOffset
            $actualCurrentIterationLogText = [System.IO.File]::ReadAllText($currentIterationLogCandidate)
            $normalizedExpectedSlice = Normalize-LogSliceForComparison -Text $expectedCurrentIterationLogText
            $normalizedActualSlice = Normalize-LogSliceForComparison -Text $actualCurrentIterationLogText
            $logText = $expectedCurrentIterationLogText
            if (-not [string]::Equals($normalizedActualSlice, $normalizedExpectedSlice, [System.StringComparison]::Ordinal)) {
                Add-Finding `
                    -Findings $findings `
                    -Signal 'current_iteration_log_slice_mismatch' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'godot.log.current-iteration does not match godot.log.after-launch from LogScanOffsetBytes, so the retained slice may be stale or hand-assembled.' `
                    -NextStep 'Use the derived full-log slice from LogScanOffsetBytes for source routing, then fix current-iteration log retention before trusting packet evidence.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            }
        }
    }

    $logOwnerArea = Get-OwnerAreaFromText -Text $logText -Command ''
    $commandOwnerArea = Get-OwnerAreaFromText -Text '' -Command $command

    $auditExists = Test-Path -LiteralPath $auditCandidate -PathType Leaf
    $auditJsonValid = (-not $auditExists) -or (Test-JsonFileParses -Path $auditCandidate)
    $auditHits = if ($auditExists) { Get-AuditHits -Path $auditCandidate } else { [System.Collections.Generic.List[object]]::new() }
    $failureCodes = if ($result) { Get-JsonArrayValues -Object $result -Name 'FailureReasonCodes' } else { [System.Collections.Generic.List[object]]::new() }
    $hangSignals = if ($result) { Get-JsonArrayValues -Object $result -Name 'HangSignals' } else { [System.Collections.Generic.List[object]]::new() }

    if ($iterationResultMissing) {
        $missingResultRationale = if ($null -ne $SummaryResult) {
            'iteration-result.json is missing or could not be parsed. monkey-summary.json provided a fallback row for routing, but it is not the canonical per-iteration evidence artifact.'
        } else {
            'iteration-result.json is missing or could not be parsed, and monkey-summary.json did not provide a usable iteration result.'
        }

        Add-Finding `
            -Findings $findings `
            -Signal 'iteration_result_missing_or_invalid' `
            -Severity 'blocking' `
            -OwnerArea 'RuntimeHarness' `
            -Rationale $missingResultRationale `
            -NextStep 'Fix evidence retention or rerun the packet after validation lanes are unpaused; do not classify gameplay behavior from an incomplete iteration packet.' `
            -Confidence 'high' `
            -EvidenceFiles @($resultPath, $logCandidate, $auditCandidate, $probeSamplesCandidate, $sts1ModeCandidate)
    }

    if ($auditExists -and -not $auditJsonValid) {
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

    $retainedSignals = @(($hangSignals.ToArray() + $failureCodes.ToArray()) | Select-Object -Unique)
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
            'process_unresponsive' {
                $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea $owner -Rationale 'The window was reported hung or not responding during observation.' -NextStep (Get-NextStepForOwner -OwnerArea $owner -Signal $signal) -Confidence 'medium' -EvidenceFiles $evidenceFiles
            }
            'command_ack_missing' {
                $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea $owner -Rationale 'The command was sent but the expected source-backed acknowledgement line was absent.' -NextStep 'Verify foreground/DevConsole input delivery first; if input landed, inspect the target command handler and its preconditions.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
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
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'Sts1Events' -Rationale 'The retained StS1 mode verifier did not match requested mode/source shape.' -NextStep 'Open sts1-mode-log-check.json and compare actual mode, registration count, event class set, and environment propagation.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'restore_failed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'LiveSessionRestore' -Rationale 'The live-session helper failed to restore settings/mods/current runs.' -NextStep 'Inspect restore-state/session-state and fix restore safety before another live run.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            default {
                $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
                Add-Finding -Findings $findings -Signal ([string]$signal) -Severity 'blocking' -OwnerArea $owner -Rationale 'Unclassified retained failure code from iteration-result.json.' -NextStep (Get-NextStepForOwner -OwnerArea $owner -Signal ([string]$signal)) -Confidence 'low' -EvidenceFiles $evidenceFiles
            }
        }
    }

    foreach ($hit in $auditHits) {
        $name = [string]$hit.Name
        $auditOwnerText = Get-AuditOwnerText -LogText $logText -AuditName $name
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

    if (Test-Path -LiteralPath $sts1ModeCandidate -PathType Leaf) {
        $sts1Report = Read-JsonOrNull -Path $sts1ModeCandidate
        $sts1Mismatches = if ($sts1Report) { Get-JsonArrayValues -Object $sts1Report -Name 'Mismatches' } else { [System.Collections.Generic.List[object]]::new() }
        if ($sts1Mismatches.Count -gt 0) {
            Add-Finding `
                -Findings $findings `
                -Signal 'sts1_mode_log_check_mismatch' `
                -Severity 'blocking' `
                -OwnerArea 'Sts1Events' `
                -Rationale "sts1-mode-log-check.json contains $($sts1Mismatches.Count) mismatches." `
                -NextStep 'Classify this as environment propagation if the log shows Off/default mode; otherwise inspect registration count, class set, and tuple expectations.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        }
    }

    if ($logText -match '(?i)coop_gameplay_disabled|coop_combat_hook_disabled') {
        Add-Finding -Findings $findings -Signal 'coop_fail_closed_observed' -Severity 'info' -OwnerArea 'MultiplayerPolicy' -Rationale 'The log shows co-op gameplay/combat hooks failing closed.' -NextStep 'Treat as expected only when no explicit SPIREPLUS_ALLOW_UNVERIFIED_COOP_* debug gate was intended.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($logText -match '(?i)coop_.*override_enabled|ALLOW_UNVERIFIED_COOP') {
        $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
        Add-Finding -Findings $findings -Signal 'coop_override_enabled_runtime_failure' -Severity 'blocking' -OwnerArea $owner -Rationale 'A co-op unsafe/debug override appears near a runtime failure.' -NextStep 'Treat this as deliberate unsafe two-client debugging; route by feature text and preserve both host/client logs.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
    }

    if ($logText -match '(?i)coop_local_ui_preview_enabled|prediction_prepared_multiplayer_ui_only') {
        Add-Finding -Findings $findings -Signal 'coop_preview_ui_only_observed' -Severity 'info' -OwnerArea 'PreviewTools' -Rationale 'The log shows preview tools running as local UI only in multiplayer.' -NextStep 'This supports preview-tool co-op policy, but still does not prove two-client behavior without live evidence.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($command -match '(?i)spireplus_test_ancient\s+VAKUU' -and (@($hangSignals).Count -gt 0 -or @($failureCodes).Count -gt 0)) {
        $vakuuOwner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea
        Add-Finding -Findings $findings -Signal 'vakuu_command_failed_or_hung' -Severity 'blocking' -OwnerArea $vakuuOwner -Rationale 'The failing iteration targeted Vakuu through the live-test command.' -NextStep (Get-NextStepForOwner -OwnerArea $vakuuOwner -Signal 'vakuu_command_failed_or_hung') -Confidence 'medium' -EvidenceFiles $evidenceFiles
    }

    $signals = @(
        $failureCodes.ToArray() +
        $hangSignals.ToArray() +
        @($auditHits.ToArray() | ForEach-Object { "audit:$($_.Name)" }) +
        @($findings | ForEach-Object { $_.Signal })
    ) | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } | Select-Object -Unique

    [pscustomobject]@{
        IterationDir = $Directory
        Iteration = if ($result) { [int](Get-JsonValue -Object $result -Name 'Iteration' -DefaultValue 0) } else { 0 }
        Passed = if ($result) { [bool](Get-JsonValue -Object $result -Name 'Passed' -DefaultValue $false) } else { $false }
        Command = $command
        ScenarioTag = $scenarioTag
        OwnerAreaHint = $resultOwnerArea
        OwnerAreaFromLog = $logOwnerArea
        OwnerAreaFromCommand = $commandOwnerArea
        Signals = @($signals)
        EvidenceFiles = @($evidenceFiles)
        FailureReasonCodes = @($failureCodes.ToArray())
        HangSignals = @($hangSignals.ToArray())
        AuditHits = @($auditHits.ToArray())
        Findings = @($findings)
    }
}

$iterationDirs = @()
$summary = $null
$summaryResultsByIteration = @{}
$evidenceFull = ''

if ($IterationDir) {
    $iterationDirs = @(Resolve-RepoPath -Path $IterationDir)
} elseif ($EvidenceDir) {
    $evidenceFull = Resolve-RepoPath -Path $EvidenceDir
    $summaryPath = Join-Path $evidenceFull 'monkey-summary.json'
    $summary = Read-JsonOrNull -Path $summaryPath
    if ($summary -and (Test-JsonProperty -Object $summary -Name 'Results')) {
        foreach ($result in @($summary.Results)) {
            $summaryResultsByIteration[[int](Get-JsonValue -Object $result -Name 'Iteration' -DefaultValue 0)] = $result
        }
    }

    if ($Iteration -gt 0) {
        $iterationDirs = @(Join-Path $evidenceFull ('iteration-{0:D4}' -f $Iteration))
    } elseif ($summary -and (Test-JsonProperty -Object $summary -Name 'FailedIterationIds') -and @($summary.FailedIterationIds).Count -gt 0) {
        $iterationDirs = @($summary.FailedIterationIds | ForEach-Object {
            Join-Path $evidenceFull ('iteration-{0:D4}' -f [int]$_)
        })
    } else {
        $iterationDirs = @(Get-ChildItem -LiteralPath $evidenceFull -Directory -Filter 'iteration-*' -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object { $_.FullName })
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

    $iterationDirs = @($tempDir)
} else {
    throw 'Pass -EvidenceDir, -IterationDir, or -LogPath/-AuditPath.'
}

$iterationReports = foreach ($dir in $iterationDirs) {
    $iterationNumber = if ($dir -match 'iteration-(\d+)$') { [int]$Matches[1] } else { 0 }
    $summaryResult = if ($summaryResultsByIteration.ContainsKey($iterationNumber)) { $summaryResultsByIteration[$iterationNumber] } else { $null }
    Analyze-Iteration -Directory $dir -SummaryResult $summaryResult
}

$allFindings = @($iterationReports | ForEach-Object { @($_.Findings) })
$blockingFindings = @($allFindings | Where-Object { [string]$_.Severity -eq 'blocking' })
$ownerAreas = @($allFindings | ForEach-Object { $_.OwnerArea } | Where-Object { $_ } | Select-Object -Unique)

$report = [pscustomobject]@{
    SchemaVersion = 1
    CreatedAt = (Get-Date).ToString('o')
    EvidenceDir = $evidenceFull
    AnalyzedIterationCount = @($iterationReports).Count
    BlockingFindingCount = @($blockingFindings).Count
    OwnerAreas = @($ownerAreas)
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
