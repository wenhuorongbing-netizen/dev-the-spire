using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
{
    [Fact]
    public void ReleaseEvidenceDashboardKeepsLiveRowsOpen()
    {
        var dashboard = ReadRepoText("docs", "release-evidence-status.md");
        AssertSourceContains(
            dashboard,
            "Do not mark a row passed from source review alone.",
            "## Automation Summary",
            "| Current package automation | Partial |",
            "## Verifier Row IDs",
            "These are the exact row IDs required by `scripts/verify-spire-plus-release-evidence.ps1`.",
            "| Row ID | Kind | Status | Owner | Evidence Needed |",
            "The beta.19 loader smoke is historical startup evidence only",
            "Gameplay, gated Vakuu fight-option/victory return, save-load, preview-tools live behavior, current enabled-mode proof, co-op, and full release-evidence packaging rows remain pending",
            "| fresh-current-package-loader-smoke | loader | Pass |",
            "| mod-settings-current-display | clicked-ui | Pass |",
            "| ancient-ui-urda | clicked-ui | Pass |",
            "| ancient-ui-morvi | clicked-ui | Pass |",
            "| ancient-ui-lotha | clicked-ui | Pass |",
            "| ancient-ui-vakuu-normal | clicked-ui | Pass |",
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
            "`docs/intro.zh.md`",
            "`docs/source-research/multiplayer-save-rng.md`",
            "`docs/architecture/save-state-contracts.md`");

        AssertSourceContains(
            scriptsReadme,
            "`generate-patch-inventory.ps1`",
            "`validate-repository-hygiene.ps1`",
            "`report-worktree-batches.ps1`",
            "`prune-generated-sidecars.ps1`",
            "`prune-stale-publish-packages.ps1`",
            "`spire-plus-package-evidence.ps1`");
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
            "Fresh current-package loader smoke | Historical beta.85 `v0.107.0` Off loader smoke was clean for startup only; beta.96 RitsuLib-only Off proof is previous-package startup context, previous beta.93 AdditiveBatch1 remains previous-package registration context, and live gameplay/manual runs are still pending",
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
            "141 total, 22 high-risk",
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
            "GOV-WIP-SPLIT` is source-fixed for the committed `main` baseline",
            ".\\scripts\\report-worktree-batches.ps1 -FailOnUnclassified",
            ".\\scripts\\report-worktree-batches.ps1 -FailOnUnclassified -PathspecDirectory .tools\\worktree-batches\\current",
            "git add --pathspec-from-file=<pathspec>",
            "The manifest includes the exact `git add --pathspec-from-file=<pathspec>` command for each batch.",
            "## Current Clean Baseline",
            "reported `Total dirty entries: 0` and `Unclassified: 0`",
            "## Batch Ownership Map",
            "| 0 | `.gitignore`, `output/.gdignore`, tracked `output/playwright/` evidence |",
            "| 1 | `PROJECT_STATE.md`, `README.md`, `docs/intro.zh.md`, compact status/release docs |",
            "| 2 | `docs/architecture/**`, `docs/specs/**`, `docs/month-plan/**`, archive/index docs, implementation-record archives |",
            "| 3 | `EZMicroBalanceCode/Ancients/**`, Ancient support docs, Ancient shared evidence/tests |",
            "| 4 | `EZMicroBalanceCode/Ascension/**`, `EZMicroBalance/localization/*/ascension.json`, Ascension docs/tests |",
            "| 5 | `scripts/**`, settings UI localization, `EZMicroBalanceCode/Diagnostics/**`, `EZMicroBalanceCode/Preview/**`, release/CI/test-infrastructure tests, and generated sidecar policy |",
            "| 6 | Ancient art/resource docs, active image/export resources, and waiting-test docs |",
            "| 7 | `website/**`, `forum/**` |",
            "Minimum split order for future broad work",
            "Keep preview-tool changes reviewable as their own Spire Plus batch.",
            "Do not close live/manual rows in a commit that has no live evidence folder.");

        Assert.DoesNotContain("2026-05-24 after the test UID cleanup", commitBoundaries, StringComparison.Ordinal);
        Assert.DoesNotContain("| 3 | 113 |", commitBoundaries, StringComparison.Ordinal);
        Assert.DoesNotContain("| 5 | 79 |", commitBoundaries, StringComparison.Ordinal);
    }
}
