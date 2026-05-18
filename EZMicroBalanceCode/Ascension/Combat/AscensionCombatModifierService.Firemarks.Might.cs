namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyMightFiremarkCombatStart(CombatState combatState, Creature host)
    {
        var strength = GetMightFiremarkStrength(combatState);
        await PowerCmd.Apply<MightMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, strength, host, null);
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), host, strength, host, null);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A12 applied: Might firemark host {host.Name} gained {strength} Strength.");
    }

    private static async Task AddFiremarkHeat(Creature host, AscensionCombatTracker tracker)
    {
        var heat = host.GetPower<FiremarkHeatPower>();
        if (heat == null)
        {
            await PowerCmd.Apply<FiremarkHeatPower>(new BlockingPlayerChoiceContext(), host, 1m, host, null);
            return;
        }

        await PowerCmd.Apply<FiremarkHeatPower>(new BlockingPlayerChoiceContext(), host, 1m, host, null);
        if (heat.Amount + 1m < 2m)
        {
            return;
        }

        await PowerCmd.Remove(host.GetPower<FiremarkHeatPower>());
        await PowerCmd.Apply<FiremarkHeatStrikePower>(new BlockingPlayerChoiceContext(), host, tracker.FiremarkBaseAmount, host, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Might firemark Heat is full; next attack is empowered.");
    }
}
