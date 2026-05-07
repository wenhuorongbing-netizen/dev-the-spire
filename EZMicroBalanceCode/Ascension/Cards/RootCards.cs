using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.Attributes;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[CustomID(CardId)]
[Pool(typeof(CurseCardPool))]
public sealed class Root : RootFamilyCard
{
    public const string CardId = "EZMB_ROOT";

    public Root()
        : base(2, rootblightLevel: 1, showInCardLibrary: true)
    {
    }
}

[CustomID(CardId)]
[Pool(typeof(CurseCardPool))]
public sealed class DeepRoot : RootFamilyCard
{
    public const string CardId = "EZMB_DEEP_ROOT";

    public DeepRoot()
        : base(3, rootblightLevel: 2, showInCardLibrary: true)
    {
    }
}

[CustomID(CardId)]
[Pool(typeof(CurseCardPool))]
public sealed class RootblightIII : RootFamilyCard
{
    public const string CardId = "EZMB_ROOTBLIGHT_III";

    public RootblightIII()
        : base(4, rootblightLevel: 3, showInCardLibrary: true)
    {
    }
}

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

    public int SproutRound
    {
        get => Math.Max(DefaultSproutRound, AscensionSavedStateFields.RootBudSproutRound[this]);
        set => AscensionSavedStateFields.RootBudSproutRound[this] = Math.Max(DefaultSproutRound, value);
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/card.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword;
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

public abstract class RootFamilyCard : CustomCardModel
{
    private static readonly CardKeyword[] ExhaustKeyword = [CardKeyword.Exhaust];

    protected RootFamilyCard(int cost, int rootblightLevel, bool showInCardLibrary)
        : base(cost, CardType.Curse, CardRarity.Curse, TargetType.None, showInCardLibrary)
    {
        RootblightLevel = rootblightLevel;
    }

    public int RootblightLevel { get; }

    public bool WasPresentAtCombatStart
    {
        get => AscensionSavedStateFields.RootblightWasPresentAtCombatStart[this];
        set => AscensionSavedStateFields.RootblightWasPresentAtCombatStart[this] = value;
    }

    public bool HasSplit
    {
        get => AscensionSavedStateFields.RootblightHasSplit[this];
        set => AscensionSavedStateFields.RootblightHasSplit[this] = value;
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/card.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await RootDeckService.ApplyPlayedRootblightCard(this);
    }
}
