using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static async Task BeforeCombatStart()
    {
        var activeCombatState = CombatManager.Instance.DebugOnlyGetState();
        if (activeCombatState == null)
        {
            return;
        }

        foreach (var player in activeCombatState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            ResetCombatState(combatState);
            HydrateDeathReprieveState(player, combatState);

            await TryApplyPresumptionCombatStart(player);

            if (GetSelectedBlessing(player) == LothaBlessingIds.SingleSentence)
            {
                await EnsureSingleSentencePower(
                    new ThrowingPlayerChoiceContext(),
                    player,
                    SingleSentenceReadyDisplayAmount);
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

        if (selectedBlessing == LothaBlessingIds.SingleSentence)
        {
            await EnsureSingleSentencePower(choiceContext, player, SingleSentenceReadyDisplayAmount);
        }

        await TryApplyPresumptionTurnStart(choiceContext, player, combatState, selectedBlessing);
        await TryOpenClosedCourtFirstTurn(choiceContext, player, combatState, selectedBlessing);

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

        var activeCombatState = CombatManager.Instance.DebugOnlyGetState();
        if (activeCombatState == null)
        {
            return;
        }

        foreach (var player in activeCombatState.Players.Where(player => player.IsActiveForHooks))
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

    public static async Task AfterCombatEnd(CombatRoom room)
    {
        var players = room.CombatState.RunState.Players.Where(player => player.IsActiveForHooks).ToList();

        foreach (var player in players)
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

        foreach (var player in players)
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
}
