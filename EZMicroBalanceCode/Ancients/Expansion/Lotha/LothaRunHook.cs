using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        return LothaBlessingService.BeforeCombatStart();
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        LothaBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        LothaBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) =>
        LothaBlessingService.AfterTurnEnd(choiceContext, side);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        LothaBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        LothaBlessingService.ShouldPlay(card, autoPlayType);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        LothaBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override Task AfterCombatEnd(CombatRoom room) =>
        LothaBlessingService.AfterCombatEnd(room);

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        LothaBlessingService.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room) =>
        LothaBlessingService.TryModifyRewardsLate(player, rewards, room);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        LothaBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        LothaBlessingService.TryModifyStarCost(card, originalCost, out modifiedCost);

    public override decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource) =>
        LothaBlessingService.ModifyPowerAmountGiven(power, giver, amount, target);

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount) =>
        LothaBlessingService.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource) =>
        LothaBlessingService.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

    public override bool ShouldDieLate(Creature creature) =>
        LothaBlessingService.ShouldDieLate(creature);

    public override bool ShouldDie(Creature creature) =>
        LothaBlessingService.ShouldDie(creature);

    public override Task AfterPreventingDeath(Creature creature) =>
        LothaBlessingService.AfterPreventingDeath(creature);
}

internal static class LothaBlessingService
{
    private const char ProgressSeparator = ';';
    private const int PresumptionCards = 2;
    private const int PresumptionEnergy = 1;
    private const int PresumptionBlock = 8;
    private const int PresumptionHpLoss = 8;
    private const int LothaExtraPlayCount = 2;
    private const int PowerFallbackCards = 1;
    private const int MirrorRebuttalPowerFallbackEnergy = 2;
    private const int MirrorRebuttalPowerFallbackCards = 2;
    private const int MirrorHallEchoExtraPlayCount = 1;
    private const int ClosedCourtEnergy = 4;
    private const int ClosedCourtDiscountCount = 3;
    private const int DeferredVerdictTurn = 4;
    private const int DeferredVerdictStacks = 3;
    private const int DeferredVerdictEnergy = 4;
    private const int DeferredVerdictCards = 4;
    private const int DeferredVerdictExtraPlayCount = 1;
    private const int DeferredVerdictEarlyEndHeal = 4;
    private const int PublicEvidenceEnlightenmentGain = 1;
    private const int PublicEvidenceConsumeLimit = 3;
    private const int PublicEvidenceBlockPerEnlightenment = 4;
    private const int PublicEvidenceCardsPerEnlightenment = 1;
    private const int SingleSentenceRemainingPlayLimit = 4;
    private const int DeathReprieveCards = 10;
    private const int DeathReprieveEnergy = 10;

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

    private enum DeathReprievePhase
    {
        None = 0,
        PendingStart = 1,
        Active = 2,
        Resolved = 3
    }

    private sealed record Progress(bool DeathReprieveUsed, DeathReprievePhase DeathReprievePhase)
    {
        public static Progress Default => new(false, DeathReprievePhase.None);
    }

    private static readonly ConditionalWeakTable<Player, LothaCombatState> CombatStates = new();

    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        if (blessingId != LothaBlessingIds.MirrorRebuttal)
        {
            ClearMirrorRebuttalMarkedCards(player);
        }

        SetState(player, blessingId, Progress.Default);
    }

    public static bool IsMirrorRebuttalDeckCardCandidate(CardModel card) =>
        card.Type is CardType.Attack or CardType.Skill or CardType.Power &&
        !card.HasBeenRemovedFromState;

    public static void MarkMirrorRebuttalCard(Player player, CardModel card)
    {
        ClearMirrorRebuttalMarkedCards(player);
        AncientSavedStateFields.LothaMirrorRebuttalCard[card] = true;
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal marked deck card {card.Id.Entry}.");
    }

    private static void ClearMirrorRebuttalMarkedCards(Player player)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player))
        {
            AncientSavedStateFields.LothaMirrorRebuttalCard[card] = false;
        }
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
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
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
    }

    public static async Task BeforeCombatStart()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            ResetCombatState(combatState);
            HydrateDeathReprieveState(player, combatState);

            if (GetSelectedBlessing(player) == LothaBlessingIds.Presumption)
            {
                await PowerCmd.Apply<LothaPresumptionPower>(
                    new ThrowingPlayerChoiceContext(),
                    player.Creature,
                    1,
                    player.Creature,
                    null);
                MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence applied Innocent at combat start.");
            }
        }
    }

    public static async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (!player.IsActiveForHooks)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        var selectedBlessing = GetSelectedBlessing(player);
        var activeCombat = player.Creature.CombatState;

        ResetTurnState(combatState);
        HydrateDeathReprieveState(player, combatState);

        if (selectedBlessing == LothaBlessingIds.Presumption && !combatState.PresumptionLost)
        {
            if (player.Creature.GetPower<LothaPresumptionPower>() == null)
            {
                await PowerCmd.Apply<LothaPresumptionPower>(
                    choiceContext,
                    player.Creature,
                    1,
                    player.Creature,
                    null);
            }

            await CardPileCmd.Draw(choiceContext, PresumptionCards, player);
            await PlayerCmd.GainEnergy(PresumptionEnergy, player);
            await CreatureCmd.GainBlock(player.Creature, PresumptionBlock, ValueProp.Move, null, fast: true);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence granted draw 2, Energy 1, and Block 8 while Innocent.");
        }

        if (selectedBlessing == LothaBlessingIds.ClosedCourt && !combatState.ClosedCourtUsed)
        {
            combatState.ClosedCourtUsed = true;
            combatState.ClosedCourtDiscountActiveThisTurn = true;
            combatState.ClosedCourtDiscountsRemainingThisTurn = ClosedCourtDiscountCount;

            var cardsToDraw = Math.Max(0, CardPile.MaxCardsInHand - PileType.Hand.GetPile(player).Cards.Count);
            if (cardsToDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, cardsToDraw, player);
            }

            await PlayerCmd.GainEnergy(ClosedCourtEnergy, player);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Closed Court filled the hand, granted Energy 4, and armed three Energy-cost discounts.");
        }

        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal &&
            activeCombat?.RoundNumber == 1 &&
            !combatState.MirrorRebuttalCardPulled)
        {
            combatState.MirrorRebuttalCardPulled = true;
            await TryMoveMirrorRebuttalCardToHand(player);
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            !combatState.DeferredVerdictGranted &&
            activeCombat != null &&
            activeCombat.RoundNumber == DeferredVerdictTurn)
        {
            combatState.DeferredVerdictGranted = true;
            combatState.DeferredVerdictActiveThisTurn = true;

            await PlayerCmd.GainEnergy(DeferredVerdictEnergy, player);
            await CardPileCmd.Draw(choiceContext, DeferredVerdictCards, player);
            await PowerCmd.Apply<LothaVerdictPower>(
                choiceContext,
                player.Creature,
                DeferredVerdictStacks,
                player.Creature,
                null);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict granted draw 4, Energy 4, and player-owned Verdict 3.");
        }
        else if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            combatState.DeferredVerdictGranted &&
            activeCombat?.RoundNumber > DeferredVerdictTurn)
        {
            await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
        }

        if (selectedBlessing == LothaBlessingIds.PublicEvidence)
        {
            await ConsumePublicEvidenceEnlightenmentAtTurnStart(choiceContext, player);
        }

        if (selectedBlessing == LothaBlessingIds.DeathReprieve && combatState.DeathReprievePendingStart)
        {
            await StartDeathReprieveTurn(choiceContext, player, combatState, "next player turn after lethal damage");
        }
    }

    public static async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            var selectedBlessing = GetSelectedBlessing(player);

            if (selectedBlessing == LothaBlessingIds.MirrorHallEcho)
            {
                RecordMirrorHallEchoType(player, combatState);
            }

            if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
                player.Creature.CombatState?.RoundNumber == DeferredVerdictTurn)
            {
                combatState.DeferredVerdictActiveThisTurn = false;
                await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
                MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict removed Verdict at turn end.");
            }

            if (selectedBlessing == LothaBlessingIds.DeathReprieve && combatState.DeathReprieveActive)
            {
                await ResolveDeathReprieveTurnEnd(player, combatState);
            }
        }
    }

    public static int ModifyCardPlayCount(CardModel card, int playCount)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return playCount;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (TryConsumeAutoPlayModifierBlock(card, combatState))
        {
            return playCount;
        }

        var selectedBlessing = GetSelectedBlessing(player);
        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal &&
            !combatState.MirrorRebuttalResolved &&
            IsMirrorRebuttalCombatCard(card) &&
            IsEligibleCard(card))
        {
            combatState.MirrorRebuttalResolved = true;
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal extra-played {card.Id.Entry} two additional times.");
            return playCount + LothaExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.MirrorHallEcho &&
            !combatState.MirrorHallEchoConsumedThisTurn &&
            combatState.MirrorHallEchoArmedType == card.Type &&
            IsEligibleCard(card))
        {
            combatState.MirrorHallEchoConsumedThisTurn = true;
            combatState.MirrorHallEchoArmedType = null;
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Hall Echo extra-played {card.Id.Entry} one additional time.");
            return playCount + MirrorHallEchoExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            combatState.DeferredVerdictActiveThisTurn &&
            HasDeferredVerdictStacks(player) &&
            IsDeferredVerdictConsumerCard(card) &&
            IsEligibleCard(card))
        {
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Deferred Verdict extra-played {card.Id.Entry} one additional time.");
            return playCount + DeferredVerdictExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.SingleSentence &&
            !combatState.SingleSentenceUsedThisTurn &&
            IsEligibleCard(card))
        {
            combatState.SingleSentenceUsedThisTurn = true;
            combatState.SingleSentenceRulingCard = card;
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Single Sentence extra-played {card.Id.Entry} two additional times.");
            return playCount + LothaExtraPlayCount;
        }

        return playCount;
    }

    public static bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (autoPlayType == AutoPlayType.None)
        {
            if (ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
            {
                combatState.AutoPlayCardPendingModifier = null;
            }
        }
        else
        {
            combatState.AutoPlayCardPendingModifier = card;
            return true;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.SingleSentence ||
            !combatState.SingleSentenceUsedThisTurn)
        {
            return true;
        }

        return combatState.SingleSentenceRemainingCardsPlayedThisTurn < SingleSentenceRemainingPlayLimit;
    }

    public static async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        var selectedBlessing = GetSelectedBlessing(player);

        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal)
        {
            await TryResolveMirrorRebuttalPowerFallback(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.MirrorHallEcho)
        {
            await TryResolveMirrorHallEchoPowerFallback(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict)
        {
            await TryResolveDeferredVerdictCard(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.SingleSentence)
        {
            await TryResolveSingleSentencePowerFallback(choiceContext, cardPlay, combatState);
            TrackSingleSentenceRemainingPlays(cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.ClosedCourt)
        {
            TrackClosedCourtDiscountUse(cardPlay, combatState);
        }
    }

    public static async Task AfterCombatEnd(CombatRoom room)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            if (GetSelectedBlessing(player) != LothaBlessingIds.DeferredVerdict ||
                combatState.DeferredVerdictGranted ||
                player.Creature.CombatState?.RoundNumber >= DeferredVerdictTurn ||
                !player.Creature.IsAlive)
            {
                continue;
            }

            combatState.DeferredVerdictGranted = true;
            await CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal, playAnim: false);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict healed 4 HP because combat ended before turn 4.");
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            if (GetSelectedBlessing(player) == LothaBlessingIds.DeferredVerdict)
            {
                await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
            }

            if (GetSelectedBlessing(player) == LothaBlessingIds.DeathReprieve)
            {
                combatState.DeathReprieveActive = false;
                combatState.DeathReprievePendingStart = false;
                ResolveDeathReprieveProgress(player);
                await PowerCmd.Remove<LothaDeathReprievePower>(player.Creature);
            }
        }
    }

    public static async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!target.IsPlayer ||
            target.Player is not { } player ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.Presumption)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.PresumptionLost ||
            !IsUnblockedEnemyAttackDamage(result, props, dealer, cardSource))
        {
            return;
        }

        combatState.PresumptionLost = true;
        await PowerCmd.Remove<LothaPresumptionPower>(player.Creature);
        await CreatureCmd.Damage(
            choiceContext,
            player.Creature,
            PresumptionHpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence broke after unblocked enemy attack damage and applied 8 HP loss.");
    }

    public static bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (!player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.ClosedCourt ||
            room is not CombatRoom)
        {
            return false;
        }

        var removed = rewards.RemoveAll(reward => reward is CardReward);
        if (removed <= 0)
        {
            return false;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha Closed Court suppressed {removed} post-combat card reward(s); gold, potion, and relic rewards remain.");
        return true;
    }

    public static bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (IsDeathReprieveCostFree(player, combatState))
        {
            modifiedCost = 0;
            return true;
        }

        if (IsPowerReplacementCostZeroCard(card, player, combatState))
        {
            combatState.PowerReplacementCardPendingBenefit = card;
            modifiedCost = 0;
            return true;
        }

        if (GetSelectedBlessing(player) == LothaBlessingIds.ClosedCourt &&
            combatState.ClosedCourtDiscountActiveThisTurn &&
            combatState.ClosedCourtDiscountsRemainingThisTurn > 0 &&
            card.Pile?.Type == PileType.Hand)
        {
            modifiedCost = Math.Max(0, originalCost - 1);
            return modifiedCost != originalCost;
        }

        return false;
    }

    public static bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (IsDeathReprieveCostFree(player, combatState))
        {
            modifiedCost = 0;
            return true;
        }

        if (IsPowerReplacementCostZeroCard(card, player, combatState))
        {
            combatState.PowerReplacementCardPendingBenefit = card;
            modifiedCost = 0;
            return true;
        }

        return false;
    }

    public static decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target)
    {
        if (amount == 0m ||
            target is not { IsEnemy: true } ||
            !giver.IsPlayer ||
            giver.Player is not { } player ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence ||
            !IsPublicEvidenceDebuffApplication(power, amount))
        {
            return amount;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha Public Evidence doubled player-applied debuff {power.Id.Entry}.");
        return amount * 2m;
    }

    public static bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (amount == 0m ||
            !target.IsPlayer ||
            target.Player is not { } player ||
            !player.IsActiveForHooks ||
            applier is not { IsEnemy: true } ||
            GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence ||
            !IsPublicEvidenceDebuffApplication(canonicalPower, amount))
        {
            return false;
        }

        modifiedAmount = amount * 2m;
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Public Evidence doubled enemy-applied debuff {canonicalPower.Id.Entry}.");
        return true;
    }

    public static async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount == 0m || !IsPublicEvidenceDebuffApplication(power, amount))
        {
            return;
        }

        if (applier is { IsPlayer: true, Player: { } applyingPlayer } &&
            applyingPlayer.IsActiveForHooks &&
            power.Owner.IsEnemy &&
            GetSelectedBlessing(applyingPlayer) == LothaBlessingIds.PublicEvidence)
        {
            await PowerCmd.Apply<LothaEnlightenmentPower>(
                choiceContext,
                applyingPlayer.Creature,
                PublicEvidenceEnlightenmentGain,
                applyingPlayer.Creature,
                cardSource);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Public Evidence granted Enlightenment after a player-applied debuff.");
            return;
        }

        if (power.Owner is { IsPlayer: true, Player: { } targetPlayer } &&
            targetPlayer.IsActiveForHooks &&
            applier is { IsEnemy: true } &&
            GetSelectedBlessing(targetPlayer) == LothaBlessingIds.PublicEvidence)
        {
            await RemoveOnePublicEvidenceEnlightenment(choiceContext, targetPlayer);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Public Evidence removed Enlightenment after an enemy-applied debuff.");
        }
    }

    public static bool ShouldDieLate(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return true;
        }

        if (creature.Player is not { } player)
        {
            return true;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
        {
            return false;
        }

        return GetProgress(player).DeathReprieveUsed;
    }

    public static bool ShouldDie(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        return !(combatState.DeathReprieveActive || combatState.DeathReprievePendingStart);
    }

    public static async Task AfterPreventingDeath(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return;
        }

        if (creature.Player is not { } player)
        {
            return;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        var progress = GetProgress(player);
        if (progress.DeathReprieveUsed)
        {
            if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
            {
                await CreatureCmd.SetCurrentHp(creature, 1m);
                MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve kept the player at 1 HP during the reprieve turn.");
            }

            return;
        }

        await CreatureCmd.SetCurrentHp(creature, 1m);

        if (creature.CombatState?.CurrentSide == CombatSide.Player &&
            CombatManager.Instance.IsPartOfPlayerTurn(player))
        {
            SetProgress(player, progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.Active
            });
            await StartDeathReprieveTurn(new ThrowingPlayerChoiceContext(), player, combatState, "current player turn after lethal damage");
        }
        else
        {
            SetProgress(player, progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.PendingStart
            });
            combatState.DeathReprievePendingStart = true;
            combatState.DeathReprieveActive = true;
            await EnsureDeathReprievePower(new ThrowingPlayerChoiceContext(), player);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve prevented lethal damage; reprieve turn is pending at the next player turn.");
        }
    }

    private static void ResetCombatState(LothaCombatState combatState)
    {
        combatState.MirrorRebuttalCardPulled = false;
        combatState.MirrorRebuttalResolved = false;
        combatState.MirrorHallEchoRecordedType = null;
        combatState.MirrorHallEchoArmedType = null;
        combatState.MirrorHallEchoConsumedThisTurn = false;
        combatState.ClosedCourtUsed = false;
        combatState.ClosedCourtDiscountActiveThisTurn = false;
        combatState.ClosedCourtDiscountsRemainingThisTurn = 0;
        combatState.PresumptionLost = false;
        combatState.DeferredVerdictGranted = false;
        combatState.DeferredVerdictActiveThisTurn = false;
        combatState.DeathReprieveActive = false;
        combatState.DeathReprievePendingStart = false;
        combatState.DeathReprieveStarted = false;
        combatState.SingleSentenceUsedThisTurn = false;
        combatState.SingleSentencePowerFallbackUsedThisTurn = false;
        combatState.SingleSentenceRemainingCardsPlayedThisTurn = 0;
        combatState.SingleSentenceRulingCard = null;
        combatState.AutoPlayCardPendingModifier = null;
        combatState.PowerReplacementCardPendingBenefit = null;
    }

    private static void ResetTurnState(LothaCombatState combatState)
    {
        combatState.DeferredVerdictActiveThisTurn = false;
        combatState.MirrorHallEchoArmedType = combatState.MirrorHallEchoRecordedType;
        combatState.MirrorHallEchoRecordedType = null;
        combatState.MirrorHallEchoConsumedThisTurn = false;
        combatState.ClosedCourtDiscountActiveThisTurn = false;
        combatState.ClosedCourtDiscountsRemainingThisTurn = 0;
        combatState.SingleSentenceUsedThisTurn = false;
        combatState.SingleSentencePowerFallbackUsedThisTurn = false;
        combatState.SingleSentenceRemainingCardsPlayedThisTurn = 0;
        combatState.SingleSentenceRulingCard = null;
        combatState.AutoPlayCardPendingModifier = null;
        combatState.PowerReplacementCardPendingBenefit = null;
    }

    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, LothaCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }

    private static async Task TryResolveMirrorRebuttalPowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (combatState.MirrorRebuttalResolved ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !CanUseMirrorRebuttalPowerReplacement(cardPlay.Card, combatState))
        {
            return;
        }

        combatState.MirrorRebuttalResolved = true;
        combatState.PowerReplacementCardPendingBenefit = null;
        await ApplyPowerReplacementBenefit(
            choiceContext,
            cardPlay.Card.Owner,
            MirrorRebuttalPowerFallbackEnergy,
            MirrorRebuttalPowerFallbackCards);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Mirror Rebuttal used the Power-card replacement benefit: cost 0, Energy 2, and draw 2.");
    }

    private static async Task TryResolveMirrorHallEchoPowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (combatState.MirrorHallEchoConsumedThisTurn ||
            cardPlay.IsAutoPlay ||
            !cardPlay.IsFirstInSeries ||
            !CanUseMirrorHallEchoPowerReplacement(cardPlay.Card, combatState))
        {
            return;
        }

        combatState.MirrorHallEchoConsumedThisTurn = true;
        combatState.MirrorHallEchoArmedType = null;
        combatState.PowerReplacementCardPendingBenefit = null;
        await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Mirror Hall Echo used the Power-card replacement benefit: cost 0 and draw 1.");
    }

    private static async Task TryResolveDeferredVerdictCard(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (!combatState.DeferredVerdictActiveThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !IsDeferredVerdictConsumerCard(cardPlay.Card))
        {
            return;
        }

        var player = cardPlay.Card.Owner;
        var usesPowerReplacement = CanUseDeferredVerdictPowerReplacement(cardPlay.Card, player, combatState);
        var verdict = player.Creature.GetPower<LothaVerdictPower>();
        if (verdict is not { Amount: > 0 })
        {
            return;
        }

        await PowerCmd.Decrement(verdict);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict consumed 1 player-owned Verdict.");

        if (usesPowerReplacement)
        {
            combatState.PowerReplacementCardPendingBenefit = null;
            await ApplyPowerReplacementBenefit(choiceContext, player);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict used the Power-card replacement benefit: cost 0 and draw 1.");
        }
    }

    private static async Task TryResolveSingleSentencePowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (combatState.SingleSentenceUsedThisTurn ||
            combatState.SingleSentencePowerFallbackUsedThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState))
        {
            return;
        }

        combatState.SingleSentencePowerFallbackUsedThisTurn = true;
        combatState.PowerReplacementCardPendingBenefit = null;
        await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Single Sentence Power fallback cost 0, drew 1 card, and did not consume the sentence.");
    }

    private static void TrackSingleSentenceRemainingPlays(CardPlay cardPlay, LothaCombatState combatState)
    {
        if (!combatState.SingleSentenceUsedThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            cardPlay.Card.IsClone)
        {
            return;
        }

        if (ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard))
        {
            combatState.SingleSentenceRulingCard = null;
            return;
        }

        combatState.SingleSentenceRemainingCardsPlayedThisTurn++;
    }

    private static void TrackClosedCourtDiscountUse(CardPlay cardPlay, LothaCombatState combatState)
    {
        if (!combatState.ClosedCourtDiscountActiveThisTurn ||
            combatState.ClosedCourtDiscountsRemainingThisTurn <= 0 ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay)
        {
            return;
        }

        combatState.ClosedCourtDiscountsRemainingThisTurn--;
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Closed Court consumed a first-turn discount; {combatState.ClosedCourtDiscountsRemainingThisTurn} remain.");
    }

    private static async Task TryMoveMirrorRebuttalCardToHand(Player player)
    {
        var selectedCard = player.PlayerCombatState?.AllCards.FirstOrDefault(IsMirrorRebuttalCombatCard);
        if (selectedCard == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha Mirror Rebuttal skipped combat-start pull: selected deck card was not found in combat.");
            return;
        }

        if (selectedCard.Pile?.Type == PileType.Hand)
        {
            MainFile.Logger.Info("[EZMicroBalance] Lotha Mirror Rebuttal selected card already started in hand.");
            return;
        }

        if (selectedCard.Pile?.Type.IsCombatPile() != true)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha Mirror Rebuttal skipped combat-start pull: selected card is not in a combat pile.");
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
        {
            await CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top);
            MainFile.Logger.Warn($"[EZMicroBalance] Lotha Mirror Rebuttal could not move selected card {selectedCard.Id.Entry} into a full hand; placed it on top of draw pile instead.");
            return;
        }

        var addResult = await CardPileCmd.Add(selectedCard, PileType.Hand);
        if (addResult.cardAdded.Pile?.Type == PileType.Hand)
        {
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal moved selected card {selectedCard.Id.Entry} into hand.");
        }
        else
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Lotha Mirror Rebuttal tried to move selected card {selectedCard.Id.Entry} into hand but it ended in {addResult.cardAdded.Pile?.Type.ToString() ?? "no pile"}.");
        }
    }

    private static async Task ConsumePublicEvidenceEnlightenmentAtTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var enlightenment = player.Creature.GetPower<LothaEnlightenmentPower>();
        if (enlightenment is not { Amount: > 0 })
        {
            return;
        }

        var consumed = Math.Min(PublicEvidenceConsumeLimit, enlightenment.Amount);
        await PowerCmd.ModifyAmount(choiceContext, enlightenment, -consumed, player.Creature, null);
        for (var i = 0; i < consumed; i++)
        {
            await CardPileCmd.Draw(choiceContext, PublicEvidenceCardsPerEnlightenment, player);
            await CreatureCmd.GainBlock(player.Creature, PublicEvidenceBlockPerEnlightenment, ValueProp.Move, null, fast: true);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha Public Evidence consumed {consumed} Enlightenment at turn start.");
    }

    private static async Task RemoveOnePublicEvidenceEnlightenment(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var enlightenment = player.Creature.GetPower<LothaEnlightenmentPower>();
        if (enlightenment is not { Amount: > 0 })
        {
            return;
        }

        await PowerCmd.Decrement(enlightenment);
    }

    private static void RecordMirrorHallEchoType(Player player, LothaCombatState combatState)
    {
        var lastPlayedType = CombatManager.Instance.History.CardPlaysFinished
            .Where(entry =>
                entry.Actor == player.Creature &&
                entry.CardPlay.IsFirstInSeries &&
                !entry.CardPlay.IsAutoPlay &&
                !entry.CardPlay.Card.IsClone &&
                IsMirrorHallEchoRecordableType(entry.CardPlay.Card.Type) &&
                entry.HappenedThisTurn(player.Creature.CombatState))
            .Select(entry => (CardType?)entry.CardPlay.Card.Type)
            .LastOrDefault();

        combatState.MirrorHallEchoRecordedType = lastPlayedType;
        if (lastPlayedType.HasValue)
        {
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Hall Echo recorded {lastPlayedType.Value} for next turn.");
        }
    }

    private static async Task StartDeathReprieveTurn(
        PlayerChoiceContext choiceContext,
        Player player,
        LothaCombatState combatState,
        string source)
    {
        if (combatState.DeathReprieveStarted)
        {
            return;
        }

        combatState.DeathReprieveStarted = true;
        combatState.DeathReprieveActive = true;
        combatState.DeathReprievePendingStart = false;
        SetProgress(player, GetProgress(player) with
        {
            DeathReprieveUsed = true,
            DeathReprievePhase = DeathReprievePhase.Active
        });
        await CreatureCmd.SetCurrentHp(player.Creature, 1m);
        await EnsureDeathReprievePower(choiceContext, player);
        await CardPileCmd.Draw(choiceContext, DeathReprieveCards, player);
        await PlayerCmd.GainEnergy(DeathReprieveEnergy, player);
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Death Reprieve started the reprieve turn from {source}: draw 10, Energy 10, all costs 0.");
    }

    private static async Task EnsureDeathReprievePower(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.GetPower<LothaDeathReprievePower>() != null)
        {
            return;
        }

        await PowerCmd.Apply<LothaDeathReprievePower>(
            choiceContext,
            player.Creature,
            1,
            player.Creature,
            null);
    }

    private static async Task ResolveDeathReprieveTurnEnd(Player player, LothaCombatState combatState)
    {
        combatState.DeathReprieveActive = false;
        combatState.DeathReprievePendingStart = false;
        ResolveDeathReprieveProgress(player);
        await PowerCmd.Remove<LothaDeathReprievePower>(player.Creature);

        if (player.Creature.CombatState?.Enemies.Any(enemy => enemy.IsAlive) == true)
        {
            MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve ended with enemies alive; killing the player with force=true.");
            await CreatureCmd.Kill(player.Creature, force: true);
            return;
        }

        MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve ended after victory; the run continues.");
    }

    private static async Task ApplyPowerReplacementBenefit(PlayerChoiceContext choiceContext, Player player) =>
        await CardPileCmd.Draw(choiceContext, PowerFallbackCards, player);

    private static async Task ApplyPowerReplacementBenefit(PlayerChoiceContext choiceContext, Player player, int energy, int cards)
    {
        await PlayerCmd.GainEnergy(energy, player);
        await CardPileCmd.Draw(choiceContext, cards, player);
    }

    private static bool IsPowerReplacementCostZeroCard(CardModel card, Player player, LothaCombatState combatState)
    {
        if (!IsPowerCard(card) ||
            card.Pile?.Type != PileType.Hand)
        {
            return false;
        }

        return GetSelectedBlessing(player) switch
        {
            LothaBlessingIds.MirrorRebuttal =>
                CanUseMirrorRebuttalPowerReplacement(card, combatState),
            LothaBlessingIds.MirrorHallEcho =>
                CanUseMirrorHallEchoPowerReplacement(card, combatState),
            LothaBlessingIds.DeferredVerdict =>
                CanUseDeferredVerdictPowerReplacement(card, player, combatState),
            LothaBlessingIds.SingleSentence =>
                CanUseSingleSentencePowerReplacement(card, combatState),
            _ => false,
        };
    }

    private static bool CanUseMirrorRebuttalPowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.MirrorRebuttalResolved &&
        IsPowerCard(card) &&
        IsMirrorRebuttalCombatCard(card);

    private static bool CanUseMirrorHallEchoPowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.MirrorHallEchoConsumedThisTurn &&
        combatState.MirrorHallEchoArmedType == CardType.Power &&
        IsPowerCard(card);

    private static bool CanUseDeferredVerdictPowerReplacement(
        CardModel card,
        Player player,
        LothaCombatState combatState) =>
        combatState.DeferredVerdictActiveThisTurn &&
        HasDeferredVerdictStacks(player) &&
        IsPowerCard(card);

    private static bool CanUseSingleSentencePowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.SingleSentenceUsedThisTurn &&
        !combatState.SingleSentencePowerFallbackUsedThisTurn &&
        IsPowerCard(card);

    private static bool IsEligibleCard(CardModel card) =>
        card.Type is CardType.Attack or CardType.Skill && !card.IsClone;

    private static bool IsPowerCard(CardModel card) =>
        card.Type == CardType.Power && !card.IsClone;

    private static bool IsDeferredVerdictConsumerCard(CardModel card) =>
        card.Type != CardType.Status && !card.IsClone;

    private static bool IsMirrorHallEchoRecordableType(CardType type) =>
        type is CardType.Attack or CardType.Skill or CardType.Power;

    private static bool HasDeferredVerdictStacks(Player player) =>
        player.Creature.GetPower<LothaVerdictPower>() is { Amount: > 0 };

    private static bool IsMirrorRebuttalCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard &&
        AncientSavedStateFields.LothaMirrorRebuttalCard[deckCard];

    private static bool IsPublicEvidenceDebuffApplication(PowerModel power, decimal amount) =>
        power.GetTypeForAmount(amount) == PowerType.Debuff &&
        !IsPublicEvidenceExcludedDamageDebuff(power);

    private static bool IsPublicEvidenceExcludedDamageDebuff(PowerModel power) =>
        // Core v0.105.0 models these as Debuffs, but their source resolves damage, kill, or poison ticks.
        power is PoisonPower
            or ConstrictPower
            or DemisePower
            or DisintegrationPower
            or DoomPower
            or MagicBombPower
            or StranglePower
            or TheGambitPower;

    private static bool IsUnblockedEnemyAttackDamage(
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        result.UnblockedDamage > 0 &&
        !result.WasFullyBlocked &&
        dealer is { IsEnemy: true } &&
        cardSource == null &&
        props.HasFlag(ValueProp.Move);

    private static bool IsDeathReprieveCostFree(Player player, LothaCombatState combatState) =>
        GetSelectedBlessing(player) == LothaBlessingIds.DeathReprieve &&
        combatState.DeathReprieveActive;

    private static void HydrateDeathReprieveState(Player player, LothaCombatState combatState)
    {
        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return;
        }

        var progress = GetProgress(player);
        if (!progress.DeathReprieveUsed ||
            !IsRecoverableDeathReprievePhase(progress.DeathReprievePhase) ||
            combatState.DeathReprieveActive ||
            combatState.DeathReprievePendingStart)
        {
            return;
        }

        var alreadyHasPower = player.Creature.GetPower<LothaDeathReprievePower>() != null;
        combatState.DeathReprieveActive = true;
        combatState.DeathReprievePendingStart = !alreadyHasPower;
        combatState.DeathReprieveStarted = progress.DeathReprievePhase == DeathReprievePhase.Active && alreadyHasPower;
        MainFile.Logger.Info(
            $"[EZMicroBalance] Lotha Death Reprieve restored {progress.DeathReprievePhase} combat state from deck-mirrored blessing progress; " +
            $"pendingStart={combatState.DeathReprievePendingStart}, powerAlreadyPresent={alreadyHasPower}. Active-turn save/load continuation remains live-pending.");
    }

    private static bool IsRecoverableDeathReprievePhase(DeathReprievePhase phase) =>
        phase is DeathReprievePhase.PendingStart or DeathReprievePhase.Active;

    private static void ResolveDeathReprieveProgress(Player player)
    {
        var progress = GetProgress(player);
        if (progress.DeathReprieveUsed && progress.DeathReprievePhase != DeathReprievePhase.Resolved)
        {
            SetProgress(player, progress with { DeathReprievePhase = DeathReprievePhase.Resolved });
        }
    }

    private static Progress GetProgress(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 2)
        {
            return Progress.Default;
        }

        var used = ParseBool(parts[1]);
        var phase = parts.Length >= 3
            ? ParseDeathReprievePhase(parts[2], used)
            : used ? DeathReprievePhase.Resolved : DeathReprievePhase.None;
        if (!used)
        {
            phase = DeathReprievePhase.None;
        }

        return new Progress(used, phase);
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
                progress.DeathReprieveUsed ? 1 : 0,
                (int)progress.DeathReprievePhase),
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
    }

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;

    private static DeathReprievePhase ParseDeathReprievePhase(string value, bool used)
    {
        if (int.TryParse(value, out var numeric) &&
            Enum.IsDefined(typeof(DeathReprievePhase), numeric))
        {
            return (DeathReprievePhase)numeric;
        }

        if (Enum.TryParse(value, ignoreCase: true, out DeathReprievePhase parsed))
        {
            return parsed;
        }

        return used ? DeathReprievePhase.Resolved : DeathReprievePhase.None;
    }
}
