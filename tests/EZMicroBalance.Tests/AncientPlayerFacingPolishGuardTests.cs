using System.Text;
using System.Text.RegularExpressions;
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
        var urda = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var morvi = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var lotha = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
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
                "unfinished Vakuu challenge",
                "placeholder enemy",
                "default combat scene",
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
                "未完成的瓦库挑战",
                "占位敌人",
                "普通场景",
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

            Assert.DoesNotMatch("不是.*而是", value);
            Assert.DoesNotMatch("(?i)not\\s+.+\\s+but", value);
            Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
            Assert.False(string.IsNullOrWhiteSpace(value), $"Empty active localization value: {key}");
        }
    }

    [Fact]
    public void EnglishAndSimplifiedChineseLocalizationStayInParity()
    {
        var engDir = RepoPath("EZMicroBalance", "localization", "eng");
        var zhsDir = RepoPath("EZMicroBalance", "localization", "zhs");
        var engFiles = Directory.GetFiles(engDir, "*.json")
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var zhsFiles = Directory.GetFiles(zhsDir, "*.json")
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(engFiles, zhsFiles);

        foreach (var fileName in engFiles)
        {
            var eng = JsonStringMap("EZMicroBalance", "localization", "eng", fileName!);
            var zhs = JsonStringMap("EZMicroBalance", "localization", "zhs", fileName!);

            Assert.Equal(
                eng.Keys.OrderBy(key => key, StringComparer.Ordinal),
                zhs.Keys.OrderBy(key => key, StringComparer.Ordinal));

            foreach (var key in eng.Keys.OrderBy(key => key, StringComparer.Ordinal))
            {
                var engValue = eng[key];
                var zhsValue = zhs[key];

                Assert.False(string.IsNullOrWhiteSpace(engValue), $"Empty English localization: {fileName}:{key}");
                Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs localization: {fileName}:{key}");
                AssertBalancedRichTextTags(fileName!, key, engValue);
                AssertBalancedRichTextTags(fileName!, key, zhsValue);
                Assert.Equal(DynamicVariableNames(engValue), DynamicVariableNames(zhsValue));
            }
        }
    }

    [Fact]
    public void SimplifiedChineseLocalizationFilesDoNotContainMojibake()
    {
        var zhsLocalizationDir = RepoPath("EZMicroBalance", "localization", "zhs");
        var mojibakeFragments = new[]
        {
            "闁革腹",
            "闁汇劌",
            "缂佹",
            "鐎殿",
            "闁?",
            "闁告帒娲╅崐?",
            "闁烩槄绠戠花?",
            "闂侇偄顦扮€?",
            "闁兼儳鍢茬欢?",
            "闁瑰瓨蓱閺?",
            "缂佸顭峰▍?",
            "閻犳劕婀遍弫?",
            "闁哄秴鍚嬬亸?",
            "闁稿﹤鎼慨?",
            "缂傚倹鎸诲﹢?",
            "妤犵",
            "闂傚洠鍋撻悷?",
            "闁哄啰濮磋ぐ?"
        };

        foreach (var file in Directory.GetFiles(zhsLocalizationDir, "*.json"))
        {
            var bytes = File.ReadAllBytes(file);
            Assert.True(
                bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF,
                $"{Path.GetFileName(file)} should stay UTF-8 with BOM so Simplified Chinese text opens cleanly in Windows tools.");

            foreach (var (key, value) in JsonStringMap(file))
            {
                Assert.DoesNotContain('\uFFFD', value);
                Assert.DoesNotContain(value, static ch => ch is >= '\uE000' and <= '\uF8FF');

                foreach (var fragment in mojibakeFragments)
                {
                    Assert.False(
                        value.Contains(fragment, StringComparison.Ordinal),
                        $"{Path.GetFileName(file)}:{key} still contains mojibake fragment '{fragment}'.");
                }
            }
        }
    }

    [Fact]
    public void ActiveModSourceDoesNotContainKnownMojibakeFragments()
    {
        var source = ReadSourceTree("EZMicroBalanceCode");
        var fragments = new[]
        {
            "\uFFFD",
            "鐏",
            "鎴",
            "绗",
            "鍥",
            "浼",
            "澶",
            "鏁",
            "銆",
            "闂",
            "瑁",
            "閾",
            "璇",
            "鏈",
            "寮€",
            "鑾",
            "缂",
            "顭",
            "娑",
            "锟",
            "铏"
        };

        var matches = fragments
            .Where(fragment => source.Contains(fragment, StringComparison.Ordinal))
            .ToArray();

        Assert.True(matches.Length == 0, "Found mojibake fragments in active C# source: " + string.Join(", ", matches));
    }

    [Fact]
    public void ActiveCurrentDocsDoNotContainKnownMojibakeFragments()
    {
        var docs = Directory.GetFiles(RepoPath("docs"), "*.md", SearchOption.AllDirectories)
            .Where(path => !ToRepoRelativePath(path).StartsWith("docs/archive/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var fragments = new[]
        {
            "\uFFFD",
            "涓",
            "鑰",
            "璇",
            "鐜",
            "鐏",
            "閻",
            "娑",
            "鍋",
            "婵",
            "锟",
            "妫",
            "锛",
            "鑾",
            "鎵",
            "閲",
            "瀹",
            "绁",
            "鏀",
            "璁",
            "鍊",
            "杩",
            "鐐",
            "寮傝壊",
            "鐗",
            "鎰"
        };

        var matches = docs
            .SelectMany(path =>
            {
                var text = File.ReadAllText(path, Encoding.UTF8);
                return fragments
                    .Where(fragment => text.Contains(fragment, StringComparison.Ordinal))
                    .Select(fragment => $"{ToRepoRelativePath(path)}:{fragment}");
            })
            .ToArray();

        Assert.True(matches.Length == 0, "Found mojibake fragments in active docs: " + string.Join(", ", matches));
    }

    [Fact]
    public void ActiveCurrentDocsInlineCodeBackticksAreBalancedOutsideFences()
    {
        var docs = Directory.GetFiles(RepoPath("docs"), "*.md", SearchOption.AllDirectories)
            .Where(path => !ToRepoRelativePath(path).StartsWith("docs/archive/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var failures = new List<string>();

        foreach (var path in docs)
        {
            var inFence = false;
            var lineNumber = 0;
            foreach (var line in File.ReadLines(path, Encoding.UTF8))
            {
                lineNumber++;
                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    inFence = !inFence;
                    continue;
                }

                if (inFence)
                {
                    continue;
                }

                var tickCount = line.Count(ch => ch == '`');
                if (tickCount % 2 != 0)
                {
                    failures.Add($"{ToRepoRelativePath(path)}:{lineNumber} has unbalanced inline backticks: {line}");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
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
            "fewer than [blue]3[/blue] triggers",
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
            "[blue]1[/blue] upgraded reward card");
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "max [blue]3[/blue]",
            "first chosen card is upgraded",
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

    [Fact]
    public void AncientOptionHoversPreviewNamedAddedCardsWhereSupported()
    {
        var urda = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaMapUiPatches = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapClickPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapPreviewVisuals.cs"),
            ReadRepoText("EZMicroBalanceCode", "Map", "SpirePlusMapPointHoverComposer.cs"));
        var morvi = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var vakuu = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");

        AssertSourceContains(
            urda,
            "HoverTipFactory.FromCard<UrdaSeedbed>()",
            "HoverTipFactory.FromCard<WitheredHusk>()",
            "RootSightHoverTips",
            "EZMB_URDA.root_sight.hover.title",
            "EZMB_URDA.root_sight.hover.description");
        Assert.DoesNotContain("HoverTipFactory.FromCardWithCardHoverTips<UrdaSeedbed>()", urda, StringComparison.Ordinal);
        Assert.DoesNotContain("HoverTipFactory.FromCardWithCardHoverTips<WitheredHusk>()", urda, StringComparison.Ordinal);
        AssertSourceContains(
            urdaMapUiPatches,
            "%QuestIcon",
            "MouseFilterEnum.Ignore",
            "SpirePlusMapPointHoverComposer",
            "UrdaBlessingService.TryGetRootSightHoverTip",
            "FiremarkedEliteMapHoverPatch.TryCreateHoverTip",
            "BannerRoomMapHoverPatch.TryCreateHoverTip",
            "TryGetRootSightPreviewRoomType",
            "UrdaRootSightMapPreviewIconPatch",
            "UrdaRootSightMapQuestIconPatch",
            "UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon",
            "UrdaRootSightMapPreviewVisuals.ApplyQuestIcon",
            "ApplyRootSightOverlay(pointNode, hasRootSightMarker || canTargetWithRootSight)",
            "UnknownIconPath(roomType)",
            "UnknownOutlinePath(roomType)",
            "NHoverTipSet.Remove(__instance)",
            "NHoverTipSet.CreateAndShow",
            "UrdaRootSightMapPointClickPatch",
            "HarmonyPatch(typeof(NMapPoint), \"OnRelease\")",
            "UrdaRootSightDisabledMapPointClickPatch",
            "HarmonyPatch(typeof(NClickableControl), nameof(NClickableControl._GuiInput))",
            "__instance is not NMapPoint mapPoint",
            "InputEventMouseButton { ButtonIndex: MouseButton.Left }",
            "__instance.GetViewport()?.SetInputAsHandled()",
            "UrdaBlessingService.TryCommitRootSightSelection");
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
        var seedBankSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelics.cs"));
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.DoesNotContain("UrdaTrialPlantCard", seedBankSource, StringComparison.Ordinal);
        AssertSourceContains(
            seedBankSource,
            "if (cards.Count == 0)",
            "SeedBankCardIds = string.Empty",
            "SeedBankSettled = true",
            "RefreshSeedBankRelicStatus(player)",
            "var addedCount = 0",
            "var failedSelectedIds = new List<string>()",
            "failedSelectedIds.Add(card.Id.ToString())",
            "SeedBankCardIds = string.Join(\",\", failedSelectedIds.Take(SeedBankMaxSeeds))",
            "finally",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "CreateStoredSeedsHoverTip",
            "storedSeeds.descriptionPrefix",
            "storedSeeds.descriptionFooter",
            "Seed Bank extraction preserved");
        Assert.DoesNotContain("HoverTipFactory.FromCard(card)", seedBankSource, StringComparison.Ordinal);
        Assert.DoesNotContain(".Concat(card.HoverTips)", seedBankSource, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "max [blue]3[/blue]",
            "the first chosen card is upgraded",
            "Click this relic later");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "最多[blue]3[/blue]张",
            "第一张会升级");

        foreach (var value in new[]
        {
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            engRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"],
            zhsRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"]
        })
        {
            Assert.DoesNotContain("Trial Plant", value, StringComparison.Ordinal);
            Assert.DoesNotContain("试炼种植", value, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VakuuFightTextAndSourceStayExplicitAboutRiskAndRewards()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var entry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var victory = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictory.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.DoesNotContain("TaskHelper.RunSafely", patch, StringComparison.Ordinal);
        Assert.Contains("EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)", entry, StringComparison.Ordinal);
        Assert.Contains("ClearEventNode(vakuu)", entry, StringComparison.Ordinal);
        Assert.Contains("EventNodeBackingField", entry, StringComparison.Ordinal);
        Assert.Contains("CreateVictoryFallbackOption", victory, StringComparison.Ordinal);
        Assert.Contains("VictoryFallbackDescriptionKey", victory, StringComparison.Ordinal);
        Assert.Contains("targetChoiceCount = encounter.VictoryChoiceCount", victory, StringComparison.Ordinal);
        Assert.Contains("encounter.VictoryGold", victory, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch + entry + victory, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", patch + entry, StringComparison.Ordinal);
        Assert.Contains("base(RoomType.Monster, autoAdd: false)", encounter, StringComparison.Ordinal);
        Assert.Contains("ShouldGiveRewards => false", encounter, StringComparison.Ordinal);
        Assert.Contains("CustomScenePath => VakuuFightAssetPaths.EncounterScene", encounter, StringComparison.Ordinal);
        Assert.Contains("Slots => [VakuuSlot]", encounter, StringComparison.Ordinal);
        Assert.Contains("ModelDb.Monster<EzmbVakuuTrialMonster>()", encounter, StringComparison.Ordinal);
        Assert.Contains("runState.Players.Count == 1", gate, StringComparison.Ordinal);
        Assert.Contains("ShouldEnableFight", gate, StringComparison.Ordinal);
        Assert.Contains("EZMB_ENABLE_VAKUU_FIGHT", gate, StringComparison.Ordinal);

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "Fight Vakuu",
            "greed trial",
            "No normal combat rewards",
            "[gold]Contracts[/gold]",
            "cash out",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "Death ends the run");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "与瓦库进行赃物试炼",
            "本场没有普通战斗奖励",
            "[gold]契约[/gold]",
            "收手",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "死亡会结束本局");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "Fight Vakuu",
            "greed trial",
            "[gold]Contracts[/gold]",
            "cash out",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "No normal combat rewards",
            "Death ends the run");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "与瓦库进行赃物试炼",
            "[gold]契约[/gold]",
            "收手",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "本场没有普通战斗奖励",
            "死亡会结束本局");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.title");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.description");
    }

    [Fact]
    public void TemptationDocsNoLongerDescribeImplementedGameplayAsUnimplemented()
    {
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var issue = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");

        Assert.Contains("Contract", sourceDesign, StringComparison.Ordinal);
        Assert.Contains("Contract", issue, StringComparison.Ordinal);
        Assert.DoesNotContain("Temptation remains not implemented", sourceDesign, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Temptation remains not implemented", issue, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("future content and is not implemented", sourceDesign, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("future content and was not implemented", issue, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UrdaV1SupportDesignDoesNotOverrideCurrentV33SeedbedAndAfterRain()
    {
        var urdaSourceDesign = ReadRepoText("docs", "features", "ancient-expansion-urda", "source-design.md");
        var urdaImplementationPlan = ReadRepoText("docs", "features", "ancient-expansion-urda", "implementation-plan.md");
        var urdaManualChecklist = ReadRepoText("docs", "features", "ancient-expansion-urda", "manual-test-checklist.md");
        var urdaWorkLog = ReadRepoText("docs", "features", "ancient-expansion-urda", "work-log.md");
        var v22SourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var currentUrdaDocs = string.Join(Environment.NewLine, urdaSourceDesign, urdaImplementationPlan, urdaManualChecklist, urdaWorkLog, v22SourceDesign);

        AssertSourceContains(
            urdaSourceDesign,
            "Status / authority note, 2026-05-25",
            "v3.3 Seedbed and After the Rain supersede",
            "Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight",
            "Withered Husk",
            "Rain Breath",
            "fewer than 3 Act 1 After the Rain triggers grants 75 Gold",
            "Three or more triggers heals 8 HP and upgrades 1 card");
        AssertSourceContains(
            docsIndex,
            "Urda ancient expansion support",
            "active goal/issues/v2.2/v3.3 docs override older behavior");
        AssertSourceContains(
            urdaImplementationPlan,
            "current v3.3 source behavior",
            "first unblocked enemy attack damage each combat adds one `Rain Breath`");
        AssertSourceContains(
            urdaManualChecklist,
            "adds one Seedbed card",
            "Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight",
            "first unblocked enemy attack damage each combat adds 1 Rain Breath",
            "3 or more Act 1 triggers heals 8 HP and upgrades 1 card");
        AssertSourceContains(
            v22SourceDesign,
            "Act 1 Rain Breath triggers and an Act 2 trigger-count payoff",
            "First unblocked enemy attack damage in each Act 1 combat adds Rain Breath");
        Assert.DoesNotContain("First lethal damage prevents death", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("first lethal damage prevents death", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("death-prevention hooks", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Act 1 death prevention", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Act 2 unused compensation", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("unused compensation", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("elite kills grant 20 Gold", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("ShouldDieLate", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Seedbed's Herald", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("adds one Seedling card", currentUrdaDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveAncientDocsKeepCurrentReadableNamesAndSeedBankScope()
    {
        var v22SourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var v22ManualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var urdaIssue = ReadRepoText("docs", "issues", "urda.md");
        var v33DesignReview = ReadRepoText("docs", "issues", "v3.3-design-review.md");
        var activeDocs = string.Join(Environment.NewLine, v22SourceDesign, v22ManualChecklist, urdaIssue, v33DesignReview);

        AssertSourceContains(
            v22SourceDesign,
            "播种、借阅与审判",
            "Forbidden Loan / 禁书借阅",
            "Misprint Press / 错页印刷机",
            "Overdue Library / 逾期书库",
            "Paperstorm / 纸屑风暴",
            "Blueprint Proofreading / 蓝图校样",
            "Mirror Rebuttal / 反证之镜",
            "Mirror Hall Echo / 镜厅回声",
            "Closed Court / 终审封庭",
            "Death Reprieve / 死刑缓期",
            "Public Evidence / 公开罪证");
        AssertSourceContains(
            urdaIssue,
            "Seed Bank deliberately uses the current test-slice path",
            "active source-safe behavior");
        AssertSourceContains(
            v33DesignReview,
            "current follow-up design uses `Temporary` and `Plant`",
            "future temporary negative cards, Blight Sprouts, and Rootblight are planted before entering hand",
            "Seedbed immediately plants one eligible card from draw/discard; upgraded Seedbed can plant up to two",
            "Rootblight and Blight Sprout are plantable",
            "Seedbed future-card planting",
            "Seedbed+ immediate draw/discard planting");
        AssertSourceContains(
            v22ManualChecklist,
            "Seedbed card text and hover explain Temporary and Plant",
            "Seedbed gives 8 Block, sets 2 slots, and immediately plants 1 eligible draw/discard card; Seedbed+ gives 12 Block, sets 3 slots, and can immediately plant up to 2",
            "Later Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight are planted before entering hand",
            "Permanent Curses, Withered Husk, and beneficial temporary pages are not planted",
            "Planted cards do not trigger play, discard, draw, or Exhaust synergies");

        foreach (var staleFragment in new[] { "does not yet store one unchosen", "auto-catching future cards", "Bury 1 Status, Curse, Blight Sprout, or Rootblight", "Seedbed text now says \"up to\"", "鎾", "銆", "绂", "閿", "钃", "鍙", "闀", "缁", "姝" })
        {
            Assert.DoesNotContain(staleFragment, activeDocs, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ArtDirectionDoesNotClaimTemporaryAssetsAreFinal()
    {
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");

        AssertSourceContains(
            artDirection,
            "Final browser GPTimage2 small art generated this pass",
            "Urda event background: Active event art is the original user-accepted 16:9 Urda middle-draft",
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
            engAscension["BOSS_DEDICATED_ABILITY.description"],
            "[gold]dedicated ability[/gold]");
        AssertSourceContains(
            engAscension["BOSS_BRANDED_FORM.description"],
            "[gold]Branded Form[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]2[/blue]",
            "+[blue]4[/blue]",
            "[gold]Artifact[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"],
            "After [gold]Ebb[/gold]",
            "[blue]2[/blue] Time Sand",
            "[gold]Wither[/gold]");
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
            zhsAscension["BOSS_DEDICATED_ABILITY.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_BRANDED_FORM.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]2[/blue]",
            "[blue]4[/blue]",
            "[gold]人工制品[/gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"],
            "[gold]消退[/gold]",
            "[blue]2[/blue]",
            "时砂",
            "[gold]枯萎[/gold]");
        Assert.Equal("Royal Decree", engAscension["BOSS_SEAL_CHOSEN_DECREE.title"]);
        Assert.Equal("御令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.title"]);
        Assert.Contains("[gold]御令[/gold]", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("王令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("择令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.title"], StringComparison.Ordinal);
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

    private static string[] DynamicVariableNames(string value) =>
        Regex.Matches(value, @"\{(?<name>[A-Za-z0-9_]+)(?::[^}]*)?\}")
            .Select(match => match.Groups["name"].Value)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static void AssertBalancedRichTextTags(string fileName, string key, string value)
    {
        foreach (var tag in new[] { "blue", "gold" })
        {
            var open = Regex.Matches(value, $@"\[{tag}\]").Count;
            var close = Regex.Matches(value, $@"\[/{tag}\]").Count;

            Assert.True(
                open == close,
                $"{fileName}:{key} has unbalanced [{tag}] rich-text tags.");
        }
    }

}
