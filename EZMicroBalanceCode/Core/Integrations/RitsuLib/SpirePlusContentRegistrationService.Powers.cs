using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    private static void RegisterPowers(ModContentPackBuilder content)
    {
        content.Power<LothaVerdictPower>();
        content.Power<LothaPresumptionPower>();
        content.Power<LothaDeathReprievePower>();
        content.Power<LothaSingleSentencePower>();
        content.Power<LothaEnlightenmentPower>();

        content.Power<MorviDebtPower>();
        content.Power<MorviProofreadPower>();
        content.Power<MorviOpenBookPower>();
        content.Power<MorviOverdraftPower>();
        content.Power<MorviPaperstormPower>();
        content.Power<MorviBraveryPagePower>();
        content.Power<MorviDexterityPagePower>();

        content.Power<VakuuStolenVaultPower>();
        content.Power<VakuuBloodDebtPower>();
        content.Power<VakuuBacklashPower>();

        content.Power<AeonglassHourglassPower>();
        content.Power<AeonglassLaserEchoPower>();
        content.Power<AeonglassPendingWitherPower>();
        content.Power<AeonglassLaserEchoUseCounterPower>();
        content.Power<BloodPrizeBannerTargetPower>();
        content.Power<BloodPrizeRetaliationPower>();
        content.Power<LastStandBannerPower>();
        content.Power<PressingLineStrikePower>();
        content.Power<ShieldwallBannerbearerPower>();
        content.Power<VanguardBannerPower>();
        content.Power<BoilingCriticalPower>();
        content.Power<AeonglassHourglassBossSealMarkerPower>();
        content.Power<BoilingCriticalBossSealMarkerPower>();
        content.Power<ChosenDecreeBossSealMarkerPower>();
        content.Power<HolyDazeBossSealMarkerPower>();
        content.Power<InkReturnBossSealMarkerPower>();
        content.Power<MarginalNoteBossSealMarkerPower>();
        content.Power<MartyrOathBossSealMarkerPower>();
        content.Power<MisalignedShellBossSealMarkerPower>();
        content.Power<ResidualSampleBossSealMarkerPower>();
        content.Power<SoulTideBossSealMarkerPower>();
        content.Power<StartledShellBossSealMarkerPower>();
        content.Power<StruggleBaitBossSealMarkerPower>();
        content.Power<RoyalMajestyPower>();
        content.Power<ConstantHealMarkFiremarkPower>();
        content.Power<ForgeArmorMarkFiremarkPower>();
        content.Power<GiantMarkFiremarkPower>();
        content.Power<FiremarkHeatPower>();
        content.Power<FiremarkHeatStrikePower>();
        content.Power<MightMarkFiremarkPower>();
        content.Power<FiremarkMightOverflowPower>();
        content.Power<MoltenCoreFiremarkPower>();
        content.Power<HolyDazePower>();
        content.Power<DeepThoughtPower>();
        content.Power<DeepThoughtCostTaxPower>();
        content.Power<MartyrOathPower>();
        content.Power<MartyrOathStrikePower>();
        content.Power<KaiserCalibrationPower>();
        content.Power<KaiserCalibrationStrikePower>();
        content.Power<ResidualSamplePower>();
        content.Power<TestSubjectSkillAdaptationPower>();
        content.Power<TestSubjectAttackAdaptationPower>();
        content.Power<TestSubjectAntibodySamplePower>();
        content.Power<TestSubjectContaminatedSamplePower>();
    }
}
