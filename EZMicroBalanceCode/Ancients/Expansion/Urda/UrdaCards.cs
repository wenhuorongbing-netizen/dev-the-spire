using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
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
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
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

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
public sealed class UrdaSeedbed : CustomCardModel
{
    public const string CardId = "EZMB_URDA_SEEDBED";

    private static readonly CardKeyword[] SeedbedKeywords = [CardKeyword.Exhaust];

    public UrdaSeedbed()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/urda_seedling.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/urda_seedling.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => SeedbedKeywords;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        AncientCardHelpers.TemporaryHoverTip(),
        AncientCardHelpers.PlantedHoverTip(),
        HoverTipFactory.FromCard<WitheredHusk>(),
        HoverTipFactory.FromCard<RootBud>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await UrdaBlessingService.SetupSeedbed(choiceContext, Owner, IsUpgraded ? 3 : 2, IsUpgraded, this);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Capacity", IsUpgraded ? 3m : 2m);
        description.Add(
            "ImmediateLine",
            IsUpgraded ? new LocString("cards", "EZMB_URDA_SEEDBED.upgradeLine").GetFormattedText() : string.Empty);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

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
