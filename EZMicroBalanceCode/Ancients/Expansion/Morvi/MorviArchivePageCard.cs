using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal abstract class MorviArchivePageCard : ModCardTemplate
{
    private static readonly CardKeyword[] ArchivePageKeywords = [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected MorviArchivePageCard(CardType type, TargetType targetType)
        : base(0, type, CardRarity.Token, targetType, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/morvi_archive_pages.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/morvi_archive_pages.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => ArchivePageKeywords;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            AncientCardHelpers.TemporaryHoverTip()
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;
}
