using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class MultiplayerFeaturePolicy
{
    public const string AllowUnverifiedCoopCombatHooksEnvironmentVariable = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS";
    public const string LegacyAllowUnverifiedCoopCombatHooksEnvironmentVariable = "EZMB_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS";
    public const string AllowUnverifiedCoopPreviewToolsEnvironmentVariable = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS";
    public const string LegacyAllowUnverifiedCoopPreviewToolsEnvironmentVariable = "EZMB_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS";

    private static readonly object CoopCombatGateLogLock = new();
    private static readonly HashSet<string> LoggedCoopCombatGateKeys = [];
    private static readonly object CoopPreviewGateLogLock = new();
    private static readonly HashSet<string> LoggedCoopPreviewGateKeys = [];

    public static bool IsSingleplayer(IRunState? runState)
    {
        var netType = CurrentNetType();
        return netType is NetGameType.Singleplayer or NetGameType.None &&
            (runState == null || runState.Players.Count <= 1);
    }

    public static bool IsHost(IRunState? runState) =>
        runState != null && CurrentNetType() == NetGameType.Host;

    public static bool IsClient(IRunState? runState) =>
        runState != null && CurrentNetType() == NetGameType.Client;

    public static bool CanMutateSharedRunState(IRunState? runState) =>
        IsSingleplayer(runState) || IsHost(runState);

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
            LogCoopCombatGateOnce(
                feature,
                "coop_combat_hook_override_enabled",
                runState,
                reason,
                $"[Spire Plus] {feature} is running in co-op combat because {AllowUnverifiedCoopCombatHooksEnvironmentVariable}=1 is set. This path still needs two-client proof.");
            return false;
        }

        LogCoopCombatGateOnce(
            feature,
            "coop_combat_hook_disabled",
            runState,
            reason,
            $"[Spire Plus] {feature} disabled for co-op combat: {reason}");
        return true;
    }

    public static bool ShouldDisableUnverifiedCoopPreviewTool(
        IRunState? runState,
        string feature,
        string reason)
    {
        if (IsSingleplayer(runState))
        {
            return false;
        }

        if (IsUnverifiedCoopPreviewToolOverrideEnabled)
        {
            LogCoopPreviewGateOnce(
                feature,
                "coop_preview_tool_override_enabled",
                runState,
                reason,
                $"[Spire Plus] {feature} preview is running in co-op because {AllowUnverifiedCoopPreviewToolsEnvironmentVariable}=1 is set. This path still needs two-client proof.");
            return false;
        }

        LogCoopPreviewGateOnce(
            feature,
            "coop_preview_tool_disabled",
            runState,
            reason,
            $"[Spire Plus] {feature} preview disabled for co-op: {reason}");
        return true;
    }

    public static void LogCoopEvidence(
        string feature,
        string eventName,
        IRunState? runState,
        IReadOnlyDictionary<string, object?>? data = null) =>
        ReleaseEvidenceLog.Log(feature, eventName, runState: runState, data: data);

    public static string DescribeNetMode(IRunState? runState)
    {
        var netType = CurrentNetType();
        if (netType == NetGameType.Host)
        {
            return "host";
        }

        if (netType == NetGameType.Client)
        {
            return "client";
        }

        if (IsSingleplayer(runState))
        {
            return "single";
        }

        return runState?.Players.Count > 1 ? "shared-state" : "single";
    }

    private static bool IsUnverifiedCoopCombatHookOverrideEnabled =>
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(AllowUnverifiedCoopCombatHooksEnvironmentVariable)) ||
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(LegacyAllowUnverifiedCoopCombatHooksEnvironmentVariable));

    private static bool IsUnverifiedCoopPreviewToolOverrideEnabled =>
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(AllowUnverifiedCoopPreviewToolsEnvironmentVariable)) ||
        AscensionExpansionConfig.IsTruthy(
            Environment.GetEnvironmentVariable(LegacyAllowUnverifiedCoopPreviewToolsEnvironmentVariable));

    private static void LogCoopCombatGateOnce(
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

        lock (CoopCombatGateLogLock)
        {
            if (!LoggedCoopCombatGateKeys.Add(key))
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

    private static void LogCoopPreviewGateOnce(
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

        lock (CoopPreviewGateLogLock)
        {
            if (!LoggedCoopPreviewGateKeys.Add(key))
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

    private static NetGameType CurrentNetType()
    {
        try
        {
            return RunManager.Instance?.NetService?.Type ?? NetGameType.None;
        }
        catch
        {
            return NetGameType.None;
        }
    }
}
