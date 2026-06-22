using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal sealed class RitsuLibModSettingsButtonSelectionReticlePatch : IPatchMethod
{
    private const string ReticleNodeName = "SelectionReticle";
    private const string ReticleScenePath = "res://scenes/ui/selection_reticle.tscn";
    private const string RitsuLibButtonTypeName = "STS2RitsuLib.Settings.ModSettingsGameSettingsEntryButton";

    private static readonly Type RitsuLibButtonType =
        typeof(RitsuLibFramework).Assembly.GetType(RitsuLibButtonTypeName, throwOnError: true)!;

    static string IPatchMethod.PatchId => "ritsulib-mod-settings-button-selection-reticle";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add the vanilla SelectionReticle child expected by the RitsuLib game-settings entry button on STS2 v0.107.1";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(RitsuLibButtonType, "_Ready")];

    [HarmonyPrefix]
    private static void Prefix(object __instance)
    {
        if (__instance is not Control button ||
            button.HasNode(ReticleNodeName))
        {
            return;
        }

        var reticle = CreateSelectionReticle();
        reticle.Name = ReticleNodeName;
        reticle.MouseFilter = Control.MouseFilterEnum.Ignore;
        reticle.Position = Vector2.Zero;
        reticle.Size = button.Size;
        reticle.PivotOffset = reticle.Size * 0.5f;
        button.AddChild(reticle);
    }

    private static NSelectionReticle CreateSelectionReticle()
    {
        var scene = ResourceLoader.Load<PackedScene>(
            ReticleScenePath,
            null,
            ResourceLoader.CacheMode.Reuse);
        return scene?.Instantiate<NSelectionReticle>() ?? new NSelectionReticle();
    }
}
