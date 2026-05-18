using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

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

    public bool PlantedInSeedbed
    {
        get => AscensionSavedStateFields.RootblightPlantedInSeedbed[this];
        set => AscensionSavedStateFields.RootblightPlantedInSeedbed[this] = value;
    }

    public override string CustomPortraitPath => RootPortraitPaths.BigRootblight(RootblightLevel);
    public override string PortraitPath => RootPortraitPaths.Rootblight(RootblightLevel);
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => RootblightLevel switch
    {
        1 => [HoverTipFactory.FromCard<DeepRoot>()],
        2 => [HoverTipFactory.FromCard<Root>(), HoverTipFactory.FromCard<RootblightIII>()],
        _ => [HoverTipFactory.FromCard<Root>(), HoverTipFactory.FromCard<DeepRoot>()],
    };

    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await RootDeckService.ApplyPlayedRootblightCard(this);
    }
}
