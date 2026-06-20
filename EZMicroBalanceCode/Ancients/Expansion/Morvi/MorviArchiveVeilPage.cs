
namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviArchiveVeilPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_VEIL_PAGE";

    public MorviArchiveVeilPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(14m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}
