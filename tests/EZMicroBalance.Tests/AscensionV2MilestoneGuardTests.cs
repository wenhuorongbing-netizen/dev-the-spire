using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AscensionV2MilestoneGuardTests
{
    private static readonly string[] CurrentFacingDocs =
    [
        "README.md",
        "docs/dev-environment.md",
        "docs/private-beta-verification-handoff.md",
        "docs/test-plan.md",
        "docs/release-checklist.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    [Fact]
    public void CombatModifierEntryPointsShareNodeMetadataRefreshHelpers()
    {
        var combatModifiers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");

        AssertSourceContains(
            combatModifiers,
            "private static bool TryRefreshNodeMetadata(",
            "tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState)",
            "tracker.NodeMetadata = current",
            "private static bool TryRefreshActiveBossSealMetadata(",
            "TryRefreshNodeMetadata(combatState, tracker, out metadata) &&",
            "HasActiveBossSeal(combatState, metadata)");
        Assert.DoesNotContain(
            "var metadata = tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState);",
            combatModifiers,
            StringComparison.Ordinal);

        var metadataLookupCount = Regex.Matches(
            combatModifiers,
            @"tracker\.NodeMetadata \?\? AscensionMapService\.TryGetCurrentMetadata\(combatState\.RunState\)",
            RegexOptions.CultureInvariant).Count;
        Assert.Equal(1, metadataLookupCount);
    }

    [Fact]
    public void Milestone0FeatureFlagsAreIndependentAndAllOffIsANoOp()
    {
        var config = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var gates = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureGate.cs");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");

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
        var cardsSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootBudCard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootFamilyCard.cs"));
        var deckService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatHook = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Equal("Rootblight I", englishCards["EZMB_ROOT.title"]);
        Assert.Equal("Rootblight II", englishCards["EZMB_DEEP_ROOT.title"]);
        Assert.Equal("Rootblight III", englishCards["EZMB_ROOTBLIGHT_III.title"]);
        Assert.Equal("Blight Sprout", englishCards["EZMB_ROOT_BUD.title"]);
        Assert.Contains("If seen and not played, add a [gold]Rootblight I[/gold] after combat.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("If never seen, it withers.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("No Rootblight IV.", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("your deck has no Rootblight", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

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
            "SavedSpireField<Player, string> RootblightPendingCombatDowngrades",
            "EZMicroBalanceAscensionRootblightPendingCombatDowngrades",
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
            "TrimRootblightDeckToCap(player",
            "FindRootFamilyCards(player)",
            "MarkCombatStartRootblight",
            "TryFindRootblightDeckVersion(player, card)",
            "had no unique master-deck card",
            "matchingLevel.Count == 1",
            "matchingSplitState.Count == 1 ? matchingSplitState[0] : null",
            "QueuePendingCombatDowngrade(player, downgradedLevel, splitState)",
            "ReadPendingCombatDowngrades(player)",
            "WritePendingCombatDowngrades(player, pending)",
            "ClearPendingCombatDowngrades(player)",
            "card.RootblightLevel - 1",
            "new RootblightCardToAdd(level, parts[1] == \"1\")",
            "rootFamilyCard.HasSplit = hasSplit",
            "if (!card.HasSplit)",
            "card.HasSplit = true;",
            "ignored Rootblight III split once",
            "ignored Rootblight III already split once; no Rootblight IV",
            "await AddRootblightCard(player, 1, preferOverlayNotice: true)",
            "ThenBy(entry => entry.Index)",
            "ShowRootSystemFull(player)",
            "RemoveHighestRootblight",
            "await CardPileCmd.RemoveFromDeck(card, showPreview: false)",
            "CreateRootblightCard(player, level)");

        AssertSourceContains(
            combatHook,
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)",
            "await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top)",
            "SproutDueBudsBeforeHandDraw(state, player)",
            "AfterCardChangedPiles(CardModel card, PileType oldPileType",
            "AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)",
            "await AscensionCombatModifierService.AfterCardEnteredHand(state, tracker, card)",
            "await AscensionCombatModifierService.AfterCardPlayed(state, tracker, cardPlay)",
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
    public void RootblightAndBlightSproutV22StateMachineIsSourceGuarded()
    {
        var deckService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatHook = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            deckService,
            "MaxRootblightCards = 4",
            "card.WasPresentAtCombatStart = false;",
            "if (!card.HasSplit)",
            "card.HasSplit = true;",
            "ignored Rootblight III split once",
            "ignored Rootblight III already split once; no Rootblight IV",
            "RootblightPendingCombatDowngrades[player]",
            "QueuePendingCombatDowngrade(player, downgradedLevel, splitState)",
            "ReadPendingCombatDowngrades(player)",
            "await TrimRootblightDeckToCap(player, \"pre-add cap check\")",
            "OrderByDescending(entry => entry.Card.RootblightLevel)",
            "ThenBy(entry => entry.Index)",
            "kept {MaxRootblightCards} highest/oldest Rootblight card(s)",
            "Rootblight removed through a deck-removal API",
            "remaining Rootblight cards are preserved");

        AssertSourceContains(
            combatHook,
            "return state.RunState.CurrentRoom?.RoomType == RoomType.Boss",
            "? 2",
            ": 1",
            "NormalizeExistingRootBudRounds(state, existingBuds)",
            "for (var i = 0; i < existingBuds.Count; i++)",
            "existingBuds[i].SproutRound = targetRounds[i]",
            "RootBud.BossSecondSproutRound",
            "RoomType.Boss when IsActTwoOrThree(state) && !IsSecondBossFight(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "return state.RunState.CurrentActIndex is 1 or 2;",
            "currentRow >= 3",
            "bud.HasEnteredHand && !bud.WasPlayed",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")");

        Assert.Contains("No Rootblight IV.", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("If never seen, it withers.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("没有第四阶段根蚀", zhsCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("若从未见到，则枯萎", zhsCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        Assert.Contains("Rootblight IV never appears.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("If the four-card cap blocks a Rootblight III split, the failed add does not consume that card's split marker", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight cards added during combat-end resolution do not grow again until the next combat.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("If Blight Sprout enters hand and is discarded or exhausted by a non-play effect, it still adds Rootblight I after combat.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Act 2 elites in the first 3 route rows do not add Blight Sprout.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("MaxRootblightCards = 1", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("NormalizeRootblightDeck", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("downgradedLevel == MaxRootblightLevel && splitState", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("targetRounds.Contains(bud.SproutRound)", combatHook, StringComparison.Ordinal);
        Assert.DoesNotContain("your deck has no Rootblight", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void Milestones2To4GuardFiremarksForgeTokenAndFission()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var forgeService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");

        AssertSourceContains(metadata, "Might", "Giant", "ForgeArmor", "ConstantHeal");
        AssertSourceContains(
            mapService,
            "ActOneFiremarkedEliteTargetCount = 2",
            "LaterActFiremarkedEliteTargetCount = 3",
            "MinimumFiremarkedEliteFallbackCount = 2",
            "HasHardFiremarkPlacementConflict",
            "IsAfterActOneFirstRestSite",
            "HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point)",
            "KeepsFiremarksOptional(start, boss, selected, point)");

        AssertSourceContains(
            combatService,
            "FindFiremarkHost(combatState)",
            "tracker.FiremarkHost = host",
            "GetMightFiremarkStrength(combatState)",
            "GetGiantFiremarkMaxHpPercent(combatState)",
            "GetForgeArmorBlock(combatState)",
            "GetConstantHealAmount(combatState)",
            "AddFiremarkHeat(host, tracker)",
            "TrackMoltenCoreDamage(combatState, tracker, host",
            "ResolveForgeArmorShatter(tracker)",
            "ResolveConstantHeal(combatState, tracker)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>");

        AssertSourceContains(
            rewardService,
            "FiremarkedEliteRewardTargetOptionCount = 4",
            "var duplicateTokenReward = ForgeTokenService.HasToken(player)",
            "Where(card => !duplicateTokenReward || card.IsUpgradable)",
            "CardCmd.Upgrade(extraCard)",
            "NormalFissionChancePercent = 10",
            "BannerFissionChancePercent = 15",
            "FiremarkedEliteFissionChancePercent = 20",
            "BossFissionChancePercent = 5",
            "cardRewardOptions.Any(option => option.Card.Enchantment is FissionEnchantment)",
            "card.Type is CardType.Attack or CardType.Skill",
            "IsFissionEligibleRarity(card.Rarity)",
            "!card.EnergyCost.CostsX",
            "!card.HasStarCostX",
            "card.EnergyCost.Canonical > 0",
            "card.EnergyCost.GetWithModifiers(CostModifiers.None) > 0",
            "!card.Keywords.Contains(CardKeyword.Exhaust)",
            "!card.ExhaustOnNextPlay",
            "card.Enchantment == null");
        Assert.Contains("var modifiedCard = player.RunState.CloneCard(candidate.Card)", rewardService, StringComparison.Ordinal);
        Assert.DoesNotContain("Where(option => !option.HasBeenModified)", rewardService, StringComparison.Ordinal);

        AssertSourceContains(
            forgeService,
            "DuplicateTokenGoldAmount",
            "internal static bool HasToken(Player player)",
            "AscensionSavedStateFields.ForgeTokenHeld[player]",
            "SpecialRestSiteActionPayoutEnabled = false",
            "SpecialRestSiteHealAmount = 5m",
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
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var mapUiPatches = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20Patches.cs");
        var a20Courtyard = ReadRepoText("EZMicroBalanceCode", "Ascension", "Events", "A20Courtyard.cs");
        var a20RewardScreenPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20RewardScreenPatches.cs");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var powers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var bossSealDefinition = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "BossSealDefinition.cs");
        var englishAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");
        var englishEvents = JsonStringMap("EZMicroBalance", "localization", "eng", "events.json");
        var zhsEvents = JsonStringMap("EZMicroBalance", "localization", "zhs", "events.json");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        AssertSourceContains(metadata, "Vanguard", "Shieldwall", "BloodPrize", "PressingLine", "LastStand", "BossSealDefinition?", "DeepBranchNodeKind", "IsBossBrand");
        AssertSourceContains(
            mapService,
            "BannerRoomMapQuestMarker",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "TryInsertDeepBranch",
            "EnumerateDeepBranchColumns(map)",
            "TryMatchExistingDeepBranch",
            "IsDeepBranchRouteSafe(saved, plan)",
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
            "GetVanguardStrength(combatState)",
            "VanguardRemovalRound = 3",
            "GetShieldwallTurnBlock(combatState)",
            "GetBloodPrizeGoldReward(combatState)",
            "GetPressingLinePartialBlock(combatState)",
            "GetLastStandBlock(combatState)",
            "PickBannerTarget(combatState)",
            "MinionPower",
            "room.AddExtraReward(player, new GoldReward(playerReward, player))",
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
            "metadata.Banner == BannerKind.BloodPrize",
            "ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: true)",
            "ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: false)",
            "AscensionPowerAmountHelper.RemoveTemporaryStrength(enemy, power.Amount)",
            "public static Task BeforeFlush(CombatState combatState, AscensionCombatTracker tracker, Player player)",
            "TrackSoulTideBeckonsBeforeFlush(combatState, tracker, metadata, player)",
            "tracker.SoulTideBeckonSettlementRound != combatState.RoundNumber",
            "combatState.RoundNumber < BountyDeadlineRound",
            "!includeCurrentRound && combatState.RoundNumber <= BountyDeadlineRound",
            "TrackBoilingCriticalSteam",
            "RoyalDecreeEnchantment");
        Assert.DoesNotContain("SettleSoulTideBeckons(combatState, tracker, metadata)", combatService, StringComparison.Ordinal);
        var playerTurnStartBannerSlice = SliceBetween(
            combatService,
            "private static async Task ApplyBannerTurnStart(",
            "private static async Task AfterBannerEnemyHpChanged(");
        Assert.DoesNotContain("case BannerKind.Shieldwall", playerTurnStartBannerSlice, StringComparison.Ordinal);
        Assert.Contains("case BannerKind.BloodPrize", playerTurnStartBannerSlice, StringComparison.Ordinal);
        Assert.Contains("includeCurrentRound: false", playerTurnStartBannerSlice, StringComparison.Ordinal);

        AssertSourceContains(
            powers,
            "internal static class AscensionPowerAmountHelper",
            "strength.SetAmount(strength.Amount - (int)amount, silent: true)",
            "AscensionPowerAmountHelper.RemoveTemporaryStrength(Owner, Amount)");
        Assert.Contains("warning [gold]Block[/gold]", powers, StringComparison.Ordinal);
        Assert.Contains("预警[gold]格挡[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("equal [gold]Block[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("等量[gold]格挡[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -Amount", powers, StringComparison.Ordinal);

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
        Assert.Contains("[blue]15[/blue]/[blue]30[/blue]/[blue]55[/blue] [gold]Gold[/gold]", englishAscension["BANNER_BLOOD_PRIZE.description"], StringComparison.Ordinal);
        Assert.Equal("Royal Seal", englishAscension["BOSS_ROYAL_SEAL.title"]);
        Assert.Contains("stronger [gold]King Brand[/gold]", englishAscension["BOSS_KING_BRAND.description"], StringComparison.Ordinal);
        Assert.Equal("Holy Daze", englishAscension["BOSS_SEAL_HOLY_DAZE.title"]);
        Assert.Contains("[blue]3[/blue] follower deaths", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] [gold]Slippery[/gold]", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]10[/blue]", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]16[/blue]", englishAscension["BOSS_SEAL_SOUL_TIDE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]10[/blue] [gold]Steam[/gold]", englishAscension["BOSS_SEAL_BOILING_CRITICAL.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] [gold]Artifact[/gold]", englishAscension["BOSS_SEAL_MISALIGNED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("second [gold]Marginal Note[/gold]", englishAscension["BOSS_SEAL_MARGINAL_NOTE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]5[/blue] [gold]Block[/gold]", englishAscension["BOSS_SEAL_STRUGGLE_BAIT.brand"], StringComparison.Ordinal);
        Assert.Contains("+[blue]5[/blue] [gold]Strength[/gold]", englishAscension["BOSS_SEAL_AEONGLASS_STRENGTH.brand"], StringComparison.Ordinal);
        Assert.Contains("weaken the Queen's next Strength buff", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("Play it for player Block", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("players [blue]5[/blue] [gold]Block[/gold]", englishAscension["BOSS_SEAL_CHOSEN_DECREE.brand"], StringComparison.Ordinal);
        Assert.Contains("削弱女王下一次力量强化", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("打出它获得格挡", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] [gold]weakened samples[/gold]", englishAscension["BOSS_SEAL_RESIDUAL_SAMPLE.brand"], StringComparison.Ordinal);
        foreach (var key in englishAscension.Keys.Where(key => key.StartsWith("BOSS_SEAL_", StringComparison.Ordinal)))
        {
            Assert.True(zhsAscension.ContainsKey(key), $"Missing zhs Boss Seal key: {key}");
        }

        Assert.Equal("Courtyard Ahead", englishAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("Enter the Courtyard", englishAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("\u6218\u65d7\u623f", zhsAscension["BANNER_ROOM.title"]);
        Assert.Equal("\u738b\u5370", zhsAscension["BOSS_ROYAL_SEAL.title"]);
        Assert.Contains("\u66f4\u5f3a\u7684[gold]\u738b\u70d9\u5370[/gold]", zhsAscension["BOSS_KING_BRAND.description"], StringComparison.Ordinal);
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
        var package = RepoPath("publish", $"SpirePlus-{version}.zip");
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
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live Ascension gameplay not executed yet", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", currentDocs, StringComparison.OrdinalIgnoreCase);
    }


}
