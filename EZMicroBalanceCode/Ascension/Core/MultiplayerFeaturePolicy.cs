using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class MultiplayerFeaturePolicy
{
    public static bool IsSingleplayer(IRunState? runState) =>
        runState == null ||
        (runState.Players.Count <= 1 && CurrentNetType() is NetGameType.Singleplayer or NetGameType.None);

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

    public static void LogCoopEvidence(
        string feature,
        string eventName,
        IRunState? runState,
        IReadOnlyDictionary<string, object?>? data = null) =>
        ReleaseEvidenceLog.Log(feature, eventName, runState: runState, data: data);

    public static string DescribeNetMode(IRunState? runState)
    {
        if (IsSingleplayer(runState))
        {
            return "single";
        }

        if (IsHost(runState))
        {
            return "host";
        }

        if (IsClient(runState))
        {
            return "client";
        }

        return runState?.Players.Count > 1 ? "shared-state" : "single";
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
