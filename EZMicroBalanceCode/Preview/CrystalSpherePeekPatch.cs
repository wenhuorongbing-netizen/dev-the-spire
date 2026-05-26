using System.Reflection;
using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(NCrystalSphereScreen), nameof(NCrystalSphereScreen._Ready))]
internal static class CrystalSpherePeekPatch
{
    internal const string ButtonName = "SpirePlusCrystalSpherePeekButton";

    private const string ToggleOnSfx = "event:/sfx/ui/clicks/ui_checkbox_on";
    private const string ToggleOffSfx = "event:/sfx/ui/clicks/ui_checkbox_off";

    private static readonly ConditionalWeakTable<NCrystalSphereScreen, PeekState> PeekStates = new();

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
        PeekStates.Remove(__instance);
        PeekStates.Add(__instance, new PeekState(mask, originalAlpha, button));

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

    internal static void HideForFinishedScreen(NCrystalSphereScreen screen)
    {
        if (!PeekStates.TryGetValue(screen, out var state))
        {
            screen.GetNodeOrNull<Control>("Ui/RightUi")?.GetNodeOrNull<Button>(ButtonName)?.Hide();
            ReleaseEvidenceLog.Log(
                "PreviewCrystalSphere",
                "peek_button_hidden_without_state",
                runState: RunManager.Instance?.DebugOnlyGetState());
            return;
        }

        state.Button.Hide();
        var color = state.Mask.Modulate;
        color.A = state.OriginalMaskAlpha;
        state.Mask.Modulate = color;
        ReleaseEvidenceLog.Log(
            "PreviewCrystalSphere",
            "peek_hidden_after_minigame",
            runState: RunManager.Instance?.DebugOnlyGetState(),
            data: new Dictionary<string, object?>
            {
                ["maskAlpha"] = color.A
            });
    }

    private static string GetPeekButtonText()
    {
        return LocString.GetIfExists("settings_ui", "SPIREPLUS-CRYSTAL_SPHERE_PEEK_BUTTON.title")?.GetFormattedText()
            ?? LocString.GetIfExists("settings_ui", "EZMICROBALANCE-CRYSTAL_SPHERE_PEEK_BUTTON.title")?.GetFormattedText()
            ?? (LocManager.Instance.Language == "zhs" ? "预知" : null)
            ?? "Peek";
    }

    private static void ApplyPeekMaskState(Control mask, bool pressed, float originalAlpha)
    {
        var color = mask.Modulate;
        color.A = pressed ? (float)SpirePlusModConfig.CrystalSphereMaskAlpha : originalAlpha;
        mask.Modulate = color;
        SfxCmd.Play(pressed ? ToggleOnSfx : ToggleOffSfx, 0.85f);
        ReleaseEvidenceLog.Log(
            "PreviewCrystalSphere",
            pressed ? "peek_enabled" : "peek_disabled",
            runState: RunManager.Instance?.DebugOnlyGetState(),
            data: new Dictionary<string, object?>
            {
                ["maskAlpha"] = color.A
            });
        PreviewLog.Debug(pressed ? "Crystal Sphere peek enabled." : "Crystal Sphere peek disabled.");
    }

    private sealed record PeekState(Control Mask, float OriginalMaskAlpha, Button Button);
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
