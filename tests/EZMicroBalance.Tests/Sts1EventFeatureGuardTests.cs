using Xunit;

namespace EZMicroBalance.Tests;

public sealed class Sts1EventFeatureGuardTests
{
    private static string ReadSts1RuntimeSources()
    {
        return ReadSourceTree("EZMicroBalanceCode", "Sts1Events", "Runtime");
    }

    private static string ReadSts1ModuleSource()
    {
        return ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs");
    }

    [Fact]
    public void FeatureGateDefaultsToOffWhenEnvVarIsUnset()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "private const string ModeEnvKey = \"SPIREPLUS_STS1_EVENT_MODE\"",
            "return Sts1EventRegistrationMode.Off;",
            "string.IsNullOrWhiteSpace(envValue)",
            "Enum.TryParse<Sts1EventRegistrationMode>(envValue, ignoreCase: true, out var mode)");
    }

    [Fact]
    public void FeatureGateEvaluatesAllModes()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "Sts1EventRegistrationMode.Off => FeatureGateResult.Disabled(",
            "Sts1EventRegistrationMode.CanaryOnly => FeatureGateResult.Enabled(",
            "Sts1EventRegistrationMode.AdditiveAllDraft => FeatureGateResult.Enabled(",
            "Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => FeatureGateResult.Enabled(");
    }

    [Fact]
    public void CanaryEventIdsContainsExactlyFourEvents()
    {
        var source = ReadSts1RuntimeSources();

        var canaryBlock = SliceBetween(source, "CanaryEventIds { get; } =", "]");
        AssertSourceContains(canaryBlock,
            "\"sts1_big_fish\"",
            "\"sts1_golden_idol\"",
            "\"sts1_the_lab\"",
            "\"sts1_divine_fountain\"");

        Assert.Equal(4, CountOccurrences(canaryBlock, "\"sts1_"));
    }

    [Fact]
    public void RegistrationModeEnumDefinesFourModes()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationMode.cs");

        AssertSourceContains(
            source,
            "Off = 0,",
            "CanaryOnly = 1,",
            "AdditiveAllDraft = 2,",
            "ReplaceUnknownEventsPrototype = 3,");
    }

    [Fact]
    public void RegisterGatedRoutesCanaryOnlyToRegisterCanaryOnly()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "public static void RegisterGated(string modId, Sts1EventRegistrationMode mode)",
            "case Sts1EventRegistrationMode.CanaryOnly:",
            "RegisterCanaryOnly(modId);",
            "case Sts1EventRegistrationMode.Off:",
            "return;",
            "RegisterAll(modId);");
    }

    [Fact]
    public void RegisterCanaryOnlyRegistersExactlyFourSharedEvents()
    {
        var source = ReadSts1RuntimeSources();

        var canaryMethod = SliceBetween(source, "RegisterCanaryOnly(string modId)", "content.Apply()");
        AssertSourceContains(canaryMethod,
            "content.SharedEvent<Sts1BigFish>()",
            "content.SharedEvent<Sts1GoldenIdol>()",
            "content.SharedEvent<Sts1TheLab>()",
            "content.SharedEvent<Sts1DivineFountain>()");

        Assert.Equal(4, CountOccurrences(canaryMethod, "content.SharedEvent<"));
    }

    [Fact]
    public void ActMappingUsesOvergrowthAndUnderdocksForAct1()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(source,
            "ActEvent<Overgrowth, Sts1ShiningLight>()",
            "ActEvent<Underdocks, Sts1ShiningLight>()",
            "ActEvent<Overgrowth, Sts1Mushrooms>()",
            "ActEvent<Underdocks, Sts1Mushrooms>()");
    }

    [Fact]
    public void ActMappingUsesHiveForAct2()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(source,
            "ActEvent<Hive, Sts1Altar>()",
            "ActEvent<Hive, Sts1DrugDealer>()",
            "ActEvent<Hive, Sts1TheLibrary>()",
            "ActEvent<Hive, Sts1MaskedBandits>()");
    }

    [Fact]
    public void ActMappingUsesGloryForAct3()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(source,
            "ActEvent<Glory, Sts1SensoryStone>()",
            "ActEvent<Glory, Sts1MoaiHead>()",
            "ActEvent<Glory, Sts1Falling>()",
            "ActEvent<Glory, Sts1MindBloom>()");
    }

    [Fact]
    public void Sts1EventsFeatureModuleCallsRegisterGated()
    {
        var source = ReadSts1ModuleSource();

        AssertSourceContains(
            source,
            "Sts1EventRegistrationService.RegisterGated(MainFile.ModId, mode)",
            "Sts1EventFeatureGate.ResolveMode()",
            "FeatureOrders.Sts1Events");
    }

    [Fact]
    public void FeatureModuleRegisteredInSpirePlusFeatureRegistry()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");

        AssertSourceContains(
            source,
            "using EZMicroBalance.EZMicroBalanceCode.Sts1Events;",
            ".Register(new Sts1EventsFeatureModule())");
    }

    [Fact]
    public void Sts1DuplicatorExcludedFromCompilation()
    {
        var csproj = ReadRepoText("EZMicroBalance.csproj");

        AssertSourceContains(
            csproj,
            "<Compile Remove=\"EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1Duplicator.cs\" />");
    }

    [Fact]
    public void Sts1EventRegistrationServiceIsCompiled()
    {
        var csproj = ReadRepoText("EZMicroBalance.csproj");
        Assert.DoesNotContain("Sts1EventRegistrationService.cs", csproj, StringComparison.Ordinal);
    }
}
