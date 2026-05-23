namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ConstantHealMarkFiremarkPower : FiremarkPower
{
    private const string InterruptDamageVar = "InterruptDamage";

    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkConstantHealIndicator;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new InterruptDamageDynamicVar()];

    public override LocString Description
    {
        get
        {
            var description = base.Description;
            description.Add(InterruptDamageVar, InterruptDamage);
            return description;
        }
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：恒愈",
            "敌方回合结束时，火印宿主恢复[blue]{Amount}[/blue]点[gold]生命[/gold]。若它本轮受到[blue]{InterruptDamage}[/blue]点伤害，本次治疗中断；成功治疗会溢火治疗一名副目标。",
            "受到[blue]{InterruptDamage}[/blue]点伤害会中断下次治疗。")
        : new PowerLoc(
            "Firemark: Constant Heal",
            "At enemy turn end, the Firemark Host heals [blue]{Amount}[/blue] [gold]HP[/gold]. If it took [blue]{InterruptDamage}[/blue] damage this round, that heal is interrupted; a successful heal splashes one secondary target.",
            "Taking [blue]{InterruptDamage}[/blue] damage interrupts its next heal.");

    private int InterruptDamage => Amount switch
    {
        <= 4 => 12,
        <= 8 => 24,
        _ => 48
    };

    private sealed class InterruptDamageDynamicVar : DynamicVar
    {
        public InterruptDamageDynamicVar()
            : base(InterruptDamageVar, 12m)
        {
        }

        protected override decimal GetBaseValueForIConvertible()
        {
            return _owner is ConstantHealMarkFiremarkPower power
                ? power.InterruptDamage
                : BaseValue;
        }
    }
}
