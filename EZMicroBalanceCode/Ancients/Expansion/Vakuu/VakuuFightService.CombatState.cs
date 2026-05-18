using MegaCrit.Sts2.Core.Commands;

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

    public static async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (result.UnblockedDamage <= 0 ||
            dealer?.IsPlayer != true ||
            target.Monster is not EzmbVakuuTrialMonster ||
            target.CombatState?.Encounter is not EzmbVakuuTrialEncounter encounter ||
            target.CombatState.CurrentSide != CombatSide.Player)
        {
            return;
        }

        var round = target.CombatState.RoundNumber;
        if (encounter.DamageRound != round)
        {
            encounter.DamageRound = round;
            encounter.DamageThisRound = 0m;
        }

        encounter.DamageThisRound += result.UnblockedDamage;
        if (encounter.DamageLockRound == round ||
            encounter.DamageThisRound < EzmbVakuuTrialEncounter.DamageLockThreshold)
        {
            return;
        }

        encounter.DamageLockRound = round;
        await BreakLock(choiceContext, target.CombatState, "damage threshold");
    }

    public static Creature? FindVakuuCreature(ICombatState? combatState) =>
        combatState?.Enemies.FirstOrDefault(enemy => enemy.Monster is EzmbVakuuTrialMonster);

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

        encounter.BloodDebt++;
        var vakuu = FindVakuuCreature(combatState);
        if (vakuu != null)
        {
            await PowerCmd.Apply<VakuuBloodDebtPower>(
                choiceContext,
                vakuu,
                1m,
                player.Creature,
                cardSource);
        }

        await BreakLock(choiceContext, combatState, "contract");
        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
    }

    private static async Task BreakLock(PlayerChoiceContext choiceContext, ICombatState combatState, string source)
    {
        if (combatState.Encounter is not EzmbVakuuTrialEncounter encounter ||
            encounter.RemainingLocks <= 0)
        {
            return;
        }

        encounter.BrokenLocks++;
        var vakuu = FindVakuuCreature(combatState);
        var vault = vakuu?.GetPower<VakuuStolenVaultPower>();
        if (vault != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, vault, -1m, null, null);
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu Stolen Vault lock broken by {source}: {encounter.BrokenLocks}/{EzmbVakuuTrialEncounter.MaxLocks}.");
    }
}
