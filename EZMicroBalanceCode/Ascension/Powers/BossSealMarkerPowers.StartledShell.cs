namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class StartledShellBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.StartledShell;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：多重护甲苏醒",
        "乐加维林族母被提前打醒时获得[blue]4[/blue]层[gold]多重护甲[/gold]；自然醒来时获得[blue]8[/blue]层。第一次[gold]摄魂[/gold]后，当前多重护甲减少一半。[gold]烙印形态[/gold]改为提前打醒[blue]6[/blue]层、自然醒来[blue]10[/blue]层，摄魂只减少三分之一。多人模式按首领战规则缩放最终层数。",
        "醒来时获得多重护甲；第一次摄魂会削减它。",
        "Dedicated Ability: Plating Wake",
        "Lagavulin Matriarch gains [blue]4[/blue] [gold]Plating[/gold] when woken early, or [blue]8[/blue] when it wakes naturally. After the first [gold]Soul Siphon[/gold], current Plating is halved. [gold]Branded Form[/gold] changes this to [blue]6[/blue] if woken early or [blue]10[/blue] if it wakes naturally, and only removes one third. Multiplayer uses the boss Plating scaling.",
        "Wake-up grants Plating; the first Soul Siphon trims it.");
}
