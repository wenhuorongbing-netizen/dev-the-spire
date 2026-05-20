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
}
