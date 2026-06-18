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

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
}

function Resolve-AnalysisPath {
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
}

function Test-BytePrefix {
    param(
        [Parameter(Mandatory = $true)][byte[]]$Prefix,
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
        return ,$failures
    }

    foreach ($field in $RequiredTrueFields) {
        if (-not [bool](Get-JsonValue -Object $Observation -Name $field -DefaultValue $false)) {
            $failures.Add($field) | Out-Null
        }
    }

    foreach ($field in $RequiredFalseFields) {
        if ([bool](Get-JsonValue -Object $Observation -Name $field -DefaultValue $true)) {
            $failures.Add($field) | Out-Null
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($ZeroCountField)) {
        try {
            if ([int](Get-JsonValue -Object $Observation -Name $ZeroCountField -DefaultValue -1) -ne 0) {
                $failures.Add($ZeroCountField) | Out-Null
            }
        } catch {
            $failures.Add($ZeroCountField) | Out-Null
        }
    }

    return ,$failures
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
    param([AllowEmptyCollection()][object[]]$AuditItems)

    $hits = [System.Collections.Generic.List[object]]::new()
    foreach ($item in @($AuditItems)) {
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

function ConvertTo-AuditSummary {
    param([AllowNull()]$Audit)

    $items = @($Audit)
    $dirtyItems = 0
    $hitCount = 0
    $itemPaths = [System.Collections.Generic.List[string]]::new()
    $itemLengths = [System.Collections.Generic.List[long]]::new()
    $itemSha256s = [System.Collections.Generic.List[string]]::new()

    foreach ($item in $items) {
        if (-not (Test-JsonProperty -Object $item -Name 'Clean') -or -not [bool]$item.Clean) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Path') -and -not [string]::IsNullOrWhiteSpace([string]$item.Path)) {
            $itemPaths.Add([System.IO.Path]::GetFullPath([string]$item.Path)) | Out-Null
        }

        if (Test-JsonProperty -Object $item -Name 'Length') {
            $itemLengths.Add([long]$item.Length) | Out-Null
        }

        if ((Test-JsonProperty -Object $item -Name 'Sha256') -and -not [string]::IsNullOrWhiteSpace([string]$item.Sha256)) {
            $itemSha256s.Add(([string]$item.Sha256).ToLowerInvariant()) | Out-Null
        }

        foreach ($hit in (Get-JsonArrayValues -Object $item -Name 'SignatureHits')) {
            if (Test-JsonProperty -Object $hit -Name 'Count') {
                $hitCount += [int]$hit.Count
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
        [int]$DefaultIteration = 0
    )

    $resultPath = Join-Path $Directory $ResultFileName
    $result = Read-JsonOrNull -Path $resultPath
    $iterationResultMissing = $null -eq $result
    if ($null -eq $result -and $null -ne $SummaryResult) {
        $result = $SummaryResult
    }

    $runnerKind = if ($result) { [string](Get-JsonValue -Object $result -Name 'RunnerKind' -DefaultValue '') } else { '' }
    $isGameNativeAutoSlay = [string]::Equals($runnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)
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
    }

    $beforeLogCandidate = Join-Path $Directory 'godot.log.before'
    $fullLogCandidate = Join-Path $Directory 'godot.log.after-launch'
    $currentIterationLogCandidate = Join-Path $Directory 'godot.log.current-iteration'
    $auditCandidate = Join-Path $Directory 'godot-log-audit.json'
    $probeSamplesCandidate = Join-Path $Directory 'runtime-probe-samples.json'
    $sts1ModeCandidate = Join-Path $Directory 'sts1-mode-log-check.json'
    if ($isGameNativeAutoSlay -and $result) {
        $beforeLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogBeforePath' -DefaultValue 'godot.log.before'))
        $fullLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogAfterLaunchPath' -DefaultValue 'godot.log.after-launch'))
        $currentIterationLogCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogCurrentIterationPath' -DefaultValue 'godot.log.current-iteration'))
        $auditCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'GodotLogAuditPath' -DefaultValue 'godot-log-audit.json'))
        $probeSamplesCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'RuntimeProbeSamplesPath' -DefaultValue 'runtime-probe-samples.json'))
        $sts1ModeCandidate = Resolve-AnalysisPath -BaseDir $Directory -Path ([string](Get-JsonValue -Object $result -Name 'Sts1ModeLogCheckPath' -DefaultValue 'sts1-mode-log-check.json'))
    }

    $logCandidate = if (Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf) {
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
        $sts1ModeCandidate,
        $autoSlayLogCandidate
    )
    $evidenceFiles = @($candidateEvidenceFiles | Where-Object {
        Test-Path -LiteralPath $_ -PathType Leaf
    })
    $currentIterationLogExists = Test-Path -LiteralPath $currentIterationLogCandidate -PathType Leaf
    $beforeLogExists = Test-Path -LiteralPath $beforeLogCandidate -PathType Leaf
    $fullLogExists = Test-Path -LiteralPath $fullLogCandidate -PathType Leaf
    $autoSlayLogExists = Test-Path -LiteralPath $autoSlayLogCandidate -PathType Leaf
    $autoSlayLogText = if ($autoSlayLogExists) { Get-Content -LiteralPath $autoSlayLogCandidate -Raw -Encoding UTF8 } else { '' }
    $logText = ''
    $logTextTrustedForOwner = $false
    if ($result -and $currentIterationLogExists) {
        if ($isGameNativeAutoSlay) {
            if (-not ($beforeLogExists -and $fullLogExists)) {
                Add-Finding `
                    -Findings $findings `
                    -Signal 'current_iteration_log_before_after_binding_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'GameNativeAutoSlay evidence has godot.log.current-iteration without both godot.log.before and godot.log.after-launch, so the retained current slice may be stale or hand-assembled.' `
                    -NextStep 'Fix AutoSlay before/after/current log retention or rerun the packet after validation lanes are unpaused; do not route ownership from an unbound current-iteration slice.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            } else {
                $sliceBinding = Test-CurrentSliceFromBeforeAfter -BeforePath $beforeLogCandidate -AfterPath $fullLogCandidate -CurrentPath $currentIterationLogCandidate
                $logText = [System.IO.File]::ReadAllText($currentIterationLogCandidate)
                $logTextTrustedForOwner = [bool]$sliceBinding.SliceMatches
                if (-not [bool]$sliceBinding.SliceMatches) {
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'current_iteration_log_slice_mismatch' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale $sliceBinding.Detail `
                        -NextStep 'Use only byte-bound current-iteration slices for AutoSlay source routing, then fix evidence retention before trusting packet evidence.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                }
            }
        } else {
            $hasOffsetBinding = $fullLogExists -and (Test-JsonProperty -Object $result -Name 'LogScanOffsetBytes')
            if (-not $hasOffsetBinding) {
                Add-Finding `
                    -Findings $findings `
                    -Signal 'current_iteration_log_offset_binding_missing' `
                    -Severity 'blocking' `
                    -OwnerArea 'RuntimeHarness' `
                    -Rationale 'godot.log.current-iteration exists without both godot.log.after-launch and LogScanOffsetBytes, so the retained current slice may be stale or hand-assembled.' `
                    -NextStep 'Fix current-iteration log offset binding or rerun the packet after validation lanes are unpaused; do not route ownership from an unbound current-iteration slice.' `
                    -Confidence 'high' `
                    -EvidenceFiles $evidenceFiles
            } else {
                $logScanOffset = [long](Get-JsonValue -Object $result -Name 'LogScanOffsetBytes' -DefaultValue -1)
                $fullLogLength = [long](Get-Item -LiteralPath $fullLogCandidate).Length
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
                } else {
                    $expectedCurrentIterationLogText = Read-TextAfterByteOffset -Path $fullLogCandidate -Offset $logScanOffset
                    $actualCurrentIterationLogText = [System.IO.File]::ReadAllText($currentIterationLogCandidate)
                    $normalizedExpectedSlice = Normalize-LogSliceForComparison -Text $expectedCurrentIterationLogText
                    $normalizedActualSlice = Normalize-LogSliceForComparison -Text $actualCurrentIterationLogText
                    $logText = $expectedCurrentIterationLogText
                    $logTextTrustedForOwner = $true
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
        }
    } elseif (Test-Path -LiteralPath $logCandidate -PathType Leaf) {
        $logText = Get-Content -LiteralPath $logCandidate -Raw -Encoding UTF8
    }

    if ($result -and -not $currentIterationLogExists -and $fullLogExists) {
        $logText = Get-Content -LiteralPath $fullLogCandidate -Raw -Encoding UTF8
    }

    if ($isGameNativeAutoSlay) {
        if ([string]::IsNullOrWhiteSpace($seed)) {
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

        if (-not (Test-Path -LiteralPath $probeSamplesCandidate -PathType Leaf)) {
            Add-Finding `
                -Findings $findings `
                -Signal 'autoslay_runtime_probe_samples_missing' `
                -Severity 'blocking' `
                -OwnerArea 'RuntimeHarness' `
                -Rationale 'GameNativeAutoSlay evidence did not retain runtime-probe-samples.json for this seed.' `
                -NextStep 'Fix AutoSlay process/window/log sampling retention before routing this packet to gameplay source.' `
                -Confidence 'high' `
                -EvidenceFiles $evidenceFiles
        } else {
            try {
                $probeSamplesParsed = Get-Content -LiteralPath $probeSamplesCandidate -Raw -Encoding UTF8 | ConvertFrom-Json
                $probeSamples = @($probeSamplesParsed)
                $requiredProbeFields = @('Phase', 'ProcessId', 'ProcessObserved', 'MainWindowObserved', 'HungWindow', 'Responding', 'StaleProcessCount')

                if ($probeSamples.Count -eq 0) {
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'autoslay_runtime_probe_samples_empty' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale 'GameNativeAutoSlay runtime-probe-samples.json has no process/window/log samples.' `
                        -NextStep 'Retain the sampled process/window/log timeline before classifying gameplay source.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                } elseif (-not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Phase') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessId') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessObserved') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'MainWindowObserved') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'HungWindow') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Responding') -or
                    -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'StaleProcessCount')) {
                    $missingProbeFields = @($requiredProbeFields | Where-Object {
                        -not (Test-AllJsonPropertiesPresent -Items $probeSamples -Name $_)
                    })
                    Add-Finding `
                        -Findings $findings `
                        -Signal 'autoslay_runtime_probe_samples_incomplete' `
                        -Severity 'blocking' `
                        -OwnerArea 'RuntimeHarness' `
                        -Rationale "GameNativeAutoSlay runtime-probe-samples.json is missing required fields: $($missingProbeFields -join ', ')." `
                        -NextStep 'Record Phase, ProcessId, ProcessObserved, MainWindowObserved, HungWindow, Responding, and StaleProcessCount for every probe sample.' `
                        -Confidence 'high' `
                        -EvidenceFiles $evidenceFiles
                } else {
                    if (-not (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'ProcessObserved')) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never observed the SlayTheSpire2 process.' -NextStep 'Fix process selection before routing this AutoSlay packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'MainWindowObserved')) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_main_window_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never observed the main game window.' -NextStep 'Fix process/window binding before treating the packet as runtime gameplay proof.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-NoJsonPropertyTrue -Items $probeSamples -Name 'HungWindow')) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_hung_window' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples report a hung game window.' -NextStep 'Inspect the retained runtime probe timeline and current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-NoJsonPropertyFalse -Items $probeSamples -Name 'Responding')) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_not_responding' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples include Responding=false.' -NextStep 'Inspect the retained runtime probe timeline and current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $staleProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
                    if ($staleProcessSamples.Count -gt 0) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_stale_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw stale SlayTheSpire2 processes, so shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing clients and recapture the packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $observedProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    if ($observedProcessIds.Count -ne 1) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_identity_unstable' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime probe samples must bind to exactly one positive process id; observed count=$($observedProcessIds.Count)." -NextStep 'Fix AutoSlay process selection and stale-process rejection before trusting this packet.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }
                }
            } catch {
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
            $mainMenuSignal = if ($mainMenuObservationFailures -contains 'missing') { 'autoslay_main_menu_observation_missing' } else { 'autoslay_main_menu_observation_unhealthy' }
            Add-Finding -Findings $findings -Signal $mainMenuSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay MainMenuObservation is not clean: $($mainMenuObservationFailures -join ', ')." -NextStep 'Fix main-menu process/window/log observation before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        $runtimeObservation = if ($result) { Get-JsonValue -Object $result -Name 'RuntimeObservation' -DefaultValue $null } else { $null }
        $runtimeObservationFailures = @(Get-UnhealthyObservationFields `
            -Observation $runtimeObservation `
            -RequiredTrueFields @('Passed', 'ProcessObserved', 'LogObserved') `
            -RequiredFalseFields @('ProcessExitedAfterObservation', 'HungWindowDetected', 'StaleProcessObserved') `
            -ZeroCountField 'MaxStaleProcessCount')
        if ($runtimeObservationFailures.Count -gt 0) {
            $runtimeSignal = if ($runtimeObservationFailures -contains 'missing') { 'autoslay_runtime_observation_missing' } else { 'autoslay_runtime_observation_unhealthy' }
            Add-Finding -Findings $findings -Signal $runtimeSignal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay RuntimeObservation is not clean: $($runtimeObservationFailures -join ', ')." -NextStep 'Fix runtime process/window/log observation before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if (-not $autoSlayLogExists) {
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

    $ownerLogText = if ($logTextTrustedForOwner -and $isGameNativeAutoSlay) {
        "$logText`n$autoSlayLogText"
    } elseif ($logTextTrustedForOwner) {
        $logText
    } else {
        ''
    }
    $logOwnerArea = Get-OwnerAreaFromText -Text $ownerLogText -Command ''
    $commandOwnerArea = Get-OwnerAreaFromText -Text '' -Command $command

    $auditExists = Test-Path -LiteralPath $auditCandidate -PathType Leaf
    $auditJsonValid = (-not $auditExists) -or (Test-JsonFileParses -Path $auditCandidate)
    $auditData = if ($auditExists -and $auditJsonValid) { Read-JsonOrNull -Path $auditCandidate } else { $null }
    $auditSummary = if ($null -ne $auditData) { ConvertTo-AuditSummary -Audit $auditData } else { $null }
    $auditTrustedForOwner = $false
    $auditHits = [System.Collections.Generic.List[object]]::new()
    $failureCodes = if ($result) { Get-JsonArrayValues -Object $result -Name 'FailureReasonCodes' } else { [System.Collections.Generic.List[object]]::new() }
    $hangSignals = if ($result) { Get-JsonArrayValues -Object $result -Name 'HangSignals' } else { [System.Collections.Generic.List[object]]::new() }

    if ($result -and -not $currentIterationLogExists -and -not ($failureCodes.ToArray() -contains 'current_iteration_log_missing')) {
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
                        $auditTrustedForOwner = $true
                        $auditHits = Get-AuditHits -AuditItems @($recomputedAudit)
                    } else {
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
            'stale_process_observed' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The retained probe observed a SlayTheSpire2 process that started before this iteration; shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing game clients, rerun the packet after validation lanes are unpaused, and do not route ownership from this iteration log.' -Confidence 'high' -EvidenceFiles $evidenceFiles
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

    $blockingFindingsSoFar = @($findings | Where-Object { [string]$_.Severity -eq 'blocking' }).Count
    if ($result -and
        -not [bool](Get-JsonValue -Object $result -Name 'Passed' -DefaultValue $false) -and
        $failureCodes.Count -eq 0 -and
        $hangSignals.Count -eq 0 -and
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

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_gameplay_disabled|coop_combat_hook_disabled') {
        Add-Finding -Findings $findings -Signal 'coop_fail_closed_observed' -Severity 'info' -OwnerArea 'MultiplayerPolicy' -Rationale 'The log shows co-op gameplay/combat hooks failing closed.' -NextStep 'Treat as expected only when no explicit SPIREPLUS_ALLOW_UNVERIFIED_COOP_* debug gate was intended.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_.*override_enabled|ALLOW_UNVERIFIED_COOP') {
        $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
        Add-Finding -Findings $findings -Signal 'coop_override_enabled_runtime_failure' -Severity 'blocking' -OwnerArea $owner -Rationale 'A co-op unsafe/debug override appears near a runtime failure.' -NextStep 'Treat this as deliberate unsafe two-client debugging; route by feature text and preserve both host/client logs.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
    }

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_local_ui_preview_enabled|prediction_prepared_multiplayer_ui_only') {
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
        Iteration = if ($result) { [int](Get-JsonValue -Object $result -Name 'Iteration' -DefaultValue $DefaultIteration) } else { $DefaultIteration }
        Seed = $seed
        RunnerKind = $runnerKind
        EventKind = $eventKind
        AncientId = $ancientId
        Passed = if ($result) { [bool](Get-JsonValue -Object $result -Name 'Passed' -DefaultValue $false) } else { $false }
        Command = $command
        ScenarioTag = $scenarioTag
        OwnerAreaHint = $resultOwnerArea
        OwnerAreaFromLog = $logOwnerArea
        LogTextTrustedForOwner = $logTextTrustedForOwner
        OwnerAreaFromCommand = $commandOwnerArea
        Signals = @($signals)
        EvidenceFiles = @($evidenceFiles)
        FailureReasonCodes = @($failureCodes.ToArray())
        HangSignals = @($hangSignals.ToArray())
        AuditTrustedForOwner = $auditTrustedForOwner
        AuditHits = @($auditHits.ToArray())
        Findings = @($findings)
    }
}

$analysisTargets = @()
$summary = $null
$summaryResultsByIteration = @{}
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
    $autoSlaySummary = Read-JsonOrNull -Path $autoSlaySummaryPath
    $summary = Read-JsonOrNull -Path $summaryPath
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

            $runResultPath = Resolve-AnalysisPath -BaseDir $evidenceFull -Path ([string](Get-JsonValue -Object $run -Name 'RunResultPath' -DefaultValue ('run-{0:D4}/run-result.json' -f $runIndex)))
            $runDirectory = [System.IO.Path]::GetDirectoryName($runResultPath)
            if ([string]::IsNullOrWhiteSpace($runDirectory)) {
                $runDirectory = Join-Path $evidenceFull ('run-{0:D4}' -f $runIndex)
            }

            $analysisTargets += [pscustomobject]@{
                Directory = $runDirectory
                SummaryResult = $run
                ResultFileName = [System.IO.Path]::GetFileName($runResultPath)
                DefaultIteration = $runIndex
            }
        }
    } else {
        if ($summary -and (Test-JsonProperty -Object $summary -Name 'Results')) {
            foreach ($result in @($summary.Results)) {
                $summaryResultsByIteration[[int](Get-JsonValue -Object $result -Name 'Iteration' -DefaultValue 0)] = $result
            }
        }

        $iterationDirs = @()
        if ($Iteration -gt 0) {
            $iterationDirs = @(Join-Path $evidenceFull ('iteration-{0:D4}' -f $Iteration))
        } elseif ($summary -and (Test-JsonProperty -Object $summary -Name 'FailedIterationIds') -and @($summary.FailedIterationIds).Count -gt 0) {
            $iterationDirs = @($summary.FailedIterationIds | ForEach-Object {
                Join-Path $evidenceFull ('iteration-{0:D4}' -f [int]$_)
            })
        } else {
            $iterationDirs = @(Get-ChildItem -LiteralPath $evidenceFull -Directory -Filter 'iteration-*' -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object { $_.FullName })
        }

        foreach ($dir in $iterationDirs) {
            $iterationNumber = if ($dir -match 'iteration-(\d+)$') { [int]$Matches[1] } else { 0 }
            $summaryResult = if ($summaryResultsByIteration.ContainsKey($iterationNumber)) { $summaryResultsByIteration[$iterationNumber] } else { $null }
            $analysisTargets += [pscustomobject]@{
                Directory = $dir
                SummaryResult = $summaryResult
                ResultFileName = 'iteration-result.json'
                DefaultIteration = $iterationNumber
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
    Analyze-Iteration -Directory $target.Directory -SummaryResult $target.SummaryResult -ResultFileName $target.ResultFileName -DefaultIteration $target.DefaultIteration
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
