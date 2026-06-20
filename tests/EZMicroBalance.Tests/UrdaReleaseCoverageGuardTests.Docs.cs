using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaReleaseCoverageGuardTests
{
    [Fact]
    public void UrdaDocsKeepLiveAndSaveLoadVerificationPending()
    {
        var issueIndex = ReadRepoText("docs", "issues.md");
        var urdaIssue = ReadRepoText("docs", "issues", "urda.md");
        var urdaReadme = ReadRepoText("docs", "features", "ancient-expansion-urda", "README.md");
        var urdaApi = ReadRepoText("docs", "features", "ancient-expansion-urda", "api-research.md");
        var urdaChecklist = ReadRepoText("docs", "features", "ancient-expansion-urda", "manual-test-checklist.md");
        var v22Readme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");
        var v22Api = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var v22Plan = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "implementation-plan.md");
        var v22SourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var v22Risk = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");

        var currentUrdaDocs = string.Join(
            Environment.NewLine,
            issueIndex,
            urdaIssue,
            urdaReadme,
            urdaApi,
            urdaChecklist,
            v22Readme,
            v22Api,
            v22Plan,
            v22SourceDesign,
            v22Risk);

        Assert.Contains("`URDA-PROTOTYPE` P0 open", issueIndex, StringComparison.Ordinal);
        Assert.Contains("live gameplay and save/load proof remain pending", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Live gameplay and save/load verification for current Urda remains pending", v22Readme, StringComparison.Ordinal);
        Assert.Contains("not source-proven as persisted", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("## 1A. Live evidence protocol", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\\window-preflight.json -RequireSpireForeground", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <evidence-dir>\\godot.log -OutFile <evidence-dir>\\godot-log-audit.json -FailOnHit", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-postfix-20260513-131752", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-continue-postfix-20260513-134337", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("do not satisfy any gameplay row", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("RitsuLib `ModAncientEventTemplate`", urdaIssue, StringComparison.Ordinal);
        Assert.Contains("SharedAncient<EzmbUrda>()", urdaIssue, StringComparison.Ordinal);
        Assert.Contains("Prefer native game command APIs, RitsuLib APIs, and template-supported APIs before Harmony", v22Api, StringComparison.Ordinal);
        Assert.Contains("BaseLib references as historical", v22Plan, StringComparison.Ordinal);
        Assert.Contains("BaseLib notes are historical migration context only", v22SourceDesign, StringComparison.Ordinal);
        Assert.DoesNotContain("Urda live gameplay verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Urda save/load verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("URDA-PROTOTYPE | Closed", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Urda now derives from BaseLib", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("BaseLib/template Ancient-pool API", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Inspect BaseLib/template APIs", currentUrdaDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x]", urdaChecklist, StringComparison.Ordinal);
    }
}
