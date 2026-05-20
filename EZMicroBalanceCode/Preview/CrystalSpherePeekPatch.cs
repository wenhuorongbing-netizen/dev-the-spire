using System.Reflection;
using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(NCrystalSphereScreen), nameof(NCrystalSphereScreen._Ready))]
internal static class CrystalSpherePeekPatch
{
    internal const string ButtonName = "EZMicroBalanceCrystalSpherePeekButton";

    private static readonly ConditionalWeakTable<NCrystalSphereScreen, PeekState> PeekStates = new();

    private static void Postfix(NCrystalSphereScreen __instance)
    {
        if (!EZMicroBalanceModConfig.EnableCrystalSpherePeek)
        {
            return;
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
                var color = mask.Modulate;
                color.A = pressed ? (float)EZMicroBalanceModConfig.CrystalSphereMaskAlpha : originalAlpha;
                mask.Modulate = color;
                PreviewLog.Debug(pressed ? "Crystal Sphere peek enabled." : "Crystal Sphere peek disabled.");
            }));

        rightControl.AddChild(button);
        PeekStates.Remove(__instance);
        PeekStates.Add(__instance, new PeekState(mask, originalAlpha, button));

        if (rightControl.GetChildCount() > 2)
        {
            rightControl.MoveChild(button, 2);
        }
    }

    internal static void HideForFinishedScreen(NCrystalSphereScreen screen)
    {
        if (!PeekStates.TryGetValue(screen, out var state))
        {
            screen.GetNodeOrNull<Control>("Ui/RightUi")?.GetNodeOrNull<Button>(ButtonName)?.Hide();
            return;
        }

        state.Button.Hide();
        var color = state.Mask.Modulate;
        color.A = state.OriginalMaskAlpha;
        state.Mask.Modulate = color;
    }

    private static string GetPeekButtonText()
    {
        return LocString.GetIfExists("settings_ui", "EZMICROBALANCE-CRYSTAL_SPHERE_PEEK_BUTTON.title")?.GetFormattedText()
            ?? (LocManager.Instance.Language == "zhs" ? "预知" : null)
            ?? "Peek";
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
