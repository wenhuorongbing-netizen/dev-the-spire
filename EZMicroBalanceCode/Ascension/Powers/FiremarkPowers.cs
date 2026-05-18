using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class FiremarkPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => Amount;

    protected virtual string FiremarkIconPath => AscensionAssetPaths.FiremarkedEliteIndicator;

    public override string CustomPackedIconPath => FiremarkIconPath;

    public override string CustomBigIconPath => FiremarkIconPath;

    public override abstract List<(string, string)>? Localization { get; }
}

internal sealed class MightMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：烈力",
            "[gold]火印精英[/gold]开场获得[blue]{Amount}[/blue]点[gold]力量[/gold]。造成未被格挡的攻击伤害后获得[gold]热势[/gold]。",
            "造成未被格挡的攻击伤害后获得[gold]热势[/gold]。")
        : new PowerLoc(
            "Firemark: Might",
            "The [gold]Firemarked enemy[/gold] starts with [blue]{Amount}[/blue] [gold]Strength[/gold]. Unblocked attack damage builds Heat.",
            "Unblocked attack damage builds Heat.");
}

internal sealed class GiantMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkGiantIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：巨躯",
            "[gold]火印精英[/gold]开场最大生命提高[blue]{Amount}%[/blue]。半血时暴露[gold]熔核[/gold]。",
            "半血时暴露[gold]熔核[/gold]。")
        : new PowerLoc(
            "Firemark: Giant",
            "The [gold]Firemarked enemy[/gold] starts with +[blue]{Amount}%[/blue] [gold]Max HP[/gold]. At half HP, it exposes a Molten Core.",
            "Exposes a Molten Core at half HP.");
}

internal sealed class ForgeArmorMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkForgeArmorIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：铸甲",
            "[gold]火印精英[/gold]在每个敌方回合后获得[blue]{Amount}[/blue]点[gold]熔甲[/gold]。若下个玩家回合击碎熔甲，下次熔甲不会生成。",
            "击碎熔甲后，下次熔甲不会生成。")
        : new PowerLoc(
            "Firemark: Forge Armor",
            "After each enemy turn, the [gold]Firemarked enemy[/gold] gains [blue]{Amount}[/blue] [gold]Molten Armor[/gold]. If that armor is broken next turn, it skips the next armor gain.",
            "Broken Molten Armor skips the next armor gain.");
}

internal sealed class ConstantHealMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkConstantHealIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：恒愈",
            "每个敌方回合结束时，[gold]火印精英[/gold]恢复[blue]{Amount}[/blue]点[gold]生命[/gold]。单回合造成足够伤害可中断本回合治疗。",
            "单回合造成足够伤害可中断治疗。")
        : new PowerLoc(
            "Firemark: Constant Heal",
            "At the end of each enemy turn, the [gold]Firemarked enemy[/gold] heals [blue]{Amount}[/blue] [gold]HP[/gold]. Enough damage in one player turn interrupts that heal.",
            "Enough damage in one player turn interrupts this heal.");
}

internal sealed class FiremarkHeatPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "热势",
            "达到[blue]2[/blue]层后，火印精英的下一次攻击更危险。",
            "达到[blue]2[/blue]层后强化下一次攻击。")
        : new PowerLoc(
            "Heat",
            "At [blue]2[/blue] Heat, the Firemarked enemy's next attack becomes more dangerous.",
            "At [blue]2[/blue] Heat, the next attack is stronger.");
}

internal sealed class FiremarkHeatStrikePower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "热势爆发",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害，然后移除此效果。",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害。")
        : new PowerLoc(
            "Heat Burst",
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

internal sealed class MoltenCoreFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkGiantIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "熔核暴露",
            "本回合对火印精英造成足够伤害，可使其失去[blue]10%[/blue][gold]最大生命[/gold]。失败时，它获得[blue]1[/blue]层[gold]人工制品[/gold]。",
            "打破熔核可削弱最大生命。")
        : new PowerLoc(
            "Molten Core",
            "Deal enough damage to the Firemarked enemy this turn to make it lose [blue]10%[/blue] [gold]Max HP[/gold]. If you fail, it gains [blue]1[/blue] [gold]Artifact[/gold].",
            "Break the core to reduce Max HP.");
}
