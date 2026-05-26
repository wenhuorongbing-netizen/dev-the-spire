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
        "Spire Plus is a single Slay the Spire 2 gameplay expansion for private testing. Enable this one mod to test inspectable Ancient relic choices, selected vanilla relic revisions, A11-A20 test rules, Firemarked Elites, Banner Rooms, boss dedicated abilities, Branded Form, Crystal Sphere peek, and transform preview. The design goal is stronger rewards with readable costs: the player should know what is gained now, what is paid later, and how combat will resolve. Seedbed is the clearest example. It is a 1-cost defense card that gives 8/12 Block and sets up 2/3 total Seedbed spaces. On play, it can spend 1/2 of those spaces to plant eligible draw or discard pile cards immediately; any remaining spaces catch later pollution before it enters hand. Planting means combat-only isolation, not play, discard, or the Exhaust keyword. It does not permanently delete Curses or trigger Exhaust-Curse payoffs. Temporary Status and Curse cards leave the fight and disappear after combat. A planted Blight Sprout is handled for this combat without actually being played and adds no Rootblight I after combat. A planted Rootblight is frozen for this combat only: it does not improve, worsen, split, remove, downgrade, or clean itself, and it stays in the master deck at the same level after combat. Each successful plant gives 1 Withered Husk, so Seedbed prevents a bad draw now and turns it into 3 Block later. Requires BaseLib.";

    private const string SimplifiedChineseDescription =
        "Spire Plus 是一个用于私测的《杀戮尖塔 2》单体玩法扩展。启用这一个 Mod，就能测试可悬停查看的先古之民遗物奖励、部分原版遗物调整、A11-A20 测试进阶、火印精英、战旗房、首领专属能力、烙印形态、水晶球预知和变换预览。设计目标是奖励更强，代价更清楚：玩家应该知道现在拿到什么，之后要付出什么，战斗结算时会发生什么。苗床是最典型的例子。它是 1 费防御牌，获得 8/12 点格挡，并设置总共 2/3 格苗床。打出时，可以先用掉其中 1/2 格，从抽牌堆或弃牌堆种下合格牌；剩余格数继续拦截之后进手的污染牌。种下就是本战隔离，不是打出、弃牌或关键词消耗。它不会永久删除诅咒，也不会触发消耗诅咒的收益。临时状态牌和临时诅咒牌会离开本战，战后消失。根芽被种下后按已处理结算，效果接近本场解决了它，但不算真的打出，战后不会生成根蚀 I。根蚀被种下后只冻结这一场：不变好、不恶化、不分裂、不移除、不降级、不净化，战后仍以原等级留在主牌组。每种下 1 张牌，获得 1 张枯壳，所以苗床是在现在少抽一张污染牌，并把它换成之后 3 点格挡。需要 BaseLib。";

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
