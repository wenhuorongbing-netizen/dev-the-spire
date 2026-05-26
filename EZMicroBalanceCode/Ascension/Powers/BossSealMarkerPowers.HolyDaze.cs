namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class HolyDazeBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.HolyDaze;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：圣昏",
        "首次眩晕期间，每次受击最多受到[blue]1[/blue]点伤害。眩晕结束后，A19获得[blue]1[/blue]点[gold]力量[/gold]；[gold]烙印形态[/gold]获得[blue]2[/blue]点。",
        "首次眩晕限制受击，并在结束后获得力量。",
        "Dedicated Ability: Holy Daze",
        "During the first stun, each hit deals at most [blue]1[/blue] damage. When the stun ends, A19 gains [blue]1[/blue] [gold]Strength[/gold]; [gold]Branded Form[/gold] gains [blue]2[/blue].",
        "The first stun caps hits and later grants Strength.");
}
