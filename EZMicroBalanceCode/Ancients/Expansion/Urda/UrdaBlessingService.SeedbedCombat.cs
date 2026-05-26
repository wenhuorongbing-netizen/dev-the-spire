using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    public static async Task SetupSeedbed(
        PlayerChoiceContext choiceContext,
        Player player,
        int capacity,
        int immediatePlantCount,
        AbstractModel source)
    {
        if (capacity <= 0 ||
            player.Creature.CombatState == null)
        {
            return;
        }

        var state = GetOrRestoreSeedbed(player) ?? Seedbeds.GetValue(player, _ => new SeedbedCombatState());
        state.RemainingSlots = Math.Max(state.RemainingSlots, capacity);
        PersistSeedbed(player, state);

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed set {state.RemainingSlots} slot(s); eligible negative cards entering hand will be planted.");

        if (immediatePlantCount > 0)
        {
            await PlantSeedbedCandidatesFromDrawOrDiscard(choiceContext, player, source, immediatePlantCount);
        }
    }

    public static async Task<bool> TryPlantSeedbedCardFromHand(CardModel card, string source)
    {
        if (card.Owner is not { } player ||
            card.Pile?.Type != PileType.Hand ||
            GetOrRestoreSeedbed(player) is not { RemainingSlots: > 0 } state ||
            !IsSeedbedSeedableCard(card))
        {
            return false;
        }

        await PlantSeedbedCard(card, state, source);
        return true;
    }

    public static bool IsSeedbedSeedableCard(CardModel card)
    {
        if (card is WitheredHusk)
        {
            return false;
        }

        if (card is RootBud)
        {
            return true;
        }

        if (card is RootFamilyCard rootblight)
        {
            return card.Pile?.Type.IsCombatPile() == true &&
                RootDeckService.CanHoldRootblightBySeedbed(rootblight);
        }

        return card.DeckVersion == null &&
            card.Pile?.Type.IsCombatPile() == true &&
            card.Type is CardType.Status or CardType.Curse;
    }

    private static async Task PlantSeedbedCandidatesFromDrawOrDiscard(
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source,
        int maxPlantCount)
    {
        if (GetOrRestoreSeedbed(player) is not { RemainingSlots: > 0 } state)
        {
            return;
        }

        var candidates = GetSeedbedImmediateCandidates(player).ToList();
        if (candidates.Count == 0)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Seedbed found no eligible Draw or Discard pile card to plant immediately.");
            return;
        }

        var selectionCount = Math.Min(Math.Min(maxPlantCount, state.RemainingSlots), candidates.Count);
        var selectedCards = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            candidates,
            player,
            new CardSelectorPrefs(new LocString("cards", "EZMB_URDA_SEEDBED.selectionScreenPrompt"), 1, selectionCount)
            {
                RequireManualConfirmation = true
            })).ToList();

        foreach (var selected in selectedCards)
        {
            if (state.RemainingSlots <= 0)
            {
                break;
            }

            await PlantSeedbedCard(selected, state, $"Seedbed immediate:{source.Id.Entry}");
        }
    }

    private static IEnumerable<CardModel> GetSeedbedImmediateCandidates(Player player) =>
        PileType.Draw.GetPile(player).Cards
            .Concat(PileType.Discard.GetPile(player).Cards)
            .Where(IsSeedbedSeedableCard);

    private static async Task PlantSeedbedCard(CardModel card, SeedbedCombatState state, string source)
    {
        if (state.RemainingSlots <= 0 ||
            card.Owner is not { } player ||
            card.Pile?.Type is not (PileType.Hand or PileType.Draw or PileType.Discard) ||
            !IsSeedbedSeedableCard(card))
        {
            return;
        }

        var sourcePile = card.Pile.Type;
        if (card is RootFamilyCard rootblight &&
            !RootDeckService.TryHoldRootblightBySeedbed(rootblight))
        {
            return;
        }

        MarkSeedbedPlantedCard(card);
        state.RemainingSlots--;
        PersistSeedbed(player, state);

        await CardPileCmd.RemoveFromCombat(card, skipVisuals: true);

        if (player.Creature.CombatState != null &&
            !CombatManager.Instance.IsOverOrEnding &&
            CombatManager.Instance.IsInProgress)
        {
            var husk = player.Creature.CombatState.CreateCard<WitheredHusk>(player);
            await AncientCardHelpers.TryAddGeneratedCardToCombat(husk, PileType.Hand, player);
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed planted {card.Id.Entry} from {sourcePile} via {source}; " +
            $"remaining slots {state.RemainingSlots}. Planting skipped play, discard, and Exhaust synergies.");
    }

}
