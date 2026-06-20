
namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviArchiveDexterityPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE";

    public MorviArchiveDexterityPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MorviDexterityPagePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);
    }
}
