using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(NCrystalSphereScreen), nameof(NCrystalSphereScreen._Ready))]
internal static partial class CrystalSpherePeekPatch
{
    internal const string ButtonName = "SpirePlusCrystalSpherePeekButton";

    private static void Postfix(NCrystalSphereScreen __instance)
    {
        if (!SpirePlusModConfig.EnableCrystalSpherePeek)
        {
            return;
        }

        var runState = RunManager.Instance?.DebugOnlyGetState();
        if (!MultiplayerFeaturePolicy.IsSingleplayer(runState))
        {
            MultiplayerFeaturePolicy.LogCoopEvidence(
                "PreviewCrystalSphere",
                "coop_local_ui_preview_enabled",
                runState,
                new Dictionary<string, object?>
                {
                    ["reason"] = "Crystal Sphere peek only changes local ScryMask alpha and does not reveal cells, spend charges, or grant rewards."
                });
        }

        var mask = __instance.GetNodeOrNull<Control>("%ScryMask");
        if (mask == null)
        {
            PreviewLog.Warn("Crystal Sphere peek skipped: ScryMask node was not found.");
            return;
        }

        var rightUi = __instance.GetNodeOrNull("Ui/RightUi")
            ?? __instance.GetNodeOrNull<Control>("%BigDivinationButton")?.GetParent();

        if (rightUi is not Control rightControl)
        {
            PreviewLog.Warn("Crystal Sphere peek skipped: right-side UI container was not found.");
            return;
        }

        if (rightControl.GetNodeOrNull<Button>(ButtonName) != null)
        {
            return;
        }

        // This preview intentionally changes only the mask alpha. It must not use
        // source reveal, cell-resolution, or reward APIs because those mutate the
        // minigame result instead of merely letting the player inspect it.
        var originalAlpha = mask.Modulate.A;
        var button = new Button
        {
            Name = ButtonName,
            Text = GetPeekButtonText(),
            ToggleMode = true,
            FocusMode = Control.FocusModeEnum.All,
            CustomMinimumSize = new Vector2(0, 54)
        };

        button.Connect(
            BaseButton.SignalName.Toggled,
            Callable.From<bool>(pressed =>
            {
                ApplyPeekMaskState(mask, pressed, originalAlpha);
            }));

        rightControl.AddChild(button);
        RegisterPeekButtonState(__instance, mask, originalAlpha, button);

        if (rightControl.GetChildCount() > 2)
        {
            rightControl.MoveChild(button, 2);
        }

        ReleaseEvidenceLog.Log(
            "PreviewCrystalSphere",
            "peek_button_added",
            runState: runState,
            data: new Dictionary<string, object?>
            {
                ["button"] = ButtonName,
                ["originalAlpha"] = originalAlpha,
                ["configuredAlpha"] = SpirePlusModConfig.CrystalSphereMaskAlpha
            });
    }
}

[HarmonyPatch]
internal static class CrystalSpherePeekFinishedPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NCrystalSphereScreen), "OnMinigameFinished")!;
    }

    private static void Postfix(NCrystalSphereScreen __instance)
    {
        CrystalSpherePeekPatch.HideForFinishedScreen(__instance);
    }
}
