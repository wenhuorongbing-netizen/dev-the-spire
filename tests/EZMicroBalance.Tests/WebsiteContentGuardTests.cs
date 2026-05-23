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
        Assert.Contains("1D294871C211B48EAE9DA246BC94E8BF5422985A3FC589D62048BAF32469BB26", websiteData, StringComparison.Ordinal);
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
            "Spire Plus 不是单纯的加强包",
            "避免把游戏直接变简单",
            "A20 的挑战难度",
            "short: \"\\u5361\\u724c\"",
            "Spire Plus is not a pure power-up pack",
            "does not simply become easier",
            "A20 as the top-end test",
            "Temporary Status cards, Temporary Curse cards, or Blight Sprouts",
            "On pickup, choose up to 6 cards and transform them into Maul.",
            "No longer transforms deck cards. Choose 1 of 4 Curses",
            "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor",
            "Deal 12/24/48 damage in the round to interrupt the heal",
            "1D294871C211B48EAE9DA246BC94E8BF5422985A3FC589D62048BAF32469BB26");

        AssertSourceContains(
            index,
            "content-data.js?v=20260523-wording-sync",
            "app.js?v=20260523-wording-sync");

        AssertSourceContains(
            ReadRepoText("website", "app.js"),
            "requestUrl.searchParams.set(\"v\", appVersion);",
            "fetch(requestUrl, { cache: \"no-cache\" })");

        foreach (var staleText in new[]
                 {
                     "Deal 22 damage to Vakuu",
                     "Gain 24 Block and lose 3 HP",
                     "Dealing 20/40/80 damage",
                     "gain Molten Armor after each enemy turn",
                     "Dedicated token cards",
                     "short: \"Tokens\"",
                     "清单编号 EZMicroBalance",
                     "manifest id EZMicroBalance",
                     "18,874,569"
                 })
        {
            Assert.DoesNotContain(staleText, websiteData, StringComparison.Ordinal);
        }
    }
}
