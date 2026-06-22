using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Ascension.Events;
using EZMicroBalance.EZMicroBalanceCode.Core.Localization;
using EZMicroBalance.EZMicroBalanceCode.Map;
using EZMicroBalance.EZMicroBalanceCode.Modding;
using EZMicroBalance.EZMicroBalanceCode.Preview;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Owns the RitsuLib ModPatcher migration list so bootstrap code can focus on startup order.
/// </summary>
internal static class SpirePlusMigratedPatchRegistry
{
    public static void RegisterAll(ModPatcher patcher)
    {
        RegisterBatch4a(patcher);
        RegisterBatch4b(patcher);
        RegisterAncientRewardPatches(patcher);
        RegisterAncientEventUiPatches(patcher);
        RegisterClickedUiPatches(patcher);
        RegisterMapUiPatches(patcher);
        RegisterSereTalonUiPatches(patcher);
        RegisterPreviewUiPatches(patcher);
        RegisterRelicVisualHoverPatches(patcher);
        RegisterRemainingUiPatches(patcher);
        RegisterAscensionSelectionUiPatches(patcher);
        RegisterAscensionIntentUiPatches(patcher);
        RegisterEnemyDamagePolishPatches(patcher);
        RegisterBatch4cLocalizationPatches(patcher);
        RegisterInlineLocalizationPatches(patcher);
        RegisterRitsuLibCompatibilityPatches(patcher);
    }

    private static void RegisterBatch4a(ModPatcher patcher)
    {
        RegisterFiddlePatches(patcher);
        patcher.RegisterPatch<ChoicesParadoxPatch>();
        RegisterDistinguishedCapePatches(patcher);
        patcher.RegisterPatch<BlackStarObtainPatch>();
    }

    private static void RegisterBatch4b(ModPatcher patcher)
    {
        RegisterCrossbowPatches(patcher);
        RegisterBrightestFlamePatches(patcher);
        RegisterDebtAndCardPatches(patcher);
        RegisterSealOfGoldPatches(patcher);
        patcher.RegisterPatch<AncientPickupBalancePatch>();
    }

    private static void RegisterClickedUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<UrdaRootSightMapQuestIconInputPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPreviewIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapQuestIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightDisabledMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightMapClosePatch>();
    }

    private static void RegisterAncientEventUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<NeowInitialOptionRerollPatch>();
        patcher.RegisterPatch<UrdaOvergrowthPatch>();
        patcher.RegisterPatch<UrdaUnderdocksPatch>();
        patcher.RegisterPatch<UrdaOptionRelicClickPatch>();
        patcher.RegisterPatch<MorviHivePatch>();
        patcher.RegisterPatch<LothaGloryPatch>();
        patcher.RegisterPatch<VakuuForceAncientPatch>();
        patcher.RegisterPatch<VakuuFightOptionPatch>();
        patcher.RegisterPatch<VakuuFightCommandForceCleanupPatch>();
        patcher.RegisterPatch<VakuuFightResumePatch>();
        patcher.RegisterPatch<VakuuFightPreFinishedParentRestoreHealPatch>();
    }

    private static void RegisterAncientRewardPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<IronClubVarsPatch>();
        patcher.RegisterPatch<BrilliantScarfVarsPatch>();
        patcher.RegisterPatch<BeautifulBraceletVarsPatch>();
        patcher.RegisterPatch<BeautifulBraceletPatch>();
        patcher.RegisterPatch<MusicBoxBeforeCardPlayedPatch>();
        patcher.RegisterPatch<MusicBoxAfterCardPlayedPatch>();
        patcher.RegisterPatch<MusicBoxTurnResetPatch>();
        patcher.RegisterPatch<MusicBoxCombatResetPatch>();
        patcher.RegisterPatch<VelvetChokerVarsPatch>();
        patcher.RegisterPatch<VelvetChokerDisplayAmountPatch>();
        patcher.RegisterPatch<VelvetChokerShouldPlayPatch>();
        patcher.RegisterPatch<VelvetChokerEnergyCostPatch>();
        patcher.RegisterPatch<VelvetChokerXCostCanPlayPatch>();
        patcher.RegisterPatch<VelvetChokerXCostSpendPatch>();
        patcher.RegisterPatch<VelvetChokerAfterCardPlayedPatch>();
        patcher.RegisterPatch<VelvetChokerTurnResetPatch>();
        patcher.RegisterPatch<VelvetChokerRoomResetPatch>();
        patcher.RegisterPatch<VelvetChokerCombatResetPatch>();
    }

    private static void RegisterMapUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SpirePlusMapPointHoverComposer>();
        patcher.RegisterPatch<FiremarkedEliteMapIconPatch>();
        patcher.RegisterPatch<BossMapPointHoverPatch>();
    }

    private static void RegisterSereTalonUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SereTalonAncientEventOptionButtonPatch>();
        patcher.RegisterPatch<SereTalonRelicNodeReloadPatch>();
    }

    private static void RegisterPreviewUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<CrystalSpherePeekPatch>();
        patcher.RegisterPatch<CrystalSpherePeekFinishedPatch>();
        patcher.RegisterPatch<TransformPreviewInitializePatch>();
        patcher.RegisterPatch<TransformPreviewCyclePatch>();
        patcher.RegisterPatch<TransformPredictionAromaOfChaosRngPatch>();
        patcher.RegisterPatch<TransformPredictionEndlessConveyorRngPatch>();
        patcher.RegisterPatch<TransformPredictionSymbioteRngPatch>();
        patcher.RegisterPatch<TransformPredictionWhisperingHollowRngPatch>();
        patcher.RegisterPatch<TransformPredictionMorphicGroveNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionTrialNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionNewLeafNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionAstrolabeNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionSelectionLifetimePatch>();
    }

    private static void RegisterRelicVisualHoverPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SereTalonIconPathPatch>();
        patcher.RegisterPatch<SereTalonPackedIconPathPatch>();
        patcher.RegisterPatch<SereTalonPackedIconOutlinePathPatch>();
        patcher.RegisterPatch<SereTalonBigIconPathPatch>();
        patcher.RegisterPatch<SereTalonIconTexturePatch>();
        patcher.RegisterPatch<SereTalonIconOutlineTexturePatch>();
        patcher.RegisterPatch<SereTalonBigIconTexturePatch>();
        patcher.RegisterPatch<PrismaticGemHoverTipsPatch>();
        patcher.RegisterPatch<PrismaticGemHoverTipsExcludingRelicPatch>();
        patcher.RegisterPatch<JewelryBoxExtraHoverTipsPatch>();
        patcher.RegisterPatch<JewelryBoxHoverTipsPatch>();
        patcher.RegisterPatch<JewelryBoxHoverTipsExcludingRelicPatch>();
        patcher.RegisterPatch<SovereignBladeJadeBoonsHoverTipsPatch>();
    }

    private static void RegisterRemainingUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<PrismaticGemRewardScreenHintPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenReadyPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenStatePatch>();
        patcher.RegisterPatch<AscensionA20CourtyardProceedPatch>();
        patcher.RegisterPatch<AscensionA20CourtyardPortraitPatch>();
        patcher.RegisterPatch<ModInfoLocalizationPatches>();
        patcher.RegisterPatch<CombatHandInputSafetyPatch>();
        patcher.RegisterPatch<MeatCleaverCookIsEnabledPatch>();
        patcher.RegisterPatch<MeatCleaverCookDescriptionPatch>();
        patcher.RegisterPatch<MeatCleaverCookPatch>();
    }

    private static void RegisterAscensionSelectionUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<StartRunLobbySetSingleplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunLocallyPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdateMaxMultiplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdatePreferredAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbySyncAscensionChangeA20WarningPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunForAllPlayersA20WarningPatch>();
    }

    private static void RegisterAscensionIntentUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AeonglassLaserEchoIntentLabelPatch>();
        patcher.RegisterPatch<AeonglassLaserEchoIntentDamagePatch>();
    }

    private static void RegisterEnemyDamagePolishPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<DecimillipedeWritheDamagePolishPatch>();
        patcher.RegisterPatch<DecimillipedeConstrictDamagePolishPatch>();
        patcher.RegisterPatch<DecimillipedeBulkDamagePolishPatch>();
        patcher.RegisterPatch<TerrorEelCrashDamagePolishPatch>();
        patcher.RegisterPatch<TerrorEelThrashDamagePolishPatch>();
        patcher.RegisterPatch<PhantasmalGardenerBiteDamagePolishPatch>();
        patcher.RegisterPatch<PhantasmalGardenerLashDamagePolishPatch>();
    }

    private static void RegisterBatch4cLocalizationPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AscensionLocalizationLocStringRawTextPatch>();
        patcher.RegisterPatch<AscensionLocalizationGetTablePatch>();
        patcher.RegisterPatch<AscensionLocalizationRawTextPatch>();
        patcher.RegisterPatch<AscensionLocalizationLocStringPatch>();
        patcher.RegisterPatch<AscensionLocalizationHasEntryPatch>();
        patcher.RegisterPatch<AscensionLocalizationIsLocalKeyPatch>();
    }

    private static void RegisterInlineLocalizationPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SpirePlusInlineLocalizationRawTextPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationLocStringPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationHasEntryPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationIsLocalKeyPatch>();
    }

    private static void RegisterRitsuLibCompatibilityPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<RitsuLibModSettingsButtonSelectionReticlePatch>();
    }

    private static void RegisterFiddlePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<FiddleVarsPatch>();
        patcher.RegisterPatch<FiddleHandDrawPatch>();
        patcher.RegisterPatch<FiddleShouldDrawPatch>();
        patcher.RegisterPatch<FiddleDrawCapPatch>();
    }

    private static void RegisterDistinguishedCapePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<DistinguishedCapeVarsPatch>();
        patcher.RegisterPatch<DistinguishedCapeEventOptionPatch>();
        patcher.RegisterPatch<DistinguishedCapePickupPatch>();
    }

    private static void RegisterCrossbowPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<CrossbowOfferPatch>();
        patcher.RegisterPatch<CrossbowVanillaAfterTurnPatch>();
    }

    private static void RegisterBrightestFlamePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<BrightestFlameCanonicalKeywordsPatch>();
        patcher.RegisterPatch<BrightestFlameCanonicalVarsPatch>();
        patcher.RegisterPatch<BrightestFlameExhaustOnPlayBackstopPatch>();
    }

    private static void RegisterDebtAndCardPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<DebtAfterCreatedPatch>();
        patcher.RegisterPatch<DebtFromSavePatch>();
        patcher.RegisterPatch<DebtKeywordsPatch>();
        patcher.RegisterPatch<DebtVarsPatch>();
        patcher.RegisterPatch<DebtTurnEndEffectPatch>();
        patcher.RegisterPatch<DebtTurnEndInHandPatch>();
        patcher.RegisterPatch<CardModelOnPlayPatch>();
        patcher.RegisterPatch<DebtExhaustPatch>();
    }

    private static void RegisterSealOfGoldPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SealOfGoldMaxEnergyPatch>();
        patcher.RegisterPatch<SealOfGoldTurnPatch>();
    }
}
