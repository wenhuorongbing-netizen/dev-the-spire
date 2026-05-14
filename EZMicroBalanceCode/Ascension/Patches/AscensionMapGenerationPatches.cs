using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]
internal static class AscensionActModelCreateMapPatch
{
    private static void Postfix(RunState runState, ref ActMap __result)
    {
        __result = AscensionMapService.ApplyA11MapGeometryAtCreateMapBoundary(
            runState,
            __result,
            runState.CurrentActIndex);
    }
}
