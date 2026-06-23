using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionActModelCreateMapPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-act-model-create-map";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Apply gated A11 map geometry at Core's ActModel.CreateMap boundary";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(ActModel), nameof(ActModel.CreateMap), [typeof(RunState), typeof(bool)])];

    private static void Postfix(RunState runState, ref ActMap __result)
    {
        // Core creates the serializable ActMap before RunHook.ModifyGeneratedMap.
        // This boundary lets A11 add rows/columns while preserving the vanilla
        // generated map as the source for route-safety checks.
        __result = AscensionMapService.ApplyA11MapGeometryAtCreateMapBoundary(
            runState,
            __result,
            runState.CurrentActIndex);
    }
}
