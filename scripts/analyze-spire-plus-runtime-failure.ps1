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

function Test-Sha256Text {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[A-Fa-f0-9]{64}$'
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
        [int]$DefaultIteration = 0,
        [bool]$RunResultPathInsideEvidenceDir = $true
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
    if ($result -and -not $isGameNativeAutoSlay -and -not $isDirectSmoke) {
        $runtimeMonkeyRequiredArtifacts = @(
            [pscustomobject]@{ Label = 'godot.log.before'; OutsideSignal = 'runtime_monkey_before_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_before_log_not_retained_file'; Path = $beforeLogCandidate; CanonicalPath = $canonicalBeforeLogCandidate; NextStep = 'Retain godot.log.before as the standard file in the iteration directory before using runtime-monkey log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.after-launch'; OutsideSignal = 'runtime_monkey_after_launch_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_after_launch_log_not_retained_file'; Path = $fullLogCandidate; CanonicalPath = $canonicalFullLogCandidate; NextStep = 'Retain godot.log.after-launch as the standard file in the iteration directory before using runtime-monkey log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.current-iteration'; OutsideSignal = 'runtime_monkey_current_iteration_log_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_current_iteration_log_not_retained_file'; Path = $currentIterationLogCandidate; CanonicalPath = $canonicalCurrentIterationLogCandidate; NextStep = 'Retain godot.log.current-iteration as the standard file in the iteration directory before using runtime-monkey log lines for owner routing.' },
            [pscustomobject]@{ Label = 'runtime-probe-samples.json'; OutsideSignal = 'runtime_monkey_runtime_probe_samples_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_runtime_probe_samples_not_retained_file'; Path = $probeSamplesCandidate; CanonicalPath = $canonicalProbeSamplesCandidate; NextStep = 'Retain runtime-probe-samples.json as the standard file in the iteration directory before using runtime-monkey probe telemetry for triage.' },
            [pscustomobject]@{ Label = 'session-state.json'; FieldName = 'LiveSessionSessionStatePath'; HashField = 'LiveSessionSessionStateSha256'; MissingPathSignal = 'runtime_monkey_session_state_path_missing'; MissingFileSignal = 'runtime_monkey_session_state_missing'; MissingHashSignal = 'runtime_monkey_session_state_hash_missing'; HashMismatchSignal = 'runtime_monkey_session_state_hash_mismatch'; OutsideSignal = 'runtime_monkey_session_state_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_session_state_not_retained_file'; Path = $sessionStateCandidate; CanonicalPath = $canonicalSessionStateCandidate; NextStep = 'Retain session-state.json as the standard file in the iteration directory before trusting live-session restore transaction evidence.' },
            [pscustomobject]@{ Label = 'restore-state.json'; FieldName = 'LiveSessionRestoreStatePath'; HashField = 'LiveSessionRestoreStateSha256'; MissingPathSignal = 'runtime_monkey_restore_state_path_missing'; MissingFileSignal = 'runtime_monkey_restore_state_missing'; MissingHashSignal = 'runtime_monkey_restore_state_hash_missing'; HashMismatchSignal = 'runtime_monkey_restore_state_hash_mismatch'; OutsideSignal = 'runtime_monkey_restore_state_outside_iteration_dir'; NonCanonicalSignal = 'runtime_monkey_restore_state_not_retained_file'; Path = $restoreStateCandidate; CanonicalPath = $canonicalRestoreStateCandidate; NextStep = 'Retain restore-state.json as the standard file in the iteration directory before trusting live-session restore transaction evidence.' }
        )

        foreach ($artifact in $runtimeMonkeyRequiredArtifacts) {
            $artifactPath = [string]$artifact.Path
            $artifactFieldName = if (Test-JsonProperty -Object $artifact -Name 'FieldName') { [string]$artifact.FieldName } else { '' }
            $artifactHashField = if (Test-JsonProperty -Object $artifact -Name 'HashField') { [string]$artifact.HashField } else { '' }
            if (-not [string]::IsNullOrWhiteSpace($artifactFieldName) -and
                (-not (Test-JsonProperty -Object $result -Name $artifactFieldName) -or [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $result -Name $artifactFieldName -DefaultValue '')))) {
                $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                Add-Finding -Findings $findings -Signal ([string]$artifact.MissingPathSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey result JSON did not retain $artifactFieldName for $($artifact.Label)." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                continue
            }
            if ([string]::IsNullOrWhiteSpace($artifactPath)) {
                $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
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
                if ([string]::Equals([string]$artifact.Label, 'runtime-probe-samples.json', [System.StringComparison]::Ordinal)) {
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
                    Add-Finding -Findings $findings -Signal ([string]$artifact.MissingFileSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $($artifact.Label) path did not point to an existing retained file." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
                    continue
                }

                $recordedArtifactSha256 = [string](Get-JsonValue -Object $result -Name $artifactHashField -DefaultValue '')
                if (-not (Test-JsonProperty -Object $result -Name $artifactHashField) -or -not (Test-Sha256Text -Value $recordedArtifactSha256)) {
                    $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    Add-Finding -Findings $findings -Signal ([string]$artifact.MissingHashSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey result JSON did not retain a valid $artifactHashField for $($artifact.Label)." -NextStep 'Record SHA256 bindings for live-session state files before trusting restore evidence or routing gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    continue
                }

                $actualArtifactSha256 = Get-FileSha256OrEmpty -Path $artifactPath
                if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($recordedArtifactSha256, $actualArtifactSha256)) {
                    $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                    Add-Finding -Findings $findings -Signal ([string]$artifact.HashMismatchSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey $artifactHashField does not match retained $($artifact.Label); recorded=$recordedArtifactSha256 actual=$actualArtifactSha256." -NextStep 'Regenerate or reject the packet; do not route ownership from live-session state files whose retained hashes have drifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                }
            }
        }
    }
    if ($isGameNativeAutoSlay -and $result) {
        $autoSlayRunArtifactsTrustedForOwner = $RunResultPathInsideEvidenceDir
        $autoSlayProbeArtifactTrustedForOwner = $true
        $autoSlayAuditArtifactTrustedForOwner = $true
        $autoSlaySts1ModeArtifactTrustedForOwner = $true
        $autoSlaySidecarPathTrustedForOwner = $true
        if (-not $RunResultPathInsideEvidenceDir) {
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_path_outside_evidence_dir' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'GameNativeAutoSlay autoslay-summary.json RunResultPath resolved outside the retained evidence directory.' -NextStep 'Retain each run-result.json under the AutoSlay evidence root before analyzing per-seed artifacts or routing source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        $autoSlayRequiredArtifacts = @(
            [pscustomobject]@{ Label = 'godot.log.before'; FieldName = 'GodotLogBeforePath'; Signal = 'autoslay_before_log_outside_run_dir'; MissingSignal = 'autoslay_before_log_path_missing'; NonCanonicalSignal = 'autoslay_before_log_not_retained_file'; Path = $beforeLogCandidate; CanonicalPath = $canonicalBeforeLogCandidate; NextStep = 'Retain godot.log.before beside run-result.json before using AutoSlay log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.after-launch'; FieldName = 'GodotLogAfterLaunchPath'; Signal = 'autoslay_after_launch_log_outside_run_dir'; MissingSignal = 'autoslay_after_launch_log_path_missing'; NonCanonicalSignal = 'autoslay_after_launch_log_not_retained_file'; Path = $fullLogCandidate; CanonicalPath = $canonicalFullLogCandidate; NextStep = 'Retain godot.log.after-launch beside run-result.json before using AutoSlay log slices for owner routing.' },
            [pscustomobject]@{ Label = 'godot.log.current-iteration'; FieldName = 'GodotLogCurrentIterationPath'; Signal = 'autoslay_current_iteration_log_outside_run_dir'; MissingSignal = 'autoslay_current_iteration_log_path_missing'; NonCanonicalSignal = 'autoslay_current_iteration_log_not_retained_file'; Path = $currentIterationLogCandidate; CanonicalPath = $canonicalCurrentIterationLogCandidate; NextStep = 'Retain godot.log.current-iteration beside run-result.json before using AutoSlay log lines for owner routing.' },
            [pscustomobject]@{ Label = 'runtime-probe-samples.json'; FieldName = 'RuntimeProbeSamplesPath'; Signal = 'autoslay_runtime_probe_samples_outside_run_dir'; MissingSignal = 'autoslay_runtime_probe_samples_path_missing'; NonCanonicalSignal = 'autoslay_runtime_probe_samples_not_retained_file'; Path = $probeSamplesCandidate; CanonicalPath = $canonicalProbeSamplesCandidate; NextStep = 'Retain runtime-probe-samples.json beside run-result.json before using AutoSlay probe telemetry for triage.' },
            [pscustomobject]@{ Label = 'godot-log-audit.json'; FieldName = 'GodotLogAuditPath'; Signal = 'autoslay_godot_log_audit_outside_run_dir'; MissingSignal = 'autoslay_godot_log_audit_path_missing'; NonCanonicalSignal = 'autoslay_godot_log_audit_not_retained_file'; Path = $auditCandidate; CanonicalPath = Join-Path $Directory 'godot-log-audit.json'; NextStep = 'Retain godot-log-audit.json beside run-result.json before using audit signatures for owner routing.' },
            [pscustomobject]@{ Label = 'sts1-mode-log-check.json'; FieldName = 'Sts1ModeLogCheckPath'; Signal = 'autoslay_sts1_mode_log_check_outside_run_dir'; MissingSignal = 'autoslay_sts1_mode_log_check_path_missing'; NonCanonicalSignal = 'autoslay_sts1_mode_log_check_not_retained_file'; Path = $sts1ModeCandidate; CanonicalPath = Join-Path $Directory 'sts1-mode-log-check.json'; NextStep = 'Retain sts1-mode-log-check.json beside run-result.json before using StS1 mode evidence for owner routing.' },
            [pscustomobject]@{ Label = 'autoslay.log'; FieldName = 'AutoSlayLogPath'; Signal = 'autoslay_sidecar_log_outside_run_dir'; MissingSignal = 'autoslay_sidecar_log_path_missing'; NonCanonicalSignal = 'autoslay_sidecar_log_not_retained_file'; Path = $autoSlayLogCandidate; CanonicalPath = Join-Path $Directory 'autoslay.log'; NextStep = 'Retain autoslay.log beside run-result.json before using sidecar log lines for owner routing.' }
        )

        foreach ($artifact in $autoSlayRequiredArtifacts) {
            $artifactPath = [string]$artifact.Path
            $artifactFieldRetained = Test-JsonProperty -Object $result -Name ([string]$artifact.FieldName)
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
                }
                if ([string]::Equals([string]$artifact.Label, 'autoslay.log', [System.StringComparison]::Ordinal)) {
                    $autoSlaySidecarPathTrustedForOwner = $false
                }

                Add-Finding -Findings $findings -Signal ([string]$artifact.NonCanonicalSignal) -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay $($artifact.Label) did not resolve to the retained standard per-seed file." -NextStep ([string]$artifact.NextStep) -Confidence 'high' -EvidenceFiles $evidenceFiles
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
                $logScanOffset = [long](Get-JsonValue -Object $result -Name 'LogScanOffsetBytes' -DefaultValue -1)
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
                    $recordedLength = [long](Get-JsonValue -Object $result -Name $metadataCheck.LengthField -DefaultValue -1)
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

        $startTimestampText = if ($result) { [string](Get-JsonValue -Object $result -Name 'StartTimestamp' -DefaultValue '') } else { '' }
        $endTimestampText = if ($result) { [string](Get-JsonValue -Object $result -Name 'EndTimestamp' -DefaultValue '') } else { '' }
        $startTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $startTimestampText
        $endTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $endTimestampText
        if (-not [bool]$startTimestampParse.Parsed) {
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_start_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json must retain a parseable StartTimestamp; found '$startTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp retention before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if (-not [bool]$endTimestampParse.Parsed) {
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_end_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json must retain a parseable EndTimestamp; found '$endTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp retention before classifying gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if ([bool]$startTimestampParse.Parsed -and [bool]$endTimestampParse.Parsed -and $startTimestampParse.Value -gt $endTimestampParse.Value) {
            Add-Finding -Findings $findings -Signal 'autoslay_run_result_timestamp_order_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "GameNativeAutoSlay run-result.json has StartTimestamp later than EndTimestamp; start='$startTimestampText' end='$endTimestampText'." -NextStep 'Fix AutoSlay run-result timestamp capture before using duration or ownership routing from this packet.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        }

        if ([string]::IsNullOrWhiteSpace($probeSamplesCandidate) -or -not (Test-Path -LiteralPath $probeSamplesCandidate -PathType Leaf)) {
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
                        $logExists = [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false)
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
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_main_menu_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never retained a main-menu phase sample.' -NextStep 'Fix AutoSlay probe sampling so startup and runtime phases are both represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    if (-not (Test-AnyJsonPropertyStringEquals -Items $probeSamples -Name 'Phase' -Value 'runtime')) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_runtime_phase_missing' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples never retained a runtime phase sample.' -NextStep 'Fix AutoSlay probe sampling so startup and runtime phases are both represented before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

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
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_stale_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw stale SlayTheSpire2 processes, so shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing clients and recapture the packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $unknownStartTimeSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
                    if ($unknownStartTimeSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_unknown_start_time_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw SlayTheSpire2 processes with unreadable StartTime, so current-run attribution is ambiguous.' -NextStep 'Recapture with no unreadable SlayTheSpire2 processes before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $ambiguousCurrentProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
                    if ($ambiguousCurrentProcessSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_ambiguous_current_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples saw multiple current SlayTheSpire2 processes, so shared log and PID evidence are ambiguous.' -NextStep 'Close overlapping clients and recapture the AutoSlay packet after the validation pause is lifted.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $currentProcessCountSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
                    if ($currentProcessCountSamples.Count -gt 0) {
                        $autoSlayRunArtifactsTrustedForOwner = $false
                        $autoSlayProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_current_process_count_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime probe samples did not consistently bind to exactly one current SlayTheSpire2 process.' -NextStep 'Fix process selection and contamination rejection before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $observedProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProcessStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProcessPaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedExpectedProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedExpectedProcessStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedExpectedProcessPaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $identityMismatchProbeSamples = @($probeSamples | Where-Object {
                        [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                        (-not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
                    })
                    if ($observedProcessIds.Count -ne 1) {
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_process_identity_unstable' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime probe samples must bind to exactly one positive process id; observed count=$($observedProcessIds.Count)." -NextStep 'Fix AutoSlay process selection and stale-process rejection before trusting this packet.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    } elseif ($result) {
                        $resultProcessId = [int](Get-JsonValue -Object $result -Name 'ProcessId' -DefaultValue 0)
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
                    $runtimeObservationLogGrew = $null -ne $probeRuntimeObservation -and [bool](Get-JsonValue -Object $probeRuntimeObservation -Name 'LogGrew' -DefaultValue $false)
                    $runtimeObservationInitialLogLength = if ($null -ne $probeRuntimeObservation) { [long](Get-JsonValue -Object $probeRuntimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1) } else { -1L }
                    $runtimeObservationFinalLogLength = if ($null -ne $probeRuntimeObservation) { [long](Get-JsonValue -Object $probeRuntimeObservation -Name 'LogFinalLengthBytes' -DefaultValue -1) } else { -1L }
                    $runtimeProbeLogLengths = @($probeSamples |
                        Where-Object {
                            [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'runtime', [System.StringComparison]::Ordinal) -and
                            [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        } |
                        ForEach-Object { [long](Get-JsonValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1) } |
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
                        Add-Finding -Findings $findings -Signal 'autoslay_runtime_probe_log_growth_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "RuntimeObservation.LogGrew=true is not backed by retained runtime sample LogLengthBytes; initial=$runtimeObservationInitialLogLength final=$runtimeObservationFinalLogLength maxRuntimeSample=$runtimeProbeMaxLogLength." -NextStep 'Regenerate the AutoSlay packet with runtime probe samples whose log-length timeline proves the runtime log growth claim.' -Confidence 'high' -EvidenceFiles $evidenceFiles
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
            -RequiredTrueFields @('Passed', 'ProcessObserved', 'LogObserved', 'LogGrew') `
            -RequiredFalseFields @('ProcessExitedAfterObservation', 'HungWindowDetected', 'StaleProcessObserved', 'NoLogGrowthTimeoutExceeded') `
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
                    $mainMenuObservationSampleCount = if ($null -ne $mainMenuObservation) { [int](Get-JsonValue -Object $mainMenuObservation -Name 'Samples' -DefaultValue -1) } else { -1 }
                    $runtimeObservationSampleCount = if ($null -ne $runtimeObservation) { [int](Get-JsonValue -Object $runtimeObservation -Name 'Samples' -DefaultValue -1) } else { -1 }

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
                        $logExists = [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        $logLastWriteParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'LogLastWriteTimeUtc' -DefaultValue ''))
                        (-not [bool]$sampledAtParse.Parsed) -or
                            ($logExists -and -not [bool]$logLastWriteParse.Parsed) -or
                            ($logExists -and [bool]$sampledAtParse.Parsed -and [bool]$logLastWriteParse.Parsed -and $logLastWriteParse.Value -gt $sampledAtParse.Value)
                    })
                    if ($invalidTimestampProbeSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_timestamp_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "Runtime monkey runtime-probe-samples.json has invalid probe timestamps; invalidCount=$($invalidTimestampProbeSamples.Count)." -NextStep 'Regenerate runtime-probe-samples.json with parseable SampledAt values, parseable LogLastWriteTimeUtc values when LogExists=true, and no log write time later than the sample timestamp.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $staleProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
                    if ($staleProcessSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_stale_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw stale SlayTheSpire2 processes, so shared godot.log evidence may be contaminated.' -NextStep 'Close pre-existing game clients and recapture the packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $unknownStartTimeSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
                    if ($unknownStartTimeSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_unknown_start_time_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw SlayTheSpire2 processes with unreadable StartTime, so current-run attribution is ambiguous.' -NextStep 'Recapture with no unreadable SlayTheSpire2 processes before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $ambiguousCurrentProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
                    if ($ambiguousCurrentProcessSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_ambiguous_current_process' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples saw multiple current SlayTheSpire2 processes, so shared log and PID evidence are ambiguous.' -NextStep 'Close overlapping clients and recapture the runtime monkey packet after validation lanes are unpaused.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $currentProcessCountSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
                    if ($currentProcessCountSamples.Count -gt 0) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        $runtimeMonkeyRunArtifactsTrustedForOwner = $false
                        $runtimeMonkeyProbeArtifactTrustedForOwner = $false
                        $logTextTrustedForOwner = $false
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_current_process_count_invalid' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'Runtime monkey probe samples did not consistently bind to exactly one current SlayTheSpire2 process.' -NextStep 'Fix process selection and contamination rejection before routing this packet to gameplay source.' -Confidence 'high' -EvidenceFiles $evidenceFiles
                    }

                    $resultGameProcessId = [int](Get-JsonValue -Object $result -Name 'GameProcessId' -DefaultValue 0)
                    $resultGameProcessStartParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $result -Name 'GameProcessStartTimeUtc' -DefaultValue ''))
                    $resultGameProcessStartTime = if ([bool]$resultGameProcessStartParse.Parsed) { $resultGameProcessStartParse.Value.ToString('o') } else { '' }
                    $resultGameProcessPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $result -Name 'GameProcessPath' -DefaultValue ''))
                    $resultLiveSessionSelectedGameProcessId = [int](Get-JsonValue -Object $result -Name 'LiveSessionSelectedGameProcessId' -DefaultValue 0)
                    $resultLiveSessionStartParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $result -Name 'LiveSessionSelectedGameProcessStartTimeUtc' -DefaultValue ''))
                    $resultLiveSessionStartTime = if ([bool]$resultLiveSessionStartParse.Parsed) { $resultLiveSessionStartParse.Value.ToString('o') } else { '' }
                    $resultLiveSessionPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $result -Name 'LiveSessionSelectedGameProcessPath' -DefaultValue ''))
                    $observedProbeProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProbeStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbePaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbeExpectedProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProbeExpectedStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object {
                            $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue ''))
                            if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                        } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $observedProbeExpectedPaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                        Sort-Object -Unique)
                    $identityMismatchProbeSamples = @($probeSamples | Where-Object {
                        [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                        (-not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
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

                    $runtimeObservationLogGrowthRequired = if ($null -ne $runtimeObservation) { [bool](Get-JsonValue -Object $runtimeObservation -Name 'RuntimeLogGrowthRequired' -DefaultValue $false) } else { $false }
                    $runtimeObservationLogGrew = if ($null -ne $runtimeObservation) { [bool](Get-JsonValue -Object $runtimeObservation -Name 'LogGrew' -DefaultValue $false) } else { $false }
                    $runtimeObservationInitialLogLength = if ($null -ne $runtimeObservation) { [long](Get-JsonValue -Object $runtimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1) } else { -1L }
                    $postCommandRuntimeProbeLogLengths = @($postCommandRuntimeProbeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false) } |
                        ForEach-Object { [long](Get-JsonValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1) } |
                        Where-Object { $_ -ge 0 })
                    $postCommandRuntimeProbeMaxLogLength = if ($postCommandRuntimeProbeLogLengths.Count -gt 0) {
                        [long](@($postCommandRuntimeProbeLogLengths | Sort-Object -Descending)[0])
                    } else {
                        -1L
                    }
                    if ($runtimeObservationLogGrowthRequired -and $runtimeObservationLogGrew -and
                        ($runtimeObservationInitialLogLength -lt 0 -or $postCommandRuntimeProbeMaxLogLength -le $runtimeObservationInitialLogLength)) {
                        $runtimeMonkeyProbeEvidenceInvalid = $true
                        Add-Finding -Findings $findings -Signal 'runtime_monkey_probe_runtime_log_growth_mismatch' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale "RuntimeObservation.LogGrew=true is not backed by retained PostCommandRuntime sample LogLengthBytes; initial=$runtimeObservationInitialLogLength maxRuntimeSample=$postCommandRuntimeProbeMaxLogLength." -NextStep 'Regenerate the packet with runtime probe samples whose log-length timeline proves the post-command log growth claim.' -Confidence 'high' -EvidenceFiles $evidenceFiles
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

    if ($isDirectSmoke -and $result) {
        $directSmokePassed = [bool](Get-JsonValue -Object $result -Name 'Passed' -DefaultValue $false)
        $directSmokeAuditClean = [bool](Get-JsonValue -Object $result -Name 'AuditClean' -DefaultValue $false)
        $directSmokeModeVerifierMismatches = [int](Get-JsonValue -Object $result -Name 'ModeVerifierMismatches' -DefaultValue 0)
        $directSmokePacketVerifierMismatches = [int](Get-JsonValue -Object $result -Name 'PacketVerifierMismatches' -DefaultValue 0)
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
                        if ($autoSlayRunArtifactsTrustedForOwner -and $autoSlayAuditArtifactTrustedForOwner -and $runtimeMonkeyRunArtifactsTrustedForOwner) {
                            $auditTrustedForOwner = $true
                            $auditHits = Get-AuditHits -AuditItems @($recomputedAudit)
                        }
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
            'runtime_log_stalled' {
                Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'godot.log stopped growing during runtime observation.' -NextStep 'Inspect RuntimeObservation, runtime-probe-samples.json, and the current-iteration log before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
            }
            'process_unresponsive' {
                if ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner) {
                    Add-Finding -Findings $findings -Signal $signal -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The window was reported hung or not responding, but runtime monkey run/probe evidence is missing, invalid, or not byte-bound to retained files.' -NextStep 'Fix runtime monkey artifact retention and probe/sample binding before assigning gameplay ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
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

            if ((-not $isGameNativeAutoSlay -or $autoSlaySts1ModeArtifactTrustedForOwner) -and
                -not [string]::IsNullOrWhiteSpace($sts1ModeCandidate) -and
                (Test-Path -LiteralPath $sts1ModeCandidate -PathType Leaf)) {
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

    if ($logTextTrustedForOwner -and $logText -match '(?i)\bcoop_[a-z0-9_]*override_enabled\b') {
        $owner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea -PreferLog
        Add-Finding -Findings $findings -Signal 'coop_override_enabled_runtime_failure' -Severity 'blocking' -OwnerArea $owner -Rationale 'A co-op unsafe/debug override appears near a runtime failure.' -NextStep 'Treat this as deliberate unsafe two-client debugging; route by feature text and preserve both host/client logs.' -Confidence 'medium' -EvidenceFiles $evidenceFiles
    }

    if ($logTextTrustedForOwner -and $logText -match '(?i)coop_local_ui_preview_enabled|prediction_prepared_multiplayer_ui_only') {
        Add-Finding -Findings $findings -Signal 'coop_preview_ui_only_observed' -Severity 'info' -OwnerArea 'PreviewTools' -Rationale 'The log shows preview tools running as local UI only in multiplayer.' -NextStep 'This supports preview-tool co-op policy, but still does not prove two-client behavior without live evidence.' -Confidence 'high' -EvidenceFiles $evidenceFiles
    }

    if ($command -match '(?i)spireplus_test_ancient\s+VAKUU' -and (@($hangSignals).Count -gt 0 -or @($failureCodes).Count -gt 0)) {
        if ($runtimeMonkeyProbeEvidenceInvalid -or -not $runtimeMonkeyRunArtifactsTrustedForOwner) {
            Add-Finding -Findings $findings -Signal 'vakuu_command_failed_or_hung' -Severity 'blocking' -OwnerArea 'RuntimeHarness' -Rationale 'The failing iteration targeted Vakuu through the live-test command, but runtime monkey run/probe evidence is missing, invalid, or not byte-bound to retained files.' -NextStep 'Fix runtime monkey artifact retention and probe/sample binding before assigning Vakuu source ownership.' -Confidence 'high' -EvidenceFiles $evidenceFiles
        } else {
            $vakuuOwner = Resolve-OwnerArea -PlannedOwnerArea $resultOwnerArea -LogOwnerArea $logOwnerArea -CommandOwnerArea $commandOwnerArea
            Add-Finding -Findings $findings -Signal 'vakuu_command_failed_or_hung' -Severity 'blocking' -OwnerArea $vakuuOwner -Rationale 'The failing iteration targeted Vakuu through the live-test command.' -NextStep (Get-NextStepForOwner -OwnerArea $vakuuOwner -Signal 'vakuu_command_failed_or_hung') -Confidence 'medium' -EvidenceFiles $evidenceFiles
        }
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
        RuntimeMonkeyRunArtifactsTrustedForOwner = $runtimeMonkeyRunArtifactsTrustedForOwner
        AutoSlaySidecarTrustedForOwner = $autoSlaySidecarTrustedForOwner
        AutoSlayRunArtifactsTrustedForOwner = $autoSlayRunArtifactsTrustedForOwner
        AutoSlayProbeArtifactTrustedForOwner = $autoSlayProbeArtifactTrustedForOwner
        AutoSlayAuditArtifactTrustedForOwner = $autoSlayAuditArtifactTrustedForOwner
        AutoSlaySts1ModeArtifactTrustedForOwner = $autoSlaySts1ModeArtifactTrustedForOwner
        OwnerAreaFromCommand = $commandOwnerArea
        Signals = @($signals)
        EvidenceFiles = @($evidenceFiles)
        FailureReasonCodes = @($failureCodes.ToArray())
        HangSignals = @($hangSignals.ToArray())
        AuditTrustedForOwner = $auditTrustedForOwner
        AuditHits = @($auditHits.ToArray())
        BaseLibPatchFailures = @($baseLibPatchFailures)
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

        if ($iterationDirs.Count -eq 0 -and
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
    Analyze-Iteration -Directory $target.Directory -SummaryResult $target.SummaryResult -ResultFileName $target.ResultFileName -DefaultIteration $target.DefaultIteration -RunResultPathInsideEvidenceDir $runResultPathInsideEvidenceDir
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
