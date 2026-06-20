
namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviArchiveDiscountPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE";

    public MorviArchiveDiscountPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MorviBlessingService.ArmOverdueLibraryDiscount(Owner, this);
        return Task.CompletedTask;
    }
}
