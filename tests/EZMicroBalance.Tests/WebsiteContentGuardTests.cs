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
        Assert.Contains("8FD25AE6EFECCD76CFEDA13B99CAB355DF02824EDA595A4F8F1A0BBABDFC5D0E", websiteData, StringComparison.Ordinal);
        Assert.DoesNotContain("2D86E610141E5FD7500ABDC8973F924E21442EBFBC7F2025B60F982F0D712605", websiteData, StringComparison.Ordinal);

        var packagePath = RepoPath("publish", "SpirePlus-v0.1.0-private-beta.5.zip");
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

        AssertSourceContains(
            websiteData,
            "EZMB_URDA_RAIN_BREATH",
            "EZMB_VAKUU_TRICK_CONTRACT",
            "EZMB_VAKUU_CASH_OUT_CONTRACT",
            "\u4e00\u4e2a\u66f4\u597d\u7684\u300a\u6740\u622e\u5c16\u5854 2\u300b\u62d3\u5c55",
            "\u65b0\u589e\u8fdb\u9636 A20 \u548c\u5148\u53e4\u4e4b\u6c11\u4fee\u6539",
            "short: \"\\u5361\\u724c\"",
            "A better Slay the Spire 2 expansion",
            "New A20 Ascension and Ancient changes",
            "\u82e5\u5408\u9002\u623f\u95f4\u4e0d\u8db3\uff0c\u81f3\u5c11\u653e\u51652\u4e2a",
            "\u53ef\u9644\u9b54\u724c",
            "If there are not enough suitable rooms, at least 2 are placed.",
            "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission.",
            "Temporary Status cards, Temporary Curse cards, or Blight Sprouts",
            "On pickup, choose 1 of 4 Curses. Add it, 2 Wish, and 1 Wish+.",
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "Transforms up to 6 cards into upgraded Maul.",
            "\"SERE_TALON.description\": \"sere_talon.png\"",
            "\"CLAWS.description\": \"claws.png\"",
            "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor",
            "Deal 12/24/48 damage in the round to interrupt the heal",
            "8FD25AE6EFECCD76CFEDA13B99CAB355DF02824EDA595A4F8F1A0BBABDFC5D0E");

        AssertSourceContains(
            index,
            "content-data.js?v=20260525-known-issues",
            "app.js?v=20260525-known-issues");

        AssertSourceContains(
            ReadRepoText("website", "app.js"),
            "requestUrl.searchParams.set(\"v\", appVersion);",
            "fetch(requestUrl, { cache: \"no-cache\" })");

        Assert.True(
            File.Exists(RepoPath("website", "assets", "source-art", "relics", "sere_talon.png")),
            "Sere Talon must use its original source icon so Vakuu's reward is not shown with Tanx Claws art or a placeholder.");

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
                      "18,904,206",
                       "18,879,316",
                      "18,874,569"
                  })
        {
            Assert.DoesNotContain(staleText, websiteData, StringComparison.Ordinal);
        }
    }
}
