namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int BlueprintProofStacks = 3;
    private const int BlueprintProofCostReduction = 1;
    private const int BlueprintProofBlock = 4;

    private static async Task ResolveBlueprintProofAfterPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        MorviCombatState combatState)
    {
        var card = cardPlay.Card;
        var player = card.Owner;

        if (combatState.BlueprintTemporaryUpgradeCards.Remove(card))
        {
            CardCmd.Downgrade(card);
        }

        if (combatState.BlueprintDrawAfterCards.Remove(card))
        {
            await CardPileCmd.Draw(choiceContext, 1m, player);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof drew 1 after {card.Id.Entry}.");
        }

        if (combatState.BlueprintBlockAfterCards.Remove(card))
        {
            await CreatureCmd.GainBlock(player.Creature, BlueprintProofBlock, ValueProp.Move, cardPlay, fast: true);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof granted {BlueprintProofBlock} Block after upgraded card {card.Id.Entry}.");
        }
    }

    private static async Task EnsureBlueprintProofInitialized(
        Player player,
        MorviCombatState combatState,
        string reason)
    {
        if (!TryInitializeBlueprintProofState(player, combatState, reason))
        {
            return;
        }

        await SetCounterPower<MorviProofreadPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            combatState.ProofreadRemaining);
    }

    private static bool TryInitializeBlueprintProofState(
        Player player,
        MorviCombatState combatState,
        string reason)
    {
        if (combatState.BlueprintProofInitializedThisCombat ||
            GetSelectedBlessing(player) != MorviBlessingIds.BlueprintProof ||
            player.PlayerCombatState == null ||
            player.Creature.CombatState == null)
        {
            return false;
        }

        var visibleProofread = player.Creature.GetPower<MorviProofreadPower>()?.Amount ?? 0;
        combatState.ProofreadRemaining = visibleProofread > 0
            ? visibleProofread
            : BlueprintProofStacks;
        combatState.BlueprintProofInitializedThisCombat = true;
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof initialized {combatState.ProofreadRemaining} Proofread ({reason}).");
        return true;
    }

    private static bool IsBlueprintProofEligible(CardModel card) =>
        IsNaturalPlayerCombatCard(card) &&
        card.Type is not CardType.Status and not CardType.Curse;
}
