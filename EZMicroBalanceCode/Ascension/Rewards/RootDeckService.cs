using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Context;
using Godot;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class RootDeckService
{
    public const int MaxRootblightLevel = 3;
    public const int MaxRootblightCards = 4;
    private const double RootblightNoticeSeconds = 5.0;

    private static readonly ConditionalWeakTable<CardModel, InternalSyncMarker> InternalSyncRemovals = new();
    private static readonly ConditionalWeakTable<Player, RootblightCombatResolution> PendingCombatResolutions = new();

    public static IReadOnlyList<RootFamilyCard> FindRootFamilyCards(Player player)
    {
        return EnumerateRootFamilyCards(player)
            .Select(entry => entry.Card)
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
            await TrimRootblightDeckToCap(player, "run-state sync");
            if (!HasRootBeginsApplied(player))
            {
                MarkRootBeginsApplied(player);
                var addedStartingRoot = false;
                if (FindRootFamilyCards(player).Count == 0)
                {
                    addedStartingRoot = await AddRootblightCard(player, 1);
                }

                SetDiagnosticLevelFromDeck(player);
                MainFile.Logger.Info(
                    addedStartingRoot
                        ? $"[EZMicroBalance] Ascension A14 applied: Rootblight I added for player {runState.GetPlayerSlotIndex(player)}."
                        : $"[EZMicroBalance] Ascension A14 applied: starting Rootblight already present for player {runState.GetPlayerSlotIndex(player)}; no duplicate added.");
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
            EnumerateRootFamilyCards(player)
                .FirstOrDefault(entry => entry.Card.RootblightLevel == card.RootblightLevel)
                .Card;
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

        MarkRootBeginsApplied(player);
        await TrimRootblightDeckToCap(player, $"{source} add");
        if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))
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

        await TrimRootblightDeckToCap(player, "combat-end sync");
        var existingRootblight = FindRootFamilyCards(player)
            .Where(card => card.WasPresentAtCombatStart)
            .ToList();

        foreach (var card in existingRootblight)
        {
            card.WasPresentAtCombatStart = false;
            if (card.RootblightLevel >= MaxRootblightLevel)
            {
                if (!card.HasSplit)
                {
                    card.HasSplit = true;
                    if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))
                    {
                        ShowRootSystemFull(player);
                        MainFile.Logger.Info(
                            $"[EZMicroBalance] Ascension Rootblight capped: skipped Rootblight I from ignored Rootblight III because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                    }
                    else
                    {
                        MainFile.Logger.Info(
                            $"[EZMicroBalance] Ascension Rootblight applied: ignored Rootblight III split once and added Rootblight I for player {player.RunState.GetPlayerSlotIndex(player)}.");
                    }
                }
                else
                {
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension Rootblight applied: ignored Rootblight III already split once; no Rootblight IV for player {player.RunState.GetPlayerSlotIndex(player)}.");
                }

                continue;
            }

            await ReplaceRootblightCard(player, card, card.RootblightLevel + 1, card.HasSplit);
        }

        if (PendingCombatResolutions.TryGetValue(player, out var pending))
        {
            foreach (var cardToAdd in pending.CardsToAddAfterGrowth)
            {
                if (!await AddRootblightCard(player, cardToAdd.Level, cardToAdd.HasSplit, preferOverlayNotice: true))
                {
                    ShowRootSystemFull(player);
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension Rootblight capped: skipped queued level {cardToAdd.Level} downgrade because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                }
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

        var rootblightCard = EnumerateRootFamilyCards(player)
            .OrderByDescending(entry => entry.Card.RootblightLevel)
            .ThenBy(entry => entry.Index)
            .Select(entry => entry.Card)
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
        MarkRootBeginsApplied(player);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight removed through a deck-removal API for player {player.RunState.GetPlayerSlotIndex(player)}; remaining Rootblight cards are preserved.");
        return Task.CompletedTask;
    }

    public static int GetRootblightLevel(Player player)
    {
        return GetLevel(player);
    }

    private static int GetLevel(Player player)
    {
        var deckLevel = FindRootFamilyCards(player)
            .Select(card => card.RootblightLevel)
            .DefaultIfEmpty(0)
            .Max();
        var cachedLevel = Math.Clamp(AscensionSavedStateFields.RootblightLevel[player], 0, MaxRootblightLevel);
        return Math.Clamp(Math.Max(cachedLevel, deckLevel), 0, MaxRootblightLevel);
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

    private static bool HasRootBeginsApplied(Player player)
    {
        return AscensionSavedStateFields.RootBeginsApplied[player] ||
            FindRootFamilyCards(player).Count > 0 ||
            player.RunState.CurrentActIndex > 0 ||
            player.RunState.ActFloor > 0 ||
            player.RunState.CurrentMapCoord.HasValue ||
            player.RunState.MapPointHistory.Any(actHistory => actHistory.Count > 0);
    }

    private static void MarkRootBeginsApplied(Player player)
    {
        AscensionSavedStateFields.RootBeginsApplied[player] = true;
    }

    private static async Task ReplaceRootblightCard(Player player, RootFamilyCard card, int nextLevel, bool hasSplit)
    {
        InternalSyncRemovals.GetValue(card, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(card, showPreview: false);
        await AddRootblightCard(player, nextLevel, hasSplit, preferOverlayNotice: true);
    }

    private static async Task<bool> AddRootblightCard(Player player, int level, bool hasSplit = false, bool preferOverlayNotice = false)
    {
        await TrimRootblightDeckToCap(player, "pre-add cap check");
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

        var addResult = await CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, source: null, skipVisuals: true);
        if (!addResult.success)
        {
            return false;
        }

        ShowRootblightAdded(player, preferOverlayNotice);
        return true;
    }

    private static IEnumerable<(RootFamilyCard Card, int Index)> EnumerateRootFamilyCards(Player player)
    {
        return player.Deck.Cards
            .Select((card, index) => (Card: card, Index: index))
            .Where(entry => entry.Card is RootFamilyCard)
            .Select(entry => (Card: (RootFamilyCard)entry.Card, Index: entry.Index));
    }

    private static async Task TrimRootblightDeckToCap(Player player, string reason)
    {
        var cards = EnumerateRootFamilyCards(player).ToList();
        if (cards.Count <= MaxRootblightCards)
        {
            SetDiagnosticLevelFromDeck(player);
            return;
        }

        var cardsToKeep = cards
            .OrderByDescending(entry => entry.Card.RootblightLevel)
            .ThenBy(entry => entry.Index)
            .Take(MaxRootblightCards)
            .Select(entry => entry.Card)
            .ToHashSet();
        var cardsToRemove = cards
            .Where(entry => !cardsToKeep.Contains(entry.Card))
            .OrderByDescending(entry => entry.Index)
            .Select(entry => entry.Card)
            .ToList();
        foreach (var duplicate in cardsToRemove)
        {
            InternalSyncRemovals.GetValue(duplicate, _ => new InternalSyncMarker());
            await CardPileCmd.RemoveFromDeck(duplicate, showPreview: false);
        }

        SetDiagnosticLevelFromDeck(player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Rootblight trimmed by {reason}: kept {MaxRootblightCards} highest/oldest Rootblight card(s) and removed {cardsToRemove.Count} excess Rootblight card(s) for player {player.RunState.GetPlayerSlotIndex(player)}.");
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
        ShowLocalRootblightNotice(
            player,
            new LocString("ascension", "ROOT_SYSTEM_FULL"),
            "cap");
    }

    private static void ShowRootblightAdded(Player player, bool preferOverlayNotice)
    {
        ShowLocalRootblightNotice(
            player,
            new LocString("ascension", "ROOTBLIGHT_ADDED"),
            "add",
            preferOverlayNotice);
    }

    private static void ShowLocalRootblightNotice(
        Player player,
        LocString line,
        string noticeKind,
        bool preferOverlayNotice = false)
    {
        if (!LocalContext.IsMe(player))
        {
            return;
        }

        try
        {
            if (preferOverlayNotice && TryShowRunOverlayNotice(line))
            {
                return;
            }

            var creatureVfxContainer = player.Creature.GetVfxContainer();
            if (creatureVfxContainer != null)
            {
                ThinkCmd.Play(line, player.Creature, RootblightNoticeSeconds);
                return;
            }

            if (TryShowEventRoomNotice(line))
            {
                return;
            }

            TryShowRunOverlayNotice(line);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Ascension Rootblight {noticeKind} notice could not be displayed: {ex.Message}");
        }
    }

    private static bool TryShowEventRoomNotice(LocString line)
    {
        var container = NEventRoom.Instance?.VfxContainer;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = container.GlobalPosition + new Vector2(220f, MathF.Max(180f, container.Size.Y * 0.55f));
        return true;
    }

    private static bool TryShowRunOverlayNotice(LocString line)
    {
        return TryShowTopLevelRunNotice(line) || TryShowGlobalRunNotice(line);
    }

    private static bool TryShowTopLevelRunNotice(LocString line)
    {
        var container = NGame.Instance;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = new Vector2(110f, 90f);
        return true;
    }

    private static bool TryShowGlobalRunNotice(LocString line)
    {
        var container = NRun.Instance?.GlobalUi.AboveTopBarVfxContainer;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = container.GlobalPosition + new Vector2(220f, 180f);
        return true;
    }

    private static void PrepareOverlayNotice(NThoughtBubbleVfx bubble)
    {
        bubble.MouseFilter = Control.MouseFilterEnum.Ignore;
        bubble.ZAsRelative = false;
        bubble.ZIndex = 4096;
    }
}
