using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class BoilingCriticalPower : BossSealPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：沸腾临界",
            "死亡爆发每层额外造成[blue]2[/blue]点伤害。爆发回合开始时，玩家获得预警[gold]格挡[/gold]。",
            "爆发更强；爆发前会给出预警[gold]格挡[/gold]。")
        : new PowerLoc(
            "Royal Seal: Boiling Critical",
            "Death explosion deals [blue]2[/blue] more damage per stack. At the start of the explosion turn, players gain warning [gold]Block[/gold].",
            "Explosion is stronger and gives warning [gold]Block[/gold].");

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || Owner.Monster is not WaterfallGiant || Owner.Monster.NextMove.StateId != "EXPLODE_MOVE")
        {
            return 0m;
        }

        return Amount * 2m;
    }
}
