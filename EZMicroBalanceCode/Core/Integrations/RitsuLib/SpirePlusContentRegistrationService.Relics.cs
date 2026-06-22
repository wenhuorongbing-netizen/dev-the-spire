using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    private static void RegisterRelics(ModContentPackBuilder content)
    {
        // Option relics are permanent player-visible markers for selected
        // Ancient rewards, so keep them registered as shared relic models.
        content.Relic<SharedRelicPool, AncientInitialRerollOptionRelic>();

        content.Relic<SharedRelicPool, LothaMirrorRebuttalOptionRelic>();
        content.Relic<SharedRelicPool, LothaMirrorHallEchoOptionRelic>();
        content.Relic<SharedRelicPool, LothaPresumptionOptionRelic>();
        content.Relic<SharedRelicPool, LothaClosedCourtOptionRelic>();
        content.Relic<SharedRelicPool, LothaDeferredVerdictOptionRelic>();
        content.Relic<SharedRelicPool, LothaDeathReprieveOptionRelic>();
        content.Relic<SharedRelicPool, LothaSingleSentenceOptionRelic>();
        content.Relic<SharedRelicPool, LothaPublicEvidenceOptionRelic>();

        content.Relic<SharedRelicPool, MorviForbiddenLoanOptionRelic>();
        content.Relic<SharedRelicPool, MorviMisprintPressOptionRelic>();
        content.Relic<SharedRelicPool, MorviRedInkOverdraftOptionRelic>();
        content.Relic<SharedRelicPool, MorviOverdueLibraryOptionRelic>();
        content.Relic<SharedRelicPool, MorviOpenBookExamOptionRelic>();
        content.Relic<SharedRelicPool, MorviPaperstormOptionRelic>();
        content.Relic<SharedRelicPool, MorviBlueprintProofOptionRelic>();
        content.Relic<SharedRelicPool, MorviDebtSettlementOptionRelic>();

        content.Relic<SharedRelicPool, UrdaRootSightOptionRelic>();
        content.Relic<SharedRelicPool, UrdaSeedBankOptionRelic>();
        content.Relic<SharedRelicPool, UrdaSeedbedOptionRelic>();
        content.Relic<SharedRelicPool, UrdaHumusPactOptionRelic>();
        content.Relic<SharedRelicPool, UrdaMoltingOptionRelic>();
        content.Relic<SharedRelicPool, UrdaMossMapOptionRelic>();
        content.Relic<SharedRelicPool, UrdaTrialBranchOptionRelic>();
        content.Relic<SharedRelicPool, UrdaShallowRootRelicOptionRelic>();
        content.Relic<SharedRelicPool, UrdaEliteRootOptionRelic>();
        content.Relic<SharedRelicPool, UrdaRootedRouteOptionRelic>();
        content.Relic<SharedRelicPool, UrdaAfterRainOptionRelic>();

        content.Relic<SharedRelicPool, VakuuFightOptionRelic>();
        content.Relic<SharedRelicPool, ForgeTokenRelic>();
    }
}
