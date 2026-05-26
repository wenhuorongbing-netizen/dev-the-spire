using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

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
        HoverTipFactory.FromCard<RootBud>(),
        HoverTipFactory.FromCard<Root>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await UrdaBlessingService.SetupSeedbed(choiceContext, Owner, IsUpgraded ? 3 : 2, IsUpgraded ? 2 : 1, this);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Capacity", IsUpgraded ? 3m : 2m);
        description.Add("ImmediatePlantCount", IsUpgraded ? 2m : 1m);
        var immediateLine = new LocString("cards", "EZMB_URDA_SEEDBED.immediateLine");
        immediateLine.Add("ImmediatePlantCount", IsUpgraded ? 2m : 1m);
        description.Add("ImmediateLine", immediateLine.GetFormattedText());
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
