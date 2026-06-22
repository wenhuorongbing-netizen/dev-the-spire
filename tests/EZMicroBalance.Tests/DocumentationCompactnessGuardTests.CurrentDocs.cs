using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
    [Fact]
    public void CurrentDocsAvoidStaleNormalValidationCounts()
    {
        var docsToCheck = new[]
        {
            "README.md",
            "docs/BETA_COMPATIBILITY.md",
            "docs/dev-environment.md",
            "docs/toreview.md",
            "docs/test-ready-completion-audit.md",
            "docs/private-beta-verification-handoff.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/features/ancients-rework-v4/completion-audit.md",
            "docs/review.md"
        };
        var staleCounts = new[]
        {
            "202 passed / 18 skipped",
            "220 passed / 0 skipped",
            "235 passed / 18 skipped",
            "235 passed, 18 skipped",
            "244 passed / 18 skipped",
            "245 passed / 18 skipped",
            "246 passed / 18 skipped",
            "247 passed / 18 skipped",
            "248 passed / 18 skipped",
            "249 passed / 18 skipped",
            "250 passed / 18 skipped",
            "251 passed / 18 skipped",
            "252 passed / 18 skipped",
            "252 passed, 18 skipped",
            "253 passed / 18 skipped",
            "253 passed, 18 skipped",
            "253 passed / 0 skipped",
            "253 passed, 0 skipped",
            "257 passed / 18 skipped",
            "257 passed, 18 skipped",
            "258 passed / 18 skipped",
            "258 passed, 18 skipped",
            "262 passed / 18 skipped",
            "262 passed, 18 skipped",
            "263 passed / 18 skipped",
            "263 passed, 18 skipped",
            "264 passed / 20 skipped",
            "264 passed, 20 skipped",
            "266 passed / 20 skipped",
            "266 passed, 20 skipped",
            "267 passed / 20 skipped",
            "267 passed, 20 skipped",
            "268 passed / 20 skipped",
            "268 passed, 20 skipped",
            "269 passed / 20 skipped",
            "269 passed, 20 skipped",
            "280 passed / 0 skipped",
            "280 passed, 0 skipped",
            "281 passed / 0 skipped",
            "281 passed, 0 skipped",
            "286 passed / 0 skipped",
            "286 passed, 0 skipped",
            "287 passed / 0 skipped",
            "287 passed, 0 skipped",
            "289 passed / 0 skipped",
            "289 passed, 0 skipped",
            "275 passed / 0 skipped",
            "275 passed, 0 skipped",
            "264 passed / 0 skipped",
            "452 passed / 21 skipped",
            "452 passed, 21 skipped",
            "455 passed / 21 skipped",
            "455 passed, 21 skipped",
            "461 passed / 21 skipped",
            "461 passed, 21 skipped",
            "462 passed / 21 skipped",
            "462 passed, 21 skipped",
            "270 passed / 0 skipped",
            "270 passed, 0 skipped",
            "288 passed / 20 skipped",
            "288 passed, 20 skipped",
            "308 passed / 0 skipped",
            "308 passed, 0 skipped"
        };
        var staleSavedFieldCounts = new[]
        {
            "current source defines 26 PreviousSavedStates",
            "Current source defines 26 PreviousSavedStates",
            "current source now defines 26 PreviousSavedStates",
            "Current source now defines 26 PreviousSavedStates"
        };
        var failures = new List<string>();

        foreach (var path in docsToCheck)
        {
            var text = ReadRepoText(path.Split('/'));

            foreach (var staleCount in staleCounts)
            {
                if (text.Contains(staleCount, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale validation count `{staleCount}`.");
                }
            }

            foreach (var staleSavedFieldCount in staleSavedFieldCounts)
            {
                if (text.Contains(staleSavedFieldCount, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale saved-field count `{staleSavedFieldCount}`.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));

        var currentDocs = string.Join(
            Environment.NewLine,
            docsToCheck.Select(path => ReadRepoText(path.Split('/'))));
        Assert.Contains("475 passed / 21 skipped", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current beta.114 RitsuLib-only manual-test package", currentDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentDocClaimGuardScansProjectStateAndPassedCountWording()
    {
        var checker = ReadRepoText("scripts", "check-sts1-event-current-doc-claims.ps1");

        Assert.Contains(
            "$currentDocClaimSummaryFiles = @('PROJECT_STATE.md'",
            checker,
            StringComparison.Ordinal);
        Assert.Contains("current-doc claims passed", checker, StringComparison.Ordinal);
        Assert.Contains("current-doc claims (744|745|746", checker, StringComparison.Ordinal);
        Assert.Contains("|893|896|897|898|914|915)", checker, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentChangelogUsesSpirePlusEnvironmentNamesFirst()
    {
        var changelog = ReadRepoText("docs", "mod-changelog.md");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            changelog,
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "SPIREPLUS_DISABLE_MORVI=1",
            "preferred `SPIREPLUS_DISABLE_MORVI` plus legacy `EZMB_DISABLE_MORVI`",
            "preferred `SPIREPLUS_DISABLE_LOTHA` plus legacy `EZMB_DISABLE_LOTHA`",
            "SPIREPLUS_FORCE_MORVI_BLESSING=morvi_misprint_press",
            "SPIREPLUS_DISABLE_URDA=1",
            "Legacy `EZMB_ENABLE_VAKUU_FIGHT=1` still works",
            "Legacy `EZMB_FORCE_MORVI_BLESSING` still works",
            "Legacy `EZMB_DISABLE_URDA=1` still works");
        AssertSourceContains(
            scriptsReadme,
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "legacy `EZMB_ENABLE_VAKUU_FIGHT=1`");
        Assert.DoesNotContain("behind `EZMB_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_DISABLE_LOTHA` / `SPIREPLUS_DISABLE_LOTHA`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("Set TEZMB_FORCE_MORVI_BLESSING=", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("Set `EZMB_DISABLE_URDA=1`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_ENABLE_VAKUU_FIGHT=1` / `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`", scriptsReadme, StringComparison.Ordinal);
    }
}
