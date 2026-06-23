using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaReleaseCoverageGuardTests
{
    [Fact]
    public void UrdaIsDefaultOnDisableableAndBlessingSliceSourceBacked()
    {
        var urdaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureGate.cs");
        var urdaAncient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingIds.cs");
        var urdaCardModels = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedling.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedbed.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRainBreath.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "WitheredHusk.cs"));
        var witheredHusk = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "WitheredHusk.cs");
        var ritsuRegistration = ReadSourceTree("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib");
        var urdaInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaInitializer.cs");
        var urdaMapUiPatches = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapClickPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapPreviewVisuals.cs"),
            ReadRepoText("EZMicroBalanceCode", "Map", "SpirePlusMapPointHoverComposer.cs"));
        var urdaOptionRelics = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelic.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaStandardOptionRelics.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightOptionRelic.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedBankOptionRelic.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelicClickPatch.cs"));
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var urdaAfterRain = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.AfterRain.cs");
        var urdaEliteRoot = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.EliteRoot.cs");
        var urdaHumusPact = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.HumusPact.cs");
        var urdaRootedRoute = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootedRoute.cs");
        var urdaRootedRouteReward = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootedRouteReward.cs");
        var urdaShallowRootRelic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.ShallowRootRelic.cs");
        var urdaTrialBranchCombat = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchCombat.cs");
        var urdaTrialBranchDisplay = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchDisplay.cs");
        var urdaTrialBranchOffer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchOffer.cs");
        var urdaTrialBranchResolution = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.TrialBranchResolution.cs");
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
        Assert.Contains("ModAncientEventTemplate", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomBackgroundScenePath", urdaAncient, StringComparison.Ordinal);
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
        Assert.DoesNotContain("base(autoAdd: false)", urdaAncient, StringComparison.Ordinal);
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
            "UrdaOptionRelic : ModRelicTemplate",
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
            "UrdaOptionRelicClickPatch",
            "IPatchMethod.PatchId => \"urda-option-relic-click\"",
            "new ModPatchTarget(typeof(NRelicInventory), \"OnRelicClicked\", [typeof(RelicModel)])",
            "UrdaBlessingService.GetSeedBankStoredCount(seedBank.Owner)",
            "TaskHelper.RunSafely(UrdaBlessingService.TryExtractSeedBankFromRelicClick(seedBank.Owner))",
            "RefreshStoredSeedDisplay",
            "CreateStoredSeedsHoverTip",
            "storedSeeds.descriptionPrefix");
        Assert.Contains("RegisterPatch<UrdaOptionRelicClickPatch>();", ritsuRegistration, StringComparison.Ordinal);
        Assert.DoesNotContain("HoverTipFactory.FromCard(card)", urdaOptionRelics, StringComparison.Ordinal);
        Assert.DoesNotContain(".Concat(card.HoverTips)", urdaOptionRelics, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(SharedRelicPool))]", urdaOptionRelics, StringComparison.Ordinal);
        Assert.Equal(11, Regex.Matches(ritsuRegistration, @"content\.Relic<SharedRelicPool, Urda[A-Za-z]+(?:OptionRelic|RelicOptionRelic)>\(\);").Count);
        Assert.DoesNotContain(
            "HarmonyPatch(typeof(NRelicInventory)",
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedBankOptionRelic.cs"),
            StringComparison.Ordinal);

        AssertUrdaSceneAndAssetCoverage(urdaScene, exportPreset);

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
        AssertSourceContains(
            urdaEliteRoot,
            "room.RoomType != RoomType.Elite",
            "player.Creature.IsDead",
            "SpirePlusFeedback.ConfirmRelicPayoff(eliteRoot)",
            "CreatureCmd.Heal(player.Creature, EliteRootHeal)",
            "UrdaEliteRoot",
            "elite_victory_heal");
        Assert.DoesNotContain("OnSkipped", urdaRunHook, StringComparison.Ordinal);
        var seedbedRewardSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.Seedbed.cs");
        var seedbedCombatSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedCombat.cs");
        var seedbedPlantingQueueSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedPlantingQueue.cs");
        var seedbedStateSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedState.cs");
        var seedbedPatchSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedbedAfterCardDrawnPatch.cs");
        var seedbedSource = string.Join(Environment.NewLine, seedbedRewardSource, seedbedCombatSource, seedbedPlantingQueueSource, seedbedStateSource, seedbedPatchSource);
        AssertSourceContains(
            urdaRunHook,
            "public override Task AfterCardChangedPiles",
            "_ = UrdaBlessingService.QueueSeedbedPlantFromHand(card, \"card entered hand\")",
            "UrdaBlessingService.SyncPersistentState(card.Owner)");
        Assert.DoesNotContain("TryPlantSeedbedCardFromHand(card, \"card entered hand\")", urdaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryCatchSeedbedCardFromHand", urdaRunHook, StringComparison.Ordinal);
        AssertSourceContains(
            seedbedCombatSource,
            "CardSelectorPrefs(new LocString(\"cards\", \"EZMB_URDA_SEEDBED.selectionScreenPrompt\"), 1, selectionCount)",
            "card is WitheredHusk",
            "card is RootFamilyCard rootblight",
            "RootDeckService.CanHoldRootblightBySeedbed(rootblight)",
            "RootDeckService.TryHoldRootblightBySeedbed(rootblight)",
            "card.DeckVersion == null",
            "CardPileCmd.RemoveFromCombat(card, skipVisuals: true)",
            "TryAddGeneratedCardToCombat(husk, PileType.Hand, player)",
            "Planting skipped play, discard, and Exhaust synergies");
        AssertSourceContains(
            seedbedPlantingQueueSource,
            "Queue<SeedbedPlantingRequest> PendingRequests",
            "Task<bool> QueueSeedbedPlantFromHand",
            "bool IsProcessing",
            "ProcessSeedbedPlantingQueue");
        AssertSourceContains(
            seedbedStateSource,
            "ConditionalWeakTable<Player, SeedbedCombatState>",
            "ConditionalWeakTable<CardModel, SeedbedPlantMarker>",
            "GetOrRestoreSeedbed(Player player)",
            "SeedbedCombatSlots",
            "MarkSeedbedPlantedCard",
            "WasPlantedBySeedbed(CardModel card)");
        AssertSourceContains(
            seedbedPatchSource,
            "class UrdaSeedbedAfterCardDrawnPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"urda-seedbed-after-card-drawn\"",
            "new ModPatchTarget(",
            "typeof(Hook)",
            "nameof(Hook.AfterCardDrawn)",
            "[typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool)])",
            "WasPlantedBySeedbed(card)",
            "skipped AfterCardDrawn hooks for planted card");
        Assert.DoesNotContain("HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))", seedbedCombatSource, StringComparison.Ordinal);
        Assert.Contains(
            "Set up a [blue]{Capacity}[/blue]-space [gold]Seedbed[/gold]",
            JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json")["EZMB_URDA_SEEDBED.description"],
            StringComparison.Ordinal);
        AssertSourceContains(
            urdaCardModels,
            "public sealed class UrdaSeedbed",
            "new BlockVar(8m, ValueProp.Move)",
            "DynamicVars.Block.UpgradeValueBy(4m)");
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
        var trialBranchCombat = urdaTrialBranchCombat;
        AssertSourceContains(
            trialBranchCombat,
            "AfterCardPlayed",
            "TrialBranchCombats = 3",
            "TrialBranchRequiredSuccesses = 3",
            "cardPlay.Card.DeckVersion is not { } deckCard",
            "TrialPlayedThisCombat = true",
            "RefreshTrialBranchEnchantment(player)");
        Assert.DoesNotContain("RemoveFromDeck", trialBranchCombat, StringComparison.Ordinal);
        var trialBranchResolution = urdaTrialBranchResolution;
        AssertSourceContains(
            trialBranchResolution,
            "ResolveTrialBranchCombat",
            "TrialSuccessfulCombats = progress.TrialSuccessfulCombats + (playedThisCombat ? 1 : 0)",
            "if (!playedThisCombat)",
            "ClearTrialBranchMarkerAndEnchantment",
            "await CardPileCmd.RemoveFromDeck(trialCard)");
        Assert.DoesNotContain("AfterCardPlayed", trialBranchResolution, StringComparison.Ordinal);
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
        AssertSourceContains(
            urdaRootedRoute,
            "FindRootedRouteTarget(player)",
            "point.coord.row + 1 <= RootedRouteMaxVisibleFloor",
            "EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>",
            "RootedRouteCoord = FormatCoord(target.coord)");
        Assert.DoesNotContain("RewardsSet(player)", urdaRootedRoute, StringComparison.Ordinal);
        AssertSourceContains(
            urdaRootedRouteReward,
            "TryResolveRootedRouteReward(Player player)",
            "CreateRootedRouteRewardCards(player)",
            "RootedRouteCardRewards = 3",
            "WithCustomRewards(cards.Select<CardModel, Reward>(card => new SpecialCardReward(card, player)).ToList())",
            "RemoveQuestMarker<UrdaRootedRouteMapQuestMarker>(current)");
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
            "SpirePlusMapPointHoverComposer",
            "IPatchMethod.PatchId => \"spire-plus-map-point-hover-composer\"",
            "new ModPatchTarget(typeof(NNormalMapPoint), \"OnFocus\")",
            "UrdaBlessingService.TryGetRootSightHoverTip",
            "FiremarkedEliteMapHoverPatch.TryCreateHoverTip",
            "BannerRoomMapHoverPatch.TryCreateHoverTip",
            "TryGetRootSightPreviewRoomType",
            "UrdaRootSightMapQuestIconInputPatch",
            "IPatchMethod.PatchId => \"urda-root-sight-map-point-ready\"",
            "new ModPatchTarget(typeof(NNormalMapPoint), nameof(NNormalMapPoint._Ready))",
            "UrdaRootSightMapPreviewIconPatch",
            "IPatchMethod.PatchId => \"urda-root-sight-map-refresh-state\"",
            "new ModPatchTarget(typeof(NNormalMapPoint), \"RefreshState\")",
            "UrdaRootSightMapQuestIconPatch",
            "IPatchMethod.PatchId => \"urda-root-sight-map-quest-icon-refresh\"",
            "new ModPatchTarget(typeof(NNormalMapPoint), \"RefreshMarkedIconVisibility\")",
            "UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon",
            "UrdaRootSightMapPreviewVisuals.ApplyQuestIcon",
            "UrdaBlessingService.CanRootSightTarget(pointNode.Point)",
            "ApplyRootSightOverlay(pointNode, hasRootSightMarker || canTargetWithRootSight)",
            "questIcon.Visible = true",
            "NHoverTipSet.CreateAndShow",
            "UrdaRootSightMapPointClickPatch",
            "IPatchMethod.PatchId => \"urda-root-sight-map-point-click\"",
            "new ModPatchTarget(typeof(NMapPoint), \"OnRelease\")",
            "UrdaRootSightDisabledMapPointClickPatch",
            "IPatchMethod.PatchId => \"urda-root-sight-disabled-map-point-click\"",
            "new ModPatchTarget(typeof(NClickableControl), nameof(NClickableControl._GuiInput), [typeof(InputEvent)])",
            "InputEventMouseButton { ButtonIndex: MouseButton.Left }",
            "__instance.GetViewport()?.SetInputAsHandled()",
            "UrdaRootSightMapClosePatch",
            "IPatchMethod.PatchId => \"urda-root-sight-map-close\"",
            "new ModPatchTarget(typeof(NMapScreen), nameof(NMapScreen.Close), [typeof(bool)])",
            "UrdaBlessingService.CancelRootSightSelection");
        AssertSourceContains(
            ritsuRegistration,
            "RegisterPatch<UrdaRootSightMapQuestIconInputPatch>();",
            "RegisterPatch<UrdaRootSightMapPreviewIconPatch>();",
            "RegisterPatch<UrdaRootSightMapQuestIconPatch>();",
            "RegisterPatch<UrdaRootSightMapPointClickPatch>();",
            "RegisterPatch<UrdaRootSightDisabledMapPointClickPatch>();",
            "RegisterPatch<UrdaRootSightMapClosePatch>();",
            "RegisterPatch<SpirePlusMapPointHoverComposer>();");
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
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionCommit.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionGuard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"));
        var seedBankExtraction = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs");
        var seedBankExtractionCommit = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionCommit.cs");
        var seedBankExtractionGuard = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionGuard.cs");
        var seedBankExtractionState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs");
        AssertSourceContains(
            seedBank,
            "GetSeedBankCardIds(progress)",
            "CardSelectCmd.FromSimpleGrid",
            "selected.Id.ToString()",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "RefreshSeedBankRelicStatus(player)",
            "SeedBankCardIds");
        AssertSourceContains(
            seedBankExtractionCommit,
            "CommitSeedBankSelectedCards",
            "CardPileCmd.Add(card, PileType.Deck)",
            "failedSelectedIds.Add(card.Id.ToString())",
            "SeedBankCardIds = string.Join(\",\", failedSelectedIds.Take(SeedBankMaxSeeds))",
            "SeedBankSettled = true",
            "storage_cleared",
            "extracted_by_relic_click");
        AssertSourceContains(
            seedBankExtractionState,
            "ConditionalWeakTable<Player, SeedBankExtractionState>",
            "private sealed class SeedBankExtractionState",
            "public bool InProgress { get; set; }",
            "private static readonly ConditionalWeakTable<Player, SeedBankExtractionState> SeedBankExtractionInProgress = new()");
        AssertSourceContains(
            seedBankExtractionGuard,
            "public static async Task TryExtractSeedBankFromRelicClick(Player player)",
            "SeedBankExtractionInProgress.GetOrCreateValue(player)",
            "if (extractionState.InProgress)",
            "[Spire Plus] Urda Seed Bank extraction ignored: a Seed Bank selection is already open.",
            "extractionState.InProgress = true",
            "try",
            "TryExtractSeedBankFromRelicClickOnce(player)",
            "finally",
            "extractionState.InProgress = false");
        Assert.DoesNotContain("ConditionalWeakTable<Player, SeedBankExtractionState>", seedBankExtraction, StringComparison.Ordinal);
        Assert.DoesNotContain("UrdaTrialPlantCard", seedBank, StringComparison.Ordinal);
        Assert.DoesNotContain("RootDeckService.FindRootFamilyCards(card.Owner)", seedbedSource, StringComparison.Ordinal);
        Assert.DoesNotContain("SettleSeedBankBeforeActOneBoss", urdaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("room.RoomType == RoomType.Boss", urdaRunHook, StringComparison.Ordinal);
        var huskTransformPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaWitheredHuskTransformPatches.cs");
        Assert.Contains("content.Card<CurseCardPool, WitheredHusk>(FullEntry(WitheredHusk.CardId));", ritsuRegistration, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(CurseCardPool))]", witheredHusk, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(TokenCardPool))]", witheredHusk, StringComparison.Ordinal);
        AssertSourceContains(
            huskTransformPatch,
            "class WitheredHuskTransformablePatch : IPatchMethod",
            "IPatchMethod.PatchId => \"urda-withered-husk-transformable\"",
            "new ModPatchTarget(typeof(CardModel), nameof(CardModel.IsTransformable), MethodType.Getter)",
            "__instance is WitheredHusk",
            "__result = false",
            "class WitheredHuskTransformationOptionsPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"urda-withered-husk-transformation-options\"",
            "typeof(CardFactory)",
            "nameof(CardFactory.GetDefaultTransformationOptions)",
            "[typeof(CardModel), typeof(bool)])",
            "__result.Where(card => card is not WitheredHusk)");
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
        AssertUrdaLocalizationCoverage(
            engCards,
            zhsCards,
            engCardRewardUi,
            zhsCardRewardUi,
            engCardRewardUiMap,
            zhsCardRewardUiMap,
            engRelics,
            zhsRelics,
            engAncients,
            zhsAncients);
    }
}
