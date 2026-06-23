using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Reward, relic, and card-behavior registrations migrated to RitsuLib ModPatcher.
/// </summary>
internal static partial class SpirePlusMigratedPatchRegistry
{
    private static void RegisterCardRelicBehaviorPatches(ModPatcher patcher)
    {
        RegisterFiddlePatches(patcher);
        patcher.RegisterPatch<ChoicesParadoxPatch>();
        RegisterDistinguishedCapePatches(patcher);
        patcher.RegisterPatch<BlackStarObtainPatch>();
    }

    private static void RegisterCardTextAndPickupPatches(ModPatcher patcher)
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
        // These narrow reward hooks stay grouped so future high-risk migration
        // reviews can see exactly which reward behavior is already RitsuLib-owned.
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
