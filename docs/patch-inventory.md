# Harmony Patch Inventory

Generated: 2026-06-22

Purpose: keep every Harmony patch visible, owned, and risk-labeled. Regenerate after adding, moving, or deleting patch declarations.

Regenerate:

```powershell
.\scripts\generate-patch-inventory.ps1
.\scripts\validate-repository-hygiene.ps1
```

## Summary

| Metric | Count |
| --- | ---: |
| Total raw HarmonyPatch declarations | 91 |
| Migrated to RitsuLib ModPatcher | 78 |
| Raw HarmonyPatch remaining | 91 |
| Tracked patch units total | 169 |
| High risk (raw Harmony) | 21 |
| Medium risk (raw Harmony) | 8 |
| Low risk (raw Harmony) | 62 |
| Unclassified owner | 0 |

## Risk Meaning

- High: run, room, save, lobby, multiplayer, or game lifecycle surface.
- Medium: UI, card, relic, reward, combat object, or model hook surface.
- Low: narrow local hook with lower source-drift blast radius.

## Migrated Patches (RitsuLib ModPatcher)

These 78 patch classes implement `IPatchMethod` and are registered via
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
| `UrdaOptionRelicClickPatch.cs` | 1 | `urda-option-relic-click` | clicked-ui |
| `UrdaMapUiPatches.cs` | 3 | `urda-root-sight-map-point-ready, urda-root-sight-map-refresh-state, urda-root-sight-map-quest-icon-refresh` | clicked-ui |
| `UrdaRootSightMapClickPatches.cs` | 3 | `urda-root-sight-map-point-click, urda-root-sight-disabled-map-point-click, urda-root-sight-map-close` | clicked-ui |
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
| `ModInfoLocalizationPatches.cs` | 1 | `spire-plus-mod-info-localization` | clicked-ui |
| `CombatHandInputSafetyPatches.cs` | 1 | `combat-hand-input-safety` | clicked-ui |
| `MeatCleaverCookPatches.cs` | 3 | `meat-cleaver-cook-is-enabled, meat-cleaver-cook-description, meat-cleaver-cook-on-select` | clicked-ui |
| `AscensionLocalizationTablePatches.cs` | 6 | `ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key` | 4c-localization |

Double-patch guard: migrated classes contain no `[HarmonyPatch]` attributes.
`Harmony.PatchAll()` will not pick them up. Verified clean separation.

## Raw HarmonyPatch Declarations (Unmigrated)

These 91 `[HarmonyPatch]` declarations remain on raw `Harmony.PatchAll()`.

| Owner | Risk | File | Line | Patch |
| --- | --- | --- | ---: | --- |
| Ancient shared infrastructure | Low | `EZMicroBalanceCode/Ancients/Common/NeowInitialOptionRerollPatch.cs` | 7 | `[HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]` |
| Lotha | Low | `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAct3AncientService.cs` | 49 | `[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]` |
| Morvi | Low | `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAct2AncientService.cs` | 49 | `[HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 49 | `[HarmonyPatch(typeof(Overgrowth), nameof(Overgrowth.GetUnlockedAncients))]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 57 | `[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]` |
| Urda | High | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightRoomPatches.cs` | 5 | `[HarmonyPatch(typeof(RunManager), "RollRoomTypeFor")]` |
| Urda | High | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightRoomPatches.cs` | 21 | `[HarmonyPatch(typeof(RunManager), "CreateRoom")]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedAfterCardDrawnPatch.cs` | 7 | `[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedCardPileDrawPatch.cs` | 9 | `[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaWitheredHuskTransformPatches.cs` | 3 | `[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsTransformable), MethodType.Getter)]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaWitheredHuskTransformPatches.cs` | 15 | `[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.GetDefaultTransformationOptions))]` |
| Vakuu | Low | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 13 | `[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]` |
| Vakuu | Low | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 29 | `[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), "GenerateInitialOptions")]` |
| Vakuu | Low | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 61 | `[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]` |
| Vakuu | Low | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 80 | `[HarmonyPatch(typeof(EventModel), nameof(EventModel.Resume))]` |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 97 | `[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]` |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 107 | `[HarmonyPatch(typeof(EventRoom), nameof(EventRoom.EnterInternal))]` |
| Vakuu | Low | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 121 | `[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]` |
| Vakuu | High | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | 140 | `[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JeweledMaskPatches.cs` | 3 | `[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 5 | `[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 39 | `[HarmonyPatch(typeof(Apotheosis), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsHornPhase1Patch.cs` | 11 | `[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 3 | `[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 13 | `[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 24 | `[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs` | 3 | `[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs` | 31 | `[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PreservedFogPatches.cs` | 3 | `[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PreservedFogPatches.cs` | 29 | `[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs` | 5 | `[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemRewardContextPatches.cs` | 3 | `[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemRewardContextPatches.cs` | 14 | `[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonPickupPatches.cs` | 3 | `[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SovereignBladeForgePatches.cs` | 43 | `[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SovereignBladeForgePatches.cs` | 76 | `[HarmonyPatch(typeof(SovereignBlade), "OnPlay")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/TanxClawsMaulTuningPatches.cs` | 3 | `[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/ToastyMittensPatches.cs` | 3 | `[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 3 | `[HarmonyPatch(typeof(IronClub), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 14 | `[HarmonyPatch(typeof(BrilliantScarf), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 25 | `[HarmonyPatch(typeof(BeautifulBracelet), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 36 | `[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 60 | `[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 96 | `[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 131 | `[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs` | 144 | `[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 3 | `[HarmonyPatch(typeof(VelvetChoker), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 14 | `[HarmonyPatch(typeof(VelvetChoker), "get_DisplayAmount")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 25 | `[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 36 | `[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 55 | `[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 73 | `[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 95 | `[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 114 | `[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 130 | `[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/VelvetChokerPatches.cs` | 146 | `[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/WhisperingEarringPatches.cs` | 3 | `[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]` |
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
| Ascension events | Low | `EZMicroBalanceCode/Ascension/Events/A20Courtyard.cs` | 148 | `[HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AeonglassIntentPatches.cs` | 8 | `[HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AeonglassIntentPatches.cs` | 31 | `[HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | 10 | `[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionMapGenerationPatches.cs` | 8 | `[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionPatches.cs` | 154 | `[HarmonyPatch(typeof(StartRunLobby), "SetSingleplayerAscensionAfterCharacterChanged")]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionRunStartPatches.cs` | 7 | `[HarmonyPatch(typeof(StartRunLobby), "BeginRunLocally")]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionRunStartPatches.cs` | 63 | `[HarmonyPatch(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionRunStartPatches.cs` | 84 | `[HarmonyPatch(typeof(StartRunLobby), "UpdatePreferredAscension")]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionRunStartPatches.cs` | 100 | `[HarmonyPatch(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange))]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionRunStartPatches.cs` | 109 | `[HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 21 | `[HarmonyPatch(typeof(DecimillipedeSegment), "get_WritheDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 30 | `[HarmonyPatch(typeof(DecimillipedeSegment), "get_ConstrictDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 39 | `[HarmonyPatch(typeof(DecimillipedeSegment), "get_BulkDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 48 | `[HarmonyPatch(typeof(TerrorEel), "get_CrashDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 57 | `[HarmonyPatch(typeof(TerrorEel), "get_ThrashDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 66 | `[HarmonyPatch(typeof(PhantasmalGardener), "get_BiteDamage")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/EnemyDamagePolishPatches.cs` | 75 | `[HarmonyPatch(typeof(PhantasmalGardener), "get_LashDamage")]` |
| Core localization | Low | `EZMicroBalanceCode/Core/Localization/SpirePlusInlineLocalizationPatches.cs` | 9 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]` |
| Core localization | Low | `EZMicroBalanceCode/Core/Localization/SpirePlusInlineLocalizationPatches.cs` | 30 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))]` |
| Core localization | Low | `EZMicroBalanceCode/Core/Localization/SpirePlusInlineLocalizationPatches.cs` | 52 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))]` |
| Core localization | Low | `EZMicroBalanceCode/Core/Localization/SpirePlusInlineLocalizationPatches.cs` | 64 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))]` |
| STS1 event replacements | Low | `EZMicroBalanceCode/Sts1Events/Runtime/Sts1ReplacementPrototype.cs` | 45 | `[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]` |
