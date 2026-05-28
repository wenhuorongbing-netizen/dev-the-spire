using HarmonyLib;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
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
        SpirePlusDebug.Log("RitsuLib", $"Bootstrap starting. RitsuLib {GetRitsuLibVersion()}.");

        var harmony = new Harmony(modId);
        harmony.PatchAll();

        logger.Info($"Harmony patches applied via {modId}.");
        SpirePlusDebug.Log("RitsuLib", "Harmony patches applied.");

        if (RitsuLibFramework.IsActive)
        {
            logger.Info("RitsuLib framework is active.");
            SpirePlusDebug.Log("RitsuLib", "Framework is active.");
        }
        else
        {
            logger.Warn("RitsuLib framework is not active; some features may be unavailable.");
            SpirePlusDebug.Warn("RitsuLib", "Framework is not active; some features may be unavailable.");
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
