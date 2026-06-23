# Harmony Patch Inventory

Generated: 2026-06-23

Purpose: keep every Harmony patch visible, owned, and risk-labeled. Regenerate after adding, moving, or deleting patch declarations.

Regenerate:

```powershell
.\scripts\generate-patch-inventory.ps1
.\scripts\validate-repository-hygiene.ps1
```

## Summary

| Metric | Count |
| --- | ---: |
| Total raw HarmonyPatch declarations | 15 |
| Migrated to RitsuLib ModPatcher | 155 |
| Raw HarmonyPatch remaining | 15 |
| Tracked patch units total | 170 |
| High risk (raw Harmony) | 13 |
| Medium risk (raw Harmony) | 0 |
| Low risk (raw Harmony) | 2 |
| Unclassified owner | 0 |

## Risk Meaning

- High: run, room, save, lobby, multiplayer, or game lifecycle surface.
- Medium: UI, card, relic, reward, combat object, or model hook surface.
- Low: narrow local hook with lower source-drift blast radius.

## Migrated Patches (RitsuLib ModPatcher)

These 155 patch classes implement `IPatchMethod` and are registered via
`SpirePlusMigratedPatchRegistry.RegisterAll(...)`. They use `ModPatcher.PatchAll()`
and are NOT picked up by raw `Harmony.PatchAll()`.

| File | Classes | PatchIds | Batch |
| --- | --- | --- | --- |
| `FiddlePatches.cs` | 4 | `fiddle-vars, fiddle-hand-draw, fiddle-should-draw, fiddle-draw-cap` | 4a |
| `ChoicesParadoxPatches.cs` | 1 | `choices-paradox-turn-start` | 4a |
| `DistinguishedCapePatches.cs` | 3 | `distinguished-cape-vars, distinguished-cape-event-option, distinguished-cape-pickup` | 4a |
| `BlackStarCompensationPatches.cs` | 1 | `black-star-obtain` | 4a |
| `CrossbowPatches.cs` | 2 | `crossbow-offer, crossbow-vanilla-after-turn` | 4b |
| `BrightestFlameExhaustDrawPatch.cs` | 3 | `brightest-flame-keywords, brightest-flame-vars, brightest-flame-exhaust-backstop` | 4b |
| `DebtAndCardPatches.cs` | 8 | `debt-after-created, debt-from-save, debt-keywords, debt-vars, debt-turn-end-effect, debt-turn-end-in-hand, card-model-on-play, debt-exhaust` | 4b |
| `SealOfGoldPatches.cs` | 2 | `seal-of-gold-max-energy, seal-of-gold-turn` | 4b |
| `PickupRewardPatches.cs` | 1 | `ancient-pickup-balance` | 4b |
| `VakuRewardPatches.cs` | 8 | `iron-club-vars, brilliant-scarf-vars, beautiful-bracelet-vars, beautiful-bracelet-after-obtained, music-box-before-card-played, music-box-after-card-played, music-box-turn-reset, music-box-combat-reset` | ancient-reward |
| `VelvetChokerPatches.cs` | 10 | `velvet-choker-vars, velvet-choker-display-amount, velvet-choker-should-play, velvet-choker-energy-cost, velvet-choker-x-cost-can-play, velvet-choker-x-cost-spend, velvet-choker-after-card-played, velvet-choker-turn-reset, velvet-choker-room-reset, velvet-choker-combat-reset` | ancient-reward |
| `JeweledMaskPatches.cs` | 1 | `jeweled-mask-combat-start` | ancient-reward-low-risk |
| `JewelryBoxPatches.cs` | 2 | `jewelry-box-after-obtained, jewelry-box-apotheosis-keywords` | ancient-reward-low-risk |
| `PaelsHornPhase1Patch.cs` | 1 | `paels-horn-after-obtained` | ancient-reward-low-risk |
| `PaelsToothPatches.cs` | 3 | `paels-tooth-after-obtained, paels-tooth-after-combat-end, paels-tooth-act-transition` | ancient-reward-low-risk |
| `PreservedFogPatches.cs` | 2 | `preserved-fog-after-obtained, preserved-fog-folly-keywords` | ancient-reward-low-risk |
| `PickupRewardGatePatches.cs` | 2 | `sozu-initial-potion-gate, ectoplasm-initial-gold-gate` | ancient-reward-low-risk |
| `SereTalonPickupPatches.cs` | 1 | `sere-talon-after-obtained` | ancient-reward-low-risk |
| `SovereignBladeForgePatches.cs` | 2 | `sovereign-blade-forge-exhaust, sovereign-blade-on-play-jade-boons` | ancient-reward-low-risk |
| `TanxClawsMaulTuningPatches.cs` | 1 | `tanx-claws-after-obtained` | ancient-reward-low-risk |
| `ToastyMittensPatches.cs` | 1 | `toasty-mittens-before-hand-draw` | ancient-reward-low-risk |
| `WhisperingEarringPatches.cs` | 1 | `whispering-earring-auto-pre-play` | ancient-reward-low-risk |
| `PrismaticGemPatches.cs` | 1 | `prismatic-gem-reward-options` | ancient-reward-medium-risk |
| `PrismaticGemRewardContextPatches.cs` | 2 | `prismatic-gem-pool-noop, prismatic-gem-reward-screen-context` | ancient-reward-medium-risk |
| `NeowInitialOptionRerollPatch.cs` | 1 | `neow-initial-option-reroll` | clicked-ui |
| `UrdaAct1AncientService.cs` | 2 | `urda-overgrowth-ancient-unlock, urda-underdocks-ancient-unlock` | clicked-ui |
| `UrdaOptionRelicClickPatch.cs` | 1 | `urda-option-relic-click` | clicked-ui |
| `MorviAct2AncientService.cs` | 1 | `morvi-hive-ancient-unlock` | clicked-ui |
| `LothaAct3AncientService.cs` | 1 | `lotha-glory-ancient-unlock` | clicked-ui |
| `VakuuFightPatch.cs` | 5 | `vakuu-force-ancient-unlock, vakuu-fight-option, vakuu-fight-command-force-cleanup, vakuu-fight-victory-resume, vakuu-fight-prefinished-parent-heal-skip` | clicked-ui |
| `UrdaMapUiPatches.cs` | 3 | `urda-root-sight-map-point-ready, urda-root-sight-map-refresh-state, urda-root-sight-map-quest-icon-refresh` | clicked-ui |
| `UrdaRootSightMapClickPatches.cs` | 3 | `urda-root-sight-map-point-click, urda-root-sight-disabled-map-point-click, urda-root-sight-map-close` | clicked-ui |
| `UrdaRootSightRoomPatches.cs` | 2 | `urda-root-sight-roll-room-type, urda-root-sight-create-room` | urda-root-sight-routing |
| `SpirePlusMapPointHoverComposer.cs` | 1 | `spire-plus-map-point-hover-composer` | clicked-ui |
| `AscensionMapIconPatches.cs` | 1 | `ascension-map-marker-icon-refresh` | clicked-ui |
| `AscensionMapBossSealHoverPatches.cs` | 1 | `ascension-boss-map-point-hover` | clicked-ui |
| `SereTalonVisualUiPatches.cs` | 2 | `sere-talon-event-option-button-ready, sere-talon-relic-node-reload` | clicked-ui |
| `CrystalSpherePeekPatch.cs` | 2 | `crystal-sphere-peek-ready, crystal-sphere-peek-finished` | clicked-ui |
| `TransformPreviewPatch.cs` | 2 | `transform-preview-initialize, transform-preview-cycle-display` | clicked-ui |
| `TransformPredictionEventRngSourcePatches.cs` | 4 | `transform-prediction-aroma-of-chaos-rng, transform-prediction-endless-conveyor-rng, transform-prediction-symbiote-rng, transform-prediction-whispering-hollow-rng` | clicked-ui |
| `TransformPredictionNicheRngSourcePatches.cs` | 4 | `transform-prediction-morphic-grove-niche-rng, transform-prediction-trial-niche-rng, transform-prediction-new-leaf-niche-rng, transform-prediction-astrolabe-niche-rng` | clicked-ui |
| `TransformPredictionSelectionLifetimePatch.cs` | 1 | `transform-prediction-selection-lifetime` | clicked-ui |
| `SereTalonVisualPatches.cs` | 7 | `sere-talon-icon-path, sere-talon-packed-icon-path, sere-talon-packed-icon-outline-path, sere-talon-big-icon-path, sere-talon-icon-texture, sere-talon-icon-outline-texture, sere-talon-big-icon-texture` | visual-hover-ui |
| `PrismaticGemHoverPatches.cs` | 2 | `prismatic-gem-hover-tips, prismatic-gem-hover-tips-excluding-relic` | visual-hover-ui |
| `JewelryBoxPatches.cs` | 3 | `jewelry-box-extra-hover-tips, jewelry-box-hover-tips, jewelry-box-hover-tips-excluding-relic` | visual-hover-ui |
| `SovereignBladeForgePatches.cs` | 1 | `sovereign-blade-jade-boons-hover-tips` | visual-hover-ui |
| `PrismaticGemRewardScreenHintPatch.cs` | 1 | `prismatic-gem-reward-screen-hint` | clicked-ui |
| `AscensionA20RewardScreenPatches.cs` | 2 | `ascension-a20-reward-screen-ready, ascension-a20-reward-screen-state` | clicked-ui |
| `AscensionA20Patches.cs` | 1 | `ascension-a20-courtyard-proceed` | clicked-ui |
| `A20Courtyard.cs` | 1 | `ascension-a20-courtyard-portrait` | event-visual-ui |
| `ModInfoLocalizationPatches.cs` | 1 | `spire-plus-mod-info-localization` | clicked-ui |
| `CombatHandInputSafetyPatches.cs` | 1 | `combat-hand-input-safety` | clicked-ui |
| `MeatCleaverCookPatches.cs` | 3 | `meat-cleaver-cook-is-enabled, meat-cleaver-cook-description, meat-cleaver-cook-on-select` | clicked-ui |
| `AscensionSelectionPatches.cs` | 1 | `ascension-selection-singleplayer-character-change` | clicked-ui |
| `AscensionSelectionRunStartPatches.cs` | 5 | `ascension-selection-begin-run-locally, ascension-selection-update-max-multiplayer, ascension-selection-update-preferred, ascension-selection-sync-warning, ascension-selection-begin-run-for-all-warning` | clicked-ui |
| `AeonglassIntentPatches.cs` | 2 | `aeonglass-laser-echo-intent-label, aeonglass-laser-echo-intent-damage` | intent-ui |
| `EnemyDamagePolishPatches.cs` | 7 | `decimillipede-writhe-damage-polish, decimillipede-constrict-damage-polish, decimillipede-bulk-damage-polish, terror-eel-crash-damage-polish, terror-eel-thrash-damage-polish, phantasmal-gardener-bite-damage-polish, phantasmal-gardener-lash-damage-polish` | enemy-damage-polish |
| `AscensionLocalizationTablePatches.cs` | 6 | `ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key` | 4c-localization |
| `SpirePlusInlineLocalizationPatches.cs` | 4 | `spire-plus-inline-localization-raw-text, spire-plus-inline-localization-loc-string, spire-plus-inline-localization-has-entry, spire-plus-inline-localization-is-local-key` | inline-localization |
| `RitsuLibModSettingsButtonSelectionReticlePatch.cs` | 1 | `ritsulib-mod-settings-button-selection-reticle` | ritsulib-compatibility |
| `UrdaWitheredHuskTransformPatches.cs` | 2 | `urda-withered-husk-transformable, urda-withered-husk-transformation-options` | urda-transform-seedbed |
| `UrdaSeedbedAfterCardDrawnPatch.cs` | 1 | `urda-seedbed-after-card-drawn` | urda-transform-seedbed |
| `UrdaSeedbedCardPileDrawPatch.cs` | 1 | `urda-seedbed-card-pile-draw` | urda-transform-seedbed |
| `AscensionMapGenerationPatches.cs` | 1 | `ascension-act-model-create-map` | ascension-map-generation |
| `Sts1ReplacementPrototype.cs` | 1 | `sts1-replacement-prototype-generate-rooms` | sts1-replacement-prototype |

Double-patch guard: migrated classes contain no `[HarmonyPatch]` attributes.
`Harmony.PatchAll()` will not pick them up. Verified clean separation.

## Raw HarmonyPatch Declarations (Unmigrated)

These 15 `[HarmonyPatch]` declarations remain on raw `Harmony.PatchAll()`.

| Owner | Risk | File | Line | Patch |
| --- | --- | --- | ---: | --- |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 119 | `[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]` |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 129 | `[HarmonyPatch(typeof(EventRoom), nameof(EventRoom.EnterInternal))]` |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 167 | `[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.JoinFlow.cs` | 10 | `[HarmonyPatch(typeof(JoinFlow), "HandleInitialGameInfoMessage")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.Lobby.cs` | 10 | `[HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.Lobby.cs` | 40 | `[HarmonyPatch(typeof(StartRunLobby), "BeginRunLocally")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.Lobby.cs` | 58 | `[HarmonyPatch(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.RunState.cs` | 10 | `[HarmonyPatch(typeof(NGame), "StartNewMultiplayerRun")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.RunState.cs` | 32 | `[HarmonyPatch(typeof(RunManager), "EnterAct")]` |
| Ascension core | Low | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.RunState.cs` | 62 | `[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]` |
| Ascension core | Low | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.SaveQuit.cs` | 8 | `[HarmonyPatch]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.SaveQuit.cs` | 11 | `[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveRun), typeof(AbstractRoom), typeof(bool))]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.SaveQuit.cs` | 26 | `[HarmonyPatch(typeof(NGame), "ReturnToMainMenu")]` |
| Ascension core | High | `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.SaveQuit.cs` | 38 | `[HarmonyPatch(typeof(NGame), "Quit")]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | 10 | `[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]` |
