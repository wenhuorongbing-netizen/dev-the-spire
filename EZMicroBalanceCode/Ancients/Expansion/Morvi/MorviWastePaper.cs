using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviWastePaper : ModCardTemplate
{
    public const string CardId = "EZMB_MORVI_WASTE_PAPER";

    private static readonly CardKeyword[] WastePaperKeywords = [CardKeyword.Ethereal, CardKeyword.Unplayable];

    public MorviWastePaper()
        : base(-1, CardType.Status, CardRarity.Status, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/morvi_waste_paper.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/morvi_waste_paper.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => WastePaperKeywords;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
            AncientCardHelpers.TemporaryHoverTip()
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;
}
