using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class BossDedicatedAbilityV41GuardTests
{
    [Fact]
    public void WebsiteA19A20ContentUsesV41DedicatedAbilityTerminology()
    {
        var websiteData = ReadRepoText("website", "content-data.js");
        var websiteEnglishAscension = ReadRepoText("website", "assets", "localization", "eng", "ascension.json");
        var websiteChineseAscension = ReadRepoText("website", "assets", "localization", "zhs", "ascension.json");
        var websiteReadme = ReadRepoText("website", "README.md");
        var combinedWebsiteText = string.Join(
            Environment.NewLine,
            websiteData,
            websiteEnglishAscension,
            websiteChineseAscension,
            websiteReadme);

        AssertSourceContains(
            websiteData,
            "Each Boss gets its own dedicated ability",
            "A20 Branded Form behavior remains development-test scope",
            "Dedicated Ability",
            "Branded Form",
            "Vanilla bosses do not have A19 dedicated abilities or A20 Branded Form.",
            "bossSeal(\"aeonglass_hourglass\"",
            "Time Sand Reflow",
            "Plating Wake",
            "Escape Fatigue");

        AssertSourceContains(
            websiteChineseAscension,
            "\u6240\u6709\u9996\u9886\u83b7\u5f97\u4e13\u5c5e\u7279\u6b8a\u80fd\u529b",
            "\u70d9\u5370\u5f62\u6001",
            "\u65f6\u7802\u56de\u6d41",
            "\u591a\u91cd\u62a4\u7532",
            "\u5b9e\u9a8c\u8bb0\u5f55",
            "\u529b\u91cf\u6b8b\u7559",
            "\u6d3b\u529b");

        AssertSourceContains(
            websiteEnglishAscension,
            "Boss Dedicated Abilities",
            "Only the second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]",
            "Time Sand Reflow",
            "Vigor",
            "Plating");

        AssertSourceContains(
            websiteReadme,
            "public-info site",
            "not a release-ready claim",
            "Do not copy original non-art source materials");

        var activeDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "review.md"),
            ReadRepoText("docs", "toreview.md"));
        Assert.DoesNotContain("Royal Seal", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u5370", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u70d9\u5370", activeDocs, StringComparison.Ordinal);
        AssertRepoPathDoesNotExist("website", "localization_qa.md");
        AssertRepoFileExists("docs", "archive", "implementation-records", "website-localization-qa-20260522.md");

        foreach (var staleTerm in new[]
                 {
                     "Royal Seal",
                     "King Brand",
                     "\u738b\u5370",
                     "\u738b\u70d9\u5370",
                     "\u997f\u566c",
                     "\u7532\u58f3",
                     "\u68a6\u58f3",
                     "\u62a4\u58f3",
                     "Every 5 cards played adds 1 Wither",
                     "Returns 2 Slippery",
                     "8/12/16",
                     "12/16/20"
                 })
        {
            Assert.DoesNotContain(staleTerm, combinedWebsiteText, StringComparison.Ordinal);
        }
    }
}
