namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    public static bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.OverdueLibraryDiscountArmed &&
            !ReferenceEquals(combatState.OverdueLibraryDiscountSourceCard, card) &&
            card.Pile?.Type == PileType.Hand &&
            originalCost >= 0)
        {
            modifiedCost = 0;
            return modifiedCost != originalCost;
        }

        if (GetSelectedBlessing(player) == MorviBlessingIds.BlueprintProof &&
            card.Pile?.Type == PileType.Hand &&
            IsBlueprintProofEligible(card))
        {
            TryInitializeBlueprintProofState(player, combatState, "energy-cost guard");
            if (combatState.ProofreadRemaining > 0 && card.IsUpgraded)
            {
                modifiedCost = Math.Max(0, originalCost - BlueprintProofCostReduction);
                return modifiedCost != originalCost;
            }
        }

        return false;
    }

    public static async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks || !cardPlay.IsFirstInSeries || cardPlay.IsAutoPlay)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        TryConsumeOverdueLibraryDiscount(card, combatState);

        if (GetSelectedBlessing(player) != MorviBlessingIds.BlueprintProof ||
            !IsBlueprintProofEligible(card))
        {
            return;
        }

        await EnsureBlueprintProofInitialized(player, combatState, "before-card-play guard");
        if (combatState.ProofreadRemaining <= 0)
        {
            return;
        }

        combatState.ProofreadRemaining--;
        await SetCounterPower<MorviProofreadPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            combatState.ProofreadRemaining);

        if (card.IsUpgraded)
        {
            combatState.BlueprintBlockAfterCards.Add(card);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof armed upgraded-card Block for {card.Id.Entry}.");
            return;
        }

        if (card.IsUpgradable)
        {
            CardCmd.Upgrade(card, CardPreviewStyle.None);
            combatState.BlueprintTemporaryUpgradeCards.Add(card);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof temporarily upgraded {card.Id.Entry} for this play.");
        }

        combatState.BlueprintDrawAfterCards.Add(card);
    }

    public static async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return;
        }

        await ResolveBorrowedAncientPlayCost(choiceContext, cardPlay);

        var combatState = CombatStates.GetOrCreateValue(player);
        await ResolveMisprintPressAfterPlay(choiceContext, cardPlay, combatState);

        if (cardPlay.IsLastInSeries)
        {
            await ResolveBlueprintProofAfterPlay(choiceContext, cardPlay, combatState);
        }
    }
}
