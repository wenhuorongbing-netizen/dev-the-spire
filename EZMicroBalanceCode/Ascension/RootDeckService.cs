using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class RootDeckService
{
    public const int MaxRootblightLevel = 3;
    public const int MaxRootblightCards = 4;

    private static readonly ConditionalWeakTable<CardModel, InternalSyncMarker> InternalSyncRemovals = new();
    private static readonly ConditionalWeakTable<Player, RootblightCombatResolution> PendingCombatResolutions = new();

    public static IReadOnlyList<RootFamilyCard> FindRootFamilyCards(Player player)
    {
        return player.Deck.Cards
            .OfType<RootFamilyCard>()
            .ToList();
    }

    public static async Task EnsureStartingRoot(RunState runState)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(runState))
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            if (!AscensionSavedStateFields.RootBeginsApplied[player])
            {
                AscensionSavedStateFields.RootBeginsApplied[player] = true;
                await AddRootblightCard(player, 1);
                SetDiagnosticLevelFromDeck(player);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A14 applied: Rootblight I added for player {runState.GetPlayerSlotIndex(player)}.");
                continue;
            }

            SetDiagnosticLevelFromDeck(player);
        }
    }

    public static async Task ApplyPlayedRootblightCard(RootFamilyCard card)
    {
        var player = card.Owner;
        if (player == null || !AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var deckCard = card.DeckVersion as RootFamilyCard ??
            FindRootFamilyCards(player).FirstOrDefault(deckCard => deckCard.RootblightLevel == card.RootblightLevel);
        var splitState = deckCard?.HasSplit ?? card.HasSplit;
        if (deckCard != null)
        {
            InternalSyncRemovals.GetValue(deckCard, _ => new InternalSyncMarker());
            await CardPileCmd.RemoveFromDeck(deckCard, showPreview: false);
        }

        var downgradedLevel = card.RootblightLevel - 1;
        if (downgradedLevel > 0)
        {
            PendingCombatResolutions.GetValue(player, _ => new RootblightCombatResolution())
                .CardsToAddAfterGrowth
                .Add(new RootblightCardToAdd(downgradedLevel, splitState));
        }

        SetDiagnosticLevelFromDeck(player);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight applied: played level {card.RootblightLevel}; removed its master-deck card and queued {downgradedLevel} downgrade level for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static async Task AddRootblightI(Player player, string source)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        AscensionSavedStateFields.RootBeginsApplied[player] = true;
        if (!await AddRootblightCard(player, 1))
        {
            ShowRootSystemFull(player);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension Rootblight capped: skipped Rootblight I from {source} because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
            return;
        }

        SetDiagnosticLevelFromDeck(player);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight applied: added Rootblight I from {source} for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static void MarkCombatStartRootblight(Player player)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        foreach (var card in FindRootFamilyCards(player))
        {
            card.WasPresentAtCombatStart = true;
        }
    }

    public static async Task ResolveCombatEndRootblight(Player player)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var existingRootblight = FindRootFamilyCards(player)
            .Where(card => card.WasPresentAtCombatStart)
            .ToList();

        foreach (var card in existingRootblight)
        {
            card.WasPresentAtCombatStart = false;
            if (card.RootblightLevel >= MaxRootblightLevel)
            {
                if (card.HasSplit)
                {
                    continue;
                }

                card.HasSplit = true;
                if (!await AddRootblightCard(player, 1))
                {
                    ShowRootSystemFull(player);
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension Rootblight capped: Rootblight III split was blocked for player {player.RunState.GetPlayerSlotIndex(player)} because the deck already has {MaxRootblightCards} Rootblight cards.");
                }
                else
                {
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension Rootblight applied: ignored unsplit Rootblight III added one Rootblight I for player {player.RunState.GetPlayerSlotIndex(player)}.");
                }

                continue;
            }

            await ReplaceRootblightCard(player, card, card.RootblightLevel + 1, card.HasSplit);
        }

        if (PendingCombatResolutions.TryGetValue(player, out var pending))
        {
            foreach (var cardToAdd in pending.CardsToAddAfterGrowth)
            {
                await AddRootblightCard(player, cardToAdd.Level, cardToAdd.HasSplit);
            }

            pending.CardsToAddAfterGrowth.Clear();
        }

        SetDiagnosticLevelFromDeck(player);
    }

    public static async Task RemoveHighestRootblight(Player player, string reason)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var rootblightCard = player.Deck.Cards
            .OfType<RootFamilyCard>()
            .OrderByDescending(card => card.RootblightLevel)
            .FirstOrDefault();
        if (rootblightCard == null)
        {
            return;
        }

        InternalSyncRemovals.GetValue(rootblightCard, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(rootblightCard, showPreview: false);
        SetDiagnosticLevelFromDeck(player);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight removed by {reason}: removed level {rootblightCard.RootblightLevel} for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static Task BeforeCardRemoved(CardModel card)
    {
        if (card.Owner == null ||
            card is not RootFamilyCard ||
            InternalSyncRemovals.TryGetValue(card, out _))
        {
            return Task.CompletedTask;
        }

        var player = card.Owner;
        SetLevelFromCards(
            player,
            FindRootFamilyCards(player)
                .Where(deckCard => !ReferenceEquals(deckCard, card)));
        AscensionSavedStateFields.RootBeginsApplied[player] = true;
        if (PendingCombatResolutions.TryGetValue(player, out var pending))
        {
            pending.CardsToAddAfterGrowth.Clear();
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight cleared because a Rootblight card was removed from player {player.RunState.GetPlayerSlotIndex(player)}'s master deck.");
        return Task.CompletedTask;
    }

    public static int GetRootblightLevel(Player player)
    {
        return GetLevel(player);
    }

    private static int GetLevel(Player player)
    {
        return Math.Clamp(AscensionSavedStateFields.RootblightLevel[player], 0, MaxRootblightLevel);
    }

    private static void SetLevel(Player player, int level)
    {
        AscensionSavedStateFields.RootblightLevel[player] = Math.Clamp(level, 0, MaxRootblightLevel);
    }

    private static void SetDiagnosticLevelFromDeck(Player player)
    {
        SetLevelFromCards(player, FindRootFamilyCards(player));
    }

    private static void SetLevelFromCards(Player player, IEnumerable<RootFamilyCard> cards)
    {
        SetLevel(
            player,
            cards
                .Select(card => card.RootblightLevel)
                .DefaultIfEmpty(0)
                .Max());
    }

    private static async Task ReplaceRootblightCard(Player player, RootFamilyCard card, int nextLevel, bool hasSplit)
    {
        InternalSyncRemovals.GetValue(card, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(card, showPreview: false);
        await AddRootblightCard(player, nextLevel, hasSplit);
    }

    private static async Task<bool> AddRootblightCard(Player player, int level, bool hasSplit = false)
    {
        if (FindRootFamilyCards(player).Count >= MaxRootblightCards)
        {
            return false;
        }

        var rootblightCard = CreateRootblightCard(player, level);
        if (rootblightCard is RootFamilyCard rootFamilyCard)
        {
            rootFamilyCard.HasSplit = hasSplit;
            rootFamilyCard.WasPresentAtCombatStart = false;
        }

        await CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, source: null, skipVisuals: true);
        return true;
    }

    private static CardModel CreateRootblightCard(Player player, int level)
    {
        return level switch
        {
            1 => player.RunState.CreateCard<Root>(player),
            2 => player.RunState.CreateCard<DeepRoot>(player),
            _ => player.RunState.CreateCard<RootblightIII>(player),
        };
    }

    private sealed class InternalSyncMarker
    {
    }

    private sealed class RootblightCombatResolution
    {
        public List<RootblightCardToAdd> CardsToAddAfterGrowth { get; } = new();
    }

    private readonly record struct RootblightCardToAdd(int Level, bool HasSplit);

    private static void ShowRootSystemFull(Player player)
    {
        try
        {
            ThinkCmd.Play(new LocString("ascension", "ROOT_SYSTEM_FULL"), player.Creature, 2.0);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Ascension Rootblight cap notice could not be displayed: {ex.Message}");
        }
    }
}
