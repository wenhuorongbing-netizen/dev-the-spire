namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    public static Task AfterCreatureAddedToCombat(Creature creature) =>
        EnsureStolenVaultPower(creature);

    public static async Task EnsureStolenVaultPower(Creature creature)
    {
        if (creature.Monster is not EzmbVakuuTrialMonster ||
            creature.CombatState?.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        var remainingLocks = encounter.RemainingLocks;
        var vault = creature.GetPower<VakuuStolenVaultPower>();
        if (remainingLocks <= 0)
        {
            if (vault != null)
            {
                await PowerCmd.Remove(vault);
            }

            return;
        }

        if (vault == null)
        {
            await PowerCmd.Apply<VakuuStolenVaultPower>(
                new BlockingPlayerChoiceContext(),
                creature,
                remainingLocks,
                null,
                null,
                silent: true);
            return;
        }

        var delta = remainingLocks - vault.Amount;
        if (delta != 0)
        {
            await PowerCmd.ModifyAmount(
                new BlockingPlayerChoiceContext(),
                vault,
                delta,
                null,
                null);
        }
    }
}
