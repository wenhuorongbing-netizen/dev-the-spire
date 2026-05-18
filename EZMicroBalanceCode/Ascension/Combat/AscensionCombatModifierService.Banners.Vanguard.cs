namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private const int VanguardRemovalRound = 3;

    private static async Task ApplyVanguardCombatStart(CombatState combatState)
    {
        var strength = GetVanguardStrength(combatState);
        foreach (var enemy in PrimaryAliveEnemies(combatState))
        {
            await PowerCmd.Apply<VanguardBannerPower>(new BlockingPlayerChoiceContext(), enemy, strength, enemy, null);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Vanguard banner granted enemies temporary Strength.");
    }

    private static async Task RemoveVanguardStrength(CombatState combatState)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            var power = enemy.GetPower<VanguardBannerPower>();
            if (power == null)
            {
                continue;
            }

            await AscensionPowerAmountHelper.RemoveTemporaryStrength(enemy, power.Amount);
            await PowerCmd.Remove(power);
        }
    }
}
