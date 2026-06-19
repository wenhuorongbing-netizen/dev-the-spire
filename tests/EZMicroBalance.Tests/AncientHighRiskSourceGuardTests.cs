using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void AncientRunAndCombatHooksKeepSingleDispatchOwnership()
    {
        var morviRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var morviCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviCombatHook.cs");
        var lothaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var lothaCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaCombatHook.cs");
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var urdaCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaCombatHook.cs");

        AssertSourceContains(
            morviRunHook,
            "BeforeCombatStart",
            "AfterCardChangedPiles",
            "AfterCombatEnd");
        Assert.DoesNotContain("AfterPlayerTurnStartEarly", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterTurnEnd", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ModifyCardPlayCount", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryModifyEnergyCostInCombat", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("BeforeCardPlayed", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterCardPlayed", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterCardDrawn", morviRunHook, StringComparison.Ordinal);
        AssertSourceContains(
            morviCombatHook,
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "TryModifyEnergyCostInCombat",
            "BeforeCardPlayed",
            "AfterCardPlayed",
            "AfterCardDrawn");
        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviHooks.cs");

        AssertSourceContains(
            lothaRunHook,
            "BeforeCombatStart",
            "AfterCardChangedPiles",
            "AfterCombatEnd",
            "AfterDamageReceived",
            "TryModifyRewardsLate",
            "ShouldDieLate",
            "ShouldDie",
            "AfterPreventingDeath");
        foreach (var combatOnly in new[]
        {
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "ShouldPlay",
            "AfterCardPlayed",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "ModifyPowerAmountGivenAdditive",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged"
        })
        {
            Assert.DoesNotContain(combatOnly, lothaRunHook, StringComparison.Ordinal);
        }

        AssertSourceContains(
            lothaCombatHook,
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "ShouldPlay",
            "AfterCardPlayed",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "ModifyPowerAmountGivenAdditive",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged");

        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaHooks.cs");

        Assert.DoesNotContain("public override Task AfterCardPlayed", urdaRunHook, StringComparison.Ordinal);
        Assert.Contains("public override Task AfterCardPlayed", urdaCombatHook, StringComparison.Ordinal);
    }

    [Fact]
    public void UrdaCombatVictoryUsesRoomScopedRunState()
    {
        var lifecycle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RunLifecycle.cs");
        var afterCombatVictory = SliceFrom(lifecycle, "public static async Task AfterCombatVictory(CombatRoom room)");

        AssertSourceContains(
            afterCombatVictory,
            "var runState = room.CombatState.RunState;",
            "runState.Players.Where(player => player.IsActiveForHooks)");
        Assert.DoesNotContain("RunManager.Instance.DebugOnlyGetState()", afterCombatVictory, StringComparison.Ordinal);
    }

    [Fact]
    public void UrdaStateCleanupAvoidsGuessingAndRefreshesVisibleTrialBranchState()
    {
        var seedbed = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.Seedbed.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedCombat.cs"));
        var seedBank = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionCommit.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionGuard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"));
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.State.cs");

        Assert.DoesNotContain("FirstOrDefault(candidate => candidate.RootblightLevel == rootblight.RootblightLevel)", seedbed, StringComparison.Ordinal);
        Assert.DoesNotContain("RootDeckService.FindRootFamilyCards(card.Owner)", seedbed, StringComparison.Ordinal);
        AssertSourceContains(
            seedbed,
            "card is WitheredHusk",
            "card is RootFamilyCard rootblight",
            "RootDeckService.CanHoldRootblightBySeedbed(rootblight)",
            "RootDeckService.TryHoldRootblightBySeedbed(rootblight)",
            "card.DeckVersion == null",
            "Planting skipped play, discard, and Exhaust synergies");
        AssertSourceContains(
            seedBank,
            "try",
            "finally",
            "foreach (var card in cards)",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "player.RunState.Players.Count > 1",
            "single-player only until host-authoritative reward selection sync is implemented");
        AssertSourceContains(
            state,
            "AncientPlayerState.SyncDeck(",
            "GetSelectedBlessing(player) == UrdaBlessingIds.TrialBranch",
            "RefreshTrialBranchEnchantment(player)");
    }

    [Fact]
    public void QualityFlameUsesDynamicDrawAndVisibleExhaustKeyword()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "BrightestFlameExhaustDrawPatch.cs");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");

        AssertSourceContains(
            source,
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "__instance is not BrightestFlame",
            "CardKeyword.Exhaust",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "dynamicVar is CardsVar cards",
            "new CardsVar(cards.IntValue + ExtraDraw)",
            "Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.",
            "upgrade draws 4",
            "Does not affect Pumpkin Candle relic vanilla behavior.");

        Assert.DoesNotContain("DrawExtraAfterVanilla", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CardPileCmd.Draw(choiceContext, 1", source, StringComparison.Ordinal);

        Assert.Equal("Brilliant Flame", englishCards["BRIGHTEST_FLAME.title"]);
        Assert.Contains("{Cards:diff()}", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("{Cards:diff()}", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Draw 3 cards", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("鎶?寮犵墝", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("Brilliant Flame / Brightest Flame", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("BrightestFlame", apiDiscovery, StringComparison.Ordinal);
    }

    [Fact]
    public void PrismaticGemOffColorReplacementKeepsNormalRewardBoundariesAllSlotsAndRunStateClean()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");

        AssertSourceContains(
            source,
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]",
            "HarmonyPrefix",
            "player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted)",
            "foreach (var listener in runState.IterateHookListeners(null))",
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "if (listenerModified)",
            "modifiers.Add(listener)",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "if (!creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward))",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications)",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications)",
            "creationOptions.Source == CardCreationSource.Encounter",
            "creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter",
            "creationOptions.CustomCardPool == null",
            "creationOptions.CardPoolFilter == null",
            "creationOptions.CardPools.Count > 0",
            "!creationOptions.CardPools.All(pool => pool.IsColorless)",
            "ModelDb.AllCharacterCardPools",
            ".Where(pool => !pool.Id.Equals(homePool.Id) && !pool.IsColorless)",
            ".Where(card => rarity == null || card.Rarity == rarity)",
            ".Where(card => type == null || card.Type == type)",
            ".Where(card => card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest)",
            ".Where(card => card.CanBeGeneratedByModifiers)",
            ".Where(card => !excludedIds.Contains(card.Id))",
            ".DistinctBy(card => card.Id)",
            "var excludedIds = cardRewardOptions",
            "for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)",
            "PreserveUpgradeState(originalCard, replacement)",
            "reward.ModifyCard(replacement, prismaticGem)",
            "RewardResultHints.GetValue(reward, _ => new RewardResultHintState())",
            "excludedIds.Add(replacement.Id)",
            "player.RunState.RemoveCard(originalCard)",
            "RemoveUnpiledReplacements(replacements)",
            "AncientCardHelpers.RemoveUnpiledRunCard(replacement)",
            "RestoreCounterAfterFailedReplacement(prismaticGem, screenState)",
            "GetOffColorRewardPool(player, originalCard.Rarity, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)",
            "GetOffColorRewardPool(player, null, null, excludedIds)",
            "return player.RunState.CreateCard(replacementCanonical, player)");

        Assert.DoesNotContain("var slotIndex = cardRewardOptions.Count - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ReplaceRightmostRewardSlot", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DirectAncientRelicPatchesRespectMeltedRelics()
    {
        var prismatic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var fiddle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "FiddlePatches.cs");

        AssertSourceContains(
            prismatic,
            "FirstOrDefault(relic => !relic.IsMelted)");
        AssertSourceContains(
            fiddle,
            "if (__instance.IsMelted)",
            "__result = count;",
            "__result = true;",
            "player.GetRelic<Fiddle>() is not { IsMelted: false }");
    }

    [Fact]
    public void TurnStartAndAutoPlayAncientsKeepOwnerRoundAndTargetGuards()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var helpers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientCardHelpers.cs");

        AssertSourceContains(
            source,
            "if (side != crossbow.Owner.Creature.Side)",
            "card.Type == CardType.Attack && card.CanBeGeneratedInCombat",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "if (player != __instance.Owner)",
            "await CardPileCmd.ShuffleIfNecessary(choiceContext, player)",
            "combatState.RoundNumber == 1",
            "cards.FirstOrDefault(card => !card.Keywords.Contains(CardKeyword.Innate))",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "await PowerCmd.Apply<StrengthPower>",
            "if (combatState.RoundNumber > 3)",
            ".Where(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay))",
            ".OrderByDescending(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card)))",
            ".ThenBy(item => item.Index)",
            "if (card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly && !card.CanPlayTargeting(target))",
            "await VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources)",
            "await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true)");

        AssertSourceContains(
            helpers,
            "card.EnergyCost.CostsX",
            "card.Owner.PlayerCombatState?.Energy ?? 0",
            "card.HasStarCostX",
            "card.Owner.PlayerCombatState?.Stars ?? 0",
            "TargetType.AnyEnemy => combatState.HittableEnemies.OrderByDescending(creature => creature.CurrentHp).FirstOrDefault()",
            "TargetType.AnyPlayer => owner.Creature",
            "public static async Task<CardPileAddResult?> TryAddGeneratedCardToCombat",
            "CombatManager.Instance.IsOverOrEnding",
            "!CombatManager.Instance.IsInProgress",
            "card.Owner?.Creature.CombatState == null",
            "RemoveUnpiledCombatCard(card)",
            "await CardPileCmd.AddGeneratedCardsToCombat([card], pileType, creator, position)",
            "var result = results.FirstOrDefault()",
            "result.cardAdded == null",
            "|| !result.success)");
    }

    [Fact]
    public void SavedStateKeysAreUniqueSerializableAndScopedToActiveMod()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var prismaticSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var paelsToothSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var jewelryBoxSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var playerStateSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var urdaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var morviSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var lothaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var ancientSourceWithoutPlayerStateHelper = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode", "Ancients"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith("AncientPlayerState.cs", StringComparison.Ordinal) &&
                           !path.EndsWith("AncientSavedStateFields.cs", StringComparison.Ordinal))
            .Select(path => File.ReadAllText(path, Encoding.UTF8));

        var keys = Regex.Matches(savedFields, "\"(?<key>EZMicroBalance[^\"]+)\"")
            .Select(match => match.Groups["key"].Value)
            .ToArray();

        Assert.Equal(14, keys.Length);
        Assert.Equal(keys.Length, keys.Distinct(StringComparer.Ordinal).Count());
        Assert.All(keys, key => Assert.StartsWith("EZMicroBalance", key, StringComparison.Ordinal));
        Assert.DoesNotContain("EzDailyContent", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            savedFields,
            "SavedSpireField<PrismaticGem, int> PrismaticGemNormalRewardCounter",
            "SavedSpireField<PaelsTooth, int> PaelsToothNonBossCombatCounter",
            "SavedSpireField<CardModel, bool> JewelryBoxNonInnateApotheosis",
            "SavedSpireField<Player, string> UrdaStateKey",
            "SavedSpireField<CardModel, string> UrdaDeckStateKey",
            "SavedSpireField<CardModel, bool> UrdaTrialPlantCard",
            "SavedSpireField<Player, string> MorviStateKey",
            "SavedSpireField<CardModel, string> MorviDeckStateKey",
            "SavedSpireField<CardModel, bool> MorviBorrowedAncientCard",
            "SavedSpireField<CardModel, bool> MorviOpenBookSealedCard",
            "SavedSpireField<Player, string> LothaStateKey",
            "SavedSpireField<CardModel, string> LothaDeckStateKey",
            "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard",
            "SavedSpireField<Player, string> AncientInitialOptionRerollStateKey",
            "\"EZMicroBalanceNormalRewardCounter\"",
            "\"EZMicroBalanceNonBossCombatCounter\"",
            "\"EZMicroBalanceJewelryBoxNonInnateApotheosis\"",
            "\"EZMicroBalanceUrdaStateKey\"",
            "\"EZMicroBalanceUrdaDeckStateKey\"",
            "\"EZMicroBalanceUrdaTrialPlantCard\"",
            "\"EZMicroBalanceMorviStateKey\"",
            "\"EZMicroBalanceMorviDeckStateKey\"",
            "\"EZMicroBalanceMorviBorrowedAncientCard\"",
            "\"EZMicroBalanceMorviOpenBookSealedCard\"",
            "\"EZMicroBalanceLothaStateKey\"",
            "\"EZMicroBalanceLothaDeckStateKey\"",
            "\"EZMicroBalanceLothaMirrorRebuttalCard\"",
            "\"EZMicroBalanceAncientInitialOptionRerollStateKey\"");

        AssertSourceContains(
            playerStateSource,
            "public static string Get(",
            "SavedSpireField<Player, string> runtimeField",
            "SavedSpireField<CardModel, string> deckField",
            "runtimeField[player] = deckState",
            "player.Deck.Cards",
            ".Where(card => card.Owner == player && !card.HasBeenRemovedFromState)",
            "deckField[card] = state",
            "!card.HasBeenRemovedFromState");

        Assert.Contains("AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem]", prismaticSource, StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth]", paelsToothSource, StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]", jewelryBoxSource, StringComparison.Ordinal);
        AssertSourceContains(
            urdaSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.UrdaStateKey",
            "AncientSavedStateFields.UrdaDeckStateKey");
        AssertSourceContains(
            morviSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.MorviStateKey",
            "AncientSavedStateFields.MorviDeckStateKey");
        AssertSourceContains(
            lothaSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.LothaStateKey",
            "AncientSavedStateFields.LothaDeckStateKey");
        Assert.DoesNotMatch(
            @"\b(?:UrdaStateKey|UrdaDeckStateKey|MorviStateKey|MorviDeckStateKey|LothaStateKey|LothaDeckStateKey)\s*\[",
            string.Join(Environment.NewLine, ancientSourceWithoutPlayerStateHelper));
    }

}
