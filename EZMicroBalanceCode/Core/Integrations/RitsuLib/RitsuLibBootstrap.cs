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

        ApplyMigratedRitsuLibPatches();
        var harmony = ApplyLegacyHarmonyFallbackPatches();
        AuditRitsuLibRuntimeState();

        return harmony;

        void ApplyMigratedRitsuLibPatches()
        {
            // RitsuLib ModPatcher owns every migrated patch class. Keeping this
            // path first makes the RitsuLib migration the canonical route and
            // prevents a migrated patch from silently drifting back to broad
            // Harmony.PatchAll() discovery.
            var patcher = RitsuLibFramework.CreatePatcher(modId, "SpirePlus");
            SpirePlusMigratedPatchRegistry.RegisterAll(patcher);
            patcher.PatchAll();

            logger.Info($"ModPatcher applied {patcher.AppliedPatchCount} patches ({patcher.RegisteredPatchCount} registered).");
            SpirePlusDebug.Log("RitsuLib", $"ModPatcher applied {patcher.AppliedPatchCount} patches.");
        }

        Harmony ApplyLegacyHarmonyFallbackPatches()
        {
            // Legacy Harmony discovery is still needed for patch surfaces that
            // have not been proven through RitsuLib IPatchMethod yet. Treat this
            // as a migration backlog boundary: do not register new dependencies
            // or new current RitsuLib-compatible patches here.
            var fallbackHarmony = new Harmony(modId);
            fallbackHarmony.PatchAll();

            logger.Info($"Harmony patches applied via {modId}.");
            SpirePlusDebug.Log("RitsuLib", "Harmony patches applied.");

            return fallbackHarmony;
        }

        void AuditRitsuLibRuntimeState()
        {
            // A false state here is not permission for fallback to another mod framework.
            // It means the current RitsuLib install/runtime load must be diagnosed
            // before making feature or package-readiness claims.
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

        }
    }

    private static string GetRitsuLibVersion()
    {
        var asm = typeof(RitsuLibFramework).Assembly;
        var version = asm.GetName().Version;
        return version?.ToString(3) ?? "unknown";
    }
}
