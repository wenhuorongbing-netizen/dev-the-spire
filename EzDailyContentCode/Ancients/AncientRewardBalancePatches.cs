using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace EzDailyContent.EzDailyContentCode.Ancients;

internal static class AncientSavedStateFields
{
    public static readonly SavedSpireField<PrismaticGem, int> PrismaticGemNormalRewardCounter =
        new(() => 0, "EzDailyContentNormalRewardCounter");

    public static readonly SavedSpireField<PaelsTooth, int> PaelsToothNonBossCombatCounter =
        new(() => 0, "EzDailyContentNonBossCombatCounter");
}

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]
internal static class AncientPickupBalancePatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref Task __result)
    {
        switch (__instance)
        {
            case BlackStar blackStar:
                __result = GrantBlackStarActThreeCompensation(blackStar);
                return false;
            case WarHammer warHammer:
                __result = UpgradeTwoCardsOnWarHammerPickup(warHammer);
                return false;
            case Sozu sozu:
                __result = FillPotionSlotsForSozu(sozu);
                return false;
            case Ectoplasm ectoplasm:
                __result = GainInitialGoldForEctoplasm(ectoplasm);
                return false;
            case SealOfGold sealOfGold:
                __result = AddDebtsForSealOfGold(sealOfGold);
                return false;
            case Claws claws:
                __result = ChooseCurseAndAddWishes(claws);
                return false;
            case JeweledMask jeweledMask:
                __result = ChoosePermanentFreePower(jeweledMask);
                return false;
            default:
                return true;
        }
    }

    private static async Task GrantBlackStarActThreeCompensation(BlackStar blackStar)
    {
        if (blackStar.Owner.RunState.CurrentActIndex < 2)
        {
            return;
        }

        var relic = RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable();
        await RelicCmd.Obtain(relic, blackStar.Owner);
        MainFile.Logger.Info($"[EZMicroBalance] BlackStar applied: act 3+ immediate relic {relic.Id.Entry}.");
    }

    private static async Task UpgradeTwoCardsOnWarHammerPickup(WarHammer warHammer)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2);
        var cards = (await CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)).ToList();
        CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        MainFile.Logger.Info($"[EZMicroBalance] WarHammer applied: upgraded {cards.Count} card(s) on pickup.");
    }

    private static async Task FillPotionSlotsForSozu(Sozu sozu)
    {
        var generated = new List<PotionModel>();
        SozuPotionGatePatch.AllowInitialPotionFill = true;
        try
        {
            while (sozu.Owner.HasOpenPotionSlots)
            {
                var potion = PotionFactory.CreateRandomPotionOutOfCombat(
                    sozu.Owner,
                    sozu.Owner.PlayerRng.Rewards,
                    sozu.Owner.Potions.Concat(generated)).ToMutable();
                generated.Add(potion);

                var result = await PotionCmd.TryToProcure(potion, sozu.Owner);
                if (!result.success)
                {
                    break;
                }
            }
        }
        finally
        {
            SozuPotionGatePatch.AllowInitialPotionFill = false;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Sozu applied: filled {generated.Count} potion slot(s) on pickup.");
    }

    private static async Task GainInitialGoldForEctoplasm(Ectoplasm ectoplasm)
    {
        EctoplasmGoldGatePatch.AllowInitialGold = true;
        try
        {
            await PlayerCmd.GainGold(250m, ectoplasm.Owner);
        }
        finally
        {
            EctoplasmGoldGatePatch.AllowInitialGold = false;
        }

        MainFile.Logger.Info("[EZMicroBalance] Ectoplasm applied: gained 250 initial gold.");
    }

    private static async Task AddDebtsForSealOfGold(SealOfGold sealOfGold)
    {
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < 2; i++)
        {
            var debt = sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner);
            DebtCardPatch.ConfigureDebt(debt);
            results.Add(await CardPileCmd.Add(debt, PileType.Deck));
        }

        CardCmd.PreviewCardPileAdd(results, 2f);
        MainFile.Logger.Info("[EZMicroBalance] SealOfGold applied: added 2 Debt cards on pickup.");
    }

    private static async Task ChooseCurseAndAddWishes(Claws claws)
    {
        var curseDraft = CreateClawsCurseDraft(claws.Owner);
        var selectedCurse = (await CardSelectCmd.FromChooseABundleScreen(
                claws.Owner,
                curseDraft.Select(card => (IReadOnlyList<CardModel>)new[] { card }).ToList()))
            .FirstOrDefault();

        var addedCards = new List<CardPileAddResult>();
        if (selectedCurse != null)
        {
            addedCards.Add(await CardPileCmd.Add(selectedCurse, PileType.Deck));
        }

        foreach (var unselected in curseDraft.Where(card => card != selectedCurse))
        {
            claws.Owner.RunState.RemoveCard(unselected);
        }

        for (var i = 0; i < 2; i++)
        {
            addedCards.Add(await CardPileCmd.Add(claws.Owner.RunState.CreateCard<Wish>(claws.Owner), PileType.Deck));
        }

        var upgradedWish = claws.Owner.RunState.CreateCard<Wish>(claws.Owner);
        CardCmd.Upgrade(upgradedWish);
        addedCards.Add(await CardPileCmd.Add(upgradedWish, PileType.Deck));

        CardCmd.PreviewCardPileAdd(addedCards, 2f);
        MainFile.Logger.Info($"[EZMicroBalance] Claws applied: added curse {selectedCurse?.Id.Entry ?? "NONE"}, 2 Wish, and 1 upgraded Wish+.");
    }

    private static List<CardModel> CreateClawsCurseDraft(Player owner)
    {
        return new CardModel[]
            {
                ModelDb.Card<BadLuck>(),
                ModelDb.Card<Clumsy>(),
                ModelDb.Card<Decay>(),
                ModelDb.Card<Doubt>(),
                ModelDb.Card<Guilty>(),
                ModelDb.Card<Injury>(),
                ModelDb.Card<Normality>(),
                ModelDb.Card<Regret>(),
                ModelDb.Card<Shame>(),
                ModelDb.Card<Writhe>()
            }
            .ToList()
            .StableShuffle(owner.PlayerRng.Rewards)
            .Take(4)
            .Select(canonical => owner.RunState.CreateCard(canonical, owner))
            .ToList();
    }

    private static async Task ChoosePermanentFreePower(JeweledMask jeweledMask)
    {
        var owner = jeweledMask.Owner;
        var deckSelectionPrefs = new CardSelectorPrefs(new LocString("relics", "JEWELED_MASK.ezSelectionScreenPrompt"), 0, 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var selected = (await CardSelectCmd.FromDeckGeneric(
                owner,
                deckSelectionPrefs,
                card => card.Type == CardType.Power && card.Enchantment == null))
            .FirstOrDefault();

        if (selected == null)
        {
            selected = await DraftGeneratedPowerForJeweledMask(owner);
        }

        if (selected == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] JeweledMask skipped: no eligible unenchanted deck or generated power target.");
            return;
        }

        CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m);
        MainFile.Logger.Info($"[EZMicroBalance] JeweledMask applied: marked {selected.Id.Entry} as permanent 0-cost combat-start power.");
    }

    private static async Task<CardModel?> DraftGeneratedPowerForJeweledMask(Player owner)
    {
        var pool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Power && card.CanBeGeneratedByModifiers)
            .ToList()
            .StableShuffle(owner.PlayerRng.Rewards)
            .Take(3)
            .Select(canonical => owner.RunState.CreateCard(canonical, owner))
            .ToList();

        if (pool.Count == 0)
        {
            return null;
        }

        var selected = (await CardSelectCmd.FromChooseABundleScreen(
                owner,
                pool.Select(card => (IReadOnlyList<CardModel>)new[] { card }).ToList()))
            .FirstOrDefault();

        foreach (var unselected in pool.Where(card => card != selected))
        {
            owner.RunState.RemoveCard(unselected);
        }

        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Deck);
        }

        return selected;
    }
}

[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]
internal static class SozuPotionGatePatch
{
    public static bool AllowInitialPotionFill { get; set; }

    [HarmonyPrefix]
    private static bool Prefix(Sozu __instance, Player player, ref bool __result)
    {
        if (AllowInitialPotionFill && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ShouldGainGold))]
internal static class EctoplasmGoldGatePatch
{
    public static bool AllowInitialGold { get; set; }

    [HarmonyPrefix]
    private static bool Prefix(Ectoplasm __instance, Player player, ref bool __result)
    {
        if (AllowInitialGold && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]
internal static class PrismaticGemPoolPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCreationOptions options, ref CardCreationOptions __result)
    {
        __result = options;
        return false;
    }
}

[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]
internal static class PrismaticGemRewardScreenContextPatch
{
    [ThreadStatic]
    private static Stack<CardReward>? PopulateStack;

    internal static CardReward? CurrentReward =>
        PopulateStack is { Count: > 0 } ? PopulateStack.Peek() : null;

    [HarmonyPrefix]
    private static void Prefix(CardReward __instance)
    {
        (PopulateStack ??= new Stack<CardReward>()).Push(__instance);
    }

    [HarmonyFinalizer]
    private static void Finalizer(CardReward __instance)
    {
        if (PopulateStack is not { Count: > 0 })
        {
            return;
        }

        if (ReferenceEquals(PopulateStack.Peek(), __instance))
        {
            PopulateStack.Pop();
            return;
        }

        PopulateStack.Clear();
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]
internal static class PrismaticGemRewardPatch
{
    private sealed class RewardScreenState
    {
        public bool HasTriggerDecision { get; set; }

        public bool ShouldReplaceRightmostSlot { get; set; }

        public int CounterAtDecision { get; set; }
    }

    private static readonly ConditionalWeakTable<CardReward, RewardScreenState> RewardStates = new();

    [HarmonyPrefix]
    private static bool Prefix(
        AbstractModel __instance,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions,
        ref bool __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = TryReplaceRightmostNormalReward(prismaticGem, player, cardRewardOptions, creationOptions);
        return false;
    }

    private static bool TryReplaceRightmostNormalReward(
        PrismaticGem prismaticGem,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (player != prismaticGem.Owner || cardRewardOptions.Count == 0)
        {
            return false;
        }

        var isNormalCardReward = IsNormalCardReward(creationOptions);
        var rewardScreen = PrismaticGemRewardScreenContextPatch.CurrentReward;
        if (rewardScreen == null)
        {
            if (isNormalCardReward)
            {
                MainFile.Logger.Warn("[EZMicroBalance] PrismaticGem skipped: normal card reward modification had no CardReward screen context.");
            }

            return false;
        }

        var screenState = RewardStates.GetValue(rewardScreen, _ => new RewardScreenState());
        var madeTriggerDecision = !screenState.HasTriggerDecision;
        if (madeTriggerDecision)
        {
            screenState.HasTriggerDecision = true;
            if (!isNormalCardReward)
            {
                MainFile.Logger.Info("[EZMicroBalance] PrismaticGem ignored non-normal card reward screen; no counter increment.");
                return false;
            }

            screenState.CounterAtDecision = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] + 1;
            AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] = screenState.CounterAtDecision;
            screenState.ShouldReplaceRightmostSlot = screenState.CounterAtDecision % 2 == 0;
        }
        else if (!isNormalCardReward)
        {
            return false;
        }

        if (!screenState.ShouldReplaceRightmostSlot)
        {
            if (madeTriggerDecision)
            {
                MainFile.Logger.Info($"[EZMicroBalance] PrismaticGem applied: counted normal card reward {screenState.CounterAtDecision}; no replacement for this reward screen.");
            }

            return false;
        }

        var rightmostReward = cardRewardOptions[^1];
        var originalCard = rightmostReward.Card;
        var candidatePool = GetOffColorRewardPool(player, originalCard.Rarity, cardRewardOptions)
            .ToList()
            .StableShuffle(player.PlayerRng.Rewards);
        var replacementCanonical = candidatePool.FirstOrDefault();
        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem skipped: no off-color {originalCard.Rarity} card available for reward {screenState.CounterAtDecision}.");
            return false;
        }

        var replacement = player.RunState.CreateCard(replacementCanonical, player);
        if (originalCard.IsUpgraded && replacement.IsUpgradable)
        {
            CardCmd.Upgrade(replacement);
        }

        rightmostReward.ModifyCard(replacement, prismaticGem);
        if (player.RunState.ContainsCard(originalCard))
        {
            player.RunState.RemoveCard(originalCard);
        }

        prismaticGem.Flash();
        MainFile.Logger.Info($"[EZMicroBalance] PrismaticGem applied: replaced rightmost reward {originalCard.Id.Entry} with off-color {replacement.Id.Entry} on normal reward {screenState.CounterAtDecision}.");
        return true;
    }

    private static bool IsNormalCardReward(CardCreationOptions creationOptions)
    {
        if (!creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward))
        {
            return false;
        }

        if (creationOptions.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications) ||
            creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications))
        {
            return false;
        }

        return creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter &&
            creationOptions.CustomCardPool == null &&
            creationOptions.CardPoolFilter == null &&
            creationOptions.CardPools.Count > 0 &&
            !creationOptions.CardPools.All(pool => pool.IsColorless);
    }

    private static IEnumerable<CardModel> GetOffColorRewardPool(
        Player player,
        CardRarity rarity,
        IEnumerable<CardCreationResult> currentRewards)
    {
        var homePool = player.Character.CardPool;
        var excludedIds = currentRewards
            .Select(result => result.Card.Id)
            .ToHashSet();

        return ModelDb.AllCharacterCardPools
            .Where(pool => !pool.Id.Equals(homePool.Id) && !pool.IsColorless)
            .SelectMany(pool => pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(card => card.Rarity == rarity)
            .Where(card => card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest)
            .Where(card => card.CanBeGeneratedByModifiers)
            .Where(card => !excludedIds.Contains(card.Id))
            .Distinct();
    }
}

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]
internal static class PaelsToothPickupPatch
{
    [HarmonyPostfix]
    private static void Postfix(PaelsTooth __instance, ref Task __result)
    {
        __result = ResetCounterAfterPickup(__instance, __result);
    }

    private static async Task ResetCounterAfterPickup(PaelsTooth paelsTooth, Task original)
    {
        await original;
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
        MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: stored {paelsTooth.SerializableCards.Count} removed card(s) and reset combat counter.");
    }
}

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]
internal static class PaelsToothCombatPatch
{
    private static readonly System.Reflection.MethodInfo UpdateCardListMethod =
        AccessTools.Method(typeof(PaelsTooth), "UpdateCardList");

    [HarmonyPrefix]
    private static bool Prefix(PaelsTooth __instance, CombatRoom room, ref Task __result)
    {
        __result = AfterCombatEnd(__instance, room);
        return false;
    }

    private static async Task AfterCombatEnd(PaelsTooth paelsTooth, CombatRoom room)
    {
        if (paelsTooth.Owner.Creature.IsDead)
        {
            return;
        }

        if (paelsTooth.SerializableCards.Count == 0)
        {
            AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
            RefreshStoredCardDisplay(paelsTooth);
            return;
        }

        if (room.RoomType == RoomType.Boss)
        {
            ClearStoredCards(paelsTooth, "act boss combat ended");
            return;
        }

        var counter = AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] + 1;
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = counter;
        if (counter < 2)
        {
            MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: counted non-boss combat {counter}/2 before next upgraded return.");
            return;
        }

        var returnedCard = await ChooseAndReturnStoredCard(paelsTooth);
        if (returnedCard != null)
        {
            AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
            MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: returned upgraded stored card {returnedCard.Id.Entry}; {paelsTooth.SerializableCards.Count} stored card(s) remain.");
        }
    }

    private static async Task<CardModel?> ChooseAndReturnStoredCard(PaelsTooth paelsTooth)
    {
        var previews = new List<(SerializableCard Saved, CardModel Card)>();
        foreach (var savedCard in paelsTooth.SerializableCards)
        {
            var card = CardModel.FromSerializable(savedCard);
            if (!paelsTooth.Owner.RunState.ContainsCard(card))
            {
                paelsTooth.Owner.RunState.AddCard(card, paelsTooth.Owner);
            }

            previews.Add((savedCard, card));
        }

        var selected = (await CardSelectCmd.FromChooseABundleScreen(
                paelsTooth.Owner,
                previews.Select(preview => (IReadOnlyList<CardModel>)new[] { preview.Card }).ToList()))
            .FirstOrDefault();

        foreach (var preview in previews.Where(preview => preview.Card != selected))
        {
            if (paelsTooth.Owner.RunState.ContainsCard(preview.Card))
            {
                paelsTooth.Owner.RunState.RemoveCard(preview.Card);
            }
        }

        var selectedPreview = previews.FirstOrDefault(preview => preview.Card == selected);
        if (selected == null || selectedPreview.Saved == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] PaelsTooth skipped: stored-card choice returned no selection.");
            return null;
        }

        paelsTooth.Flash();
        if (selected.IsUpgradable)
        {
            CardCmd.Upgrade(selected, CardPreviewStyle.MessyLayout);
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        CardCmd.PreviewCardPileAdd(addResult);
        paelsTooth.SerializableCards.Remove(selectedPreview.Saved);
        RefreshStoredCardDisplay(paelsTooth);
        return selected;
    }

    public static void ClearStoredCards(PaelsTooth paelsTooth, string reason)
    {
        var remaining = paelsTooth.SerializableCards.Count;
        paelsTooth.SerializableCards.Clear();
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
        RefreshStoredCardDisplay(paelsTooth);
        MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: cleared {remaining} stored card(s) after {reason}.");
    }

    private static void RefreshStoredCardDisplay(PaelsTooth paelsTooth)
    {
        UpdateCardListMethod.Invoke(paelsTooth, Array.Empty<object>());
    }
}

[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]
internal static class SovereignBladeForgeExhaustPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task<IEnumerable<SovereignBlade>> __result)
    {
        __result = AddExhaustToForgedBlades(player, __result);
    }

    private static async Task<IEnumerable<SovereignBlade>> AddExhaustToForgedBlades(
        Player player,
        Task<IEnumerable<SovereignBlade>> original)
    {
        var blades = (await original).ToList();
        var modifiedCount = 0;
        foreach (var blade in blades.Where(blade =>
                     blade.Owner == player &&
                     blade.CreatedThroughForge &&
                     !blade.Keywords.Contains(CardKeyword.Exhaust)))
        {
            CardCmd.ApplyKeyword(blade, CardKeyword.Exhaust);
            modifiedCount++;
        }

        if (modifiedCount > 0)
        {
            MainFile.Logger.Info($"[EZMicroBalance] SovereignBlade applied: added Exhaust to {modifiedCount} forged temporary blade(s).");
        }

        return blades;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]
internal static class PaelsToothActTransitionPatch
{
    [HarmonyPrefix]
    private static bool Prefix(AbstractModel __instance, ref Task __result)
    {
        if (__instance is not PaelsTooth paelsTooth || paelsTooth.SerializableCards.Count == 0)
        {
            return true;
        }

        PaelsToothCombatPatch.ClearStoredCards(paelsTooth, "act transition");
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.ModifyMaxEnergy))]
internal static class SealOfGoldMaxEnergyPatch
{
    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, Player player, ref decimal __result)
    {
        if (__instance is SealOfGold sealOfGold && player == sealOfGold.Owner)
        {
            __result += sealOfGold.DynamicVars.Energy.BaseValue;
        }
    }
}

[HarmonyPatch(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))]
internal static class SealOfGoldTurnPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]
internal static class JewelryBoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(JewelryBox __instance, ref Task __result)
    {
        __result = AddNonInnateApotheosis(__instance);
        return false;
    }

    private static async Task AddNonInnateApotheosis(JewelryBox jewelryBox)
    {
        var card = jewelryBox.Owner.RunState.CreateCard<Apotheosis>(jewelryBox.Owner);
        AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate);
        var result = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] JewelryBox applied: added Apotheosis without Innate.");
    }
}

[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]
internal static class PreservedFogPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PreservedFog __instance, ref Task __result)
    {
        __result = RemoveFourCardsAndAddPersistentFolly(__instance);
        return false;
    }

    private static async Task RemoveFourCardsAndAddPersistentFolly(PreservedFog preservedFog)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4);
        foreach (var card in await CardSelectCmd.FromDeckForRemoval(preservedFog.Owner, prefs))
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        var folly = preservedFog.Owner.RunState.CreateCard<Folly>(preservedFog.Owner);
        AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain);
        var result = await CardPileCmd.Add(folly, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] PreservedFog applied: removed up to 4 cards and added Folly without Ethereal/Retain.");
    }
}

[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]
internal static class FollyKeywordsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate };
        return false;
    }
}

[HarmonyPatch(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))]
internal static class ChoicesParadoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ChoicesParadox __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = ChooseRareTemporaryCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task ChooseRareTemporaryCard(ChoicesParadox choicesParadox, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.RoundNumber != 1)
        {
            return;
        }

        var pool = ModelDb.AllCharacterCardPools
            .Concat(new[] { ModelDb.CardPool<ColorlessCardPool>() })
            .SelectMany(cardPool => cardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(IsChoicesParadoxEligibleRare)
            .Distinct()
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(
                player,
                pool,
                choicesParadox.DynamicVars.Cards.IntValue,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (generated.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] ChoicesParadox skipped: no eligible rare combat cards generated.");
            return;
        }

        choicesParadox.Flash();
        foreach (var card in generated)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                generated,
                player,
                new CardSelectorPrefs(new LocString("relics", "CHOICES_PARADOX.selectionScreenPrompt"), 1)))
            .FirstOrDefault();

        foreach (var card in generated.Where(card => card != selected))
        {
            combatState.RemoveCard(card);
        }

        if (selected != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, player);
        }

        MainFile.Logger.Info($"[EZMicroBalance] ChoicesParadox applied: offered {generated.Count} rare card(s), selected {selected?.Id.Entry ?? "NONE"}.");
    }

    private static bool IsChoicesParadoxEligibleRare(CardModel card)
    {
        return card.Rarity == CardRarity.Rare &&
            card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest &&
            !card.Keywords.Contains(CardKeyword.Unplayable) &&
            card.CanBeGeneratedInCombat &&
            card.CanBeGeneratedByModifiers;
    }
}

[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]
internal static class JeweledMaskCombatStartPatch
{
    [HarmonyPrefix]
    private static bool Prefix(JeweledMask __instance, Player player, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner || combatState.RoundNumber > 1)
        {
            return true;
        }

        __result = PullMarkedPowerToHand(__instance, player);
        return false;
    }

    private static async Task PullMarkedPowerToHand(JeweledMask jeweledMask, Player player)
    {
        var drawPile = PileType.Draw.GetPile(player);
        var markedPower = drawPile.Cards.FirstOrDefault(AncientCardHelpers.IsJeweledMaskPower);
        if (markedPower != null)
        {
            jeweledMask.Flash();
            await CardPileCmd.Add(markedPower, PileType.Hand);
            MainFile.Logger.Info($"[EZMicroBalance] JeweledMask applied: moved marked power {markedPower.Id.Entry} from draw pile to hand.");
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Any(AncientCardHelpers.IsJeweledMaskPower))
        {
            MainFile.Logger.Info("[EZMicroBalance] JeweledMask skipped pull: marked power already in hand.");
            return;
        }

        MainFile.Logger.Info("[EZMicroBalance] JeweledMask skipped pull: no marked power in draw pile or hand.");
    }
}

[HarmonyPatch(typeof(Fiddle), "get_CanonicalVars")]
internal static class FiddleVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(AncientCardHelpers.FiddleHandLimit) };
        return false;
    }
}

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))]
internal static class FiddleHandDrawPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, ref decimal __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        var handCount = PileType.Hand.GetPile(player).Cards.Count;
        __result = Math.Max(0, AncientCardHelpers.FiddleHandLimit - handCount);
        return false;
    }
}

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]
internal static class FiddleShouldDrawPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, ref bool __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]
internal static class FiddleDrawCapPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref decimal count, Player player, bool fromHandDraw, ref Task<IEnumerable<CardModel>> __result)
    {
        if (fromHandDraw || player.GetRelic<Fiddle>() == null)
        {
            return true;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.CurrentSide != player.Creature.Side)
        {
            return true;
        }

        var remainingRoom = AncientCardHelpers.FiddleHandLimit - PileType.Hand.GetPile(player).Cards.Count;
        if (remainingRoom <= 0)
        {
            __result = Task.FromResult<IEnumerable<CardModel>>(Array.Empty<CardModel>());
            MainFile.Logger.Info("[EZMicroBalance] Fiddle applied: prevented draw above 7-card player-turn hand cap.");
            return false;
        }

        count = Math.Min(count, remainingRoom);
        return true;
    }
}

[HarmonyPatch(typeof(IronClub), "get_CanonicalVars")]
internal static class IronClubVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(5) };
        return false;
    }
}

[HarmonyPatch(typeof(BrilliantScarf), "get_CanonicalVars")]
internal static class BrilliantScarfVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(6) };
        return false;
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), "get_CanonicalVars")]
internal static class BeautifulBraceletVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(3), new DynamicVar("Swift", 2m) };
        return false;
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]
internal static class BeautifulBraceletPatch
{
    [HarmonyPrefix]
    private static bool Prefix(BeautifulBracelet __instance, ref Task __result)
    {
        __result = AddSwiftTwo(__instance);
        return false;
    }

    private static async Task AddSwiftTwo(BeautifulBracelet bracelet)
    {
        var swift = ModelDb.Enchantment<Swift>();
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, bracelet.DynamicVars.Cards.IntValue);
        var cards = (await CardSelectCmd.FromDeckForEnchantment(bracelet.Owner, swift, 2, prefs)).ToList();
        foreach (var card in cards)
        {
            CardCmd.Enchant<Swift>(card, 2m);
        }

        MainFile.Logger.Info($"[EZMicroBalance] BeautifulBracelet applied: enchanted {cards.Count} card(s) with Swift 2.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]
internal static class MusicBoxBeforeCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]
internal static class MusicBoxStateTracker
{
    private sealed class State
    {
        public bool WasUsedThisTurn { get; set; }
    }

    private static readonly ConditionalWeakTable<MusicBox, State> States = new();

    public static bool WasUsedThisTurn(MusicBox musicBox)
    {
        return States.GetOrCreateValue(musicBox).WasUsedThisTurn;
    }

    public static void MarkUsed(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = true;
    }

    public static void Reset(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = false;
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]
internal static class MusicBoxAfterCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(MusicBox __instance, CardPlay cardPlay, ref Task __result)
    {
        __result = AfterCardPlayed(__instance, cardPlay);
        return false;
    }

    private static async Task AfterCardPlayed(MusicBox musicBox, CardPlay cardPlay)
    {
        if (MusicBoxStateTracker.WasUsedThisTurn(musicBox) ||
            cardPlay.Card.Owner != musicBox.Owner ||
            cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        musicBox.Flash();
        var copy = cardPlay.Card.CreateClone();
        AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1);
        AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust);
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner);
        MusicBoxStateTracker.MarkUsed(musicBox);
        MainFile.Logger.Info("[EZMicroBalance] MusicBox applied: created attack copy with -1 cost, Ethereal, and Exhaust.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]
internal static class MusicBoxTurnResetPatch
{
    [HarmonyPostfix]
    private static void ResetOnTurnStart(MusicBox __instance, CombatSide side)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            MusicBoxStateTracker.Reset(__instance);
        }
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]
internal static class MusicBoxCombatResetPatch
{
    [HarmonyPostfix]
    private static void ResetAfterCombat(MusicBox __instance)
    {
        MusicBoxStateTracker.Reset(__instance);
    }
}

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.BeforeSideTurnStart))]
internal static class CrossbowOfferPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (__instance is not Crossbow crossbow)
        {
            return true;
        }

        if (side != crossbow.Owner.Creature.Side)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = OfferTemporaryAttack(crossbow, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTemporaryAttack(Crossbow crossbow, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        var owner = crossbow.Owner;
        var attackPool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Attack && card.CanBeGeneratedInCombat)
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generated == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Crossbow skipped: no eligible attack generated.");
            return;
        }

        AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1);
        AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { generated }, owner, canSkip: true);
        if (selected == generated)
        {
            crossbow.Flash();
            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, owner);
            MainFile.Logger.Info($"[EZMicroBalance] Crossbow applied: accepted temporary attack {generated.Id.Entry}.");
            return;
        }

        combatState.RemoveCard(generated);
        MainFile.Logger.Info($"[EZMicroBalance] Crossbow applied: skipped temporary attack {generated.Id.Entry}.");
    }
}

[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]
internal static class CrossbowVanillaAfterTurnPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]
internal static class ToastyMittensPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ToastyMittens __instance, Player player, PlayerChoiceContext choiceContext, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = OfferTopCardExhaust(__instance, player, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTopCardExhaust(ToastyMittens mittens, Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
        var cards = PileType.Draw.GetPile(player).Cards;
        var topCard = combatState.RoundNumber == 1
            ? cards.FirstOrDefault(card => !card.Keywords.Contains(CardKeyword.Innate))
            : null;
        topCard ??= cards.FirstOrDefault();

        if (topCard == null)
        {
            MainFile.Logger.Info("[EZMicroBalance] ToastyMittens skipped: no draw-pile card to offer.");
            return;
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true);
        if (selected != topCard)
        {
            MainFile.Logger.Info($"[EZMicroBalance] ToastyMittens applied: kept top card {topCard.Id.Entry}.");
            return;
        }

        mittens.Flash();
        await CardCmd.Exhaust(choiceContext, topCard);
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, mittens.DynamicVars.Strength.BaseValue, player.Creature, null);
        MainFile.Logger.Info($"[EZMicroBalance] ToastyMittens applied: exhausted {topCard.Id.Entry} and gained Strength.");
    }
}

[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]
internal static class WhisperingEarringPatch
{
    [HarmonyPrefix]
    private static bool Prefix(WhisperingEarring __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = AutoPlayOneHighestCostCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task AutoPlayOneHighestCostCard(WhisperingEarring earring, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (combatState.RoundNumber > 3)
        {
            return;
        }

        var card = PileType.Hand.GetPile(player).Cards
            .Select((card, index) => new { Card = card, Index = index })
            .Where(item => item.Card.CanPlay())
            .OrderByDescending(item => AncientCardHelpers.EffectiveCost(item.Card))
            .ThenBy(item => item.Index)
            .Select(item => item.Card)
            .FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var target = AncientCardHelpers.GetPreferredTarget(card, combatState, player);
        if (!card.CanPlayTargeting(target))
        {
            return;
        }

        earring.Flash();
        await card.SpendResources();
        await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
        MainFile.Logger.Info($"[EZMicroBalance] WhisperingEarring applied: auto-played {card.Id.Entry} on round {combatState.RoundNumber}.");
    }
}

[HarmonyPatch(typeof(PumpkinCandle), nameof(PumpkinCandle.AfterRoomEntered))]
internal static class PumpkinCandlePatch
{
    private const int ExtinguishedSentinel = -2;

    [HarmonyPrefix]
    private static bool Prefix(PumpkinCandle __instance, ref Task __result)
    {
        if (__instance.ActiveAct >= 0 &&
            __instance.Owner.RunState.CurrentActIndex >= 2 &&
            __instance.ActiveAct != __instance.Owner.RunState.CurrentActIndex)
        {
            __result = ExtinguishAndUpgrade(__instance);
            return false;
        }

        return true;
    }

    private static Task ExtinguishAndUpgrade(PumpkinCandle candle)
    {
        var cards = PileType.Deck.GetPile(candle.Owner).Cards
            .Where(card => card.IsUpgradable)
            .ToList()
            .StableShuffle(candle.Owner.RunState.Rng.Niche)
            .Take(2)
            .ToList();
        if (cards.Count > 0)
        {
            candle.Flash();
            CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        }

        candle.ActiveAct = ExtinguishedSentinel;
        candle.Status = RelicStatus.Disabled;
        MainFile.Logger.Info($"[EZMicroBalance] PumpkinCandle applied: extinguished and upgraded {cards.Count} card(s).");
        return Task.CompletedTask;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), MethodType.Constructor, typeof(Player))]
internal static class MeatCleaverCookCtorPatch
{
    [HarmonyPostfix]
    private static void Postfix(CookRestSiteOption __instance, Player owner)
    {
        if (owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner))
        {
            __instance.IsEnabled = false;
        }
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), "get_Description")]
internal static class MeatCleaverCookDescriptionPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref LocString __result)
    {
        var owner = MeatCleaverCookPatch.GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = new LocString(
            "rest_site_ui",
            __instance.IsEnabled ? "OPTION_COOK.ezDescription" : "OPTION_COOK.ezDescriptionDisabled");
        __result.Add("Cards", MeatCleaverCookPatch.CardsToRemove);
        __result.Add("Hp", MeatCleaverCookPatch.HpToLose);
        return false;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]
internal static class MeatCleaverCookPatch
{
    public const int CardsToRemove = 2;

    public const int HpToLose = 5;

    private static readonly System.Reflection.MethodInfo OwnerGetter =
        AccessTools.PropertyGetter(typeof(RestSiteOption), "Owner");

    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref Task<bool> __result)
    {
        var owner = GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = Cook(owner);
        return false;
    }

    public static Player GetOwner(RestSiteOption option)
    {
        return (Player)OwnerGetter.Invoke(option, Array.Empty<object>())!;
    }

    public static bool CanCook(Player owner)
    {
        return owner.Creature.CurrentHp > HpToLose &&
            PileType.Deck.GetPile(owner).Cards.Count(card => card.IsRemovable) >= CardsToRemove;
    }

    private static async Task<bool> Cook(Player owner)
    {
        if (!CanCook(owner))
        {
            MainFile.Logger.Info("[EZMicroBalance] MeatCleaver skipped: cook unavailable due to HP or removable-card count.");
            return false;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, CardsToRemove)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var cards = (await CardSelectCmd.FromDeckForRemoval(owner, prefs)).ToList();
        if (cards.Count != CardsToRemove)
        {
            return false;
        }

        foreach (var card in cards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose);
        MainFile.Logger.Info("[EZMicroBalance] MeatCleaver applied: cooked by removing 2 cards and losing 5 HP.");
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterCreated))]
internal static class DebtAfterCreatedPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance)
    {
        if (__instance is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.FromSerializable))]
internal static class DebtFromSavePatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __result)
    {
        if (__result is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

[HarmonyPatch(typeof(Debt), "get_CanonicalKeywords")]
internal static class DebtKeywordsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new CardKeyword[] { CardKeyword.Exhaust };
        return false;
    }
}

[HarmonyPatch(typeof(Debt), "get_CanonicalVars")]
internal static class DebtVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new GoldVar(5) };
        return false;
    }
}

[HarmonyPatch(typeof(Debt), "get_HasTurnEndInHandEffect")]
internal static class DebtTurnEndEffectPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(Debt), nameof(Debt.OnTurnEndInHand))]
internal static class DebtTurnEndInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "OnPlay")]
internal static class CardModelOnPlayPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, CardPlay cardPlay, ref Task __result)
    {
        switch (__instance)
        {
            case Debt debt:
                __result = PlayDebt(debt);
                return false;
            case Enthralled enthralled:
                __result = PlayEnthralled(enthralled, cardPlay);
                return false;
            default:
                return true;
        }
    }

    private static Task PlayDebt(Debt debt)
    {
        debt.ExhaustOnNextPlay = true;
        MainFile.Logger.Info("[EZMicroBalance] Debt applied: will exhaust after play.");
        return Task.CompletedTask;
    }

    private static async Task PlayEnthralled(Enthralled enthralled, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m, ValueProp.Move, cardPlay);
        MainFile.Logger.Info("[EZMicroBalance] Enthralled applied: gained 10 block.");
    }
}

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]
internal static class DebtExhaustPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel card)
    {
        if (card is Debt debt)
        {
            DebtCardPatch.LoseGoldForDebt(debt);
        }
    }
}

internal static class DebtCardPatch
{
    public static void ConfigureDebt(Debt debt)
    {
        AncientCardHelpers.EnsureKeywordsInitialized(debt);
        if (debt.Keywords.Contains(CardKeyword.Unplayable))
        {
            debt.RemoveKeyword(CardKeyword.Unplayable);
        }

        if (!debt.Keywords.Contains(CardKeyword.Exhaust))
        {
            debt.AddKeyword(CardKeyword.Exhaust);
        }

        if (!debt.EnergyCost.CostsX)
        {
            debt.EnergyCost.SetCustomBaseCost(1);
        }
    }

    public static void LoseGoldForDebt(Debt debt)
    {
        var goldToLose = Math.Min(5, debt.Owner.Gold);
        if (goldToLose > 0)
        {
            PlayerCmd.LoseGold(goldToLose, debt.Owner).GetAwaiter().GetResult();
        }

        MainFile.Logger.Info($"[EZMicroBalance] Debt applied: lost {goldToLose} gold on exhaust.");
    }
}

internal sealed class JeweledMaskFreePower : CustomEnchantmentModel, ILocalizationProvider
{
    public override bool HasExtraCardText => true;

    public List<(string, string)>? Localization => new CardModifierLoc(
        "Jeweled Mask",
        "This card's cost was permanently set to 0 by Jeweled Mask.",
        "Costs 0 from Jeweled Mask.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Power;
    }

    protected override void OnEnchant()
    {
        if (!Card.EnergyCost.CostsX)
        {
            Card.EnergyCost.UpgradeBy(-Card.EnergyCost.GetWithModifiers(CostModifiers.None));
        }
    }
}

internal static class AncientCardHelpers
{
    public const int FiddleHandLimit = 7;

    public static void EnsureKeywordsInitialized(CardModel card)
    {
        _ = card.Keywords.Count;
    }

    public static void ApplyKeywords(CardModel card, params CardKeyword[] keywords)
    {
        EnsureKeywordsInitialized(card);
        CardCmd.ApplyKeyword(card, keywords);
    }

    public static void RemoveKeywords(CardModel card, params CardKeyword[] keywords)
    {
        EnsureKeywordsInitialized(card);
        CardCmd.RemoveKeyword(card, keywords);
    }

    public static void ApplyTemporaryCostReduction(CardModel card, int amount)
    {
        if (!card.EnergyCost.CostsX)
        {
            card.EnergyCost.AddThisTurnOrUntilPlayed(-amount, reduceOnly: true);
        }

        if (!card.HasStarCostX && card.CurrentStarCost > 0)
        {
            card.SetStarCostThisTurn(card.CurrentStarCost - amount);
        }
    }

    public static int EffectiveCost(CardModel card)
    {
        var energyCost = card.EnergyCost.CostsX
            ? card.Owner.PlayerCombatState?.Energy ?? 0
            : card.EnergyCost.GetWithModifiers(CostModifiers.All);
        var starCost = card.HasStarCostX
            ? card.Owner.PlayerCombatState?.Stars ?? 0
            : Math.Max(0, card.GetStarCostWithModifiers());
        return energyCost + starCost;
    }

    public static Creature? GetPreferredTarget(CardModel card, ICombatState combatState, Player owner)
    {
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.OrderByDescending(creature => creature.CurrentHp).FirstOrDefault(),
            TargetType.AnyAlly => combatState.Allies.FirstOrDefault(creature => creature.IsAlive && creature.IsPlayer && creature != owner.Creature),
            TargetType.AnyPlayer => owner.Creature,
            _ => null
        };
    }

    public static bool IsJeweledMaskPower(CardModel card)
    {
        return card.Enchantment is JeweledMaskFreePower;
    }
}
