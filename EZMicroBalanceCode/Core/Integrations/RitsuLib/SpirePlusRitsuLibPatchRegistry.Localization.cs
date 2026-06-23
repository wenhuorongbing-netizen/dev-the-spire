using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Core.Localization;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Localization and RitsuLib compatibility registrations.
/// </summary>
internal static partial class SpirePlusRitsuLibPatchRegistry
{
    private static void RegisterAscensionLocalizationFallbackPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AscensionLocalizationLocStringRawTextPatch>();
        patcher.RegisterPatch<AscensionLocalizationGetTablePatch>();
        patcher.RegisterPatch<AscensionLocalizationRawTextPatch>();
        patcher.RegisterPatch<AscensionLocalizationLocStringPatch>();
        patcher.RegisterPatch<AscensionLocalizationHasEntryPatch>();
        patcher.RegisterPatch<AscensionLocalizationIsLocalKeyPatch>();
    }

    private static void RegisterInlineLocalizationPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<SpirePlusInlineLocalizationRawTextPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationLocStringPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationHasEntryPatch>();
        patcher.RegisterPatch<SpirePlusInlineLocalizationIsLocalKeyPatch>();
    }

    private static void RegisterRitsuLibCompatibilityPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<RitsuLibModSettingsButtonSelectionReticlePatch>();
    }
}
