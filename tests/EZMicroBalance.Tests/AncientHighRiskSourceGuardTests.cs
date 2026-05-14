using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void PickupRewardCompensationAndLockoutPatchesStayScoped()
    {
        var hornSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PaelsHornPhase1Patch.cs");
        var pickupSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PickupRewardPatches.cs");
        var sealSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SealOfGoldPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        AssertSourceContains(
            hornSource,
            "owner.RunState.CreateCard<Relax>(owner)",
            "CardCmd.Upgrade(upgradedRelax)",
            "await CardPileCmd.Add(normalRelax, PileType.Deck)",
            "await CardPileCmd.Add(upgradedRelax, PileType.Deck)");

        AssertSourceContains(
            pickupSource,
            "if (blackStar.Owner.RunState.CurrentActIndex < 2)",
            "RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable()",
            "await RelicCmd.Obtain(relic, blackStar.Owner)",
            "new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2)",
            "CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)",
            "CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)",
            "SozuPotionGatePatch.BeginInitialPotionFill(sozu.Owner)",
            "SozuPotionGatePatch.EndInitialPotionFill(sozu.Owner)",
            "while (sozu.Owner.HasOpenPotionSlots)",
            "PotionFactory.CreateRandomPotionOutOfCombat",
            "PotionCmd.TryToProcure(potion, sozu.Owner)",
            "if (InitialPotionFillOwners.Contains(player) && player == __instance.Owner)",
            "EctoplasmGoldGatePatch.BeginInitialGold(ectoplasm.Owner)",
            "await PlayerCmd.GainGold(250m, ectoplasm.Owner)",
            "EctoplasmGoldGatePatch.EndInitialGold(ectoplasm.Owner)",
            "if (InitialGoldOwners.Contains(player) && player == __instance.Owner)",
            "for (var i = 0; i < 2; i++)",
            "sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner)",
            "DebtCardPatch.ConfigureDebt(debt)",
            "CardPileCmd.Add(debt, PileType.Deck)");

        AssertSourceContains(
            sealSource,
            "__result += sealOfGold.DynamicVars.Energy.BaseValue",
            "__result = Task.CompletedTask");

        Assert.Contains("immediately obtain 1 random Relic", relics["BLACK_STAR.description"], StringComparison.Ordinal);
        Assert.Contains("fill all empty Potion slots", relics["SOZU.description"], StringComparison.Ordinal);
        Assert.Contains("gain 250 Gold", relics["ECTOPLASM.description"], StringComparison.Ordinal);
        Assert.Contains("Add 2 playable Debt", relics["SEAL_OF_GOLD.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void DraftAndGeneratedCardFlowsRemoveUnselectedTemporaryCards()
    {
        var pickupSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PickupRewardPatches.cs");
        var vakuSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        AssertSourceContains(
            pickupSource,
            "ModelDb.Card<BadLuck>()",
            "ModelDb.Card<Clumsy>()",
            "ModelDb.Card<Decay>()",
            "ModelDb.Card<Doubt>()",
            "ModelDb.Card<Guilty>()",
            "ModelDb.Card<Injury>()",
            "ModelDb.Card<Normality>()",
            "ModelDb.Card<Regret>()",
            "ModelDb.Card<Shame>()",
            "ModelDb.Card<Writhe>()",
            ".StableShuffle(owner.PlayerRng.Rewards)",
            ".Take(4)",
            "foreach (var unselected in curseDraft.Where(card => card != selectedCurse))",
            "claws.Owner.RunState.RemoveCard(unselected)",
            "claws.Owner.RunState.CreateCard<Wish>(claws.Owner)",
            "CardCmd.Upgrade(upgradedWish)");

        AssertSourceContains(
            vakuSource,
            "combatState.RoundNumber != 1",
            "ModelDb.AllCharacterCardPools",
            "ModelDb.CardPool<ColorlessCardPool>()",
            ".Where(IsChoicesParadoxEligibleRare)",
            ".Distinct()",
            "CardFactory.GetDistinctForCombat",
            "CardCmd.ApplyKeyword(card, CardKeyword.Retain)",
            "foreach (var card in generated.Where(card => card != selected))",
            "combatState.RemoveCard(card)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player)",
            "card.Rarity == CardRarity.Rare",
            "card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest",
            "!card.Keywords.Contains(CardKeyword.Unplayable)",
            "card.CanBeGeneratedInCombat",
            "card.CanBeGeneratedByModifiers");

        AssertSourceContains(
            debtSource,
            "case Enthralled enthralled:",
            "await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m",
            "DebtCardPatch.ConfigureDebt(debt)",
            "debt.RemoveKeyword(CardKeyword.Unplayable)",
            "debt.AddKeyword(CardKeyword.Exhaust)",
            "debt.EnergyCost.SetCustomBaseCost(1)");

        Assert.Equal("If this is in your hand, you must play it before other cards. Gain 10 Block. Eternal.", cards["ENTHRALLED.description"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
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
            "[HarmonyPatch(typeof(CardModel), \"get_CanonicalKeywords\")]",
            "__instance is not BrightestFlame",
            "CardKeyword.Exhaust",
            "[HarmonyPatch(typeof(BrightestFlame), \"get_CanonicalVars\")]",
            "dynamicVar is CardsVar cards",
            "new CardsVar(cards.IntValue + ExtraDraw)",
            "Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.",
            "upgrade draws 4",
            "Does not affect Pumpkin Candle relic vanilla behavior.");

        Assert.DoesNotContain("DrawExtraAfterVanilla", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CardPileCmd.Draw(choiceContext, 1", source, StringComparison.Ordinal);

        Assert.Equal("Quality Flame", englishCards["BRIGHTEST_FLAME.title"]);
        Assert.Contains("{Cards:diff()}", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("{Cards:diff()}", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Draw 3 cards", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("抽3张牌", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("Quality Flame / Brightest Flame", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("BrightestFlame", apiDiscovery, StringComparison.Ordinal);
    }

    [Fact]
    public void PrismaticGemOffColorReplacementKeepsNormalRewardBoundariesAllSlotsAndRunStateClean()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");

        AssertSourceContains(
            source,
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
            "GetOffColorRewardPool(player, null, excludedIds)",
            "return player.RunState.CreateCard(replacementCanonical, player)");

        Assert.DoesNotContain("var slotIndex = cardRewardOptions.Count - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ReplaceRightmostRewardSlot", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TurnStartAndAutoPlayAncientsKeepOwnerRoundAndTargetGuards()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TurnOfferAndRestPatches.cs");
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
        var prismaticSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var paelsToothSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PaelsToothAndForgePatches.cs");
        var jewelryBoxSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var playerStateSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var urdaSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var morviSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var lothaSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var ancientSourceWithoutPlayerStateHelper = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode", "Ancients"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith("AncientPlayerState.cs", StringComparison.Ordinal) &&
                           !path.EndsWith("AncientSavedStateFields.cs", StringComparison.Ordinal))
            .Select(path => File.ReadAllText(path, Encoding.UTF8));

        var keys = Regex.Matches(savedFields, "\"(?<key>EZMicroBalance[^\"]+)\"")
            .Select(match => match.Groups["key"].Value)
            .ToArray();

        Assert.Equal(13, keys.Length);
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
            "\"EZMicroBalanceLothaMirrorRebuttalCard\"");

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

    [Fact]
    public void ManualAncientRuntimeEvidenceRemainsExplicitlyPending()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var ancientMatrix = SliceBetween(
            manualMatrix,
            "## Ancient Reward Matrix",
            "## Simplified Chinese Localization Spot Checks");

        Assert.Contains("- [x] Every implemented Ancient reward change has a manual checklist row.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", releaseChecklist, StringComparison.Ordinal);

        Assert.Contains("| Prismatic Gem |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Meat Cleaver |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Blood-Soaked Rose / Enthralled |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("Pending", ancientMatrix, StringComparison.Ordinal);
        Assert.DoesNotContain("| Pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Result: pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("manually verified", ancientMatrix, StringComparison.OrdinalIgnoreCase);
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

    private static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");

        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end >= 0, $"Missing end marker: {endMarker}");

        return source[start..end];
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
