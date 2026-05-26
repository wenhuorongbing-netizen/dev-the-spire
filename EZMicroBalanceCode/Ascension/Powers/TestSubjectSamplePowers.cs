using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class TestSubjectSamplePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.ResidualSample;
}

internal sealed class TestSubjectSkillAdaptationPower : TestSubjectSamplePower
{
    private int _skillsThisTurn;

    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => Loc(
        "技能适应",
        "本阶段第一次在单回合打出第[blue]3[/blue]张技能牌时，实验体获得[blue]1[/blue]点[gold]力量[/gold]。",
        "第3张技能牌会给实验体力量。",
        "Skill Adaptation",
        "The first time [blue]3[/blue] Skills are played in one turn this phase, Test Subject gains [blue]1[/blue] [gold]Strength[/gold].",
        "The third Skill gives Test Subject Strength.");

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _skillsThisTurn = 0;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Skill || Owner.Monster is not TestSubject)
        {
            return;
        }

        _skillsThisTurn++;
        if (_skillsThisTurn >= 3)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 1m, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class TestSubjectAttackAdaptationPower : TestSubjectSamplePower
{
    private int _attacksThisTurn;

    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => Loc(
        "攻击适应",
        "本阶段第一次在单回合打出第[blue]4[/blue]张攻击牌时，实验体获得[blue]1[/blue]层[gold]人工制品[/gold]。",
        "第4张攻击牌会给实验体人工制品。",
        "Attack Adaptation",
        "The first time [blue]4[/blue] Attacks are played in one turn this phase, Test Subject gains [blue]1[/blue] [gold]Artifact[/gold].",
        "The fourth Attack gives Test Subject Artifact.");

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _attacksThisTurn = 0;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || Owner.Monster is not TestSubject)
        {
            return;
        }

        _attacksThisTurn++;
        if (_attacksThisTurn >= 4)
        {
            await ApplyFinalArtifact(choiceContext, Owner, 1);
            await PowerCmd.Remove(this);
        }
    }

    private static async Task ApplyFinalArtifact(PlayerChoiceContext choiceContext, Creature owner, int amount)
    {
        var existing = owner.GetPower<ArtifactPower>()?.Amount ?? 0;
        await PowerCmd.Apply<ArtifactPower>(choiceContext, owner, 1m, owner, null);
        var artifact = owner.GetPower<ArtifactPower>();
        if (artifact != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, artifact, existing + amount - artifact.Amount, owner, null);
        }
    }
}

internal sealed class TestSubjectAntibodySamplePower : TestSubjectSamplePower
{
    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => Loc(
        "抗体样本",
        "本阶段第一次受到[gold]负面状态[/gold]时，实验体获得[blue]2[/blue]层[gold]人工制品[/gold]。",
        "首次负面状态会给实验体人工制品。",
        "Antibody Sample",
        "The first time Test Subject receives a [gold]debuff[/gold] this phase, it gains [blue]2[/blue] [gold]Artifact[/gold].",
        "The first debuff gives Test Subject Artifact.");

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || power.GetTypeForAmount(amount) != PowerType.Debuff || amount <= 0m)
        {
            return;
        }

        await ApplyFinalArtifact(choiceContext, Owner, 2);
        await PowerCmd.Remove(this);
    }

    private static async Task ApplyFinalArtifact(PlayerChoiceContext choiceContext, Creature owner, int amount)
    {
        var existing = owner.GetPower<ArtifactPower>()?.Amount ?? 0;
        await PowerCmd.Apply<ArtifactPower>(choiceContext, owner, 1m, owner, null);
        var artifact = owner.GetPower<ArtifactPower>();
        if (artifact != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, artifact, existing + amount - artifact.Amount, owner, null);
        }
    }
}

internal sealed class TestSubjectContaminatedSamplePower : TestSubjectSamplePower
{
    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => Loc(
        "污染样本",
        "本阶段第一次洗牌时，将[blue]1[/blue]张[gold]晕眩[/gold]加入触发玩家的弃牌堆，之后移除此效果。",
        "首次洗牌会加入晕眩。",
        "Contaminated Sample",
        "The first shuffle this phase adds [blue]1[/blue] [gold]Dazed[/gold] to the triggering player's discard pile, then this is removed.",
        "The first shuffle adds Dazed.");

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (Owner.Monster is not TestSubject || !shuffler.IsActiveForHooks)
        {
            return;
        }

        if (Owner.CombatState is not { } combatState)
        {
            return;
        }

        var dazed = combatState.CreateCard<Dazed>(shuffler);
        await CardPileCmd.AddGeneratedCardToCombat(dazed, PileType.Discard, shuffler, CardPilePosition.Bottom);
        await PowerCmd.Remove(this);
    }
}
