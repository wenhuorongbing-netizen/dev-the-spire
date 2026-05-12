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
        "docs/test-plan.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    private static readonly string[] KnownCurrentHashDocs =
    [
        "docs/dev-environment.md",
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
        var packageZip = RepoPath("publish", $"EZMicroBalance-{ManifestVersion()}.zip");

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
        using var archive = ZipFile.OpenRead(RepoPath("publish", $"EZMicroBalance-{ManifestVersion()}.zip"));
        var zippedEntries = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));

        Assert.Equal(installedEntries.OrderBy(entry => entry, StringComparer.Ordinal), zippedEntries.OrderBy(entry => entry, StringComparer.Ordinal));

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".json", StringComparison.Ordinal)))
        {
            Assert.Contains(resource, installedEntries);
            Assert.Contains(resource, zippedEntries);
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

        Assert.Equal(14, fields.Length);
        Assert.Equal(fields.Length, fields.Select(field => field.Key).Distinct(StringComparer.Ordinal).Count());
        Assert.All(fields, field => Assert.StartsWith("EZMicroBalance", field.Key, StringComparison.Ordinal));
        Assert.All(fields, field => Assert.Contains(field.Name, sourceWithoutDefinitions, StringComparison.Ordinal));
        Assert.Contains(fields, field => field.Types == "PrismaticGem, int");
        Assert.Contains(fields, field => field.Types == "PaelsTooth, int");
        Assert.Contains(fields, field => field.Types == "CardModel, bool");
        Assert.Contains(fields, field => field.Types == "Player, bool");
        Assert.Contains(fields, field => field.Types == "Player, int");
        Assert.Contains(fields, field => field.Types == "Player, string");
        Assert.Contains(fields, field => field.Types == "RootBud, bool");
        Assert.Contains(fields, field => field.Types == "RootBud, int");
        Assert.Contains(fields, field => field.Types == "RootFamilyCard, bool");

        var currentDocs = ReadCurrentFacingDocs();
        Assert.Contains("current source defines 14 SavedSpireFields", currentDocs, StringComparison.Ordinal);
        Assert.Contains("previous smoke reported `Found 13 SavedSpireFields` and is stale", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Found 9 SavedSpireFields", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
        Assert.DoesNotContain("reported 7 SavedSpireFields", CurrentDocsWithoutWorkLogs(), StringComparison.Ordinal);
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
        var packageName = $"EZMicroBalance-{version}";
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = RepoPath("publish", $"{packageName}.zip");

        var dllHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.dll"));
        var manifestHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.json"));
        var pckHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.pck"));
        var zipHash = Sha256(zipPath);
        var artHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));
        var knownCurrentHashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            dllHash,
            manifestHash,
            pckHash,
            zipHash,
            artHash
        };

        Assert.Equal(dllHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.dll")));
        Assert.Equal(dllHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.dll")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.json")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.json")));
        Assert.Equal(pckHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.pck")));
        Assert.Equal(pckHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.pck")));

        using (var archive = ZipFile.OpenRead(zipPath))
        {
            Assert.Equal(dllHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.dll")));
            Assert.Equal(manifestHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.json")));
            Assert.Equal(pckHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck")));
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
            using var document = JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8));
            foreach (var (key, value) in JsonStringValues(document.RootElement))
            {
                var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
                visibleValue = Regex.Replace(visibleValue, @"\[(?:/)?[A-Za-z][^\]]*\]", string.Empty, RegexOptions.CultureInvariant);
                foreach (Match match in Regex.Matches(visibleValue, @"[A-Za-z][A-Za-z0-9_-]*", RegexOptions.CultureInvariant))
                {
                    if (match.Value is "I" or "II" or "III")
                    {
                        continue;
                    }

                    failures.Add($"{ToRepoRelativePath(file)}:{key} contains raw ASCII word `{match.Value}` in `{value}`");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void CurrentFacingDocsStillFailOnFalseReleaseClaims()
    {
        var currentDocs = ReadCurrentFacingDocs();
        var projectState = ReadRepoText("PROJECT_STATE.md");

        Assert.DoesNotMatch(@"(?i)\b(private beta|release)\s+(?:is\s+)?ready\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bready\s+for\s+(?:private beta|release)\b", currentDocs);
        Assert.Contains("- [x] BaseLib appears in Mod Settings.", currentDocs, StringComparison.Ordinal);
        Assert.Contains("- [x] EZ Micro Balance appears in Mod Settings.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Save/load-sensitive behavior is tested.", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Disable-mod gameplay behavior is tested in a run.", currentDocs, StringComparison.Ordinal);

        Assert.Contains("RC1 normal Steam-client Mod Settings verification passed after adding the no-op EZ Micro Balance BaseLib config page", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("c8bcaa9", projectState, StringComparison.OrdinalIgnoreCase);
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
            "[INFO] Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance).",
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
            summary.SavedSpireFieldCount == 13 &&
            summary.EzMicroBalanceErrorLines.Length == 0;
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
                InitializedEzMicroBalance: lines.Any(line => line.Contains("Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance)", StringComparison.Ordinal)),
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
            (Path.GetExtension(relativePath) is ".json" or ".png" or ".txt");
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
