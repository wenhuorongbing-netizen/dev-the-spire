using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AscensionFeatureGuardTests
{
    [Fact]
    public void AscensionSelectionExtendsOriginalStandardLobbiesWithoutGlobalProgressGetterPatch()
    {
        var source = ReadAscensionSource();

        AssertSourceContains(
            source,
            "DebugLevelEnvironmentVariable = \"EZMB_ASCENSION_DEBUG_LEVEL\"",
            "PublicGateEnvironmentVariable = \"EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION\"",
            "DisablePublicSelectionEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION\"",
            "DisableMultiplayerSelectionEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION\"",
            "MaxSupportedAscensionLevel = 20",
            "return 0;",
            "return IsPublicSelectionEnabled && runState.AscensionLevel >= requiredAscensionLevel",
            "public static bool IsPublicSelectionEnabled =>",
            "!IsTruthy(Environment.GetEnvironmentVariable(DisablePublicSelectionEnvironmentVariable))",
            "public static bool IsMultiplayerSelectionDisabled =>",
            "IsTruthy(Environment.GetEnvironmentVariable(DisableMultiplayerSelectionEnvironmentVariable))",
            "A11-A20 selection is default-on for private-beta multiplayer testing",
            "set {AscensionFeatureGate.DisablePublicSelectionEnvironmentVariable}=1 to restore vanilla A1-A10 selection",
            "set {AscensionFeatureGate.DisableMultiplayerSelectionEnvironmentVariable}=1 to disable only host-multiplayer A11-A20 selection",
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
            "Multiplayer A20 selection is enabled for development testing.",
            "Dual King Brands / second-boss Brand gameplay is currently disabled or downgraded in co-op pending live verification.",
            "A11-A19 inherited systems may still apply if their gates are enabled, subject to live verification.",
            "WarnIfA20MultiplayerDowngraded",
            "ShouldWarnA20MultiplayerDowngrade",
            "lobby.Ascension >= AscensionFeatureGate.DoubleRoyalBrandLevel",
            "players: {lobby.Players.Count}",
            "HarmonyPatch(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange))",
            "HarmonyPatch(typeof(StartRunLobby), \"BeginRunForAllPlayers\")",
            "host multiplayer ascension selection",
            "host multiplayer run start",
            "IsDualKingBrandsSinglePlayerEnabled(IRunState runState)",
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
    public void RootStarterUsesSavedPlayerMarkerAndCommandDeckMutation()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var service = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, bool> RootBeginsApplied",
            "EZMicroBalanceAscensionRootBeginsApplied");

        AssertSourceContains(
            service,
            "AscensionSavedStateFields.RootblightLevel[player]",
            "AscensionSavedStateFields.RootBeginsApplied[player] = true;",
            "player.RunState.CreateCard<Root>(player)",
            "player.RunState.CreateCard<DeepRoot>(player)",
            "player.RunState.CreateCard<RootblightIII>(player)",
            "private static async Task<bool> AddRootblightCard(Player player, int level, bool hasSplit = false)",
            "MaxRootblightCards = 4",
            "CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, source: null, skipVisuals: true)",
            "await CardPileCmd.RemoveFromDeck(card, showPreview: false)");

        AssertSourceContains(
            runHook,
            "public RootRunHook()",
            "AfterActEntered()",
            "RunManager.Instance.DebugOnlyGetState()",
            "AscensionFeatureGate.IsRootblightEnabled(runState)");

        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        Assert.Contains("ModelDb.GetById<RootRunHook>(ModelDb.GetId<RootRunHook>())", initializer, StringComparison.Ordinal);
        Assert.DoesNotContain("new RootRunHook(", initializer, StringComparison.Ordinal);
    }

    [Fact]
    public void RootBudSeedingUsesExistingPileScanAndSavedPerCardFlags()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var combatHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.cs");
        var cards = ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<RootBud, bool> RootBudEnteredHand",
            "SavedSpireField<RootBud, bool> RootBudPlayed",
            "SavedSpireField<RootBud, bool> RootBudSprouted",
            "SavedSpireField<RootBud, int> RootBudSproutRound");

        AssertSourceContains(
            combatHook,
            "public RootBudCombatHook()",
            "public override bool ShouldReceiveCombatHooks => true",
            "CurrentCombatState()",
            "CombatManager.Instance.DebugOnlyGetState()",
            "var existingBuds = FindRootBudsInCombat(player)",
            "GetRootBudCountForCurrentRoom(state)",
            "GetRootBudSproutRoundForCurrentRoom(state, i)",
            "RootBud.BossSecondSproutRound",
            "player.Piles",
            "SelectMany(pile => pile.Cards)",
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)",
            "await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top)",
            "AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "MarkEnteredHand(state, bud)",
            "Trackers.Remove(state)");

        AssertSourceContains(
            cards,
            "get => AscensionSavedStateFields.RootBudEnteredHand[this]",
            "get => AscensionSavedStateFields.RootBudPlayed[this]",
            "get => AscensionSavedStateFields.RootBudSprouted[this]",
            "get => Math.Max(DefaultSproutRound, AscensionSavedStateFields.RootBudSproutRound[this])",
            "ExhaustOnNextPlay = true");

        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        Assert.Contains("ModelDb.GetById<RootBudCombatHook>(ModelDb.GetId<RootBudCombatHook>())", initializer, StringComparison.Ordinal);
        Assert.DoesNotContain("new RootBudCombatHook(", initializer, StringComparison.Ordinal);
    }

    [Fact]
    public void RootBudGameplayGateProtectsDiagnosticsActOneElitesAndPlayerDeath()
    {
        var combatHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.cs");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            combatHook,
            "IsGameplayEnabledForCurrentRoom(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "return state.RunState.CurrentActIndex is 1 or 2;",
            "currentRow >= 3",
            "after combat end without Blight Sprout growth",
            "AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)",
            "GetTracker(state).DiedPlayers.Add(creature.Player)",
            "!tracker.DiedPlayers.Contains(bud.Owner)");

        Assert.Contains("Act 1 bosses and Act 1 elites are excluded from the current Blight Sprout slice.", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Diagnostics-only mode must not raise Rootblight from restored Blight Sprout cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Knockout/revive should not raise Rootblight from that combat's Blight Sprout.", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void MapAndCombatSlicesStayWithinDocumentedA12AndA19Tuning()
    {
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionMapService.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.cs");
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
            "CreatureCmd.SetMaxAndCurrentHp",
            "ApplyBossSealCombatStart(combatState, metadata)");

        Assert.DoesNotContain("await ApplyStrengthToEnemies(combatState, 2m);", combatService, StringComparison.Ordinal);
        Assert.Contains("Act 1 firemarked elite appears only after the first rest-site row.", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void BossSealCatalogAvoidsHardRuntimeReferencesToOptionalEarlyAccessBossTypes()
    {
        var bossSealDefinition = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "BossSealDefinition.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.cs");

        AssertSourceContains(
            bossSealDefinition,
            "private const string EncounterCategory = \"ENCOUNTER\"",
            "private static ModelId EncounterId(string entry)",
            "EncounterId(\"AEONGLASS_BOSS\")",
            "EncounterId(\"QUEEN_BOSS\")",
            "EncounterId(\"TEST_SUBJECT_BOSS\")");

        Assert.DoesNotContain("using MegaCrit.Sts2.Core.Models.Encounters", bossSealDefinition, StringComparison.Ordinal);
        Assert.DoesNotContain("ModelDb.GetId<", bossSealDefinition, StringComparison.Ordinal);
        Assert.DoesNotContain("DoormakerBoss", bossSealDefinition, StringComparison.Ordinal);
        Assert.DoesNotContain("QueenBoss", bossSealDefinition, StringComparison.Ordinal);
        Assert.DoesNotContain("TestSubjectBoss", bossSealDefinition, StringComparison.Ordinal);

        AssertSourceContains(
            combatService,
            "BossSealId.AeonglassStrength",
            "private static readonly ModelId AeonglassMonsterId = new(\"MONSTER\", \"AEONGLASS\")",
            "FirstOrDefault(enemy => enemy.ModelId == AeonglassMonsterId)",
            "AeonglassStrengthAmount = 5m",
            "Ascension AeonglassStrength: applied +5 Strength");

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
        var featureGate = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureGate.cs");
        var expansionConfig = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionMapService.cs");
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
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
            "EnableMapGeometryEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_MAP_GEOMETRY\"",
            "EnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"");

        AssertSourceContains(
            mapService,
            "if (AscensionFeatureGate.IsMapGeometryEnabled(runState))",
            "if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))",
            "VanillaMapColumns = 7",
            "A11InsertedColumn = 4",
            "TryInsertA11WidthChoice(saved)",
            "HasA11InsertedColumnRouteChoice",
            "HasSerializablePath(saved.StartingPoint",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "runState.Players.Count > 1",
            "IsDeepBranchAct(actIndex)",
            "canBeModified: false",
            "MapPointType.Elite",
            "MapPointType.Treasure",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "safe-route reconnect");

        AssertSourceContains(
            metadata,
            "DeepBranchNodeKind",
            "EnhancedReward",
            "DeepBranch.HasValue");

        Assert.Contains("A11 converts the generated map", apiResearch, StringComparison.Ordinal);
        Assert.Contains("reachable optional route", apiResearch, StringComparison.Ordinal);
        Assert.Contains("A17 uses the same saved-map replacement path", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Multiplayer Deep Branch insertion is intentionally skipped", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Map width increases from 7 to 8 columns.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("At least one reachable optional node appears in the inserted width column.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("No A11-specific marker, icon, or hover tooltip appears", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A safer parallel route from the branch parent to reconnect remains available", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("MarkLongRoad", mapService, StringComparison.Ordinal);
        Assert.DoesNotContain("LongRoad", metadata, StringComparison.Ordinal);
        Assert.DoesNotContain("LONG_ROAD_NODE", mapService, StringComparison.Ordinal);
    }

    [Fact]
    public void FiremarkTokenAndFissionPlayerFacingSurfacesAreGuarded()
    {
        var mapPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionMapUiPatches.cs");
        var forgeToken = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "ForgeTokenService.cs");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var firemarkPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "FiremarkPowers.cs");
        var fission = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "FissionEnchantment.cs");
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "AscensionRewardService.cs");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var engAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            mapPatch,
            "NNormalMapPoint",
            "\"_questIcon\"",
            "FiremarkedEliteMapQuestMarker",
            "AscensionAssetPaths.FiremarkedEliteIndicator");

        AssertSourceContains(
            forgeToken,
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "player.RunState.Rng.Niche.NextItem(targets)",
            "ModifyExtraRestSiteHealText");
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
            "GiantMarkFiremarkPower",
            "ForgeArmorMarkFiremarkPower",
            "ConstantHealMarkFiremarkPower",
            "Firemark: Might",
            "Firemark: Constant Heal");

        AssertSourceContains(
            fission,
            "CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon",
            "这张牌[gold]耗能[/gold]降低[blue]1[/blue]，并获得[gold]消耗[/gold]。",
            "[gold]耗能[/gold]降低[blue]1[/blue]。",
            "HoverTipFactory.FromKeyword(CardKeyword.Exhaust)");
        Assert.DoesNotContain("energyPrefix:energyIcons", fission, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(fission, "[gold]消耗[/gold]"));
        Assert.Equal(1, CountOccurrences(fission, "\"[gold]耗能[/gold]降低[blue]1[/blue]。\""));

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
        Assert.Contains("optional [gold]Firemarked Elites[/gold]", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("Firemark Host", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("可选[gold]火印精英[/gold]", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("铸令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("注令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("路线", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("费用", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]耗能[/gold]降低[blue]1[/blue]", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        Assert.Contains("Forge Token special rest-site action payout is disabled", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Special rest-site actions heal 5 HP", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void AscensionMapMetadataIsReappliedBeforeCombatLookup()
    {
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionMapService.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.cs");
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

    [Fact]
    public void RootFamilyCardsAreLocalizedAndGuardedAgainstKnownRandomGenerationPaths()
    {
        var rootCards = ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Equal("Rootblight I", englishCards["EZMB_ROOT.title"]);
        Assert.Equal("Rootblight II", englishCards["EZMB_DEEP_ROOT.title"]);
        Assert.Equal("Rootblight III", englishCards["EZMB_ROOTBLIGHT_III.title"]);
        Assert.Equal("Blight Sprout", englishCards["EZMB_ROOT_BUD.title"]);

        foreach (var key in new[] { "EZMB_ROOT.title", "EZMB_ROOT.description", "EZMB_DEEP_ROOT.title", "EZMB_DEEP_ROOT.description", "EZMB_ROOTBLIGHT_III.title", "EZMB_ROOTBLIGHT_III.description", "EZMB_ROOT_BUD.title", "EZMB_ROOT_BUD.description" })
        {
            Assert.True(simplifiedChineseCards.ContainsKey(key), $"Missing zhs card key: {key}");
        }

        Assert.Equal(4, CountOccurrences(rootCards, "[Pool(typeof(CurseCardPool))]"));
        Assert.Equal(2, CountOccurrences(rootCards, "public override bool CanBeGeneratedInCombat => false;"));
        Assert.Equal(2, CountOccurrences(rootCards, "public override bool CanBeGeneratedByModifiers => false;"));
        Assert.Contains("CurseCardPool", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Runtime registration and random transform/reward exclusion pending", apiResearch, StringComparison.Ordinal);
    }

    private static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.String, property.Value.ValueKind);
            map.Add(property.Name, property.Value.GetString() ?? string.Empty);
        }

        return map;
    }

    private static string ReadAscensionSource()
    {
        var sourceRoot = RepoPath("EZMicroBalanceCode", "Ascension");
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    private static int CountOccurrences(string source, string snippet)
    {
        var count = 0;
        var index = 0;

        while ((index = source.IndexOf(snippet, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += snippet.Length;
        }

        return count;
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
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
