using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ChosenDecreeReductionPower : BossSealPower
{
    private sealed class Data
    {
        public bool Used { get; set; }
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：择令",
            "下一次由[gold]女王[/gold]给予的[gold]力量[/gold]减少[blue]1[/blue]，然后移除此效果。",
            "女王下一次[gold]力量[/gold]强化-[blue]1[/blue]。")
        : new PowerLoc(
            "Royal Seal: Chosen Decree",
            "The next [gold]Strength[/gold] gain from the [gold]Queen[/gold] is reduced by [blue]1[/blue], then this is removed.",
            "Next Queen [gold]Strength[/gold] gain -[blue]1[/blue].");

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner ||
            canonicalPower is not StrengthPower ||
            amount <= 0m ||
            applier?.Monster is not Queen ||
            GetInternalData<Data>().Used)
        {
            return false;
        }

        GetInternalData<Data>().Used = true;
        modifiedAmount = Math.Max(0m, amount - 1m);
        return true;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (GetInternalData<Data>().Used)
        {
            await PowerCmd.Remove(this);
        }
    }
}
