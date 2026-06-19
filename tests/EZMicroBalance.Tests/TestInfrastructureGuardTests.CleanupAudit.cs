using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class TestInfrastructureGuardTests
{
    [Fact]
    public void CleanupAuditKeepsPromptCoverageAndOwnerDeletionDecisions()
    {
        var cleanupAudit = ReadRepoText("docs", "worktree-cleanup-audit.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");

        Assert.Contains("worktree-cleanup-audit.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("worktree-cleanup-audit.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/worktree-cleanup-audit.md", docInventory, StringComparison.Ordinal);

        AssertSourceContains(
            cleanupAudit,
            "# Worktree Cleanup Audit",
            "## Objective",
            "## Refactor Cleanup Completed",
            "## Completion Audit Against Cleanup Goal",
            "## Prompt-To-Artifact Checklist",
            "## Owner Deletion Decision Checklist",
            "Use this checklist before any permanent deletion.",
            "Confirm whether every uncertain area is useless before permanent deletion",
            "Complete: remaining retained areas have current evidence or hard-rule justification",
            "Remaining large ignored/local areas are retained intentionally",
            "Removed duplicate root mod surfaces",
            "Former root `legacy/`",
            "`source code/`",
            "`publish/`",
            "`.tools/`",
            "`website/` and `.github/workflows/spire-plus-site.yml`");

        Assert.DoesNotContain("Status: Complete", cleanupAudit, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("| `source code/` | Default keep because current tests/docs require it. |", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("Promoted and tracked as current public site source and Pages workflow. Generated `website/forum/` output and `website/**/*.import` metadata remain ignored", cleanupAudit, StringComparison.Ordinal);
    }
}
