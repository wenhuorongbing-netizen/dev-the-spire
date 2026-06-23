using EZMicroBalance.EZMicroBalanceCode.Preview;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib ModPatcher registrations for local preview-tool UI patches.
/// These patches are separated from Ancient/map UI because their runtime
/// contract is local-only: they must display predictions without adding
/// choices, rewards, or real RNG advancement.
/// </summary>
internal static partial class SpirePlusMigratedPatchRegistry
{
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
}
