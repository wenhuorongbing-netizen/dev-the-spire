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

    public static async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        // Core skips AfterDamageReceived for lethal hits, so lock damage is tracked from the dealer-side hook.
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

        await AddBloodDebt(choiceContext, player, cardSource, 1);
        await BreakLock(choiceContext, combatState, "contract");
        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
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
            $"[EZMicroBalance] Vakuu risky contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
    }

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

        MainFile.Logger.Info($"[EZMicroBalance] Vakuu Blood Debt reduced by {removed}; remaining {encounter.BloodDebt}.");
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
            $"[EZMicroBalance] Vakuu cash out played after {encounter.BrokenLocks} broken locks and {encounter.BloodDebt} Blood Debt.");
        await CreatureCmd.Kill(vakuu, force: true);
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

        if (encounter.RemainingLocks <= 0)
        {
            if (vakuu is { IsDead: false })
            {
                encounter.CashedOut = true;
                MainFile.Logger.Info("[EZMicroBalance] Vakuu trial fully unlocked; ending fight through the normal victory path.");
                await CreatureCmd.Kill(vakuu, force: true);
            }

            return;
        }

        await VakuuContractService.OfferCashOutAfterLockBreak(choiceContext, combatState, encounter);
    }
}
