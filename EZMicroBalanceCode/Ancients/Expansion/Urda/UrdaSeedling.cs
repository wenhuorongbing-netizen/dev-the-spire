using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

public sealed class UrdaSeedling : ModCardTemplate
{
    public const string CardId = "EZMB_URDA_SEEDLING";

    private static readonly CardKeyword[] SeedlingKeywords = [CardKeyword.Exhaust];

    public UrdaSeedling()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/urda_seedling.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/urda_seedling.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => SeedlingKeywords;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        AncientCardHelpers.TemporaryHoverTip(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
