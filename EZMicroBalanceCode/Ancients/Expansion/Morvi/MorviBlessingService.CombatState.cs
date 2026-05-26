using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
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

    private static readonly ConditionalWeakTable<Player, MorviCombatState> CombatStates = new();
}
