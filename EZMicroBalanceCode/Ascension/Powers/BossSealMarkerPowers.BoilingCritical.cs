namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class BoilingCriticalBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.BoilingCritical;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：不可削弱",
        "瀑布巨兽进入爆发回合时，清除自身[gold]虚弱[/gold]和攻击降低。本回合爆发伤害不受虚弱或力量降低影响，并获得足够[gold]人工制品[/gold]直到爆发结算后。受爆发影响的玩家获得[gold]易伤[/gold]：A19为[blue]1[/blue]回合，[gold]烙印形态[/gold]为[blue]2[/blue]回合。",
        "爆发不能被虚弱或降攻压低，并会施加易伤。",
        "Dedicated Ability: Unweakenable",
        "When Waterfall Giant enters its explosion turn, clear its [gold]Weak[/gold] and attack reduction. The explosion ignores Weak and Strength loss, and the Giant gains enough [gold]Artifact[/gold] until the explosion resolves. Players hit by the explosion gain [gold]Vulnerable[/gold]: A19 [blue]1[/blue] turn, [gold]Branded Form[/gold] [blue]2[/blue] turns.",
        "The explosion ignores Weak and applies Vulnerable.");
}
