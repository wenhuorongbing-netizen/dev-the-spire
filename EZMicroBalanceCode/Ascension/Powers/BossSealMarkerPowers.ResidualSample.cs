namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ResidualSampleBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.ResidualSample;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：实验记录",
        "实验体进入新阶段时，根据上一阶段记录获得[blue]1[/blue]份残留样本：力量残留、技能适应、攻击适应、抗体样本或污染样本。[gold]烙印形态[/gold]每次获得[blue]2[/blue]份不同样本。样本会改变下一阶段的首次出牌、负面状态或洗牌结算。",
        "上一阶段的打法会留下样本影响下一阶段。",
        "Dedicated Ability: Experimental Record",
        "When Test Subject enters a new phase, it gains [blue]1[/blue] Residual Sample based on the previous phase: Strength Residue, Skill Adaptation, Attack Adaptation, Antibody Sample, or Contaminated Sample. [gold]Branded Form[/gold] gains [blue]2[/blue] different samples each time. Samples affect the next phase's first card-count, debuff, or shuffle event.",
        "The previous phase leaves samples for the next phase.");
}
