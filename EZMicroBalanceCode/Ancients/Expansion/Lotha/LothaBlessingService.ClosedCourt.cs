using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int ClosedCourtFirstTurn = 1;
    private const int ClosedCourtFirstTurnCards = 4;
    private const int ClosedCourtFirstTurnEnergy = 2;
    private const int ClosedCourtSecondPulseTurn = 4;
    private const int ClosedCourtSecondPulseCards = 2;
    private const int ClosedCourtSecondPulseEnergy = 2;

    private static async Task TryApplyClosedCourtTurnStart(
        PlayerChoiceContext choiceContext,
        Player player,
        LothaCombatState combatState,
        string selectedBlessing)
    {
        if (selectedBlessing != LothaBlessingIds.ClosedCourt ||
            player.Creature.CombatState is not { } activeCombat)
        {
            return;
        }

        if (activeCombat.RoundNumber == ClosedCourtFirstTurn && !combatState.ClosedCourtFirstTurnUsed)
        {
            combatState.ClosedCourtFirstTurnUsed = true;
            await CardPileCmd.Draw(choiceContext, ClosedCourtFirstTurnCards, player);
            await PlayerCmd.GainEnergy(ClosedCourtFirstTurnEnergy, player);
            MainFile.Logger.Info("[Spire Plus] Lotha Closed Court turn 1 granted draw 4 and Energy 2.");
            return;
        }

        if (activeCombat.RoundNumber == ClosedCourtSecondPulseTurn && !combatState.ClosedCourtSecondPulseUsed)
        {
            combatState.ClosedCourtSecondPulseUsed = true;
            await CardPileCmd.Draw(choiceContext, ClosedCourtSecondPulseCards, player);
            await PlayerCmd.GainEnergy(ClosedCourtSecondPulseEnergy, player);
            MainFile.Logger.Info("[Spire Plus] Lotha Closed Court turn 4 granted draw 2 and Energy 2.");
        }
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

        MainFile.Logger.Info($"[Spire Plus] Lotha Closed Court suppressed {removed} post-combat card reward(s); gold, potion, and relic rewards remain.");
        return true;
    }
}
