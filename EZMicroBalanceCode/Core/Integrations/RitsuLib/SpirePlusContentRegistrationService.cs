using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static class SpirePlusContentRegistrationService
{
    public static void Register(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[Spire Plus] RitsuLib not active; skipping Spire Plus content registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[Spire Plus] Registering native content with RitsuLib...");

        // Keep all RitsuLib content registration in one service so future model
        // additions use the same CreateContentPack path and do not drift back
        // into manifest-only or ad hoc registration.
        var content = RitsuLibFramework.CreateContentPack(modId);

        RegisterAncients(content);
        RegisterVakuuEncounter(content);
        RegisterCards(content);
        RegisterRelics(content);
        RegisterPowers(content);
        RegisterEnchantments(content);

        content.Apply();

        SpirePlusInlineLocalizationRegistry.RegisterKnownProviders();

        logger.Info("[Spire Plus] Native content registered successfully.");
    }

    private static void RegisterAncients(ModContentPackBuilder content)
    {
        content.SharedAncient<EzmbUrda>();
        content.SharedAncient<EzmbMorvi>();
        content.SharedAncient<EzmbLotha>();
    }

    private static void RegisterVakuuEncounter(ModContentPackBuilder content)
    {
        content.Monster<EzmbVakuuTrialMonster>();
        content.GlobalEncounter<EzmbVakuuTrialEncounter>();
    }

    private static void RegisterCards(ModContentPackBuilder content)
    {
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

    private static void RegisterRelics(ModContentPackBuilder content)
    {
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

    private static void RegisterEnchantments(ModContentPackBuilder content)
    {
        content.Enchantment<JeweledMaskFreePower>();
        content.Enchantment<UrdaTrialBranchEnchantment>();
        content.Enchantment<FissionEnchantment>();
        content.Enchantment<RoyalDecreeEnchantment>();
    }

    // Public-entry strings already include the Spire Plus namespace. Passing
    // them through RitsuLib preserves the public id used by localization,
    // saves, screenshots, and package evidence.
    private static ModelPublicEntryOptions FullEntry(string publicEntry) =>
        ModelPublicEntryOptions.FromFullPublicEntry(publicEntry);
}
