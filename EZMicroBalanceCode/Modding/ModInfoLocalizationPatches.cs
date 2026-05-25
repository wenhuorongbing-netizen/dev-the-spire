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
        "Spire Plus is one gameplay mod for private testing. It turns Ancient rewards into visible relic choices, revises selected vanilla relics, and adds the A11-A20 test ruleset: Blight Sprouts, Rootblight, Firemarked Elites, Banner Rooms, boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. The goal is simple: rewards can be stronger, but their costs and follow-up rules must be readable. For example, Seedbed explains exactly what Planting does to temporary negative cards, Blight Sprouts, and Rootblight. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是一个私测中的完整玩法 mod。它把先古之民奖励做成可查看的遗物选择，调整部分原版遗物，并加入 A11-A20 测试进阶：根芽、根蚀、火印精英、战旗房、首领专属能力、烙印形态、水晶球预知和变换预览。核心目标很直接：奖励可以更强，但代价和后续处理必须讲清楚。比如苗床会明确说明种下怎样处理临时负面牌、根芽和根蚀。需要 BaseLib。";

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
        var useSimplifiedChinese = string.Equals(language, "zhs", StringComparison.Ordinal) ||
            string.Equals(language, "zh-Hans", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(language, "zh_CN", StringComparison.OrdinalIgnoreCase);
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
