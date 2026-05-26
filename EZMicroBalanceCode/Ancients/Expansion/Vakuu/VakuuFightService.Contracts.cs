namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    public static async Task SignContract(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        decimal hpLoss)
    {
        if (player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            player.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            null,
            cardSource);

        await AddBloodDebt(choiceContext, player, cardSource, 1);
        await BreakLock(choiceContext, combatState, "contract");
        MainFile.Logger.Info(
            $"[Spire Plus] Vakuu contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
    }

    public static async Task BreakLockFromContract(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        int bloodDebt,
        int backlash)
    {
        if (player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        await AddBloodDebt(choiceContext, player, cardSource, bloodDebt);
        var vakuu = FindVakuuCreature(combatState);
        if (backlash > 0 && vakuu != null)
        {
            await PowerCmd.Apply<VakuuBacklashPower>(
                choiceContext,
                vakuu,
                backlash,
                player.Creature,
                cardSource);
        }

        await BreakLock(choiceContext, combatState, "contract");
        MainFile.Logger.Info(
            $"[Spire Plus] Vakuu risky contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
    }

    public static async Task CashOut(PlayerChoiceContext choiceContext, Player player, CardModel cardSource)
    {
        if (player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter ||
            encounter.BrokenLocks <= 0)
        {
            return;
        }

        var vakuu = FindVakuuCreature(combatState);
        if (vakuu == null || vakuu.IsDead)
        {
            return;
        }

        encounter.CashedOut = true;
        MainFile.Logger.Info(
            $"[Spire Plus] Vakuu cash out played after {encounter.BrokenLocks} broken locks and {encounter.BloodDebt} Blood Debt.");
        await CreatureCmd.Kill(vakuu, force: true);
    }
}
