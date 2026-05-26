using MegaCrit.Sts2.Core.Runs;
using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class MultiplayerFeaturePolicy
{
    public const string AllowUnverifiedCoopCombatHooksEnvironmentVariable = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS";
    public const string LegacyAllowUnverifiedCoopCombatHooksEnvironmentVariable = "EZMB_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS";
    public const string AllowUnverifiedCoopGameplayEnvironmentVariable = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY";
    public const string LegacyAllowUnverifiedCoopGameplayEnvironmentVariable = "EZMB_ALLOW_UNVERIFIED_COOP_GAMEPLAY";

    private static readonly object CoopGameplayGateLogLock = new();
    private static readonly HashSet<string> LoggedCoopGameplayGateKeys = [];
    private static readonly object CoopCombatGateLogLock = new();
    private static readonly HashSet<string> LoggedCoopCombatGateKeys = [];

    public static bool ShouldDisableUnverifiedCoopFeature(
        IRunState? runState,
        string feature,
        string reason)
    {
        if (IsSingleplayer(runState))
        {
            return false;
        }

        // Until a feature has two-client proof, client-side mutation is more
        // dangerous than disabling the feature. The evidence log makes the gate
        // visible during manual co-op tests instead of silently diverging state.
        LogCoopEvidence(
            feature,
            "coop_gate_disabled",
            runState,
            new Dictionary<string, object?>
            {
                ["reason"] = reason,
                ["canMutateSharedState"] = CanMutateSharedRunState(runState),
                ["players"] = runState?.Players.Count ?? 0
            });
        return true;
    }

    public static bool ShouldDisableUnverifiedCoopGameplay(
        IRunState? runState,
        string feature,
        string reason)
    {
        if (IsSingleplayer(runState))
        {
            return false;
        }

        if (IsUnverifiedCoopGameplayOverrideEnabled)
        {
            LogCoopGateOnce(
                CoopGameplayGateLogLock,
                LoggedCoopGameplayGateKeys,
                feature,
                "coop_gameplay_override_enabled",
                runState,
                reason,
                $"[Spire Plus] {feature} is mutating co-op run state because {AllowUnverifiedCoopGameplayEnvironmentVariable}=1 is set. This path still needs two-client proof.");
            return false;
        }

        LogCoopGateOnce(
            CoopGameplayGateLogLock,
            LoggedCoopGameplayGateKeys,
            feature,
            "coop_gameplay_disabled",
            runState,
            reason,
            $"[Spire Plus] {feature} disabled for co-op: {reason}");
        return true;
    }

    public static bool ShouldDisableUnverifiedCoopCombatHook(
        IRunState? runState,
        string feature,
        string reason)
    {
        if (IsSingleplayer(runState))
        {
            return false;
        }

        if (IsUnverifiedCoopCombatHookOverrideEnabled)
        {
            LogCoopGateOnce(
                CoopCombatGateLogLock,
                LoggedCoopCombatGateKeys,
                feature,
                "coop_combat_hook_override_enabled",
                runState,
                reason,
                $"[Spire Plus] {feature} is running in co-op combat because {AllowUnverifiedCoopCombatHooksEnvironmentVariable}=1 is set. This path still needs two-client proof.");
            return false;
        }

        LogCoopGateOnce(
            CoopCombatGateLogLock,
            LoggedCoopCombatGateKeys,
            feature,
            "coop_combat_hook_disabled",
            runState,
            reason,
            $"[Spire Plus] {feature} disabled for co-op combat: {reason}");
        return true;
    }

    private static bool IsUnverifiedCoopCombatHookOverrideEnabled =>
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(AllowUnverifiedCoopCombatHooksEnvironmentVariable)) ||
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(LegacyAllowUnverifiedCoopCombatHooksEnvironmentVariable));

    private static bool IsUnverifiedCoopGameplayOverrideEnabled =>
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(AllowUnverifiedCoopGameplayEnvironmentVariable)) ||
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(LegacyAllowUnverifiedCoopGameplayEnvironmentVariable));

    private static void LogCoopGateOnce(
        object gateLogLock,
        HashSet<string> loggedGateKeys,
        string feature,
        string eventName,
        IRunState? runState,
        string reason,
        string message)
    {
        var key = string.Join(
            "|",
            RuntimeHelpers.GetHashCode(runState),
            DescribeNetMode(runState),
            runState?.Players.Count ?? 0,
            feature,
            eventName);

        lock (gateLogLock)
        {
            if (!loggedGateKeys.Add(key))
            {
                return;
            }
        }

        MainFile.Logger.Warn(message);
        LogCoopEvidence(
            feature,
            eventName,
            runState,
            new Dictionary<string, object?>
            {
                ["reason"] = reason,
                ["canMutateSharedState"] = CanMutateSharedRunState(runState),
                ["players"] = runState?.Players.Count ?? 0,
                ["netMode"] = DescribeNetMode(runState)
            });
    }
}
