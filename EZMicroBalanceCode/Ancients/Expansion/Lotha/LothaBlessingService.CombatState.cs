using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private sealed class LothaCombatState
    {
        public bool MirrorRebuttalCardPulled { get; set; }

        public bool MirrorRebuttalResolved { get; set; }

        public CardType? MirrorHallEchoRecordedType { get; set; }

        public CardType? MirrorHallEchoArmedType { get; set; }

        public bool MirrorHallEchoConsumedThisTurn { get; set; }

        public bool ClosedCourtUsed { get; set; }

        public bool ClosedCourtDiscountActiveThisTurn { get; set; }

        public int ClosedCourtDiscountsRemainingThisTurn { get; set; }

        public HashSet<CardModel> ClosedCourtDiscountedCardsThisTurn { get; } = [];

        public bool PresumptionLost { get; set; }

        public bool DeferredVerdictGranted { get; set; }

        public bool DeferredVerdictActiveThisTurn { get; set; }

        public bool DeathReprieveActive { get; set; }

        public bool DeathReprievePendingStart { get; set; }

        public bool DeathReprieveStarted { get; set; }

        public bool SingleSentenceUsedThisTurn { get; set; }

        public bool SingleSentencePowerFallbackUsedThisTurn { get; set; }

        public int SingleSentenceRemainingCardsPlayedThisTurn { get; set; }

        public CardModel? SingleSentenceRulingCard { get; set; }

        public CardModel? AutoPlayCardPendingModifier { get; set; }

        public CardModel? PowerReplacementCardPendingBenefit { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, LothaCombatState> CombatStates = new();
}
