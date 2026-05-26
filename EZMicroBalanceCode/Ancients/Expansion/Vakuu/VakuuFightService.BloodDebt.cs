namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    public static async Task ReduceBloodDebt(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        int amount)
    {
        if (amount <= 0 ||
            player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter ||
            encounter.BloodDebt <= 0)
        {
            return;
        }

        var removed = Math.Min(amount, encounter.BloodDebt);
        encounter.BloodDebt -= removed;
        var vakuu = FindVakuuCreature(combatState);
        var debt = vakuu?.GetPower<VakuuBloodDebtPower>();
        if (debt != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, debt, -removed, player.Creature, cardSource);
            if (debt.Amount <= 0)
            {
                await PowerCmd.Remove(debt);
            }
        }

        MainFile.Logger.Info($"[Spire Plus] Vakuu Blood Debt reduced by {removed}; remaining {encounter.BloodDebt}.");
    }

    private static async Task AddBloodDebt(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        int amount)
    {
        if (amount <= 0 ||
            player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        encounter.BloodDebt += amount;
        var vakuu = FindVakuuCreature(combatState);
        if (vakuu != null)
        {
            await PowerCmd.Apply<VakuuBloodDebtPower>(
                choiceContext,
                vakuu,
                amount,
                player.Creature,
                cardSource);
        }
    }
}
