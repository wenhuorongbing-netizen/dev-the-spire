using EZMicroBalance.EZMicroBalanceCode.Ancients;
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
