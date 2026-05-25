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
        "Spire Plus is a single Slay the Spire 2 gameplay expansion for private testing. Enable this one mod to test inspectable Ancient relic choices, selected vanilla relic revisions, A11-A20 test rules, and preview tools: Blight Sprouts, Rootblight, Firemarked Elites, Banner Rooms, boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. Strong rewards are welcome only when the cost, timing, and follow-up rules are readable. Seedbed is the clearest example: it gives 8/12 Block, immediately plants deck pollution, leaves slots for later pollution, and gives Withered Husk for every planted card. Planting is not exhausting a Curse, playing a card, or discarding a card. It does not delete permanent Curses. A planted Blight Sprout is handled without being played and adds no Rootblight I. A planted Rootblight is frozen for this combat only, stays in the master deck at the same level, and does not get better or worse after combat. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是一个用于私测的《杀戮尖塔 2》单体玩法扩展。启用这一个 Mod，就能测试先古之民遗物式奖励、部分原版遗物调整、A11-A20 测试进阶与预览工具：根芽、根蚀、火印精英、战旗房、首领专属能力、烙印形态、水晶球预知和变换预览。奖励可以更强，但代价、触发时机和后续结算必须能读懂。苗床是最清楚的例子：它给 8/12 点格挡，打出时立刻种下牌堆污染，留下格子截住后续污染，每种下 1 张都会给枯壳。种下不是消耗诅咒，也不是打出或弃牌。它不会删除永久诅咒。根芽被种下后按已处理结算，战后不会生成根蚀 I。根蚀被种下后只在本战冻结，仍按同等级留在主牌组，战后不变好也不恶化。需要 BaseLib。";

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
