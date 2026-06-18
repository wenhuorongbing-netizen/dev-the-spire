using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
    [Fact]
    public void SourceFixedLivePendingIssuesHaveManualRetestRows()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var toReview = ReadRepoText("docs", "toreview.md");

        var mappings = new (string IssueId, string[] RetestEvidence)[]
        {
            ("SERE-TALON/TANX-CLAWS-ROUTING", ["MANUAL-20260519-COUNTERS-PEEK-TAGS", "MANUAL-20260524-SERE-TALON-ART"]),
            ("HUSK-CARD-BEHAVIOR", ["MANUAL-20260519-MOLTING"]),
            ("SERE-TALON-VISUAL-IDENTITY", ["MANUAL-20260524-SERE-TALON-ART", "MANUAL-20260524-SERE-TALON-TANX-CLAWS-REPORT"]),
            ("ROOT-SIGHT-ENCOUNTER-POOL", ["URDA-ROOT-EYES"]),
            ("FIREMARK-HEAL/TEXT", ["MANUAL-20260519-BANNER-FIREMARK"]),
            ("UNKNOWN-EVENT-PREVIEW-READABILITY", ["URDA-ROOT-EYES"]),
            ("ROOTBLIGHT-STARTER-MISSING", ["ASCENSION-A11-A20"]),
            ("WATERFALL-BOSS-SEAL", ["MANUAL-20260522-BOSS-SEALS"]),
            ("HOURGLASS-BOSS-SEAL-DESIGN", ["MANUAL-20260522-BOSS-SEALS", "MANUAL-20260522-SEAL-INDICATORS"]),
            ("QUEEN-BOSS-SEAL-WEAKNESS", ["MANUAL-20260522-BOSS-SEALS"]),
            ("FIREMARK-OVERFLOW/FORGE-ARMOR", ["MANUAL-20260519-BANNER-FIREMARK"]),
            ("BANNER-ROOM-PREVIEW", ["MANUAL-20260519-ACT-VALUES"]),
            ("ROOT-EYES-CONFLICTS-COOP", ["MANUAL-20260522-ROOT-EYES-CONFLICTS"]),
            ("PREVIEW-TOOLS-REWARD-HOOKS", ["MANUAL-20260522-PREVIEW-TOOLS"]),
            ("SEAL-BANNER-VISIBILITY", ["MANUAL-20260522-SEAL-INDICATORS"]),
            ("V33-DESIGN-PASS", ["MANUAL-20260522-V33-DESIGN"]),
            ("STRICT-AUDIT-VAKUU-CULTURE-SAVE", ["VAKUU-FIGHT"]),
            ("STRICT-AUDIT-PATCH-SURFACE", ["VAKUU-FIGHT", "ASCENSION-A11-A20", "MANUAL-20260520-EVIDENCE-LOG"]),
            ("STRICT-AUDIT-EVIDENCE-LOG", ["MANUAL-20260520-EVIDENCE-LOG"])
        };

        var failures = new List<string>();
        foreach (var mapping in mappings)
        {
            if (!issues.Contains($"`{mapping.IssueId}`", StringComparison.Ordinal))
            {
                failures.Add($"docs/issues.md no longer lists `{mapping.IssueId}`.");
                continue;
            }

            foreach (var retestEvidence in mapping.RetestEvidence)
            {
                if (!toReview.Contains(retestEvidence, StringComparison.Ordinal))
                {
                    failures.Add($"`{mapping.IssueId}` lacks manual retest evidence `{retestEvidence}` in docs/toreview.md.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void CurrentIssueDetailDocsAvoidReadableTypoArtifacts()
    {
        var issueDetails = ReadCurrentFacingDocs(
            "docs/issues/ancient-expansion-v2.2.md",
            "docs/issues/urda.md");

        AssertSourceContains(
            issueDetails,
            "SPIREPLUS_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_LOTHA",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "ISSUE-2026-05-13-LOTHA-FULL-TEST-IMPLEMENTATION",
            "ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES",
            "SavedSpireField<Player,string>",
            "UrdaDeckStateKey");
        var cardPowerSafetyIssue = SliceBetween(
            issueDetails,
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES",
            "## ISSUE-2026-05-12-MORVI-V22-PLANNING");
        AssertSourceContains(
            cardPowerSafetyIssue,
            "Status: source-fixed / live-pending",
            "Power-card replacement rewards",
            "Runtime closure still requires live gameplay");
        Assert.DoesNotContain("Status: open", cardPowerSafetyIssue, StringComparison.Ordinal);

        foreach (var typoArtifact in new[]
        {
            "SPIREPiUS",
            "DISABiE",
            "ENABiE",
            "BiESSING",
            "iOTHA",
            "FUii",
            "IMPiEMENTATION",
            "PiANNING",
            "AiIGNMENT",
            "MIiESTONE",
            "TECHNICAi",
            "iive",
            "iocal",
            "iegacy",
            "iinkedRewardSet",
            "SHEiTER",
            "Onokipped",
            "oavedopireField",
            "UrdaDeckotateKey",
            "otatus",
            "oource gameplay"
        })
        {
            Assert.DoesNotContain(typoArtifact, issueDetails, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveQueuesKeepSereTalonAndTanxClawsChineseReadable()
    {
        var activeQueues = ReadCurrentFacingDocs(
            "docs/issues.md",
            "docs/toreview.md",
            "docs/review.md");

        AssertSourceContains(
            activeQueues,
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "Maul+",
            "\u6495\u54ac+");

        foreach (var staleTanxClawsTuning in new[]
        {
            "Numeric Maul tuning is pending",
            "Maul tuning is pending",
            "design-pending",
            "[blue]1[/blue] more damage"
        })
        {
            Assert.DoesNotContain(staleTanxClawsTuning, activeQueues, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void CurrentAncientManualDocsKeepTanxClawsUpgradedMaulDesignCurrent()
    {
        var manualDocs = ReadCurrentFacingDocs(
            "docs/features/ancients-rework-v4/manual-test-checklist.md",
            "docs/features/ancients-rework-v4/manual-verification-matrix.md",
            "docs/features/ancients-rework-v4/source-design.md",
            "docs/features/ancients-rework-v4/implementation-plan.md");

        AssertSourceContains(
            manualDocs,
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "upgraded Maul",
            "Maul+",
            "\u6495\u54ac+");

        foreach (var staleTanxClawsTuning in new[]
        {
            "Numeric Maul tuning is pending",
            "Maul tuning is pending",
            "design-pending",
            "[blue]1[/blue] more damage"
        })
        {
            Assert.DoesNotContain(staleTanxClawsTuning, manualDocs, StringComparison.Ordinal);
        }
    }
}
