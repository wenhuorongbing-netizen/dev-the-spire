using EZMicroBalance.EZMicroBalanceCode.Ascension;

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
