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
        "Spire Plus is a single Slay the Spire 2 gameplay expansion for private testing. It turns Ancient rewards into inspectable relic-style choices, revises selected vanilla relics, and adds the A11-A20 test ruleset plus preview tools: Blight Sprouts, Rootblight, Firemarked Elites, Banner Rooms, boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. The design goal is direct: stronger rewards are welcome only when the cost, timing, and follow-up rules are readable. Seedbed is the model case: it gives real Block, plants temporary negative cards before they enter hand, handles Blight Sprouts without adding Rootblight, and freezes Rootblight for one combat without deleting it. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是一个用于私测的《杀戮尖塔 2》玩法扩展。它把先古之民奖励做成可以查看的遗物式选择，调整部分原版遗物，并加入 A11-A20 测试进阶与预览工具：根芽、根蚀、火印精英、战旗房、首领专属能力、烙印形态、水晶球预知和变换预览。设计目标很直接：奖励可以更强，但代价、触发时机和后续结算必须能读懂。苗床就是这个标准：它先给格挡，在负面牌进手前种下它；根芽会被处理，不会长成根蚀；根蚀只在本场停住，不会被删除。需要 BaseLib。";

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
