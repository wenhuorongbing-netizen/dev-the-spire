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
        var gates = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");

        AssertSourceContains(
            config,
            "DisableAllEnvironmentVariable = \"SPIREPLUS_ASCENSION_DISABLE_ALL_SYSTEMS\"",
            "LegacyDisableAllEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_ALL_SYSTEMS\"",
            "EnableRootblightEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_ROOTBLIGHT\"",
            "LegacyEnableRootblightEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_ROOTBLIGHT\"",
            "EnableBlightSproutEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BLIGHT_SPROUT\"",
            "LegacyEnableBlightSproutEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT\"",
            "EnableFiremarkedElitesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FIRE_MARK_ELITES\"",
            "LegacyEnableFiremarkedElitesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES\"",
            "EnableForgeTokenEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FORGE_TOKEN\"",
            "LegacyEnableForgeTokenEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FORGE_TOKEN\"",
            "EnableFissionEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FISSION\"",
            "LegacyEnableFissionEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FISSION\"",
            "EnableBannerRoomsEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BANNER_ROOMS\"",
            "LegacyEnableBannerRoomsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BANNER_ROOMS\"",
            "EnableDeepBranchesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "LegacyEnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "EnableBossSealsEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BOSS_SEALS\"",
            "LegacyEnableBossSealsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BOSS_SEALS\"",
            "EnableBrandedFormEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BRANDED_FORM\"",
            "LegacyEnableBrandedFormEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BRANDED_FORM\"",
            "EnableDualKingBrandsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS\"",
            "EnableBrandedForm => IsEnabled(EnableBrandedFormEnvironmentVariable, LegacyEnableBrandedFormEnvironmentVariable) && IsEnabled(EnableDualKingBrandsEnvironmentVariable)",
            "return false;",
            "rootblight={EnableRootblight}",
            "brandedForm={EnableBrandedForm}");

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
            "IsBrandedFormEnabled",
            "IsBrandedFormSinglePlayerEnabled",
            "runState.Players.Count == 1");

        AssertSourceContains(
            initializer,
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||",
            "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) &&",
            "!AscensionFeatureGate.IsDiagnosticsEnabled",
            "ShouldDisableUnverifiedCoopCombatHook");
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
            "RoomType.Boss when IsActTwoOrThree(state)",
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
            "RoomType.Boss when IsActTwoOrThree(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "return state.RunState.CurrentActIndex is 1 or 2;",
            "currentRow >= 3",
            "bud.HasEnteredHand && !bud.WasPlayed",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")");

        Assert.Contains("first time this occurs", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("if never drawn, it withers away.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("首次发生该恶化", zhsCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("若本场从未抽到，则会枯萎消逝", zhsCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

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
            "GetMightOverflowStrength(combatState)",
            "GetGiantFiremarkMaxHpPercent(combatState)",
            "GetGiantOverflowDamage(combatState)",
            "GetForgeArmorBlock(combatState)",
            "GetForgeArmorOverflowBlock(combatState)",
            "GetConstantHealAmount(combatState)",
            "GetConstantHealOverflowHeal(combatState)",
            "AddFiremarkHeat(host, tracker)",
            "ApplyMightOverflow(combatState, tracker)",
            "TrackMoltenCoreDamage(combatState, tracker, host",
            "ApplyGiantOverflowDamage(combatState, tracker, host)",
            "ResolveForgeArmorShatter(tracker)",
            "ApplyForgeArmorOverflow(combatState, tracker)",
            "ResolveConstantHeal(combatState, tracker)",
            "ApplyConstantHealOverflow(combatState, tracker)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>",
            "PowerCmd.Apply<FiremarkMightOverflowPower>",
            "FiremarkOverflowCandidates(combatState, tracker)");

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
        var marginalNoteSource = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MarginalNote.cs");
        var aeonglassIntentPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AeonglassIntentPatches.cs");
        var powers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var bossSealSource = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var attackIntentSource = ReadRepoText("source code", "src", "Core", "MonsterMoves", "Intents", "AttackIntent.cs");
        var kinBossSource = ReadRepoText("source code", "src", "Core", "Models", "Encounters", "TheKinBoss.cs");
        var kinPriestSource = ReadRepoText("source code", "src", "Core", "Models", "Monsters", "KinPriest.cs");
        var slipperyPowerSource = ReadRepoText("source code", "src", "Core", "Models", "Powers", "SlipperyPower.cs");
        var platingPowerSource = ReadRepoText("source code", "src", "Core", "Models", "Powers", "PlatingPower.cs");
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
            "var brandedFormEnabled = AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState);",
            "if (!bossSealsEnabled && !brandedFormEnabled)",
            "if (bossSealsEnabled)",
            "if (!brandedFormEnabled)",
            "IsBossBrand = true",
            "vanilla boss map icons reveal the boss order");

        AssertSourceContains(
            a20Patch,
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
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
            "GetSecondBossBrandIconPath(runState)",
            "AscensionAssetPaths.GetBossSealIndicator(definition.Id)",
            "PreloadManager.Cache.GetTexture2D(A20Courtyard.GetSecondBossBrandIconPath(__instance.Owner?.RunState))",
            "BossSealCatalog.GetLocalizationKey(definition.Id)");

        AssertSourceContains(
            a20RewardScreenPatch,
            "HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))",
            "HarmonyPatch(typeof(NRewardsScreen), \"UpdateScreenState\")",
            "IsA20BossOneIntermission",
            "A20_INTERMISSION_HEADER",
            "A20_INTERMISSION_PROCEED",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "TryGetFieldValue",
            "WarnOnce",
            "runState.Map.SecondBossMapPoint != null",
            "runState.CurrentMapCoord == runState.Map.BossMapPoint.coord");

        AssertSourceContains(
            mapUiPatches,
            "HarmonyPatch(typeof(NBossMapPoint), \"OnFocus\")",
            "BossMapPointHoverPatch",
            "BOSS_DEDICATED_ABILITY",
            "BOSS_BRANDED_FORM",
            "CreateHoverTip(metadata.BossSeal, metadata.IsBossBrand)",
            "BossSealCatalog.GetLocalizationKey(definition.Id)",
            "PreloadManager.Cache.GetTexture2D(AscensionAssetPaths.GetBossSealIndicator(definition.Id))",
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
            "const int triggerCap = 2;",
            "var strikeDamage = metadata.IsBossBrand ? 4m : 3m;",
            "PowerCmd.Apply<MartyrOathPower>",
            "PowerCmd.Apply<MartyrOathStrikePower>",
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(priest, 1, priest, null)",
            "CalculateInkReturnRestoreAmount",
            "tracker.InkReturnLastObservedSlippery",
            "tracker.InkReturnRestoreAmount",
            "ApplyPowerWithFinalDisplayedGain<SlipperyPower>(vantom, slippery, vantom, null)",
            "StartledShellWakeByPlayerDamagePending",
            "wokeFromPlayerDamage",
            "metadata.IsBossBrand ? 6 : 4",
            "metadata.IsBossBrand ? 10 : 8",
            "PowerCmd.Apply<PlatingPower>",
            "var divisor = metadata.IsBossBrand ? 3m : 2m;",
            "await ApplyBoilingExplosionFortification(combatState, tracker, metadata)",
            "await ApplyBoilingExplosionVulnerability(combatState, tracker, metadata, giant)",
            "metadata.IsBossBrand ? 2m : 1m",
            "PowerCmd.Apply<VulnerablePower>",
            "tracker.BoilingExplosionVulnerabilityRound = combatState.RoundNumber",
            "giant.GetPower<WeakPower>()",
            "PowerCmd.Remove(weak)",
            "strength is { Amount: < 0 }",
            "ApplySoulTidePendingBlock",
            "tracker.PendingSoulTideBlock",
            "SoulTideBlockCap",
            "CreatureCmd.GainBlock",
            "var threshold = metadata.IsBossBrand ? 0.30m : 0.35m;",
            "PowerCmd.Apply<KaiserCalibrationStrikePower>",
            "var roundRoom = Math.Max(0, 2 - tracker.MarginalDeepThoughtAddedThisRound)",
            "PowerCmd.Apply<DeepThoughtPower>",
            "tracker.StruggleBaitGeneratedEscapes.Add(escape)",
            "TrackRoyalEscapePlayed",
            "PowerCmd.Apply<VigorPower>",
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2",
            "TrackAeonglassEnergySpent",
            "tracker.AeonglassExtraWitherFromSands",
            "INCREASING_INTENSITY_MOVE",
            "CardPileCmd.AddToCombatAndPreview<Wither>",
            "PowerCmd.Apply<AeonglassLaserEchoPower>",
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
            "RoyalDecreeEnchantment",
            "TrackAeonglassEnemyMove",
            "SettleAeonglassTimeSand",
            "ApplyAeonglassTimeSandAfterEbb");
        AssertSourceContains(
            kinBossSource,
            "new string[3] { \"slot1\", \"slot2\", \"leaderSlot\" }",
            "(kinFollower, \"slot1\")",
            "(ModelDb.Monster<KinFollower>().ToMutable(), \"slot2\")",
            "(ModelDb.Monster<KinPriest>().ToMutable(), \"leaderSlot\")");
        Assert.DoesNotContain("Summon", kinBossSource + kinPriestSource, StringComparison.OrdinalIgnoreCase);
        AssertSourceContains(
            slipperyPowerSource,
            "public override bool ShouldScaleInMultiplayer => true",
            "return amount * (decimal)combatState.Players.Count");
        AssertSourceContains(
            platingPowerSource,
            "public override bool ShouldScaleInMultiplayer => true",
            "base.DynamicVars[\"Decrement\"].BaseValue = base.Owner.CombatState.RunState.Players.Count",
            "return (decimal)((combatState.Players.Count - 1) * 2 + 1) * amount");
        Assert.DoesNotContain("triggerCap = metadata.IsBossBrand ? 3 : 2", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("triggers up to [blue]3[/blue] times", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("SettleSoulTideBeckons(combatState, tracker, metadata)", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("PowerCmd.Apply<StrengthPower>", marginalNoteSource, StringComparison.Ordinal);
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
        Assert.Contains("[gold]Intangible[/gold]", powers, StringComparison.Ordinal);
        Assert.Contains("[gold]无形[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("equal [gold]Block[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("等量[gold]格挡[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -Amount", powers, StringComparison.Ordinal);
        AssertSourceContains(
            attackIntentSource,
            "Hook.ModifyDamage(",
            "ValueProp.Move",
            "ModifyDamageHookType.All");
        AssertSourceContains(
            aeonglassIntentPatch,
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))",
            "__instance.Repeats + 1",
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))");

        AssertSourceContains(
            bossSealSource,
            "BossSealImplementationStatus.SourceGuardedPendingLiveVerification",
            "RuntimeEvidence",
            "HolyDaze",
            "BOSS_SEAL_HOLY_DAZE",
            "BOSS_SEAL_STRUGGLE_BAIT",
            "source-confirmed two KinFollower deaths",
            "Restores 35% of the cleared Slippery",
            "natural wake grants 10",
            "clear Weak and attack reduction",
            "claws' HP percentages differ",
            "Unplayed Notes become Deep Thought",
            "Every 3 ability-made Frantic Escapes played gives 3 Vigor",
            "Time Sand Reflow",
            "ResidualSample");
        Assert.DoesNotContain("Brand parameters are not designed for A20 yet", bossSealSource, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "BossRewardTargetOptionCount = 4",
            "TryAddBossSealRewardOption",
            "TryAddA20BossOneCardReward",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "runState.Map.SecondBossMapPoint == null",
            "runState.CurrentMapCoord != runState.Map.BossMapPoint.coord",
            "new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player)");
        Assert.Equal("Banner Room", englishAscension["BANNER_ROOM.title"]);
        Assert.Contains("round [blue]3[/blue]", englishAscension["BANNER_VANGUARD.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]{Gold}[/blue] [gold]Gold[/gold]", englishAscension["BANNER_BLOOD_PRIZE.description"], StringComparison.Ordinal);
        Assert.Equal("Boss Dedicated Ability", englishAscension["BOSS_DEDICATED_ABILITY.title"]);
        Assert.Equal("Boss Dedicated Abilities", englishAscension["LEVEL_19.title"]);
        Assert.Equal("Branded Form", englishAscension["LEVEL_20.title"]);
        Assert.Contains("Attack changes from this ability are shown in intent", englishAscension["BOSS_DEDICATED_ABILITY.description"], StringComparison.Ordinal);
        Assert.Equal("Branded Form", englishAscension["BOSS_BRANDED_FORM.title"]);
        Assert.Contains("second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]", englishAscension["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
        Assert.Equal("Holy Daze", englishAscension["BOSS_SEAL_HOLY_DAZE.title"]);
        Assert.Contains("capped at [blue]2[/blue]", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("+[blue]4[/blue] damage per Oath", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]1[/blue] [gold]Artifact[/gold]", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]35%[/blue]", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("max [blue]18[/blue]", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]10[/blue]", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("one-third", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("Team cap: solo [blue]12[/blue], 2 players [blue]16[/blue], 3-4 players [blue]20[/blue]", englishAscension["BOSS_SEAL_SOUL_TIDE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] turns of [gold]Vulnerable[/gold]", englishAscension["BOSS_SEAL_BOILING_CRITICAL.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]30%[/blue] HP difference", englishAscension["BOSS_SEAL_MISALIGNED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("Deep Thought", englishAscension["BOSS_SEAL_MARGINAL_NOTE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]3[/blue] [gold]Vigor[/gold]", englishAscension["BOSS_SEAL_STRUGGLE_BAIT.brand"], StringComparison.Ordinal);
        Assert.Contains("Eye Lasers", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]3[/blue] Time Sand", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("extra [gold]Wither[/gold]", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("Playing the Decree has no extra penalty", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("non-Decree Bound card gives Queen [blue]1[/blue] [gold]Majesty[/gold]", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("Play it for player Block", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("Majesty cap becomes [blue]3[/blue]", englishAscension["BOSS_SEAL_CHOSEN_DECREE.brand"], StringComparison.Ordinal);
        Assert.Contains("\u6253\u51fa\u5fa1\u4ee4\u724c\u4e0d\u4f1a\u89e6\u53d1\u989d\u5916\u60e9\u7f5a", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u4ee4", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u6253\u51fa\u5b83\u83b7\u5f97\u683c\u6321", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] different samples", englishAscension["BOSS_SEAL_RESIDUAL_SAMPLE.brand"], StringComparison.Ordinal);
        foreach (var key in englishAscension.Keys.Where(key => key.StartsWith("BOSS_SEAL_", StringComparison.Ordinal)))
        {
            Assert.True(zhsAscension.ContainsKey(key), $"Missing zhs Boss Seal key: {key}");
        }

        Assert.Equal("Courtyard Ahead", englishAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("Enter the Courtyard", englishAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("\u6218\u65d7\u623f", zhsAscension["BANNER_ROOM.title"]);
        Assert.Equal("\u9996\u9886\u4e13\u5c5e\u80fd\u529b", zhsAscension["BOSS_DEDICATED_ABILITY.title"]);
        Assert.Equal("\u70d9\u5370\u5f62\u6001", zhsAscension["BOSS_BRANDED_FORM.title"]);
        Assert.Contains("\u7b2c[blue]3[/blue]\u5e55\u7b2c\u4e8c\u540d\u9996\u9886\u8fdb\u5165[gold]\u70d9\u5370\u5f62\u6001[/gold]", zhsAscension["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
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
            Assert.Contains("Boss 2 Branded Form metadata", apiResearch, StringComparison.Ordinal);
        }

        Assert.Contains("A20 creates the final-act second Boss through the vanilla double-boss map path when the A20 gate is active.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 Boss 1 reward screen offers one Boss card reward before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Boss 1 reward screen opens the A20 courtyard event before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seal / King Brand", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seal / 王印", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand / 王烙印", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"(?i)\bA20\b[^\r\n.]*\b(?:release-ready|fully verified|complete)\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bA11-A20\b[^\r\n.]*\b(?:release-ready|fully verified)\b", currentDocs);
    }

    [Fact]
    public void SplitBossSealPowerFilesKeepBehaviorAndReadableLocalization()
    {
        var basePower = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BossSealPowers.cs");
        var combatStart = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.CombatStart.cs");
        var holyDaze = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "HolyDazePower.cs");
        var boilingCritical = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BoilingCriticalPower.cs");
        var residualSample = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "ResidualSamplePower.cs");
        var aeonglassHourglass = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "AeonglassHourglassPower.cs");

        AssertSourceContains(
            basePower,
            "internal abstract class BossSealPower : CustomPowerModel, ILocalizationProvider",
            "public override PowerType Type => PowerType.Buff",
            "public override PowerStackType StackType => PowerStackType.Single",
            "public override int DisplayAmount => Amount",
            "protected virtual BossSealId? SealId => null",
            "AscensionAssetPaths.GetBossSealIndicator(id)",
            "public override string CustomPackedIconPath => BossSealIconPath",
            "public override string CustomBigIconPath => BossSealIconPath",
            "AscensionAssetPaths.BossSealIndicator");
        AssertSourceContains(
            basePower,
            "internal abstract class BossSealMarkerPower : BossSealPower",
            "public override int DisplayAmount => 0",
            "HolyDazeBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.HolyDaze",
            "MartyrOathBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MartyrOath",
            "InkReturnBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.InkReturn",
            "StartledShellBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.StartledShell",
            "SoulTideBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.SoulTide",
            "BoilingCriticalBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.BoilingCritical",
            "MisalignedShellBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MisalignedShell",
            "MarginalNoteBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MarginalNote",
            "StruggleBaitBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.StruggleBait",
            "AeonglassHourglassBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.AeonglassHourglass",
            "ChosenDecreeBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.ChosenDecree",
            "ResidualSampleBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.ResidualSample",
            "Dedicated Ability");
        AssertSourceContains(
            combatStart,
            "await ApplyBossSealVisibilityMarker(combatState, definition)",
            "FindBossSealVisibilityOwner(combatState, definition.Id)",
            "PowerCmd.Apply<HolyDazeBossSealMarkerPower>",
            "PowerCmd.Apply<MartyrOathBossSealMarkerPower>",
            "PowerCmd.Apply<InkReturnBossSealMarkerPower>",
            "PowerCmd.Apply<StartledShellBossSealMarkerPower>",
            "PowerCmd.Apply<SoulTideBossSealMarkerPower>",
            "PowerCmd.Apply<BoilingCriticalBossSealMarkerPower>",
            "PowerCmd.Apply<MisalignedShellBossSealMarkerPower>",
            "PowerCmd.Apply<MarginalNoteBossSealMarkerPower>",
            "PowerCmd.Apply<StruggleBaitBossSealMarkerPower>",
            "PowerCmd.Apply<AeonglassHourglassBossSealMarkerPower>",
            "PowerCmd.Apply<ChosenDecreeBossSealMarkerPower>",
            "PowerCmd.Apply<ResidualSampleBossSealMarkerPower>");
        Assert.DoesNotContain("internal sealed class HolyDazePower", basePower, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class BoilingCriticalPower", basePower, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class ResidualSamplePower", basePower, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class ChosenDecreeReductionPower", basePower, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class AeonglassHourglassPower", basePower, StringComparison.Ordinal);

        AssertSourceContains(
            holyDaze,
            "internal sealed class HolyDazePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.HolyDaze",
            "PowerStackType.Counter",
            "\"圣昏\"",
            "每次受到的伤害最多为[blue]1[/blue]",
            "受击最多[blue]1[/blue]点",
            "\"Holy Daze\"",
            "damage taken from each hit is capped at [blue]1[/blue]",
            "Damage taken is capped at [blue]1[/blue].",
            "ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)",
            "return target == Owner ? 1m : decimal.MaxValue;");

        AssertSourceContains(
            boilingCritical,
            "internal sealed class BoilingCriticalPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.BoilingCritical",
            "\"不可削弱\"",
            "爆发回合",
            "[gold]虚弱[/gold]",
            "[gold]易伤[/gold]",
            "[gold]人工制品[/gold]",
            "\"Unweakenable\"",
            "On the explosion turn",
            "[gold]Weak[/gold]",
            "[gold]Vulnerable[/gold]",
            "[gold]Artifact[/gold]",
            "public override int DisplayAmount => 0");

        AssertSourceContains(
            residualSample,
            "internal sealed class ResidualSamplePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.ResidualSample",
            "\"实验记录\"",
            "下个阶段会保留[blue]{Amount}[/blue]份[gold]残留样本[/gold]",
            "复苏后结算残留样本",
            "\"Experimental Record\"",
            "The next phase keeps [blue]{Amount}[/blue] [gold]Residual Sample[/gold]",
            "Residual samples resolve after respawn.",
            "ShouldPowerBeRemovedAfterOwnerDeath()",
            "return false;");

        AssertSourceContains(
            aeonglassHourglass,
            "internal sealed class AeonglassHourglassPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.AeonglassHourglass",
            "\"时砂回流\"",
            "剩余[blue]{Amount}[/blue]枚时砂",
            "每花费[blue]1[/blue]点能量",
            "\"Time Sand Reflow\"",
            "[blue]{Amount}[/blue] Time Sand remaining",
            "Each energy spent removes [blue]1[/blue]",
            "Eye Lasers hits [blue]1[/blue] extra time");

        foreach (var source in new[] { basePower, holyDaze, boilingCritical, residualSample, aeonglassHourglass })
        {
            AssertNoMojibake(source, "鐏", "鎴", "绗", "鍥", "浼", "澶", "銆", "闂", "鏈", "寮€", "鑾", "缂", "锟", "铏", "鐑", "杈", "绉", "灞");
        }
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
        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live Ascension gameplay not executed yet", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", currentDocs, StringComparison.OrdinalIgnoreCase);
    }


}
