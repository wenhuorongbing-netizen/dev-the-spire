using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal sealed class EZMicroBalanceModConfig : SimpleModConfig
{
    public static bool ModSettingsPageVisible { get; set; } = true;

    public override void SetupConfigUI(Control optionContainer)
    {
        var text = LocString.GetIfExists("settings_ui", "EZMICROBALANCE-NO_CONFIG_OPTIONS.title")?.GetFormattedText()
            ?? "No configurable options.";
        var label = CreateRawLabelControl("[center]" + text + "[/center]", 28);
        label.FitContent = true;
        optionContainer.AddChild(label);
    }
}
