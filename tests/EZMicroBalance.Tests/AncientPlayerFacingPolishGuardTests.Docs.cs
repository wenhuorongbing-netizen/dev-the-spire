using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
{
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
}
