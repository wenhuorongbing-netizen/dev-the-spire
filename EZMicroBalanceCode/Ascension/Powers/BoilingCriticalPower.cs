using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class BoilingCriticalPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.BoilingCritical;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => 0;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "不可削弱",
            "爆发回合，瀑布巨兽清除自身[gold]虚弱[/gold]和攻击降低，并获得足够[gold]人工制品[/gold]直到爆发结算后。本次爆发伤害不会被虚弱或攻击降低压低，并会使受影响玩家[gold]易伤[/gold]。",
            "爆发不能被虚弱或降攻压低，并施加易伤。")
        : new PowerLoc(
            "Unweakenable",
            "On the explosion turn, Waterfall Giant clears [gold]Weak[/gold] and attack reduction, then gains enough [gold]Artifact[/gold] until the explosion resolves. The explosion cannot be reduced by Weak or attack reduction, and affected players become [gold]Vulnerable[/gold].",
            "The explosion ignores Weak and applies Vulnerable.");
}
