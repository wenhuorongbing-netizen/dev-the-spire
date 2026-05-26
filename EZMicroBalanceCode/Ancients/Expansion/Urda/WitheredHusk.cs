using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[CustomID(CardId)]
[Pool(typeof(CurseCardPool))]
public sealed class WitheredHusk : CustomCardModel
{
    public const string CardId = "EZMB_WITHERED_HUSK";

    private static readonly CardKeyword[] HuskKeywords = [CardKeyword.Ethereal, CardKeyword.Exhaust];

    public WitheredHusk()
        : base(0, CardType.Curse, CardRarity.Curse, TargetType.Self, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/withered_husk.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/withered_husk.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => HuskKeywords;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        AncientCardHelpers.TemporaryHoverTip()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this || Owner?.Creature == null)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}
