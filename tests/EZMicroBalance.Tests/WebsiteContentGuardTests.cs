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
        Assert.Contains("209226DF15AB8B53A5E1FC9C9BBC965E05D7B53104ABEF84EBBBBAC16641240F", websiteData, StringComparison.Ordinal);
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
            "临时状态牌、临时诅咒牌或根芽",
            "EZMB_URDA_RAIN_BREATH",
            "EZMB_VAKUU_TRICK_CONTRACT",
            "EZMB_VAKUU_CASH_OUT_CONTRACT",
            "本轮造成12/24/48点伤害可中断治疗",
            "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor",
            "Deal 12/24/48 damage in the round to interrupt the heal",
            "209226DF15AB8B53A5E1FC9C9BBC965E05D7B53104ABEF84EBBBBAC16641240F");

        AssertSourceContains(
            index,
            "content-data.js?v=20260523-content-sync",
            "app.js?v=20260523-content-sync");

        AssertSourceContains(
            ReadRepoText("website", "app.js"),
            "requestUrl.searchParams.set(\"v\", appVersion);",
            "fetch(requestUrl, { cache: \"no-cache\" })");

        foreach (var staleText in new[]
                 {
                     "根芽、根蚀、状态牌或诅咒牌会先种入苗床",
                     "对瓦库造成22点伤害",
                     "获得24点格挡，失去3点生命",
                     "每个敌方回合后获得熔甲",
                     "Dealing 20/40/80 damage",
                     "gain Molten Armor after each enemy turn",
                     "18,874,569"
                 })
        {
            Assert.DoesNotContain(staleText, websiteData, StringComparison.Ordinal);
        }
    }
}
