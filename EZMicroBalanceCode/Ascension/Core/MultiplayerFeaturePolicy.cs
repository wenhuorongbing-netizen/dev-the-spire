using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class MultiplayerFeaturePolicy
{
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

    public static IRunState? CurrentRunStateOrNull()
    {
        try
        {
            return RunManager.Instance?.DebugOnlyGetState();
        }
        catch
        {
            return null;
        }
    }

    public static void LogCoopEvidence(
        string feature,
        string eventName,
        IRunState? runState,
        IReadOnlyDictionary<string, object?>? data = null)
    {
        var policy = MultiplayerPolicyRegistry.Lookup(feature);
        var evidence = data == null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(data);

        evidence["policyRegistered"] = policy != null;
        evidence["policyFeatureId"] = policy?.FeatureId;
        evidence["policyCategory"] = policy?.Category.ToString();
        evidence["policyEnvOverride"] = policy?.EnvOverride;
        evidence["policyVerified"] = policy?.IsVerified;

        ReleaseEvidenceLog.Log(feature, eventName, runState: runState, data: evidence);
    }

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
