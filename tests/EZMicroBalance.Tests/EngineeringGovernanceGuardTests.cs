using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text.Json;
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
        AssertRepoFileExists("scripts", "report-worktree-batches.ps1");

        var gitignore = ReadRepoText(".gitignore");
        Assert.Contains("/tests/**/*.cs.uid", gitignore, StringComparison.Ordinal);
        var testUidFiles = Directory.GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs.uid", SearchOption.TopDirectoryOnly);
        Assert.True(
            testUidFiles.Length == 0,
            $"Test project .cs.uid files are not part of the active deliverable:{Environment.NewLine}{string.Join(Environment.NewLine, testUidFiles)}");

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
            "SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS");

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
            "Local output hygiene",
            "Status and release docs",
            @"^tests/EZMicroBalance\.Tests/[^/]+\.cs\.uid$",
            "Ancient source and tests",
            "Ascension source and tests",
            "Scripts, CI, and validation tests",
            "Website public-info surface",
            "Unclassified",
            "Found $unclassifiedCount unclassified dirty worktree entries.");

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
            "Preview tools",
            "High: run, room, save, lobby, multiplayer, or game lifecycle surface.");
    }

    [Fact]
    public void WorktreeBatchScriptRunsAndWritesBatchPathspecs()
    {
        var script = AssertRepoFileExists("scripts", "report-worktree-batches.ps1");
        var outputDirectory = RepoPath(".tools", "test-worktree-batches", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        try
        {
            var result = RunPowerShell(
                script,
                "-Format",
                "Json",
                "-FailOnUnclassified",
                "-PathspecDirectory",
                outputDirectory);

            Assert.True(result.ExitCode == 0, $"report-worktree-batches.ps1 failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(result.Output);
            var root = document.RootElement;
            Assert.Equal("git status --short", root.GetProperty("Command").GetString());

            var totalDirtyEntries = root.GetProperty("TotalDirtyEntries").GetInt32();
            var summary = root.GetProperty("Summary").EnumerateArray().ToArray();
            var suggestedCommands = root.GetProperty("SuggestedGitAddCommands").EnumerateArray().ToArray();
            Assert.Contains(summary, row => row.GetProperty("Batch").GetInt32() == -1 && row.GetProperty("Count").GetInt32() == 0);
            Assert.Equal(9, suggestedCommands.Length);
            Assert.All(
                suggestedCommands,
                row => Assert.Contains("git add --pathspec-from-file=", row.GetProperty("GitAddCommand").GetString(), StringComparison.Ordinal));

            var pathspecLineTotal = 0;
            for (var batch = 0; batch <= 8; batch++)
            {
                var pathspecPath = Path.Combine(outputDirectory, $"batch-{batch}.pathspec");
                Assert.True(File.Exists(pathspecPath), $"Missing pathspec for batch {batch}.");
                pathspecLineTotal += File.ReadAllLines(pathspecPath).Length;
            }

            Assert.Equal(totalDirtyEntries, pathspecLineTotal);
            Assert.True(File.Exists(Path.Combine(outputDirectory, "manifest.json")));
        }
        finally
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseEvidenceDashboardKeepsLiveRowsOpen()
    {
        var dashboard = ReadRepoText("docs", "release-evidence-status.md");
        AssertSourceContains(
            dashboard,
            "Do not mark a row passed from source review alone.",
            "## Automation Summary",
            "| Current package automation | Passed |",
            "## Verifier Row IDs",
            "These are the exact row IDs required by `scripts/verify-spire-plus-release-evidence.ps1`.",
            "| Row ID | Kind | Status | Owner | Evidence Needed |",
            "verifier fails closed with 19 remaining manual/live rows",
            "| fresh-current-package-loader-smoke | loader | Passed |",
            "| ancient-ui-urda | clicked-ui | Pending |",
            "| ancient-ui-morvi | clicked-ui | Pending |",
            "| ancient-ui-lotha | clicked-ui | Pending |",
            "| ancient-ui-vakuu-normal | clicked-ui | Pending |",
            "| ancient-ui-vakuu-fight | clicked-ui | Pending |",
            "| ancient-reward-visible-relics | gameplay | Pending |",
            "| player-text-tooltip-readability | gameplay | Pending |",
            "| art-resource-routing-live-preview | clicked-ui | Pending |",
            "| vakuu-victory-no-black-screen | gameplay | Pending |",
            "| vakuu-failure-death-path | gameplay | Pending |",
            "| vakuu-active-fight-save-load | save-load | Pending |",
            "| ancient-state-save-load | save-load | Pending |",
            "| rootblight-visual-behavior | gameplay | Pending |",
            "| a11-natural-route-traversal | gameplay | Pending |",
            "| ascension-selector-localization | clicked-ui | Pending |",
            "| a19-a20-dedicated-boss-abilities | gameplay | Pending |",
            "| disable-mod-gameplay | gameplay | Pending |",
            "| preview-tools-live-proof | preview-tools | Pending |",
            "| coop-disposition | coop | Pending |",
            ".\\scripts\\verify-spire-plus-release-evidence.ps1");

        Assert.DoesNotContain("clicked-ancient-ui-urda-morvi-lotha-vakuu", dashboard, StringComparison.Ordinal);
    }

    [Fact]
    public void DocumentationIndexReferencesGovernanceDocsAndScripts()
    {
        var docsReadme = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            docsReadme,
            "`patch-inventory.md`",
            "`release-evidence-status.md`",
            "`specs/release-scope-v1.md`",
            "`specs/release-traceability-matrix.md`",
            "`source-research/run-room-event-reward.md`",
            "`architecture/patch-boundaries.md`",
            "`../scripts/README.md`");

        AssertSourceContains(
            projectMap,
            "`docs/patch-inventory.md`",
            "`docs/release-evidence-status.md`",
            "`docs/specs/release-scope-v1.md`",
            "`docs/specs/release-traceability-matrix.md`",
            "`docs/month-plan/baseline-2026-05-20.md`",
            "`docs/month-plan/commit-boundaries.md`",
            "`docs/adr/`",
            "Generated Harmony patch owner/risk inventory");

        AssertSourceContains(
            docInventory,
            "`docs/specs/release-scope-v1.md`",
            "`docs/specs/release-traceability-matrix.md`",
            "`docs/specs/website-claim-audit.md`",
            "`docs/source-research/multiplayer-save-rng.md`",
            "`docs/architecture/save-state-contracts.md`");

        AssertSourceContains(
            scriptsReadme,
            "`generate-patch-inventory.ps1`",
            "`validate-repository-hygiene.ps1`",
            "`report-worktree-batches.ps1`");
    }

    [Fact]
    public void ReleasePlanningDocsExistAndKeepLiveRowsOpen()
    {
        var baseline = ReadRepoText("docs", "month-plan", "baseline-2026-05-20.md");
        var scope = ReadRepoText("docs", "specs", "release-scope-v1.md");
        var websiteAudit = ReadRepoText("docs", "specs", "website-claim-audit.md");
        var traceability = ReadRepoText("docs", "specs", "release-traceability-matrix.md");
        var runEvidence = ReadRepoText("docs", "source-research", "run-room-event-reward.md");
        var multiplayerEvidence = ReadRepoText("docs", "source-research", "multiplayer-save-rng.md");
        var boundedContexts = ReadRepoText("docs", "architecture", "bounded-contexts.md");
        var patchBoundaries = ReadRepoText("docs", "architecture", "patch-boundaries.md");
        var saveContracts = ReadRepoText("docs", "architecture", "save-state-contracts.md");
        var commitBoundaries = ReadRepoText("docs", "month-plan", "commit-boundaries.md");

        AssertSourceContains(
            baseline,
            "This is not live evidence.",
            "HEAD | `25f99fb",
            "Total patch declarations | 135",
            "Fresh current-package loader smoke | Pending live run",
            "README_INSTALL | `F933C266CBA1A6B1C81A2AC3D4BF1AA30A407BF6676703E95F1EB86724126C04`");

        AssertSourceContains(
            scope,
            "This file freezes the release-candidate decision boundary",
            "Manual-test build now; release candidate only after live evidence",
            "Vakuu fight | Hidden by default",
            "Website | Public-info surface, not mod-release proof",
            "Source review, tests, and package hashes are not enough to close live rows.");

        AssertSourceContains(
            websiteAudit,
            "The active website is a public-info surface, not release-readiness proof.",
            "Current tracked website data",
            "Active CI for the website only.",
            "Preview tools now ship inside the Spire Plus page");

        AssertSourceContains(
            traceability,
            "This matrix maps player-visible promises to source, guard, and evidence state.",
            "Manual-test package only",
            "Hidden by default",
            "Integrated into Spire Plus",
            "May describe manual-test package only",
            "Do not advertise full support");

        AssertSourceContains(
            runEvidence,
            "`source code/src/Core/Runs/RunManager.cs`",
            "`source code/src/Core/Rooms/CombatRoom.cs`",
            "`RewardsSetSynchronizer.SelectLocalReward`",
            "A20 dual boss/courtyard",
            "`ToSerializable` throws if `ParentEventId` is set on a non-prefinished combat room.",
            "Source evidence can justify code shape and tests.");

        AssertSourceContains(
            multiplayerEvidence,
            "`source code/src/Core/Multiplayer/Game/Lobby/StartRunLobby.cs`",
            "`source code/src/Core/Random/PlayerRngSet.cs`",
            "`source code/src/Core/Multiplayer/Game/RewardsSetSynchronizer.cs`",
            "`source code/src/Core/Nodes/GodotExtensions/NClickableControl.cs`",
            "`SyncAscensionChange` warnings",
            "Preview systems that advance real RNG counters before the player commits.",
            "A multiplayer claim requires two-client evidence.");

        AssertSourceContains(
            boundedContexts,
            "AncientExpansionVakuu",
            "AscensionCore",
            "PreviewTools",
            "ReleaseEvidence");

        AssertSourceContains(
            patchBoundaries,
            "Current count: 137 Harmony patch declarations",
            "A20 dual boss",
            "RewardsSetSynchronizer",
            "StartRunLobby.SyncAscensionChange",
            "RootSightPreviewPolicy",
            "VakuuFightFlow",
            "PreviewTransformPolicy",
            "High-Risk Manual Evidence Map",
            "Vakuu child combat",
            "Urda Root Eyes room commit",
            "A20 dual boss flow",
            "Ascension lobby selection",
            "Multiplayer diagnostics",
            "A11-A20 map generation",
            "Reward and preview UI",
            "source-mapped while live proof is still pending",
            "Every high-risk patch group above has a matching row");

        AssertSourceContains(
            saveContracts,
            "Root Eyes",
            "Seed Bank",
            "Lotha Death Reprieve",
            "A20 dual boss/courtyard",
            "Reward alternatives",
            "Preview tools",
            "They do not replace live save/load proof");

        AssertSourceContains(
            commitBoundaries,
            "GOV-WIP-SPLIT remains open",
            ".\\scripts\\report-worktree-batches.ps1 -FailOnUnclassified",
            ".\\scripts\\report-worktree-batches.ps1 -FailOnUnclassified -PathspecDirectory .tools\\worktree-batches\\current",
            "git add --pathspec-from-file=<pathspec>",
            "The manifest includes the exact `git add --pathspec-from-file=<pathspec>` command for each batch.",
            "## Current Dirty Snapshot",
            "Snapshot command: `.\\scripts\\report-worktree-batches.ps1 -FailOnUnclassified -PathspecDirectory .tools\\worktree-batches\\current`, 2026-05-24 after the test UID cleanup.",
            "This snapshot is not a commit manifest.",
            "| 0 | 1 | `.gitignore`, `output/.gdignore`, tracked `output/playwright/` evidence |",
            "| 2 | 12 | `docs/architecture/**`, `docs/specs/**`, `docs/month-plan/**`, archive/index docs |",
            "| 3 | 113 | `EZMicroBalanceCode/Ancients/**`, Ancient support docs, Ancient shared evidence/tests |",
            "| 4 | 69 | `EZMicroBalanceCode/Ascension/**`, `EZMicroBalance/localization/*/ascension.json`, Ascension docs/tests |",
            "| 5 | 79 | `scripts/**`, settings UI localization, `EZMicroBalanceCode/Diagnostics/**`, `EZMicroBalanceCode/Preview/**`, release/CI/test-infrastructure tests, and removed test `.cs.uid` metadata |",
            "| 6 | 13 | Ancient art/resource docs, active image/export resources, and waiting-test docs |",
            "| 7 | 12 | `website/**`, `forum/**` |",
            "Minimum split order: land batches 0, 1, 2, and 5 before gameplay batches",
            "Keep preview-tool changes reviewable as their own Spire Plus batch.",
            "Do not close live/manual rows in a commit that has no live evidence folder.");
    }

    private static (int ExitCode, string Output, string Error) RunPowerShell(string scriptPath, params string[] arguments)
    {
        var executable = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh";
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output, error);
    }

}
