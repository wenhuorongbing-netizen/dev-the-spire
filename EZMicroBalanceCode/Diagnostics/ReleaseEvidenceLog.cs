using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static class ReleaseEvidenceLog
{
    public const string EnvironmentVariable = "SPIREPLUS_RELEASE_EVIDENCE_LOG";
    public const string LegacyEnvironmentVariable = "EZMB_RELEASE_EVIDENCE_LOG";
    public const string Prefix = "[SPIREPLUS-EVIDENCE]";

    public static bool IsEnabled =>
        AscensionExpansionConfig.IsTruthy(Environment.GetEnvironmentVariable(EnvironmentVariable)) ||
        AscensionExpansionConfig.IsTruthy(Environment.GetEnvironmentVariable(LegacyEnvironmentVariable));

    public static void Log(
        string feature,
        string eventName,
        IRunState? runState = null,
        Player? player = null,
        IReadOnlyDictionary<string, object?>? data = null)
    {
        if (!IsEnabled)
        {
            return;
        }

        MainFile.Logger.Info(
            $"{Prefix} {Token(feature)} {Token(eventName)} run={RunId(runState)} player={PlayerId(runState, player)} net={NetMode(runState)} data={FormatData(data)}");
    }

    public static void Log(
        string feature,
        string eventName,
        Player player,
        IReadOnlyDictionary<string, object?>? data = null) =>
        Log(feature, eventName, player.RunState, player, data);

    private static string RunId(IRunState? runState)
    {
        if (runState == null)
        {
            return "none";
        }

        return $"{runState.Rng.Seed}:{runState.CurrentActIndex}:{runState.ActFloor}";
    }

    private static string PlayerId(IRunState? runState, Player? player)
    {
        if (runState == null || player == null)
        {
            return "none";
        }

        try
        {
            return runState.GetPlayerSlotIndex(player).ToString();
        }
        catch
        {
            return "unknown";
        }
    }

    private static string NetMode(IRunState? runState)
    {
        try
        {
            var type = RunManager.Instance?.NetService?.Type ?? NetGameType.None;
            return type switch
            {
                NetGameType.Host => "host",
                NetGameType.Client => "client",
                NetGameType.Singleplayer when runState?.Players.Count > 1 => "shared-state",
                NetGameType.Singleplayer => "single",
                NetGameType.Replay => "replay",
                _ when runState?.Players.Count > 1 => "shared-state",
                _ => "single"
            };
        }
        catch
        {
            return runState?.Players.Count > 1 ? "shared-state" : "single";
        }
    }

    private static string FormatData(IReadOnlyDictionary<string, object?>? data)
    {
        if (data == null || data.Count == 0)
        {
            return "{}";
        }

        return "{" + string.Join(",", data.Select(entry => $"{Token(entry.Key)}:{Value(entry.Value)}")) + "}";
    }

    private static string Value(object? value)
    {
        return value switch
        {
            null => "null",
            bool boolean => boolean ? "true" : "false",
            string text => $"\"{Escape(text)}\"",
            Enum enumValue => $"\"{enumValue}\"",
            _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "null"
        };
    }

    private static string Token(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? "unknown"
            : Escape(value.Trim()).Replace(' ', '_');

    private static string Escape(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
}
