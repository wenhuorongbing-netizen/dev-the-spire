using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal sealed partial class CrystalSpherePeekPatch
{
    private const string ToggleOnSfx = "event:/sfx/ui/clicks/ui_checkbox_on";
    private const string ToggleOffSfx = "event:/sfx/ui/clicks/ui_checkbox_off";

    private static readonly ConditionalWeakTable<NCrystalSphereScreen, PeekState> PeekStates = new();

    private static void RegisterPeekButtonState(
        NCrystalSphereScreen screen,
        Control mask,
        float originalAlpha,
        Button button)
    {
        PeekStates.Remove(screen);
        PeekStates.Add(screen, new PeekState(mask, originalAlpha, button));
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
