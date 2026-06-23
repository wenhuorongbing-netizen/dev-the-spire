using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib registrations for display-only UI patches: icon getters, hover tips,
/// intent labels, and damage preview numbers.
/// </summary>
internal static partial class SpirePlusRitsuLibPatchRegistry
{
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
