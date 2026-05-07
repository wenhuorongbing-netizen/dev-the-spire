using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class FiremarkPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => AscensionAssetPaths.FiremarkedEliteIndicator;

    public override string CustomBigIconPath => AscensionAssetPaths.FiremarkedEliteIndicator;

    public override abstract List<(string, string)>? Localization { get; }
}

internal sealed class MightMarkFiremarkPower : FiremarkPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "\u706b\u5370\uff1a\u70c8\u529b",
            "\u706b\u5370\u5bbf\u4e3b\u5f00\u5c40\u83b7\u5f97[blue]{Amount}[/blue]\u70b9[gold]\u529b\u91cf[/gold]\u3002",
            "\u5f00\u5c40\u83b7\u5f97[blue]{Amount}[/blue]\u70b9[gold]\u529b\u91cf[/gold]\u3002")
        : new PowerLoc(
            "Firemark: Might",
            "The [gold]Firemark Host[/gold] starts with [blue]{Amount}[/blue] [gold]Strength[/gold].",
            "Starts with [blue]{Amount}[/blue] [gold]Strength[/gold].");
}

internal sealed class GiantMarkFiremarkPower : FiremarkPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "\u706b\u5370\uff1a\u5de8\u8eaf",
            "\u706b\u5370\u5bbf\u4e3b\u5f00\u5c40[gold]\u6700\u5927\u751f\u547d[/gold]\u63d0\u9ad8[blue]{Amount}%[/blue]\u3002",
            "\u5f00\u5c40[gold]\u6700\u5927\u751f\u547d[/gold]\u63d0\u9ad8[blue]{Amount}%[/blue]\u3002")
        : new PowerLoc(
            "Firemark: Giant",
            "The [gold]Firemark Host[/gold] starts with +[blue]{Amount}%[/blue] [gold]Max HP[/gold].",
            "Starts with +[blue]{Amount}%[/blue] [gold]Max HP[/gold].");
}

internal sealed class ForgeArmorMarkFiremarkPower : FiremarkPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "\u706b\u5370\uff1a\u94f8\u7532",
            "\u706b\u5370\u5bbf\u4e3b\u5728\u81ea\u5df1\u7684\u56de\u5408\u7ed3\u675f\u65f6\u83b7\u5f97[blue]{Amount}[/blue]\u70b9[gold]\u683c\u6321[/gold]\u3002",
            "\u56de\u5408\u7ed3\u675f\u65f6\u83b7\u5f97[blue]{Amount}[/blue]\u70b9[gold]\u683c\u6321[/gold]\u3002")
        : new PowerLoc(
            "Firemark: Forge Armor",
            "At end of its turn, the [gold]Firemark Host[/gold] gains [blue]{Amount}[/blue] [gold]Block[/gold].",
            "At end of turn, gains [blue]{Amount}[/blue] [gold]Block[/gold].");

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && Owner.IsAlive)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null, fast: true);
        }
    }
}

internal sealed class ConstantHealMarkFiremarkPower : FiremarkPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "\u706b\u5370\uff1a\u6052\u6108",
            "\u706b\u5370\u5bbf\u4e3b\u5728\u81ea\u5df1\u7684\u56de\u5408\u7ed3\u675f\u65f6\u56de\u590d[blue]{Amount}[/blue]\u70b9[gold]\u751f\u547d[/gold]\u3002",
            "\u56de\u5408\u7ed3\u675f\u65f6\u56de\u590d[blue]{Amount}[/blue]\u70b9[gold]\u751f\u547d[/gold]\u3002")
        : new PowerLoc(
            "Firemark: Constant Heal",
            "At end of its turn, the [gold]Firemark Host[/gold] heals [blue]{Amount}[/blue] [gold]HP[/gold].",
            "At end of turn, heals [blue]{Amount}[/blue] [gold]HP[/gold].");

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && Owner.IsAlive)
        {
            Flash();
            await CreatureCmd.Heal(Owner, Amount);
        }
    }
}
