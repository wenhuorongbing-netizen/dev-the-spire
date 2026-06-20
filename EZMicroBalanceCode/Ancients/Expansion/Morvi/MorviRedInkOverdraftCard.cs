using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviRedInkOverdraftCard : ModCardTemplate
{
    public const string CardId = "EZMB_MORVI_RED_INK_OVERDRAFT";

    private static readonly CardKeyword[] OverdraftKeywords = [CardKeyword.Ethereal, CardKeyword.Exhaust];

    public MorviRedInkOverdraftCard()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/morvi_red_ink_overdraft.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/morvi_red_ink_overdraft.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => OverdraftKeywords;

    protected override bool IsPlayable => MorviBlessingService.CanUseRedInkOverdraft(Owner);

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            AncientCardHelpers.TemporaryHoverTip(),
            HoverTipFactory.FromPower<MorviOverdraftPower>()
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MorviBlessingService.UseRedInkOverdraft(choiceContext, Owner);
    }
}
