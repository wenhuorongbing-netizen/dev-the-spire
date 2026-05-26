using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[CustomID(CardId)]
[Pool(typeof(TokenCardPool))]
public sealed class UrdaRainBreath : CustomCardModel
{
    public const string CardId = "EZMB_URDA_RAIN_BREATH";

    private static readonly CardKeyword[] RainBreathKeywords = [CardKeyword.Exhaust];

    public UrdaRainBreath()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/urda_seedling.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/urda_seedling.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => RainBreathKeywords;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        AncientCardHelpers.TemporaryHoverTip(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new CardsVar(1)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}
