using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    private static void RegisterCards(ModContentPackBuilder content)
    {
        // Explicit pool registration keeps generated, token, curse, and status
        // cards out of the wrong Core discovery paths.
        content.Card<ColorlessCardPool, MorviArchiveBraveryPage>(FullEntry(MorviArchiveBraveryPage.CardId));
        content.Card<ColorlessCardPool, MorviArchiveBurnPage>(FullEntry(MorviArchiveBurnPage.CardId));
        content.Card<ColorlessCardPool, MorviArchiveDexterityPage>(FullEntry(MorviArchiveDexterityPage.CardId));
        content.Card<ColorlessCardPool, MorviArchiveDiscountPage>(FullEntry(MorviArchiveDiscountPage.CardId));
        content.Card<ColorlessCardPool, MorviArchiveDrawPage>(FullEntry(MorviArchiveDrawPage.CardId));
        content.Card<ColorlessCardPool, MorviArchiveVeilPage>(FullEntry(MorviArchiveVeilPage.CardId));
        content.Card<ColorlessCardPool, MorviRedInkOverdraftCard>(FullEntry(MorviRedInkOverdraftCard.CardId));
        content.Card<StatusCardPool, MorviWastePaper>(FullEntry(MorviWastePaper.CardId));

        content.Card<TokenCardPool, UrdaRainBreath>(FullEntry(UrdaRainBreath.CardId));
        content.Card<ColorlessCardPool, UrdaSeedbed>(FullEntry(UrdaSeedbed.CardId));
        content.Card<ColorlessCardPool, UrdaSeedling>(FullEntry(UrdaSeedling.CardId));
        content.Card<CurseCardPool, WitheredHusk>(FullEntry(WitheredHusk.CardId));

        content.Card<ColorlessCardPool, VakuuKnifeContract>(FullEntry(VakuuKnifeContract.CardId));
        content.Card<ColorlessCardPool, VakuuTemptation>(FullEntry(VakuuTemptation.CardId));
        content.Card<ColorlessCardPool, VakuuShelterContract>(FullEntry(VakuuShelterContract.CardId));
        content.Card<ColorlessCardPool, VakuuTrickContract>(FullEntry(VakuuTrickContract.CardId));
        content.Card<ColorlessCardPool, VakuuCashOutContract>(FullEntry(VakuuCashOutContract.CardId));

        content.Card<StatusCardPool, MarginalNote>(FullEntry(MarginalNote.CardId));
        content.Card<CurseCardPool, RootBud>(FullEntry(RootBud.CardId));
        content.Card<CurseCardPool, Root>(FullEntry(Root.CardId));
        content.Card<CurseCardPool, DeepRoot>(FullEntry(DeepRoot.CardId));
        content.Card<CurseCardPool, RootblightIII>(FullEntry(RootblightIII.CardId));
    }
}
