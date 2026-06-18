using System;
using System.IO;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
{
    [Fact]
    public void IFeatureModuleDeclaresMetadataProperties()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "IFeatureModule.cs");

        AssertSourceContains(source,
            "string DisplayName => Id;",
            "string Category => \"General\";",
            "IReadOnlyList<string> DisableEnvKeys =>",
            "IReadOnlyList<string> ForceEnvKeys =>");
    }

    [Fact]
    public void FeatureRegistryTracksBootstrapRecords()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");

        AssertSourceContains(source,
            "List<FeatureBootstrapRecord> bootstrapRecords",
            "IReadOnlyList<FeatureBootstrapRecord> BootstrapRecords",
            "GetBootstrapRecord(string id)",
            "LogFeatureSummary()");
    }

    [Fact]
    public void FeatureBootstrapRecordDefinesLiveStatus()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureBootstrapRecord.cs");

        AssertSourceContains(source,
            "enum FeatureLiveStatus",
            "Enabled = 0",
            "Disabled = 1",
            "Failed = 2",
            "record FeatureBootstrapRecord",
            "bool IsActive");
    }

    [Fact]
    public void AllFeatureModulesProvideDisplayName()
    {
        var moduleFiles = new[]
        {
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs"),
        };

        foreach (var filePath in moduleFiles)
        {
            Assert.True(File.Exists(filePath), $"Feature module file not found: {filePath}");
            var source = File.ReadAllText(filePath);
            AssertSourceContains(source, "string DisplayName =>");
        }
    }

    [Fact]
    public void AllFeatureModulesProvideCategory()
    {
        var moduleFiles = new[]
        {
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureModule.cs"),
            RepoPath("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs"),
        };

        foreach (var filePath in moduleFiles)
        {
            Assert.True(File.Exists(filePath), $"Feature module file not found: {filePath}");
            var source = File.ReadAllText(filePath);
            AssertSourceContains(source, "string Category =>");
        }
    }

    [Fact]
    public void MainFileCallsLogFeatureSummary()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");

        Assert.Contains("registry.LogFeatureSummary()", mainFile, StringComparison.Ordinal);
    }

    [Fact]
    public void Sts1EventsModuleIsOffByDefault()
    {
        // Verify the Sts1EventFeatureGate evaluates to Disabled when env var is unset.
        var gateSource = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventFeatureGate.cs");

        // ResolveMode must return Off when env var is empty/null
        AssertSourceContains(gateSource,
            "string.IsNullOrWhiteSpace(envValue)",
            "return Sts1EventRegistrationMode.Off;");

        // EvaluateGate must map Off to Disabled
        AssertSourceContains(gateSource,
            "Sts1EventRegistrationMode.Off => FeatureGateResult.Disabled(",
            "StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.");

        // The mode variable is handled by Sts1EventFeatureGate; treating it as a
        // generic disable key would block CanaryOnly/AdditiveBatch1 before init.
        var moduleSource = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs");
        Assert.DoesNotContain("DisableEnvKeys", moduleSource, StringComparison.Ordinal);
    }

    [Fact]
    public void VakuuFightModuleRegistersHooksButFightIsHiddenByDefault()
    {
        // The feature module itself is EnabledByDefault (registers hooks),
        // but the fight entry is gated by VakuuFightFeatureGate at runtime.
        var moduleSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs");
        AssertSourceContains(moduleSource,
            "FeatureGateResult.EnabledByDefault(",
            "hooks registered; fight entry remains hidden by VakuuFightFeatureGate.");

        // The runtime gate requires env vars to enable the fight
        var gateSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        AssertSourceContains(gateSource,
            "EnableEnvironmentVariable",
            "DisableEnvironmentVariable",
            "ShouldEnableFight",
            "IsFightEnabled(");

        // ShouldEnableFight requires at least one env var to be truthy (no unconditional true path)
        Assert.DoesNotContain("ShouldEnableFight => true", gateSource, StringComparison.Ordinal);

        // IsFightEnabled must check ShouldEnableFight AND NOT disable vars
        AssertSourceContains(gateSource,
            "ShouldEnableFight &&",
            "!AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)");
    }

    [Fact]
    public void FeatureBootstrapRecordCapturesCorrectFieldsPerModule()
    {
        // Verify FeatureBootstrapRecord is a record with all required fields
        var recordSource = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureBootstrapRecord.cs");
        AssertSourceContains(recordSource,
            "string Id,",
            "string DisplayName,",
            "string Category,",
            "FeatureGateResult Gate,",
            "FeatureLiveStatus LiveStatus,",
            "string? FailureMessage",
            "bool IsActive");

        // Verify the registry creates records with all fields populated
        var registrySource = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");
        AssertSourceContains(registrySource,
            "new FeatureBootstrapRecord(",
            "module.Id, module.DisplayName, module.Category,",
            "gate, FeatureLiveStatus.Disabled)",
            "gate, FeatureLiveStatus.Enabled)",
            "gate, FeatureLiveStatus.Failed");

        // Verify the summary log includes Id, DisplayName, Category, Gate, LiveStatus, Reason
        AssertSourceContains(registrySource,
            "record.DisplayName",
            "record.Gate.IsEnabled",
            "record.LiveStatus",
            "record.Gate.Reason");

        // Verify SpirePlusFeatureRegistry registers exactly the 6 expected modules
        var registryFactory = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        AssertSourceContains(registryFactory,
            "new LothaFeatureModule()",
            "new MorviFeatureModule()",
            "new UrdaFeatureModule()",
            "new VakuuFightFeatureModule()",
            "new AscensionFeatureModule()",
            "new Sts1EventsFeatureModule()");

        // Each registered module must have a matching FeatureOrders constant
        var featureOrders = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureOrders.cs");
        AssertSourceContains(featureOrders,
            "AncientsLotha",
            "AncientsMorvi",
            "AncientsUrda",
            "AncientsVakuuFight",
            "AscensionA11A20",
            "Sts1Events");
    }

    [Fact]
    public void FeatureRegistryAppliesUnifiedEnvironmentOverrides()
    {
        var registrySource = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");
        var environmentSource = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.Environment.cs");

        AssertSourceContains(registrySource,
            "ApplyEnvironmentOverrides(module, module.EvaluateGate())");

        Assert.DoesNotContain("private static FeatureGateResult ApplyEnvironmentOverrides(IFeatureModule module, FeatureGateResult gate)", registrySource, StringComparison.Ordinal);
        Assert.DoesNotContain("private static string? FirstTruthyEnvironmentKey(IEnumerable<string> keys)", registrySource, StringComparison.Ordinal);
        Assert.DoesNotContain("private static bool IsTruthyEnvironmentValue(string? value) =>", registrySource, StringComparison.Ordinal);

        AssertSourceContains(environmentSource,
            "internal sealed partial class FeatureRegistry",
            "private static FeatureGateResult ApplyEnvironmentOverrides(IFeatureModule module, FeatureGateResult gate)",
            "FirstTruthyEnvironmentKey(module.ForceEnvKeys)",
            "FirstTruthyEnvironmentKey(module.DisableEnvKeys)",
            "forced by {forceKey}; original gate:",
            "disabled by {disableKey}; original gate:",
            "private static string? FirstTruthyEnvironmentKey(IEnumerable<string> keys)",
            "private static bool IsTruthyEnvironmentValue(string? value) =>",
            "!string.Equals(value, \"0\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(value, \"false\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(value, \"off\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(value, \"no\", StringComparison.OrdinalIgnoreCase)");

        AssertBefore(environmentSource,
            "FirstTruthyEnvironmentKey(module.ForceEnvKeys)",
            "FirstTruthyEnvironmentKey(module.DisableEnvKeys)");
    }
}
