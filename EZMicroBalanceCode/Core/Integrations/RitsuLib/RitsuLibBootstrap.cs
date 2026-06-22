using HarmonyLib;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using STS2RitsuLib;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib bootstrap: logging, diagnostics, and lifecycle hooks.
/// Migrated patches use ModPatcher (IPatchMethod); remaining legacy Harmony
/// patches stay behind Harmony.PatchAll() until each batch has owner approval,
/// source evidence, and focused runtime validation.
/// </summary>
internal static class RitsuLibBootstrap
{
    public static Harmony ApplyPatches(string modId)
    {
        var logger = RitsuLibFramework.CreateLogger(modId);

        logger.Info($"RitsuLib {GetRitsuLibVersion()} bootstrap starting.");
        SpirePlusDebug.Log("RitsuLib", $"Bootstrap starting. RitsuLib {GetRitsuLibVersion()}.");

        // RitsuLib ModPatcher owns every migrated patch class. Keeping this
        // explicit list prevents accidental double-patching through PatchAll().
        var patcher = RitsuLibFramework.CreatePatcher(modId, "SpirePlus");
        SpirePlusMigratedPatchRegistry.RegisterAll(patcher);
        patcher.PatchAll();
        logger.Info($"ModPatcher applied {patcher.AppliedPatchCount} patches ({patcher.RegisteredPatchCount} registered).");
        SpirePlusDebug.Log("RitsuLib", $"ModPatcher applied {patcher.AppliedPatchCount} patches.");

        // Legacy Harmony patches remain only for surfaces not yet migrated.
        // Do not add new framework dependencies here; new migration work should
        // either use RitsuLib IPatchMethod or stay raw Harmony with a documented
        // Batch 4c+ approval boundary.
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
