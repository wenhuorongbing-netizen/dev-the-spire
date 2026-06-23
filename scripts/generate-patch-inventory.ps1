param(
    [switch]$Check,
    [string]$OutputPath = 'docs\patch-inventory.md'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$resolvedOutputPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
    [System.IO.Path]::GetFullPath($OutputPath)
} else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputPath))
}

function Get-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $root = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\') + '\'
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($root.Length).Replace('\', '/')
    }

    return $fullPath.Replace('\', '/')
}

function Normalize-InventoryForCheck {
    param([Parameter(Mandatory = $true)][string]$Content)

    return (($Content -replace "`r`n", "`n") -replace '(?m)^Generated: .+$', 'Generated: <ignored>').TrimEnd()
}

function Get-OwnerFromPath {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Urda/*') { return 'Urda' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Morvi/*') { return 'Morvi' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Lotha/*') { return 'Lotha' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Vakuu/*') { return 'Vakuu' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Common/*') { return 'Ancient shared infrastructure' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Patches/*') { return 'Ancient reward rebalance' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Core/*') { return 'Ascension core' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Map/*') { return 'Ascension map' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Combat/*') { return 'Ascension combat' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Rewards/*') { return 'Ascension rewards' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Patches/*') { return 'Ascension patches' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Events/*') { return 'Ascension events' }
    if ($RelativePath -like 'EZMicroBalanceCode/Core/Localization/*') { return 'Core localization' }
    if ($RelativePath -like 'EZMicroBalanceCode/Map/*') { return 'Map hover composition' }
    if ($RelativePath -like 'EZMicroBalanceCode/Modding/*') { return 'Mod info localization' }
    if ($RelativePath -like 'EZMicroBalanceCode/Preview/*') { return 'Preview tools' }
    if ($RelativePath -like 'EZMicroBalanceCode/Sts1Events/*') { return 'STS1 event replacements' }
    return 'Unclassified'
}

function Get-RiskFromPatch {
    param(
        [Parameter(Mandatory = $true)][string]$RelativePath,
        [Parameter(Mandatory = $true)][string]$PatchText
    )

    $combined = "$RelativePath $PatchText"
    if ($combined -match 'CombatRoom|EventRoom|RunManager|SaveManager|StartRunLobby|NGame|JoinFlow') { return 'High' }
    if ($combined -match 'NMap|NRewardsScreen|NCrystalSphere|NTransformPreview|NNormalMapPoint|NBossMapPoint|NRelicInventory|NModInfoContainer') { return 'Medium' }
    if ($combined -match 'CardModel|RelicModel|CardReward|CardPileCmd|Creature|Power|AbstractModel') { return 'Medium' }
    return 'Low'
}

$sourceRoots = @('EZMicroBalanceCode')
$patches = [System.Collections.Generic.List[object]]::new()

foreach ($root in $sourceRoots) {
    $absoluteRoot = Join-Path $repoRoot $root
    if (-not (Test-Path -LiteralPath $absoluteRoot)) {
        continue
    }

    Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter '*.cs' |
        Sort-Object -Property FullName |
        ForEach-Object {
            $relativePath = Get-RepoRelativePath $_.FullName
            $lines = Get-Content -LiteralPath $_.FullName -Encoding UTF8
            for ($index = 0; $index -lt $lines.Count; $index++) {
                $line = $lines[$index].Trim()
                if ($line.StartsWith('[HarmonyPatch', [System.StringComparison]::Ordinal)) {
                    $patchText = $line
                    $cursor = $index + 1
                    while ($patchText -notmatch '\]$' -and $cursor -lt $lines.Count) {
                        $patchText += ' ' + $lines[$cursor].Trim()
                        $cursor++
                    }

                    $owner = Get-OwnerFromPath $relativePath
                    $risk = Get-RiskFromPatch $relativePath $patchText
                    [void]$patches.Add([pscustomobject]@{
                        File = $relativePath
                        Line = $index + 1
                        Owner = $owner
                        Risk = $risk
                        Patch = ($patchText -replace '\|', '\|')
                    })
                }
            }
        }
}

$generatedAt = (Get-Date).ToString('yyyy-MM-dd')
$highCount = @($patches | Where-Object Risk -eq 'High').Count
$mediumCount = @($patches | Where-Object Risk -eq 'Medium').Count
$lowCount = @($patches | Where-Object Risk -eq 'Low').Count
$unclassifiedCount = @($patches | Where-Object Owner -eq 'Unclassified').Count
$migratedPatchRows = @(
    [pscustomobject]@{ File = 'FiddlePatches.cs'; Classes = 4; PatchIds = 'fiddle-vars, fiddle-hand-draw, fiddle-should-draw, fiddle-draw-cap'; Batch = '4a' },
    [pscustomobject]@{ File = 'ChoicesParadoxPatches.cs'; Classes = 1; PatchIds = 'choices-paradox-turn-start'; Batch = '4a' },
    [pscustomobject]@{ File = 'DistinguishedCapePatches.cs'; Classes = 3; PatchIds = 'distinguished-cape-vars, distinguished-cape-event-option, distinguished-cape-pickup'; Batch = '4a' },
    [pscustomobject]@{ File = 'BlackStarCompensationPatches.cs'; Classes = 1; PatchIds = 'black-star-obtain'; Batch = '4a' },
    [pscustomobject]@{ File = 'CrossbowPatches.cs'; Classes = 2; PatchIds = 'crossbow-offer, crossbow-vanilla-after-turn'; Batch = '4b' },
    [pscustomobject]@{ File = 'BrightestFlameExhaustDrawPatch.cs'; Classes = 3; PatchIds = 'brightest-flame-keywords, brightest-flame-vars, brightest-flame-exhaust-backstop'; Batch = '4b' },
    [pscustomobject]@{ File = 'DebtAndCardPatches.cs'; Classes = 8; PatchIds = 'debt-after-created, debt-from-save, debt-keywords, debt-vars, debt-turn-end-effect, debt-turn-end-in-hand, card-model-on-play, debt-exhaust'; Batch = '4b' },
    [pscustomobject]@{ File = 'SealOfGoldPatches.cs'; Classes = 2; PatchIds = 'seal-of-gold-max-energy, seal-of-gold-turn'; Batch = '4b' },
    [pscustomobject]@{ File = 'PickupRewardPatches.cs'; Classes = 1; PatchIds = 'ancient-pickup-balance'; Batch = '4b' },
    [pscustomobject]@{ File = 'VakuRewardPatches.cs'; Classes = 8; PatchIds = 'iron-club-vars, brilliant-scarf-vars, beautiful-bracelet-vars, beautiful-bracelet-after-obtained, music-box-before-card-played, music-box-after-card-played, music-box-turn-reset, music-box-combat-reset'; Batch = 'ancient-reward' },
    [pscustomobject]@{ File = 'VelvetChokerPatches.cs'; Classes = 10; PatchIds = 'velvet-choker-vars, velvet-choker-display-amount, velvet-choker-should-play, velvet-choker-energy-cost, velvet-choker-x-cost-can-play, velvet-choker-x-cost-spend, velvet-choker-after-card-played, velvet-choker-turn-reset, velvet-choker-room-reset, velvet-choker-combat-reset'; Batch = 'ancient-reward' },
    [pscustomobject]@{ File = 'JeweledMaskPatches.cs'; Classes = 1; PatchIds = 'jeweled-mask-combat-start'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'JewelryBoxPatches.cs'; Classes = 2; PatchIds = 'jewelry-box-after-obtained, jewelry-box-apotheosis-keywords'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'PaelsHornPhase1Patch.cs'; Classes = 1; PatchIds = 'paels-horn-after-obtained'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'PaelsToothPatches.cs'; Classes = 3; PatchIds = 'paels-tooth-after-obtained, paels-tooth-after-combat-end, paels-tooth-act-transition'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'PreservedFogPatches.cs'; Classes = 2; PatchIds = 'preserved-fog-after-obtained, preserved-fog-folly-keywords'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'PickupRewardGatePatches.cs'; Classes = 2; PatchIds = 'sozu-initial-potion-gate, ectoplasm-initial-gold-gate'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'SereTalonPickupPatches.cs'; Classes = 1; PatchIds = 'sere-talon-after-obtained'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'SovereignBladeForgePatches.cs'; Classes = 2; PatchIds = 'sovereign-blade-forge-exhaust, sovereign-blade-on-play-jade-boons'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'TanxClawsMaulTuningPatches.cs'; Classes = 1; PatchIds = 'tanx-claws-after-obtained'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'ToastyMittensPatches.cs'; Classes = 1; PatchIds = 'toasty-mittens-before-hand-draw'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'WhisperingEarringPatches.cs'; Classes = 1; PatchIds = 'whispering-earring-auto-pre-play'; Batch = 'ancient-reward-low-risk' },
    [pscustomobject]@{ File = 'NeowInitialOptionRerollPatch.cs'; Classes = 1; PatchIds = 'neow-initial-option-reroll'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'UrdaAct1AncientService.cs'; Classes = 2; PatchIds = 'urda-overgrowth-ancient-unlock, urda-underdocks-ancient-unlock'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'UrdaOptionRelicClickPatch.cs'; Classes = 1; PatchIds = 'urda-option-relic-click'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'MorviAct2AncientService.cs'; Classes = 1; PatchIds = 'morvi-hive-ancient-unlock'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'LothaAct3AncientService.cs'; Classes = 1; PatchIds = 'lotha-glory-ancient-unlock'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'VakuuFightPatch.cs'; Classes = 5; PatchIds = 'vakuu-force-ancient-unlock, vakuu-fight-option, vakuu-fight-command-force-cleanup, vakuu-fight-victory-resume, vakuu-fight-prefinished-parent-heal-skip'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'UrdaMapUiPatches.cs'; Classes = 3; PatchIds = 'urda-root-sight-map-point-ready, urda-root-sight-map-refresh-state, urda-root-sight-map-quest-icon-refresh'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'UrdaRootSightMapClickPatches.cs'; Classes = 3; PatchIds = 'urda-root-sight-map-point-click, urda-root-sight-disabled-map-point-click, urda-root-sight-map-close'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'SpirePlusMapPointHoverComposer.cs'; Classes = 1; PatchIds = 'spire-plus-map-point-hover-composer'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionMapIconPatches.cs'; Classes = 1; PatchIds = 'ascension-map-marker-icon-refresh'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionMapBossSealHoverPatches.cs'; Classes = 1; PatchIds = 'ascension-boss-map-point-hover'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'SereTalonVisualUiPatches.cs'; Classes = 2; PatchIds = 'sere-talon-event-option-button-ready, sere-talon-relic-node-reload'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'CrystalSpherePeekPatch.cs'; Classes = 2; PatchIds = 'crystal-sphere-peek-ready, crystal-sphere-peek-finished'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'TransformPreviewPatch.cs'; Classes = 2; PatchIds = 'transform-preview-initialize, transform-preview-cycle-display'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'TransformPredictionEventRngSourcePatches.cs'; Classes = 4; PatchIds = 'transform-prediction-aroma-of-chaos-rng, transform-prediction-endless-conveyor-rng, transform-prediction-symbiote-rng, transform-prediction-whispering-hollow-rng'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'TransformPredictionNicheRngSourcePatches.cs'; Classes = 4; PatchIds = 'transform-prediction-morphic-grove-niche-rng, transform-prediction-trial-niche-rng, transform-prediction-new-leaf-niche-rng, transform-prediction-astrolabe-niche-rng'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'TransformPredictionSelectionLifetimePatch.cs'; Classes = 1; PatchIds = 'transform-prediction-selection-lifetime'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'SereTalonVisualPatches.cs'; Classes = 7; PatchIds = 'sere-talon-icon-path, sere-talon-packed-icon-path, sere-talon-packed-icon-outline-path, sere-talon-big-icon-path, sere-talon-icon-texture, sere-talon-icon-outline-texture, sere-talon-big-icon-texture'; Batch = 'visual-hover-ui' },
    [pscustomobject]@{ File = 'PrismaticGemHoverPatches.cs'; Classes = 2; PatchIds = 'prismatic-gem-hover-tips, prismatic-gem-hover-tips-excluding-relic'; Batch = 'visual-hover-ui' },
    [pscustomobject]@{ File = 'JewelryBoxPatches.cs'; Classes = 3; PatchIds = 'jewelry-box-extra-hover-tips, jewelry-box-hover-tips, jewelry-box-hover-tips-excluding-relic'; Batch = 'visual-hover-ui' },
    [pscustomobject]@{ File = 'SovereignBladeForgePatches.cs'; Classes = 1; PatchIds = 'sovereign-blade-jade-boons-hover-tips'; Batch = 'visual-hover-ui' },
    [pscustomobject]@{ File = 'PrismaticGemRewardScreenHintPatch.cs'; Classes = 1; PatchIds = 'prismatic-gem-reward-screen-hint'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionA20RewardScreenPatches.cs'; Classes = 2; PatchIds = 'ascension-a20-reward-screen-ready, ascension-a20-reward-screen-state'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionA20Patches.cs'; Classes = 1; PatchIds = 'ascension-a20-courtyard-proceed'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'A20Courtyard.cs'; Classes = 1; PatchIds = 'ascension-a20-courtyard-portrait'; Batch = 'event-visual-ui' },
    [pscustomobject]@{ File = 'ModInfoLocalizationPatches.cs'; Classes = 1; PatchIds = 'spire-plus-mod-info-localization'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'CombatHandInputSafetyPatches.cs'; Classes = 1; PatchIds = 'combat-hand-input-safety'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'MeatCleaverCookPatches.cs'; Classes = 3; PatchIds = 'meat-cleaver-cook-is-enabled, meat-cleaver-cook-description, meat-cleaver-cook-on-select'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionSelectionPatches.cs'; Classes = 1; PatchIds = 'ascension-selection-singleplayer-character-change'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AscensionSelectionRunStartPatches.cs'; Classes = 5; PatchIds = 'ascension-selection-begin-run-locally, ascension-selection-update-max-multiplayer, ascension-selection-update-preferred, ascension-selection-sync-warning, ascension-selection-begin-run-for-all-warning'; Batch = 'clicked-ui' },
    [pscustomobject]@{ File = 'AeonglassIntentPatches.cs'; Classes = 2; PatchIds = 'aeonglass-laser-echo-intent-label, aeonglass-laser-echo-intent-damage'; Batch = 'intent-ui' },
    [pscustomobject]@{ File = 'EnemyDamagePolishPatches.cs'; Classes = 7; PatchIds = 'decimillipede-writhe-damage-polish, decimillipede-constrict-damage-polish, decimillipede-bulk-damage-polish, terror-eel-crash-damage-polish, terror-eel-thrash-damage-polish, phantasmal-gardener-bite-damage-polish, phantasmal-gardener-lash-damage-polish'; Batch = 'enemy-damage-polish' },
    [pscustomobject]@{ File = 'AscensionLocalizationTablePatches.cs'; Classes = 6; PatchIds = 'ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key'; Batch = '4c-localization' },
    [pscustomobject]@{ File = 'SpirePlusInlineLocalizationPatches.cs'; Classes = 4; PatchIds = 'spire-plus-inline-localization-raw-text, spire-plus-inline-localization-loc-string, spire-plus-inline-localization-has-entry, spire-plus-inline-localization-is-local-key'; Batch = 'inline-localization' },
    [pscustomobject]@{ File = 'RitsuLibModSettingsButtonSelectionReticlePatch.cs'; Classes = 1; PatchIds = 'ritsulib-mod-settings-button-selection-reticle'; Batch = 'ritsulib-compatibility' },
    [pscustomobject]@{ File = 'UrdaWitheredHuskTransformPatches.cs'; Classes = 2; PatchIds = 'urda-withered-husk-transformable, urda-withered-husk-transformation-options'; Batch = 'urda-transform-seedbed' },
    [pscustomobject]@{ File = 'UrdaSeedbedAfterCardDrawnPatch.cs'; Classes = 1; PatchIds = 'urda-seedbed-after-card-drawn'; Batch = 'urda-transform-seedbed' },
    [pscustomobject]@{ File = 'UrdaSeedbedCardPileDrawPatch.cs'; Classes = 1; PatchIds = 'urda-seedbed-card-pile-draw'; Batch = 'urda-transform-seedbed' }
)
$migratedPatchCount = ($migratedPatchRows | Measure-Object -Property Classes -Sum).Sum
$trackedPatchUnitCount = $migratedPatchCount + $patches.Count

$builder = [System.Text.StringBuilder]::new()
[void]$builder.AppendLine('# Harmony Patch Inventory')
[void]$builder.AppendLine()
[void]$builder.AppendLine("Generated: $generatedAt")
[void]$builder.AppendLine()
[void]$builder.AppendLine('Purpose: keep every Harmony patch visible, owned, and risk-labeled. Regenerate after adding, moving, or deleting patch declarations.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('Regenerate:')
[void]$builder.AppendLine()
[void]$builder.AppendLine('```powershell')
[void]$builder.AppendLine('.\scripts\generate-patch-inventory.ps1')
[void]$builder.AppendLine('.\scripts\validate-repository-hygiene.ps1')
[void]$builder.AppendLine('```')
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Summary')
[void]$builder.AppendLine()
[void]$builder.AppendLine("| Metric | Count |")
[void]$builder.AppendLine("| --- | ---: |")
[void]$builder.AppendLine("| Total raw HarmonyPatch declarations | $($patches.Count) |")
[void]$builder.AppendLine("| Migrated to RitsuLib ModPatcher | $migratedPatchCount |")
[void]$builder.AppendLine("| Raw HarmonyPatch remaining | $($patches.Count) |")
[void]$builder.AppendLine("| Tracked patch units total | $trackedPatchUnitCount |")
[void]$builder.AppendLine("| High risk (raw Harmony) | $highCount |")
[void]$builder.AppendLine("| Medium risk (raw Harmony) | $mediumCount |")
[void]$builder.AppendLine("| Low risk (raw Harmony) | $lowCount |")
[void]$builder.AppendLine("| Unclassified owner | $unclassifiedCount |")
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Risk Meaning')
[void]$builder.AppendLine()
[void]$builder.AppendLine('- High: run, room, save, lobby, multiplayer, or game lifecycle surface.')
[void]$builder.AppendLine('- Medium: UI, card, relic, reward, combat object, or model hook surface.')
[void]$builder.AppendLine('- Low: narrow local hook with lower source-drift blast radius.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Migrated Patches (RitsuLib ModPatcher)')
[void]$builder.AppendLine()
[void]$builder.AppendLine("These $migratedPatchCount patch classes implement ``IPatchMethod`` and are registered via")
[void]$builder.AppendLine('`SpirePlusMigratedPatchRegistry.RegisterAll(...)`. They use `ModPatcher.PatchAll()`')
[void]$builder.AppendLine('and are NOT picked up by raw `Harmony.PatchAll()`.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('| File | Classes | PatchIds | Batch |')
[void]$builder.AppendLine('| --- | --- | --- | --- |')
foreach ($row in $migratedPatchRows) {
    [void]$builder.AppendLine(('| `{0}` | {1} | `{2}` | {3} |' -f $row.File, $row.Classes, $row.PatchIds, $row.Batch))
}
[void]$builder.AppendLine()
[void]$builder.AppendLine('Double-patch guard: migrated classes contain no `[HarmonyPatch]` attributes.')
[void]$builder.AppendLine('`Harmony.PatchAll()` will not pick them up. Verified clean separation.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Raw HarmonyPatch Declarations (Unmigrated)')
[void]$builder.AppendLine()
[void]$builder.AppendLine("These $($patches.Count) ``[HarmonyPatch]`` declarations remain on raw ``Harmony.PatchAll()``.")
[void]$builder.AppendLine()
[void]$builder.AppendLine('| Owner | Risk | File | Line | Patch |')
[void]$builder.AppendLine('| --- | --- | --- | ---: | --- |')
foreach ($patch in $patches) {
    $fileCell = '`' + $patch.File + '`'
    $patchCell = '`' + $patch.Patch + '`'
    [void]$builder.AppendLine(('| {0} | {1} | {2} | {3} | {4} |' -f $patch.Owner, $patch.Risk, $fileCell, $patch.Line, $patchCell))
}

$content = $builder.ToString()
if ($Check) {
    if (-not (Test-Path -LiteralPath $resolvedOutputPath)) {
        throw "Patch inventory missing: $resolvedOutputPath"
    }

    $current = Get-Content -Raw -LiteralPath $resolvedOutputPath -Encoding UTF8
    $normalizedCurrent = Normalize-InventoryForCheck $current
    $normalizedExpected = Normalize-InventoryForCheck $content
    if ($normalizedCurrent -ne $normalizedExpected) {
        throw "Patch inventory is stale: $resolvedOutputPath"
    }

    Write-Host "Patch inventory is fresh: $resolvedOutputPath"
    return
}

$parent = Split-Path -Parent $resolvedOutputPath
if ($parent) {
    New-Item -ItemType Directory -Force -Path $parent | Out-Null
}

[System.IO.File]::WriteAllText($resolvedOutputPath, $content, [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $resolvedOutputPath"
