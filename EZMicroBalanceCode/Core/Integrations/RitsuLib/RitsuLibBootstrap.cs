using HarmonyLib;
using STS2RitsuLib;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib bootstrap: logging, diagnostics, and lifecycle hooks.
/// Patch application remains on raw Harmony for now; migrating to
/// RitsuLib's ModPatcher (IPatchMethod/IModPatchProvider) is a future batch.
/// </summary>
internal static class RitsuLibBootstrap
{
    public static Harmony ApplyPatches(string modId)
    {
        var logger = RitsuLibFramework.CreateLogger(modId);

        logger.Info($"RitsuLib {GetRitsuLibVersion()} bootstrap starting.");

        var harmony = new Harmony(modId);
        harmony.PatchAll();

        logger.Info($"Harmony patches applied via {modId}.");

        if (RitsuLibFramework.IsActive)
        {
            logger.Info("RitsuLib framework is active.");
        }
        else
        {
            logger.Warn("RitsuLib framework is not active; some features may be unavailable.");
        }

        return harmony;
    }

    private static string GetRitsuLibVersion()
    {
        var asm = typeof(RitsuLibFramework).Assembly;
        var version = asm.GetName().Version;
        return version?.ToString(3) ?? "unknown";
    }
}
