using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseSafetyExpandedGuardTests
{
    [Fact]
    public void ActiveTrackedTextDoesNotExposeRemovedFrameworkNames()
    {
        var removedFramework = new string(new[] { (char)66, (char)97, (char)115, (char)101, (char)76, (char)105, (char)98 });
        var removedSavedFieldApi = "Saved" + "Spire" + "Field";
        var blockedTerms = new[]
        {
            removedFramework,
            "Alchyr.Sts2." + removedFramework,
            "STS2-" + removedFramework,
            removedSavedFieldApi,
            removedSavedFieldApi + "s"
        };
        var textExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cs",
            ".css",
            ".csv",
            ".html",
            ".js",
            ".json",
            ".md",
            ".ps1",
            ".tsv",
            ".txt",
            ".xml",
            ".yaml",
            ".yml"
        };
        var skippedPrefixes = new[]
        {
            ".godot/",
            ".git/",
            ".tools/",
            "bin/",
            "obj/",
            "output/playwright/",
            "publish/",
            "source code/",
            "tests/EZMicroBalance.Tests/TestResults/"
        };

        var offenders = Directory.GetFiles(Root, "*", SearchOption.AllDirectories)
            .Select(path => new { FullPath = path, RelativePath = ToRepoRelativePath(path) })
            .Where(file => !skippedPrefixes.Any(prefix => file.RelativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .Where(file => textExtensions.Contains(Path.GetExtension(file.RelativePath)))
            .SelectMany(file =>
            {
                var text = File.ReadAllText(file.FullPath, Encoding.UTF8);
                return blockedTerms
                    .Where(term => text.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .Select(term => $"{file.RelativePath}: {term}");
            })
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Active text files must route developers to RitsuLib-only docs and must not expose removed framework/API names: "
            + string.Join(" | ", offenders));
    }

    [Fact]
    public void BootstrapAndActiveAncientWorkLogStayCurrentAndReadable()
    {
        var bootstrap = ReadRepoText("scripts", "bootstrap-windows.ps1");
        var workLog = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md");

        AssertSourceContains(
            bootstrap,
            "Spire Plus Windows bootstrap",
            "Install STS2-RitsuLib v0.4.34 or newer under <GameRoot>\\mods\\STS2-RitsuLib before game verification.",
            "STS2-RitsuLib and Spire Plus appear and are enabled.");
        Assert.DoesNotContain("EzDailyContent Windows bootstrap", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain("previous dependency framework v3.1.0", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain("previous dependency framework plus EzDailyContent appear", bootstrap, StringComparison.Ordinal);

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
            "sFound 22 PreviousSavedStatess"
        })
        {
            Assert.DoesNotContain(corruptedMarker, workLog, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void SavedAttachedStatesAcrossActiveSourceAreUniqueCoveredAndSmokeDocumented()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var ancientFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var ascensionFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var savedStateRegistration = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibSavedStateRegistration.cs");
        var sourceWithoutDefinitions = string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith("AncientSavedStateFields.cs", StringComparison.Ordinal) &&
                               !path.EndsWith("AscensionSavedStateFields.cs", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));

        var fields = Regex.Matches(
                allSource,
                @"SavedAttachedState<(?<types>[^>]+)>\s+(?<name>[A-Za-z0-9_]+)\s*=\s*\r?\n\s*new\(""(?<key>EZMicroBalance[^""]+)""",
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
        AssertSourceContains(
            ancientFields,
            "public static void EnsureRegistered()",
            "RitsuLibSavedStateRegistration.EnsureRegistered(typeof(AncientSavedStateFields));");
        AssertSourceContains(
            ascensionFields,
            "public static void EnsureRegistered()",
            "RitsuLibSavedStateRegistration.EnsureRegistered(typeof(AscensionSavedStateFields));");
        AssertSourceContains(
            savedStateRegistration,
            "RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);",
            "BindingFlags.Public | BindingFlags.Static",
            "GetGenericTypeDefinition() == typeof(SavedAttachedState<,>)",
            "did not initialize its SavedAttachedState");
        AssertSourceContains(
            mainFile,
            "AncientSavedStateFields.EnsureRegistered();",
            "AscensionSavedStateFields.EnsureRegistered();",
            "RitsuLib saved-state fields registered.");

        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");
        Assert.Contains("Current source defines 30 SavedAttachedState fields", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", currentDocs, StringComparison.Ordinal);
        Assert.Contains("`Found 22 previous saved-state registrations`", currentDocs, StringComparison.Ordinal);
        Assert.Contains("fresh-current-package-loader-smoke", currentDocs, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus error signatures for technical id `EZMicroBalance`", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Historical 22-field loader evidence", devEnvironment, StringComparison.Ordinal);
        Assert.Contains("Historical beta.19 loader evidence:", devEnvironment, StringComparison.Ordinal);
        Assert.Contains("Current source defines 30 SavedAttachedState fields", devEnvironment, StringComparison.Ordinal);
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
        Assert.DoesNotContain("current source defines 22 previous saved-state registrations, while", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Found 9 previous saved-state registrations", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
        Assert.DoesNotContain("reported 7 previous saved-state registrations", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
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
        Assert.Contains("Current source defines 30 SavedAttachedState fields", audit, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", audit, StringComparison.Ordinal);
        Assert.Contains("historical log records", audit, StringComparison.Ordinal);
        Assert.Contains("`Found 22 previous saved-state registrations`", audit, StringComparison.Ordinal);
        Assert.Contains("Beta.19 loader parity is covered", audit, StringComparison.Ordinal);
        Assert.Contains("reports `v0.1.0-private-beta.19`", audit, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus error signatures for technical id `EZMicroBalance`", audit, StringComparison.Ordinal);
        Assert.Contains("beta.19 normal Steam-client startup/log verification reports `Found 30 previous saved-state registrations`", audit, StringComparison.Ordinal);
        Assert.Contains("historical beta.19 loader", audit, StringComparison.Ordinal);
        Assert.Contains("Current beta.123 clicked Ancient UI smoke is `.tools/runtime-evidence/monkey-stability-20260622-235746/`", audit, StringComparison.Ordinal);
        Assert.Contains("Two-client multiplayer matrix is pending", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveSessionHelperStaysRestoreSafeAndDocsKeepLoaderEvidenceSeparateFromGameplay()
    {
        var helper = ReadRepoText("scripts", "spire-plus-live-session.ps1");
        var windowPreflight = ReadRepoText("scripts", "check-spire-window-preflight.ps1");
        var windowCapture = ReadRepoText("scripts", "capture-spire-window.ps1");
        var consoleCommand = ReadRepoText("scripts", "send-spire-dev-console-command.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        AssertSourceContains(
            helper,
            "[ValidateSet('Prepare', 'Restore')]",
            "[switch]$MoveOtherMods",
            "[switch]$MoveCurrentRuns",
            "[switch]$PrepareDefaultSettings",
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
            "$settings.mod_settings.mods_enabled = $true",
            "default.settings.save.before",
            "DefaultSettingsPath = $defaultSettingsPath",
            "PrepareDefaultSettings = [bool]$PrepareDefaultSettings",
            "Set-SpirePlusSettings -SettingsPath $defaultSettingsPath",
            "live-spire-plus-disabled-session",
            "$defaultAllowedModIds = @('STS2-RitsuLib', 'EZMicroBalance')",
            "id = 'STS2-RitsuLib'",
            "DisableSpirePlus requires -MoveOtherMods",
            "$allowedModIds = if ($DisableSpirePlus) { @('STS2-RitsuLib') } else { $defaultAllowedModIds }",
            "AllowedModIds = @($allowedModIds)",
            "DisableSpirePlus = [bool]$DisableSpirePlus",
            "Start-Process -FilePath $SteamExe",
            "$steamAppId = '2868840'",
            "$launchArgumentList = @('-applaunch', $steamAppId)",
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
        AssertSourceContains(
            windowCapture,
            "SetProcessDpiAwarenessContext",
            "$dpiAwarenessPerMonitorV2 = [IntPtr](-4)",
            "per-monitor DPI-aware",
            "CopyFromScreen reads physical pixels",
            "Wait-SpireForeground",
            "SetForegroundWindow",
            "kernel32.dll",
            "AttachThreadInput",
            "BringWindowToTop",
            "ForegroundProcessId",
            "CopyFromScreen");
        AssertSourceContains(
            consoleCommand,
            "GetForegroundWindow",
            "GetWindowThreadProcessId",
            "Wait-SpireForeground",
            "SetForegroundWindow",
            "kernel32.dll",
            "AttachThreadInput",
            "BringWindowToTop",
            "Clipboard]::SetText($Command)",
            "SendWait(\"^v\")",
            "{ENTER}",
            "UsedClipboardPaste",
            "ForegroundReady",
            "could not become the foreground window");
        Assert.Contains("spire-plus-live-session.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("check-spire-window-preflight.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-DisableSpirePlus", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-PrepareDefaultSettings", scriptsReadme, StringComparison.Ordinal);
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
            "AscensionA20CourtyardProceedPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"ascension-a20-courtyard-proceed\"",
            "new ModPatchTarget(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))",
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
    public void CurrentFacingDocsStillFailOnFalseReleaseClaims()
    {
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        var projectState = ReadRepoText("PROJECT_STATE.md");

        Assert.DoesNotMatch(@"(?i)\b(private beta|release)\s+(?:is\s+)?ready\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bready\s+for\s+(?:private beta|release)\b", currentDocs);
        Assert.Contains("- [x] STS2-RitsuLib appears in Mod Settings for the beta.99 RitsuLib-only package; this is previous-package context after beta.123 and should be recaptured if the settings row becomes release-blocking.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Historical refreshed Mod Settings UI list screenshot shows `Spire Plus` after the display-name refresh package is installed.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Previous beta.99 Mod Settings list plus Spire Plus config page screenshots are captured under release-evidence row `mod-settings-current-display`.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Save/load-sensitive behavior is tested.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Disable-mod gameplay behavior is tested in a run.", currentDocs, StringComparison.Ordinal);

        Assert.Contains("Previous beta.96 RitsuLib-only Off proof has been recaptured", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Previous beta.93 AdditiveBatch1 registration proof has been recaptured", currentDocs, StringComparison.Ordinal);
        Assert.Contains("loader/registration evidence, not gameplay proof", currentDocs, StringComparison.Ordinal);
        Assert.Contains("mod-settings-beta99-ritsulib-click-20260621-223210", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Previous beta.99 RitsuLib Mod Settings UI proof is captured under `.tools\\runtime-evidence\\mod-settings-beta99-ritsulib-click-20260621-223210`", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", currentDocs, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("Latest pushed migration baseline must be read directly from `git log -1 --oneline --decorate`", projectState, StringComparison.Ordinal);
        Assert.Contains("Active M5 Revision S truth", projectState, StringComparison.Ordinal);
        Assert.Contains("Latest package target is beta.128", projectState, StringComparison.Ordinal);
        Assert.Contains("build, publish, package refresh, installed-package parity, runtime preflight, source-workspace validation, and local RitsuLib runtime install are current", projectState, StringComparison.Ordinal);
        Assert.Contains("Current beta.123 clicked Ancient UI smoke proof is", projectState, StringComparison.Ordinal);
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
                var mentions22FieldSmoke = lower.Contains("found 22 previous saved-state registrations", StringComparison.Ordinal);
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
