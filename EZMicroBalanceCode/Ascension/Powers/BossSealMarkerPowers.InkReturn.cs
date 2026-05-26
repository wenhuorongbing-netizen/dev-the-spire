namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class InkReturnBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.InkReturn;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：墨返",
        "[gold]滑溜[/gold]首次被完全移除后，下个敌方回合开始时返还一部分。A19返还清除量的[blue]25%[/blue]，至少[blue]3[/blue]层，最多[blue]12[/blue]层。[gold]烙印形态[/gold]返还[blue]35%[/blue]，至少[blue]5[/blue]层，最多[blue]18[/blue]层。每场触发[blue]1[/blue]次。",
        "首次清除滑溜后会返还一次。",
        "Dedicated Ability: Ink Return",
        "The first time [gold]Slippery[/gold] is fully removed, part of it returns next enemy turn. A19 restores [blue]25%[/blue] of the cleared amount, min [blue]3[/blue], max [blue]12[/blue]. [gold]Branded Form[/gold] restores [blue]35%[/blue], min [blue]5[/blue], max [blue]18[/blue]. Triggers once.",
        "The first full Slippery removal returns once.");
}
