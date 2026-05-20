using Xunit;

namespace EZMicroBalance.Tests;

public sealed class DocumentationCompactnessGuardTests
{
    [Fact]
    public void TestReadyDevelopmentGoalStaysCompactAndCurrent()
    {
        var goal = ReadRepoText("docs", "test-ready-development-goal.md");
        var lineCount = goal.Split('\n').Length;

        Assert.True(lineCount <= 120, $"docs/test-ready-development-goal.md should stay compact; current line count is {lineCount}.");
        AssertSourceContains(
            goal,
            "Goal: keep the current `Spire Plus` workspace at a user-test-ready manual test build",
            "Current stop line: Codex should not chase release-ready evidence in this pass.",
            "`source code/src/Core/**` is the primary source evidence",
            "Future Peek is a separate mod idea",
            "No live-game, save-load, death/failure, or co-op evidence may be claimed from these commands.");
        Assert.DoesNotContain("One-Shot Prompt", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## Subagent Plan", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P0:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P1:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P2:", goal, StringComparison.OrdinalIgnoreCase);
    }

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
        Assert.Contains("Latest normal Steam-client startup/log evidence is historical for the pre-review Spire Plus package", projectState, StringComparison.Ordinal);
        Assert.Contains("Current manual-test package is not a release-readiness claim", projectState, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", projectState, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.Ordinal);

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
            "Current package hashes, 2026-05-20:",
            "| ZIP |",
            "| DLL |",
            "## Active blockers",
            "## Manual Proof Gates");
        Assert.DoesNotContain("Latest verified package hashes after", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("source-split/refactor passes", issues, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PrivateBetaHandoffAvoidsPinnedDirtyStatusSnapshots()
    {
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var longestLine = handoff
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 800, $"docs/private-beta-verification-handoff.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            handoff,
            "Current source/package highlights:",
            "This handoff is not a commit manifest",
            "Do not trust a point-in-time dirty-file list",
            "git status --short --branch",
            "git log -1 --oneline --decorate",
            "git diff --stat");
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Pre-commit local cleanup status summary", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Latest package note, 2026-05-18: the package hashes below include", handoff, StringComparison.Ordinal);
    }

    [Fact]
    public void PrivateBetaReleaseAuditAvoidsPassHistoryDumpRows()
    {
        var audit = ReadRepoText("docs", "private-beta-release-completion-audit.md");
        var longestLine = audit
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 1100, $"docs/private-beta-release-completion-audit.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            audit,
            "Latest source/package refresh",
            "detailed pass history lives in `docs/review.md` and `docs/archive/**`",
            "gameplay verification pending",
            "## Missing Or Weakly Verified Items");
        Assert.DoesNotContain("includes this mod/resource state plus the browser GPTimage2 small-art rebuild", audit, StringComparison.Ordinal);
        Assert.DoesNotContain("and release-evidence verifier hardening.", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void DevEnvironmentAvoidsReleaseJournalLines()
    {
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");
        var longestLine = devEnvironment
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 850, $"docs/dev-environment.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            devEnvironment,
            "Historical 22-field loader evidence:",
            "Current source defines 25 SavedSpireFields",
            "not refreshed 25-field package parity",
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Last private beta package:",
            "Zip SHA256:",
            "DLL SHA256:",
            "Manual game verification");
        Assert.DoesNotContain("strict hook/text audit", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Banner temporary Strength cleanup", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Ascension side-turn state fix", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Root Eyes stale-preview cleanup", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("duplicate-reservation avoidance", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Soul Tide Beckon pre-flush fix", devEnvironment, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentReleaseDocsAvoidReleaseJournalLines()
    {
        var docsToCheck = new[]
        {
            "docs/release-checklist.md",
            "docs/test-plan.md",
            "docs/test-ready-completion-audit.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/features/ancients-rework-v4/completion-audit.md"
        };
        var knownPassHistoryFragments = new[]
        {
            "strict hook/text audit",
            "Banner temporary Strength cleanup",
            "Ascension side-turn state fix",
            "Root Eyes stale-preview cleanup",
            "duplicate-reservation avoidance",
            "Soul Tide Beckon pre-flush fix"
        };
        var failures = new List<string>();

        foreach (var path in docsToCheck)
        {
            var text = ReadRepoText(path.Split('/'));
            var longestLine = text
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Max(line => line.Length);

            if (longestLine > 850)
            {
                failures.Add($"{path} has a release-journal line of {longestLine} characters.");
            }

            foreach (var fragment in knownPassHistoryFragments)
            {
                if (text.Contains(fragment, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale pass-history fragment `{fragment}`.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));

        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var ancientCompletionAudit = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md");

        AssertSourceContains(
            releaseChecklist,
            "Current package hashes:",
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "fresh live loader parity remains pending",
            "Manual feature results are pending");
        AssertSourceContains(
            testPlan,
            "Current automated suite count and command results are recorded",
            "current source defines 25 SavedSpireFields",
            "manual feature matrix has runtime gameplay",
            "A20 multiplayer selection is not full A20 co-op support");
        AssertSourceContains(
            ancientCompletionAudit,
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "current source defines 25 SavedSpireFields",
            "historical loader/resource evidence only");
    }
}
