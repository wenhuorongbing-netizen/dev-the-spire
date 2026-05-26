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
        "Spire Plus is a single Slay the Spire 2 gameplay expansion for private testing. Enable this one mod to test Ancient relic rewards, selected relic revisions, A11-A20 test rules, Firemarked Elites, Banner Rooms, Boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. The design goal is readable risk: every reward should tell you what you gain now, what cost you accept, and what remains after combat. Seedbed shows that style. It is a 1-cost Skill that gives 8/12 Block, sets up 2/3 Seedbed spaces, immediately plants 1/2 eligible cards from draw or discard, then uses remaining spaces to catch later pollution before it enters hand. Planting removes a card from this combat before it reaches hand and gives 1 Withered Husk. Planting does not count as play, discard, or Exhaust, and it triggers none of those payoffs. Temporary Status and Curse cards disappear after combat. Permanent Curses are not permanently deleted. A planted Blight Sprout is handled for this combat, does not enter hand, does not grow, and adds no Rootblight I. A planted Rootblight pauses this combat's end check: it does not cleanse, downgrade, worsen, or split, and it keeps the same stage after combat. Seedbed is valuable because it gives Block now, prevents a pollution draw, and turns each planted card into a Husk that later Exhausts for 3 Block. The Mod Settings panel localizes this description by client language. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是用于私测的《杀戮尖塔 2》单体玩法扩展。启用这一个 Mod，就能测试先古之民遗物奖励、部分原版遗物调整、A11-A20 测试进阶、火印精英、战旗房、首领专属能力、烙印形态、水晶球预知和变换预览。设计目标是让风险读得懂：玩家要知道现在拿到什么、之后付出什么、战斗结束时留下什么。苗床是这个风格的例子。它是 1 费技能牌，获得 8/12 点格挡，设置 2/3 格苗床，打出时先从抽牌堆或弃牌堆种下 1/2 张合格牌，剩余格数继续拦截之后进入手牌的污染。种下会在牌进入手牌前把它移出本场战斗，并给你 1 张枯壳；不计为打出、弃牌或关键词消耗，也不会触发这些收益。临时状态牌和临时诅咒牌战后消失。永久诅咒不会被永久删除。根芽被种下后本场已处理：不进手、不生长、战后不生成根蚀 I。根蚀被种下后暂停本场结束检查：不净化、不降级、不恶化、不分裂，战后保持原阶段。苗床的价值来自即时格挡、少抽一次污染牌、以及每张枯壳之后被消耗时的 3 点格挡。Mod 设置页会根据客户端语言显示这段介绍。需要 BaseLib。";

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
