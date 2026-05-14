using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.Attributes;
using Godot;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
public sealed class UrdaSeedling : CustomCardModel
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
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
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

[CustomID(CardId)]
[Pool(typeof(StatusCardPool))]
public sealed class WitheredHusk : CustomCardModel
{
    public const string CardId = "EZMB_WITHERED_HUSK";

    private static readonly CardKeyword[] HuskKeywords = [CardKeyword.Ethereal, CardKeyword.Unplayable];

    public WitheredHusk()
        : base(-1, CardType.Status, CardRarity.Status, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/withered_husk.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/withered_husk.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => HuskKeywords;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        }
    }
}
