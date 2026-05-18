using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BannerPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => 0;

    protected virtual string BannerIconPath => AscensionAssetPaths.BannerRoomIndicator;

    public override string CustomPackedIconPath => BannerIconPath;

    public override string CustomBigIconPath => BannerIconPath;

    public override abstract List<(string, string)>? Localization { get; }
}

internal sealed class VanguardBannerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerVanguardIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：先锋",
            "开场获得[blue]{Amount}[/blue]点[gold]力量[/gold]。第[blue]3[/blue]回合开始时失去这些力量。",
            "第[blue]3[/blue]回合开始时失去这些[gold]力量[/gold]。")
        : new PowerLoc(
            "Banner: Vanguard",
            "Starts with [blue]{Amount}[/blue] [gold]Strength[/gold]. At the start of round [blue]3[/blue], loses that Strength.",
            "Loses this [gold]Strength[/gold] at the start of round [blue]3[/blue].");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
}

internal sealed class ShieldwallBannerbearerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerShieldFormationIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：盾阵",
            "旗手存活时，其他敌人每回合获得[gold]格挡[/gold]。旗手死亡时，其他敌人立刻获得一次[gold]格挡[/gold]。",
            "保护其他敌人。")
        : new PowerLoc(
            "Banner: Shieldwall",
            "While this bannerbearer is alive, other enemies gain [gold]Block[/gold] each round. When it dies, other enemies immediately gain [gold]Block[/gold].",
            "Protects other enemies.");
}

internal sealed class BloodPrizeBannerTargetPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerBountyIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：血赏",
            "第[blue]3[/blue]回合结束前击杀这名敌人，战斗后获得额外[gold]金币[/gold]。若它存活，它会获得[gold]反扑[/gold]。",
            "快速击杀可获得额外[gold]金币[/gold]。")
        : new PowerLoc(
            "Banner: Blood Prize",
            "Kill this enemy before round [blue]3[/blue] ends to gain extra [gold]Gold[/gold] after combat. If it survives, it gains retaliation.",
            "Kill quickly for extra [gold]Gold[/gold].");
}

internal sealed class BloodPrizeRetaliationPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerBountyIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "血赏反扑",
            "血赏目标存活。它获得[gold]力量[/gold]和[gold]人工制品[/gold]。",
            "血赏失败后的强化。")
        : new PowerLoc(
            "Blood Prize Retaliation",
            "The bounty target survived. It gained [gold]Strength[/gold] and [gold]Artifact[/gold].",
            "The failed bounty's retaliation.");
}

internal sealed class PressingLineStrikePower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerPressingLineIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：压阵",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害，然后移除此效果。",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害。")
        : new PowerLoc(
            "Banner: Pressing Line",
            "The next attack deals [blue]{Amount}[/blue] extra damage, then this is removed.",
            "The next attack deals [blue]{Amount}[/blue] extra damage.");

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount
            : 0m;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class LastStandBannerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerLastStandIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：残阵",
            "获得[blue]{Amount}[/blue]点临时[gold]力量[/gold]。敌方回合结束时失去这些力量。",
            "敌方回合结束时失去这些[gold]力量[/gold]。")
        : new PowerLoc(
            "Banner: Last Stand",
            "Gains [blue]{Amount}[/blue] temporary [gold]Strength[/gold]. Loses it at the end of the enemy turn.",
            "Loses this [gold]Strength[/gold] at the end of the enemy turn.");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && Owner.IsAlive)
        {
            Flash();
            await AscensionPowerAmountHelper.RemoveTemporaryStrength(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}
