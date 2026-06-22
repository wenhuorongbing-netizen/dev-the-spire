using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionFeatureGuardTests
{
    [Fact]
    public void AscensionSelectionExtendsOriginalStandardLobbiesWithoutGlobalProgressGetterPatch()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ascension");

        AssertSourceContains(
            source,
            "DebugLevelEnvironmentVariable = \"SPIREPLUS_ASCENSION_DEBUG_LEVEL\"",
            "LegacyDebugLevelEnvironmentVariable = \"EZMB_ASCENSION_DEBUG_LEVEL\"",
            "PublicGateEnvironmentVariable = \"SPIREPLUS_ASCENSION_ALLOW_PUBLIC_ASCENSION\"",
            "LegacyPublicGateEnvironmentVariable = \"EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION\"",
            "DisablePublicSelectionEnvironmentVariable = \"SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION\"",
            "LegacyDisablePublicSelectionEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION\"",
            "DisableMultiplayerSelectionEnvironmentVariable = \"SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION\"",
            "LegacyDisableMultiplayerSelectionEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION\"",
            "MaxSupportedAscensionLevel = 20",
            "return 0;",
            "return IsPublicSelectionEnabled && runState.AscensionLevel >= requiredAscensionLevel",
            "public static bool IsPublicSelectionEnabled =>",
            "LegacyDisablePublicSelectionEnvironmentVariable",
            "public static bool IsMultiplayerSelectionDisabled =>",
            "LegacyDisableMultiplayerSelectionEnvironmentVariable",
            "A11-A20 selection is default-on for single-player private-beta testing",
            "multiplayer A11-A20 gameplay is fail-closed unless",
            "AllowUnverifiedCoopGameplayEnvironmentVariable",
            "Set {AscensionFeatureGate.DisablePublicSelectionEnvironmentVariable}=1 to restore vanilla A1-A10 selection",
            "Math.Clamp(level, 0, MaxSupportedAscensionLevel)",
            "StartRunLobbySetSingleplayerAscensionPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), \"SetSingleplayerAscensionAfterCharacterChanged\", [typeof(ModelId)])",
            "StartRunLobbyBeginRunLocallyPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), \"BeginRunLocally\", [typeof(string), typeof(List<ModifierModel>)])",
            "StartRunLobbyUpdateMaxMultiplayerAscensionPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), \"UpdateMaxMultiplayerAscension\")",
            "StartRunLobbyUpdatePreferredAscensionPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), \"UpdatePreferredAscension\")",
            "AccessTools.Field(typeof(StartRunLobby), \"<MaxAscension>k__BackingField\")",
            "if (MaxAscensionBackingField == null)",
            "lobby.NetService.Type == NetGameType.Singleplayer",
            "lobby.NetService.Type == NetGameType.Host",
            "lobby.GameMode != GameMode.Daily",
            "stats.MaxAscension = Math.Min(",
            "TemporarilyExpandMultiplayerUnlocks",
            "maxMultiplayerAscensionUnlocked = AscensionFeatureGate.MaxSupportedAscensionLevel",
            "RestoreMultiplayerUnlocks",
            "ShouldSkipVanillaPreferredAscensionSave",
            "not writing it to vanilla progress",
            "MultiplayerA20DowngradeWarning",
            "Multiplayer A11-A20 gameplay is fail-closed by default after crash logs.",
            "Set {MultiplayerFeaturePolicy.AllowUnverifiedCoopGameplayEnvironmentVariable}=1 only for focused two-client debugging.",
            "A11-A20 co-op selection is disabled by default because run-state, map, reward, and combat mutations do not yet have two-client proof.",
            "WarnIfA20MultiplayerDowngraded",
            "ShouldWarnA20MultiplayerDowngrade",
            "lobby.Ascension >= AscensionFeatureGate.DoubleRoyalBrandLevel",
            "players: {lobby.Players.Count}",
            "StartRunLobbySyncAscensionChangeA20WarningPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange), [typeof(int)])",
            "StartRunLobbyBeginRunForAllPlayersA20WarningPatch : IPatchMethod",
            "new ModPatchTarget(typeof(StartRunLobby), \"BeginRunForAllPlayers\", [typeof(string), typeof(List<ModifierModel>)])",
            "host multiplayer ascension selection",
            "host multiplayer run start",
            "IsBrandedFormSinglePlayerEnabled(IRunState runState)",
            "runState.Players.Count == 1",
            "__state.Stats.MaxAscension = __state.OriginalMaxAscension");

        Assert.DoesNotContain("NAscensionPanel", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsTruthy(Environment.GetEnvironmentVariable(PublicGateEnvironmentVariable))", source, StringComparison.Ordinal);
        Assert.DoesNotContain("HarmonyPatch(typeof(CharacterStats", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ProgressSaveManager", source, StringComparison.Ordinal);
        Assert.DoesNotContain("HarmonyPatch(typeof(ProgressState", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AscensionManager.maxAscensionAllowed", source, StringComparison.Ordinal);
        Assert.DoesNotContain("lobby.Players.Count > 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Default-off gate", source, StringComparison.Ordinal);
    }

    [Fact]
    public void EnvironmentTruthyHelpersTrimWhitespaceForTesterRunFlags()
    {
        var ascensionConfig = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var ascensionGate = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var ancientGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientFeatureGate.cs");

        AssertSourceContains(
            ascensionConfig,
            "var candidate = value?.Trim();",
            "candidate.Equals(\"1\", StringComparison.OrdinalIgnoreCase)",
            "candidate.Equals(\"true\", StringComparison.OrdinalIgnoreCase)",
            "candidate.Equals(\"yes\", StringComparison.OrdinalIgnoreCase)",
            "candidate.Equals(\"on\", StringComparison.OrdinalIgnoreCase)");
        Assert.Contains("FirstRawEnvironmentValue(", ascensionGate, StringComparison.Ordinal);
        Assert.Contains("DebugLevelEnvironmentVariable,", ascensionGate, StringComparison.Ordinal);
        Assert.Contains("LegacyDebugLevelEnvironmentVariable)?.Trim()", ascensionGate, StringComparison.Ordinal);
        AssertSourceContains(
            ancientGate,
            "IsTruthyEnvironmentVariable(string name, bool trimValue = true)",
            "var candidate = trimValue ? value?.Trim() : value;");
    }

    [Fact]
    public void AscensionLocalizationBridgeCoversModdedOriginalAscensionTableKeys()
    {
        var bridge = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionLocalizationBridge.cs");
        var patches = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionLocalizationTablePatches.cs");
        var source = string.Join(Environment.NewLine, bridge, patches);
        var englishAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            source,
            "new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetRawText))",
            "new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetLocString))",
            "new ModPatchTarget(typeof(LocTable), nameof(LocTable.HasEntry))",
            "new ModPatchTarget(typeof(LocTable), nameof(LocTable.IsLocalKey))",
            "new ModPatchTarget(typeof(LocManager), nameof(LocManager.GetTable))",
            "new ModPatchTarget(typeof(LocString), nameof(LocString.GetRawText))",
            "[HarmonyPrefix]",
            "[HarmonyPostfix]",
            "[HarmonyFinalizer]",
            "AscensionLocalizationBridge.MergeIntoIfAscensionTable(__result)",
            "table.MergeWith(new Dictionary<string, string>(localizedTable, StringComparer.Ordinal))",
            "TableNameField?.GetValue(table) is string tableName",
            "tableName.Equals(\"ascension\", StringComparison.Ordinal)",
            "$\"{MainFile.ResPath}/localization/{language}/ascension.json\"",
            "TryGetTextForLanguage(\"eng\", key, out text)",
            "AscensionLocalizationBridge.IsAscensionLevelKey(__instance.LocEntryKey)",
            "AscensionLocalizationBridge.TryGetText(__instance.LocEntryKey, out var text)",
            "__result = new LocString(\"ascension\", key)",
            "Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read)");
        Assert.DoesNotContain("internal static class AscensionLocalizationBridge", patches, StringComparison.Ordinal);
        Assert.DoesNotContain("Godot.FileAccess.Open", patches, StringComparison.Ordinal);
        Assert.DoesNotContain("HarmonyPatch(", bridge, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(", patches, StringComparison.Ordinal);

        for (var level = 11; level <= 20; level++)
        {
            var prefix = $"LEVEL_{level:D2}";
            Assert.True(englishAscension.ContainsKey($"{prefix}.title"), $"Missing English {prefix}.title");
            Assert.True(englishAscension.ContainsKey($"{prefix}.description"), $"Missing English {prefix}.description");
            Assert.True(zhsAscension.ContainsKey($"{prefix}.title"), $"Missing zhs {prefix}.title");
            Assert.True(zhsAscension.ContainsKey($"{prefix}.description"), $"Missing zhs {prefix}.description");
        }
    }

    [Fact]
    public void MultiplayerVersionMismatchDiagnosticsExposeModelHashHandshakeWithoutBypass()
    {
        var diagnostics = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerDiagnostics.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerDiagnostics.JoinFlow.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerDiagnostics.Lobby.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerDiagnostics.RunState.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerDiagnostics.SaveQuit.cs"));
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var runbook = ReadRepoText("docs", "features", "ascension-11-20", "multiplayer-test-runbook.md");

        AssertSourceContains(
            diagnostics,
            "HarmonyPatch(typeof(JoinFlow), \"HandleInitialGameInfoMessage\")",
            "InitialGameInfoMessage message",
            "ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? GitHelper.ShortCommitId ?? \"UNKNOWN\"",
            "ModelIdSerializationCache.Hash",
            "ModManager.GetGameplayRelevantModNameList()",
            "ModManager.GetNonGameplayRelevantModNameList()",
            "message.gameplayAffectingMods",
            "message.otherMods",
            "message.idDatabaseHash == localModelHash",
            "visible game versions match, but the ModelDb hash does not; vanilla will report this as VersionMismatch",
            "non-gameplay relevant mod mismatch is allowed by vanilla",
            "missingOnHost",
            "missingOnLocal");

        Assert.Contains("the version string matches but the hash differs", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Record both `Got initial game info message. Version: ... Hash: ...` and local `ModelIdSerializationCache initialized... Hash: ...` lines.", runbook, StringComparison.Ordinal);

        Assert.DoesNotContain("ConnectionFailureReason.VersionMismatch = null", diagnostics, StringComparison.Ordinal);
        Assert.DoesNotContain("message.idDatabaseHash = ModelIdSerializationCache.Hash", diagnostics, StringComparison.Ordinal);
        Assert.DoesNotContain("message.mods", diagnostics, StringComparison.Ordinal);
        Assert.DoesNotContain("return false", diagnostics, StringComparison.Ordinal);
    }

    [Fact]
    public void MapAndCombatSlicesStayWithinDocumentedA12AndA19Tuning()
    {
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var firemarkTargeting = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Firemarks.Targeting.cs");
        var turnLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.TurnLifecycle.cs");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            mapService,
            "IsAfterActOneFirstRestSite(map, point, actIndex)",
            "ActOneFiremarkedEliteTargetCount = 2",
            "LaterActFiremarkedEliteTargetCount = 3",
            "PickFiremarkedElitesByAct",
            "HasHardFiremarkPlacementConflict",
            "IsOnSameRoute",
            "AreAdjacent",
            "CanReach(left, right) || CanReach(right, left)",
            "KeepsFiremarksOptional(start, boss, selected, point)",
            "EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point)",
            "candidate.PointType == MapPointType.RestSite",
            "point.coord.row > firstRestSiteRow.Value");

        AssertSourceContains(
            combatService,
            "FindFiremarkHost(combatState)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>",
            "PowerCmd.Apply<FiremarkMightOverflowPower>",
            "CreatureCmd.SetMaxAndCurrentHp",
            "tracker.FiremarkHost = host",
            "FiremarkOverflowCandidates(combatState, tracker)",
            "LowestHpRatioOverflowTarget(combatState, tracker)",
            "ApplyBossSealCombatStart(combatState, metadata)");
        AssertSourceContains(
            firemarkTargeting,
            "private static Creature? FindFiremarkHost(CombatState combatState)",
            "enemy.HasPower<MinionPower>()",
            ".OrderByDescending(enemy => enemy.MaxHp)",
            "private static IEnumerable<Creature> FiremarkOverflowCandidates",
            "private static Creature? LowestHpRatioOverflowTarget",
            "enemy.GetHpPercentRemaining()");
        AssertSourceContains(
            combatService,
            "MaxForgeArmorShatters = 2",
            "ApplyFiremarkPlayerTurnStart(combatState, tracker, metadata.Firemark!.Value)",
            "ApplyForgeArmorGain(combatState, tracker)",
            "ApplyForgeArmorOverflow(combatState, tracker)",
            "tracker.FiremarkHost.Block > 0",
            "tracker.FiremarkArmorSkippedNextTurn = true");
        Assert.DoesNotContain("TrackForgeArmorBlockedDamage", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("FiremarkArmorBlockBaseline", combatService, StringComparison.Ordinal);

        var beforeSideTurnStart = SliceBetween(
            turnLifecycle,
            "public static async Task BeforeSideTurnStart",
            "public static async Task AfterTurnEnd");
        var afterTurnEnd = SliceFrom(
            turnLifecycle,
            "public static async Task AfterTurnEnd");
        Assert.DoesNotContain("ApplyShieldwallTurnBlock", beforeSideTurnStart, StringComparison.Ordinal);
        AssertSourceContains(
            afterTurnEnd,
            "metadata.Banner == BannerKind.Shieldwall",
            "side == CombatSide.Enemy",
            "await ApplyShieldwallTurnBlock(combatState, tracker)");

        Assert.DoesNotContain("await ApplyStrengthToEnemies(combatState, 2m);", combatService, StringComparison.Ordinal);
        Assert.Contains("Act 1 firemarked elite appears only after the first rest-site row.", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void AscensionMapModifierVarietyPreviewAndFissionDiagnosticsAreGuarded()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var mapPatch = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var bannerTargeting = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Banners.Targeting.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var playerGuide = ReadRepoText("docs", "features", "ascension-11-20", "player-facing-modifier-guide.md");
        var englishAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            metadata,
            "Might",
            "Giant",
            "ForgeArmor",
            "ConstantHeal",
            "Vanguard",
            "Shieldwall",
            "BloodPrize",
            "PressingLine",
            "LastStand");

        AssertSourceContains(
            mapService,
            "StableMarkerHash",
            "runState.Rng.StringSeed",
            "runState.Rng.Seed",
            "markerFamily",
            "coord.col",
            "coord.row",
            "GetKindFromActOrder<FiremarkKind>",
            "GetKindFromActOrder<BannerKind>",
            "EnumerateByStableMarkerOrder",
            "Ascension map assignment: actIndex=",
            "map marker distribution");

        Assert.DoesNotContain("(actIndex + markedCount) % Enum.GetValues<FiremarkKind>().Length", mapService, StringComparison.Ordinal);
        Assert.DoesNotContain("(actIndex + markedCount) % Enum.GetValues<BannerKind>().Length", mapService, StringComparison.Ordinal);

        AssertSourceContains(
            combatService,
            "ResolveBannerForCombat(combatState, metadata)",
            "RequiresMultiplePrimaryEnemies(banner)",
            "metadata.Banner = fallback",
            "BannerKind.BloodPrize",
            "converted {banner} banner to {fallback}");
        AssertSourceContains(
            bannerTargeting,
            "private static BannerKind ResolveBannerForCombat(CombatState combatState, AscensionNodeMetadata metadata)",
            "private static bool RequiresMultiplePrimaryEnemies(BannerKind banner)",
            "banner is BannerKind.Shieldwall or BannerKind.LastStand",
            "private static Creature? PickBannerTarget(CombatState combatState)",
            "PrimaryAliveEnemies(combatState).ToList()",
            "AliveEnemies(combatState).ToList()",
            ".OrderByDescending(enemy => enemy.MaxHp)",
            ".ThenBy(enemy => combatState.Enemies.IndexOf(enemy))");

        AssertSourceContains(
            mapPatch,
            "FiremarkedEliteMapHoverPatch",
            "GetFiremarkIndicator(firemark)",
            "GetBannerIndicator(banner)",
            "FIREMARK_MIGHT",
            "FIREMARK_GIANT",
            "FIREMARK_FORGE_ARMOR",
            "FIREMARK_CONSTANT_HEAL",
            "BannerRoomMapHoverPatch",
            "BANNER_VANGUARD",
            "BANNER_SHIELDWALL",
            "BANNER_BLOOD_PRIZE",
            "BANNER_PRESSING_LINE",
            "BANNER_LAST_STAND",
            "AddCurrentActFiremarkValues(description, firemark)",
            "AddCurrentActBannerValues(description, banner)",
            "RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex",
            "description.Add(\"OverflowStrength\", ActValue(actIndex, 1m, 1m, 2m))",
            "description.Add(\"OverflowDamage\", ActValue(actIndex, 6m, 12m, 24m))",
            "description.Add(\"OverflowBlock\", ActValue(actIndex, 3m, 6m, 12m))",
            "description.Add(\"OverflowHeal\", ActValue(actIndex, 2m, 4m, 8m))",
            "description.Add(\"Armor\", ActValue(actIndex, 8m, 14m, 24m))",
            "description.Add(\"InterruptDamage\", ActValue(actIndex, 18m, 36m, 72m))",
            "description.Add(\"DeathBlock\", ActValue(actIndex, 5m, 10m, 20m))",
            "description.Add(\"Gold\", ActValue(actIndex, 15m, 30m, 55m))",
            "BossMapPointHoverPatch",
            "CreateHoverTip(metadata.BossSeal, metadata.IsBossBrand)",
            "PreloadManager.Cache.GetTexture2D(AscensionAssetPaths.GetBossSealIndicator(definition.Id))",
            "sourceFallbackDescription = isBossBrand ? definition.BrandSummary : definition.Summary");
        AssertSourceContains(
            combatService,
            "var wasExposedBeforeThisHit = tracker.FiremarkCoreExposed",
            "!wasExposedBeforeThisHit",
            ".OrderByDescending(enemy => enemy.MaxHp)",
            ".ThenBy(enemy => combatState.Enemies.IndexOf(enemy))");
        Assert.DoesNotContain("RunState.Rng.Niche.NextItem(candidates)", combatService, StringComparison.Ordinal);

        foreach (var key in new[]
                 {
                     "FIREMARK_MIGHT",
                     "FIREMARK_GIANT",
                     "FIREMARK_FORGE_ARMOR",
                      "FIREMARK_CONSTANT_HEAL",
                      "BANNER_VANGUARD",
                     "BANNER_SHIELDWALL",
                     "BANNER_BLOOD_PRIZE",
                     "BANNER_PRESSING_LINE",
                     "BANNER_LAST_STAND"
                  })
        {
            Assert.True(englishAscension.ContainsKey($"{key}.title"), $"Missing English title: {key}");
            Assert.True(englishAscension.ContainsKey($"{key}.description"), $"Missing English description: {key}");
            Assert.True(zhsAscension.ContainsKey($"{key}.title"), $"Missing zhs title: {key}");
            Assert.True(zhsAscension.ContainsKey($"{key}.description"), $"Missing zhs description: {key}");
        }

        foreach (var key in new[]
                 {
                     "FIREMARK_MIGHT",
                     "FIREMARK_GIANT",
                     "FIREMARK_FORGE_ARMOR",
                     "FIREMARK_CONSTANT_HEAL",
                     "BANNER_VANGUARD",
                     "BANNER_SHIELDWALL",
                     "BANNER_BLOOD_PRIZE",
                     "BANNER_PRESSING_LINE",
                     "BANNER_LAST_STAND"
                 })
        {
            Assert.DoesNotMatch("\\[blue\\][^\\[]+\\[/blue\\]/\\[blue\\]", englishAscension[$"{key}.description"]);
            Assert.DoesNotMatch("\\[blue\\][^\\[]+\\[/blue\\]/\\[blue\\]", zhsAscension[$"{key}.description"]);
        }

        Assert.Contains("After [gold]Ebb[/gold], create [blue]2[/blue] Time Sand", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"], StringComparison.Ordinal);
        Assert.Contains("extra [gold]Wither[/gold]", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"], StringComparison.Ordinal);
        var bossSealMarkerPowers = ReadBossSealMarkerPowerSources();
        Assert.Contains("花费能量清时砂；剩余时砂会增加枯萎", bossSealMarkerPowers, StringComparison.Ordinal);
        Assert.DoesNotContain("剩余时砂会增加根蚀", bossSealMarkerPowers, StringComparison.Ordinal);
        Assert.Contains("地图悬停", zhsAscension["MODIFIER_GUIDE.description"], StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll",
            "var rewardRng = creationOptions.RngOverride ?? player.PlayerRng.Rewards",
            "sourceLabel={sourceLabel}",
            "chancePercent={chancePercent}",
            "eligibleCandidateCount={eligibleCandidateCount}",
            "applied={applied}",
            "cardId={cardId ?? \"<none>\"}");
        Assert.DoesNotContain("player.PlayerRng.Rewards.NextInt", rewardService, StringComparison.Ordinal);
        Assert.DoesNotContain("player.PlayerRng.Rewards.NextItem", rewardService, StringComparison.Ordinal);

        AssertSourceContains(
            playerGuide,
            "Firemarked Elite",
            "Banner Room",
            "Boss Dedicated Ability",
            "Branded Form",
            "Fission reward enchantment",
            "Map hover previews");
    }

    [Fact]
    public void AscensionMapMetadataIsReappliedBeforeCombatLookup()
    {
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");

        AssertSourceContains(
            mapService,
            "public static AscensionNodeMetadata? TryGetCurrentMetadata(IRunState runState)",
            "var appliedMap = Apply(runState, runState.Map, runState.CurrentActIndex)",
            "runState.Map = appliedMap",
            "return TryGetMetadata(runState.CurrentMapPoint);");

        AssertSourceContains(
            combatService,
            "tracker.NodeMetadata = AscensionMapService.TryGetCurrentMetadata(combatState.RunState)",
            "tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState)");

        Assert.DoesNotContain("TryGetMetadata(combatState.RunState.CurrentMapPoint", combatService, StringComparison.Ordinal);
        Assert.Contains("Combat modifier lookup re-applies deterministic map metadata", apiResearch, StringComparison.Ordinal);
    }
}
