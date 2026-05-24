using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class UrdaReleaseCoverageGuardTests
{
    [Fact]
    public void UrdaIsDefaultOnDisableableAndBlessingSliceSourceBacked()
    {
        var urdaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureGate.cs");
        var urdaAncient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingIds.cs");
        var urdaCards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaCards.cs");
        var urdaInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaInitializer.cs");
        var urdaMapUiPatches = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapClickPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapPreviewVisuals.cs"),
            ReadRepoText("EZMicroBalanceCode", "Map", "SpirePlusMapPointHoverComposer.cs"));
        var urdaOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelics.cs");
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var urdaAfterRain = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.AfterRain.cs");
        var urdaHumusPact = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.HumusPact.cs");
        var urdaRouteRewards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RouteRewards.cs");
        var urdaShallowRootRelic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.ShallowRootRelic.cs");
        var urdaTrialBranch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranch.cs");
        var urdaTrialBranchDisplay = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchDisplay.cs");
        var urdaTrialBranchOffer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchOffer.cs");
        var urdaTrialBranchEnchantment = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaTrialBranchEnchantment.cs");
        var urdaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaScene = ReadRepoText("EZMicroBalance", "scenes", "events", "background_scenes", "ezmb_urda.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engCards = ReadRepoText("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = ReadRepoText("EZMicroBalance", "localization", "zhs", "cards.json");
        var engCardRewardUi = ReadRepoText("EZMicroBalance", "localization", "eng", "card_reward_ui.json");
        var zhsCardRewardUi = ReadRepoText("EZMicroBalance", "localization", "zhs", "card_reward_ui.json");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engCardRewardUiMap = JsonStringMap("EZMicroBalance", "localization", "eng", "card_reward_ui.json");
        var zhsCardRewardUiMap = JsonStringMap("EZMicroBalance", "localization", "zhs", "card_reward_ui.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.Contains("ForceAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyForceAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("DisableAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyDisableAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("DisableAncientEnvironmentVariable = \"SPIREPLUS_DISABLE_URDA\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyDisableAncientEnvironmentVariable = \"EZMB_DISABLE_URDA\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_URDA", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForcedAncient", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ShouldForceUrda", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForceAncientEnvironmentVariable = \"SPIREPLUS_FORCE_ANCIENT\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyForceAncientEnvironmentVariable = \"EZMB_FORCE_ANCIENT\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForceBlessingEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyForceBlessingEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForceBlessingEnvironmentVariable = \"SPIREPLUS_FORCE_URDA_BLESSING\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("LegacyForceBlessingEnvironmentVariable = \"EZMB_FORCE_URDA_BLESSING\"", urdaGate, StringComparison.Ordinal);
        Assert.Contains("AncientFeatureGate.FirstNonBlankEnvironmentValue", urdaGate, StringComparison.Ordinal);
        Assert.Contains("FirstNonBlankEnvironmentValue(ForceAncientEnvironmentVariable, LegacyForceAncientEnvironmentVariable)", urdaGate, StringComparison.Ordinal);
        Assert.Contains("FirstNonBlankEnvironmentValue(ForceBlessingEnvironmentVariable, LegacyForceBlessingEnvironmentVariable)", urdaGate, StringComparison.Ordinal);
        Assert.Contains("OrdinalIgnoreCase", urdaGate, StringComparison.Ordinal);
        Assert.Contains("string.Equals(", urdaGate, StringComparison.Ordinal);
        Assert.Contains("AncientFeatureGate.IsTruthyEnvironmentVariable(DisableAncientEnvironmentVariable, trimValue: true)", urdaGate, StringComparison.Ordinal);
        Assert.Contains("AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableAncientEnvironmentVariable, trimValue: true)", urdaGate, StringComparison.Ordinal);

        Assert.Contains("IsUrdaEnabled", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaFeatureGate.ShouldForceUrda", urdaSource, StringComparison.Ordinal);
        Assert.Contains("CustomAncientModel", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomScenePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomMapIconPath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomMapIconOutlinePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomRunHistoryIconPath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomRunHistoryIconOutlinePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.BackgroundScene", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.MapIcon", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.MapIconOutline", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.RunHistoryIcon", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.RunHistoryIconOutline", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaSeedbedOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaHumusPactOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaMoltingOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaMossMapOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaTrialBranchOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaShallowRootRelicOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaEliteRootOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaRootedRouteOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaAfterRainOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaRootSightOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaSeedBankOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("ExpectedInitialOptionCount = 4", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("AncientInitialOptionReroll.CanOffer", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("base(autoAdd: false)", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("AllPossibleOptions", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingIds.Seedbed", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaRewardSelectionService.SelectBlessing<T>", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.SetSelectedBlessing", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyMolting", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyTrialBranch", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyShallowRootRelic", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyRootedRoute", urdaSource, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyRootSight", urdaSource, StringComparison.Ordinal);
        Assert.DoesNotContain("NeowEpoch", urdaAncient, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("urda_seedbed", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_humus_pact", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_molting", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_moss_map", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_trial_branch", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_shallow_root_relic", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_elite_root", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_rooted_route", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_after_rain", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_root_sight", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_seed_bank", urdaBlessings, StringComparison.Ordinal);
        Assert.Equal(11, Regex.Matches(urdaAncient, @"UrdaBlessingIds\.[A-Za-z]+")
            .Cast<Match>()
            .Select(match => match.Value)
            .Distinct(StringComparer.Ordinal)
            .Count());
        Assert.Contains("Done();", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => UrdaAssetPaths.BackgroundScene", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => UrdaAssetPaths.Icon", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => $\"{MainFile.ResPath}/images/events/ezmb_urda.png\"", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("urda_morvi", urdaBlessings, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("urda_lotha", urdaAncient, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("urda_vakuu", urdaAncient, StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            urdaOptionRelics,
            "UrdaOptionRelic : CustomRelicModel",
            "Rarity => RelicRarity.Event",
            "IsAllowed(IRunState runState) => false",
            "IsAllowedAtNeow(Player player) => false",
            "IsAllowedInShops => false",
            "UrdaSeedbedOptionRelic",
            "UrdaHumusPactOptionRelic",
            "UrdaMoltingOptionRelic",
            "UrdaMossMapOptionRelic",
            "UrdaTrialBranchOptionRelic",
            "UrdaShallowRootRelicOptionRelic",
            "UrdaEliteRootOptionRelic",
            "UrdaRootedRouteOptionRelic",
            "UrdaAfterRainOptionRelic",
            "UrdaRootSightOptionRelic",
            "UrdaSeedBankOptionRelic");
        AssertSourceContains(
            urdaOptionRelics,
            "UrdaSeedBankRelicClickPatch",
            "HarmonyPatch(typeof(NRelicInventory), \"OnRelicClicked\")",
            "UrdaBlessingService.GetSeedBankStoredCount(seedBank.Owner)",
            "TaskHelper.RunSafely(UrdaBlessingService.TryExtractSeedBankFromRelicClick(seedBank.Owner))",
            "RefreshStoredSeedDisplay",
            "HoverTipFactory.FromCard(card)");
        Assert.Equal(11, Regex.Matches(urdaOptionRelics, @"\[Pool\(typeof\(SharedRelicPool\)\)\]").Count);

        AssertSourceContains(
            urdaScene,
            "[node name=\"EzmbUrdaBackground\" type=\"Control\"]",
            "[node name=\"Artwork\" type=\"TextureRect\" parent=\".\"]",
            "texture = ExtResource(\"1_urda\")");
        Assert.DoesNotContain("[node name=\"EzmbUrdaBackground\" type=\"Node2D\"]", urdaScene, StringComparison.Ordinal);
        Assert.DoesNotContain("type=\"Sprite2D\"", urdaScene, StringComparison.Ordinal);

        foreach (var relativePath in new[]
        {
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seedbed.png",
            "EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png",
            "EZMicroBalance/images/ancients/urda/options/urda_molting.png",
            "EZMicroBalance/images/ancients/urda/options/urda_moss_map.png",
            "EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png",
            "EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png",
            "EZMicroBalance/images/ancients/urda/options/urda_elite_root.png",
            "EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png",
            "EZMicroBalance/images/ancients/urda/options/urda_after_rain.png",
            "EZMicroBalance/images/ancients/urda/options/urda_root_sight.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png"
        })
        {
            AssertRepoFileExists(relativePath.Split('/'));
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }

        AssertSourceContains(
            urdaInitializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModHelper.SubscribeForCombatStateHooks",
            "UrdaFeatureGate.IsUrdaEnabled(runState.UnlockState)",
            "UrdaFeatureGate.IsUrdaEnabled(combatState.RunState.UnlockState)",
            "ModelDb.GetById<UrdaRunHook>",
            "ModelDb.GetById<UrdaCombatHook>");
        AssertSourceContains(
            urdaSource,
            "TryModifyCardRewardAlternatives",
            "AfterRewardTaken",
            "BeforeRoomEntered",
            "AfterCardPlayed",
            "AfterCombatVictory",
            "AfterDamageReceived",
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward",
            "AcceptSeedbed",
            "CanPaySeedbedCost",
            "CreatureCmd.LoseMaxHp",
            "CreatureCmd.SetMaxHp",
            "TryAddHumusPactAlternative",
            "ChooseHumusPact",
            "HumusCompletionPending",
            "PlayerCmd.GainGold",
            "ResolveHumusCompletion",
            "CardSelectCmd.FromDeckForRemoval",
            "WithSkippingDisallowed",
            "CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll",
            "ApplyMolting",
            "CreateCard<WitheredHusk>",
            "AfterRoomEntered",
            "player.IsActiveForHooks",
            "ApplyMossMapRoomReward",
            "PotionCmd.TryToProcure",
            "ApplyTrialBranch",
            "TrialBranchOfferCount = 4",
            "TrialBranchCombats = 3",
            "TrialBranchRequiredSuccesses = 3",
            "UrdaCombatHook",
            "AncientSavedStateFields.UrdaTrialPlantCard",
            "ApplyShallowRootRelic",
            "ShallowRootRelicChoices = 2",
            "ShallowRootInitialGold = 75",
            "ShallowRootEliteGold = 35",
            "RootedRouteMaxVisibleFloor = 7",
            "RootedRouteCardRewards = 3",
            "RootedRouteWitherHpLoss = 8",
            "RootedRouteWitherGold = 25",
            "MapPointType.Monster",
            "EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>",
            "AfterRainGoldPayoff = 75",
            "AfterRainRecoveryHeal = 8",
            "AfterRainCleanActOneThreshold = 3",
            "UrdaRainBreath",
            "AfterRainTriggerCount",
            "RootSightStartingEyes = 5",
            "MapPointType.Unknown",
            "MapPointType.Elite",
            "RoomType.Boss",
            "SeedBankMaxSeeds = 3",
            "SeedBankMaxSettlementCards = 2",
            "SetupSeedbed",
            "TryPlantSeedbedCardFromHand",
            "IsSeedbedSeedableCard",
            "SeedbedCombatSlots",
            "CardPileCmd.RemoveFromCombat",
            "TryAddSeedBankAlternative",
            "EZMB_URDA_SEED_BANK_STORE",
            "UrdaStateKey");
        Assert.DoesNotContain("[HarmonyPatch(typeof(CardReward), nameof(CardReward.OnSkipped))]", urdaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("OnSkipped", urdaRunHook, StringComparison.Ordinal);
        var seedbedRewardSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.Seedbed.cs");
        var seedbedCombatSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedCombat.cs");
        var seedbedSource = string.Join(Environment.NewLine, seedbedRewardSource, seedbedCombatSource);
        AssertSourceContains(
            urdaRunHook,
            "public override async Task AfterCardChangedPiles",
            "UrdaBlessingService.TryPlantSeedbedCardFromHand(card, \"card entered hand\")",
            "UrdaBlessingService.SyncPersistentState(card.Owner)");
        Assert.DoesNotContain("TryCatchSeedbedCardFromHand", urdaRunHook, StringComparison.Ordinal);
        AssertSourceContains(
            seedbedCombatSource,
            "CardSelectorPrefs(new LocString(\"cards\", \"EZMB_URDA_SEEDBED.selectionScreenPrompt\"), 1, 1)",
            "card is WitheredHusk or RootFamilyCard",
            "card.DeckVersion == null",
            "CardPileCmd.RemoveFromCombat(card, skipVisuals: true)",
            "TryAddGeneratedCardToCombat(husk, PileType.Hand, player)",
            "HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))",
            "WasPlantedBySeedbed(card)",
            "Planting skipped play, discard, and Exhaust synergies");
        Assert.Contains(
            "Set up a [blue]{Capacity}[/blue]-space [gold]Seedbed[/gold]",
            JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json")["EZMB_URDA_SEEDBED.description"],
            StringComparison.Ordinal);
        var seedbedAlternative = SliceBetween(seedbedRewardSource, "private static bool TryAddSeedbedAlternative", "private static async Task AcceptSeedbed");
        Assert.DoesNotContain("SeedbedChecks = progress.SeedbedChecks + 1", seedbedAlternative, StringComparison.Ordinal);
        AssertSourceContains(
            seedbedAlternative,
            "CardReward cardReward",
            "AcceptSeedbed(player, cardReward)",
            "progress.SeedbedAccepted >= MaxSeedbedChecks",
            "!CanPaySeedbedCost(player)",
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward");
        var seedbedAccept = SliceBetween(seedbedRewardSource, "private static async Task AcceptSeedbed", "private static bool CanPaySeedbedCost");
        Assert.True(
            seedbedAccept.IndexOf("var addResult = await CardPileCmd.Add(seedbed, PileType.Deck)", StringComparison.Ordinal) <
            seedbedAccept.IndexOf("await CreatureCmd.LoseMaxHp", StringComparison.Ordinal),
            "Seedbed must only charge Max HP after Core accepts the card into the deck.");
        AssertSourceContains(
            seedbedAccept,
            "CardReward cardReward",
            "CardRewardContexts.TryGetValue(cardReward, out var context)",
            "context.SeedbedHandled",
            "context.SeedbedHandled = true",
            "duplicate reward alternative click ignored",
            "!CanPaySeedbedCost(player)",
            "CreatureCmd.LoseMaxHp",
            "cost and progress were not applied",
            "SeedbedChecks = progress.SeedbedChecks + 1",
            "CreatureCmd.SetMaxHp");
        Assert.Contains("public bool SeedbedHandled { get; set; }", urdaSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.GainMaxHp", seedbedAccept, StringComparison.Ordinal);
        var chooseHumus = SliceBetween(urdaHumusPact, "private static async Task ChooseHumusPact", "private static async Task<bool> ResolveHumusCompletion");
        AssertSourceContains(
            chooseHumus,
            "context.HumusPactHandled",
            "progress.HumusCompleted || progress.HumusCompletionPending",
            "progress = progress with { HumusSkips = progress.HumusSkips + 1 }",
            "HumusCompletionPending = true",
            "PlayerCmd.GainGold");
        Assert.DoesNotContain("RewardsSet", chooseHumus, StringComparison.Ordinal);
        Assert.DoesNotContain("CardSelectCmd.FromDeckForRemoval", chooseHumus, StringComparison.Ordinal);
        var humusAfterReward = SliceBetween(urdaHumusPact, "public static async Task AfterRewardTaken", "private static bool TryAddHumusPactAlternative");
        AssertSourceContains(
            humusAfterReward,
            "var resolved = await ResolveHumusCompletion(player);",
            "if (!resolved)",
            "progress = GetProgress(player) with { HumusCompletionPending = false };");
        Assert.True(
            humusAfterReward.IndexOf("ResolveHumusCompletion(player)", StringComparison.Ordinal) <
            humusAfterReward.IndexOf("HumusCompletionPending = false", StringComparison.Ordinal),
            "Humus completion pending should clear only after the payoff resolver succeeds.");
        var humusCompletion = SliceBetween(urdaHumusPact, "private static async Task<bool> ResolveHumusCompletion", "private static CardModel? CreateRandomRewardCard");
        AssertSourceContains(
            humusCompletion,
            "var rewardCard = CreateRandomRewardCard(player);",
            "return false;",
            "CardSelectCmd.FromDeckForRemoval",
            "WithSkippingDisallowed",
            "return true;");
        Assert.True(
            humusCompletion.IndexOf("CreateRandomRewardCard(player)", StringComparison.Ordinal) <
            humusCompletion.IndexOf("CardSelectCmd.FromDeckForRemoval", StringComparison.Ordinal),
            "Humus should generate the payoff card before optional removals so a no-card fallback cannot consume removals.");
        var trialBranchOffer = urdaTrialBranchOffer;
        AssertSourceContains(
            trialBranchOffer,
            "CreateTrialBranchOffers(player)",
            "CardSelectCmd.FromSimpleGrid",
            "CardCmd.Upgrade(selected, CardPreviewStyle.None)",
            "CardPileCmd.Add(selected, PileType.Deck)",
            "AncientSavedStateFields.UrdaTrialPlantCard[addResult.cardAdded] = true",
            "CardCmd.Enchant<UrdaTrialBranchEnchantment>",
            "RefreshTrialBranchEnchantment");
        Assert.Contains("card.Rarity == CardRarity.Rare", trialBranchOffer, StringComparison.Ordinal);
        var trialBranch = urdaTrialBranch;
        AssertSourceContains(
            trialBranch,
            "TrialSuccessfulCombats = progress.TrialSuccessfulCombats + (playedThisCombat ? 1 : 0)",
            "if (!playedThisCombat)",
            "ClearTrialBranchMarkerAndEnchantment",
            "await CardPileCmd.RemoveFromDeck(trialCard)");
        AssertSourceContains(
            urdaTrialBranchDisplay,
            "TryGetTrialBranchDisplayProgress",
            "FindTrialBranchCards(player)",
            "enchantment.SetProgress(combatsLeft, playedThisCombat, playsLeft)",
            "CardCmd.ClearEnchantment(card)");
        AssertSourceContains(
            urdaTrialBranchEnchantment,
            "internal sealed class UrdaTrialBranchEnchantment",
            "HasExtraCardText => true",
            "ShowAmount => true",
            "CombatsLeft",
            "PlayedThisCombat",
            "PlaysLeft",
            "Missing any combat removes it",
            "漏掉任意一场会移除它。");
        var shallowRoot = urdaShallowRootRelic;
        AssertSourceContains(
            shallowRoot,
            "RelicFactory.PullNextRelicFromFront",
            "RelicRarity.Common",
            "RelicSelectCmd.FromChooseARelicScreen",
            "RelicCmd.Obtain",
            "PlayerCmd.GainGold(ShallowRootInitialGold");
        var rootedRoute = urdaRouteRewards;
        AssertSourceContains(
            rootedRoute,
            "FindRootedRouteTarget(player)",
            "point.coord.row + 1 <= RootedRouteMaxVisibleFloor",
            "EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>",
            "RootedRouteCoord = FormatCoord(target.coord)");
        var rootSight = urdaSource;
        AssertSourceContains(
            rootSight,
            "RootSightStartingEyes",
            "TryBeginRootSightSelection",
            "TryCommitRootSightSelection",
            "RootSightPreviewRecords",
            "var mapScreen = NMapScreen.Instance",
            "selection could not start because the map screen is not available",
            "mapScreen.Open(isOpenedFromTopBar: true)",
            "mapScreen.RefreshAllPointVisuals()",
            "EnsureQuestMarker<UrdaRootSightMapQuestMarker>");
        AssertSourceContains(
            rootSight,
            "point.PointType is not (MapPointType.Monster or MapPointType.Unknown or MapPointType.Elite)",
            ".BuildRoomTypeBlacklist",
            "RootSightUnknownBlacklist",
            "TryPeekNextValidEvent",
            "TryGetRootSightRoomTypeForCurrentPoint",
            "TryGetRootSightModelForCurrentPoint");
        var rootSightEncounters = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEncounters.cs");
        var rootSightEvents = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEvents.cs");
        AssertSourceContains(
            rootSightEncounters,
            "return candidates[0];",
            "var startIndex = visited % source.Count",
            "source[(startIndex + offset) % source.Count]");
        Assert.DoesNotContain("NextItem(candidates)", rootSightEncounters, StringComparison.Ordinal);
        AssertSourceContains(
            rootSightEvents,
            "return candidates[0];");
        Assert.DoesNotContain("NextItem(candidates)", rootSightEvents, StringComparison.Ordinal);
        AssertSourceContains(
            urdaMapUiPatches,
            "UrdaRootSightMapHoverPatch",
            "HarmonyPatch(typeof(NNormalMapPoint), \"OnFocus\")",
            "SpirePlusMapPointHoverComposer",
            "UrdaBlessingService.TryGetRootSightHoverTip",
            "FiremarkedEliteMapHoverPatch.TryCreateHoverTip",
            "BannerRoomMapHoverPatch.TryCreateHoverTip",
            "TryGetRootSightPreviewRoomType",
            "UrdaRootSightMapPreviewIconPatch",
            "UrdaRootSightMapQuestIconPatch",
            "UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon",
            "UrdaRootSightMapPreviewVisuals.ApplyQuestIcon",
            "UrdaBlessingService.CanRootSightTarget(pointNode.Point)",
            "ApplyRootSightOverlay(pointNode, hasRootSightMarker || canTargetWithRootSight)",
            "questIcon.Visible = true",
            "NHoverTipSet.CreateAndShow",
            "UrdaRootSightMapPointClickPatch",
            "HarmonyPatch(typeof(NMapPoint), \"OnRelease\")",
            "UrdaRootSightDisabledMapPointClickPatch",
            "HarmonyPatch(typeof(NClickableControl), nameof(NClickableControl._GuiInput))",
            "InputEventMouseButton { ButtonIndex: MouseButton.Left }",
            "__instance.GetViewport()?.SetInputAsHandled()",
            "UrdaRootSightMapClosePatch",
            "UrdaBlessingService.CancelRootSightSelection");
        var rootSightRoomPatches = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightRoomPatches.cs");
        AssertSourceContains(
            rootSightRoomPatches,
            "HarmonyPatch(typeof(RunManager), \"RollRoomTypeFor\")",
            "TryGetRootSightRoomTypeForCurrentPoint",
            "HarmonyPatch(typeof(RunManager), \"CreateRoom\")",
            "TryGetRootSightModelForCurrentPoint");
        AssertSourceContains(
            urdaAncient,
            "RootSightHoverTips",
            "EZMB_URDA.root_sight.hover.title",
            "EZMB_URDA.root_sight.hover.description");
        var afterRain = urdaAfterRain;
        AssertSourceContains(
            afterRain,
            "player.RunState.CurrentActIndex != 0",
            "AfterRainTriggeredThisCombat",
            "IsAfterRainTrigger",
            "CreateCard<UrdaRainBreath>",
            "CardPileCmd.AddGeneratedCardToCombat",
            "CompensateAfterRainAtActTwo",
            "PlayerCmd.GainGold(AfterRainGoldPayoff",
            "CreatureCmd.Heal(player.Creature, AfterRainRecoveryHeal",
            "CardSelectCmd.FromDeckForUpgrade");
        var seedBank = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"));
        AssertSourceContains(
            seedBank,
            "GetSeedBankCardIds(progress)",
            "ConditionalWeakTable<Player, SeedBankExtractionState>",
            "SeedBankExtractionInProgress.GetOrCreateValue(player)",
            "if (extractionState.InProgress)",
            "try",
            "finally",
            "CardSelectCmd.FromSimpleGrid",
            "selected.Id.ToString()",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "RefreshSeedBankRelicStatus(player)",
            "SeedBankCardIds");
        Assert.DoesNotContain("UrdaTrialPlantCard", seedBank, StringComparison.Ordinal);
        Assert.DoesNotContain("RootDeckService.FindRootFamilyCards(card.Owner)", seedbedSource, StringComparison.Ordinal);
        Assert.DoesNotContain("rootblight.PlantedInSeedbed = true", seedbedSource, StringComparison.Ordinal);
        Assert.DoesNotContain("SettleSeedBankBeforeActOneBoss", urdaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("room.RoomType == RoomType.Boss", urdaRunHook, StringComparison.Ordinal);
        var huskTransformPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaWitheredHuskTransformPatches.cs");
        var normalizedUrdaCards = urdaCards.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains(
            "[Pool(typeof(CurseCardPool))]\npublic sealed class WitheredHusk",
            normalizedUrdaCards,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "[Pool(typeof(TokenCardPool))]\npublic sealed class WitheredHusk",
            normalizedUrdaCards,
            StringComparison.Ordinal);
        AssertSourceContains(
            huskTransformPatch,
            "HarmonyPatch(typeof(CardModel), nameof(CardModel.IsTransformable), MethodType.Getter)",
            "__instance is WitheredHusk",
            "__result = false",
            "HarmonyPatch(typeof(CardFactory), nameof(CardFactory.GetDefaultTransformationOptions))",
            "__result.Where(card => card is not WitheredHusk)");
        var witheredHusk = SliceFrom(urdaCards, "public sealed class WitheredHusk");
        AssertSourceContains(
            witheredHusk,
            "WitheredHusk",
            "base(0, CardType.Curse, CardRarity.Curse, TargetType.Self",
            "CardKeyword.Ethereal",
            "CardKeyword.Exhaust",
            "HoverTipFactory.FromKeyword(CardKeyword.Ethereal)",
            "HoverTipFactory.FromKeyword(CardKeyword.Exhaust)",
            "public override async Task AfterCardExhausted",
            "if (card != this",
            "CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null)");
        Assert.DoesNotContain("CardKeyword.Unplayable", witheredHusk, StringComparison.Ordinal);
        Assert.DoesNotContain("protected override async Task OnPlay", witheredHusk, StringComparison.Ordinal);
        Assert.DoesNotContain("ExhaustOnNextPlay = true", witheredHusk, StringComparison.Ordinal);
        AssertSourceContains(
            engCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        AssertSourceContains(
            zhsCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", engCardRewardUi, StringComparison.Ordinal);
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", zhsCardRewardUi, StringComparison.Ordinal);
        Assert.Equal("Compost Reward", engCardRewardUiMap["OPTION_EZMB_URDA_HUMUS_PACT.name"]);
        AssertLocalizedKeys(
            [
                "OPTION_EZMB_URDA_SEEDBED.name",
                "OPTION_EZMB_URDA_HUMUS_PACT.name",
                "OPTION_EZMB_URDA_SEED_BANK_STORE.name"
            ],
            engCardRewardUiMap,
            zhsCardRewardUiMap,
            "Urda card-reward option localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Urda option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient",
                "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.title",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.title",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.description",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.title",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.title",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.title",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.description",
                "EZMB_URDA.pages.INITIAL.options.urda_elite_root.title",
                "EZMB_URDA.pages.INITIAL.options.urda_elite_root.description",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.title",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.description",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.title",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.description",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.title",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.description",
                "EZMB_URDA.root_sight.hover.title",
                "EZMB_URDA.root_sight.hover.description",
                "EZMB_URDA.root_sight.selection_hover.title",
                "EZMB_URDA.root_sight.selection_hover.description",
                "EZMB_URDA.root_sight.map_hover.title",
                "EZMB_URDA.root_sight.map_hover.description",
                "EZMB_URDA.root_sight.map_hover.preview_description",
                "EZMB_URDA.root_sight.map_hover.event_preview_description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.storeSelectionPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.settlementSelectionPrompt"
            ],
            engAncients,
            zhsAncients,
            "Urda ancient localization");
    }

    [Fact]
    public void UrdaDocsKeepLiveAndSaveLoadVerificationPending()
    {
        var issueIndex = ReadRepoText("docs", "issues.md");
        var urdaIssue = ReadRepoText("docs", "issues", "urda.md");
        var urdaReadme = ReadRepoText("docs", "features", "ancient-expansion-urda", "README.md");
        var urdaApi = ReadRepoText("docs", "features", "ancient-expansion-urda", "api-research.md");
        var urdaChecklist = ReadRepoText("docs", "features", "ancient-expansion-urda", "manual-test-checklist.md");
        var v22Readme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");
        var v22Api = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var v22Risk = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");

        var currentUrdaDocs = string.Join(
            Environment.NewLine,
            issueIndex,
            urdaIssue,
            urdaReadme,
            urdaApi,
            urdaChecklist,
            v22Readme,
            v22Api,
            v22Risk);

        Assert.Contains("`URDA-PROTOTYPE` P0 open", issueIndex, StringComparison.Ordinal);
        Assert.Contains("live gameplay and save/load proof remain pending", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Live gameplay and save/load verification for current Urda remains pending", v22Readme, StringComparison.Ordinal);
        Assert.Contains("not source-proven as persisted", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("## 1A. Live evidence protocol", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\\window-preflight.json -RequireSpireForeground", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <evidence-dir>\\godot.log -OutFile <evidence-dir>\\godot-log-audit.json -FailOnHit", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-postfix-20260513-131752", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-continue-postfix-20260513-134337", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("do not satisfy any gameplay row", urdaChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Urda live gameplay verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Urda save/load verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("URDA-PROTOTYPE | Closed", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("- [x]", urdaChecklist, StringComparison.Ordinal);
    }
}
