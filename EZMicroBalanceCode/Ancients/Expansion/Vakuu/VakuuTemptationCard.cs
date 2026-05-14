using BaseLib.Utils.Attributes;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

[CustomID(CardId)]
[Pool(typeof(StatusCardPool))]
internal sealed class VakuuTemptation : CustomCardModel
{
    public const string CardId = "EZMB_VAKUU_TEMPTATION";

    private static readonly CardKeyword[] TemptationKeywords =
        [CardKeyword.Ethereal, CardKeyword.Unplayable];

    public VakuuTemptation()
        : base(-1, CardType.Status, CardRarity.Status, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/vakuu_temptation.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/vakuu_temptation.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => TemptationKeywords;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Energy", 1m), new IntVar("HpLoss", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
            HoverTipFactory.Static(StaticHoverTip.Energy)
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal)
    {
        if (card != this)
        {
            return;
        }

        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Vakuu Temptation exhausted: gained 1 Energy and lost 3 HP.");
    }
}
