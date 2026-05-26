namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
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
            $"[Spire Plus] Vakuu Stolen Vault lock broken by {source}: {encounter.BrokenLocks}/{EzmbVakuuTrialEncounter.MaxLocks}.");

        if (encounter.RemainingLocks <= 0)
        {
            if (vakuu is { IsDead: false })
            {
                encounter.CashedOut = true;
                MainFile.Logger.Info("[Spire Plus] Vakuu trial fully unlocked; ending fight through the normal victory path.");
                await CreatureCmd.Kill(vakuu, force: true);
            }

            return;
        }

        await VakuuContractService.OfferCashOutAfterLockBreak(choiceContext, combatState, encounter);
    }
}
