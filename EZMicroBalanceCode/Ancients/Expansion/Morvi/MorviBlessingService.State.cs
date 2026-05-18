using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const char ProgressSeparator = ';';

    private sealed class MorviCombatState
    {
        public bool MisprintUsedThisTurn { get; set; }

        public HashSet<CardModel> MisprintDrawAfterCards { get; } = [];

        public CardModel? AutoPlayCardPendingModifier { get; set; }

        public bool RedInkUsedThisTurn { get; set; }

        public int RedInkDebtsThisCombat { get; set; }

        public bool OverdueLibraryDiscountArmed { get; set; }

        public CardModel? OverdueLibraryDiscountSourceCard { get; set; }

        public bool OpenBookResolved { get; set; }

        public HashSet<CardModel> OpenBookDrawnCards { get; } = [];

        public List<CardModel> OpenBookSealedCards { get; } = [];

        public int PaperstormTriggersRemainingThisTurn { get; set; }

        public int ProofreadRemaining { get; set; }

        public bool BlueprintProofInitializedThisCombat { get; set; }

        public HashSet<CardModel> BlueprintTemporaryUpgradeCards { get; } = [];

        public HashSet<CardModel> BlueprintDrawAfterCards { get; } = [];

        public HashSet<CardModel> BlueprintBlockAfterCards { get; } = [];
    }

    private sealed record Progress(
        int DebtRemaining,
        string BorrowedCardId,
        bool BorrowedSettled)
    {
        public static Progress Default => new(0, string.Empty, false);
    }

    private static readonly ConditionalWeakTable<Player, MorviCombatState> CombatStates = new();

    public static async Task<bool> TrySetSelectedBlessing(Player player, string blessingId)
    {
        ClearBorrowedAncientCards(player);

        switch (blessingId)
        {
            case MorviBlessingIds.ForbiddenLoan:
                var forbiddenLoanProgress = await TrySelectForbiddenLoanCard(player);
                if (forbiddenLoanProgress == null)
                {
                    ClearState(player);
                    SyncPersistentState(player);
                    return false;
                }

                SetState(player, blessingId, forbiddenLoanProgress);
                break;
            case MorviBlessingIds.DebtSettlement:
                SetState(player, blessingId, Progress.Default);
                await ResolveDebtSettlementPickup(player);
                break;
            default:
                SetState(player, blessingId, Progress.Default);
                break;
        }

        SyncPersistentState(player);
        return true;
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static void SyncPersistentState(Player? player)
    {
        if (player == null)
        {
            return;
        }

        AncientPlayerState.SyncDeck(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
    }

    private static Progress GetProgress(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 4)
        {
            return Progress.Default;
        }

        return new Progress(
            ParseInt(parts[1]),
            parts[2],
            ParseBool(parts[3]));
    }

    private static void SetProgress(Player player, Progress progress)
    {
        var selectedBlessing = GetSelectedBlessing(player);
        if (!string.IsNullOrWhiteSpace(selectedBlessing))
        {
            SetState(player, selectedBlessing, progress);
        }
    }

    private static void SetState(Player player, string blessingId, Progress progress)
    {
        AncientPlayerState.Set(
            player,
            string.Join(
                ProgressSeparator,
                blessingId,
                progress.DebtRemaining,
                progress.BorrowedCardId,
                progress.BorrowedSettled ? 1 : 0),
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
    }

    private static void ClearState(Player player) =>
        AncientPlayerState.Set(
            player,
            string.Empty,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
