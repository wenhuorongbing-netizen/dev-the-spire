using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
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
            "Current automated snapshot:",
            "SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1",
            "Historical detailed command logs are preserved",
            "Automated evidence closes only the smoke-level clicked Ancient UI rows above",
            "does not close live gameplay, gated Vakuu fight-option/victory return, save-load, death/failure, route traversal, preview-tools, or co-op rows.");
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Pre-commit local cleanup status summary", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Latest package note, 2026-05-18: the package hashes below include", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("V22 text/art-fit recheck", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Source-audited text correction recheck", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Source-guard follow-up, 2026-05-14", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("RC1 normal Steam-client isolated startup log started", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Previous controlled `--force-steam off` smoke evidence", handoff, StringComparison.Ordinal);
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
        var archivedRuntimeHistory = ReadRepoText(
            "docs",
            "archive",
            "implementation-records",
            "dev-environment-runtime-smoke-history-20260526.md");
        var longestLine = devEnvironment
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 750, $"docs/dev-environment.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            devEnvironment,
            "Historical 22-field loader evidence:",
            "Current source defines 30 SavedAttachedState fields",
            "Historical beta.19 loader evidence:",
            "dev-environment-runtime-smoke-history-20260526.md",
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Last private beta package:",
            "Zip SHA256:",
            "DLL SHA256:",
            "## Pending manual checks",
            "Manual game verification");
        AssertSourceContains(
            archivedRuntimeHistory,
            "Historical archive.",
            "direct `SlayTheSpire2.exe` launch",
            "ConnectToGlobalUser failed",
            "RootBudCombatHook",
            "rc1-normal-steam-clean-godot-20260508-090122",
            "live-spire-plus-disabled-session-20260513-142835",
            "current-package-smoke-20260514-015901");
        Assert.DoesNotContain("## Runtime smoke attempts", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("direct `SlayTheSpire2.exe` smoke launch", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("ConnectToGlobalUser failed", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("RC1 normal Steam-client launch/log probe", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("VampireSurvivors", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("## TODO", devEnvironment, StringComparison.Ordinal);
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
            "active manifest is `v0.1.0-private-beta.115` after the Sovereign Blade hover ModPatcher migration and package refresh pass",
            "Manual feature results are pending");
        AssertSourceContains(
            testPlan,
            "Current automated suite count and command results are recorded",
            "Historical Steam-client loader evidence",
            "manual feature matrix has runtime gameplay",
            "A20 multiplayer selection is not full A20 co-op support");
        AssertSourceContains(
            ancientCompletionAudit,
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Historical beta.19 package smoke",
            "gameplay/manual rows pending");
    }
}
