using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseSafetyExpandedGuardTests
{
    private static readonly string[] CurrentFacingDocs =
    [
        "README.md",
        "docs/dev-environment.md",
        "docs/private-beta-verification-handoff.md",
        "docs/private-beta-release-completion-audit.md",
        "docs/test-plan.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    private static readonly string[] KnownCurrentHashDocs =
    [
        "docs/dev-environment.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md"
    ];

    private static readonly string[] CurrentReleaseHashClaimLineMarkers =
    [
        "zip",
        "package",
        "dll",
        "manifest",
        "json",
        "pck",
        "installed",
        "staging",
        "versioned",
        "current"
    ];

    [ReleaseArtifactFact]
    public void ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage()
    {
        var activeCover = RepoPath("EZMicroBalance", "mod_image.png");
        var auditedCover = RepoPath("publish", "EZMicroBalance-cover-source.png");
        var inactiveRootCover = RepoPath("EZMicroBalance", "mod_real.png");
        var inactiveRootCoverImport = RepoPath("EZMicroBalance", "mod_real.png.import");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        var packageZip = RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip");

        Assert.True(File.Exists(activeCover), $"Missing active cover art: {activeCover}");
        Assert.True(File.Exists(auditedCover), $"Missing audited cover source copy: {auditedCover}");
        Assert.Equal(Sha256(activeCover), Sha256(auditedCover));

        Assert.False(File.Exists(inactiveRootCover), "Root-level EZMicroBalance/mod_real.png is banned from the active mod resource tree.");
        Assert.False(File.Exists(inactiveRootCoverImport), "Root-level EZMicroBalance/mod_real.png.import is banned from the active mod resource tree.");

        var exported = ParseExportFiles(exportPreset);
        Assert.Contains("res://EZMicroBalance/mod_image.png", exported);
        Assert.DoesNotContain(exported, path => path.Contains("mod_real", StringComparison.OrdinalIgnoreCase));

        var pckEntries = ReadPckDirectory(installedPck);
        Assert.Contains("EZMicroBalance/mod_image.png", pckEntries);
        Assert.Contains("EZMicroBalance/mod_image.png.import", pckEntries);
        Assert.DoesNotContain(pckEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));

        using var archive = ZipFile.OpenRead(packageZip);
        var zipEntries = archive.Entries.Select(entry => entry.FullName.Replace('\\', '/')).ToArray();
        Assert.DoesNotContain(zipEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(zipEntries, entry => entry.EndsWith("mod_image.png", StringComparison.OrdinalIgnoreCase));

        var zippedPckEntries = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));
        Assert.Contains("EZMicroBalance/mod_image.png", zippedPckEntries);
        Assert.Contains("EZMicroBalance/mod_image.png.import", zippedPckEntries);
        Assert.DoesNotContain(zippedPckEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));
    }

    [ReleaseArtifactFact]
    public void ExportedResourcesInstalledPckAndPackagePckStayInParity()
    {
        var exportedResources = ParseExportFiles(ReadRepoText("export_presets.cfg"))
            .Select(path => path["res://".Length..])
            .Concat(
                Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories)
                    .Select(path => ToRepoRelativePath(path))
                    .Where(IsActiveExportResource))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        var installedEntries = ReadPckDirectory(installedPck);
        using var archive = ZipFile.OpenRead(RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip"));
        var zippedEntries = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));

        Assert.Equal(installedEntries.OrderBy(entry => entry, StringComparer.Ordinal), zippedEntries.OrderBy(entry => entry, StringComparer.Ordinal));

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".json", StringComparison.Ordinal)))
        {
            Assert.Contains(resource, installedEntries);
            Assert.Contains(resource, zippedEntries);
        }

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".tscn", StringComparison.Ordinal)))
        {
            Assert.True(
                installedEntries.Contains(resource) || installedEntries.Contains($"{resource}.remap"),
                $"Installed PCK is missing exported scene or remap: {resource}");
            Assert.True(
                zippedEntries.Contains(resource) || zippedEntries.Contains($"{resource}.remap"),
                $"Package PCK is missing exported scene or remap: {resource}");
        }

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".png", StringComparison.Ordinal)))
        {
            Assert.True(File.Exists(RepoPath(resource.Split('/'))), $"Export references missing PNG: {resource}");
            Assert.True(File.Exists(RepoPath((resource + ".import").Split('/'))), $"Exported PNG has no import metadata: {resource}");
            Assert.Contains(resource + ".import", installedEntries);
            Assert.Contains(resource + ".import", zippedEntries);
        }

        var activeLocalizationJson = Directory.GetFiles(RepoPath("EZMicroBalance", "localization"), "*.json", SearchOption.AllDirectories)
            .Select(path => ToRepoRelativePath(path))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var exportedLocalization = exportedResources
            .Where(path => path.StartsWith("EZMicroBalance/localization/", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(activeLocalizationJson.Length, exportedLocalization.Length);
        Assert.All(activeLocalizationJson, resource => Assert.Contains(resource, exportedLocalization));
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

        Assert.Equal(22, fields.Length);
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

        var currentDocs = ReadCurrentFacingDocs();
        Assert.Contains("current source defines 22 SavedSpireFields", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Found 22 SavedSpireFields", currentDocs, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus / `EZMicroBalance` error signatures", currentDocs, StringComparison.Ordinal);
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
            "current-package controlled smoke",
            "It is not private-beta release-ready");

        Assert.Contains("`EZMicroBalance`", audit, StringComparison.Ordinal);
        Assert.Contains("`Spire Plus`", audit, StringComparison.Ordinal);
        Assert.Contains("current source defines 22 SavedSpireFields", audit, StringComparison.Ordinal);
        Assert.Contains("current-package-smoke-20260514-015901", audit, StringComparison.Ordinal);
        Assert.Contains("Found 22 SavedSpireFields", audit, StringComparison.Ordinal);
        Assert.Contains("0 Spire Plus / `EZMicroBalance` error signatures", audit, StringComparison.Ordinal);
        Assert.Contains("Current normal Steam-client startup/log verification passed for the refreshed `Spire Plus` display name", audit, StringComparison.Ordinal);
        Assert.Contains("refreshed Mod Settings UI list capture now shows `Spire Plus`", audit, StringComparison.Ordinal);
        Assert.Contains("Two-client multiplayer matrix is pending", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveSessionHelperStaysRestoreSafeAndDocsKeepLoaderEvidenceSeparateFromGameplay()
    {
        var helper = ReadRepoText("scripts", "spire-plus-live-session.ps1");
        var windowPreflight = ReadRepoText("scripts", "check-spire-window-preflight.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");
        var currentDocs = ReadCurrentFacingDocs();

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
        Assert.Contains("This is a previous loader/helper validation pass, not live gameplay evidence.", currentDocs, StringComparison.Ordinal);
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
            "## Prompt-To-Artifact Checklist",
            "## Missing Or Weakly Verified Items",
            "## Conclusion",
            "151 passed / 18 skipped",
            "169 passed / 0 skipped",
            "current-package-smoke-20260514-015901",
            "urda-pck-resource-load-20260513-123345",
            "window-preflight-smoke-20260513-135402",
            "Full Ancient reward runtime matrix",
            "Disable-mod gameplay behavior in an actual run",
            "Natural A11 click-by-click traversal",
            "Two-client multiplayer/co-op matrix",
            "Not achieved.",
            "It is not private-beta release-ready");

        Assert.Contains("| Ancient reward gameplay |", audit, StringComparison.Ordinal);
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
    public void AscensionPrototypeMutationPathsStayBehindGatesAndCommandApis()
    {
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionMapService.cs");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20Patches.cs");
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "AscensionRewardService.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.cs");
        var forgeService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "ForgeTokenService.cs");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");

        AssertSourceContains(
            initializer,
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||",
            "AscensionFeatureGate.IsDiagnosticsEnabled",
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) ||");

        AssertSourceContains(
            mapService,
            "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&",
            "return map;",
            "if (AscensionFeatureGate.IsMapGeometryEnabled(runState))",
            "if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))",
            "if (!AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))",
            "if (!AscensionFeatureGate.IsBannerRoomEnabled(runState))",
            "var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);",
            "var dualKingBrandsEnabled = AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState);",
            "if (!bossSealsEnabled && !dualKingBrandsEnabled)",
            "if (bossSealsEnabled)",
            "if (!dualKingBrandsEnabled)",
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
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState)",
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
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState)",
            "new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player)",
            "metadata?.DeepBranch != DeepBranchNodeKind.EnhancedReward",
            "new RelicReward(RelicRarity.Uncommon, player)");

        AssertSourceContains(
            combatService,
            "AscensionFeatureGate.IsFiremarkedEliteEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBannerRoomEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(combatState.RunState)",
            "await CreatureCmd.GainBlock",
            "FindFiremarkHost(combatState)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>",
            "await PowerCmd.Apply<ArtifactPower>",
            "await PowerCmd.Apply<StrengthPower>",
            "var definition = metadata.BossSeal",
            "TrackInkReturnIfSlipperySpent",
            "TrackKnowledgeDemonEnemyMove",
            "BossSealId.AeonglassStrength",
            "FirstOrDefault(enemy => enemy.ModelId == AeonglassMonsterId)",
            "AeonglassStrengthAmount = 5m",
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

    [ReleaseArtifactFact]
    public void CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts()
    {
        var version = ManifestVersion();
        var packageName = $"SpirePlus-{version}";
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = RepoPath("publish", $"{packageName}.zip");

        var dllHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.dll"));
        var manifestHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.json"));
        var pckHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.pck"));
        var readmeHash = Sha256(Path.Combine(stagingDir, "README_INSTALL.txt"));
        var zipHash = Sha256(zipPath);
        var artHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));
        var knownCurrentHashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            dllHash,
            manifestHash,
            pckHash,
            readmeHash,
            zipHash,
            artHash
        };

        Assert.Equal(dllHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.dll")));
        Assert.Equal(dllHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.dll")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.json")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.json")));
        Assert.Equal(pckHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.pck")));
        Assert.Equal(pckHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.pck")));
        Assert.Equal(readmeHash, Sha256(Path.Combine(versionedDir, "README_INSTALL.txt")));

        using (var archive = ZipFile.OpenRead(zipPath))
        {
            Assert.Equal(dllHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.dll")));
            Assert.Equal(manifestHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.json")));
            Assert.Equal(pckHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck")));
            Assert.Equal(readmeHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/README_INSTALL.txt")));
        }

        var hashDocs = string.Join(Environment.NewLine, KnownCurrentHashDocs.Select(path => ReadRepoText(path.Split('/'))));
        Assert.Contains(dllHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(manifestHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(pckHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(zipHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(artHash, hashDocs, StringComparison.Ordinal);

        var documentedHashes = hashDocs
            .Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Where(line =>
                CurrentReleaseHashClaimLineMarkers.Any(marker => line.Contains(marker, StringComparison.OrdinalIgnoreCase)))
            .SelectMany(line => Regex.Matches(line, @"\b[A-Fa-f0-9]{64}\b").Cast<Match>().Select(match => match.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.All(documentedHashes, hash => Assert.Contains(hash, knownCurrentHashes));
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
                    key == "EZMICROBALANCE.mod_title")
                {
                    continue;
                }

                var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
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
            "妫卞僵",
            "寮傝壊",
            "鏀炬澗",
            "鏀朵笅",
            "鍖栦负",
            "鍋胯繕",
            "绁炲寲",
            "璁告効",
            "鎵ц糠",
            "杩呴",
            "鍥烘湁",
            "鎰氳",
            "姘告亽",
            "瀹濈煶",
            "鐏典綋",
            "闈為",
            "棣栭",
            "鍊哄姟",
            "娆犳",
            "淇濈暀",
            "铏氭棤",
            "娑堣",
            "鍔涢噺",
            "鑾峰緱",
            "鐐硅兘",
            "澶卞幓",
            "绗",
            "绋充綇",
            "涓涵",
            "闇€瑕佽嚦"
        };

        var matches = fragments
            .Where(fragment => allText.Contains(fragment, StringComparison.Ordinal))
            .ToArray();

        Assert.True(matches.Length == 0, "Found mojibake fragments in active zhs localization: " + string.Join(", ", matches));
    }

    [Fact]
    public void CurrentFacingDocsStillFailOnFalseReleaseClaims()
    {
        var currentDocs = ReadCurrentFacingDocs();
        var projectState = ReadRepoText("PROJECT_STATE.md");

        Assert.DoesNotMatch(@"(?i)\b(private beta|release)\s+(?:is\s+)?ready\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bready\s+for\s+(?:private beta|release)\b", currentDocs);
        Assert.Contains("- [x] BaseLib appears in Mod Settings.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Spire Plus / `EZMicroBalance` appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] Spire Plus appears in a refreshed Mod Settings UI screenshot after the display-name refresh package is installed.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Save/load-sensitive behavior is tested.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Disable-mod gameplay behavior is tested in a run.", currentDocs, StringComparison.Ordinal);

        Assert.Contains("Current normal Steam-client startup/log verification passed for the Spire Plus display-name package", currentDocs, StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", currentDocs, StringComparison.Ordinal);
        Assert.Contains("RC1 normal Steam-client Mod Settings UI verification remains historical evidence for the old EZ Micro Balance display name", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("a2183ee", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("git diff --check", projectState, StringComparison.OrdinalIgnoreCase);
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

    [Fact]
    public void SmokeLogParserDistinguishesEzMicroBalancePassFromUnrelatedManifestErrors()
    {
        var syntheticLog = string.Join(
            Environment.NewLine,
            "[ERROR] Mod manifest D:\\Steam\\mods\\OtherMod\\bad.json is missing the 'id' field! This is not allowed.",
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\BaseLib\\BaseLib.dll",
            "[INFO] Finished mod initialization for 'BaseLib' (BaseLib).",
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.dll",
            "[INFO] Loading Godot PCK D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.pck",
            "[INFO] Finished mod initialization for 'Spire Plus' (EZMicroBalance).",
            "[INFO] [BaseLib] Found 13 SavedSpireFields.",
            "[INFO] [Startup] Time to main menu: 12,648ms");

        var summary = SmokeLogParser.Parse(syntheticLog);

        Assert.True(summary.LoadedBaseLibDll);
        Assert.True(summary.InitializedBaseLib);
        Assert.True(summary.LoadedEzDll);
        Assert.True(summary.LoadedEzPck);
        Assert.True(summary.InitializedEzMicroBalance);
        Assert.True(summary.ReachedMainMenu);
        Assert.Equal(13, summary.SavedSpireFieldCount);
        Assert.Empty(summary.EzMicroBalanceErrorLines);
        Assert.Single(summary.UnrelatedManifestErrorLines);
    }

    [ReleaseArtifactFact]
    public void RecentRuntimeLogMustNotContainV105ApiDriftOrBaseLibDependencyFailures()
    {
        var logPath = CurrentGodotLogPath();
        var logsDir = Path.GetDirectoryName(logPath);
        Assert.NotNull(logsDir);
        if (!Directory.Exists(logsDir))
        {
            return;
        }

        var recentLog = Directory
            .GetFiles(logsDir, "godot*.log", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();

        if (recentLog == null)
        {
            return;
        }

        var logContent = ReadAllTextShared(recentLog);

        var forbiddenSignatures = new[]
        {
            // v0.105.0 API drift: Creature.get_ShowsInfiniteHp removed
            "Creature.get_ShowsInfiniteHp",
            // BaseLib HealthBarForecastPatch calls removed API
            "BaseLib.Patches.UI.HealthBarForecastPatch.RefreshForegroundOverlay",
            // DamageMeter calls removed API
            "DamageMeter.Scripts.CombatDataCollector.SnapshotEnemyHp",
            // BaseLib patch failures against v0.105.0
            "Undefined target method for patch method static System.Void BaseLib.Patches.Features",
        };

        var matches = new List<string>();
        foreach (var signature in forbiddenSignatures)
        {
            if (logContent.Contains(signature, StringComparison.Ordinal))
            {
                matches.Add(signature);
            }
        }

        Assert.True(
            matches.Count == 0,
            $"Recent runtime log {Path.GetFileName(recentLog)} contains forbidden v0.105.0 API drift or dependency failure signatures: {string.Join("; ", matches)}. " +
            "The test environment may have incompatible mods (DamageMeter, non-EZMB mods) or an incompatible BaseLib version. " +
            "Disable all mods except BaseLib + EZMicroBalance and retest. See ISSUE-2026-05-08-V105-BASELIB-CREATURE-SHOWSINFINITEHP-API-DRIFT in docs/issues.md.");
    }

    [ReleaseArtifactFact]
    public void RecentSmokeLogSupportsControlledSmokeClaims()
    {
        var logPath = CurrentGodotLogPath();
        var logsDir = Path.GetDirectoryName(logPath);
        Assert.NotNull(logsDir);
        Assert.True(Directory.Exists(logsDir), $"Missing log directory: {logsDir}");

        var candidates = Directory
            .GetFiles(logsDir, "godot*.log", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToArray();
        Assert.NotEmpty(candidates);

        var passingLogs = candidates
            .Select(path => (path, summary: SmokeLogParser.Parse(ReadAllTextShared(path))))
            .Where(candidate => IsControlledSmokePass(candidate.summary))
            .ToArray();

        if (passingLogs.Length == 0)
        {
            var currentDocs = CurrentDocsWithoutWorkLogs();
            Assert.Contains("refreshed runtime smoke remains pending", currentDocs, StringComparison.Ordinal);
            Assert.DoesNotContain("current package smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("refreshed runtime smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
            return;
        }

        if (File.Exists(logPath))
        {
            var currentSummary = SmokeLogParser.Parse(ReadAllTextShared(logPath));
            Assert.Empty(currentSummary.EzMicroBalanceErrorLines);
        }
    }

    private static bool IsControlledSmokePass(SmokeLogSummary summary)
    {
        return summary.LoadedBaseLibDll &&
            summary.InitializedBaseLib &&
            summary.LoadedEzDll &&
            summary.LoadedEzPck &&
            summary.InitializedEzMicroBalance &&
            summary.ReachedMainMenu &&
            summary.SavedSpireFieldCount == 22 &&
            summary.EzMicroBalanceErrorLines.Length == 0;
    }

    [ReleaseArtifactFact]
    public void DisabledSpirePlusPlugOffEvidenceSupportsDocs()
    {
        var evidenceDir = RepoPath(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020");
        Assert.True(Directory.Exists(evidenceDir), $"Missing plug-off evidence directory: {evidenceDir}");

        using var summary = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "disabled-startup-summary.json"));
        var root = summary.RootElement;
        Assert.True(root.GetProperty("DisableSpirePlus").GetBoolean());
        Assert.True(root.GetProperty("MovedEzmb").GetBoolean());
        Assert.True(root.GetProperty("ReachedMainMenu").GetBoolean());
        Assert.True(root.GetProperty("ContainsBaseLibInitialization").GetBoolean());
        Assert.False(root.GetProperty("ContainsSpirePlusInitialization").GetBoolean());
        Assert.False(root.GetProperty("ContainsEzmbError").GetBoolean());
        Assert.Equal(["BaseLib"], root.GetProperty("AllowedModIds").EnumerateArray().Select(value => value.GetString() ?? string.Empty).ToArray());
        Assert.Contains(root.GetProperty("LoadedLines").EnumerateArray().Select(value => value.GetString() ?? string.Empty), line => line.Contains("Loaded 1 mods (1 total)", StringComparison.Ordinal));

        var log = ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "godot.log");
        Assert.Contains("Loaded 1 mods (1 total)", log, StringComparison.Ordinal);
        Assert.Contains("Finished mod initialization for 'BaseLib' (BaseLib)", log, StringComparison.Ordinal);
        Assert.DoesNotContain("Finished mod initialization for 'Spire Plus' (EZMicroBalance)", log, StringComparison.Ordinal);
        Assert.DoesNotContain("Registered config for mod EZMicroBalance", log, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMicroBalance.dll", log, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMicroBalance.pck", log, StringComparison.Ordinal);

        using var audit = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "godot-log-audit.json"));
        Assert.True(audit.RootElement.GetProperty("Clean").GetBoolean());
        Assert.All(audit.RootElement.GetProperty("SignatureHits").EnumerateArray(), hit => Assert.Equal(0, hit.GetProperty("Count").GetInt32()));

        using var restore = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "restore-output.json"));
        Assert.Equal(25, restore.RootElement.GetProperty("RestoredModCount").GetInt32());
        Assert.Equal(1, restore.RootElement.GetProperty("RestoredCurrentRunCount").GetInt32());

        var currentDocs = ReadCurrentFacingDocs();
        Assert.Contains("live-spire-plus-disabled-session-20260513-143020", currentDocs, StringComparison.Ordinal);
        Assert.Contains("settings-only disabled attempt", currentDocs, StringComparison.Ordinal);
        Assert.Contains("This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.", currentDocs, StringComparison.Ordinal);
    }

    private sealed record SmokeLogSummary(
        bool LoadedBaseLibDll,
        bool InitializedBaseLib,
        bool LoadedEzDll,
        bool LoadedEzPck,
        bool InitializedEzMicroBalance,
        bool ReachedMainMenu,
        int? SavedSpireFieldCount,
        string[] EzMicroBalanceErrorLines,
        string[] UnrelatedManifestErrorLines);

    private static class SmokeLogParser
    {
        public static SmokeLogSummary Parse(string log)
        {
            var lines = log.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var savedFieldCount = Regex.Match(log, @"Found (?<count>\d+) SavedSpireFields\.");

            return new SmokeLogSummary(
                LoadedBaseLibDll: lines.Any(line => line.Contains("Loading assembly DLL", StringComparison.Ordinal) &&
                                                    line.Contains("BaseLib.dll", StringComparison.Ordinal)),
                InitializedBaseLib: lines.Any(line => line.Contains("Finished mod initialization for 'BaseLib' (BaseLib)", StringComparison.Ordinal)),
                LoadedEzDll: lines.Any(line => line.Contains("Loading assembly DLL", StringComparison.Ordinal) &&
                                               line.Contains("EZMicroBalance.dll", StringComparison.Ordinal)),
                LoadedEzPck: lines.Any(line => line.Contains("Loading Godot PCK", StringComparison.Ordinal) &&
                                               line.Contains("EZMicroBalance.pck", StringComparison.Ordinal)),
                InitializedEzMicroBalance: lines.Any(line =>
                    line.Contains("Finished mod initialization for 'Spire Plus' (EZMicroBalance)", StringComparison.Ordinal) ||
                    line.Contains("Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance)", StringComparison.Ordinal)),
                ReachedMainMenu: lines.Any(line => line.Contains("Time to main menu", StringComparison.Ordinal)),
                SavedSpireFieldCount: savedFieldCount.Success ? int.Parse(savedFieldCount.Groups["count"].Value) : null,
                EzMicroBalanceErrorLines: lines
                    .Where(line => line.Contains("EZMicroBalance", StringComparison.Ordinal) &&
                                   Regex.IsMatch(line, @"\b(error|exception|failed|missingmethodexception)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    .ToArray(),
                UnrelatedManifestErrorLines: lines
                    .Where(line => line.Contains("Mod manifest", StringComparison.Ordinal) &&
                                   line.Contains("[ERROR]", StringComparison.Ordinal) &&
                                   !line.Contains("EZMicroBalance", StringComparison.Ordinal) &&
                                   !line.Contains("BaseLib", StringComparison.Ordinal))
                    .ToArray());
        }
    }

    private static string CurrentDocsWithoutWorkLogs()
    {
        return string.Join(Environment.NewLine, CurrentFacingDocs.Select(path => ReadRepoText(path.Split('/'))));
    }

    private static string ReadCurrentFacingDocs()
    {
        return string.Join(Environment.NewLine, CurrentFacingDocs.Select(path => ReadRepoText(path.Split('/'))));
    }

    private static string[] ParseExportFiles(string exportPreset)
    {
        var match = Regex.Match(exportPreset, @"export_files=PackedStringArray\((?<files>[^)]*)\)");
        Assert.True(match.Success, "Could not find export_files in export_presets.cfg.");

        return Regex.Matches(match.Groups["files"].Value, @"""(?<path>[^""]+)""")
            .Cast<Match>()
            .Select(match => match.Groups["path"].Value)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsActiveExportResource(string relativePath)
    {
        return IsActiveReleaseResource(relativePath) &&
            (Path.GetExtension(relativePath) is ".json" or ".png" or ".txt" or ".tscn");
    }

    private static bool IsActiveReleaseResource(string relativePath)
    {
        return relativePath.StartsWith("EZMicroBalance/", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png.import", StringComparison.Ordinal);
    }

    private static IReadOnlyList<string> ReadPckDirectory(string path)
    {
        Assert.True(File.Exists(path), $"Missing PCK: {path}");
        return ReadPckDirectory(File.ReadAllBytes(path));
    }

    private static IReadOnlyList<string> ReadPckDirectory(byte[] bytes)
    {
        var directoryOffset = (int)BitConverter.ToUInt64(bytes, 0x20);
        var count = (int)BitConverter.ToUInt32(bytes, directoryOffset);
        var offset = directoryOffset + 4;
        var entries = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var length = (int)BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            entries.Add(Encoding.UTF8.GetString(bytes, offset, length).TrimEnd('\0'));
            offset += length;
            offset += 8 + 8 + 16 + 4;
        }

        return entries;
    }

    private static byte[] ReadZipBytes(ZipArchive archive, string entryName)
    {
        var entry = archive.Entries.FirstOrDefault(candidate =>
            candidate.FullName.Replace('\\', '/').Equals(entryName, StringComparison.Ordinal));
        Assert.NotNull(entry);

        using var stream = entry.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static IEnumerable<(string key, string value)> JsonStringValues(JsonElement element, string keyPrefix = "")
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(keyPrefix)
                    ? property.Name
                    : $"{keyPrefix}.{property.Name}";

                foreach (var value in JsonStringValues(property.Value, key))
                {
                    yield return value;
                }
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                foreach (var value in JsonStringValues(item, $"{keyPrefix}[{index}]"))
                {
                    yield return value;
                }

                index++;
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            yield return (keyPrefix, element.GetString() ?? string.Empty);
        }
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
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

    private static string ManifestVersion()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        return document.RootElement.GetProperty("version").GetString() ?? throw new InvalidOperationException("Missing manifest version.");
    }

    private static string ReadSourceTree(params string[] parts)
    {
        var root = RepoPath(parts);
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    private static string Sha256(string path)
    {
        Assert.True(File.Exists(path), $"Missing file to hash: {path}");
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static string Sha256(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static string CurrentGodotLogPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SlayTheSpire2",
            "logs",
            "godot.log");
    }

    private static string ReadAllTextShared(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(FindRepoRoot(), path).Replace('\\', '/');
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string GamePath(params string[] parts)
    {
        var root = Environment.GetEnvironmentVariable("STS2_PATH");
        if (string.IsNullOrWhiteSpace(root))
        {
            root = @"D:\Steam\steamapps\common\Slay the Spire 2";
        }

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
