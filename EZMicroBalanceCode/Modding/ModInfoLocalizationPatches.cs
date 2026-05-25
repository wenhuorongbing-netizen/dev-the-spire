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
        "Spire Plus is a single Slay the Spire 2 gameplay expansion for private testing. Enable this one mod to test inspectable Ancient relic choices, selected vanilla relic revisions, and the A11-A20 test ruleset: Blight Sprouts, Rootblight, Firemarked Elites, Banner Rooms, boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. The design goal is stronger rewards with readable costs: the player should know what is gained now, what is paid later, and how the combat state will resolve. Seedbed shows that rule clearly. It is a 1-cost defense card that gives 8/12 Block, immediately plants 1/2 eligible cards from the draw or discard pile, leaves 2/3 slots for later pollution, and gives 1 Withered Husk for each planted card. Planting means combat-only isolation. The card leaves this combat before entering hand; it does not count as play, discard, or Exhaust, so those triggers and exhaust-Curse payoffs do not fire. Permanent Curses are not deleted. Temporary Status and Curse cards are gone for this combat. A planted Blight Sprout is treated as handled for this combat and adds no Rootblight I after combat. A planted Rootblight is frozen for this combat only: it stays in the master deck at the same level, with no upgrade, split, removal, downgrade, or cleanup. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是用于私测的《杀戮尖塔 2》单体玩法扩展。启用这一个 Mod，就能测试可悬停查看的先古之民遗物奖励、部分原版遗物调整、A11-A20 测试进阶、根芽、根蚀、火印精英、战旗房、首领专属能力、王烙印、水晶球预知和变换预览。设计目标是奖励更强，代价更清楚：玩家应当知道现在拿到了什么，之后要付出什么，战斗结束时会怎样结算。苗床是典型例子。它是 1 费防御牌，给 8/12 点格挡；打出时先从抽牌堆或弃牌堆种下 1/2 张可种下的牌；之后留下 2/3 格继续拦截污染；每种下 1 张牌，获得 1 张枯壳。种下就是“本战隔离”：牌在进入手牌前离开本场战斗，不算打出、丢弃或消耗，不触发这些联动，也不触发消耗诅咒收益。永久诅咒不会被删除。临时状态牌和临时诅咒牌本战不再出现。根芽被种下后按本战已处理结算，战后不会生成根蚀 I。根蚀被种下后只冻结这一场：仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级，也不会被净化。需要 BaseLib。";

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
