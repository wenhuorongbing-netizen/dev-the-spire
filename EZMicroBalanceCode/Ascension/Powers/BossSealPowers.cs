using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BossSealPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => Amount;

    protected virtual BossSealId? SealId => null;

    private string BossSealIconPath => SealId is { } id
        ? AscensionAssetPaths.GetBossSealIndicator(id)
        : AscensionAssetPaths.BossSealIndicator;

    public override string CustomPackedIconPath => BossSealIconPath;

    public override string CustomBigIconPath => BossSealIconPath;

    public override abstract List<(string, string)>? Localization { get; }

    protected static List<(string, string)> Loc(string zhsTitle, string zhsDescription, string zhsShort, string engTitle, string engDescription, string engShort) =>
        LocManager.Instance.Language == "zhs"
            ? new PowerLoc(zhsTitle, zhsDescription, zhsShort)
            : new PowerLoc(engTitle, engDescription, engShort);
}

internal abstract class BossSealMarkerPower : BossSealPower
{
    public override int DisplayAmount => 0;
}

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

internal sealed class SoulTideBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.SoulTide;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：魂潮",
        "灵魂异鱼进入[gold]无形[/gold]时获得[blue]1[/blue]层[gold]人工制品[/gold]。玩家回合结束时，手牌中每张[gold]呼唤[/gold]使它在下一次玩家回合开始时获得格挡。A19每张[blue]2[/blue]格挡，上限为单人[blue]8[/blue]、2人[blue]12[/blue]、3-4人[blue]16[/blue]；[gold]烙印形态[/gold]每张[blue]3[/blue]格挡，上限为单人[blue]12[/blue]、2人[blue]16[/blue]、3-4人[blue]20[/blue]。",
        "未处理的呼唤会让灵魂异鱼在下一次玩家回合开始时获得格挡。",
        "Dedicated Ability: Soul Tide",
        "When Soul Fysh becomes [gold]Intangible[/gold], it gains [blue]1[/blue] [gold]Artifact[/gold]. At player turn end, each [gold]Beckon[/gold] in hand gives it Block at the next player turn start. A19: [blue]2[/blue] Block each, capped at solo [blue]8[/blue], 2 players [blue]12[/blue], 3-4 players [blue]16[/blue]. [gold]Branded Form[/gold]: [blue]3[/blue] Block each, capped at solo [blue]12[/blue], 2 players [blue]16[/blue], 3-4 players [blue]20[/blue].",
        "Unanswered Beckons give Soul Fysh Block.");
}

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

internal sealed class MarginalNoteBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：旁注",
        "[gold]知识诅咒[/gold]后，每名玩家的弃牌堆加入[blue]1[/blue]张临时[gold]旁注[/gold]。打出旁注会抽[blue]1[/blue]张牌。若回合结束时旁注仍在手牌中，知识恶魔获得[gold]深思[/gold]并消耗旁注。深思会给下一次知识诅咒添加附加代价。[gold]烙印形态[/gold]使深思上限变为[blue]3[/blue]，每回合最多增加[blue]2[/blue]层。",
        "旁注不处理会让下一次知识诅咒更重。",
        "Dedicated Ability: Marginal Note",
        "After [gold]Curse of Knowledge[/gold], add [blue]1[/blue] temporary [gold]Marginal Note[/gold] to each player's discard pile. Playing it draws [blue]1[/blue]. If a Note remains in hand at turn end, Knowledge Demon gains [gold]Deep Thought[/gold] and exhausts it. Deep Thought adds a side cost to the next Knowledge curse. [gold]Branded Form[/gold] raises the cap to [blue]3[/blue]; each turn can add at most [blue]2[/blue].",
        "Unplayed notes make the next Knowledge curse worse.");
}

internal sealed class StruggleBaitBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.StruggleBait;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：逃亡疲劳",
        "无厌沙虫获得[gold]力量[/gold]或推进[gold]沙坑[/gold]时，将[blue]1[/blue]张由首领能力生成的[gold]狂乱逃离[/gold]加入受影响玩家的弃牌堆。全队每打出第[blue]3[/blue]张这类逃离，无厌沙虫获得[gold]活力[/gold]：A19为[blue]2[/blue]点，[gold]烙印形态[/gold]为[blue]3[/blue]点。每个玩家回合最多触发[blue]1[/blue]次。",
        "打出多张首领生成的逃离会让沙虫获得活力。",
        "Dedicated Ability: Escape Fatigue",
        "When The Insatiable gains [gold]Strength[/gold] or advances [gold]Sandpit[/gold], add [blue]1[/blue] ability-made [gold]Frantic Escape[/gold] to the affected player's discard pile. Every [blue]3[/blue] such Escapes played by the team gives The Insatiable [gold]Vigor[/gold]: A19 [blue]2[/blue], [gold]Branded Form[/gold] [blue]3[/blue]. Triggers at most once each player turn.",
        "Ability-made Escapes give The Insatiable Vigor.");
}

internal sealed class ChosenDecreeBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.ChosenDecree;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：御令",
        "女王施加[gold]束缚[/gold]时，其中[blue]1[/blue]张束缚牌获得[gold]御令[/gold]。打出御令牌不会触发额外惩罚。打出非御令束缚牌时，女王获得[blue]1[/blue]层[gold]威仪[/gold]；没有打出束缚牌时，女王获得[blue]1[/blue]层威仪，火炬头获得[blue]1[/blue]点[gold]力量[/gold]。威仪使下一次防御或屏障动作额外获得[blue]8[/blue]格挡。[gold]烙印形态[/gold]使威仪上限变为[blue]3[/blue]。",
        "打出正确的束缚牌可以避开御令惩罚。",
        "Dedicated Ability: Royal Decree",
        "When the Queen applies [gold]Bound[/gold], one Bound card gains [gold]Royal Decree[/gold]. Playing the Decree has no extra penalty. Playing a non-Decree Bound card gives the Queen [blue]1[/blue] [gold]Majesty[/gold]; playing no Bound card gives [blue]1[/blue] Majesty and gives Torch Head [blue]1[/blue] [gold]Strength[/gold]. Majesty adds [blue]8[/blue] Block to the next defense or barrier action. [gold]Branded Form[/gold] raises the Majesty cap to [blue]3[/blue].",
        "Play the correct Bound card to avoid the decree penalty.");
}

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

internal sealed class AeonglassHourglassBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：时砂回流",
        "永世沙漏使用[gold]消退[/gold]后生成时砂。下个玩家回合中，每花费[blue]1[/blue]点能量移除[blue]1[/blue]枚。回合结束时，每剩余[blue]1[/blue]枚时砂，使下一次[gold]加大力度[/gold]额外加入[blue]1[/blue]张[gold]枯萎[/gold]。[gold]烙印形态[/gold]生成[blue]3[/blue]枚时砂；若[gold]眼部激光[/gold]开始时仍有时砂，额外命中[blue]1[/blue]次，每场最多[blue]2[/blue]次。",
        "花费能量清时砂；剩余时砂会增加枯萎。",
        "Dedicated Ability: Time Sand Reflow",
        "After [gold]Ebb[/gold], Aeonglass creates Time Sand. During the next player turn, each energy spent removes [blue]1[/blue]. At turn end, each remaining Time Sand makes the next [gold]Increasing Intensity[/gold] add [blue]1[/blue] extra [gold]Wither[/gold]. [gold]Branded Form[/gold] creates [blue]3[/blue] Time Sand; if [gold]Eye Lasers[/gold] starts while any remain, it hits [blue]1[/blue] extra time, up to [blue]2[/blue] times.",
        "Spend energy to clear Time Sand.");
}

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
