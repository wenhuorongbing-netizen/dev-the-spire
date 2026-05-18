using EZFuturePeek.EZFuturePeekCode.Config;
using EZFuturePeek.EZFuturePeekCode.Diagnostics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

namespace EZFuturePeek.EZFuturePeekCode.Patches;

[HarmonyPatch(typeof(NCrystalSphereScreen), nameof(NCrystalSphereScreen._Ready))]
internal static class CrystalSpherePeekPatch
{
    private const string ButtonName = "EZFuturePeekCrystalSphereButton";

    private static void Postfix(NCrystalSphereScreen __instance)
    {
        if (!EZFuturePeekConfig.EnableCrystalSpherePeek)
        {
            return;
        }

        var mask = __instance.GetNodeOrNull<Control>("%ScryMask");
        if (mask == null)
        {
            FuturePeekLog.Warn("Crystal Sphere peek skipped: ScryMask node was not found.");
            return;
        }

        var rightUi = __instance.GetNodeOrNull("Ui/RightUi")
            ?? __instance.GetNodeOrNull<Control>("%BigDivinationButton")?.GetParent();

        if (rightUi is not Control rightControl)
        {
            FuturePeekLog.Warn("Crystal Sphere peek skipped: right-side UI container was not found.");
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
                color.A = pressed ? (float)EZFuturePeekConfig.CrystalSphereMaskAlpha : originalAlpha;
                mask.Modulate = color;
                FuturePeekLog.Debug(pressed ? "Crystal Sphere peek enabled." : "Crystal Sphere peek disabled.");
            }));

        rightControl.AddChild(button);
        if (rightControl.GetChildCount() > 2)
        {
            rightControl.MoveChild(button, 2);
        }
    }

    private static string GetPeekButtonText()
    {
        return LocString.GetIfExists("settings_ui", "EZFUTUREPEEK-CRYSTAL_SPHERE_PEEK_BUTTON.title")?.GetFormattedText()
            ?? "Peek";
    }
}
