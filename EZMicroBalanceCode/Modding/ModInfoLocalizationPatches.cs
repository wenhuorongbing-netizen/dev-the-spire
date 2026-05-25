using System;
using System.Text;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;

namespace EZMicroBalance.EZMicroBalanceCode.Modding;

[HarmonyPatch(typeof(NModInfoContainer), nameof(NModInfoContainer.Fill))]
internal static class ModInfoLocalizationPatches
{
    private const string EnglishDescription =
        "Spire Plus expands the run with Ancient blessings, A11-A20 progression, Rootblight pressure, banner rooms, Firemarked Elites, boss dedicated abilities, and preview tools. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 扩展先古奖励、A11-A20 进阶、根芽/根蚀、战旗房、火印精英、首领专属能力和预见工具。需要 BaseLib。";

    private static void Postfix(NModInfoContainer __instance, Mod mod)
    {
        if (!string.Equals(mod.manifest?.id, MainFile.ModId, StringComparison.Ordinal))
        {
            return;
        }

        var description = __instance.GetNodeOrNull<MegaRichTextLabel>("ModDescription");
        if (description == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Could not localize Mod Settings description because ModDescription node was not found.");
            return;
        }

        description.Text = BuildLocalizedDescription(mod);
    }

    private static string BuildLocalizedDescription(Mod mod)
    {
        var language = LocManager.Instance?.Language;
        var useSimplifiedChinese = string.Equals(language, "zhs", StringComparison.Ordinal);
        var builder = new StringBuilder();

        builder.AppendLine(useSimplifiedChinese
            ? $"[gold]作者[/gold]: {mod.manifest?.author ?? "unknown"}"
            : $"[gold]Author[/gold]: {mod.manifest?.author ?? "unknown"}");
        builder.AppendLine(useSimplifiedChinese
            ? $"[gold]版本[/gold]: {mod.manifest?.version ?? "unknown"}"
            : $"[gold]Version[/gold]: {mod.manifest?.version ?? "unknown"}");
        builder.AppendLine();
        builder.AppendLine(useSimplifiedChinese ? SimplifiedChineseDescription : EnglishDescription);

        return builder.ToString();
    }
}
