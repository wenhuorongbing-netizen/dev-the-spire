using BaseLib.Utils.Attributes;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveDrawPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DRAW_PAGE";

    public MorviArchiveDrawPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}
