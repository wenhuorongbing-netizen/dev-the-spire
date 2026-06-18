using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
{
    [Fact]
    public void AncientOptionRelicTextDoesNotExposeMarkerImplementationWording()
    {
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        foreach (var key in LegacyUrdaOptionRelicKeys().Concat(CanonicalOptionRelicKeys()))
        {
            Assert.True(engRelics.TryGetValue($"{key}.description", out var engDescription), $"Missing English option relic description: {key}");
            Assert.True(zhsRelics.TryGetValue($"{key}.description", out var zhsDescription), $"Missing zhs option relic description: {key}");

            foreach (var value in new[] { engDescription, zhsDescription })
            {
                Assert.DoesNotContain("option art marker", value, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("cannot be obtained", value, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("美术标记", value, StringComparison.Ordinal);
                Assert.DoesNotContain("无法获得", value, StringComparison.Ordinal);
                Assert.Contains("[blue]", value, StringComparison.Ordinal);
            }
        }

        AssertSourceContains(
            engRelics["EZMICROBALANCE-UrdaHumusPactOptionRelic.description"],
            "[gold]Compost Reward[/gold]",
            "[blue]15[/blue] [gold]Gold[/gold]");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-UrdaSeedbedOptionRelic.description"],
            "[gold]Seedbed[/gold]",
            "[blue]2[/blue] [gold]Max HP[/gold]",
            "[gold]Temporary[/gold] Status cards",
            "[gold]Temporary[/gold] Curse cards",
            "[gold]Blight Sprouts[/gold]");
        Assert.Equal(
            engRelics["EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description"],
            engRelics["EZMICROBALANCE-UrdaMossMapOptionRelic.description"]);
        AssertSourceContains(
            engRelics["EZMICROBALANCE-UrdaMossMapOptionRelic.description"],
            "Monster +[blue]25[/blue] [gold]Gold[/gold]",
            "Event heal [blue]5[/blue]",
            "Rest Site +[blue]3[/blue] [gold]Max HP[/gold]");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description"],
            "Act [blue]1[/blue]",
            "[gold]Rain Breath[/gold]",
            "if triggered fewer than [blue]3[/blue] times",
            "[blue]75[/blue] [gold]Gold[/gold]",
            "heal [blue]8[/blue] HP");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"],
            "[blue]1[/blue] HP",
            "[blue]8[/blue] HP");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]Debt[/gold] drops by [blue]40[/blue] either way");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-UrdaHumusPactOptionRelic.description"],
            "[gold]化为腐殖[/gold]",
            "[blue]15[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-UrdaSeedbedOptionRelic.description"],
            "[gold]苗床[/gold]",
            "[blue]2[/blue]点[gold]最大生命[/gold]",
            "[gold]临时[/gold]状态牌",
            "[gold]临时[/gold]诅咒牌",
            "[gold]根芽[/gold]");
        Assert.Equal(
            zhsRelics["EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description"],
            zhsRelics["EZMICROBALANCE-UrdaMossMapOptionRelic.description"]);
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-UrdaMossMapOptionRelic.description"],
            "怪物 +[blue]25[/blue] [gold]金币[/gold]",
            "事件治疗[blue]5[/blue]",
            "休息处 +[blue]3[/blue] [gold]最大生命[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description"],
            "第[blue]1[/blue]幕",
            "[gold]雨息[/gold]",
            "少于[blue]3[/blue]次",
            "[blue]75[/blue][gold]金币[/gold]",
            "回复[blue]8[/blue]点生命");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"],
            "[gold]攻击牌[/gold]和[gold]技能牌[/gold]失去[blue]1[/blue]点生命",
            "[gold]能力牌[/gold]失去[blue]8[/blue]点生命");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]债务[/gold]照常减少[blue]40[/blue]点");
    }

    [Fact]
    public void AncientPlayerFacingCustomConceptsUseReadableRichText()
    {
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description"],
            "[blue]15[/blue] [gold]Gold[/gold]",
            "[blue]1[/blue] upgraded card reward");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "save up to [blue]3[/blue] cards",
            "the first is upgraded",
            "Click this relic later");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description"],
            "[blue]15[/blue][gold]金币[/gold]",
            "[blue]1[/blue]张已升级奖励牌");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "最多[blue]3[/blue]张",
            "第一张会升级");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.title"],
            "Root Eyes");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"],
            "[gold]Root Eyes[/gold]",
            "Click this relic on the map",
            "Monster, Unknown, or Elite",
            "enemy group or event",
            "Hover the marked room");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.title"],
            "根眼");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"],
            "[gold]根眼[/gold]",
            "点击此遗物",
            "怪物、随机或精英",
            "敌群或事件",
            "悬停标记房间");
        AssertSourceContains(
            engAncients["EZMB_URDA.root_sight.map_hover.description"],
            "previewed this room");
        AssertSourceContains(
            engAncients["EZMB_URDA.root_sight.map_hover.preview_description"],
            "previewed this result",
            "Enter this room");
        AssertSourceContains(
            engAncients["EZMB_URDA.root_sight.map_hover.event_preview_description"],
            "previewed this event",
            "{Options}");
        AssertSourceContains(
            engAncients["EZMB_URDA.root_sight.hover.description"],
            "click this relic",
            "Monster, Unknown, or Elite",
            "Hover the marked room",
            "Rest Sites, Shops, Treasure, and Boss rooms cannot be chosen");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.root_sight.map_hover.description"],
            "根眼已经预见");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.root_sight.map_hover.preview_description"],
            "根眼预见了这个结果",
            "进入该房间");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.root_sight.map_hover.event_preview_description"],
            "根眼预见了这个事件",
            "{Options}");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.root_sight.hover.description"],
            "点击此遗物",
            "怪物、随机或精英",
            "悬停标记房间",
            "不能选择篝火、商店、宝箱和首领");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title"],
            "Root Eyes");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description"],
            "Root Eyes",
            "Hover the marked room");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title"],
            "根眼");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description"],
            "根眼",
            "悬停标记房间");

        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"],
            "Choose [blue]1[/blue] of [blue]3[/blue] upgraded [gold]Ancient[/gold] cards and add it to your deck",
            "[blue]180[/blue] [gold]Gold[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[gold]Overdraft[/gold]",
            "[gold]red-ink debt[/gold]",
            "after combat");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description"],
            "[gold]Archive Pages[/gold]",
            "[blue]3[/blue]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.description"],
            "[gold]Waste Paper[/gold]",
            "[blue]4[/blue]");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"],
            "已升级的[gold]远古[/gold]牌",
            "[blue]180[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "临时[gold]透支[/gold]",
            "[gold]红墨债[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description"],
            "[gold]档案页[/gold]",
            "[blue]3[/blue]张");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.description"],
            "[gold]废纸[/gold]",
            "[blue]4[/blue]张");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt"],
            "[gold]Rebuttal Card[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt"],
            "[gold]反证牌[/gold]");

        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.description"],
            "[blue]1[/blue] [gold]red-ink debt[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.description"],
            "[blue]1[/blue]笔[gold]红墨债[/gold]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-MORVI_OVERDRAFT_POWER.description"],
            "[gold]red-ink debt[/gold]",
            "[blue]12[/blue] [gold]Gold[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-MORVI_OVERDRAFT_POWER.description"],
            "[gold]红墨债[/gold]",
            "[blue]12[/blue][gold]金币[/gold]");

        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]Debt[/gold] drops by [blue]40[/blue] either way");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]债务[/gold]照常减少[blue]40[/blue]点");
    }

    [Fact]
    public void CustomCardTextUsesCanonicalKeywordsOnlyOnce()
    {
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Contains("{Block:diff()} [gold]Block[/gold]", engCards["EZMB_URDA_SEEDLING.description"], StringComparison.Ordinal);
        Assert.Contains("{Block:diff()}点[gold]格挡[/gold]", zhsCards["EZMB_URDA_SEEDLING.description"], StringComparison.Ordinal);
        Assert.Contains("When exhausted, gain {Block:diff()} [gold]Block[/gold]", engCards["EZMB_WITHERED_HUSK.description"], StringComparison.Ordinal);
        Assert.Contains("被消耗时，获得{Block:diff()}点[gold]格挡[/gold]", zhsCards["EZMB_WITHERED_HUSK.description"], StringComparison.Ordinal);

        foreach (var key in new[]
        {
            "EZMB_URDA_SEEDLING.description",
            "EZMB_WITHERED_HUSK.description",
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.description",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description",
            "EZMB_MARGINAL_NOTE.description",
            "EZMB_MORVI_RED_INK_OVERDRAFT.description",
            "EZMB_MORVI_WASTE_PAPER.description"
        })
        {
            Assert.DoesNotContain("[gold]Exhaust[/gold]", engCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Ethereal[/gold]", engCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Unplayable[/gold]", engCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]消耗[/gold]", zhsCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]虚无[/gold]", zhsCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]无法打出[/gold]", zhsCards[key], StringComparison.Ordinal);
        }

        Assert.DoesNotContain("Retain", engCards["EZMB_MARGINAL_NOTE.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Exhaust", engCards["EZMB_MARGINAL_NOTE.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("保留", zhsCards["EZMB_MARGINAL_NOTE.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("消耗", zhsCards["EZMB_MARGINAL_NOTE.description"], StringComparison.Ordinal);

        Assert.Contains("[gold]red-ink debt[/gold]", engCards["EZMB_MORVI_RED_INK_OVERDRAFT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]红墨债[/gold]", zhsCards["EZMB_MORVI_RED_INK_OVERDRAFT.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[gold]Overdraft[/gold] debt", engCards["EZMB_MORVI_RED_INK_OVERDRAFT.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[gold]透支[/gold]债", zhsCards["EZMB_MORVI_RED_INK_OVERDRAFT.description"], StringComparison.Ordinal);
    }

}
