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
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Owns the RitsuLib ModPatcher migration list so bootstrap code can focus on startup order.
/// </summary>
internal static partial class SpirePlusMigratedPatchRegistry
{
    public static void RegisterAll(ModPatcher patcher)
    {
        RegisterBatch4a(patcher);
        RegisterBatch4b(patcher);
        RegisterAncientRewardPatches(patcher);
        RegisterLowRiskRewardHookPatches(patcher);
        RegisterPrismaticGemRewardPatches(patcher);
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
        RegisterUrdaTransformAndSeedbedPatches(patcher);
        RegisterAscensionMapGenerationPatches(patcher);
#if REPLACEMENT_PROTOTYPE_ENABLED
        RegisterSts1ReplacementPrototypePatches(patcher);
#endif
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

    private static void RegisterLowRiskRewardHookPatches(ModPatcher patcher)
    {
        // These were direct Harmony attributes on narrow reward hooks. Keeping
        // them in one RitsuLib group makes the remaining fallback boundary
        // smaller without mixing reward behavior into the clicked-UI registry.
        patcher.RegisterPatch<JeweledMaskCombatStartPatch>();
        patcher.RegisterPatch<JewelryBoxPatch>();
        patcher.RegisterPatch<JewelryBoxApotheosisCanonicalKeywordsPatch>();
        patcher.RegisterPatch<PaelsHornPhase1Patch>();
        patcher.RegisterPatch<PaelsToothPickupPatch>();
        patcher.RegisterPatch<PaelsToothCombatPatch>();
        patcher.RegisterPatch<PaelsToothActTransitionPatch>();
        patcher.RegisterPatch<PreservedFogPatch>();
        patcher.RegisterPatch<FollyKeywordsPatch>();
        patcher.RegisterPatch<SozuPotionGatePatch>();
        patcher.RegisterPatch<EctoplasmGoldGatePatch>();
        patcher.RegisterPatch<SereTalonPickupPatches>();
        patcher.RegisterPatch<SovereignBladeForgeExhaustPatch>();
        patcher.RegisterPatch<SovereignBladeJadeBoonsOnPlayPatch>();
        patcher.RegisterPatch<TanxClawsMaulTuningPatches>();
        patcher.RegisterPatch<ToastyMittensPatch>();
        patcher.RegisterPatch<WhisperingEarringPatch>();
    }

    private static void RegisterPrismaticGemRewardPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<PrismaticGemPoolPatch>();
        patcher.RegisterPatch<PrismaticGemRewardScreenContextPatch>();
        patcher.RegisterPatch<PrismaticGemRewardPatch>();
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

    private static void RegisterUrdaTransformAndSeedbedPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<WitheredHuskTransformablePatch>();
        patcher.RegisterPatch<WitheredHuskTransformationOptionsPatch>();
        patcher.RegisterPatch<UrdaSeedbedAfterCardDrawnPatch>();
        patcher.RegisterPatch<UrdaSeedbedCardPileDrawPatch>();
    }

    private static void RegisterAscensionMapGenerationPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AscensionActModelCreateMapPatch>();
    }

#if REPLACEMENT_PROTOTYPE_ENABLED
    private static void RegisterSts1ReplacementPrototypePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<Sts1ReplacementPrototype>();
    }
#endif

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
