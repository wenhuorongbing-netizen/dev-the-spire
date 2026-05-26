using BaseLib.Utils.Attributes;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveBraveryPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE";

    public MorviArchiveBraveryPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MorviBraveryPagePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Strength.BaseValue,
            Owner.Creature,
            this);
    }
}
