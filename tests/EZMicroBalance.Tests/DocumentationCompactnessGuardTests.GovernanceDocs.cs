using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
    [Fact]
    public void ProjectStateStaysCurrentAndHistoricalPassLogIsArchived()
    {
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var archive = ReadRepoText("docs", "archive", "project-state-history-20260516.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");

        Assert.True(
            projectState.Split('\n').Length <= 100,
            "PROJECT_STATE.md should remain a compact first-read current-state file; archive historical pass logs instead.");
        Assert.Contains("docs/archive/project-state-history-20260516.md", projectState, StringComparison.Ordinal);
        Assert.Contains("Archive note: this is the pre-cleanup `PROJECT_STATE.md` snapshot", archive, StringComparison.Ordinal);
        Assert.Contains("Historical M5 Revision N truth", projectState, StringComparison.Ordinal);
        Assert.Contains("runtime blocker was resolved for loader/patch application", projectState, StringComparison.Ordinal);
        Assert.Contains("2026-05-24 after the Sere Talon `NRelic` fallback package refresh", projectState, StringComparison.Ordinal);
        Assert.Contains("focused Sere Talon/release-evidence/documentation/website guards", projectState, StringComparison.Ordinal);
        Assert.Contains("beta.19 packages have historical loader/startup evidence", projectState, StringComparison.Ordinal);
        Assert.Contains("beta.19 Steam-client loader smoke", projectState, StringComparison.Ordinal);
        Assert.Contains("Current manual-test package is not a release-readiness claim", projectState, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", projectState, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.Ordinal);
        Assert.DoesNotContain("2026-05-18 package was not live-loader-smoked", projectState, StringComparison.Ordinal);
        Assert.DoesNotContain("235 passed / 18 skipped", projectState, StringComparison.Ordinal);

        foreach (var stalePassMarker in new[]
        {
            "Latest Vakuu/text/reward polish package refresh",
            "Latest event-background live UI correction",
            "Latest browser GPTimage2 oil-repaint art rebuild",
            "Latest Ancient art promotion",
            "Historical source/package refresh",
            "Current-package smoke/log/resource verification"
        })
        {
            Assert.DoesNotContain(stalePassMarker, projectState, StringComparison.Ordinal);
            Assert.Contains(stalePassMarker, archive, StringComparison.Ordinal);
        }

        Assert.Contains("archive/project-state-history-20260516.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("docs/archive/project-state-history-20260516.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/archive/project-state-history-20260516.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("project-state-history-20260516.md", archiveReadme, StringComparison.Ordinal);
    }

    [Fact]
    public void ToReviewQueueStaysCompactAndHistoricalRowsAreArchived()
    {
        var toReview = ReadRepoText("docs", "toreview.md");
        var archive = ReadRepoText("docs", "archive", "feature-audits", "toreview-pre-slim-20260518.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");

        Assert.True(
            toReview.Split('\n').Length <= 60,
            "docs/toreview.md should stay a compact current manual retest queue; archive historical implementation rows.");
        Assert.Contains("docs/archive/feature-audits/toreview-pre-slim-20260518.md", toReview, StringComparison.Ordinal);
        Assert.Contains("BANNER-TEMP-STRENGTH-CLEANUP-20260518", archive, StringComparison.Ordinal);
        Assert.DoesNotContain("BANNER-TEMP-STRENGTH-CLEANUP-20260518", toReview, StringComparison.Ordinal);
        Assert.Contains("URDA-ROOT-EYES", toReview, StringComparison.Ordinal);
        Assert.Contains("VAKUU-FIGHT", toReview, StringComparison.Ordinal);
        Assert.DoesNotContain("candidate gating", toReview, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("toreview-pre-slim-20260518.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", archiveReadme, StringComparison.Ordinal);
    }

    [Fact]
    public void IssuesQueueStaysCompactAndDoesNotBecomeAReleaseJournal()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var lineCount = issues.Split('\n').Length;
        var longestLine = issues
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(lineCount <= 45, $"docs/issues.md should stay a compact blocker queue; current line count is {lineCount}.");
        Assert.True(longestLine <= 260, $"docs/issues.md should avoid release-journal lines; longest line is {longestLine} characters.");
        AssertSourceContains(
            issues,
            "Current target: test-ready manual build, not release-ready.",
            "Current package hashes, 2026-06-18:",
            "| ZIP |",
            "| DLL |",
            "## Active blockers",
            "`SERE-TALON/TANX-CLAWS-ROUTING`",
            "`TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending",
            "`SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending",
            "`GOV-WIP-SPLIT` P0 source-fixed",
            "`DOC-CONFLICT-GOVERNANCE` P2 source-fixed",
            "`PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending",
            "read latest pushed migration HEAD from `git log -1 --oneline --decorate`",
            "recapture worktree status before final handoff",
            "## Manual Proof Gates");
        Assert.DoesNotContain("Latest verified package hashes after", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("source-split/refactor passes", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SERE-TALON/CLAWS-ROUTING", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("SERE-TALON-VISUAL-IDENTITYT P0 source-fixed / package/live-pending", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("latest pushed migration preflight/governance slice is clean at `f885d64d`", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("current worktree is clean after intentional batches", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("current local Runtime Proof + Governance Closure worktree is dirty", issues, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentGovernanceDocsDoNotCarryStaleCleanupState()
    {
        var docsByPath = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["PROJECT_STATE.md"] = ReadRepoText("PROJECT_STATE.md"),
            ["docs/issues.md"] = ReadRepoText("docs", "issues.md"),
            ["docs/worktree-cleanup-audit.md"] = ReadRepoText("docs", "worktree-cleanup-audit.md"),
            ["docs/private-beta-release-completion-audit.md"] = ReadRepoText("docs", "private-beta-release-completion-audit.md"),
            ["docs/features/ancients-rework-v4/completion-audit.md"] = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md")
        };

        AssertSourceContains(
            docsByPath["PROJECT_STATE.md"],
            "Active M5 Revision P truth",
            "current beta.91 RitsuLib-only loader proof supersedes it",
            "tester-package handoff decisions remain pending");
        AssertSourceContains(
            docsByPath["docs/worktree-cleanup-audit.md"],
            "Current beta.86 evidence should be read from the latest validated HEAD");
        AssertSourceContains(
            docsByPath["docs/worktree-cleanup-audit.md"],
            "current package evidence derived from the manifest/versioned artifacts");
        AssertSourceContains(
            docsByPath["docs/issues.md"],
            "`DOC-CONFLICT-GOVERNANCE` P2 source-fixed",
            "`PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending");
        AssertSourceContains(
            docsByPath["docs/private-beta-release-completion-audit.md"],
            "Earlier dirty implementation batches have been split, validated, committed, and pushed on `main`",
            "Final release handoff capture: current `git status --short --branch`, validated HEAD, and pushed branch after the last validation pass.");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "Beta.38 loader startup and gameplay/manual rows pending");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "Legacy root surfaces including `EzDailyContent.json` are absent from the active root",
            "OnlySpirePlusIsAnActiveRootModSurface");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "avoid copying stale historical commit labels into beta.86 handoff notes");

        foreach (var staleFragment in new[]
                 {
                     "beta.26 startup remains pending",
                     "beta.26 loader proof still needs a fresh run",
                     "current worktree is clean after intentional batches",
                     "319 dirty entries",
                     "47 dirty entries",
                     "9 dirty entries",
                     "10 dirty entries",
                     "14 dirty entries",
                     "21 dirty entries",
                     "45 dirty entries",
                     "80 dirty entries",
                     "current dirty worktree is intentionally batch-classified",
                     "remains dirty with many source/resource/docs/tests changes",
                     "remains dirty with many pending source/docs/test/resource changes",
                     "no commit or push has been performed",
                     "Clean intentional commit state and pushed branch after validation",
                     "Beta.35 loader startup",
                     "`EzDailyContent.json` still uses id `EzDailyContent`",
                     "efb3dc4",
                     "Refresh beta37 package evidence"
                 })
        {
            foreach (var (path, text) in docsByPath)
            {
                Assert.DoesNotContain(staleFragment, text, StringComparison.Ordinal);
            }
        }
    }
}
