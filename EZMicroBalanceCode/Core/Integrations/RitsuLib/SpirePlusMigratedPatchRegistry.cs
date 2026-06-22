using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
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
        RegisterClickedUiPatches(patcher);
        RegisterMapUiPatches(patcher);
        RegisterSereTalonUiPatches(patcher);
        RegisterPreviewUiPatches(patcher);
        RegisterRemainingUiPatches(patcher);
        RegisterBatch4cLocalizationPatches(patcher);
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
        patcher.RegisterPatch<UrdaOptionRelicClickPatch>();
        patcher.RegisterPatch<UrdaRootSightMapQuestIconInputPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPreviewIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapQuestIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightDisabledMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightMapClosePatch>();
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
    }

    private static void RegisterRemainingUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<PrismaticGemRewardScreenHintPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenReadyPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenStatePatch>();
        patcher.RegisterPatch<ModInfoLocalizationPatches>();
        patcher.RegisterPatch<CombatHandInputSafetyPatch>();
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
