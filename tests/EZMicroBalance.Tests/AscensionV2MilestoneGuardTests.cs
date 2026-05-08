using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AscensionV2MilestoneGuardTests
{
    [Fact]
    public void Milestone0FeatureFlagsAreIndependentAndAllOffIsANoOp()
    {
        var config = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionExpansionConfig.cs");
        var gates = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionFeatureGate.cs");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionInitializer.cs");
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionMapService.cs");
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionRewardService.cs");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "RootRunHook.cs");

        AssertSourceContains(
            config,
            "DisableAllEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_ALL_SYSTEMS\"",
            "EnableRootblightEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_ROOTBLIGHT\"",
            "EnableBlightSproutEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT\"",
            "EnableFiremarkedElitesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES\"",
            "EnableForgeTokenEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FORGE_TOKEN\"",
            "EnableFissionEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FISSION\"",
            "EnableBannerRoomsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BANNER_ROOMS\"",
            "EnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "EnableBossSealsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BOSS_SEALS\"",
            "EnableDualKingBrandsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS\"",
            "return false;",
            "rootblight={EnableRootblight}",
            "dualBrands={EnableDualKingBrands}");

        AssertSourceContains(
            gates,
            "if (!AscensionExpansionConfig.Current.AnyGameplaySystemEnabled)",
            "return false;",
            "IsMapGeometryEnabled",
            "IsFiremarkedEliteEnabled",
            "IsForgeTokenEnabled",
            "IsFissionEnabled",
            "IsRootblightEnabled",
            "IsBossBlightSproutEnabled",
            "IsBannerRoomEnabled",
            "IsDeepBranchesEnabled",
            "IsBossSealsEnabled",
            "IsDualKingBrandsEnabled",
            "IsDualKingBrandsSinglePlayerEnabled",
            "runState.Players.Count == 1");

        AssertSourceContains(
            initializer,
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||",
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) ||");
        AssertSourceContains(mapService, "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&", "return map;");
        AssertSourceContains(rewardService, "AscensionFeatureGate.IsFiremarkedEliteEnabled", "AscensionFeatureGate.IsBossSealsEnabled", "AscensionFeatureGate.IsFissionEnabled");
        AssertSourceContains(rootRunHook, "AscensionFeatureGate.IsRootblightEnabled", "ForgeTokenService.SyncVisibleTokens");
    }

    [Fact]
    public void Milestone1RootblightAndBlightSproutUseV2NamingStateAndHooks()
    {
        var cardsSource = ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs");
        var deckService = ReadRepoText("EZMicroBalanceCode", "Ascension", "RootDeckService.cs");
        var combatHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "RootBudCombatHook.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionSavedStateFields.cs");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Equal("Rootblight I", englishCards["EZMB_ROOT.title"]);
        Assert.Equal("Rootblight II", englishCards["EZMB_DEEP_ROOT.title"]);
        Assert.Equal("Rootblight III", englishCards["EZMB_ROOTBLIGHT_III.title"]);
        Assert.Equal("Blight Sprout", englishCards["EZMB_ROOT_BUD.title"]);
        Assert.Contains("add 1 Rootblight I", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        foreach (var key in new[] { "EZMB_ROOT.title", "EZMB_DEEP_ROOT.title", "EZMB_ROOTBLIGHT_III.title", "EZMB_ROOT_BUD.title" })
        {
            Assert.True(zhsCards.TryGetValue(key, out var value), $"Missing zhs card localization: {key}");
            Assert.False(string.IsNullOrWhiteSpace(value), $"Empty zhs card localization: {key}");
        }

        AssertSourceContains(
            cardsSource,
            "public sealed class RootblightIII",
            "rootblightLevel: 3",
            "public const int DefaultSproutRound = 3",
            "public const int BossSecondSproutRound = 4",
            "AscensionSavedStateFields.RootBudSproutRound[this]",
            "AscensionSavedStateFields.RootblightWasPresentAtCombatStart[this]",
            "AscensionSavedStateFields.RootblightHasSplit[this]",
            "public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword",
            "ExhaustOnNextPlay = true");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, int> RootblightLevel",
            "SavedSpireField<RootFamilyCard, bool> RootblightWasPresentAtCombatStart",
            "SavedSpireField<RootFamilyCard, bool> RootblightHasSplit",
            "SavedSpireField<RootBud, bool> RootBudEnteredHand",
            "SavedSpireField<RootBud, bool> RootBudPlayed",
            "SavedSpireField<RootBud, bool> RootBudSprouted",
            "SavedSpireField<RootBud, int> RootBudSproutRound");

        AssertSourceContains(
            deckService,
            "MaxRootblightLevel = 3",
            "MaxRootblightCards = 4",
            "FindRootFamilyCards(player)",
            "MarkCombatStartRootblight",
            "PendingCombatResolutions",
            "CardsToAddAfterGrowth",
            "card.RootblightLevel - 1",
            "card.HasSplit = true",
            "ShowRootSystemFull(player)",
            "RemoveHighestRootblight",
            "await CardPileCmd.RemoveFromDeck(card, showPreview: false)",
            "CreateRootblightCard(player, level)");

        AssertSourceContains(
            combatHook,
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)",
            "await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top)",
            "AfterCardChangedPiles(CardModel card, PileType oldPileType",
            "AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)",
            "FindKnownBuds(state)",
            "bud.HasEnteredHand && !bud.WasPlayed",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")",
            "!tracker.DiedPlayers.Contains(bud.Owner)",
            "GetRootBudCountForCurrentRoom(state)",
            "GetRootBudSproutRoundForCurrentRoom(state, i)",
            "RootBud.BossSecondSproutRound",
            "RoomType.Boss when IsActTwoOrThree(state) && !IsSecondBossFight(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)");
    }

    [Fact]
    public void Milestones2To4GuardFiremarksForgeTokenAndFission()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionNodeMetadata.cs");
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionMapService.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionCombatModifierService.cs");
        var forgeService = ReadRepoText("EZMicroBalanceCode", "Ascension", "ForgeTokenService.cs");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionRewardService.cs");

        AssertSourceContains(metadata, "Might", "Giant", "ForgeArmor", "ConstantHeal");
        AssertSourceContains(
            mapService,
            "ActOneFiremarkedEliteTargetCount = 2",
            "LaterActFiremarkedEliteTargetCount = 3",
            "MinimumFiremarkedEliteFallbackCount = 2",
            "HasHardFiremarkPlacementConflict",
            "IsAfterActOneFirstRestSite",
            "HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point)");

        AssertSourceContains(
            combatService,
            "FindFiremarkHost(combatState)",
            "2m + actIndex",
            "Math.Ceiling(host.MaxHp * 1.3m)",
            "8m + (5m * actIndex)",
            "6m + (4m * actIndex)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>");

        AssertSourceContains(
            rewardService,
            "FiremarkedEliteRewardTargetOptionCount = 4",
            "NormalFissionChancePercent = 25",
            "BannerFissionChancePercent = 35",
            "FiremarkedEliteFissionChancePercent = 40",
            "BossFissionChancePercent = 15",
            "cardRewardOptions.Any(option => option.Card.Enchantment is FissionEnchantment)",
            "card.Type is CardType.Attack or CardType.Skill",
            "!card.EnergyCost.CostsX",
            "!card.HasStarCostX",
            "card.EnergyCost.GetWithModifiers(CostModifiers.None) > 0",
            "!card.Keywords.Contains(CardKeyword.Exhaust)",
            "!card.ExhaustOnNextPlay",
            "card.Enchantment == null");

        AssertSourceContains(
            forgeService,
            "DuplicateTokenGoldAmount",
            "AscensionSavedStateFields.ForgeTokenHeld[player]",
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "await PlayerCmd.GainGold",
            "CardCmd.Upgrade",
            "player.Relics.OfType<ForgeTokenRelic>().ToList()");
        AssertSourceContains(forgeRelic, "ShowCounter => true", "DisplayAmount => 1", "Max [blue]1[/blue].");
    }

    [Fact]
    public void Milestones5To8GuardBannersDeepBranchesBossSealsAndBlockedA20Claims()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionNodeMetadata.cs");
        var mapService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionMapService.cs");
        var mapUiPatches = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionMapUiPatches.cs");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionA20Patches.cs");
        var a20Courtyard = ReadRepoText("EZMicroBalanceCode", "Ascension", "Events", "A20Courtyard.cs");
        var a20RewardScreenPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionA20RewardScreenPatches.cs");
        var combatService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionCombatModifierService.cs");
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionRewardService.cs");
        var bossSealDefinition = ReadRepoText("EZMicroBalanceCode", "Ascension", "BossSealDefinition.cs");
        var englishAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");
        var englishEvents = JsonStringMap("EZMicroBalance", "localization", "eng", "events.json");
        var zhsEvents = JsonStringMap("EZMicroBalance", "localization", "zhs", "events.json");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var currentDocs = ReadCurrentFacingDocs();

        AssertSourceContains(metadata, "Vanguard", "ShieldFormation", "Bounty", "BossSealDefinition?", "DeepBranchNodeKind", "IsBossBrand");
        AssertSourceContains(
            mapService,
            "BannerRoomMapQuestMarker",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "TryInsertDeepBranch",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "runState.Players.Count > 1",
            "BossSealCatalog.TryGetForEncounter",
            "var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);",
            "var dualKingBrandsEnabled = AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState);",
            "if (!bossSealsEnabled && !dualKingBrandsEnabled)",
            "if (bossSealsEnabled)",
            "if (!dualKingBrandsEnabled)",
            "IsBossBrand = true",
            "vanilla boss map icons reveal the boss order");

        AssertSourceContains(
            a20Patch,
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))",
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState)",
            "finalAct.HasSecondBoss",
            "finalAct.SetSecondBossEncounter(secondBoss)",
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))",
            "AscensionA20CourtyardService.ShouldEnterCourtyard(runState)",
            "AscensionA20CourtyardService.EnterCourtyard(__instance, runState)",
            "vanilla double-boss map path");

        AssertSourceContains(
            a20Courtyard,
            "internal sealed class A20Courtyard : EventModel",
            "public override bool IsAllowed(IRunState runState) => false;",
            "A20_COURTYARD.pages.INITIAL.description",
            "ThatWontSaveToChoiceHistory",
            "ModelDb.Event<A20Courtyard>()",
            "EnterRoomWithoutExitingCurrentRoom(eventRoom, fadeToBlack: true)",
            "SaveManager.Instance.SaveRun(eventRoom, saveProgress: false)",
            "HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))",
            "AscensionAssetPaths.BossSealIndicator",
            "BossSealCatalog.GetLocalizationKey(definition.Id)");

        AssertSourceContains(
            a20RewardScreenPatch,
            "HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))",
            "HarmonyPatch(typeof(NRewardsScreen), \"UpdateScreenState\")",
            "IsA20BossOneIntermission",
            "A20_INTERMISSION_HEADER",
            "A20_INTERMISSION_PROCEED",
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState)",
            "TryGetFieldValue",
            "WarnOnce",
            "runState.Map.SecondBossMapPoint != null",
            "runState.CurrentMapCoord == runState.Map.BossMapPoint.coord");

        AssertSourceContains(
            mapUiPatches,
            "HarmonyPatch(typeof(NBossMapPoint), \"OnFocus\")",
            "BossMapPointHoverPatch",
            "BOSS_ROYAL_SEAL",
            "BOSS_KING_BRAND",
            "CreateHoverTip(metadata.BossSeal, metadata.IsBossBrand)",
            "BossSealCatalog.GetLocalizationKey(definition.Id)",
            "sealDescriptionKey = isBossBrand ? \"brand\" : \"summary\"",
            "metadata.IsBossBrand");

        AssertSourceContains(
            combatService,
            "VanguardStrength = 2m",
            "VanguardRemovalRound = 3",
            "ShieldFormationTurnBlock = 5m",
            "BountyGoldReward = 15",
            "PickBannerTarget(combatState)",
            "MinionPower",
            "room.AddExtraReward(player, new GoldReward(BountyGoldReward, player))",
            "metadata.BossSeal != null",
            "metadata.IsBossBrand",
            "HolyDazePower",
            "var triggerCap = metadata.IsBossBrand ? 3 : 2;",
            "var block = metadata.IsBossBrand ? 14m : 12m;",
            "var slippery = metadata.IsBossBrand ? 2m : 1m;",
            "var plating = metadata.IsBossBrand ? 6m : 4m;",
            "var platingAmount = metadata.IsBossBrand ? 10m : 8m;",
            "var divisor = metadata.IsBossBrand ? 3m : 2m;",
            "var steamThreshold = metadata.IsBossBrand ? 10m : 12m;",
            "var blockPerStack = metadata.IsBossBrand ? 1m : 2m;",
            "var block = metadata.IsBossBrand ? 8m : 6m;",
            "var artifact = metadata.IsBossBrand ? 2m : 1m;",
            "var noteCount = metadata.IsBossBrand ? 2 : 1;",
            "tracker.StruggleBaitBrandEscapeAges[escape] = 0;",
            "SettleStruggleBaitBrandEscapes",
            "age >= 2",
            "var block = maturedEscapes.Count * 5m;",
            "TrackBoilingCriticalSteam",
            "RoyalDecreeEnchantment");

        AssertSourceContains(
            bossSealDefinition,
            "BossSealImplementationStatus.SourceGuardedPendingLiveVerification",
            "RuntimeEvidence",
            "HolyDaze",
            "BOSS_SEAL_HOLY_DAZE",
            "BOSS_SEAL_STRUGGLE_BAIT",
            "Trigger cap rises to 3 follower deaths",
            "Restores 2 Slippery",
            "Wake Plating rises to 10",
            "Boiling milestones trigger every 10 Steam",
            "Back-attack Block rises to 8",
            "adds a second Marginal Note",
            "Each unplayed generated Frantic Escape grants 5 Block",
            "ResidualSample");
        Assert.DoesNotContain("Brand parameters are not designed for A20 yet", bossSealDefinition, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "BossRewardTargetOptionCount = 4",
            "TryAddBossSealRewardOption",
            "TryAddA20BossOneCardReward",
            "AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState)",
            "runState.Map.SecondBossMapPoint == null",
            "runState.CurrentMapCoord != runState.Map.BossMapPoint.coord",
            "new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player)");
        Assert.Equal("Banner Room", englishAscension["BANNER_ROOM.title"]);
        Assert.Contains("round [blue]3[/blue]", englishAscension["BANNER_VANGUARD.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]15[/blue] [gold]Gold[/gold]", englishAscension["BANNER_BOUNTY.description"], StringComparison.Ordinal);
        Assert.Equal("Royal Seal", englishAscension["BOSS_ROYAL_SEAL.title"]);
        Assert.Contains("stronger Brand", englishAscension["BOSS_KING_BRAND.description"], StringComparison.Ordinal);
        Assert.Equal("Holy Daze", englishAscension["BOSS_SEAL_HOLY_DAZE.title"]);
        Assert.Contains("3 follower deaths", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("2 Slippery", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("10", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("16", englishAscension["BOSS_SEAL_SOUL_TIDE.brand"], StringComparison.Ordinal);
        Assert.Contains("10 Steam", englishAscension["BOSS_SEAL_BOILING_CRITICAL.brand"], StringComparison.Ordinal);
        Assert.Contains("2 Artifact", englishAscension["BOSS_SEAL_MISALIGNED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("second Marginal Note", englishAscension["BOSS_SEAL_MARGINAL_NOTE.brand"], StringComparison.Ordinal);
        Assert.Contains("5 Block", englishAscension["BOSS_SEAL_STRUGGLE_BAIT.brand"], StringComparison.Ordinal);
        Assert.Contains("4th Attack", englishAscension["BOSS_SEAL_DOOR_WEDGE.brand"], StringComparison.Ordinal);
        Assert.Contains("players 5 Block", englishAscension["BOSS_SEAL_CHOSEN_DECREE.brand"], StringComparison.Ordinal);
        Assert.Contains("2 weakened samples", englishAscension["BOSS_SEAL_RESIDUAL_SAMPLE.brand"], StringComparison.Ordinal);
        foreach (var key in englishAscension.Keys.Where(key => key.StartsWith("BOSS_SEAL_", StringComparison.Ordinal)))
        {
            Assert.True(zhsAscension.ContainsKey(key), $"Missing zhs Boss Seal key: {key}");
        }

        Assert.Equal("Courtyard Ahead", englishAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("Enter the Courtyard", englishAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("\u6218\u65d7\u623f", zhsAscension["BANNER_ROOM.title"]);
        Assert.Equal("\u738b\u5370", zhsAscension["BOSS_ROYAL_SEAL.title"]);
        Assert.Contains("\u66f4\u5f3a\u7684\u70d9\u5370", zhsAscension["BOSS_KING_BRAND.description"], StringComparison.Ordinal);
        Assert.Equal("\u524d\u65b9\u4e2d\u5ead", zhsAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("\u8fdb\u5165\u4e2d\u5ead", zhsAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("Courtyard Before the Second King", englishEvents["A20_COURTYARD.title"]);
        Assert.Contains("{SealSummary}", englishEvents["A20_COURTYARD.pages.INITIAL.description"], StringComparison.Ordinal);
        Assert.Equal("\u7b2c\u4e8c\u738b\u524d\u7684\u4e2d\u5ead", zhsEvents["A20_COURTYARD.title"]);
        Assert.Contains("{SealSummary}", zhsEvents["A20_COURTYARD.pages.READY.description"], StringComparison.Ordinal);

        var hasV2BossSealTable = ReadSourceTree("EZMicroBalanceCode", "Ascension").Contains("BossSealDefinition", StringComparison.Ordinal);
        if (!hasV2BossSealTable)
        {
            Assert.Contains("source-guarded through supported hooks", apiResearch, StringComparison.Ordinal);
            Assert.Contains("Armor/Rage/Barrier/Chaos", apiResearch, StringComparison.Ordinal);
            Assert.Contains("Boss 2 Brand metadata", apiResearch, StringComparison.Ordinal);
        }

        Assert.Contains("A20 creates the final-act second Boss through the vanilla double-boss map path when the A20 gate is active.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 Boss 1 reward screen offers one Boss card reward before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Boss 1 reward screen opens the A20 courtyard event before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"(?i)\bA20\b[^\r\n.]*\b(?:release-ready|fully verified|complete)\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bA11-A20\b[^\r\n.]*\b(?:release-ready|fully verified)\b", currentDocs);
    }

    [ReleaseArtifactFact]
    public void PackageContainsCurrentAscensionLocalization()
    {
        var version = ManifestVersion();
        var package = RepoPath("publish", $"EZMicroBalance-{version}.zip");
        Assert.True(File.Exists(package), $"Missing package zip: {package}");

        using var archive = ZipFile.OpenRead(package);
        var pck = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));

        Assert.Contains("EZMicroBalance/localization/eng/ascension.json", pck);
        Assert.Contains("EZMicroBalance/localization/zhs/ascension.json", pck);
        Assert.Contains("EZMicroBalance/localization/eng/cards.json", pck);
        Assert.Contains("EZMicroBalance/localization/zhs/cards.json", pck);

    }

    [Fact]
    public void CurrentDocsDoNotClaimAscensionReadiness()
    {
        var currentDocs = ReadCurrentFacingDocs();

        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live Ascension gameplay not executed yet", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", currentDocs, StringComparison.OrdinalIgnoreCase);
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

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadCurrentFacingDocs()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "README.md",
                "docs/dev-environment.md",
                "docs/private-beta-verification-handoff.md",
                "docs/test-plan.md",
                "docs/release-checklist.md",
                "docs/features/ascension-11-20/api-research.md",
                "docs/features/ascension-11-20/manual-test-checklist.md"
            }.Select(path => ReadRepoText(path.Split('/'))));
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

    private static string ManifestVersion()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        return document.RootElement.GetProperty("version").GetString() ?? throw new InvalidOperationException("Missing manifest version.");
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
