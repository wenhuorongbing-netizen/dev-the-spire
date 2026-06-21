using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseSafetyExpandedGuardTests
{
    [Fact]
    public void PrivateBetaReleaseCompletionAuditMapsFinishGoalToActualEvidenceAndOpenBlockers()
    {
        var audit = ReadRepoText("docs", "private-beta-release-completion-audit.md");
        var docsIndex = ReadRepoText("docs", "README.md");

        AssertSourceContains(
            audit,
            "# Spire Plus Private Beta Release Completion Audit",
            "## Objective Restated",
            "## Objective Coverage Recheck",
            "## Prompt-To-Artifact Checklist",
            "## Missing Or Weakly Verified Items",
            "## Conclusion",
            "earlier beta.88 pass",
            "runtime preflight 27 / 0",
            "current-doc claims 1314 / 0",
            "static suite 15 / 0",
            "beta.99 RitsuLib-only source/build/publish/package refresh",
            "beta.99 loader/settings proof pending, previous beta.96 Off loader proof, previous beta.93 AdditiveBatch1 loader/registration proof",
            "current-package-smoke-20260514-015901",
            "urda-pck-resource-load-20260513-123345",
            "window-preflight-smoke-20260513-135402",
            "Full Ancient reward runtime matrix",
            "Disable-mod gameplay behavior in an actual run",
            "Natural A11 click-by-click traversal",
            "Two-client multiplayer/co-op matrix",
            "Vakuu dedicated combat loop",
            "Ancient reward visibility",
            "Player text, UI, and resource routing",
            "fails closed with 21 manual rows",
            "release-ready-path-containment-smoke",
            "evidence dirs outside the evidence root",
            "required-file/screenshot paths that escape their row evidence dir",
            "Not achieved.",
            "It is not private-beta release-ready");

        Assert.Contains("| Ancient reward gameplay |", audit, StringComparison.Ordinal);
        Assert.Contains("| Source/package guarded; not release-ready until live victory return, no-black-screen, active-fight/pre-finished save-load, and failure/death rows pass. |", audit, StringComparison.Ordinal);
        Assert.Contains("| Source guarded; live relic-bar visibility and hover readability remain pending. |", audit, StringComparison.Ordinal);
        Assert.Contains("| Static/resource guarded; clicked Ancient screenshots, combat-scene screenshots, and live tooltip fit remain pending. |", audit, StringComparison.Ordinal);
        Assert.Contains("| Not complete: runtime results remain pending |", audit, StringComparison.Ordinal);
        Assert.Contains("Invalid live screenshot attempts are not counted as gameplay evidence.", audit, StringComparison.Ordinal);
        Assert.Contains("they do not satisfy live Urda, Rootblight, or gameplay rows.", audit, StringComparison.Ordinal);
        AssertEvidenceLabelOnlyReferencedAsInvalid(audit, "live-urda-postfix-20260513-131752");
        AssertEvidenceLabelOnlyReferencedAsInvalid(audit, "live-urda-continue-postfix-20260513-134337");
        Assert.Contains("| Worktree/release handoff |", audit, StringComparison.Ordinal);
        Assert.Contains("| Not complete |", audit, StringComparison.Ordinal);
        Assert.Contains("Release completion audit", docsIndex, StringComparison.Ordinal);
        Assert.Contains("private-beta-release-completion-audit.md", docsIndex, StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", audit, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotMatch(@"(?i)\b(?:is|are)\s+(?:private-beta\s+)?release-ready\b", audit);
    }

    [Fact]
    public void ReleaseChecklistProofAuditKeepsManualTestRowsOpen()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var testReadyGoal = ReadRepoText("docs", "test-ready-development-goal.md");

        AssertSourceContains(
            testReadyGoal,
            "Goal: keep the current `Spire Plus` workspace at a user-test-ready manual test build",
            "Current stop line: Codex should not chase release-ready evidence in this pass.",
            "The user will run live/manual testing.",
            "This is not a release-ready claim.");

        AssertSourceContains(
            releaseChecklist,
            "## Proof Audit",
            "Dedicated Vakuu combat loop",
            "Ancient rewards visible to players",
            "Player text and tooltip polish",
            "UI and art resource routing",
            "Automation and package parity",
            "Documented publish blockers",
            "live victory return",
            "no-black-screen proof",
            "active-fight/pre-finished save-load",
            "death/failure path",
            "co-op disposition",
            "relic-bar visibility",
            "hover readability",
            "clicked Ancient screenshots",
            "combat-scene screenshots",
            "Current package is a user-test-ready handoff; publish-proof requires the open manual rows");

        Assert.Contains("- [ ] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Save/load-sensitive behavior is tested.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Fight Vakuu remains hidden by default.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Save/load-sensitive behavior is tested.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Fight Vakuu remains hidden by default.", releaseChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseEvidenceVerifierCoversManualBlockersBeforeReleaseClaims()
    {
        var verifier = ReadRepoText("scripts", "verify-spire-plus-release-evidence.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");

        AssertSourceContains(
            verifier,
            "PackageSha256 = \"\"",
            "PackagePath = \"\"",
            "Get-SpirePlusPackageRelativePath -RepoRoot $repoRoot",
            "Resolve-SpirePlusPackagePath -RepoRoot $repoRoot -PackagePath $PackagePath",
            "Get-SpirePlusPackageSha256 -RepoRoot $repoRoot -PackagePath $PackagePath",
            "MinScreenshotWidth = 800",
            "MinScreenshotHeight = 450",
            "Get-SpirePlusFileSha256 -Path $packageFull",
            "ActualPackageSha256 = $actualPackageSha256",
            "WritePassMarker",
            "PassMarkerPath is outside EvidenceRoot",
            "release-evidence-verifier-pass.json",
            "ManifestPath is outside EvidenceRoot",
            "Test-PathWithin",
            "Resolve-EvidenceFilePath",
            "Merge-RequiredEvidenceFiles",
            "DefaultFiles",
            "RowFiles",
            "Get-RequiredRowExtraFiles",
            "ExtraRequiredFiles",
            "ancient-reward-relics-checklist.md",
            "Test-AncientRewardRelicsChecklist",
            "player-text-qa-checklist.md",
            "Test-PlayerTextQaChecklist",
            "art-resource-routing-checklist.md",
            "Test-ArtResourceRoutingChecklist",
            "rootblight-behavior-checklist.md",
            "Test-RootblightBehaviorChecklist",
            "vakuu-victory-checklist.md",
            "vakuu-failure-death-checklist.md",
            "vakuu-save-load-checklist.md",
            "preview-tools-checklist.md",
            "coop-disposition-checklist.md",
            "Test-SimpleChecklistRows",
            "boss-ability-checklist.md",
            "Kind '$rowKind' does not match required kind",
            "EvidenceDir is outside EvidenceRoot",
            "required evidence file path escapes EvidenceDir",
            "screenshot path escapes EvidenceDir",
            "Add-Warning",
            "$requiredRowIds",
            "Release evidence manifest contains a row with no Id; it is ignored.",
            "Unknown release evidence row id ignored",
            "$requiredReleaseRows",
            "command.txt",
            "fresh-current-package-loader-smoke",
            "loader",
            "enabled-mods.txt",
            "ancient-ui-urda",
            "ancient-ui-morvi",
            "ancient-ui-lotha",
            "ancient-ui-vakuu-normal",
            "ancient-ui-vakuu-fight",
            "ancient-reward-visible-relics",
            "player-text-tooltip-readability",
            "art-resource-routing-live-preview",
            "vakuu-victory-no-black-screen",
            "vakuu-failure-death-path",
            "vakuu-active-fight-save-load",
            "ancient-state-save-load",
            "rootblight-visual-behavior",
            "a11-natural-route-traversal",
            "a19-a20-dedicated-boss-abilities",
            "disable-mod-gameplay",
            "preview-tools-live-proof",
            "preview-tools",
            "coop-disposition",
            "godot-log-audit.json",
            "SpireForeground",
            "Clean",
            "ExplicitOwnerDecision",
            "ReleaseNote",
            "AllowDeferred",
            "Package under test does not exist",
            "Actual package SHA256",
            "Duplicate release evidence row id",
            "$invalidEvidenceNotePattern",
            "required evidence file is empty",
            "requiredFileString.EndsWith('.md'",
            "evidence note '$requiredFileString' is empty",
            "evidence note '$requiredFileString' describes invalid or non-counting evidence",
            "ResultNote describes invalid or non-counting evidence",
            "screenshot file is empty",
            "Test-PngSignature",
            "Get-PngDimensions",
            "Test-PngMinimumDimensions",
            "screenshot file is not a valid PNG",
            "screenshot file is too small",
            "has no valid PNG screenshots at least",
            "has no valid non-empty PNG screenshots",
            "has only empty PNG screenshots",
            "not counted|invalid|main menu|wrong surface|covered by|not gameplay evidence|do not satisfy|does not satisfy|loader health only");

        Assert.Contains("verify-spire-plus-release-evidence.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("verify-spire-plus-release-evidence.ps1", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("verify-spire-plus-release-evidence.ps1", handoff, StringComparison.Ordinal);
        Assert.Contains("Deferred rows fail", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Use `-AllowDeferred` only after an explicit owner-approved release-note deferral", handoff, StringComparison.Ordinal);
    }

}
