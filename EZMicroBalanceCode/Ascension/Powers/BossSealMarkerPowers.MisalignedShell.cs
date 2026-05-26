namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MisalignedShellBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.MisalignedShell;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：错壳校准",
        "玩家回合结束时，检查两只爪的生命百分比。若差距至少[blue]35%[/blue]，生命百分比较高的爪获得[blue]1[/blue]层校准。校准达到[blue]2[/blue]层时，该爪下一次攻击每次命中额外造成[blue]4[/blue]点伤害；每只爪每场最多触发[blue]1[/blue]次。[gold]烙印形态[/gold]改为[blue]30%[/blue]差距和每次命中+[blue]5[/blue]点伤害。",
        "两只爪血线差距过大时，高血爪会校准攻击。",
        "Dedicated Ability: Claw Calibration",
        "At player turn end, compare both claws' HP percentages. If the gap is at least [blue]35%[/blue], the higher-HP claw gains [blue]1[/blue] Calibration. At [blue]2[/blue] Calibration, each hit of its next attack deals [blue]4[/blue] extra damage; each claw can trigger once per combat. [gold]Branded Form[/gold] changes this to a [blue]30%[/blue] gap and [blue]5[/blue] extra damage per hit.",
        "Uneven claw HP makes the healthier claw calibrate its attack.");
}
