namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MartyrOathBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.MartyrOath;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：殉誓",
        "亲族随从死亡时，亲族祭司获得[gold]殉誓[/gold]，最多[blue]2[/blue]枚。下一次施加负面状态时，每枚使持续时间+[blue]1[/blue]；下一次攻击时，每次命中每枚额外造成[blue]3[/blue]点伤害。[gold]烙印形态[/gold]改为每枚[blue]4[/blue]点；若同一回合两名随从死亡，祭司获得[blue]1[/blue]层[gold]人工制品[/gold]。",
        "随从死亡会强化祭司的下一次负面状态或攻击。",
        "Dedicated Ability: Martyr Oath",
        "When a Kin follower dies, Kin Priest gains [gold]Martyr Oath[/gold], up to [blue]2[/blue]. The next debuff lasts [blue]1[/blue] longer per Oath; each hit of the next attack deals [blue]3[/blue] extra damage per Oath. [gold]Branded Form[/gold] changes the hit bonus to [blue]4[/blue], and if both followers die in one turn the Priest gains [blue]1[/blue] [gold]Artifact[/gold].",
        "Follower deaths empower Kin Priest's next debuff or attack.");
}
