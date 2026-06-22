using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Ascension.Events;
using EZMicroBalance.EZMicroBalanceCode.Core.Localization;
using EZMicroBalance.EZMicroBalanceCode.Map;
using EZMicroBalance.EZMicroBalanceCode.Modding;
using EZMicroBalance.EZMicroBalanceCode.Preview;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib ModPatcher registrations for click, hover, and other UI-facing patches.
/// Keeping these groups separate makes the completed clicked-UI migration easy to audit.
/// </summary>
internal static partial class SpirePlusMigratedPatchRegistry
{
    private static void RegisterAncientEventUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<NeowInitialOptionRerollPatch>();
        patcher.RegisterPatch<UrdaOvergrowthPatch>();
        patcher.RegisterPatch<UrdaUnderdocksPatch>();
        patcher.RegisterPatch<UrdaOptionRelicClickPatch>();
        patcher.RegisterPatch<MorviHivePatch>();
        patcher.RegisterPatch<LothaGloryPatch>();
        patcher.RegisterPatch<VakuuForceAncientPatch>();
        patcher.RegisterPatch<VakuuFightOptionPatch>();
        patcher.RegisterPatch<VakuuFightCommandForceCleanupPatch>();
        patcher.RegisterPatch<VakuuFightResumePatch>();
        patcher.RegisterPatch<VakuuFightPreFinishedParentRestoreHealPatch>();
    }

    private static void RegisterClickedUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<UrdaRootSightMapQuestIconInputPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPreviewIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapQuestIconPatch>();
        patcher.RegisterPatch<UrdaRootSightMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightDisabledMapPointClickPatch>();
        patcher.RegisterPatch<UrdaRootSightMapClosePatch>();
    }

    private static void RegisterMapUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SpirePlusMapPointHoverComposer>();
        patcher.RegisterPatch<FiremarkedEliteMapIconPatch>();
        patcher.RegisterPatch<BossMapPointHoverPatch>();
    }

    private static void RegisterSereTalonUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SereTalonAncientEventOptionButtonPatch>();
        patcher.RegisterPatch<SereTalonRelicNodeReloadPatch>();
    }

    private static void RegisterPreviewUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<CrystalSpherePeekPatch>();
        patcher.RegisterPatch<CrystalSpherePeekFinishedPatch>();
        patcher.RegisterPatch<TransformPreviewInitializePatch>();
        patcher.RegisterPatch<TransformPreviewCyclePatch>();
        patcher.RegisterPatch<TransformPredictionAromaOfChaosRngPatch>();
        patcher.RegisterPatch<TransformPredictionEndlessConveyorRngPatch>();
        patcher.RegisterPatch<TransformPredictionSymbioteRngPatch>();
        patcher.RegisterPatch<TransformPredictionWhisperingHollowRngPatch>();
        patcher.RegisterPatch<TransformPredictionMorphicGroveNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionTrialNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionNewLeafNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionAstrolabeNicheRngPatch>();
        patcher.RegisterPatch<TransformPredictionSelectionLifetimePatch>();
    }

    private static void RegisterRelicVisualHoverPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SereTalonIconPathPatch>();
        patcher.RegisterPatch<SereTalonPackedIconPathPatch>();
        patcher.RegisterPatch<SereTalonPackedIconOutlinePathPatch>();
        patcher.RegisterPatch<SereTalonBigIconPathPatch>();
        patcher.RegisterPatch<SereTalonIconTexturePatch>();
        patcher.RegisterPatch<SereTalonIconOutlineTexturePatch>();
        patcher.RegisterPatch<SereTalonBigIconTexturePatch>();
        patcher.RegisterPatch<PrismaticGemHoverTipsPatch>();
        patcher.RegisterPatch<PrismaticGemHoverTipsExcludingRelicPatch>();
        patcher.RegisterPatch<JewelryBoxExtraHoverTipsPatch>();
        patcher.RegisterPatch<JewelryBoxHoverTipsPatch>();
        patcher.RegisterPatch<JewelryBoxHoverTipsExcludingRelicPatch>();
        patcher.RegisterPatch<SovereignBladeJadeBoonsHoverTipsPatch>();
    }

    private static void RegisterRemainingUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<PrismaticGemRewardScreenHintPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenReadyPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenStatePatch>();
        patcher.RegisterPatch<AscensionA20CourtyardProceedPatch>();
        patcher.RegisterPatch<AscensionA20CourtyardPortraitPatch>();
        patcher.RegisterPatch<ModInfoLocalizationPatches>();
        patcher.RegisterPatch<CombatHandInputSafetyPatch>();
        patcher.RegisterPatch<MeatCleaverCookIsEnabledPatch>();
        patcher.RegisterPatch<MeatCleaverCookDescriptionPatch>();
        patcher.RegisterPatch<MeatCleaverCookPatch>();
    }

    private static void RegisterAscensionSelectionUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<StartRunLobbySetSingleplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunLocallyPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdateMaxMultiplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdatePreferredAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbySyncAscensionChangeA20WarningPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunForAllPlayersA20WarningPatch>();
    }

    private static void RegisterAscensionIntentUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AeonglassLaserEchoIntentLabelPatch>();
        patcher.RegisterPatch<AeonglassLaserEchoIntentDamagePatch>();
    }

    private static void RegisterEnemyDamagePolishPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<DecimillipedeWritheDamagePolishPatch>();
        patcher.RegisterPatch<DecimillipedeConstrictDamagePolishPatch>();
        patcher.RegisterPatch<DecimillipedeBulkDamagePolishPatch>();
        patcher.RegisterPatch<TerrorEelCrashDamagePolishPatch>();
        patcher.RegisterPatch<TerrorEelThrashDamagePolishPatch>();
        patcher.RegisterPatch<PhantasmalGardenerBiteDamagePolishPatch>();
        patcher.RegisterPatch<PhantasmalGardenerLashDamagePolishPatch>();
    }
}
