using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class EngineeringGovernanceGuardTests
{
    [Fact]
    public void RepositoryHygieneWorkflowAndTemplatesExist()
    {
        AssertRepoFileExists(".github", "workflows", "repository-hygiene.yml");
        AssertRepoFileExists(".github", "workflows", "full-local-validation.yml");
        AssertRepoFileExists(".github", "pull_request_template.md");
        AssertRepoFileExists(".github", "ISSUE_TEMPLATE", "engineering_issue.md");
        AssertRepoFileExists(".editorconfig");
        AssertRepoFileExists("docs", "adr", "0000-template.md");
        AssertRepoFileExists("scripts", "validate-repository-hygiene.ps1");
        AssertRepoFileExists("scripts", "generate-patch-inventory.ps1");
        AssertRepoFileExists("scripts", "ci-full-validation.ps1");

        var workflow = ReadRepoText(".github", "workflows", "repository-hygiene.yml");
        Assert.Contains("validate-repository-hygiene.ps1", workflow, StringComparison.Ordinal);
        Assert.Contains("git diff --check", workflow, StringComparison.Ordinal);

        var fullWorkflow = ReadRepoText(".github", "workflows", "full-local-validation.yml");
        AssertSourceContains(
            fullWorkflow,
            "workflow_dispatch:",
            "sts2_path:",
            "godot_path:",
            "runs-on:",
            "self-hosted",
            "windows",
            "actions/setup-dotnet@v4",
            "dotnet-version: 9.0.x",
            "ci-full-validation.ps1");

        var fullScript = ReadRepoText("scripts", "ci-full-validation.ps1");
        AssertSourceContains(
            fullScript,
            "STS2_PATH or -Sts2Path is required for full validation.",
            "GODOT_PATH or -GodotPath is required for publish/package validation.",
            "data_sts2_windows_x86_64",
            "BaseLib.dll",
            "Created temporary Directory.Build.props for this validation run.",
            "Removed temporary Directory.Build.props.",
            "dotnet build EZMicroBalance.sln @msbuildProps",
            "dotnet test EZMicroBalance.sln --no-build",
            "dotnet format EZMicroBalance.sln --verify-no-changes --no-restore",
            "dotnet publish EZMicroBalance.sln @msbuildProps",
            "package-spire-plus.ps1 -GameRoot $sts2FullPath",
            "EZMB_RUN_RELEASE_ARTIFACT_TESTS",
            "dotnet test EZFuturePeek.sln --no-build");

        var pullRequestTemplate = ReadRepoText(".github", "pull_request_template.md");
        AssertSourceContains(
            pullRequestTemplate,
            "`EZMicroBalance` manifest id remains unchanged",
            "New or moved Harmony patches are reflected in `docs/patch-inventory.md`",
            "Manual evidence rows stay open unless live proof exists");

        var issueTemplate = ReadRepoText(".github", "ISSUE_TEMPLATE", "engineering_issue.md");
        AssertSourceContains(
            issueTemplate,
            "## Problem",
            "## Player Impact",
            "## Source Evidence",
            "## Acceptance Criteria",
            "## Manual Evidence Needed");

        var adrTemplate = ReadRepoText("docs", "adr", "0000-template.md");
        AssertSourceContains(
            adrTemplate,
            "## Context",
            "## Decision",
            "## Alternatives Considered",
            "## Consequences",
            "## Validation",
            "## Rollback");
    }

    [Fact]
    public void PatchInventoryIsGeneratedReadableAndClassified()
    {
        var inventory = ReadRepoText("docs", "patch-inventory.md");
        var sourcePatchCount = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(RepoPath("EZFuturePeekCode"), "*.cs", SearchOption.AllDirectories))
            .Sum(path => Regex.Matches(File.ReadAllText(path), @"\[HarmonyPatch").Count);

        Assert.Contains($"| Total patch declarations | {sourcePatchCount} |", inventory, StringComparison.Ordinal);
        Assert.Contains("| Unclassified owner | 0 |", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("$(", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("@{File=", inventory, StringComparison.Ordinal);
        AssertSourceContains(
            inventory,
            "| Owner | Risk | File | Line | Patch |",
            "Vakuu",
            "Ascension core",
            "Ascension patches",
            "Future Peek",
            "High: run, room, save, lobby, multiplayer, or game lifecycle surface.");
    }

    [Fact]
    public void ReleaseEvidenceDashboardKeepsLiveRowsOpen()
    {
        var dashboard = ReadRepoText("docs", "release-evidence-status.md");
        AssertSourceContains(
            dashboard,
            "Do not mark a row passed from source review alone.",
            "| Current package automation | Passed |",
            "| Fresh current-package loader smoke | Pending |",
            "| Clicked Ancient UI | Pending |",
            "| Vakuu fight victory | Pending |",
            "| Save/load | Pending |",
            "| Co-op disposition | Pending |",
            ".\\scripts\\verify-spire-plus-release-evidence.ps1");
    }

    [Fact]
    public void DocumentationIndexReferencesGovernanceDocsAndScripts()
    {
        var docsReadme = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            docsReadme,
            "`patch-inventory.md`",
            "`release-evidence-status.md`",
            "`../scripts/README.md`");

        AssertSourceContains(
            projectMap,
            "`docs/patch-inventory.md`",
            "`docs/release-evidence-status.md`",
            "`docs/adr/`",
            "Generated Harmony patch owner/risk inventory");

        AssertSourceContains(
            scriptsReadme,
            "`generate-patch-inventory.ps1`",
            "`validate-repository-hygiene.ps1`");
    }
}
