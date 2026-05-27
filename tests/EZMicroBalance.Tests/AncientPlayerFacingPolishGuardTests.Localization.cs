using System.Text;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
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
}
