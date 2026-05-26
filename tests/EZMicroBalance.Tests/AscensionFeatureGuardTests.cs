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
            "HarmonyPatch(typeof(StartRunLobby), \"SetSingleplayerAscensionAfterCharacterChanged\")",
            "HarmonyPatch(typeof(StartRunLobby), \"BeginRunLocally\")",
            "HarmonyPatch(typeof(StartRunLobby), \"UpdateMaxMultiplayerAscension\")",
            "HarmonyPatch(typeof(StartRunLobby), \"UpdatePreferredAscension\")",
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
            "HarmonyPatch(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange))",
            "HarmonyPatch(typeof(StartRunLobby), \"BeginRunForAllPlayers\")",
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
            "HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))",
            "HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))",
            "HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))",
            "HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))",
            "HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))",
            "HarmonyPatch(typeof(LocString), nameof(LocString.GetRawText))",
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
            "message.idDatabaseHash == localModelHash",
            "visible game versions match, but the ModelDb hash does not; vanilla will report this as VersionMismatch",
            "missingOnHost",
            "missingOnLocal");

        Assert.Contains("the version string matches but the hash differs", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Record both `Got initial game info message. Version: ... Hash: ...` and local `ModelIdSerializationCache initialized... Hash: ...` lines.", runbook, StringComparison.Ordinal);

        Assert.DoesNotContain("ConnectionFailureReason.VersionMismatch = null", diagnostics, StringComparison.Ordinal);
        Assert.DoesNotContain("message.idDatabaseHash = ModelIdSerializationCache.Hash", diagnostics, StringComparison.Ordinal);
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
    public void BossSealCatalogAvoidsHardRuntimeReferencesToOptionalEarlyAccessBossTypes()
    {
        var bossSealSource = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");

        AssertSourceContains(
            bossSealSource,
            "private const string EncounterCategory = \"ENCOUNTER\"",
            "private static ModelId EncounterId(string entry)",
            "EncounterId(\"AEONGLASS_BOSS\")",
            "EncounterId(\"QUEEN_BOSS\")",
            "EncounterId(\"TEST_SUBJECT_BOSS\")");

        Assert.DoesNotContain("using MegaCrit.Sts2.Core.Models.Encounters", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("ModelDb.GetId<", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("DoormakerBoss", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("QueenBoss", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("TestSubjectBoss", bossSealSource, StringComparison.Ordinal);

        AssertSourceContains(
            combatService,
            "BossSealId.AeonglassHourglass",
            "enemy.Monster is Aeonglass",
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2",
            "PowerCmd.Apply<AeonglassHourglassPower>",
            "TrackAeonglassEnergySpent",
            "SettleAeonglassTimeSand",
            "tracker.AeonglassExtraWitherFromSands",
            "INCREASING_INTENSITY_MOVE",
            "CardPileCmd.AddToCombatAndPreview<Wither>",
            "PowerCmd.Apply<AeonglassLaserEchoPower>",
            "Time Sand Reflow created");

        Assert.DoesNotContain(
            "var boss = AliveEnemies(combatState)\n                .OrderByDescending(enemy => enemy.MaxHp)\n                .FirstOrDefault();",
            combatService.Replace("\r\n", "\n"),
            StringComparison.Ordinal);
        Assert.DoesNotContain("enemy.Monster is Doormaker", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<HungerPower>", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<ScrutinyPower>", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<GraspPower>", combatService, StringComparison.Ordinal);
    }

    [Fact]
    public void A11AndA17MapGeometryStayGatedOptionalAndRouteSafe()
    {
        var featureGate = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var expansionConfig = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var mapProof = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "A11MapGeometryProof.cs");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");
        var mapGenerationPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionMapGenerationPatches.cs");
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var coreRunManager = ReadRepoText("source code", "src", "Core", "Runs", "RunManager.cs");
        var coreMapScreen = ReadRepoText("source code", "src", "Core", "Nodes", "Screens", "Map", "NMapScreen.cs");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            featureGate,
            "A11ExtraMapColumns = 1",
            "A11ActOneExtraMapRows = 1",
            "A11ActTwoExtraMapRows = 1",
            "A11ActThreeExtraMapRows = 2",
            "IsMapGeometryEnabled(IRunState runState)",
            "IsDeepBranchesEnabled(IRunState runState)",
            "AscensionExpansionConfig.Current.EnableMapGeometry",
            "AscensionExpansionConfig.Current.EnableDeepBranches");

        AssertSourceContains(
            expansionConfig,
            "EnableMapGeometryEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_MAP_GEOMETRY\"",
            "LegacyEnableMapGeometryEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_MAP_GEOMETRY\"",
            "EnableDeepBranchesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "LegacyEnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"");

        AssertSourceContains(
            mapService,
            "if (AscensionFeatureGate.IsMapGeometryEnabled(runState))",
            "if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))",
            "VanillaMapColumns = 7",
            "A11InsertedColumn = 4",
            "TryInsertA11WidthChoice(saved)",
            "HasA11InsertedColumnRouteChoice",
            "ApplyA11MapGeometryAtCreateMapBoundary",
            "ActModel.CreateMap",
            "Ascension A11 source-boundary check",
            "insertedColumnRoute={evidence.HasInsertedColumnRouteChoice}",
            "originalRoutePreserved={evidence.HasStartToBossRouteAvoidingInsertedColumn}",
            "insertedColumnRouteChoices={evidence.InsertedColumnRouteChoiceCount}",
            "TryGetA11GeometryEvidence(map, out var evidence)",
            "catch (Exception ex)",
            "A11 map geometry diagnostic failed",
            "A11 map geometry diagnostic failed closed",
            "GetA11TargetRowCount(runState, actIndex)",
            "HasA11OriginalRoutePreserved(saved)",
            "A11MapGeometryProof.Analyze",
            "HasSerializablePath(saved.StartingPoint",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "EnumerateDeepBranchColumns(map)",
            "TryMatchExistingDeepBranch",
            "IsDeepBranchRouteSafe(saved, plan)",
            "HasSerializablePathAvoiding",
            "runState.Players.Count > 1",
            "IsDeepBranchAct(actIndex)",
            "canBeModified: false",
            "MapPointType.Elite",
            "MapPointType.Treasure",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "safe-route reconnect");

        AssertSourceContains(
            mapProof,
            "A11MapGeometryGraph",
            "A11MapGeometryEvidence",
            "HasInsertedColumnRouteChoice",
            "HasStartToBossRouteAvoidingInsertedColumn",
            "InsertedColumnRouteChoiceCount");

        AssertSourceContains(
            rootRunHook,
            "public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)",
            "return AscensionMapService.Apply(runState, map, actIndex);",
            "public override ActMap ModifyGeneratedMapLate(IRunState runState, ActMap map, int actIndex)");

        AssertSourceContains(
            mapGenerationPatch,
            "HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))",
            "Postfix(RunState runState, ref ActMap __result)",
            "AscensionMapService.ApplyA11MapGeometryAtCreateMapBoundary",
            "runState.CurrentActIndex");

        AssertSourceContains(
            coreRunManager,
            "ActMap map2 = State.Act.CreateMap(State, replaceTreasureWithElites: false)",
            "map = Hook.ModifyGeneratedMap(State, map2, State.CurrentActIndex)",
            "State.Map = map",
            "NMapScreen.Instance?.SetMap(map, State.Rng.Seed, clearDrawings: true)");

        AssertSourceContains(
            coreMapScreen,
            "int rowCount = map.GetRowCount()",
            "int columnCount = map.GetColumnCount()",
            "_distY = 2325f / (float)(rowCount - 1)",
            "_distX = 1050f / (float)columnCount",
            "foreach (MapPoint allMapPoint in map.GetAllMapPoints())");

        AssertSourceContains(
            metadata,
            "DeepBranchNodeKind",
            "EnhancedReward",
            "DeepBranch.HasValue",
            "IsDeepBranchEntry");

        Assert.Contains("A11 converts the generated map", apiResearch, StringComparison.Ordinal);
        Assert.Contains("reachable optional route", apiResearch, StringComparison.Ordinal);
        Assert.Contains("A17 uses the same saved-map replacement path", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Multiplayer Deep Branch insertion is intentionally skipped", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Map width increases from 7 to 8 columns.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Act 1 visible route rows increase by 1, Act 2 visible route rows increase by 1, and Act 3 visible route rows increase by 2.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("At least one reachable optional node appears in the inserted width column.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("No A11-specific marker, icon, or hover tooltip appears", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A safer parallel route from the branch parent to reconnect remains available", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Deep Branch insertion now searches for an empty branch column", apiResearch, StringComparison.Ordinal);
        Assert.Contains("start-to-boss route that skips branch nodes remains", apiResearch, StringComparison.Ordinal);
        Assert.DoesNotContain("MarkLongRoad", mapService, StringComparison.Ordinal);
        Assert.DoesNotContain("LongRoad", metadata, StringComparison.Ordinal);
        Assert.DoesNotContain("LONG_ROAD_NODE", mapService, StringComparison.Ordinal);
    }

    [Fact]
    public void FiremarkTokenAndFissionPlayerFacingSurfacesAreGuarded()
    {
        var mapPatch = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var forgeToken = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var firemarkPowers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var corePowerNode = ReadRepoText("source code", "src", "Core", "Nodes", "Combat", "NPower.cs");
        var fission = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "FissionEnchantment.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var engAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            mapPatch,
            "NNormalMapPoint",
            "\"_questIcon\"",
            "FiremarkedEliteMapQuestMarker",
            "AscensionAssetPaths.GetFiremarkIndicator(firemark)",
            "AscensionAssetPaths.GetBannerIndicator(banner)",
            "AscensionAssetPaths.FiremarkedEliteIndicator");

        AssertSourceContains(
            forgeToken,
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "return targets.FirstOrDefault()",
            "SpecialRestSiteActionPayoutEnabled = false",
            "ModifyExtraRestSiteHealText");
        Assert.DoesNotContain("player.RunState.Rng.Niche.NextItem(targets)", forgeToken, StringComparison.Ordinal);
        Assert.DoesNotContain("RestSiteSynchronizer", forgeToken, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplyAfterSpecialRestSiteAction", forgeToken, StringComparison.Ordinal);

        AssertSourceContains(
            forgeRelic,
            "public override RelicRarity Rarity => RelicRarity.Event",
            "public override bool ShowCounter => true",
            "public override int DisplayAmount => 1",
            "additionalRestSiteHealText",
            "[gold]Smith[/gold] heals [blue]7[/blue] HP");

        AssertSourceContains(
            firemarkPowers,
            "MightMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkMightIndicator",
            "GiantMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkGiantIndicator",
            "ForgeArmorMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkForgeArmorIndicator",
            "ConstantHealMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkConstantHealIndicator",
            "FiremarkMightOverflowPower",
            "Overflow: Might",
            "protected override IEnumerable<DynamicVar> CanonicalVars => [new InterruptDamageDynamicVar()]",
            "description.Add(InterruptDamageVar, InterruptDamage)",
            "private int InterruptDamage => Amount switch",
            "Taking [blue]{InterruptDamage}[/blue] damage interrupts its next heal.",
            "internal abstract class FiremarkPower",
            "public override PowerStackType StackType => PowerStackType.Counter",
            "public override int DisplayAmount => Amount",
            "Firemark: Might",
            "Firemark: Constant Heal");
        Assert.Contains("Model.StackType == PowerStackType.Counter", corePowerNode, StringComparison.Ordinal);

        AssertSourceContains(
            fission,
            "CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon",
            "这张牌的[gold]耗能[/gold]降低[blue]1[/blue]。打出后进入[gold]消耗[/gold]牌堆，并正常触发[gold]消耗[/gold]效果。",
            "[gold]耗能[/gold]降低[blue]1[/blue]。正常触发[gold]消耗[/gold]效果。",
            "This card costs [blue]1[/blue] less. After play, it enters the [gold]Exhaust[/gold] pile and triggers [gold]Exhaust[/gold] effects normally.",
            "Costs [blue]1[/blue] less. Triggers [gold]Exhaust[/gold] effects normally.",
            "HoverTipFactory.FromKeyword(CardKeyword.Exhaust)");
        Assert.DoesNotContain("energyPrefix:energyIcons", fission, StringComparison.Ordinal);
        Assert.DoesNotContain("\"[gold]能量[/gold]费用降低[blue]1[/blue]。\"", fission, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "ModelDb.Enchantment<FissionEnchantment>().CanEnchant(card)",
            "card.Type is CardType.Attack or CardType.Skill",
            "!card.EnergyCost.CostsX",
            "!card.HasStarCostX",
            "card.CurrentStarCost <= 0",
            "!card.Keywords.Contains(CardKeyword.Exhaust)",
            "!card.ExhaustOnNextPlay",
            "card.Enchantment == null");

        Assert.StartsWith("火印精英", zhsAscension["LEVEL_12.title"], StringComparison.Ordinal);
        Assert.Contains("[gold]Firemarked Elites[/gold] appear on the map", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Forge Token[/gold]", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Act 1 has", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Acts 2 and 3", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Firemark Host", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("地图上会出现[gold]火印精英[/gold]", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("铸令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第一幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第二幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第三幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("火印宿主", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("注令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("路线", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("费用", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]耗能[/gold]降低[blue]1[/blue]", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        var allAscensionText = string.Join(Environment.NewLine, engAscension.Values.Concat(zhsAscension.Values));
        Assert.DoesNotContain("Wake-up source", allAscensionText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Queen-side settlement", allAscensionText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u7206\u53d1\u538b\u529b", allAscensionText, StringComparison.Ordinal);
        Assert.DoesNotContain("\u7206\u53d1\u9884\u8b66", allAscensionText, StringComparison.Ordinal);
        Assert.Contains("Forge Token special rest-site action payout is disabled", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Special rest-site actions heal 5 HP", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void FissionUsesCanonicalExhaustPipelineAndTriggersExhaustListeners()
    {
        var fission = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "FissionEnchantment.cs");
        var cardModel = ReadRepoText("source code", "src", "Core", "Models", "CardModel.cs");
        var cardCmd = ReadRepoText("source code", "src", "Core", "Commands", "CardCmd.cs");
        var hook = ReadRepoText("source code", "src", "Core", "Hooks", "Hook.cs");
        var drumOfBattle = ReadRepoText("source code", "src", "Core", "Models", "Cards", "DrumOfBattle.cs");
        var howlFromBeyond = ReadRepoText("source code", "src", "Core", "Models", "Cards", "HowlFromBeyond.cs");
        var feelNoPain = ReadRepoText("source code", "src", "Core", "Models", "Powers", "FeelNoPainPower.cs");
        var darkEmbrace = ReadRepoText("source code", "src", "Core", "Models", "Powers", "DarkEmbracePower.cs");
        var charonsAshes = ReadRepoText("source code", "src", "Core", "Models", "Relics", "CharonsAshes.cs");

        AssertSourceContains(
            fission,
            "Card.AddKeyword(CardKeyword.Exhaust)",
            "triggers [gold]Exhaust[/gold] effects normally",
            "正常触发[gold]消耗[/gold]效果");

        var resultPile = SliceBetween(cardModel, "protected virtual PileType GetResultPileTypeForCardPlay()", "public async Task MoveToResultPileWithoutPlaying");
        AssertSourceContains(
            resultPile,
            "if (ExhaustOnNextPlay || Keywords.Contains(CardKeyword.Exhaust))",
            "return PileType.Exhaust;");

        var playWrapper = SliceBetween(cardModel, "public async Task OnPlayWrapper", "protected async Task<int> GeneratePlayCount");
        AssertSourceContains(
            playWrapper,
            "var (resultPileType, resultPilePosition) = Hook.ModifyCardPlayResultPileTypeAndPosition",
            "case PileType.Exhaust:",
            "await CardCmd.Exhaust(choiceContext, this, causedByEthereal: false, skipCardPileVisuals);");

        AssertSourceContains(
            cardCmd,
            "public static async Task Exhaust",
            "await CardPileCmd.Add(card, PileType.Exhaust",
            "CombatManager.Instance.History.CardExhausted(combatState, card)",
            "await Hook.AfterCardExhausted(combatState, choiceContext, card, causedByEthereal)");

        var exhaustHook = SliceBetween(hook, "public static async Task AfterCardExhausted", "public static async Task AfterCardGeneratedForCombat");
        AssertSourceContains(
            exhaustHook,
            "foreach (AbstractModel model in combatState.IterateHookListeners())",
            "await model.AfterCardExhausted(choiceContext, card, causedByEthereal)");

        AssertSourceContains(
            drumOfBattle,
            "public override async Task AfterCardExhausted",
            "if (card == this",
            "await PlayerCmd.GainEnergy");
        AssertSourceContains(
            howlFromBeyond,
            "public override async Task AfterAutoPostPlayPhaseEntered",
            "pile.Type == PileType.Exhaust",
            "await CardCmd.AutoPlay(choiceContext, this, null)");
        AssertSourceContains(
            feelNoPain,
            "public override async Task AfterCardExhausted",
            "await CreatureCmd.GainBlock");
        AssertSourceContains(
            darkEmbrace,
            "public override async Task AfterCardExhausted",
            "await CardPileCmd.Draw");
        AssertSourceContains(
            charonsAshes,
            "public override async Task AfterCardExhausted",
            "await CreatureCmd.Damage");
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
