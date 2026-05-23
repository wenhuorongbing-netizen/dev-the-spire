using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[CustomID(CardId)]
[Pool(typeof(CurseCardPool))]
public sealed class RootBud : CustomCardModel
{
    public const string CardId = "EZMB_ROOT_BUD";
    public const int DefaultSproutRound = 3;
    public const int BossSecondSproutRound = 4;

    private static readonly CardKeyword[] ExhaustKeyword = [CardKeyword.Exhaust];

    public RootBud()
        : base(2, CardType.Curse, CardRarity.Curse, TargetType.None, showInCardLibrary: false)
    {
    }

    public bool HasEnteredHand
    {
        get => AscensionSavedStateFields.RootBudEnteredHand[this];
        set => AscensionSavedStateFields.RootBudEnteredHand[this] = value;
    }

    public bool WasPlayed
    {
        get => AscensionSavedStateFields.RootBudPlayed[this];
        set => AscensionSavedStateFields.RootBudPlayed[this] = value;
    }

    public bool HasSprouted
    {
        get => AscensionSavedStateFields.RootBudSprouted[this];
        set => AscensionSavedStateFields.RootBudSprouted[this] = value;
    }

    public bool PlantedInSeedbed
    {
        get => AscensionSavedStateFields.RootBudPlantedInSeedbed[this];
        set => AscensionSavedStateFields.RootBudPlantedInSeedbed[this] = value;
    }

    public int SproutRound
    {
        get => Math.Max(DefaultSproutRound, AscensionSavedStateFields.RootBudSproutRound[this]);
        set => AscensionSavedStateFields.RootBudSproutRound[this] = Math.Max(DefaultSproutRound, value);
    }

    public override string CustomPortraitPath => RootPortraitPaths.BigBlightSprout;
    public override string PortraitPath => RootPortraitPaths.BlightSprout;
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        AncientCardHelpers.TemporaryHoverTip(),
        HoverTipFactory.FromCard<Root>()
    ];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        WasPlayed = true;
        ExhaustOnNextPlay = true;
        return Task.CompletedTask;
    }
}
