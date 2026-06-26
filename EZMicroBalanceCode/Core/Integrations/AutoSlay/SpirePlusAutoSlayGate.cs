using System;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.AutoSlay;

/// <summary>
/// Pure, dependency-free gate for the headless game-native AutoSlay smoke runner.
///
/// This type deliberately references no Slay the Spire 2 or RitsuLib types so the
/// gating + seed/log resolution can be unit-tested in isolation (the test project
/// compiles this file directly and does not load the game/RitsuLib assemblies).
///
/// Default-OFF contract: when <see cref="EnableEnvironmentVariable"/> is unset
/// (or not truthy), <see cref="IsEnabled"/> is false and the driver must do
/// nothing, leaving zero impact on normal play.
/// </summary>
internal static class SpirePlusAutoSlayGate
{
    /// <summary>Master gate. Truthy value ("1"/"true"/"yes"/"on") arms AutoSlay.</summary>
    public const string EnableEnvironmentVariable = "SPIREPLUS_ENABLE_AUTOSLAY";

    /// <summary>Optional seed override. Falls back to the <c>seed</c> CLI arg, then a random seed.</summary>
    public const string SeedEnvironmentVariable = "SPIREPLUS_AUTOSLAY_SEED";

    /// <summary>Optional autoslay.log path. Falls back to the <c>log-file</c> CLI arg, else null.</summary>
    public const string LogFileEnvironmentVariable = "SPIREPLUS_AUTOSLAY_LOG";

    /// <summary>The CLI key AutoSlay reads for the seed, mirrored by the launcher.</summary>
    public const string SeedCommandLineKey = "seed";

    /// <summary>The CLI key AutoSlay reads for the log file, mirrored by the launcher.</summary>
    public const string LogFileCommandLineKey = "log-file";

    /// <summary>True only when the enable env var is present and truthy.</summary>
    public static bool IsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(EnableEnvironmentVariable));

    /// <summary>
    /// Resolved AutoSlay launch parameters. <see cref="Seed"/> is non-null/non-empty
    /// (a random seed is generated when nothing was supplied). <see cref="LogFile"/>
    /// is null when no log file was requested (matching <c>AutoSlayer.Start</c>'s
    /// optional logFile parameter).
    /// </summary>
    public readonly struct Resolution
    {
        public Resolution(bool enabled, string seed, string? logFile)
        {
            Enabled = enabled;
            Seed = seed;
            LogFile = logFile;
        }

        public bool Enabled { get; }

        public string Seed { get; }

        public string? LogFile { get; }
    }

    /// <summary>
    /// Resolves whether AutoSlay should run and, if so, the seed and log file.
    /// Env vars take precedence over CLI values. Pure: callers inject the CLI
    /// lookup and the random-seed factory so this stays game-assembly-free.
    /// </summary>
    /// <param name="commandLineValue">
    /// Resolver for a CLI value by key (e.g. wrap <c>CommandLineHelper.GetValue</c>).
    /// May be null in tests; treated as "no CLI value".
    /// </param>
    /// <param name="randomSeedFactory">
    /// Factory for a fresh random seed when neither env nor CLI supplied one
    /// (e.g. wrap <c>SeedHelper.GetRandomSeed</c>). May be null; a deterministic
    /// fallback seed is then used so the result is never empty.
    /// </param>
    public static Resolution Resolve(
        Func<string, string?>? commandLineValue = null,
        Func<string>? randomSeedFactory = null)
    {
        if (!IsEnabled)
        {
            return new Resolution(enabled: false, seed: string.Empty, logFile: null);
        }

        var seed = FirstNonBlank(
            Environment.GetEnvironmentVariable(SeedEnvironmentVariable),
            SafeCommandLineValue(commandLineValue, SeedCommandLineKey));

        if (string.IsNullOrWhiteSpace(seed))
        {
            seed = SafeRandomSeed(randomSeedFactory);
        }

        var logFile = FirstNonBlank(
            Environment.GetEnvironmentVariable(LogFileEnvironmentVariable),
            SafeCommandLineValue(commandLineValue, LogFileCommandLineKey));

        // Normalize blank -> null so AutoSlayer.Start skips file logging.
        if (string.IsNullOrWhiteSpace(logFile))
        {
            logFile = null;
        }

        return new Resolution(enabled: true, seed: seed!.Trim(), logFile: logFile?.Trim());
    }

    private static string? SafeCommandLineValue(Func<string, string?>? commandLineValue, string key)
    {
        if (commandLineValue is null)
        {
            return null;
        }

        try
        {
            return commandLineValue(key);
        }
        catch
        {
            return null;
        }
    }

    private static string SafeRandomSeed(Func<string>? randomSeedFactory)
    {
        if (randomSeedFactory is not null)
        {
            try
            {
                var generated = randomSeedFactory();
                if (!string.IsNullOrWhiteSpace(generated))
                {
                    return generated.Trim();
                }
            }
            catch
            {
                // Fall through to the deterministic fallback below.
            }
        }

        // Last-resort, never-empty fallback. Not used when a real seed factory is
        // supplied (the driver always supplies SeedHelper.GetRandomSeed).
        return "SPIREPLUS-AUTOSLAY-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
    }

    private static string? FirstNonBlank(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsTruthy(string? value)
    {
        var candidate = value?.Trim();
        return !string.IsNullOrWhiteSpace(candidate) &&
            (candidate.Equals("1", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("true", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("on", StringComparison.OrdinalIgnoreCase));
    }
}
