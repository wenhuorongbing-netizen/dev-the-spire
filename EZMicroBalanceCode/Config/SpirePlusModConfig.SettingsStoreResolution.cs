using STS2RitsuLib;
using STS2RitsuLib.Data;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Keep RitsuLib activation and store lookup behind one helper boundary so
    // preview runtime accessors can stay fallback-aware without depending on
    // RitsuLib bootstrap details.
    private static bool CanUseRitsuLibStore => registeredModId is not null && RitsuLibFramework.IsActive;

    private static ModDataStore Store => RitsuLibFramework.GetDataStore(registeredModId ?? MainFile.ModId);
}
