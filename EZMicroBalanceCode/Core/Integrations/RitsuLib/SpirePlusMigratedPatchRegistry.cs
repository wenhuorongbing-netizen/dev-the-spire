using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Owns the RitsuLib ModPatcher migration list so bootstrap code can focus on startup order.
/// </summary>
internal static partial class SpirePlusMigratedPatchRegistry
{
    public static void RegisterAll(ModPatcher patcher)
    {
        // Keep this order stable: retained runtime packets and patch-inventory
        // guards bind the visible patch count to this RitsuLib registration path.
        RegisterCardRelicBehaviorPatches(patcher);
        RegisterCardTextAndPickupPatches(patcher);
        RegisterAncientRewardPatches(patcher);
        RegisterLowRiskRewardHookPatches(patcher);
        RegisterPrismaticGemRewardPatches(patcher);
        RegisterAncientEventUiPatches(patcher);
        RegisterClickedUiPatches(patcher);
        RegisterMapUiPatches(patcher);
        RegisterSereTalonUiPatches(patcher);
        RegisterPreviewUiPatches(patcher);
        RegisterRelicVisualHoverPatches(patcher);
        RegisterRemainingUiPatches(patcher);
        RegisterAscensionSelectionUiPatches(patcher);
        RegisterAscensionIntentUiPatches(patcher);
        RegisterEnemyDamagePolishPatches(patcher);
        RegisterAscensionLocalizationFallbackPatches(patcher);
        RegisterInlineLocalizationPatches(patcher);
        RegisterRitsuLibCompatibilityPatches(patcher);
        RegisterUrdaBehaviorPatches(patcher);
        RegisterVakuuLifecyclePatches(patcher);
        RegisterAscensionBossFlowPatches(patcher);
        RegisterAscensionMapGenerationPatches(patcher);
        RegisterAscensionDiagnosticPatches(patcher);
#if REPLACEMENT_PROTOTYPE_ENABLED
        RegisterSts1ReplacementPrototypePatches(patcher);
#endif
    }
}
