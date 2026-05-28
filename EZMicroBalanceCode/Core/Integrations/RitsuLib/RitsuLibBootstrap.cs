using HarmonyLib;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib bootstrap: logging, diagnostics, and lifecycle hooks.
/// Migrated patches use ModPatcher (IPatchMethod); unmigrated patches
/// remain on raw Harmony.PatchAll().
/// </summary>
internal static class RitsuLibBootstrap
{
    public static Harmony ApplyPatches(string modId)
    {
        var logger = RitsuLibFramework.CreateLogger(modId);

        logger.Info($"RitsuLib {GetRitsuLibVersion()} bootstrap starting.");
        SpirePlusDebug.Log("RitsuLib", $"Bootstrap starting. RitsuLib {GetRitsuLibVersion()}.");

        // Apply migrated patches via ModPatcher
        var patcher = RitsuLibFramework.CreatePatcher(modId, "SpirePlus");
        RegisterMigratedPatches(patcher);
        patcher.PatchAll();
        logger.Info($"ModPatcher applied {patcher.AppliedPatchCount} patches ({patcher.RegisteredPatchCount} registered).");
        SpirePlusDebug.Log("RitsuLib", $"ModPatcher applied {patcher.AppliedPatchCount} patches.");

        // Apply remaining [HarmonyPatch]-attributed patches via raw Harmony
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

    private static void RegisterMigratedPatches(ModPatcher patcher)
    {
        // FiddlePatches (4 classes)
        patcher.RegisterPatch<FiddleVarsPatch>();
        patcher.RegisterPatch<FiddleHandDrawPatch>();
        patcher.RegisterPatch<FiddleShouldDrawPatch>();
        patcher.RegisterPatch<FiddleDrawCapPatch>();

        // ChoicesParadoxPatches (1 class)
        patcher.RegisterPatch<ChoicesParadoxPatch>();

        // DistinguishedCapePatches (3 classes)
        patcher.RegisterPatch<DistinguishedCapeVarsPatch>();
        patcher.RegisterPatch<DistinguishedCapeEventOptionPatch>();
        patcher.RegisterPatch<DistinguishedCapePickupPatch>();

        // BlackStarCompensationPatches (1 class)
        patcher.RegisterPatch<BlackStarObtainPatch>();
    }

    private static string GetRitsuLibVersion()
    {
        var asm = typeof(RitsuLibFramework).Assembly;
        var version = asm.GetName().Version;
        return version?.ToString(3) ?? "unknown";
    }
}
