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
        "Spire Plus is one gameplay mod for private testing. It reworks Ancient rewards into visible relic choices, revises selected vanilla relics, adds A11-A20 test progression, Rootblight and Blight Sprouts, Firemarked Elites, Banner Rooms, boss dedicated abilities, Crystal Sphere peek, and transform preview. The design goal is clear tradeoffs: stronger rewards, visible costs, and enough information for players to plan. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是一个私测中的完整玩法 mod。它把先古之民奖励做成可见遗物选择，调整部分原版遗物，并加入 A11-A20 测试进阶、根芽/根蚀、火印精英、战旗房、首领专属能力、水晶球预知和变换预览。设计目标是让每个强奖励都有清楚代价，让玩家能提前判断路线、战斗和牌组压力。需要 BaseLib。";

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
