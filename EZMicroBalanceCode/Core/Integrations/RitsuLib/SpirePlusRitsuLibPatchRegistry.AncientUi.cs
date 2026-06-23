using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib registrations for Ancient event-option and Ancient marker UI patches.
/// </summary>
internal static partial class SpirePlusRitsuLibPatchRegistry
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

    private static void RegisterSereTalonUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SereTalonAncientEventOptionButtonPatch>();
        patcher.RegisterPatch<SereTalonRelicNodeReloadPatch>();
    }
}
