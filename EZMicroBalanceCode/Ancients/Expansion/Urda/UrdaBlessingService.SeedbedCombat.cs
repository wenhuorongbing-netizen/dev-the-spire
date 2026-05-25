using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Hooks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static readonly ConditionalWeakTable<Player, SeedbedCombatState> Seedbeds = new();
    private static readonly ConditionalWeakTable<CardModel, SeedbedPlantMarker> PlantedCards = new();

    private sealed class SeedbedCombatState
    {
        public int RemainingSlots { get; set; }
    }

    private sealed class SeedbedPlantMarker
    {
    }

    public static async Task SetupSeedbed(
        PlayerChoiceContext choiceContext,
        Player player,
        int capacity,
        bool plantImmediate,
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
            $"[Spire Plus] Urda Seedbed set {state.RemainingSlots} slot(s); future seedable temporary cards entering hand will be planted.");

        if (plantImmediate)
        {
            await PlantOneSeedbedCandidateFromDrawOrDiscard(choiceContext, player, source);
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

    private static async Task PlantOneSeedbedCandidateFromDrawOrDiscard(
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source)
    {
        if (GetOrRestoreSeedbed(player) is not { RemainingSlots: > 0 } state)
        {
            return;
        }

        var candidates = GetSeedbedImmediateCandidates(player).ToList();
        if (candidates.Count == 0)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Seedbed+ found no eligible Draw or Discard pile card to plant.");
            return;
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            candidates,
            player,
            new CardSelectorPrefs(new LocString("cards", "EZMB_URDA_SEEDBED.selectionScreenPrompt"), 1, 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();

        if (selected == null)
        {
            return;
        }

        await PlantSeedbedCard(selected, state, $"Seedbed+ immediate:{source.Id.Entry}");
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

    private static void MarkSeedbedPlantedCard(CardModel card)
    {
        PlantedCards.GetValue(card, _ => new SeedbedPlantMarker());

        if (card is RootBud bud)
        {
            bud.PlantedInSeedbed = true;
            bud.HasEnteredHand = false;
        }
    }

    internal static bool WasPlantedBySeedbed(CardModel card) => PlantedCards.TryGetValue(card, out _);

    private static SeedbedCombatState? GetOrRestoreSeedbed(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return null;
        }

        if (Seedbeds.TryGetValue(player, out var state))
        {
            return state;
        }

        var persistedSlots = GetProgress(player).SeedbedCombatSlots;
        if (persistedSlots <= 0)
        {
            return null;
        }

        state = new SeedbedCombatState { RemainingSlots = persistedSlots };
        Seedbeds.Add(player, state);
        return state;
    }

    private static void PersistSeedbed(Player player, SeedbedCombatState state)
    {
        var progress = GetProgress(player);
        SetProgress(player, progress with { SeedbedCombatSlots = Math.Max(0, state.RemainingSlots) });
    }

    private static void ClearSeedbed(Player player)
    {
        Seedbeds.Remove(player);
        var progress = GetProgress(player);
        if (progress.SeedbedCombatSlots != 0)
        {
            SetProgress(player, progress with { SeedbedCombatSlots = 0 });
        }
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]
internal static class UrdaSeedbedAfterCardDrawnPatch
{
    private static bool Prefix(CardModel card)
    {
        if (!UrdaBlessingService.WasPlantedBySeedbed(card))
        {
            return true;
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed skipped AfterCardDrawn hooks for planted card {card.Id.Entry}.");
        return false;
    }
}
