using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseSafetyExpandedGuardTests
{
    [Fact]
    public void BootstrapAndActiveAncientWorkLogStayCurrentAndReadable()
    {
        var bootstrap = ReadRepoText("scripts", "bootstrap-windows.ps1");
        var workLog = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md");

        AssertSourceContains(
            bootstrap,
            "Spire Plus Windows bootstrap",
            "Install BaseLib v3.1.4 under <GameRoot>\\mods\\BaseLib before game verification.",
            "BaseLib plus Spire Plus appear and are enabled.");
        Assert.DoesNotContain("EzDailyContent Windows bootstrap", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain("BaseLib v3.1.0", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain("BaseLib plus EzDailyContent appear", bootstrap, StringComparison.Ordinal);

        AssertSourceContains(
            workLog,
            "compact current-facing summary",
            "work-log-20260517-pre-cleanup.md",
            "Recovered the user-uploaded Morvi blue-eye court background",
            "Recovered the correct user-uploaded horizontal mirror-ensemble image",
            "Restored the user-accepted 16:9 Urda root-mother background",
            "draw 1 with no Energy gain",
            "No live game, clicked Ancient UI, save-load, failure/death path, or co-op proof is claimed here.");

        foreach (var corruptedMarker in new[]
        {
            "sdotnet",
            "sscripts",
            "sEZMicroBalance",
            "sgodot",
            "s.tools",
            "sPROJECT_STATE",
            "sAGENTS",
            "sGPTimage2s",
            "sfinal_generateds",
            "sFound 22 SavedSpireFieldss"
        })
        {
            Assert.DoesNotContain(corruptedMarker, workLog, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void SavedSpireFieldsAcrossActiveSourceAreUniqueCoveredAndSmokeDocumented()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var sourceWithoutDefinitions = string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith("AncientSavedStateFields.cs", StringComparison.Ordinal) &&
                               !path.EndsWith("AscensionSavedStateFields.cs", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));

        var fields = Regex.Matches(
                allSource,
                @"SavedSpireField<(?<types>[^>]+)>\s+(?<name>[A-Za-z0-9_]+)\s*=\s*\r?\n\s*new\([^""]*""(?<key>EZMicroBalance[^""]+)""",
                RegexOptions.CultureInvariant)
            .Cast<Match>()
            .Select(match => new
            {
                Name = match.Groups["name"].Value,
                Key = match.Groups["key"].Value,
                Types = match.Groups["types"].Value
            })
            .OrderBy(field => field.Key, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(30, fields.Length);
        Assert.Equal(fields.Length, fields.Select(field => field.Key).Distinct(StringComparer.Ordinal).Count());
        Assert.All(fields, field => Assert.StartsWith("EZMicroBalance", field.Key, StringComparison.Ordinal));
        Assert.All(fields, field => Assert.Contains(field.Name, sourceWithoutDefinitions, StringComparison.Ordinal));
        Assert.Contains(fields, field => field.Types == "PrismaticGem, int");
        Assert.Contains(fields, field => field.Types == "PaelsTooth, int");
        Assert.Contains(fields, field => field.Types == "CardModel, bool");
        Assert.Contains(fields, field => field.Types == "CardModel, string");
        Assert.Contains(fields, field => field.Types == "Player, bool");
        Assert.Contains(fields, field => field.Types == "Player, int");
        Assert.Contains(fields, field => field.Types == "Player, string");
        Assert.Contains(fields, field => field.Types == "RootBud, bool");
        Assert.Contains(fields, field => field.Types == "RootBud, int");
        Assert.Contains(fields, field => field.Types == "RootFamilyCard, bool");

        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");
        Assert.Contains("Current source defines 30 SavedSpireFields", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", currentDocs, StringComparison.Ordinal);
        Assert.Contains("`Found 22 SavedSpireFields`", currentDocs, StringComparison.Ordinal);
        Assert.Contains("fresh-current-package-loader-smoke", currentDocs, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus error signatures for technical id `EZMicroBalance`", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Historical 22-field loader evidence", devEnvironment, StringComparison.Ordinal);
        Assert.Contains("Historical beta.19 loader evidence:", devEnvironment, StringComparison.Ordinal);
        Assert.Contains("Current source defines 30 SavedSpireFields", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("current normal Steam-client helper startup/log pass", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("current-package startup/log verification", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Status: current normal Steam startup/log verification passed", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Current normal Steam helper startup/log pass", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("current-package normal Steam-client startup/log pass", currentDocs, StringComparison.Ordinal);
        foreach (var currentSmokeClaim in new[]
        {
            "current-package loader/resource smoke is clean",
            "controlled smoke profile for the current package"
        })
        {
            Assert.DoesNotContain(currentSmokeClaim, currentDocs, StringComparison.OrdinalIgnoreCase);
        }
        AssertNoCurrentFacing22FieldSmokePassClaims(currentDocs);
        Assert.DoesNotContain("latest clean controlled smoke reported 13", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("current source defines 22 SavedSpireFields, while", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Found 9 SavedSpireFields", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
        Assert.DoesNotContain("reported 7 SavedSpireFields", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
    }

    [Fact]
    public void TestReadyCompletionAuditMapsGoalToConcreteArtifacts()
    {
        var audit = ReadRepoText("docs", "test-ready-completion-audit.md");

        AssertSourceContains(
            audit,
            "# Spire Plus Test-Ready Completion Audit",
            "## Objective Restated",
            "## Prompt-To-Artifact Checklist",
            "## Missing Or Weakly Verified Items",
            "Stable technical id and display-name split",
            "Phase 4 Lotha first slice",
            "Source-complete / live-pending",
            "Phase 8 required commands",
            "Beta.19 loader parity is covered",
            "It is not private-beta release-ready");

        Assert.Contains("`EZMicroBalance`", audit, StringComparison.Ordinal);
        Assert.Contains("`Spire Plus`", audit, StringComparison.Ordinal);
        Assert.Contains("Current source defines 30 SavedSpireFields", audit, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", audit, StringComparison.Ordinal);
        Assert.Contains("historical log records", audit, StringComparison.Ordinal);
        Assert.Contains("`Found 22 SavedSpireFields`", audit, StringComparison.Ordinal);
        Assert.Contains("Beta.19 loader parity is covered", audit, StringComparison.Ordinal);
        Assert.Contains("reports `v0.1.0-private-beta.19`", audit, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus error signatures for technical id `EZMicroBalance`", audit, StringComparison.Ordinal);
        Assert.Contains("beta.19 normal Steam-client startup/log verification reports `Found 30 SavedSpireFields`", audit, StringComparison.Ordinal);
        Assert.Contains("historical beta.19 loader", audit, StringComparison.Ordinal);
        Assert.Contains("refreshed Mod Settings UI list capture now shows `Spire Plus`", audit, StringComparison.Ordinal);
        Assert.Contains("Two-client multiplayer matrix is pending", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveSessionHelperStaysRestoreSafeAndDocsKeepLoaderEvidenceSeparateFromGameplay()
    {
        var helper = ReadRepoText("scripts", "spire-plus-live-session.ps1");
        var windowPreflight = ReadRepoText("scripts", "check-spire-window-preflight.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        AssertSourceContains(
            helper,
            "[ValidateSet('Prepare', 'Restore')]",
            "[switch]$MoveOtherMods",
            "[switch]$MoveCurrentRuns",
            "[switch]$DisableSpirePlus",
            "[switch]$StopGameOnRestore",
            "[switch]$PreserveNewCurrentRunsOnRestore",
            "session-state.json",
            "restore-state.json",
            "Assert-PathInside",
            "Move-ModsForIsolation",
            "Move-CurrentRuns",
            "Move-NewCurrentRunsBeforeRestore",
            "Restore-MovedItems",
            "live-spire-plus-disabled-session",
            "DisableSpirePlus requires -MoveOtherMods",
            "$allowedModIds = if ($DisableSpirePlus) { @('BaseLib') } else { $defaultAllowedModIds }",
            "AllowedModIds = @($allowedModIds)",
            "DisableSpirePlus = [bool]$DisableSpirePlus",
            "Start-Process -FilePath $SteamExe",
            "'-applaunch', '2868840'",
            "Copy-Item -LiteralPath $settingsBefore -Destination $session.SettingsPath -Force");

        Assert.DoesNotContain("Remove-Item", helper, StringComparison.OrdinalIgnoreCase);
        AssertSourceContains(
            windowPreflight,
            "[switch]$RequireSpireForeground",
            "GetForegroundWindow",
            "GetWindowThreadProcessId",
            "SlayTheSpire2",
            "SpireForeground",
            "CaptureGuidance",
            "exit 2");
        Assert.DoesNotContain("Stop-Process", windowPreflight, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Move-Item", windowPreflight, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Remove-Item", windowPreflight, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("spire-plus-live-session.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("check-spire-window-preflight.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-DisableSpirePlus", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-RequireSpireForeground", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-PreserveNewCurrentRunsOnRestore", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("test-created `current_run*` files are preserved", currentDocs, StringComparison.Ordinal);
        Assert.Contains("covered desktop captures", currentDocs, StringComparison.Ordinal);
        Assert.Contains("window-preflight-smoke-20260513-135402", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live-helper-preserve-current-run-smoke-20260513-133431", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live-spire-plus-session-20260513-125206", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live-spire-plus-disabled-session-20260513-143020", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Loaded 1 mods (1 total)", currentDocs, StringComparison.Ordinal);
        Assert.Contains("settings-only disabled attempt", currentDocs, StringComparison.Ordinal);
        Assert.Contains("This is loader/helper evidence, not live gameplay evidence.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("This is loader/helper evidence only; gameplay/manual gates remain pending.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("manual feature matrix has runtime gameplay", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Two-client multiplayer matrix is pending", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("helper-driven startup/log pass proves gameplay", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("disable-mod gameplay verified", currentDocs, StringComparison.OrdinalIgnoreCase);
    }

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
            "296 passed / 20 skipped",
            "316 passed / 0 skipped",
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
            "fails closed with 20 manual rows",
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

    [Fact]
    public void AscensionPrototypeMutationPathsStayBehindGatesAndCommandApis()
    {
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20Patches.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var forgeService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");

        AssertSourceContains(
            initializer,
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||",
            "AscensionFeatureGate.IsDiagnosticsEnabled",
            "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) &&",
            "ShouldDisableUnverifiedCoopCombatHook");

        AssertSourceContains(
            mapService,
            "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&",
            "return map;",
            "if (AscensionFeatureGate.IsMapGeometryEnabled(runState))",
            "if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))",
            "if (!AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))",
            "if (!AscensionFeatureGate.IsBannerRoomEnabled(runState))",
            "var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);",
            "var brandedFormEnabled = AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState);",
            "if (!bossSealsEnabled && !brandedFormEnabled)",
            "if (bossSealsEnabled)",
            "if (!brandedFormEnabled)",
            "EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point)",
            "EnsureQuestMarker<AscensionMapQuestMarker>(point)",
            "point.AddQuest(ModelDb.GetById<TMarker>(ModelDb.GetId<TMarker>()))",
            "runState.Map = appliedMap",
            "new SavedActMap(saved)",
            "A11ExtraMapColumns",
            "A11ActOneExtraMapRows",
            "A11ActTwoExtraMapRows",
            "A11ActThreeExtraMapRows",
            "TryInsertA11WidthChoice(saved)",
            "HasA11InsertedColumnRouteChoice",
            "HasSerializablePath(saved.StartingPoint",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "EnumerateDeepBranchColumns(map)",
            "IsDeepBranchRouteSafe(saved, plan)",
            "HasSerializablePathAvoiding",
            "safe-route reconnect",
            "canBeModified: false",
            "DeepBranchNodeKind.EnhancedReward");

        AssertSourceContains(
            a20Patch,
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "finalAct.HasSecondBoss",
            "finalAct.SetSecondBossEncounter(secondBoss)",
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))",
            "AscensionA20CourtyardService.ShouldEnterCourtyard(runState)",
            "AscensionA20CourtyardService.EnterCourtyard(__instance, runState)");

        Assert.DoesNotContain("new AscensionMapQuestMarker(", mapService, StringComparison.Ordinal);
        Assert.DoesNotContain("new FiremarkedEliteMapQuestMarker(", mapService, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "if (AscensionFeatureGate.IsBossSealsEnabled(player.RunState))",
            "if (AscensionFeatureGate.IsFissionEnabled(player.RunState))",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications)",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoModifyHooks)",
            "CardCmd.Enchant<FissionEnchantment>(modifiedCard, 1m)",
            "new CardCreationOptions(pool, CardCreationSource.Other, CardRarityOddsType.Uniform)",
            "CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications",
            "TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)",
            "TryAddA20BossOneCardReward(player, rewards, room)",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player)",
            "metadata?.DeepBranch != DeepBranchNodeKind.EnhancedReward",
            "new RelicReward(RelicRarity.Uncommon, player)");

        AssertSourceContains(
            combatService,
            "AscensionFeatureGate.IsFiremarkedEliteEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBannerRoomEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(combatState.RunState)",
            "await CreatureCmd.GainBlock",
            "FindFiremarkHost(combatState)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>",
            "PowerCmd.Apply<FiremarkMightOverflowPower>",
            "await PowerCmd.Apply<ArtifactPower>",
            "await PowerCmd.Apply<VulnerablePower>",
            "await PowerCmd.Apply<StrengthPower>",
            "var definition = metadata.BossSeal",
            "TrackInkReturnFromDamage",
            "CalculateInkReturnRestoreAmount",
            "TrackKnowledgeDemonEnemyMove",
            "BossSealId.AeonglassHourglass",
            "enemy.Monster is Aeonglass",
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2",
            "TrackAeonglassEnergySpent",
            "SettleAeonglassTimeSand",
            "tracker.AeonglassExtraWitherFromSands",
            "CardPileCmd.AddToCombatAndPreview<Wither>",
            "TryApplyResidualSamples");

        AssertSourceContains(
            forgeService,
            "AscensionSavedStateFields.ForgeTokenHeld[player]",
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "await PlayerCmd.GainGold",
            "CardCmd.Upgrade",
            "AscensionSavedStateFields.ForgeTokenHeld[player] = false");
    }

    [Fact]
    public void SimplifiedChineseLocalizationContainsNoVisibleAsciiWords()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var failures = new List<string>();

        foreach (var file in Directory.GetFiles(zhsRoot, "*.json", SearchOption.AllDirectories))
        {
            var relativePath = ToRepoRelativePath(file);
            using var document = JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8));
            foreach (var (key, value) in JsonStringValues(document.RootElement))
            {
                if (relativePath.EndsWith("EZMicroBalance/localization/zhs/settings_ui.json", StringComparison.Ordinal) &&
                    (key == "EZMICROBALANCE.mod_title" || key == "SPIREPLUS.mod_title"))
                {
                    continue;
                }

                var visibleValue = RemoveLocalizationPlaceholders(value);
                visibleValue = Regex.Replace(visibleValue, @"\[(?:/)?[A-Za-z][^\]]*\]", string.Empty, RegexOptions.CultureInvariant);
                foreach (Match match in Regex.Matches(visibleValue, @"[A-Za-z][A-Za-z0-9_-]*", RegexOptions.CultureInvariant))
                {
                    if (match.Value is "I" or "II" or "III")
                    {
                        continue;
                    }

                    failures.Add($"{relativePath}:{key} contains raw ASCII word `{match.Value}` in `{value}`");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private static string RemoveLocalizationPlaceholders(string value)
    {
        var previous = value;
        while (true)
        {
            var next = Regex.Replace(previous, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
            if (next == previous)
            {
                return next;
            }

            previous = next;
        }
    }

    [Fact]
    public void SimplifiedChineseLocalizationContainsNoKnownMojibakeFragments()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var allText = string.Join(
            Environment.NewLine,
            Directory.GetFiles(zhsRoot, "*.json", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));

        var fragments = new[]
        {
            "\uFFFD",
            "\u6D93",
            "\u9470",
            "\u7487",
            "\u941C",
            "\u940F",
            "\u95BB",
            "\u5A11",
            "\u934B",
            "\u5A75",
            "\u951F",
            "\u59AB",
            "\u951B",
            "\u947E",
            "\u93B5",
            "\u95B2",
            "\u7039",
            "\u7EC1",
            "\u93C0",
            "\u7481",
            "\u934A",
            "\u6769",
            "\u9410",
            "\u5BEE\u509D\u58CA",
            "\u9417",
            "\u93B0"
        };
        var matches = fragments
            .Where(fragment => allText.Contains(fragment, StringComparison.Ordinal))
            .ToArray();

        Assert.True(matches.Length == 0, "Found mojibake fragments in active zhs localization: " + string.Join(", ", matches));
    }

    [Fact]
    public void CurrentFacingDocsStillFailOnFalseReleaseClaims()
    {
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        var projectState = ReadRepoText("PROJECT_STATE.md");

        Assert.DoesNotMatch(@"(?i)\b(private beta|release)\s+(?:is\s+)?ready\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bready\s+for\s+(?:private beta|release)\b", currentDocs);
        Assert.Contains("- [x] BaseLib appears in Mod Settings.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Spire Plus appears in a refreshed Mod Settings UI screenshot after the display-name refresh package is installed.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Save/load-sensitive behavior is tested.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Disable-mod gameplay behavior is tested in a run.", currentDocs, StringComparison.Ordinal);

        Assert.Contains("Fresh loader smoke for the current beta.56 package hash is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Earlier page-level Mod Settings evidence predates the display-name refresh", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", currentDocs, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("Latest pushed cleanup/package evidence baseline", projectState, StringComparison.Ordinal);
        Assert.Contains("current beta.56 RootDeck combat lifecycle split package sync", projectState, StringComparison.Ordinal);
        Assert.Contains("git log -1 --oneline --decorate", projectState, StringComparison.Ordinal);
        Assert.Contains("a2183ee", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("5be5c51", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Refresh beta35 package guards", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("git diff --check", projectState, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RootSightCurrentDocsDescribeSelectableMapPreview()
    {
        var rootSightDocs = ReadCurrentFacingDocs(
            "docs/issues/urda.md",
            "docs/features/ancient-expansion-urda/implementation-plan.md",
            "docs/features/ancient-expansion-urda/manual-test-checklist.md",
            "docs/features/ancient-expansion-urda/source-design.md",
            "docs/features/ancient-expansion-urda/work-log.md",
            "docs/features/ancient-expansion-v2.2/implementation-plan.md",
            "docs/features/ancient-expansion-v2.2/manual-test-checklist.md",
            "docs/mod-changelog.md");

        Assert.DoesNotContain("Root-Sight has no map button", rootSightDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("no source-safe map button", rootSightDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("automatically marks reachable non-Boss", rootSightDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("auto-marks non-Boss nodes instead of", rootSightDocs, StringComparison.Ordinal);
        Assert.Contains("clicking the Root Eyes relic", rootSightDocs, StringComparison.Ordinal);
        Assert.Contains("Monster, Unknown, or Elite", rootSightDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void DocsRootHasNoPromptOrAddendumFilesAndArchivedPromptsHaveHistoricalMarkers()
    {
        var docsRoot = RepoPath("docs");
        var rootMarkdown = Directory
            .GetFiles(docsRoot, "*.md", SearchOption.TopDirectoryOnly)
            .Select(path => Path.GetFileName(path))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!.ToLowerInvariant())
            .ToArray();

        Assert.DoesNotContain(rootMarkdown, name => name.Contains("prompt") && !name.Contains("codex"));
        Assert.DoesNotContain(rootMarkdown, name => name.Contains("addendum"));

        var archivedPrompts = Directory.GetFiles(RepoPath("docs", "archive", "prompts"), "*.md", SearchOption.AllDirectories);
        Assert.NotEmpty(archivedPrompts);
        foreach (var prompt in archivedPrompts)
        {
            Assert.Contains("Historical archive.", ReadRepoText(prompt.Split(Path.DirectorySeparatorChar)));
        }
    }

    private static string CurrentDocsWithoutWorkLogs()
    {
        return string.Join(Environment.NewLine, CurrentFacingDocs.Select(path => ReadRepoText(path.Split('/'))));
    }

    private static void AssertNoCurrentFacing22FieldSmokePassClaims(string currentDocs)
    {
        var staleLines = currentDocs
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Where(line =>
            {
                var lower = line.ToLowerInvariant();
                var mentions22FieldSmoke = lower.Contains("found 22 savedspirefields", StringComparison.Ordinal);
                var soundsCurrent = lower.Contains("current normal steam", StringComparison.Ordinal)
                    || lower.Contains("current-package", StringComparison.Ordinal)
                    || lower.Contains("current package", StringComparison.Ordinal)
                    || lower.Contains("pass for loader", StringComparison.Ordinal)
                    || lower.Contains("loader pass", StringComparison.Ordinal);
                var marksHistoricalBoundary = lower.Contains("historical", StringComparison.Ordinal)
                    || lower.Contains("previous", StringComparison.Ordinal)
                    || lower.Contains("earlier", StringComparison.Ordinal)
                    || lower.Contains("superseded", StringComparison.Ordinal)
                    || lower.Contains("fresh loader", StringComparison.Ordinal)
                    || lower.Contains("loader parity remains pending", StringComparison.Ordinal)
                    || lower.Contains("fresh live loader", StringComparison.Ordinal)
                    || lower.Contains("not refreshed", StringComparison.Ordinal);

                return mentions22FieldSmoke && soundsCurrent && !marksHistoricalBoundary;
            })
            .ToArray();

        Assert.True(
            staleLines.Length == 0,
            "Found current-facing docs that describe the 22-field loader smoke as a current loader pass without a historical/pending boundary: "
            + string.Join(" || ", staleLines));
    }

    private static void AssertEvidenceLabelOnlyReferencedAsInvalid(string source, string label)
    {
        var matchingLines = source
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n')
            .Where(line => line.Contains(label, StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(matchingLines);
        foreach (var line in matchingLines)
        {
            Assert.Matches(
                @"(?i)\b(?:invalid|not counted|do not satisfy|does not satisfy|not gameplay evidence|loader health only|covered by another foreground app|stayed on the main menu|wrong surface)\b",
                line);
        }
    }

}
