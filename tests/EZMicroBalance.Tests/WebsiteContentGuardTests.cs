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
        Assert.Contains("47AE3A9F110284D2BEF03B84ED190208459E3BA55547BF7A656AFA08F61735CC", websiteData, StringComparison.Ordinal);
        Assert.DoesNotContain("2D86E610141E5FD7500ABDC8973F924E21442EBFBC7F2025B60F982F0D712605", websiteData, StringComparison.Ordinal);

        var packagePath = RepoPath("publish", "SpirePlus-v0.1.0-private-beta.0.zip");
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
            "\u4e0d\u9760\u65e0\u8111\u52a0\u5f3a\u628a\u6e38\u620f\u53d8\u7b80\u5355",
            "\u8fdb\u9636\u4e8c\u5341\u4ecd\u7136\u6709\u538b\u529b",
            "short: \"\\u5361\\u724c\"",
            "without turning the game into a pile of free power",
            "A20 that still pushes back",
            "\u82e5\u5408\u9002\u623f\u95f4\u4e0d\u8db3\uff0c\u81f3\u5c11\u653e\u51652\u4e2a",
            "\u53ef\u9644\u9b54\u724c",
            "If there are not enough suitable rooms, at least 2 are placed.",
            "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission.",
            "Temporary Status cards, Temporary Curse cards, or Blight Sprouts",
            "On pickup, add 2 random Curses and 3 Wish.",
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "Transforms up to 6 cards into upgraded Maul.",
            "\"SERE_TALON.description\": \"assets/relics/sere_talon.svg\"",
            "\"CLAWS.description\": \"claws.png\"",
            "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor",
            "Deal 12/24/48 damage in the round to interrupt the heal",
            "47AE3A9F110284D2BEF03B84ED190208459E3BA55547BF7A656AFA08F61735CC");

        AssertSourceContains(
            index,
            "content-data.js?v=20260524-release-links",
            "app.js?v=20260524-release-links");

        AssertSourceContains(
            ReadRepoText("website", "app.js"),
            "requestUrl.searchParams.set(\"v\", appVersion);",
            "fetch(requestUrl, { cache: \"no-cache\" })");

        Assert.True(
            File.Exists(RepoPath("website", "assets", "relics", "sere_talon.svg")),
            "Sere Talon must use its own website icon so Vakuu's reward is not shown with Tanx Claws art.");

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
                     "Sere Talon\", \"CLAWS.description\"",
                      "Vakuu's Sere Talon\", \"CLAWS.description\"",
                      "闂佸憡顭囬崰搴ㄥ垂閸偆鈻曢悗锝庡墮閻掔睜\", \"CLAWS.description\"",
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
