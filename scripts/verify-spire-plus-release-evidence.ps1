param(
    [string]$EvidenceRoot = ".tools\runtime-evidence\release-ready-manual",

    [string]$ManifestPath,

    [string]$PackageSha256 = "",

    [string]$PackagePath = "",

    [int]$MinScreenshotWidth = 800,

    [int]$MinScreenshotHeight = 450,

    [switch]$WriteTemplate,

    [switch]$AllowDeferred,

    [switch]$WritePassMarker,

    [string]$PassMarkerPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'spire-plus-package-evidence.ps1')

if ([string]::IsNullOrWhiteSpace($PackagePath)) {
    $PackagePath = Get-SpirePlusPackageRelativePath -RepoRoot $repoRoot
}

if ([string]::IsNullOrWhiteSpace($PackageSha256)) {
    $defaultPackageFullPath = Resolve-SpirePlusPackagePath -RepoRoot $repoRoot -PackagePath $PackagePath
    if (Test-Path -LiteralPath $defaultPackageFullPath -PathType Leaf) {
        $PackageSha256 = Get-SpirePlusPackageSha256 -RepoRoot $repoRoot -PackagePath $PackagePath
    }
}

$requiredReleaseRows = @(
    @{ Id = 'fresh-current-package-loader-smoke'; Kind = 'loader'; Label = 'Fresh current-package loader smoke with current package hashes and clean log audit' },
    @{
        Id = 'mod-settings-current-display'
        Kind = 'clicked-ui'
        Label = 'Current Spire Plus Mod Settings list and config page proof'
        ExtraRequiredFiles = @('mod-settings-checklist.md')
    },
    @{ Id = 'ancient-ui-urda'; Kind = 'clicked-ui'; Label = 'Urda clicked Ancient UI' },
    @{ Id = 'ancient-ui-morvi'; Kind = 'clicked-ui'; Label = 'Morvi clicked Ancient UI' },
    @{ Id = 'ancient-ui-lotha'; Kind = 'clicked-ui'; Label = 'Lotha clicked Ancient UI' },
    @{ Id = 'ancient-ui-vakuu-normal'; Kind = 'clicked-ui'; Label = 'Vakuu normal clicked Ancient UI' },
    @{ Id = 'ancient-ui-vakuu-fight'; Kind = 'clicked-ui'; Label = 'Vakuu force-fight clicked Ancient UI' },
    @{
        Id = 'ancient-reward-visible-relics'
        Kind = 'gameplay'
        Label = 'Selected Ancient rewards visible as relics with readable hover tips'
        ExtraRequiredFiles = @('ancient-reward-relics-checklist.md')
    },
    @{
        Id = 'player-text-tooltip-readability'
        Kind = 'gameplay'
        Label = 'Player-facing text, tooltip, and hover readability'
        ExtraRequiredFiles = @('player-text-qa-checklist.md')
    },
    @{
        Id = 'art-resource-routing-live-preview'
        Kind = 'clicked-ui'
        Label = 'Live UI preview proves event art, map icons, run-history icons, relic art, card art, and power art are not misrouted'
        ExtraRequiredFiles = @('art-resource-routing-checklist.md')
    },
    @{
        Id = 'vakuu-victory-no-black-screen'
        Kind = 'gameplay'
        Label = 'Vakuu victory returns to the event without a black screen'
        ExtraRequiredFiles = @('vakuu-victory-checklist.md')
    },
    @{
        Id = 'vakuu-failure-death-path'
        Kind = 'gameplay'
        Label = 'Vakuu failure and death path does not softlock'
        ExtraRequiredFiles = @('vakuu-failure-death-checklist.md')
    },
    @{
        Id = 'vakuu-active-fight-save-load'
        Kind = 'save-load'
        Label = 'Vakuu active child-combat save/load'
        ExtraRequiredFiles = @('vakuu-save-load-checklist.md')
    },
    @{ Id = 'ancient-state-save-load'; Kind = 'save-load'; Label = 'Urda, Morvi, Lotha, and Ancient reward state save/load' },
    @{
        Id = 'rootblight-visual-behavior'
        Kind = 'gameplay'
        Label = 'Rootblight and Blight Sprout visual/gameplay behavior'
        ExtraRequiredFiles = @('rootblight-behavior-checklist.md')
    },
    @{ Id = 'a11-natural-route-traversal'; Kind = 'gameplay'; Label = 'Natural A11 route click traversal' },
    @{
        Id = 'ascension-selector-localization'
        Kind = 'clicked-ui'
        Label = 'A11-A20 character-select Ascension selector localization'
    },
    @{
        Id = 'a19-a20-dedicated-boss-abilities'
        Kind = 'gameplay'
        Label = 'A19/A20 dedicated boss abilities and Branded Form live combat proof'
        ExtraRequiredFiles = @('boss-ability-checklist.md')
    },
    @{ Id = 'disable-mod-gameplay'; Kind = 'gameplay'; Label = 'BaseLib-only disabled Spire Plus gameplay comparison' },
    @{
        Id = 'preview-tools-live-proof'
        Kind = 'preview-tools'
        Label = 'Live Preview tools evidence for Crystal Sphere and transform preview'
        ExtraRequiredFiles = @('preview-tools-checklist.md')
    },
    @{
        Id = 'coop-disposition'
        Kind = 'coop'
        Label = 'Two-client co-op disposition or explicit release-note deferral'
        ExtraRequiredFiles = @('coop-disposition-checklist.md')
    }
)

$invalidEvidenceNotePattern = '(?i)\b(not counted|invalid|main menu|wrong surface|covered by|not gameplay evidence|do not satisfy|does not satisfy|loader health only)\b'
$invalidChecklistCellPattern = '(?i)^\s*(pending|todo|tbd|n/?a|none|not tested|untested|unknown|skip|skipped|-+)?\s*$'

$requiredBossAbilityRows = @(
    'Ceremonial Beast',
    'The Kin',
    'Vantom',
    'Lagavulin Matriarch',
    'Soul Fysh',
    'Waterfall Giant',
    'Crab',
    'Knowledge Demon',
    'Insatiable Sandworm',
    'Aeonglass',
    'Queen',
    'Test Subject'
)

$requiredAncientRewardRows = @(
    @{ Ancient = 'Urda'; Reward = 'seedbed' },
    @{ Ancient = 'Urda'; Reward = 'humus_pact' },
    @{ Ancient = 'Urda'; Reward = 'molting' },
    @{ Ancient = 'Urda'; Reward = 'moss_map' },
    @{ Ancient = 'Urda'; Reward = 'trial_branch' },
    @{ Ancient = 'Urda'; Reward = 'shallow_root_relic' },
    @{ Ancient = 'Urda'; Reward = 'rooted_route' },
    @{ Ancient = 'Urda'; Reward = 'after_rain' },
    @{ Ancient = 'Urda'; Reward = 'root_sight' },
    @{ Ancient = 'Urda'; Reward = 'seed_bank' },
    @{ Ancient = 'Morvi'; Reward = 'forbidden_loan' },
    @{ Ancient = 'Morvi'; Reward = 'misprint_press' },
    @{ Ancient = 'Morvi'; Reward = 'red_ink_overdraft' },
    @{ Ancient = 'Morvi'; Reward = 'overdue_library' },
    @{ Ancient = 'Morvi'; Reward = 'open_book_exam' },
    @{ Ancient = 'Morvi'; Reward = 'paperstorm' },
    @{ Ancient = 'Morvi'; Reward = 'blueprint_proof' },
    @{ Ancient = 'Morvi'; Reward = 'debt_settlement' },
    @{ Ancient = 'Lotha'; Reward = 'mirror_rebuttal' },
    @{ Ancient = 'Lotha'; Reward = 'mirror_hall_echo' },
    @{ Ancient = 'Lotha'; Reward = 'presumption' },
    @{ Ancient = 'Lotha'; Reward = 'closed_court' },
    @{ Ancient = 'Lotha'; Reward = 'deferred_verdict' },
    @{ Ancient = 'Lotha'; Reward = 'death_reprieve' },
    @{ Ancient = 'Lotha'; Reward = 'single_sentence' },
    @{ Ancient = 'Lotha'; Reward = 'public_evidence' },
    @{ Ancient = 'Vakuu'; Reward = 'fight_option' },
    @{ Ancient = 'Vakuu'; Reward = 'victory_non_vakuu_choices' },
    @{ Ancient = 'Vakuu event'; Reward = 'sere_talon_pickup' },
    @{ Ancient = 'Tanx event'; Reward = 'claws_maul_transform' }
)

$requiredRootblightRows = @(
    'rootblight-start-eligibility',
    'normal-rootblight-continuity',
    'elite-single-sprout',
    'boss-two-sprouts-staggered',
    'husk-exhaust-block-timing',
    'combat-end-growth',
    'rootblight-cap-four',
    'rootblight-save-load',
    'ui-hover-art-readability'
)

$requiredArtRoutingRows = @(
    'title-home-preview',
    'urda-clicked-background',
    'morvi-clicked-background',
    'lotha-clicked-background',
    'vakuu-clicked-background',
    'map-icons',
    'run-history-icons',
    'option-relic-icons',
    'lasting-relic-icons',
    'card-art',
    'power-icons',
    'no-placeholder-or-official-art'
)

$requiredPlayerTextRows = @(
    'ascension-a11-a20',
    'firemark-and-banner',
    'boss-dedicated-abilities',
    'ancient-choice-text',
    'ancient-relic-hover',
    'cards-status-curses',
    'map-hover-stacks',
    'preview-tools-text',
    'vakuu-contracts',
    'en-zhs-key-parity'
)

$requiredVakuuVictoryRows = @(
    'fight-start-scene',
    'contract-turns',
    'locks-blood-debt',
    'victory-return',
    'non-vakuu-rewards',
    'no-black-screen',
    'log-clean'
)

$requiredVakuuFailureDeathRows = @(
    'fight-start-scene',
    'failure-path',
    'death-path',
    'room-state-after-exit',
    'no-softlock',
    'log-clean'
)

$requiredVakuuSaveLoadRows = @(
    'active-combat-save',
    'active-combat-load',
    'parent-event-state',
    'prefinished-save',
    'prefinished-load',
    'no-duplicate-heal-or-reward',
    'log-clean'
)

$requiredPreviewToolsRows = @(
    'crystal-sphere-button',
    'crystal-sphere-mask-only',
    'crystal-sphere-no-reward-claim',
    'transform-preview-visible',
    'transform-preview-matches-result',
    'transform-preview-no-state-mutation',
    'prismatic-gem-reward-hooks',
    'save-reopen-stability',
    'coop-gate-or-two-client-proof',
    'log-clean'
)

$requiredCoopRows = @(
    'coop-host-join-clean-logs',
    'coop-a11-a20-selection',
    'coop-ancients',
    'coop-root-eyes',
    'coop-rootblight',
    'coop-save-load-or-reconnect',
    'coop-preview-tools-disposition',
    'coop-release-note-disposition'
)

$requiredModSettingsRows = @(
    'base-lib-visible-enabled',
    'spire-plus-list-display-name',
    'spire-plus-config-page-current-name',
    'technical-id-compatibility',
    'legacy-mod-surfaces-absent',
    'clean-log-config-registration'
)

function Resolve-WorkspacePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Test-PathWithin {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$ChildPath
    )

    $trimChars = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $baseFull = [System.IO.Path]::GetFullPath($BasePath).TrimEnd($trimChars)
    $childFull = [System.IO.Path]::GetFullPath($ChildPath).TrimEnd($trimChars)
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    return $childFull.Equals($baseFull, $comparison) -or
        $childFull.StartsWith($baseFull + [System.IO.Path]::DirectorySeparatorChar, $comparison)
}

function Resolve-EvidenceFilePath {
    param(
        [Parameter(Mandatory = $true)][string]$EvidenceDir,
        [Parameter(Mandatory = $true)][string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $EvidenceDir $Path))
}

function Get-PropertyValue {
    param(
        [Parameter(Mandatory = $true)]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $Default = $null
    )

    if ($Object.PSObject.Properties.Name -contains $Name) {
        return $Object.$Name
    }

    return $Default
}

function Get-DefaultRequiredFiles {
    param([Parameter(Mandatory = $true)][string]$Kind)

    switch ($Kind) {
        'loader' {
            return @('command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'enabled-mods.txt')
        }
        'clicked-ui' {
            return @('command.txt', 'window-preflight.json', 'godot.log', 'godot-log-audit.json', 'route-note.md')
        }
        'save-load' {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'save-load-note.md')
        }
        'coop' {
            return @('command.txt', 'host-godot.log', 'host-godot-log-audit.json', 'client-godot.log', 'client-godot-log-audit.json', 'result-note.md')
        }
        'preview-tools' {
            return @('command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'result-note.md')
        }
        default {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'result-note.md')
        }
    }
}

function Get-RequiredRowExtraFiles {
    param([Parameter(Mandatory = $true)]$RequiredRow)

    if ($RequiredRow.ContainsKey('ExtraRequiredFiles')) {
        return @($RequiredRow.ExtraRequiredFiles)
    }

    return @()
}

function Merge-RequiredEvidenceFiles {
    param(
        [Parameter(Mandatory = $true)][string[]]$DefaultFiles,
        [object[]]$RowFiles = @()
    )

    $merged = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $DefaultFiles) {
        if (-not [string]::IsNullOrWhiteSpace($file) -and -not $merged.Contains($file)) {
            [void]$merged.Add($file)
        }
    }

    foreach ($file in $RowFiles) {
        $fileString = [string]$file
        if (-not [string]::IsNullOrWhiteSpace($fileString) -and -not $merged.Contains($fileString)) {
            [void]$merged.Add($fileString)
        }
    }

    return @($merged)
}

function Get-RequiredEvidenceFilesForRow {
    param([Parameter(Mandatory = $true)]$RequiredRow)

    return @(Merge-RequiredEvidenceFiles `
            -DefaultFiles (Get-DefaultRequiredFiles -Kind $RequiredRow.Kind) `
            -RowFiles (Get-RequiredRowExtraFiles -RequiredRow $RequiredRow))
}

function Read-CleanLogAudit {
    param([Parameter(Mandatory = $true)][string]$AuditPath)

    $audit = Get-Content -LiteralPath $AuditPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $items = @()
    if ($audit -is [System.Array]) {
        $items = @($audit)
    } else {
        $items = @($audit)
    }

    if ($items.Count -eq 0) {
        return $false
    }

    foreach ($item in $items) {
        if (-not [bool](Get-PropertyValue -Object $item -Name 'Clean' -Default $false)) {
            return $false
        }
    }

    return $true
}

function Test-PackageHashesEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$PackageHashesPath,
        [Parameter(Mandatory = $true)][string]$RowId,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$ManifestPackagePath
    )

    try {
        $packageHashes = Get-Content -LiteralPath $PackageHashesPath -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json is not valid JSON: $PackageHashesPath."
        return
    }

    $files = @(Get-PropertyValue -Object $packageHashes -Name 'Files' -Default @())
    if ($files.Count -eq 0) {
        Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json has no Files rows."
        return
    }

    $rowsByPath = @{}
    foreach ($file in $files) {
        $rowPath = [string](Get-PropertyValue -Object $file -Name 'Path' -Default '')
        if ([string]::IsNullOrWhiteSpace($rowPath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json contains a file row without Path."
            continue
        }

        $rowsByPath[$rowPath] = $file
    }

    foreach ($stalePath in @('publish\EZMicroBalance.dll', 'publish\EZMicroBalance.pck', 'publish\EZMicroBalance.json')) {
        if ($rowsByPath.ContainsKey($stalePath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json still records stale root publish artifact path: $stalePath."
        }
    }

    $expectedPaths = Get-SpirePlusPackageArtifactRelativePaths -RepoRoot $repoRoot -PackagePath $ManifestPackagePath

    foreach ($expectedPath in $expectedPaths) {
        if (-not $rowsByPath.ContainsKey($expectedPath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json is missing current package artifact row: $expectedPath."
            continue
        }

        $hashRow = $rowsByPath[$expectedPath]
        $exists = [bool](Get-PropertyValue -Object $hashRow -Name 'Exists' -Default $false)
        if (-not $exists) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json marks current package artifact missing: $expectedPath."
            continue
        }

        $artifactFull = Resolve-WorkspacePath -Path $expectedPath
        if (-not (Test-Path -LiteralPath $artifactFull -PathType Leaf)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json artifact path does not exist in workspace: $expectedPath."
            continue
        }

        $expectedHash = Get-SpirePlusFileSha256 -Path $artifactFull
        $actualHash = ([string](Get-PropertyValue -Object $hashRow -Name 'Sha256' -Default '')).ToUpperInvariant()
        if ($actualHash -ne $expectedHash) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json hash for $expectedPath is '$actualHash' but current file hash is '$expectedHash'."
        }
    }
}

function Test-PreflightForeground {
    param([Parameter(Mandatory = $true)][string]$PreflightPath)

    $preflight = Get-Content -LiteralPath $PreflightPath -Raw -Encoding UTF8 | ConvertFrom-Json
    return [bool](Get-PropertyValue -Object $preflight -Name 'SpireForeground' -Default $false)
}

function Test-PngSignature {
    param([Parameter(Mandatory = $true)][string]$Path)

    return $null -ne (Get-PngDimensions -Path $Path)
}

function Get-PngDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $stream = [System.IO.File]::OpenRead($Path)
    try {
        if ($stream.Length -lt 24) {
            return $null
        }

        $bytes = [byte[]]::new(24)
        [void]$stream.Read($bytes, 0, 24)
        $signature = [byte[]]@(137, 80, 78, 71, 13, 10, 26, 10)

        for ($index = 0; $index -lt $signature.Length; $index++) {
            if ($bytes[$index] -ne $signature[$index]) {
                return $null
            }
        }

        if ([char]$bytes[12] -ne 'I' -or [char]$bytes[13] -ne 'H' -or [char]$bytes[14] -ne 'D' -or [char]$bytes[15] -ne 'R') {
            return $null
        }

        $width =
            ([int]$bytes[16] -shl 24) -bor
            ([int]$bytes[17] -shl 16) -bor
            ([int]$bytes[18] -shl 8) -bor
            [int]$bytes[19]
        $height =
            ([int]$bytes[20] -shl 24) -bor
            ([int]$bytes[21] -shl 16) -bor
            ([int]$bytes[22] -shl 8) -bor
            [int]$bytes[23]

        return [pscustomobject]@{
            Width = $width
            Height = $height
        }
    } finally {
        $stream.Dispose()
    }
}

function Test-PngMinimumDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $dimensions = Get-PngDimensions -Path $Path
    if ($null -eq $dimensions) {
        return $false
    }

    return $dimensions.Width -ge $MinScreenshotWidth -and $dimensions.Height -ge $MinScreenshotHeight
}

function Test-ChecklistCellFilled {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $false
    }

    return $Value -notmatch $invalidChecklistCellPattern
}

function Test-ChecklistContainsTemplateInstruction {
    param(
        [Parameter(Mandatory = $true)][string]$Checklist,
        [Parameter(Mandatory = $true)][string]$ChecklistName
    )

    return $Checklist -match [regex]::Escape("Copy this file to ``$ChecklistName``") -or
        $Checklist -match [regex]::Escape("Template reference for ``$ChecklistName``")
}

function Test-BossAbilityChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'boss-ability-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md still contains the unfilled template instruction."
    }

    foreach ($boss in $requiredBossAbilityRows) {
        $bossPattern = '^\|\s*' + [regex]::Escape($boss) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $bossPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md is missing Boss row: $boss."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss does not have the expected table cells."
            continue
        }

        $liveResult = $cells[4]
        $evidenceFiles = $cells[5]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss has no filled Evidence file(s) cell."
        }
    }
}

function Test-AncientRewardRelicsChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'ancient-reward-relics-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md still contains the unfilled template instruction."
    }

    foreach ($requiredReward in $requiredAncientRewardRows) {
        $ancient = [string]$requiredReward.Ancient
        $reward = [string]$requiredReward.Reward
        $rowPattern = '^\|\s*' + [regex]::Escape($ancient) + '\s*\|\s*' + [regex]::Escape($reward) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md is missing Ancient reward row: $ancient / $reward."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 8) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward does not have the expected table cells."
            continue
        }

        $screenResult = $cells[4]
        $relicResult = $cells[5]
        $evidenceFiles = $cells[6]
        if (-not (Test-ChecklistCellFilled -Value $screenResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Screen option result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $relicResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Relic bar / hover result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Evidence file(s) cell."
        }
    }
}

function Test-RootblightBehaviorChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'rootblight-behavior-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md still contains the unfilled template instruction."
    }

    foreach ($scenario in $requiredRootblightRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($scenario) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md is missing Rootblight scenario row: $scenario."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario has no filled Evidence file(s) cell."
        }
    }
}

function Test-ArtResourceRoutingChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'art-resource-routing-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md still contains the unfilled template instruction."
    }

    foreach ($surface in $requiredArtRoutingRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($surface) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md is missing art surface row: $surface."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface has no filled Evidence file(s) cell."
        }
    }
}

function Test-PlayerTextQaChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'player-text-qa-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md still contains the unfilled template instruction."
    }

    foreach ($surface in $requiredPlayerTextRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($surface) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md is missing player-text row: $surface."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 7) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface does not have the expected table cells."
            continue
        }

        $englishResult = $cells[3]
        $chineseResult = $cells[4]
        $evidenceFiles = $cells[5]
        if (-not (Test-ChecklistCellFilled -Value $englishResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled EN result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $chineseResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled ZHS result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled Evidence file(s) cell."
        }
    }
}

function Test-SimpleChecklistRows {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [Parameter(Mandatory = $true)][string[]]$RequiredRows,
        [Parameter(Mandatory = $true)][string]$TemplateInstruction,
        [Parameter(Mandatory = $true)][string]$ChecklistName,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if ($checklist -match [regex]::Escape($TemplateInstruction) -or
        (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName $ChecklistName)) {
        Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName still contains the unfilled template instruction."
    }

    foreach ($requiredRow in $RequiredRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($requiredRow) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName is missing scenario row: $requiredRow."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow has no filled Evidence file(s) cell."
        }
    }
}

function New-TemplateManifest {
    param([Parameter(Mandatory = $true)][string]$OutputPath)

    $rows = foreach ($required in $requiredReleaseRows) {
        [ordered]@{
            Id = $required.Id
            Label = $required.Label
            Kind = $required.Kind
            Status = 'pending'
            EvidenceDir = ''
            RequiredFiles = @(Get-RequiredEvidenceFilesForRow -RequiredRow $required)
            ScreenshotFile = if ($required.Kind -eq 'clicked-ui') { '' } else { $null }
            ResultNote = ''
            ReleaseNote = ''
            ExplicitOwnerDecision = $false
        }
    }

    $template = [ordered]@{
        PackageSha256 = $PackageSha256
        PackagePath = $PackagePath
        CreatedAt = (Get-Date).ToString('o')
        Rows = @($rows)
    }

    $parent = Split-Path -Parent $OutputPath
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    $template | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

function Write-PassMarker {
    param(
        [Parameter(Mandatory = $true)][string]$OutputPath,
        [Parameter(Mandatory = $true)]$Summary
    )

    $parent = Split-Path -Parent $OutputPath
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    [ordered]@{
        Status = 'pass'
        Verifier = 'verify-spire-plus-release-evidence.ps1'
        ManifestPath = $Summary.ManifestPath
        EvidenceRoot = $Summary.EvidenceRoot
        PackageSha256 = $Summary.PackageSha256
        PackagePath = $Summary.PackagePath
        ActualPackageSha256 = $Summary.ActualPackageSha256
        CheckedAt = $Summary.CheckedAt
        AllowDeferred = $Summary.AllowDeferred
        RequiredRowCount = $Summary.RequiredRowCount
        WarningCount = $Summary.WarningCount
        Warnings = @($Summary.Warnings)
    } | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

function Add-Failure {
    param(
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Failures.Add($Message)
}

function Add-Warning {
    param(
        [System.Collections.Generic.List[string]]$Warnings,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Warnings.Add($Message)
}

$evidenceRootFull = Resolve-WorkspacePath -Path $EvidenceRoot
if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $evidenceRootFull 'release-evidence-manifest.json'
}

$manifestFull = Resolve-WorkspacePath -Path $ManifestPath
if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $manifestFull)) {
    Write-Error "ManifestPath is outside EvidenceRoot: $manifestFull."
    exit 1
}

if ([string]::IsNullOrWhiteSpace($PassMarkerPath)) {
    $PassMarkerPath = Join-Path $evidenceRootFull 'release-evidence-verifier-pass.json'
}

$passMarkerFull = Resolve-WorkspacePath -Path $PassMarkerPath
if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $passMarkerFull)) {
    Write-Error "PassMarkerPath is outside EvidenceRoot: $passMarkerFull."
    exit 1
}

if ($WriteTemplate) {
    New-TemplateManifest -OutputPath $manifestFull
    Write-Output "Wrote release evidence template: $manifestFull"
    exit 0
}

if (-not (Test-Path -LiteralPath $manifestFull)) {
    Write-Error "Missing release evidence manifest: $manifestFull. Run this script with -WriteTemplate first, then fill the evidence rows."
    exit 1
}

$manifest = Get-Content -LiteralPath $manifestFull -Raw -Encoding UTF8 | ConvertFrom-Json
$failures = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

$manifestPackageSha256 = Get-PropertyValue -Object $manifest -Name 'PackageSha256' -Default ''
if ($manifestPackageSha256 -ne $PackageSha256) {
    Add-Failure -Failures $failures -Message "Manifest PackageSha256 '$manifestPackageSha256' does not match current package '$PackageSha256'."
}

$manifestPackagePath = [string](Get-PropertyValue -Object $manifest -Name 'PackagePath' -Default $PackagePath)
if ([string]::IsNullOrWhiteSpace($manifestPackagePath)) {
    $manifestPackagePath = $PackagePath
}

$packageFull = Resolve-WorkspacePath -Path $manifestPackagePath
$actualPackageSha256 = ''
if (-not (Test-Path -LiteralPath $packageFull -PathType Leaf)) {
    Add-Failure -Failures $failures -Message "Package under test does not exist: $packageFull."
} else {
    $actualPackageSha256 = Get-SpirePlusFileSha256 -Path $packageFull
    if ($actualPackageSha256 -ne $PackageSha256) {
        Add-Failure -Failures $failures -Message "Actual package SHA256 '$actualPackageSha256' for '$packageFull' does not match current package '$PackageSha256'."
    }
}

$rows = @(Get-PropertyValue -Object $manifest -Name 'Rows' -Default @())
$rowMap = @{}
$requiredRowIds = @{}
foreach ($required in $requiredReleaseRows) {
    $requiredRowIds[$required.Id] = $true
}

foreach ($row in $rows) {
    $id = [string](Get-PropertyValue -Object $row -Name 'Id' -Default '')
    if ([string]::IsNullOrWhiteSpace($id)) {
        Add-Warning -Warnings $warnings -Message 'Release evidence manifest contains a row with no Id; it is ignored.'
        continue
    }

    if ($rowMap.ContainsKey($id)) {
        Add-Failure -Failures $failures -Message "Duplicate release evidence row id: $id."
        continue
    }

    if (-not $requiredRowIds.ContainsKey($id)) {
        Add-Warning -Warnings $warnings -Message "Unknown release evidence row id ignored: $id."
    }

    $rowMap[$id] = $row
}

foreach ($required in $requiredReleaseRows) {
    if (-not $rowMap.ContainsKey($required.Id)) {
        Add-Failure -Failures $failures -Message "Missing required release evidence row: $($required.Id) ($($required.Label))."
        continue
    }

    $row = $rowMap[$required.Id]
    $status = ([string](Get-PropertyValue -Object $row -Name 'Status' -Default '')).ToLowerInvariant()
    $rowKind = [string](Get-PropertyValue -Object $row -Name 'Kind' -Default $required.Kind)
    if (-not [string]::Equals($rowKind, $required.Kind, [System.StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) Kind '$rowKind' does not match required kind '$($required.Kind)'."
    }

    if ($status -eq 'deferred') {
        $explicitOwnerDecision = [bool](Get-PropertyValue -Object $row -Name 'ExplicitOwnerDecision' -Default $false)
        $releaseNote = [string](Get-PropertyValue -Object $row -Name 'ReleaseNote' -Default '')
        if (-not $AllowDeferred) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred. Re-run with -AllowDeferred only after the owner explicitly accepts a release-note deferral."
            continue
        }

        if (-not $explicitOwnerDecision -or [string]::IsNullOrWhiteSpace($releaseNote)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred without ExplicitOwnerDecision=true and a ReleaseNote."
        }
        continue
    }

    if ($status -ne 'pass') {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is not pass or accepted deferred. Current status: '$status'."
        continue
    }

    $evidenceDirRaw = [string](Get-PropertyValue -Object $row -Name 'EvidenceDir' -Default '')
    if ([string]::IsNullOrWhiteSpace($evidenceDirRaw)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) has pass status but no EvidenceDir."
        continue
    }

    $evidenceDir = Resolve-WorkspacePath -Path $evidenceDirRaw
    if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $evidenceDir)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir is outside EvidenceRoot: $evidenceDir."
        continue
    }

    if (-not (Test-Path -LiteralPath $evidenceDir -PathType Container)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir does not exist: $evidenceDir."
        continue
    }

    $defaultRequiredFiles = @(Get-RequiredEvidenceFilesForRow -RequiredRow $required)
    $rowRequiredFiles = @(Get-PropertyValue -Object $row -Name 'RequiredFiles' -Default @())
    $requiredFiles = @(Merge-RequiredEvidenceFiles -DefaultFiles $defaultRequiredFiles -RowFiles $rowRequiredFiles)
    foreach ($requiredFile in $requiredFiles) {
        $requiredFileString = [string]$requiredFile
        if ([string]::IsNullOrWhiteSpace($requiredFileString)) {
            continue
        }

        $filePath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $requiredFileString
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $filePath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file path escapes EvidenceDir: $requiredFileString."
            continue
        }

        if (-not (Test-Path -LiteralPath $filePath -PathType Leaf)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) missing required evidence file: $filePath."
            continue
        }

        if ((Get-Item -LiteralPath $filePath).Length -eq 0) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file is empty: $filePath."
        }

        if ($requiredFileString -eq 'package-hashes.json') {
            Test-PackageHashesEvidence `
                -PackageHashesPath $filePath `
                -RowId $required.Id `
                -Failures $failures `
                -ManifestPackagePath $manifestPackagePath
        }

        if ($requiredFileString.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
            $note = Get-Content -LiteralPath $filePath -Raw -Encoding UTF8
            if ([string]::IsNullOrWhiteSpace($note)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' is empty."
            }

            if ($note -match $invalidEvidenceNotePattern) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' describes invalid or non-counting evidence."
            }
        }
    }

    $logAuditFiles = @($requiredFiles | Where-Object { ([string]$_).EndsWith('godot-log-audit.json', [System.StringComparison]::OrdinalIgnoreCase) })
    foreach ($logAuditFile in $logAuditFiles) {
        $auditPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path ([string]$logAuditFile)
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $auditPath)) {
            continue
        }

        if ((Test-Path -LiteralPath $auditPath -PathType Leaf) -and -not (Read-CleanLogAudit -AuditPath $auditPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) log audit is not clean: $auditPath."
        }
    }

    if ($required.Kind -eq 'clicked-ui') {
        $preflightPath = Join-Path $evidenceDir 'window-preflight.json'
        if ((Test-Path -LiteralPath $preflightPath -PathType Leaf) -and -not (Test-PreflightForeground -PreflightPath $preflightPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) window-preflight.json does not prove Slay the Spire 2 was foreground."
        }

        $screenshotFile = [string](Get-PropertyValue -Object $row -Name 'ScreenshotFile' -Default '')
        if (-not [string]::IsNullOrWhiteSpace($screenshotFile)) {
            $screenshotPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $screenshotFile
            if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot path escapes EvidenceDir: $screenshotFile."
            } elseif (-not (Test-Path -LiteralPath $screenshotPath -PathType Leaf)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is missing: $screenshotPath."
            } elseif ((Get-Item -LiteralPath $screenshotPath).Length -eq 0) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is empty: $screenshotPath."
            } elseif (-not (Test-PngSignature -Path $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is not a valid PNG: $screenshotPath."
            } elseif (-not (Test-PngMinimumDimensions -Path $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is too small: $screenshotPath. Minimum is ${MinScreenshotWidth}x${MinScreenshotHeight}."
            }
        } else {
            $screenshots = @(Get-ChildItem -LiteralPath $evidenceDir -Filter '*.png' -File -ErrorAction SilentlyContinue)
            if ($screenshots.Count -eq 0) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) needs a PNG screenshot or ScreenshotFile."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has only empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngSignature -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid non-empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngMinimumDimensions -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid PNG screenshots at least ${MinScreenshotWidth}x${MinScreenshotHeight} in $evidenceDir."
            }
        }

    }

    $resultNote = [string](Get-PropertyValue -Object $row -Name 'ResultNote' -Default '')
    if ([string]::IsNullOrWhiteSpace($resultNote)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is pass but has no ResultNote."
    } elseif ($resultNote -match $invalidEvidenceNotePattern) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) ResultNote describes invalid or non-counting evidence."
    }

    if ($required.Id -eq 'a19-a20-dedicated-boss-abilities') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'boss-ability-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-BossAbilityChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'ancient-reward-visible-relics') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'ancient-reward-relics-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-AncientRewardRelicsChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'rootblight-visual-behavior') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'rootblight-behavior-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-RootblightBehaviorChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'art-resource-routing-live-preview') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'art-resource-routing-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-ArtResourceRoutingChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'player-text-tooltip-readability') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'player-text-qa-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-PlayerTextQaChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-victory-no-black-screen') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-victory-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuVictoryRows `
                -TemplateInstruction 'Copy this file to `vakuu-victory-checklist.md`' `
                -ChecklistName 'vakuu-victory-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-failure-death-path') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-failure-death-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuFailureDeathRows `
                -TemplateInstruction 'Copy this file to `vakuu-failure-death-checklist.md`' `
                -ChecklistName 'vakuu-failure-death-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-active-fight-save-load') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-save-load-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuSaveLoadRows `
                -TemplateInstruction 'Copy this file to `vakuu-save-load-checklist.md`' `
                -ChecklistName 'vakuu-save-load-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'preview-tools-live-proof') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'preview-tools-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredPreviewToolsRows `
                -TemplateInstruction 'Copy this file to `preview-tools-checklist.md`' `
                -ChecklistName 'preview-tools-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'coop-disposition') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'coop-disposition-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredCoopRows `
                -TemplateInstruction 'Copy this file to `coop-disposition-checklist.md`' `
                -ChecklistName 'coop-disposition-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'mod-settings-current-display') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'mod-settings-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredModSettingsRows `
                -TemplateInstruction 'Copy this file to `mod-settings-checklist.md`' `
                -ChecklistName 'mod-settings-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }
}

$summary = [ordered]@{
    ManifestPath = $manifestFull
    EvidenceRoot = $evidenceRootFull
    CheckedAt = (Get-Date).ToString('o')
    PackageSha256 = $PackageSha256
    PackagePath = $packageFull
    ActualPackageSha256 = $actualPackageSha256
    MinScreenshotWidth = $MinScreenshotWidth
    MinScreenshotHeight = $MinScreenshotHeight
    AllowDeferred = [bool]$AllowDeferred
    RequiredRowCount = $requiredReleaseRows.Count
    FailureCount = $failures.Count
    WarningCount = $warnings.Count
    Failures = @($failures)
    Warnings = @($warnings)
    WritePassMarker = [bool]$WritePassMarker
    PassMarkerPath = $passMarkerFull
}

$summary | ConvertTo-Json -Depth 20

if ($failures.Count -gt 0) {
    exit 1
}

if ($WritePassMarker) {
    Write-PassMarker -OutputPath $passMarkerFull -Summary $summary
}
