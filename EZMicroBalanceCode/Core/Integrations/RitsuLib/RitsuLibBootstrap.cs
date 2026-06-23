using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using STS2RitsuLib;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib bootstrap: logging, diagnostics, and lifecycle hooks.
/// All current patches are registered through RitsuLib ModPatcher (IPatchMethod);
/// broad Harmony PatchAll discovery is intentionally disabled.
/// </summary>
internal static class RitsuLibBootstrap
{
    public static void ApplyPatches(string modId)
    {
        var logger = RitsuLibFramework.CreateLogger(modId);

        logger.Info($"RitsuLib {GetRitsuLibVersion()} bootstrap starting.");
        SpirePlusDebug.Log("RitsuLib", $"Bootstrap starting. RitsuLib {GetRitsuLibVersion()}.");

        ApplyMigratedRitsuLibPatches();
        LogPatchAllDisabled();
        AuditRitsuLibRuntimeState();

        void ApplyMigratedRitsuLibPatches()
        {
            // RitsuLib ModPatcher owns every migrated patch class. Keeping this
            // path first makes the RitsuLib migration the canonical route and
            // prevents a migrated patch from silently drifting back to broad
            // Harmony.PatchAll() discovery.
            var patcher = RitsuLibFramework.CreatePatcher(modId, "SpirePlus");
            SpirePlusMigratedPatchRegistry.RegisterAll(patcher);
            var applied = RitsuLibFramework.ApplyRequiredPatcher(
                patcher,
                () =>
                {
                    // Required migrated patches are the mod's current runtime contract.
                    // Continuing into saved-state, content, and feature initialization
                    // after a failed ModPatcher apply would create a half-booted mod.
                    logger.Warn("Required RitsuLib ModPatcher apply failed; aborting Spire Plus bootstrap.");
                    SpirePlusDebug.Warn("RitsuLib", "Required ModPatcher apply failed; aborting bootstrap.");
                },
                "Spire Plus migrated patches");

            if (!applied)
            {
                throw new InvalidOperationException(
                    "Required RitsuLib ModPatcher apply failed; Spire Plus bootstrap stopped before feature initialization.");
            }

            logger.Info($"ModPatcher applied {patcher.AppliedPatchCount} patches ({patcher.RegisteredPatchCount} registered).");
            SpirePlusDebug.Log("RitsuLib", $"ModPatcher applied {patcher.AppliedPatchCount} patches.");
        }

        void LogPatchAllDisabled()
        {
            // RitsuLib ModPatcher creates the underlying Harmony patch owner.
            // This bootstrap deliberately avoids creating a second ad hoc
            // Harmony instance so PatchAll cannot return through an old path.
            // New patch work must enter through SpirePlusMigratedPatchRegistry.
            logger.Info($"Harmony PatchAll fallback disabled; RitsuLib ModPatcher owns Spire Plus patches for {modId}.");
            SpirePlusDebug.Log("RitsuLib", "Harmony PatchAll fallback disabled; ModPatcher owns patch registration.");
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
