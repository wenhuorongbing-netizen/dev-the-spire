using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MartyrOathPower : BossSealPower
{
    private bool _debuffConsumed;

    protected override BossSealId? SealId => BossSealId.MartyrOath;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "殉誓",
        "亲族祭司施加的下一次[gold]负面状态[/gold]持续时间+[blue]{Amount}[/blue]，之后移除此效果。若祭司先攻击，殉誓会改为强化那次攻击。",
        "下一次负面状态持续更久。",
        "Martyr Oath",
        "The next [gold]debuff[/gold] applied by Kin Priest lasts [blue]{Amount}[/blue] longer, then this is removed. If the Priest attacks first, the Oath empowers that attack instead.",
        "The next debuff lasts longer.");

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        return giver == Owner &&
            target?.IsPlayer == true &&
            amount > 0m &&
            power.GetTypeForAmount(amount) == PowerType.Debuff
                ? amount + Amount
                : amount;
    }

    public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        _debuffConsumed = true;
        await PowerCmd.Remove(Owner.GetPower<MartyrOathStrikePower>());
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (_debuffConsumed && participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class MartyrOathStrikePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MartyrOath;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "殉誓攻击",
        "下一次攻击每次命中额外造成[blue]{Amount}[/blue]点伤害，之后移除此效果。",
        "下一次攻击每次命中额外造成伤害。",
        "Martyr Strike",
        "Each hit of the next attack deals [blue]{Amount}[/blue] extra damage, then this is removed.",
        "Each hit of the next attack deals extra damage.");

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount
            : 0m;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack())
        {
            await PowerCmd.Remove(Owner.GetPower<MartyrOathPower>());
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class KaiserCalibrationPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MisalignedShell;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "校准",
        "达到[blue]2[/blue]层时，下一次攻击每次命中会额外造成伤害。每只爪每场战斗最多触发[blue]1[/blue]次。",
        "正在校准下一次攻击的命中。",
        "Calibration",
        "At [blue]2[/blue] stacks, each hit of the next attack deals extra damage. Each claw can trigger this once per combat.",
        "Calibrating the next attack's hits.");
}

internal sealed class KaiserCalibrationStrikePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MisalignedShell;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "校准攻击",
        "下一次攻击每次命中额外造成[blue]{Amount}[/blue]点伤害，之后移除此效果。",
        "下一次攻击每次命中额外造成伤害。",
        "Calibrated Strike",
        "Each hit of the next attack deals [blue]{Amount}[/blue] extra damage, then this is removed.",
        "Each hit of the next attack deals extra damage.");

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount
            : 0m;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack())
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class DeepThoughtPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "深思",
        "下一次知识诅咒会附带额外代价。每层：崩解触发时额外造成[blue]1[/blue]点伤害；心智腐烂洗入[blue]1[/blue]张[gold]晕眩[/gold]；懒惰使本回合下一张牌费用+[blue]1[/blue]；枯竭使本回合失去[blue]1[/blue]点能量。[gold]烙印形态[/gold]下，懒惰和枯竭的附加代价每次知识诅咒最多结算一次。",
        "下一次知识诅咒更重。",
        "Deep Thought",
        "The next Knowledge curse adds a side cost. Per stack: Disintegration deals +[blue]1[/blue] when it triggers; Mind Rot shuffles [blue]1[/blue] [gold]Dazed[/gold] into discard. Sloth makes your next card this turn cost +[blue]1[/blue]; Waste Away makes you lose [blue]1[/blue] [gold]Energy[/gold] this turn. In [gold]Branded Form[/gold], Sloth and Waste Away side costs resolve at most once per Knowledge curse.",
        "The next Knowledge curse is worse.");

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (!target.IsPlayer || amount <= 0m)
        {
            return false;
        }

        modifiedAmount = canonicalPower switch
        {
            DisintegrationPower => amount + Amount,
            _ => amount
        };

        return modifiedAmount != amount;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m ||
            power.Owner?.Player is not { } player ||
            applier?.Monster is not KnowledgeDemon)
        {
            return;
        }

        if (power is MindRotPower)
        {
            var combatState = player.Creature.CombatState;
            if (combatState == null)
            {
                return;
            }

            for (var i = 0; i < Amount; i++)
            {
                var dazed = combatState.CreateCard<Dazed>(player);
                await CardPileCmd.AddGeneratedCardToCombat(dazed, PileType.Discard, player, CardPilePosition.Bottom);
            }
        }
        else if (power is SlothPower)
        {
            var sideCostLayers = GetSideCostLayers(player);
            if (sideCostLayers > 0m)
            {
                await PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, sideCostLayers, applier, cardSource);
            }
        }
        else if (power is WasteAwayPower)
        {
            var sideCostLayers = GetSideCostLayers(player);
            if (sideCostLayers > 0m)
            {
                await PlayerCmd.LoseEnergy(sideCostLayers, player);
            }
        }
    }

    private decimal GetSideCostLayers(Player player)
    {
        var layers = Math.Max(0m, Amount);
        var metadata = AscensionMapService.TryGetCurrentMetadata(player.RunState);
        return metadata is { IsBossBrand: true, BossSeal.Id: BossSealId.MarginalNote }
            ? Math.Min(layers, 1m)
            : layers;
    }
}

internal sealed class DeepThoughtCostTaxPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "深思代价",
        "本回合下一张牌费用+[blue]{Amount}[/blue]，之后移除此效果。",
        "下一张牌费用提高。",
        "Deep Thought Cost",
        "The next card you play this turn costs +[blue]{Amount}[/blue], then this is removed.",
        "The next card costs more.");

    public override PowerType Type => PowerType.Debuff;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner?.Creature != Owner || originalCost < 0m)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = originalCost + Amount;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class RoyalMajestyPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.ChosenDecree;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "威仪",
        "下一次防御或屏障动作每层额外获得[blue]8[/blue]点格挡。[gold]烙印形态[/gold]最多一次消耗[blue]2[/blue]层。",
        "下一次防御获得更多格挡。",
        "Majesty",
        "The next defense or barrier action gains [blue]8[/blue] extra Block per stack. [gold]Branded Form[/gold] can spend at most [blue]2[/blue] stacks at once.",
        "The next defense gains more Block.");

    private int LayersToSpend => Math.Min(Amount, 2);

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return target == Owner && props.HasFlag(ValueProp.Move)
            ? LayersToSpend * 8m
            : 0m;
    }

    public override async Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -LayersToSpend, Owner, cardSource);
    }
}

internal sealed class AeonglassLaserEchoPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "时砂回流",
        "下一次眼部激光额外命中[blue]1[/blue]次，之后移除此效果。",
        "下一次眼部激光额外命中。",
        "Time Sand Reflow",
        "The next Eye Lasers hit [blue]1[/blue] extra time, then this is removed.",
        "The next Eye Lasers hit one extra time.");

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        return attack.Attacker == Owner && Owner.Monster is Aeonglass
            ? hitCount + 1
            : hitCount;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class AeonglassPendingWitherPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "待回流枯萎",
        "下一次[gold]加大力度[/gold]额外加入[blue]{Amount}[/blue]张[gold]枯萎[/gold]，之后移除此效果。",
        "下一次加大力度加入更多枯萎。",
        "Pending Wither",
        "The next [gold]Increasing Intensity[/gold] adds [blue]{Amount}[/blue] extra [gold]Wither[/gold], then this is removed.",
        "The next Increasing Intensity adds extra Wither.");
}

internal sealed class AeonglassLaserEchoUseCounterPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    protected override bool IsVisibleInternal => false;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "时砂激光计数",
        "本场战斗已经触发的时砂激光次数。",
        "隐藏计数。",
        "Time Sand Laser Count",
        "Hidden counter for Time Sand's extra Eye Lasers this combat.",
        "Hidden counter.");
}

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
