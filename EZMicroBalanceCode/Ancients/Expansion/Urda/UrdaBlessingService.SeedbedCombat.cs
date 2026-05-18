using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static readonly ConditionalWeakTable<Player, SeedbedCombatState> Seedbeds = new();

    public static async Task SetupSeedbed(
        PlayerChoiceContext choiceContext,
        Player player,
        int capacity,
        bool chooseImmediate,
        AbstractModel source)
    {
        var state = GetOrRestoreSeedbed(player) ?? Seedbeds.GetValue(player, _ => new SeedbedCombatState());
        state.RemainingSlots = Math.Max(state.RemainingSlots, capacity);
        PersistSeedbed(player, state);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seedbed armed: {state.RemainingSlots} slot(s) available for player {player.RunState.GetPlayerSlotIndex(player)}.");

        if (!chooseImmediate)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(new LocString("cards", "EZMB_URDA_SEEDBED.selectionScreenPrompt"), 1),
            IsSeedbedEligibleCard,
            source)).FirstOrDefault();
        if (selected != null)
        {
            await TryCatchSeedbedCardFromHand(selected, "Seedbed+");
        }
    }

    public static async Task<bool> TryCatchSeedbedCardFromHand(CardModel card, string source)
    {
        if (card.Owner is not { } player ||
            card.Pile?.Type != PileType.Hand ||
            GetOrRestoreSeedbed(player) is not { } seedbed ||
            seedbed.RemainingSlots <= 0 ||
            !IsSeedbedEligibleCard(card))
        {
            return false;
        }

        if (player.Creature.CombatState is not { } combatState)
        {
            return false;
        }

        MarkSeedbedPlantedCard(card);
        seedbed.RemainingSlots--;
        PersistSeedbed(player, seedbed);
        await CardPileCmd.RemoveFromCombat(card, skipVisuals: true);

        var husk = combatState.CreateCard<WitheredHusk>(player);
        await CardPileCmd.AddGeneratedCardToCombat(husk, PileType.Hand, player);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seedbed caught {card.Id.Entry} from {source}; {seedbed.RemainingSlots} slot(s) remain for player {player.RunState.GetPlayerSlotIndex(player)}.");
        return true;
    }

    public static bool IsSeedbedEligibleCard(CardModel card)
    {
        if (card is WitheredHusk)
        {
            return false;
        }

        return card is RootBud or RootFamilyCard ||
            card.Type is CardType.Status or CardType.Curse;
    }

    private static void MarkSeedbedPlantedCard(CardModel card)
    {
        if (card is RootBud bud)
        {
            bud.PlantedInSeedbed = true;
            bud.HasEnteredHand = false;
            return;
        }

        if (card is RootFamilyCard rootblight)
        {
            rootblight.PlantedInSeedbed = true;
            if (rootblight.DeckVersion is RootFamilyCard deckRootblight)
            {
                deckRootblight.PlantedInSeedbed = true;
                return;
            }

            MainFile.Logger.Warn(
                "[EZMicroBalance] Urda Seedbed caught a Rootblight without a DeckVersion; skipped deck marker instead of guessing by Rootblight level.");
        }
    }

    private static void ClearSeedbed(Player player)
    {
        Seedbeds.Remove(player);
        var progress = GetProgress(player);
        if (progress.SeedbedCombatSlots > 0)
        {
            SetProgress(player, progress with { SeedbedCombatSlots = 0 });
        }
    }

    private static SeedbedCombatState? GetOrRestoreSeedbed(Player player)
    {
        if (Seedbeds.TryGetValue(player, out var state) &&
            state.RemainingSlots > 0)
        {
            return state;
        }

        var persistedSlots = GetProgress(player).SeedbedCombatSlots;
        if (persistedSlots <= 0 ||
            player.Creature.CombatState == null)
        {
            return state;
        }

        state = Seedbeds.GetValue(player, _ => new SeedbedCombatState());
        state.RemainingSlots = persistedSlots;
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seedbed restored from saved combat state: {state.RemainingSlots} slot(s) for player {player.RunState.GetPlayerSlotIndex(player)}.");
        return state;
    }

    private static void PersistSeedbed(Player player, SeedbedCombatState state)
    {
        SetProgress(player, GetProgress(player) with { SeedbedCombatSlots = Math.Max(0, state.RemainingSlots) });
    }

    private sealed class SeedbedCombatState
    {
        public int RemainingSlots { get; set; }
    }
}
