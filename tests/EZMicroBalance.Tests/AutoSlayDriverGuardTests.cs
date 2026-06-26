using EZMicroBalance.EZMicroBalanceCode.Core.Integrations.AutoSlay;
using Xunit;

namespace EZMicroBalance.Tests;

/// <summary>
/// Guards for the headless game-native AutoSlay smoke runner hook.
///
/// The hook MUST be default-OFF: with no env var set, the gate is disabled, the
/// driver registers nothing, and normal play is unaffected. These tests pin that
/// contract two ways:
/// - Behavioral unit tests on the pure <see cref="SpirePlusAutoSlayGate"/>
///   (compiled directly into the test assembly; no game/RitsuLib dependency).
/// - Source-shape guards on the driver + MainFile wiring (which reference game
///   and RitsuLib types the test assembly cannot load), asserting the gate guard,
///   the RitsuLib MainMenuReadyEvent subscription, and the AutoSlayer.Start call
///   are present and ordered behind the gate.
/// </summary>
public sealed class AutoSlayDriverGuardTests
{
    private const string EnableVar = "SPIREPLUS_ENABLE_AUTOSLAY";
    private const string SeedVar = "SPIREPLUS_AUTOSLAY_SEED";
    private const string LogVar = "SPIREPLUS_AUTOSLAY_LOG";

    // ---- Default-OFF behavioral guard (the core requirement) ----

    [Fact]
    public void Gate_IsDisabled_WhenEnableEnvVarUnset()
    {
        WithEnv(enable: null, seed: null, log: null, () =>
        {
            Assert.False(SpirePlusAutoSlayGate.IsEnabled);

            // sentinel CLI/seed factory prove the driver would NOT consult them.
            var resolution = SpirePlusAutoSlayGate.Resolve(
                commandLineValue: _ => "cli-should-not-be-used",
                randomSeedFactory: () => "random-should-not-be-used");

            Assert.False(resolution.Enabled);
            Assert.Equal(string.Empty, resolution.Seed);
            Assert.Null(resolution.LogFile);
        });
    }

    [Theory]
    [InlineData("0")]
    [InlineData("false")]
    [InlineData("no")]
    [InlineData("off")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("enabledish")]
    public void Gate_IsDisabled_ForNonTruthyValues(string value)
    {
        WithEnv(enable: value, seed: null, log: null, () =>
        {
            Assert.False(SpirePlusAutoSlayGate.IsEnabled);
            Assert.False(SpirePlusAutoSlayGate.Resolve().Enabled);
        });
    }

    [Theory]
    [InlineData("1")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("Yes")]
    [InlineData("on")]
    [InlineData(" 1 ")]
    public void Gate_IsEnabled_ForTruthyValues(string value)
    {
        WithEnv(enable: value, seed: null, log: null, () =>
        {
            Assert.True(SpirePlusAutoSlayGate.IsEnabled);
        });
    }

    // ---- Seed / log resolution precedence ----

    [Fact]
    public void Resolve_PrefersEnvSeedAndLog_OverCli()
    {
        WithEnv(enable: "1", seed: "ENV-SEED", log: @"C:\evidence\autoslay.log", () =>
        {
            var resolution = SpirePlusAutoSlayGate.Resolve(
                commandLineValue: key => key == SpirePlusAutoSlayGate.SeedCommandLineKey ? "CLI-SEED" : @"C:\cli\other.log",
                randomSeedFactory: () => "RANDOM");

            Assert.True(resolution.Enabled);
            Assert.Equal("ENV-SEED", resolution.Seed);
            Assert.Equal(@"C:\evidence\autoslay.log", resolution.LogFile);
        });
    }

    [Fact]
    public void Resolve_FallsBackToCli_WhenEnvSeedAndLogMissing()
    {
        WithEnv(enable: "1", seed: null, log: null, () =>
        {
            var resolution = SpirePlusAutoSlayGate.Resolve(
                commandLineValue: key => key switch
                {
                    "seed" => "CLI-SEED",
                    "log-file" => @"C:\cli\autoslay.log",
                    _ => null
                },
                randomSeedFactory: () => "RANDOM-SHOULD-NOT-BE-USED");

            Assert.True(resolution.Enabled);
            Assert.Equal("CLI-SEED", resolution.Seed);
            Assert.Equal(@"C:\cli\autoslay.log", resolution.LogFile);
        });
    }

    [Fact]
    public void Resolve_UsesRandomSeed_AndNullLog_WhenNothingSupplied()
    {
        WithEnv(enable: "1", seed: null, log: null, () =>
        {
            var resolution = SpirePlusAutoSlayGate.Resolve(
                commandLineValue: _ => null,
                randomSeedFactory: () => "RANDOM-SEED");

            Assert.True(resolution.Enabled);
            Assert.Equal("RANDOM-SEED", resolution.Seed);
            Assert.Null(resolution.LogFile);
        });
    }

    [Fact]
    public void Resolve_NeverReturnsEmptySeed_EvenWithoutAFactory()
    {
        WithEnv(enable: "1", seed: null, log: null, () =>
        {
            var resolution = SpirePlusAutoSlayGate.Resolve();

            Assert.True(resolution.Enabled);
            Assert.False(string.IsNullOrWhiteSpace(resolution.Seed));
        });
    }

    // ---- Source-shape guards on the driver + MainFile wiring ----

    [Fact]
    public void Driver_IsEnvGated_AndDrivesAutoSlayerThroughMainMenuReadyEvent()
    {
        var driver = ReadRepoText(
            "EZMicroBalanceCode", "Core", "Integrations", "AutoSlay", "SpirePlusAutoSlayDriver.cs");

        // Gate guard must come before any subscription/start work.
        AssertSourceContains(
            driver,
            "if (!SpirePlusAutoSlayGate.IsEnabled)",
            "return false;",
            "RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent>",
            "new AutoSlayer().Start(resolution.Seed, resolution.LogFile)");

        // The default-OFF guard must be ordered before the lifecycle subscription
        // so an unset env var registers nothing.
        AssertBefore(
            driver,
            "if (!SpirePlusAutoSlayGate.IsEnabled)",
            "RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent>");

        // The literal the AutoSlay packet checker keys on:
        // "AutoSlayer.Start(seed, logFile)" must appear in source-facing form.
        Assert.Contains("AutoSlayer.Start", driver, StringComparison.Ordinal);
    }

    [Fact]
    public void MainFile_ArmsAutoSlayDriver_AndOnlyThroughTheGatedEntryPoint()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");

        AssertSourceContains(mainFile, "SpirePlusAutoSlayDriver.TryArm()");

        // MainFile must not call AutoSlayer directly; the gated driver owns that.
        Assert.DoesNotContain("new AutoSlayer(", mainFile, StringComparison.Ordinal);
        Assert.DoesNotContain("AutoSlayer.Start", mainFile, StringComparison.Ordinal);
    }

    [Fact]
    public void Gate_DeclaresStableEnvironmentAndCliContract()
    {
        // The launcher and docs depend on these exact names; pin them.
        Assert.Equal("SPIREPLUS_ENABLE_AUTOSLAY", SpirePlusAutoSlayGate.EnableEnvironmentVariable);
        Assert.Equal("SPIREPLUS_AUTOSLAY_SEED", SpirePlusAutoSlayGate.SeedEnvironmentVariable);
        Assert.Equal("SPIREPLUS_AUTOSLAY_LOG", SpirePlusAutoSlayGate.LogFileEnvironmentVariable);
        Assert.Equal("seed", SpirePlusAutoSlayGate.SeedCommandLineKey);
        Assert.Equal("log-file", SpirePlusAutoSlayGate.LogFileCommandLineKey);
    }

    // ---- helper ----

    /// <summary>
    /// Runs <paramref name="body"/> with the three AutoSlay env vars forced to the
    /// given values (null = removed), restoring the prior process environment after.
    /// Test parallelization is disabled assembly-wide, so this does not race.
    /// </summary>
    private static void WithEnv(string? enable, string? seed, string? log, Action body)
    {
        var priorEnable = Environment.GetEnvironmentVariable(EnableVar);
        var priorSeed = Environment.GetEnvironmentVariable(SeedVar);
        var priorLog = Environment.GetEnvironmentVariable(LogVar);
        try
        {
            Environment.SetEnvironmentVariable(EnableVar, enable);
            Environment.SetEnvironmentVariable(SeedVar, seed);
            Environment.SetEnvironmentVariable(LogVar, log);
            body();
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnableVar, priorEnable);
            Environment.SetEnvironmentVariable(SeedVar, priorSeed);
            Environment.SetEnvironmentVariable(LogVar, priorLog);
        }
    }
}
