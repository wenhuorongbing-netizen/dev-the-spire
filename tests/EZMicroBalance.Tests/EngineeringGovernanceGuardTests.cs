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
            "EZMB_RUN_RELEASE_ARTIFACT_TESTS");

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
            "`validate-repository-hygiene.ps1`");
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
            "README_INSTALL | `C735AA228BBB5CD002BF618334A04483C0013328C82ECC33551C65B0A1165599`");

        AssertSourceContains(
            scope,
            "This file freezes the release-candidate decision boundary",
            "Manual-test build now; release candidate only after live evidence",
            "Vakuu fight | Hidden by default",
            "Website | Not in current release surface",
            "Source review, tests, and package hashes are not enough to close live rows.");

        AssertSourceContains(
            websiteAudit,
            "The archived website is not an active release surface.",
            "EasyFirePlus",
            "needs owner decision",
            "Preview tools now ship inside the Spire Plus page");

        AssertSourceContains(
            traceability,
            "This matrix maps player-visible promises to source, guard, and evidence state.",
            "Manual-test package only",
            "Hidden by default",
            "Integrated into Spire Plus",
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
            "Current count: 134 Harmony patch declarations",
            "A20 dual boss",
            "RewardsSetSynchronizer",
            "StartRunLobby.SyncAscensionChange",
            "RootSightPreviewPolicy",
            "VakuuFightFlow",
            "PreviewTransformPolicy");

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
            "Keep preview-tool changes reviewable as their own Spire Plus batch.",
            "Do not close live/manual rows in a commit that has no live evidence folder.");
    }
}
