using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SereTalonAncientEventOptionButtonPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-event-option-button-ready";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Refresh Sere Talon icons on Ancient event option buttons after vanilla UI setup";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))];

    [HarmonyPostfix]
    private static void Postfix(NEventOptionButton __instance)
    {
        // Ancient option buttons assign the relic icon directly during _Ready().
        // Keep this surface explicit so a loader/UI drift report can name the
        // exact surface instead of grouping it with normal RelicModel getters.
        SereTalonVisualNodeRoutes.TryApplyEventOptionButton(__instance);
    }
}

internal sealed class SereTalonRelicNodeReloadPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-relic-node-reload";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Refresh Sere Talon icons on relic bar and inspect nodes after node reload";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NRelic), "Reload")];

    [HarmonyPostfix]
    private static void Postfix(NRelic __instance)
    {
        // Relic-bar and inspect nodes can reload after the model texture getters
        // have already run. Reapply only to SereTalon so Tanx Claws keeps the
        // source Maul-transform visuals.
        SereTalonVisualNodeRoutes.TryApplyRelicNode(__instance);
    }
}
