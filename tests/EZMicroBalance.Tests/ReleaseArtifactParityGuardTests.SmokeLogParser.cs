using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseArtifactParityGuardTests
{
    [Fact]
    public void SmokeLogParserDistinguishesEzMicroBalancePassFromUnrelatedManifestErrors()
    {
        var syntheticLog = string.Join(
            Environment.NewLine,
            "[ERROR] Mod manifest D:\\Steam\\mods\\OtherMod\\bad.json is missing the 'id' field! This is not allowed.",
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\BaseLib\\BaseLib.dll",
            "[INFO] Finished mod initialization for 'BaseLib' (BaseLib).",
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.dll",
            "[INFO] Loading Godot PCK D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.pck",
            "[INFO] Finished mod initialization for 'Spire Plus' (EZMicroBalance).",
            "[INFO] [BaseLib] Found 13 SavedSpireFields.",
            "[INFO] [Spire Plus] Urda Trial Branch failed after missed combat 1/3; marked card removed from deck.",
            "[INFO] [Startup] Time to main menu: 12,648ms");

        var summary = SmokeLogParser.Parse(syntheticLog);

        Assert.True(summary.LoadedBaseLibDll);
        Assert.True(summary.InitializedBaseLib);
        Assert.True(summary.LoadedEzDll);
        Assert.True(summary.LoadedEzPck);
        Assert.True(summary.InitializedEzMicroBalance);
        Assert.True(summary.ReachedMainMenu);
        Assert.Equal(13, summary.SavedSpireFieldCount);
        Assert.Empty(summary.EzMicroBalanceErrorLines);
        Assert.Single(summary.UnrelatedManifestErrorLines);
    }

    [Fact]
    public void SmokeLogParserStillFlagsEzMicroBalanceErrorSeverity()
    {
        var syntheticLog = string.Join(
            Environment.NewLine,
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.dll",
            "[ERROR] [Spire Plus] Failed to resolve custom scene.",
            "[INFO] [Spire Plus] Urda Trial Branch failed after missed combat 1/3; marked card removed from deck.",
            "[INFO] [Startup] Time to main menu: 12,648ms");

        var summary = SmokeLogParser.Parse(syntheticLog);

        Assert.Single(summary.EzMicroBalanceErrorLines);
        Assert.Contains("Failed to resolve custom scene", summary.EzMicroBalanceErrorLines[0], StringComparison.Ordinal);
    }

    [Fact]
    public void ControlledSmokePassRequiresCurrentSourceSavedSpireFieldCount()
    {
        var expectedFieldCount = ExpectedCurrentSavedSpireFieldCount();
        Assert.True(expectedFieldCount >= 26, $"Expected current source to define the refreshed 26-field package state or later, found {expectedFieldCount}.");

        var currentLog = CreateControlledSmokeLog(expectedFieldCount);
        var historicalLog = CreateControlledSmokeLog(22);

        Assert.True(IsControlledSmokePass(SmokeLogParser.Parse(currentLog)));
        Assert.False(IsControlledSmokePass(SmokeLogParser.Parse(historicalLog)));
    }

    [Fact]
    public void SmokeLogParserCapturesModManifestVersion()
    {
        var syntheticLog = string.Join(
            Environment.NewLine,
            "[INFO] [BaseLib] Finished init for BaseLib.",
            "[WARN] Mod EZMicroBalance declares version v0.1.0-private-beta.82 which is not a valid Semantic Version",
            "[INFO] Loaded some lines");

        var summary = SmokeLogParser.Parse(syntheticLog);

        Assert.Equal("v0.1.0-private-beta.82", summary.EzMicroBalanceVersion);
    }

    [Fact]
    public void SmokeLogParserCapturesEZMicroBalanceStackTraceAsError()
    {
        var syntheticLog = string.Join(
            Environment.NewLine,
            "[ERROR] System.NullReferenceException: Object reference not set to an instance of an object.",
            "   at EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda.UrdaRunHook.AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)",
            "   at EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda.UrdaBlessingService.TryPlantSeedbedCardFromHand(CardModel card, String source)");

        var summary = SmokeLogParser.Parse(syntheticLog);

        Assert.Contains(
            summary.EzMicroBalanceErrorLines,
            line => line.Contains(
                "at EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda.UrdaRunHook.AfterCardChangedPiles",
                StringComparison.Ordinal));
    }

    private static bool IsControlledSmokePass(SmokeLogSummary summary)
    {
        return summary.LoadedBaseLibDll &&
            summary.InitializedBaseLib &&
            summary.LoadedEzDll &&
            summary.LoadedEzPck &&
            summary.InitializedEzMicroBalance &&
            summary.ReachedMainMenu &&
            summary.SavedSpireFieldCount == ExpectedCurrentSavedSpireFieldCount() &&
            summary.EzMicroBalanceErrorLines.Length == 0;
    }

    private static int ExpectedCurrentSavedSpireFieldCount()
    {
        var source = ReadSourceTree("EZMicroBalanceCode");
        var count = Regex.Matches(
            source,
            @"\bpublic\s+static\s+readonly\s+SavedSpireField<",
            RegexOptions.CultureInvariant).Count;
        Assert.Equal(30, count);
        return count;
    }

    private static string CreateControlledSmokeLog(int savedSpireFieldCount)
    {
        return string.Join(
            Environment.NewLine,
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\BaseLib\\BaseLib.dll",
            "[INFO] Finished mod initialization for 'BaseLib' (BaseLib).",
            "[INFO] Loading assembly DLL D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.dll",
            "[INFO] Loading Godot PCK D:\\Steam\\mods\\EZMicroBalance\\EZMicroBalance.pck",
            "[INFO] Finished mod initialization for 'Spire Plus' (EZMicroBalance).",
            $"[INFO] [BaseLib] Found {savedSpireFieldCount} SavedSpireFields.",
            "[INFO] [Startup] Time to main menu: 12,648ms");
    }

    private sealed record SmokeLogSummary(
        bool LoadedBaseLibDll,
        bool InitializedBaseLib,
        bool LoadedEzDll,
        bool LoadedEzPck,
        bool InitializedEzMicroBalance,
        bool ReachedMainMenu,
        int? SavedSpireFieldCount,
        string[] EzMicroBalanceErrorLines,
        string? EzMicroBalanceVersion,
        string[] UnrelatedManifestErrorLines);

    private static class SmokeLogParser
    {
        public static SmokeLogSummary Parse(string log)
        {
            var lines = log.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var savedFieldCount = Regex.Match(log, @"Found (?<count>\d+) SavedSpireFields\.");
            var versionMatch = Regex.Match(
                log,
                @"Mod EZMicroBalance declares version (?<version>[^\s]+)");

            return new SmokeLogSummary(
                LoadedBaseLibDll: lines.Any(line => line.Contains("Loading assembly DLL", StringComparison.Ordinal) &&
                                                    line.Contains("BaseLib.dll", StringComparison.Ordinal)),
                InitializedBaseLib: lines.Any(line => line.Contains("Finished mod initialization for 'BaseLib' (BaseLib)", StringComparison.Ordinal)),
                LoadedEzDll: lines.Any(line => line.Contains("Loading assembly DLL", StringComparison.Ordinal) &&
                                               line.Contains("EZMicroBalance.dll", StringComparison.Ordinal)),
                LoadedEzPck: lines.Any(line => line.Contains("Loading Godot PCK", StringComparison.Ordinal) &&
                                               line.Contains("EZMicroBalance.pck", StringComparison.Ordinal)),
                InitializedEzMicroBalance: lines.Any(line =>
                    line.Contains("Finished mod initialization for 'Spire Plus' (EZMicroBalance)", StringComparison.Ordinal)),
                ReachedMainMenu: lines.Any(line => line.Contains("Time to main menu", StringComparison.Ordinal)),
                SavedSpireFieldCount: savedFieldCount.Success ? int.Parse(savedFieldCount.Groups["count"].Value) : null,
                EzMicroBalanceErrorLines: lines
                    .Where(line => (line.Contains("EZMicroBalance", StringComparison.Ordinal) ||
                                    line.Contains("Spire Plus", StringComparison.Ordinal)) &&
                                   IsEzMicroBalanceErrorLine(line))
                    .ToArray(),
                EzMicroBalanceVersion: versionMatch.Success ? versionMatch.Groups["version"].Value : null,
                UnrelatedManifestErrorLines: lines
                    .Where(line => line.Contains("Mod manifest", StringComparison.Ordinal) &&
                                   line.Contains("[ERROR]", StringComparison.Ordinal) &&
                                   !line.Contains("EZMicroBalance", StringComparison.Ordinal) &&
                                   !line.Contains("BaseLib", StringComparison.Ordinal))
                    .ToArray());
        }

        private static bool IsEzMicroBalanceErrorLine(string line)
        {
            if (line.TrimStart().StartsWith("at EZMicroBalance.", StringComparison.Ordinal))
            {
                return true;
            }

            if (Regex.IsMatch(line, @"\b(exception|missingmethodexception)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                return true;
            }

            return line.Contains("[ERROR]", StringComparison.Ordinal) &&
                   Regex.IsMatch(line, @"\b(error|failed|missing)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
