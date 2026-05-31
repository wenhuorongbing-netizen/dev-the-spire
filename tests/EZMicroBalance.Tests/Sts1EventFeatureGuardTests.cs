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
            "Sts1EventRegistrationMode.AdditiveBatch1 => FeatureGateResult.Enabled(",
            "Sts1EventRegistrationMode.AdditiveAllDraft => EvaluateAdditiveAllDraftGate()",
            "Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => EvaluateReplacementPrototypeGate()");
    }

    [Fact]
    public void UnsafeModesRequireExplicitUnsafeOverride()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "private const string UnsafeModeEnvKey = \"SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES\"",
            "set SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1 to enable",
            "IsUnsafeModeAllowed()",
            "IsTruthyEnvironmentValue(System.Environment.GetEnvironmentVariable(UnsafeModeEnvKey))");
    }

    [Fact]
    public void ReplacementPrototypeGateFailsClosedWithoutCompileSymbol()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "#if REPLACEMENT_PROTOTYPE_ENABLED",
            "#else",
            "FeatureGateResult.Disabled(",
            "REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.");
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
    public void AdditiveBatch1EventIdsContainsExactlyVerifiedScope()
    {
        var source = ReadSts1RuntimeSources();

        var batchBlock = SliceBetween(source, "AdditiveBatch1EventIds { get; } =", "]");
        AssertSourceContains(batchBlock,
            "\"sts1_big_fish\"",
            "\"sts1_golden_idol\"",
            "\"sts1_the_lab\"",
            "\"sts1_divine_fountain\"",
            "\"sts1_purifier\"",
            "\"sts1_upgrade_shrine\"",
            "\"sts1_golden_shrine\"",
            "\"sts1_the_cleric\"",
            "\"sts1_old_beggar\"",
            "\"sts1_shining_light\"");

        Assert.Equal(10, CountOccurrences(batchBlock, "\"sts1_"));
    }

    [Fact]
    public void RegistrationModeEnumDefinesFiveModes()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationMode.cs");

        AssertSourceContains(
            source,
            "Off = 0,",
            "CanaryOnly = 1,",
            "AdditiveBatch1 = 2,",
            "AdditiveAllDraft = 3,",
            "ReplaceUnknownEventsPrototype = 4,");
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
            "case Sts1EventRegistrationMode.AdditiveBatch1:",
            "RegisterAdditiveBatch1(modId);",
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
    public void RegisterAdditiveBatch1RegistersOnlyVerifiedScope()
    {
        var source = ReadSts1RuntimeSources();

        var batchMethod = SliceBetween(source, "RegisterAdditiveBatch1(string modId)", "content.Apply()");
        AssertSourceContains(batchMethod,
            "content.SharedEvent<Sts1BigFish>()",
            "content.SharedEvent<Sts1GoldenIdol>()",
            "content.SharedEvent<Sts1TheLab>()",
            "content.SharedEvent<Sts1DivineFountain>()",
            "content.SharedEvent<Sts1Purifier>()",
            "content.ActEvent<Glory, Sts1UpgradeShrine>()",
            "content.SharedEvent<Sts1GoldenShrine>()",
            "content.SharedEvent<Sts1TheCleric>()",
            "content.SharedEvent<Sts1OldBeggar>()",
            "content.ActEvent<Overgrowth, Sts1ShiningLight>()",
            "content.ActEvent<Underdocks, Sts1ShiningLight>()");

        var totalRegistrations = CountOccurrences(batchMethod, "content.ActEvent<") + CountOccurrences(batchMethod, "content.SharedEvent<");
        Assert.Equal(11, totalRegistrations);
        Assert.DoesNotContain("RegisterAll", batchMethod, StringComparison.Ordinal);
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
    public void ModeEnvironmentVariableIsNotAFeatureRegistryDisableOverride()
    {
        var moduleSource = ReadSts1ModuleSource();

        Assert.DoesNotContain("DisableEnvKeys", moduleSource, StringComparison.Ordinal);
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

    [Fact]
    public void CombatEventsDeclareIsSharedTrue()
    {
        // Combat events MUST have IsShared = true because EnterCombatWithoutExitingEvent requires it.
        var combatEventFiles = new[]
        {
            Path.Combine("Act1", "Sts1DeadAdventurer.cs"),
            Path.Combine("Act1", "Sts1ScorpionNest.cs"),
            Path.Combine("Act1", "Sts1TreasureOoze.cs"),
            Path.Combine("Act2", "Sts1MaskedBandits.cs"),
            Path.Combine("Act3", "Sts1MysteriousSphere.cs"),
            Path.Combine("Act3", "Sts1MindBloom.cs"),
        };

        foreach (var relativePath in combatEventFiles)
        {
            var filePath = Path.Combine(Root, "EZMicroBalanceCode", "Sts1Events", "Models", relativePath);
            Assert.True(File.Exists(filePath), $"Combat event model file not found: {relativePath}");
            var source = File.ReadAllText(filePath);
            AssertSourceContains(source, "public override bool IsShared => true;");
        }
    }

    [Fact]
    public void RegistryDoesNotClassifyNonCombatAct1EventsAsCombat()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistry.cs");

        AssertSourceContains(source,
            "new(\"sts1_joust\", \"Joust\", Sts1EventPhase.Simple, Sts1EventAct.Act1)",
            "new(\"sts1_the_ssssserpent\", \"The Ssssserpent\", Sts1EventPhase.Simple, Sts1EventAct.Act1)");
        Assert.DoesNotContain("new(\"sts1_joust\", \"Joust\", Sts1EventPhase.Combat", source, StringComparison.Ordinal);
        Assert.DoesNotContain("new(\"sts1_the_ssssserpent\", \"The Ssssserpent\", Sts1EventPhase.Combat", source, StringComparison.Ordinal);
    }

    [Fact]
    public void AllSharedEventModelsDeclareIsSharedTrue()
    {
        // All Shared-act models must declare IsShared => true for consistent multiplayer behavior.
        var sharedDir = Path.Combine(Root, "EZMicroBalanceCode", "Sts1Events", "Models", "Shared");
        Assert.True(Directory.Exists(sharedDir), $"Shared events directory not found: {sharedDir}");

        var csFiles = Directory.GetFiles(sharedDir, "*.cs");
        Assert.True(csFiles.Length >= 17, $"Expected at least 17 shared event models, found {csFiles.Length}");

        foreach (var filePath in csFiles)
        {
            var source = File.ReadAllText(filePath);
            AssertSourceContains(source, "public override bool IsShared => true;");
        }
    }

    [Fact]
    public void RegistryEntryCountIs50()
    {
        // 50 entries: 47 compiling + 1 compile-excluded (Duplicator) + 2 Special stubs (Neow, Combat Start).
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistry.cs");
        var entriesBlock = SliceBetween(source, "private static readonly List<Sts1EventEntry> Events = new()", "};");
        var entryCount = CountOccurrences(entriesBlock, "new(\"");
        Assert.Equal(50, entryCount);
    }

    [Fact]
    public void RegistryCanaryPhaseMatchesCanaryEventIds()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistry.cs");
        var entriesBlock = SliceBetween(source, "private static readonly List<Sts1EventEntry> Events = new()", "// Phase 2: Simple batch");

        AssertSourceContains(entriesBlock,
            "new(\"sts1_big_fish\", \"Big Fish\", Sts1EventPhase.Canary, Sts1EventAct.Shared)",
            "new(\"sts1_golden_idol\", \"Golden Idol\", Sts1EventPhase.Canary, Sts1EventAct.Shared)",
            "new(\"sts1_the_lab\", \"The Lab\", Sts1EventPhase.Canary, Sts1EventAct.Shared)",
            "new(\"sts1_divine_fountain\", \"Divine Fountain\", Sts1EventPhase.Canary, Sts1EventAct.Shared)");

        Assert.Equal(4, CountOccurrences(entriesBlock, "Sts1EventPhase.Canary"));
    }

    [Fact]
    public void RegisterAllSharedEventCountIs17()
    {
        // 17 SharedEvent calls in RegisterAll: Big Fish, Golden Idol, The Cleric, Golden Wing,
        // Living Wall, Old Beggar, Purifier, Golden Shrine, Bonfire Spirits, Divine Fountain,
        // Fountain of Cleansing, The Lab, Face Trader, The Mausoleum, Designer, The Woman in Blue,
        // Wheel of Change. Sts1Duplicator is excluded from compilation.
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationService.cs");
        var registerAllMethod = SliceBetween(source, "public static void RegisterAll(string modId)", "content.Apply();");
        var sharedEventCount = CountOccurrences(registerAllMethod, "content.SharedEvent<");
        Assert.Equal(17, sharedEventCount);
    }

    [Fact]
    public void OffModeReturnsImmediatelyWithZeroRegistrations()
    {
        // Off mode must return without registering any events.
        var source = ReadSts1RuntimeSources();
        var gatedMethod = SliceBetween(source, "public static void RegisterGated(string modId, Sts1EventRegistrationMode mode)", "}");

        AssertSourceContains(gatedMethod,
            "case Sts1EventRegistrationMode.Off:",
            "return;");
    }

    [Fact]
    public void PatchBoundariesDocumentsSts1EventsRow()
    {
        // patch-boundaries.md must have a StS1Events high-risk owner row
        // with forbidden behavior, manual evidence row, and source guard description.
        var source = ReadRepoText("docs", "architecture", "patch-boundaries.md");

        AssertSourceContains(source,
            "StS1Events",
            "Sts1EventRegistrationService",
            "Sts1EventsFeatureModule",
            "No unconditional `RegisterAll` in default init path",
            "Default Off loads 0 StS1 events",
            "CanaryOnly registers exactly 4");
    }

    [Fact]
    public void RegisterAllTotalRegistrationCallsIs54()
    {
        // 17 shared × 1 + 7 Act1 × 2 + 14 Act2 × 1 + 9 Act3 × 1 = 54 total registration calls.
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationService.cs");
        var registerAllMethod = SliceBetween(source, "public static void RegisterAll(string modId)", "content.Apply();");
        var totalRegistrations = CountOccurrences(registerAllMethod, "content.ActEvent<") + CountOccurrences(registerAllMethod, "content.SharedEvent<");
        Assert.Equal(54, totalRegistrations);
    }

    [Fact]
    public void CanaryOnlyEventsAreHardcodedNotDynamic()
    {
        // CanaryOnly must use hardcoded SharedEvent calls, not a loop or dynamic list.
        // This prevents TODO/BLOCKED events from accidentally entering safe modes.
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationService.cs");
        var canaryMethod = SliceBetween(source, "RegisterCanaryOnly(string modId)", "content.Apply()");

        // Must not contain any dynamic registration patterns
        Assert.DoesNotContain("foreach", canaryMethod, StringComparison.Ordinal);
        Assert.DoesNotContain("RegisterAll", canaryMethod, StringComparison.Ordinal);
    }

    [Fact]
    public void UnsafeModesAreDocumentedAsDevOnly()
    {
        // AdditiveAllDraft and ReplaceUnknownEventsPrototype must be documented as unsafe/dev-only.
        var source = ReadRepoText("docs", "issues", "ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md");

        AssertSourceContains(source,
            "Unsafe / dev-only",
            "AdditiveAllDraft",
            "ReplaceUnknownEventsPrototype");
    }

    [Fact]
    public void IssueDocumentsModeSafetyMatrix()
    {
        // The Sts1Events issue must document all 5 modes with risk levels.
        var source = ReadRepoText("docs", "issues", "ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md");

        AssertSourceContains(source,
            "Off",
            "CanaryOnly",
            "AdditiveBatch1",
            "AdditiveAllDraft",
            "ReplaceUnknownEventsPrototype",
            "Safe",
            "Controlled",
            "Unsafe");
    }

    [Fact]
    public void ReplacementPrototypeSourceExistsWithCorrectStructure()
    {
        // The ReplacementPrototype Harmony patch must exist and be gated behind #if REPLACEMENT_PROTOTYPE_ENABLED.
        var filePath = Path.Combine(Root, "EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1ReplacementPrototype.cs");
        Assert.True(File.Exists(filePath), "Sts1ReplacementPrototype.cs not found in Runtime directory.");

        var source = File.ReadAllText(filePath);
        AssertSourceContains(source,
            "#if REPLACEMENT_PROTOTYPE_ENABLED",
            "HarmonyPatch",
            "GenerateRooms",
            "Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype",
            "#endif");
    }
}
