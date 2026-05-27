using EZMicroBalance.EZMicroBalanceCode.Ascension;
using System.Threading;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static readonly ConditionalWeakTable<Player, SeedbedCombatState> Seedbeds = new();
    private static readonly ConditionalWeakTable<Player, SeedbedPlantingState> SeedbedPlanting = new();
    private static readonly ConditionalWeakTable<CardModel, SeedbedPlantMarker> PlantedCards = new();
    private static readonly ConditionalWeakTable<Player, SeedbedDrawDepthState> SeedbedDrawDepth = new();

    private sealed class SeedbedCombatState
    {
        public int RemainingSlots { get; set; }
    }

    private sealed class SeedbedPlantingState
    {
        public Queue<SeedbedPlantingRequest> PendingRequests { get; } = new();
        public bool IsProcessing;
    }

    private sealed class SeedbedPlantingRequest
    {
        public CardModel Card { get; init; } = null!;
        public string Source { get; init; } = "";
    }

    private sealed class SeedbedPlantMarker
    {
    }

    private sealed class SeedbedDrawDepthState
    {
        public int Depth;
    }

    internal static void BeginSeedbedDraw(Player player)
    {
        if (player == null)
        {
            return;
        }

        var drawState = SeedbedDrawDepth.GetOrCreateValue(player);
        Interlocked.Increment(ref drawState.Depth);
    }

    internal static void EndSeedbedDraw(Player player)
    {
        if (player == null || !SeedbedDrawDepth.TryGetValue(player, out var drawState))
        {
            return;
        }

        if (Interlocked.Decrement(ref drawState.Depth) <= 0)
        {
            SeedbedDrawDepth.Remove(player);
        }
    }

    private static bool IsSeedbedDrawInProgress(Player player)
    {
        return player != null && SeedbedDrawDepth.TryGetValue(player, out var drawState) && drawState.Depth > 0;
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

    private static void MarkSeedbedPlantedCardInQueue(CardModel card) =>
        PlantedCards.GetValue(card, _ => new SeedbedPlantMarker());

    private static void UnmarkSeedbedPlantedCard(CardModel card) =>
        PlantedCards.Remove(card);

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

    internal static Task<bool> QueueSeedbedPlantFromHand(CardModel card, string source)
    {
        if (card.Owner is not { } player ||
            GetOrRestoreSeedbed(player) is not { RemainingSlots: > 0 } state ||
            card.Pile?.Type != PileType.Hand ||
            !IsSeedbedSeedableCard(card))
        {
            return Task.FromResult(false);
        }

        var queue = SeedbedPlanting.GetOrCreateValue(player);
        MarkSeedbedPlantedCardInQueue(card);
        queue.PendingRequests.Enqueue(new SeedbedPlantingRequest
        {
            Card = card,
            Source = source
        });

        if (!queue.IsProcessing)
        {
            queue.IsProcessing = true;
            _ = TaskHelper.RunSafely(ProcessSeedbedPlantingQueue(player, queue));
        }

        return Task.FromResult(true);
    }

    private static async Task ProcessSeedbedPlantingQueue(Player player, SeedbedPlantingState state)
    {
        await Task.Yield();

        try
        {
            while (HasAny(player, out var request))
            {
                while (IsSeedbedDrawInProgress(player))
                {
                    await Task.Yield();
                }

                if (GetOrRestoreSeedbed(player) is not { } seedbedState || seedbedState.RemainingSlots <= 0)
                {
                    UnmarkSeedbedPlantedCard(request.Card);
                    continue;
                }

                if (request.Card.Pile?.Type != PileType.Hand)
                {
                    UnmarkSeedbedPlantedCard(request.Card);
                    continue;
                }

                try
                {
                    if (!await PlantSeedbedCard(request.Card, seedbedState, request.Source))
                    {
                        UnmarkSeedbedPlantedCard(request.Card);
                    }
                }
                catch
                {
                    UnmarkSeedbedPlantedCard(request.Card);
                    throw;
                }
            }
        }
        finally
        {
            state.IsProcessing = false;

            if (GetOrRestoreSeedbed(player) is { RemainingSlots: > 0 } &&
                SeedbedPlanting.TryGetValue(player, out var pending) &&
                pending.PendingRequests.Count > 0)
            {
                pending.IsProcessing = true;
                _ = TaskHelper.RunSafely(ProcessSeedbedPlantingQueue(player, pending));
            }
        }
    }

    private static bool HasAny(
        Player player,
        out SeedbedPlantingRequest request)
    {
        request = null!;
        if (!SeedbedPlanting.TryGetValue(player, out var queue))
        {
            return false;
        }

        if (queue.PendingRequests.Count == 0)
        {
            return false;
        }

        request = queue.PendingRequests.Dequeue();
        return true;
    }

    private static void PersistSeedbed(Player player, SeedbedCombatState state)
    {
        var progress = GetProgress(player);
        SetProgress(player, progress with { SeedbedCombatSlots = Math.Max(0, state.RemainingSlots) });
    }

    private static void ClearSeedbedPlanting(Player player)
    {
        if (!SeedbedPlanting.TryGetValue(player, out var queue))
        {
            return;
        }

        while (queue.PendingRequests.Count > 0)
        {
            UnmarkSeedbedPlantedCard(queue.PendingRequests.Dequeue().Card);
        }

        queue.PendingRequests.Clear();
        queue.IsProcessing = false;
        SeedbedPlanting.Remove(player);
    }

    private static void ClearSeedbed(Player player)
    {
        Seedbeds.Remove(player);
        var progress = GetProgress(player);
        if (progress.SeedbedCombatSlots != 0)
        {
            SetProgress(player, progress with { SeedbedCombatSlots = 0 });
        }

        ClearSeedbedPlanting(player);
    }
}
