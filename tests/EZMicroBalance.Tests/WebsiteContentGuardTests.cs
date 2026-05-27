using System.Globalization;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class WebsiteContentGuardTests
{
    [Fact]
    public void WebsiteLocalizationSubsetMatchesCurrentModLocalization()
    {
        var files = new[] { "ancients.json", "ascension.json", "cards.json", "powers.json", "relics.json" };
        var languages = new[] { "eng", "zhs" };

        foreach (var language in languages)
        {
            foreach (var file in files)
            {
                Assert.Equal(
                    NormalizeJson(RepoPath("EZMicroBalance", "localization", language, file)),
                    NormalizeJson(RepoPath("website", "assets", "localization", language, file)));
            }
        }
    }

    [Fact]
    public void WebsitePackageMetadataMatchesCurrentPackageHash()
    {
        var websiteData = ReadRepoText("website", "content-data.js");
        var packagePath = CurrentPackageZipPath();
        Assert.Contains(Sha256(packagePath), websiteData, StringComparison.Ordinal);
        Assert.DoesNotContain("2D86E610141E5FD7500ABDC8973F924E21442EBFBC7F2025B60F982F0D712605", websiteData, StringComparison.Ordinal);

        if (File.Exists(packagePath))
        {
            var package = new FileInfo(packagePath);
            Assert.Contains(Sha256(packagePath), websiteData, StringComparison.Ordinal);
            var invariantLength = package.Length.ToString("N0", CultureInfo.InvariantCulture);
            Assert.Contains($"{invariantLength} bytes", websiteData, StringComparison.Ordinal);
            Assert.Contains($"{invariantLength} \\u5b57\\u8282", websiteData, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void WebsiteHardcodedGameplaySummariesStayCurrent()
    {
        var websiteData = ReadRepoText("website", "content-data.js");
        var index = ReadRepoText("website", "index.html");
        var styles = ReadRepoText("website", "styles.css");
        var packageHash = CurrentPackageZipSha256();

        AssertSourceContains(
            websiteData,
            "EZMB_URDA_RAIN_BREATH",
            "urda_elite_root",
            "EZMB_VAKUU_TRICK_CONTRACT",
            "EZMB_VAKUU_CASH_OUT_CONTRACT",
            "\u5148\u53e4\u5956\u52b1\u3001\u539f\u7248\u9057\u7269\u3001\u9ad8\u8fdb\u9636\u8def\u7ebf\u548c\u9884\u89c8\u5de5\u5177\u90fd\u653e\u5728\u540c\u4e00\u4e2a Mod \u91cc",
            "Spire Plus \u505a\u4e86\u4ec0\u4e48",
            "short: \"\\u5361\\u724c\"",
            "trigger, payoff, cost, and after-combat result",
            "What Spire Plus Changes",
            "\u82e5\u5408\u9002\u623f\u95f4\u4e0d\u8db3\uff0c\u81f3\u5c11\u653e\u51652\u4e2a",
            "\u53ef\u9644\u9b54\u724c",
            "If there are not enough suitable rooms, at least 2 are placed.",
            "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission.",
            "Can plant Temporary Status/Curse cards, Blight Sprouts, and Rootblight",
            "Threefold Payoff: Gain 8/12 Block",
            "Capacity: Has 2/3 slots",
            "用于私测的单体玩法扩展",
            "苗床为什么值得拿",
            "自动拦截污染牌，避免污染手牌",
            "种下根蚀会冻结其状态",
            "自动拦截污染牌",
            "种下根蚀会冻结其状态",
            "不属于打出、弃牌或消耗",
            "种下根芽视为已处理",
            "不净化",
            "Planting removes cards from combat and gives a Withered Husk",
            "It is not considered playing, discarding, or exhausting",
            "Can plant Temporary Status/Curse cards, Blight Sprouts, and Rootblight",
            "Planting a Rootblight freezes its state",
            "On pickup, choose 1 of 4 Curses. Add it, 2 Wish, and 1 Wish+.",
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "拾取时选择至多6张牌，将它们变化为“撕咬”。",
            "选择至多6张牌，将它们变化为“撕咬+”。",
            "Transforms up to 6 cards into upgraded Maul.",
            "\"SERE_TALON.description\": \"sere_talon.png\"",
            "\"CLAWS.description\": \"claws.png\"",
            "assets/source-art/events/crystal_sphere/small_divination_icon.png",
            "assets/powers/morvi_archive_page.png",
            "assets/powers/morvi_open_book.png",
            "assets/powers/lotha_single_sentence.png",
            "assets/card_portraits/card.png",
            "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor",
            "Deal [blue]{InterruptDamage}[/blue] damage to it in one round to interrupt the heal",
            "soul_fysh_soul_tide",
            "BOSS_SEAL_SOUL_TIDE.summary",
            "Soul Fysh",
            "Soul Tide",
            packageHash);

        AssertSourceContains(
            index,
            "content-data.js?v=20260527-fix-sere-talon",
            "app.js?v=20260527-fix-sere-talon");

        var appJs = ReadRepoText("website", "app.js");

        AssertSourceContains(
            appJs,
            "requestUrl.searchParams.set(\"v\", appVersion);",
            "fetch(requestUrl, { cache: \"no-cache\" })");

        AssertSourceContains(
            websiteData,
            "...mechanicGlossary.map(mechanicCodexItem)",
            "function mechanicCodexItem(mech)",
            "isMechanicsCodex: true");
        Assert.DoesNotContain("// Test Guard Check", websiteData, StringComparison.Ordinal);

        AssertSourceContains(
            appJs,
            "function findRelatedMechanics(item)",
            "itemMechanic?.relatedMechanicIds?.includes(mech.id)",
            "if (item.namespace === \"mechanics\" && mech.id === itemKeyStr) continue;",
            "Related Mechanics",
            "\u673a\u5236\u89e3\u91ca");

        AssertSourceContains(
            appJs,
            "qol-feature-banner",
            "qol-banner-content",
            "qol-banner-image-container");
        AssertSourceContains(
            styles,
            ".qol-feature-banner",
            ".qol-banner-content",
            ".qol-banner-image-container");

        Assert.True(
            File.Exists(RepoPath("website", "assets", "source-art", "relics", "sere_talon.png")),
            "Sere Talon must use its original source icon so Vakuu's reward is not shown with Tanx Claws art or a placeholder.");

        foreach (var websiteImage in new[]
                 {
                     RepoPath("website", "assets", "source-art", "events", "crystal_sphere", "small_divination_icon.png"),
                     RepoPath("website", "assets", "source-art", "events", "crystal_sphere", "big_divination_icon.png"),
                     RepoPath("website", "assets", "powers", "morvi_archive_page.png"),
                     RepoPath("website", "assets", "powers", "morvi_open_book.png"),
                     RepoPath("website", "assets", "powers", "lotha_single_sentence.png"),
                     RepoPath("website", "assets", "powers", "power.png"),
                     RepoPath("website", "assets", "card_portraits", "card.png")
                 })
        {
            Assert.True(File.Exists(websiteImage), $"{websiteImage} must exist so website mechanism icons do not fall back to the generic relic placeholder.");
        }

        foreach (var rootImage in new[]
                 {
                     "rootblight_i.png",
                     "rootblight_ii.png",
                     "rootblight_iii.png",
                     "blight_sprout.png"
                 })
        {
            Assert.True(
                File.Exists(RepoPath("website", "assets", "card_portraits", rootImage)),
                $"{rootImage} must use the current packaged Rootblight/Blight Sprout portrait.");
        }

        foreach (var staleText in new[]
                 {
                     "Deal 22 damage to Vakuu",
                     "Gain 24 Block and lose 3 HP",
                     "Dealing 20/40/80 damage",
                     "gain Molten Armor after each enemy turn",
                     "Dedicated token cards",
                     "short: \"Tokens\"",
                      "manifest id EZMicroBalance",
                      "mods\\\\EZMicroBalance\\\\EZMicroBalance.json",
                      "\u5019\u9009",
                      "safe fallback",
                      "detail(\"\u5019\u9009",
                       "\"SERE_TALON.description\": \"claws.png\"",
                       "\"SERE_TALON.description\": \"assets/source-art/relics/claws.png\"",
                       "\"SERE_TALON.description\": \"assets/relics/sere_talon.svg\"",
                     "Sere Talon\", \"CLAWS.description\"",
                      "Vakuu's Sere Talon\", \"CLAWS.description\"",
                       "\u95c2\u5099\u7901\u93b2",
                      "Future Peek",
                      "separate Future Peek package",
                       "18,879,316",
                      "18,874,569"
                  })
        {
            Assert.DoesNotContain(staleText, websiteData, StringComparison.Ordinal);
        }

        foreach (var staleManualMechanicEntry in new[]
                 {
                     "manual(\"\u8840\u503a\u4e0e\u8d43\u7269\u9501\"",
                     "manual(\"\u5a01\u4eea\u4e0e\u94f8\u4ee4\"",
                     "manual(\"\u6d1b\u838e\u4e4b\u88c1\u51b3\"",
                     "manual(\"\u88c2\u53d8\u9644\u9b54\"",
                     "manual(\"\u82d7\u5e8a\u4e0e\u6839\u82bd\"",
                     "manual(\"\u706b\u5370\u7cbe\u82f1\"",
                     "manual(\"\u6218\u65d7\u623f\"",
                     "manual(\"\u6df1\u5c42\u652f\u7ebf\""
                 })
        {
            Assert.DoesNotContain(staleManualMechanicEntry, websiteData, StringComparison.Ordinal);
        }
    }
}
