using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientPlayerFacingPolishGuardTests
{
    private static readonly string[] CharacterDialogueKeys =
    [
        "IRONCLAD",
        "SILENT",
        "DEFECT",
        "NECROBINDER",
        "REGENT"
    ];

    [Fact]
    public void ActiveAncientDialogueSlotsHaveReachableBilingualText()
    {
        var urda = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.cs");
        var morvi = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var lotha = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        foreach (var source in new[] { urda, morvi, lotha })
        {
            Assert.DoesNotContain("new AncientDialogue(\"\")", source, StringComparison.Ordinal);
            Assert.Contains("AncientDialogueLine.sfxFallbackPath", source, StringComparison.Ordinal);
        }

        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient");
        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient");
        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_MORVI.talk.firstVisitEver.0-0.ancient");
        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_MORVI.talk.ANY.0-0r.ancient");
        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_LOTHA.talk.firstVisitEver.0-0.ancient");
        AssertNonEmpty(engAncients, zhsAncients, "EZMICROBALANCE-EZMB_LOTHA.talk.ANY.0-0r.ancient");

        foreach (var character in CharacterDialogueKeys)
        {
            AssertNonEmpty(engAncients, zhsAncients, $"EZMICROBALANCE-EZMB_MORVI.talk.{character}.0-0r.ancient");
            AssertNonEmpty(engAncients, zhsAncients, $"EZMICROBALANCE-EZMB_LOTHA.talk.{character}.0-0r.ancient");
        }
    }

    [Fact]
    public void ActiveAncientLocalizationDoesNotExposeDevelopmentTermsOrRawTokens()
    {
        var maps = new[]
        {
            JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json"),
            JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json"),
            JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json"),
            JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json"),
            JsonStringMap("EZMicroBalance", "localization", "eng", "card_reward_ui.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "card_reward_ui.json"),
            JsonStringMap("EZMicroBalance", "localization", "eng", "encounters.json"),
            JsonStringMap("EZMicroBalance", "localization", "zhs", "encounters.json")
        };

        foreach (var (key, value) in ActiveAncientValues(maps))
        {
            foreach (var banned in new[]
            {
                "source-safe",
                "source-ready",
                "private-beta",
                "test-ready",
                "prototype",
                "pending",
                "manual verification",
                "common/uncommon",
                "Firemark Host",
                "setup window",
                "burst window",
                "setup instead of burst",
                "debug",
                "option art marker",
                "cannot be obtained",
                "美术标记",
                "无法获得",
                "源码安全",
                "源码就绪",
                "测试就绪",
                "内测",
                "普通/罕见",
                "火印宿主",
                "蓄势窗口",
                "爆发窗口",
                "{energyPrefix",
                "energyIcons(1)",
                "\uFFFD",
                "铏",
                "锟"
            })
            {
                Assert.DoesNotContain(banned, value, StringComparison.OrdinalIgnoreCase);
            }

            Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
            Assert.False(string.IsNullOrWhiteSpace(value), $"Empty active localization value: {key}");
        }
    }

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
            "[gold]Seedling[/gold]",
            "[blue]10[/blue] [gold]Max HP[/gold]",
            "The first is upgraded",
            "without healing current HP");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description"],
            "Act [blue]1[/blue] [gold]Elite[/gold] kills",
            "[blue]20[/blue] [gold]Gold[/gold]",
            "max [blue]2[/blue]");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"],
            "[blue]1[/blue] HP",
            "[blue]8[/blue] HP");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]Debt[/gold] drops by the due amount either way");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-UrdaHumusPactOptionRelic.description"],
            "[gold]化为腐殖[/gold]",
            "[blue]15[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-UrdaSeedbedOptionRelic.description"],
            "[gold]幼芽[/gold]",
            "[blue]10[/blue]点[gold]最大生命[/gold]",
            "第一张会升级",
            "不回复当前生命");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description"],
            "击败第[blue]1[/blue]幕[gold]精英[/gold]",
            "[blue]20[/blue][gold]金币[/gold]",
            "最多[blue]2[/blue]次");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"],
            "[gold]攻击牌[/gold]和[gold]技能牌[/gold]失去[blue]1[/blue]点生命",
            "[gold]能力牌[/gold]失去[blue]8[/blue]点生命");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]债务[/gold]都会减少到期数值");
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
            "[gold]Compost Reward[/gold]",
            "[blue]3[/blue] composts");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "[gold]Store Seed[/gold]",
            "[gold]Seed[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description"],
            "[gold]化为腐殖[/gold]",
            "[blue]3[/blue]次");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "[gold]储存种子[/gold]",
            "[gold]种子[/gold]");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"],
            "[gold]Root Eyes[/gold]",
            "immediately marks [blue]1[/blue] reachable non-Boss room",
            "after each room you enter");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"],
            "[gold]根眼[/gold]",
            "立即标记[blue]1[/blue]个可到达的非首领房间",
            "每进入一个房间，再标记一个");
        Assert.DoesNotContain("reveal", engAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("提前揭示", zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_root_sight.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_URDA.root_sight.map_hover.description"],
            "marked this reachable room",
            "no extra penalty");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.root_sight.map_hover.description"],
            "标出的可到达房间",
            "没有额外惩罚");

        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"],
            "[gold]Borrowed[/gold]",
            "[blue]180[/blue] [gold]Gold[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[gold]red-ink debt[/gold]",
            "[blue]1[/blue] [gold]red-ink debt[/gold]");
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
            "[gold]借来的牌[/gold]",
            "[blue]180[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[blue]1[/blue]张临时[gold]透支[/gold]",
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
            "[gold]Debt[/gold] drops by the due amount either way");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description"],
            "[gold]债务[/gold]都会减少到期数值");
    }

    [Fact]
    public void CustomCardTextUsesCanonicalKeywordsOnlyOnce()
    {
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Contains("{Block:diff()} [gold]Block[/gold]", engCards["EZMB_URDA_SEEDLING.description"], StringComparison.Ordinal);
        Assert.Contains("{Block:diff()}点[gold]格挡[/gold]", zhsCards["EZMB_URDA_SEEDLING.description"], StringComparison.Ordinal);
        Assert.Contains("{Block:diff()} [gold]Block[/gold]", engCards["EZMB_WITHERED_HUSK.description"], StringComparison.Ordinal);
        Assert.Contains("{Block:diff()}点[gold]格挡[/gold]", zhsCards["EZMB_WITHERED_HUSK.description"], StringComparison.Ordinal);

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

    [Fact]
    public void AncientOptionHoversPreviewNamedAddedCardsWhereSupported()
    {
        var urda = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.cs");
        var urdaMapUiPatches = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs");
        var morvi = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var vakuu = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");

        AssertSourceContains(
            urda,
            "HoverTipFactory.FromCardWithCardHoverTips<UrdaSeedling>()",
            "HoverTipFactory.FromCardWithCardHoverTips<WitheredHusk>()");
        AssertSourceContains(
            urdaMapUiPatches,
            "UrdaRootSightMapQuestMarker",
            "NHoverTipSet.CreateAndShow",
            "EZMB_URDA.root_sight.map_hover.title",
            "EZMB_URDA.root_sight.map_hover.description");
        AssertSourceContains(
            morvi,
            "HoverTipFactory.FromCardWithCardHoverTips<MorviRedInkOverdraftCard>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDrawPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveVeilPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBurnPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDiscountPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBraveryPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDexterityPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviWastePaper>()");
        AssertSourceContains(
            vakuu,
            "HoverTipFactory.FromCardWithCardHoverTips<VakuuTemptation>()");
    }

    [Fact]
    public void UrdaSeedBankTextMatchesNoTrialPlantMarkerSource()
    {
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var seedBankSource = SliceBetween(urdaRunHook, "private static async Task ChooseSeedBankStore", "public static async Task ApplyTrialBranch");

        Assert.DoesNotContain("UrdaTrialPlantCard", seedBankSource, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "The first chosen card is upgraded",
            "Unchosen [gold]Seed[/gold] cards disappear");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "第一张所选牌会升级",
            "未选择的[gold]种子[/gold]会消失");

        foreach (var value in new[]
        {
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            engRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"],
            zhsRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"]
        })
        {
            Assert.DoesNotContain("Trial Plant", value, StringComparison.Ordinal);
            Assert.DoesNotContain("试炼植株", value, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VakuuFightTextAndSourceStayExplicitAboutRiskAndRewards()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.DoesNotContain("TaskHelper.RunSafely", patch, StringComparison.Ordinal);
        Assert.Contains("await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom", patch, StringComparison.Ordinal);
        Assert.Contains("CreateVictoryFallbackOption", patch, StringComparison.Ordinal);
        Assert.Contains("VictoryFallbackDescriptionKey", patch, StringComparison.Ordinal);
        Assert.Contains("options.Count == 3 ? options", patch, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch, StringComparison.Ordinal);
        Assert.Contains("base(RoomType.Event, autoAdd: false)", encounter, StringComparison.Ordinal);
        Assert.Contains("ShouldGiveRewards => false", encounter, StringComparison.Ordinal);
        Assert.Contains("runState.Players.Count == 1", gate, StringComparison.Ordinal);

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "real fight",
            "After your hand is drawn",
            "no normal combat rewards",
            "non-Vakuu Act [blue]3[/blue] Ancient blessings",
            "If you die, the run ends");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "真正战斗",
            "抽完起始手牌后",
            "不会掉落普通战斗奖励",
            "非瓦库",
            "若你死亡，本局结束");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "real fight",
            "After your hand is drawn",
            "No normal combat rewards",
            "If you die, the run ends");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "真正战斗",
            "抽完起始手牌后",
            "不会掉落普通战斗奖励",
            "死亡则本局结束");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.title");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.description");
    }

    [Fact]
    public void TemptationDocsNoLongerDescribeImplementedGameplayAsUnimplemented()
    {
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var issue = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");

        Assert.Contains("Temptation", sourceDesign, StringComparison.Ordinal);
        Assert.Contains("Temptation", issue, StringComparison.Ordinal);
        Assert.DoesNotContain("Temptation remains not implemented", sourceDesign, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Temptation remains not implemented", issue, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("future content and is not implemented", sourceDesign, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("future content and was not implemented", issue, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ArtDirectionDoesNotClaimTemporaryAssetsAreFinal()
    {
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");

        AssertSourceContains(
            artDirection,
            "Final browser GPTimage2 small art generated this pass",
            "Urda event background: Active event art is a 2.13:1 reframe",
            "Urda, Morvi, and Lotha option/icon art uses browser ChatGPT/GPTimage2 rebuilt transparent PNGs",
            "map and run-history pairs intentionally share final browser GPTimage2 filled/outline bytes",
            "Custom card portraits now use browser GPTimage2 rebuilt files",
            "Vakuu fight option art uses the same browser GPTimage2 rebuild pass",
            "No `generic_temporary` or `final_required_before_release` art blockers remain",
            "Vakuu Temptation");
    }

    [Fact]
    public void CurrentDocsDoNotRegressUrdaMorviOrLothaPowerStatus()
    {
        var currentDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "README.md"),
            ReadRepoText("docs", "doc-inventory.md"),
            ReadRepoText("docs", "issues", "urda.md"),
            ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "milestone-roadmap.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md"),
            ReadRepoText("docs", "features", "ancient-expansion-urda", "work-log.md"),
            ReadRepoText("docs", "private-beta-release-completion-audit.md"));

        Assert.Contains("ten source-backed blessings", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Morvi is default-on", currentDocs, StringComparison.Ordinal);
        Assert.Contains("draw 1 with no Energy gain", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("four source-backed blessings", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Urda default-on four-blessing slice", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Morvi remains default-off", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Morvi has a separate default-off prototype gate", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Power fallbacks grant 1 Energy and draw 1", currentDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void AscensionTextHighlightsRootblightBannerFiremarkAndBossTerms()
    {
        var engAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            engAscension["LEVEL_14.description"],
            "[gold]Rootblight I[/gold]",
            "[blue]4[/blue]",
            "[gold]Rootblights[/gold]");
        AssertSourceContains(
            engAscension["LEVEL_15.description"],
            "[gold]Blight Sprouts[/gold]",
            "[blue]3[/blue]",
            "[blue]4[/blue]");
        AssertSourceContains(
            engAscension["LEVEL_16.description"],
            "[gold]Banner Rooms[/gold]",
            "[gold]Banner[/gold]",
            "extra rewards");
        AssertSourceContains(
            engAscension["FIREMARK_ELITE.description"],
            "[gold]Firemarked Elite[/gold]");
        AssertSourceContains(
            engAscension["BOSS_ROYAL_SEAL.description"],
            "[gold]Royal Seal[/gold]");
        AssertSourceContains(
            engAscension["BOSS_KING_BRAND.description"],
            "[gold]King Brand[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]3[/blue]",
            "[blue]14[/blue]",
            "[gold]Block[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_AEONGLASS_STRENGTH.summary"],
            "+[blue]5[/blue]",
            "[gold]Strength[/gold]");
        AssertSourceContains(
            engAscension["ROOTBLIGHT_ADDED"],
            "[gold]Rootblight[/gold]");
        AssertSourceContains(
            engAscension["ROOT_SYSTEM_FULL"],
            "max [blue]4[/blue]",
            "[gold]Rootblights[/gold]");

        AssertSourceContains(
            zhsAscension["LEVEL_14.description"],
            "[gold]",
            "[blue]4[/blue]");
        AssertSourceContains(
            zhsAscension["LEVEL_15.description"],
            "[blue]2[/blue]",
            "[blue]3[/blue]",
            "[blue]4[/blue]",
            "[gold]");
        AssertSourceContains(
            zhsAscension["LEVEL_16.description"],
            "[gold]",
            "额外奖励");
        AssertSourceContains(
            zhsAscension["FIREMARK_ELITE.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_ROYAL_SEAL.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_KING_BRAND.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]3[/blue]",
            "[blue]14[/blue]",
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_AEONGLASS_STRENGTH.summary"],
            "+[blue]5[/blue]",
            "[gold]");
        AssertSourceContains(
            zhsAscension["ROOTBLIGHT_ADDED"],
            "[gold]根蚀[/gold]");
        AssertSourceContains(
            zhsAscension["ROOT_SYSTEM_FULL"],
            "[blue]4[/blue]",
            "[gold]根蚀[/gold]");
    }

    [Fact]
    public void ForgeTokenTextDoesNotExposeTemporaryDevelopmentWording()
    {
        var forgeToken = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");

        AssertSourceContains(
            forgeToken,
            "Only [gold]Rest[/gold] or [gold]Smith[/gold] spends this token",
            "只有[gold]休息[/gold]或[gold]锻造[/gold]会消耗铸令");
        Assert.DoesNotContain("do not spend this yet", forgeToken, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("暂不消耗", forgeToken, StringComparison.Ordinal);
    }

    private static IEnumerable<(string Key, string Value)> ActiveAncientValues(IEnumerable<IReadOnlyDictionary<string, string>> maps)
    {
        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (key.Contains("EZMB_URDA", StringComparison.Ordinal) ||
                    key.Contains("EZMB_MORVI", StringComparison.Ordinal) ||
                    key.Contains("EZMB_LOTHA", StringComparison.Ordinal) ||
                    key.Contains("EZMB_VAKUU", StringComparison.Ordinal) ||
                    key.Contains("URDA_", StringComparison.Ordinal) ||
                    key.Contains("MORVI_", StringComparison.Ordinal) ||
                    key.Contains("LOTHA_", StringComparison.Ordinal) ||
                    key.Contains("VAKUU_", StringComparison.Ordinal) ||
                    key.Contains("VAKUU.pages.INITIAL.options.ezmb_vakuu_fight", StringComparison.Ordinal))
                {
                    yield return (key, value);
                }
            }
        }
    }

    private static IEnumerable<string> LegacyUrdaOptionRelicKeys()
    {
        yield return "EZMICROBALANCE-UrdaHumusPactOptionRelic";
        yield return "EZMICROBALANCE-UrdaMoltingOptionRelic";
        yield return "EZMICROBALANCE-UrdaMossMapOptionRelic";
        yield return "EZMICROBALANCE-UrdaSeedbedOptionRelic";
    }

    private static IEnumerable<string> CanonicalOptionRelicKeys()
    {
        foreach (var key in new[]
        {
            "URDA_HUMUS_PACT",
            "URDA_MOLTING",
            "URDA_MOSS_MAP",
            "URDA_SEEDBED",
            "URDA_TRIAL_BRANCH",
            "URDA_SHALLOW_ROOT_RELIC",
            "URDA_ROOTED_ROUTE",
            "URDA_AFTER_RAIN",
            "URDA_ROOT_SIGHT",
            "URDA_SEED_BANK",
            "MORVI_FORBIDDEN_LOAN",
            "MORVI_MISPRINT_PRESS",
            "MORVI_RED_INK_OVERDRAFT",
            "MORVI_OVERDUE_LIBRARY",
            "MORVI_OPEN_BOOK_EXAM",
            "MORVI_PAPERSTORM",
            "MORVI_BLUEPRINT_PROOF",
            "MORVI_DEBT_SETTLEMENT",
            "LOTHA_MIRROR_REBUTTAL",
            "LOTHA_MIRROR_HALL_ECHO",
            "LOTHA_PRESUMPTION",
            "LOTHA_CLOSED_COURT",
            "LOTHA_DEFERRED_VERDICT",
            "LOTHA_DEATH_REPRIEVE",
            "LOTHA_SINGLE_SENTENCE",
            "LOTHA_PUBLIC_EVIDENCE",
            "VAKUU_FIGHT"
        })
        {
            yield return $"EZMICROBALANCE-{key}_OPTION_RELIC";
        }
    }

    private static void AssertNonEmpty(
        IReadOnlyDictionary<string, string> eng,
        IReadOnlyDictionary<string, string> zhs,
        string key)
    {
        Assert.True(eng.TryGetValue(key, out var engValue), $"Missing English localization key: {key}");
        Assert.True(zhs.TryGetValue(key, out var zhsValue), $"Missing zhs localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(engValue), $"Empty English localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs localization key: {key}");
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end > start, $"Missing end marker after {startMarker}: {endMarker}");
        return source[start..end];
    }

    private static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.String, property.Value.ValueKind);
            map.Add(property.Name, property.Value.GetString() ?? string.Empty);
        }

        return map;
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
