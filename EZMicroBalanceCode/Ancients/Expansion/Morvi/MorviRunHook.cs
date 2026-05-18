namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
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

            switch (GetSelectedBlessing(player))
            {
                case MorviBlessingIds.OverdueLibrary:
                    await AddArchivePages(player);
                    break;
                case MorviBlessingIds.Paperstorm:
                    await StartPaperstormCombat(player, combatState);
                    break;
                case MorviBlessingIds.BlueprintProof:
                    await EnsureBlueprintProofInitialized(player, combatState, "combat start");
                    break;
                case MorviBlessingIds.DebtSettlement:
                    var progress = GetProgress(player);
                    if (progress.DebtRemaining > 0)
                    {
                        await SetCounterPower<MorviDebtPower>(
                            new ThrowingPlayerChoiceContext(),
                            player,
                            progress.DebtRemaining);
                    }
                    break;
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
        ResetTurnState(combatState);

        var selectedBlessing = GetSelectedBlessing(player);
        switch (selectedBlessing)
        {
            case MorviBlessingIds.RedInkOverdraft:
                await AddRedInkOverdraftCard(player);
                break;
            case MorviBlessingIds.OpenBookExam:
                await ResolveOpenBookTurnStart(choiceContext, player, combatState);
                break;
            case MorviBlessingIds.Paperstorm:
                await ResetPaperstormTurnCounter(choiceContext, player, combatState);
                break;
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
            await TrySealOpenBookAtTurnEnd(choiceContext, player);
        }
    }

    public static async Task AfterCombatEnd(CombatRoom room)
    {
        foreach (var player in room.CombatState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            var combatState = CombatStates.GetOrCreateValue(player);

            await CleanupMorviTemporaryCards(player);

            if (selectedBlessing == MorviBlessingIds.ForbiddenLoan &&
                room.RoomType == RoomType.Boss &&
                player.RunState.CurrentActIndex == 1)
            {
                await AutoSettleForbiddenLoan(player);
            }

            if (selectedBlessing == MorviBlessingIds.RedInkOverdraft)
            {
                await PayRedInkOverdraftDebts(player, combatState);
            }

            if (selectedBlessing == MorviBlessingIds.DebtSettlement)
            {
                await PayDebtSettlementDue(player);
            }
        }
    }

    private static void ResetCombatState(MorviCombatState combatState)
    {
        combatState.RedInkDebtsThisCombat = 0;
        combatState.OpenBookResolved = false;
        combatState.OpenBookDrawnCards.Clear();
        combatState.OpenBookSealedCards.Clear();
        combatState.ProofreadRemaining = 0;
        combatState.BlueprintProofInitializedThisCombat = false;
        combatState.BlueprintTemporaryUpgradeCards.Clear();
        combatState.BlueprintDrawAfterCards.Clear();
        combatState.BlueprintBlockAfterCards.Clear();
        ResetTurnState(combatState);
    }

    private static void ResetTurnState(MorviCombatState combatState)
    {
        combatState.MisprintUsedThisTurn = false;
        combatState.MisprintDrawAfterCards.Clear();
        combatState.AutoPlayCardPendingModifier = null;
        combatState.RedInkUsedThisTurn = false;
        combatState.OverdueLibraryDiscountArmed = false;
        combatState.OverdueLibraryDiscountSourceCard = null;
        combatState.PaperstormTriggersRemainingThisTurn = 0;
    }

}
