# Harmony Patch Inventory

Generated: 2026-05-26

Purpose: keep every Harmony patch visible, owned, and risk-labeled. Regenerate after adding, moving, or deleting patch declarations.

Regenerate:

```powershell
.\scripts\generate-patch-inventory.ps1
.\scripts\validate-repository-hygiene.ps1
```

## Summary

| Metric | Count |
| --- | ---: |
| Total patch declarations | 164 |
| High risk | 22 |
| Medium risk | 44 |
| Low risk | 98 |
| Unclassified owner | 0 |

## Risk Meaning

- High: run, room, save, lobby, multiplayer, or game lifecycle surface.
- Medium: UI, card, relic, reward, combat object, or model hook surface.
- Low: narrow local hook with lower source-drift blast radius.

## Patches

| Owner | Risk | File | Line | Patch |
| --- | --- | --- | ---: | --- |
| Ancient shared infrastructure | Low | `EZMicroBalanceCode/Ancients/Common/NeowInitialOptionRerollPatch.cs` | 7 | `[HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]` |
| Lotha | Low | `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAct3AncientService.cs` | 49 | `[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]` |
| Morvi | Low | `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAct2AncientService.cs` | 49 | `[HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 50 | `[HarmonyPatch(typeof(Overgrowth), nameof(Overgrowth.GetUnlockedAncients))]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 58 | `[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaMapUiPatches.cs` | 7 | `[HarmonyPatch(typeof(NNormalMapPoint), nameof(NNormalMapPoint._Ready))]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaMapUiPatches.cs` | 31 | `[HarmonyPatch(typeof(NNormalMapPoint), "RefreshState")]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaMapUiPatches.cs` | 43 | `[HarmonyPatch(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaOptionRelicClickPatch.cs` | 6 | `[HarmonyPatch(typeof(NRelicInventory), "OnRelicClicked")]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightMapClickPatches.cs` | 7 | `[HarmonyPatch(typeof(NMapPoint), "OnRelease")]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightMapClickPatches.cs` | 23 | `[HarmonyPatch(typeof(NClickableControl), nameof(NClickableControl._GuiInput))]` |
| Urda | Medium | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightMapClickPatches.cs` | 43 | `[HarmonyPatch(typeof(NMapScreen), nameof(NMapScreen.Close))]` |
| Urda | High | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightRoomPatches.cs` | 5 | `[HarmonyPatch(typeof(RunManager), "RollRoomTypeFor")]` |
| Urda | High | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightRoomPatches.cs` | 21 | `[HarmonyPatch(typeof(RunManager), "CreateRoom")]` |
| Urda | Low | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedAfterCardDrawnPatch.cs` | 5 | `[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]` |
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
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/BlackStarCompensationPatches.cs` | 3 | `[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/BrightestFlameExhaustDrawPatch.cs` | 15 | `[HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/BrightestFlameExhaustDrawPatch.cs` | 33 | `[HarmonyPatch(typeof(BrightestFlame), "get_CanonicalVars")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/BrightestFlameExhaustDrawPatch.cs` | 49 | `[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/ChoicesParadoxPatches.cs` | 3 | `[HarmonyPatch(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/CrossbowPatches.cs` | 3 | `[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/CrossbowPatches.cs` | 60 | `[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 3 | `[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterCreated))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 16 | `[HarmonyPatch(typeof(CardModel), nameof(CardModel.FromSerializable))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 29 | `[HarmonyPatch(typeof(Debt), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 40 | `[HarmonyPatch(typeof(Debt), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 51 | `[HarmonyPatch(typeof(Debt), "get_HasTurnEndInHandEffect")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 62 | `[HarmonyPatch(typeof(Debt), "OnTurnEndInHand")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 73 | `[HarmonyPatch(typeof(CardModel), "OnPlay")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs` | 106 | `[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DistinguishedCapePatches.cs` | 3 | `[HarmonyPatch(typeof(DistinguishedCape), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DistinguishedCapePatches.cs` | 19 | `[HarmonyPatch(typeof(Vakuu), "GenerateInitialOptions")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/DistinguishedCapePatches.cs` | 104 | `[HarmonyPatch(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs` | 3 | `[HarmonyPatch(typeof(Fiddle), "get_CanonicalVars")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs` | 14 | `[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs` | 37 | `[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs` | 59 | `[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JeweledMaskPatches.cs` | 3 | `[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 3 | `[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 37 | `[HarmonyPatch(typeof(Apotheosis), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 75 | `[HarmonyPatch(typeof(JewelryBox), "get_ExtraHoverTips")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 86 | `[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/JewelryBoxPatches.cs` | 103 | `[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/MeatCleaverCookPatches.cs` | 3 | `[HarmonyPatch(typeof(CookRestSiteOption), MethodType.Constructor, typeof(Player))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/MeatCleaverCookPatches.cs` | 16 | `[HarmonyPatch(typeof(CookRestSiteOption), "get_Description")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/MeatCleaverCookPatches.cs` | 37 | `[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsHornPhase1Patch.cs` | 11 | `[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 3 | `[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 13 | `[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PaelsToothPatches.cs` | 24 | `[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs` | 3 | `[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs` | 31 | `[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ShouldGainGold))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PickupRewardPatches.cs` | 3 | `[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PreservedFogPatches.cs` | 3 | `[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/PreservedFogPatches.cs` | 29 | `[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemHoverPatches.cs` | 19 | `[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemHoverPatches.cs` | 39 | `[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs` | 3 | `[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemRewardContextPatches.cs` | 3 | `[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemRewardContextPatches.cs` | 14 | `[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/PrismaticGemRewardScreenHintPatch.cs` | 3 | `[HarmonyPatch( typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen), nameof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen.RefreshOptions))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SealOfGoldPatches.cs` | 3 | `[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SealOfGoldPatches.cs` | 16 | `[HarmonyPatch(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonPickupPatches.cs` | 3 | `[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 8 | `[HarmonyPatch(typeof(RelicModel), "get_IconPath")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 17 | `[HarmonyPatch(typeof(RelicModel), "get_PackedIconPath")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 26 | `[HarmonyPatch(typeof(RelicModel), "get_PackedIconOutlinePath")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 35 | `[HarmonyPatch(typeof(RelicModel), "get_BigIconPath")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 44 | `[HarmonyPatch(typeof(RelicModel), "get_Icon")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 53 | `[HarmonyPatch(typeof(RelicModel), "get_IconOutline")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 62 | `[HarmonyPatch(typeof(RelicModel), "get_BigIcon")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 71 | `[HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 83 | `[HarmonyPatch(typeof(NRelic), "Reload")]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SovereignBladeForgePatches.cs` | 42 | `[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]` |
| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SovereignBladeForgePatches.cs` | 75 | `[HarmonyPatch(typeof(SovereignBlade), "OnPlay")]` |
| Ancient reward rebalance | Medium | `EZMicroBalanceCode/Ancients/Patches/SovereignBladeForgePatches.cs` | 85 | `[HarmonyPatch(typeof(CardModel), "get_HoverTips")]` |
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
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | 9 | `[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]` |
| Ascension patches | High | `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | 50 | `[HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionA20RewardScreenPatches.cs` | 56 | `[HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionA20RewardScreenPatches.cs` | 94 | `[HarmonyPatch(typeof(NRewardsScreen), "UpdateScreenState")]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 6 | `[HarmonyPatch(typeof(LocString), nameof(LocString.GetRawText))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 26 | `[HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 38 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 59 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 80 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))]` |
| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 92 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionMapBossSealHoverPatches.cs` | 11 | `[HarmonyPatch(typeof(NBossMapPoint), "OnFocus")]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionMapGenerationPatches.cs` | 8 | `[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]` |
| Ascension patches | Medium | `EZMicroBalanceCode/Ascension/Patches/AscensionMapIconPatches.cs` | 6 | `[HarmonyPatch(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")]` |
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
| Map hover composition | Medium | `EZMicroBalanceCode/Map/SpirePlusMapPointHoverComposer.cs` | 12 | `[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]` |
| Mod info localization | Medium | `EZMicroBalanceCode/Modding/ModInfoLocalizationPatches.cs` | 12 | `[HarmonyPatch(typeof(NModInfoContainer), nameof(NModInfoContainer.Fill))]` |
| Preview tools | Medium | `EZMicroBalanceCode/Preview/CrystalSpherePeekPatch.cs` | 14 | `[HarmonyPatch(typeof(NCrystalSphereScreen), nameof(NCrystalSphereScreen._Ready))]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/CrystalSpherePeekPatch.cs` | 160 | `[HarmonyPatch]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionEventRngSourcePatches.cs` | 6 | `[HarmonyPatch]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionEventRngSourcePatches.cs` | 9 | `[HarmonyPatch(typeof(AromaOfChaos), "LetGo")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionEventRngSourcePatches.cs` | 14 | `[HarmonyPatch(typeof(EndlessConveyor), "JellyLiver")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionEventRngSourcePatches.cs` | 19 | `[HarmonyPatch(typeof(Symbiote), "KillWithFire")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionEventRngSourcePatches.cs` | 24 | `[HarmonyPatch(typeof(WhisperingHollow), "Hug")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionNicheRngSourcePatches.cs` | 8 | `[HarmonyPatch]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionNicheRngSourcePatches.cs` | 11 | `[HarmonyPatch(typeof(MorphicGrove), "Group")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionNicheRngSourcePatches.cs` | 16 | `[HarmonyPatch(typeof(Trial), "NondescriptInnocent")]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionNicheRngSourcePatches.cs` | 21 | `[HarmonyPatch(typeof(NewLeaf), nameof(NewLeaf.AfterObtained))]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionNicheRngSourcePatches.cs` | 26 | `[HarmonyPatch(typeof(Astrolabe), nameof(Astrolabe.AfterObtained))]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPredictionSelectionLifetimePatch.cs` | 8 | `[HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromDeckForTransformation))]` |
| Preview tools | Medium | `EZMicroBalanceCode/Preview/TransformPreviewPatch.cs` | 19 | `[HarmonyPatch(typeof(NTransformPreview), nameof(NTransformPreview.Initialize))]` |
| Preview tools | Low | `EZMicroBalanceCode/Preview/TransformPreviewPatch.cs` | 38 | `[HarmonyPatch]` |
