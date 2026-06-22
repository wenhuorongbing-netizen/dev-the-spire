using System;
using System.Diagnostics;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
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
        AssertRepoFileExists("scripts", "check-github-workflow-runs.ps1");
        AssertRepoFileExists("scripts", "report-worktree-batches.ps1");
        AssertRepoFileExists("scripts", "prune-generated-sidecars.ps1");
        AssertRepoFileExists("scripts", "prune-stale-publish-packages.ps1");
        AssertRepoFileExists("AGENTS.md");

        var gitignore = ReadRepoText(".gitignore");
        Assert.Contains("/EZMicroBalanceCode/**/*.cs.uid", gitignore, StringComparison.Ordinal);
        Assert.Contains("/tests/**/*.cs.uid", gitignore, StringComparison.Ordinal);
        Assert.Contains("/tests/**/TestResults/", gitignore, StringComparison.Ordinal);
        Assert.Contains("/.playwright-cli/", gitignore, StringComparison.Ordinal);
        Assert.Contains("/website/**/*.import", gitignore, StringComparison.Ordinal);

        var trackedSourceUidFiles = GitLsFiles("EZMicroBalanceCode/**/*.cs.uid");
        Assert.True(
            trackedSourceUidFiles.Length == 0,
            $"Godot C# source .uid sidecars are generated metadata and must not be tracked:{Environment.NewLine}{string.Join(Environment.NewLine, trackedSourceUidFiles)}");

        var trackedTestUidFiles = GitLsFiles("tests/**/*.cs.uid");
        Assert.True(
            trackedTestUidFiles.Length == 0,
            $"Test project .cs.uid files are generated metadata and must not be tracked:{Environment.NewLine}{string.Join(Environment.NewLine, trackedTestUidFiles)}");

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
            "mod_manifest.json",
            "ritsulib-variants.manifest",
            "Created temporary Directory.Build.props for this validation run.",
            "Removed temporary Directory.Build.props.",
            "[ValidateSet('SplitReleaseEvidence', 'Solution')]",
            "[string]$TestStrategy = 'SplitReleaseEvidence'",
            "Invoke-NativeCommand",
            "Invoke-SpirePlusTestLane",
            "@('build', 'EZMicroBalance.sln') + $msbuildProps",
            "@('test', 'EZMicroBalance.sln', '--no-build')",
            "FullyQualifiedName~ReleaseEvidenceGateTests",
            "FullyQualifiedName!~ReleaseEvidenceGateTests",
            "RunConfiguration.MaxCpuCount=1",
            "@('format', 'EZMicroBalance.sln', '--verify-no-changes', '--no-restore')",
            "@('publish', 'EZMicroBalance.sln') + $msbuildProps",
            "package-spire-plus.ps1 -GameRoot $sts2FullPath",
            "SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS");

        var repositoryHygieneScript = ReadRepoText("scripts", "validate-repository-hygiene.ps1");
        AssertSourceContains(
            repositoryHygieneScript,
            "Assert-RetiredSharedRuntimeAbsent",
            "git -C $repoRoot ls-files --",
            "Retired shared-runtime guidance must stay out of tracked text files",
            "Use STS2-RitsuLib docs and APIs instead",
            ".csproj");

        var workflowRunCheckScript = ReadRepoText("scripts", "check-github-workflow-runs.ps1");
        AssertSourceContains(
            workflowRunCheckScript,
            "wenhuorongbing-netizen/dev-the-spire",
            "Full Local Validation",
            "https://api.github.com/repos/$Repository/actions/runs",
            "Invoke-RestMethod",
            "MatchingRunCount",
            "LatestRun",
            "[switch]$RequireSuccessfulRun",
            "No completed successful '$WorkflowName' run was found");

        var worktreeBatchScript = ReadRepoText("scripts", "report-worktree-batches.ps1");
        AssertSourceContains(
            worktreeBatchScript,
            "git -C $repoRoot status --short",
            "[switch]$FailOnUnclassified",
            "[string]$PathspecDirectory",
            "batch-{0}.pathspec",
            "manifest.json",
            "SuggestedGitAddCommands",
            "GitAddCommand",
            "git add --pathspec-from-file=",
            "docs/intro.zh.md",
            "docs/archive/implementation-records/",
            "docs/archive/legacy-planning/",
            "Local output hygiene",
            "Status and release docs",
            @"^tests/EZMicroBalance\.Tests/[^/]+\.cs\.uid$",
            "Ancient source and tests",
            "Ascension source and tests",
            "Scripts, CI, and validation tests",
            "Website public-info surface",
            "Unclassified",
            ".playwright-cli/",
            "EZMicroBalanceCode/Core/",
            "Found $unclassifiedCount unclassified dirty worktree entries.");
        Assert.DoesNotContain("EZMicroBalanceCode/MainFile.cs.uid", worktreeBatchScript, StringComparison.Ordinal);

        var sidecarPruneScript = ReadRepoText("scripts", "prune-generated-sidecars.ps1");
        AssertSourceContains(
            sidecarPruneScript,
            "[switch]$DryRun",
            "EZMicroBalanceCode",
            "tests",
            "website",
            "*.cs.uid",
            "*.import",
            "Get-RepoRelativePath",
            "Refusing to prune path outside expected parent",
            "Remove-Item -LiteralPath $file.FullName -Force",
            "Dry run complete");
        Assert.DoesNotContain("git clean", sidecarPruneScript, StringComparison.OrdinalIgnoreCase);

        var stalePublishPruneScript = ReadRepoText("scripts", "prune-stale-publish-packages.ps1");
        AssertSourceContains(
            stalePublishPruneScript,
            "[switch]$DryRun",
            "EZMicroBalance.json",
            "publish",
            "SpirePlus-$currentVersion",
            "v0\\.1\\.0-private-beta",
            "Current package zip must exist before pruning stale packages",
            "Refusing to prune path outside expected parent",
            "[int]$Matches[1] -lt $currentBetaNumber",
            "Remove-Item -LiteralPath $candidate.FullName -Recurse -Force",
            "Dry run complete");
        Assert.DoesNotContain("git clean", stalePublishPruneScript, StringComparison.OrdinalIgnoreCase);

        var pullRequestTemplate = ReadRepoText(".github", "pull_request_template.md");
        AssertSourceContains(
            pullRequestTemplate,
            "`EZMicroBalance` manifest id remains unchanged",
            "New or moved Harmony patches are reflected in `docs/patch-inventory.md`",
            "Manual evidence rows stay open unless live proof exists",
            "High-risk patch seams",
            "Source-only pass does not close live proof gates");

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
    public void AgentInstructionsKeepFutureWorkOnRitsuLibLane()
    {
        var agentGuidance = ReadRepoText("AGENTS.md");

        AssertSourceContains(
            agentGuidance,
            "STS2-RitsuLib",
            "The active Spire Plus package must remain RitsuLib-only",
            "Prefer RitsuLib, local game command APIs, and template-supported APIs.",
            "inspect RitsuLib/template APIs",
            "Install STS2-RitsuLib `v0.4.33` under `<GameRoot>\\mods\\STS2-RitsuLib`",
            "Before release: verify STS2-RitsuLib and Spire Plus load in-game",
            "## RitsuLib Dependency Rule");

        foreach (var token in RetiredSharedRuntimeTokens())
        {
            Assert.DoesNotContain(token, agentGuidance, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void RetiredSharedRuntimeNameDoesNotReappearInTrackedText()
    {
        var textExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cs",
            ".csproj",
            ".css",
            ".cfg",
            ".editorconfig",
            ".gitattributes",
            ".gitignore",
            ".html",
            ".json",
            ".md",
            ".props",
            ".ps1",
            ".sln",
            ".targets",
            ".ts",
            ".txt",
            ".xml",
            ".yaml",
            ".yml"
        };

        Assert.Contains("AGENTS.md", GitLsFiles(), StringComparer.Ordinal);

        var violations = new List<string>();
        foreach (var trackedFile in GitLsFiles())
        {
            var extension = Path.GetExtension(trackedFile);
            if (!textExtensions.Contains(extension))
            {
                continue;
            }

            var path = RepoPath(trackedFile.Split('/'));
            var text = File.ReadAllText(path);
            foreach (var token in RetiredSharedRuntimeTokens())
            {
                if (text.Contains(token, StringComparison.OrdinalIgnoreCase))
                {
                    violations.Add(trackedFile);
                    break;
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Retired shared-runtime guidance must stay out of tracked text files. Use STS2-RitsuLib docs and APIs instead:"
            + Environment.NewLine
            + string.Join(Environment.NewLine, violations.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(path => path)));
    }

    private static string[] RetiredSharedRuntimeTokens() =>
    [
        string.Concat("Base", "Lib"),
        string.Concat("base ", "lib"),
        string.Concat("base-", "lib")
    ];

    private static string[] GitLsFiles(params string[] pathspecs)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("ls-files");
        startInfo.ArgumentList.Add("--");
        foreach (var pathspec in pathspecs)
        {
            startInfo.ArgumentList.Add(pathspec);
        }

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), "Timed out running git ls-files.");
        Assert.True(process.ExitCode == 0, $"git ls-files failed:{Environment.NewLine}{output}{error}");

        return output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
    }
}
