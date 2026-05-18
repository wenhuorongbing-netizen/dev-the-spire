using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionPowerAmountHelper
{
    public static async Task RemoveTemporaryStrength(Creature creature, decimal amount)
    {
        if (amount <= 0m)
        {
            return;
        }

        var strength = creature.GetPower<StrengthPower>();
        if (strength == null)
        {
            return;
        }

        strength.SetAmount(strength.Amount - (int)amount, silent: true);
        if (strength.Amount == 0)
        {
            await PowerCmd.Remove(strength);
        }
    }
}
