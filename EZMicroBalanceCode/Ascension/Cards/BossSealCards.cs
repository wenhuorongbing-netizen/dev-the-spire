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
[Pool(typeof(StatusCardPool))]
public sealed class MarginalNote : CustomCardModel
{
    public const string CardId = "EZMB_MARGINAL_NOTE";

    private static readonly CardKeyword[] MarginalNoteKeywords = [CardKeyword.Retain, CardKeyword.Exhaust];

    public MarginalNote()
        : base(1, CardType.Skill, CardRarity.Status, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/morvi_archive_pages.png";
    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/morvi_archive_pages.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => MarginalNoteKeywords;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [AncientCardHelpers.TemporaryHoverTip()];
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ExhaustOnNextPlay = true;
        await CardPileCmd.Draw(choiceContext, 1m, Owner);
    }
}
