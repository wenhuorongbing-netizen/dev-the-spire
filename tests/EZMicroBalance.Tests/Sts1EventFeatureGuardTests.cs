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
    public void RegisterCanaryOnlyRegistersExactlyFourCanaryEventTypes()
    {
        var source = ReadSts1RuntimeSources();

        var canaryMethod = SliceBetween(source, "RegisterCanaryOnly(string modId)", "content.Apply()");
        AssertSourceContains(canaryMethod,
            "content.ActEvent<Overgrowth, Sts1BigFish>()",
            "content.ActEvent<Underdocks, Sts1BigFish>()",
            "content.ActEvent<Overgrowth, Sts1GoldenIdol>()",
            "content.ActEvent<Underdocks, Sts1GoldenIdol>()",
            "content.SharedEvent<Sts1TheLab>()",
            "content.SharedEvent<Sts1DivineFountain>()");

        var totalRegistrations = CountOccurrences(canaryMethod, "content.ActEvent<") + CountOccurrences(canaryMethod, "content.SharedEvent<");
        Assert.Equal(6, totalRegistrations);
    }

    [Fact]
    public void RegisterAdditiveBatch1RegistersOnlyVerifiedScope()
    {
        var source = ReadSts1RuntimeSources();

        var batchMethod = SliceBetween(source, "RegisterAdditiveBatch1(string modId)", "content.Apply()");
        AssertSourceContains(batchMethod,
            "content.ActEvent<Overgrowth, Sts1BigFish>()",
            "content.ActEvent<Underdocks, Sts1BigFish>()",
            "content.ActEvent<Overgrowth, Sts1GoldenIdol>()",
            "content.ActEvent<Underdocks, Sts1GoldenIdol>()",
            "content.SharedEvent<Sts1TheLab>()",
            "content.SharedEvent<Sts1DivineFountain>()",
            "content.SharedEvent<Sts1Purifier>()",
            "content.ActEvent<Glory, Sts1UpgradeShrine>()",
            "content.SharedEvent<Sts1GoldenShrine>()",
            "content.ActEvent<Overgrowth, Sts1TheCleric>()",
            "content.ActEvent<Underdocks, Sts1TheCleric>()",
            "content.SharedEvent<Sts1OldBeggar>()",
            "content.ActEvent<Overgrowth, Sts1ShiningLight>()",
            "content.ActEvent<Underdocks, Sts1ShiningLight>()");

        var totalRegistrations = CountOccurrences(batchMethod, "content.ActEvent<") + CountOccurrences(batchMethod, "content.SharedEvent<");
        Assert.Equal(14, totalRegistrations);
        Assert.DoesNotContain("RegisterAll", batchMethod, StringComparison.Ordinal);
    }

    [Fact]
    public void ActMappingUsesOvergrowthAndUnderdocksForAct1()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(source,
            "ActEvent<Overgrowth, Sts1BigFish>()",
            "ActEvent<Underdocks, Sts1BigFish>()",
            "ActEvent<Overgrowth, Sts1GoldenIdol>()",
            "ActEvent<Underdocks, Sts1GoldenIdol>()",
            "ActEvent<Overgrowth, Sts1TheCleric>()",
            "ActEvent<Underdocks, Sts1TheCleric>()",
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
            "new(\"sts1_big_fish\", \"Big Fish\", Sts1EventPhase.Canary, Sts1EventAct.Act1)",
            "new(\"sts1_golden_idol\", \"Golden Idol\", Sts1EventPhase.Canary, Sts1EventAct.Act1)",
            "new(\"sts1_the_lab\", \"The Lab\", Sts1EventPhase.Canary, Sts1EventAct.Shared)",
            "new(\"sts1_divine_fountain\", \"Divine Fountain\", Sts1EventPhase.Canary, Sts1EventAct.Shared)");

        Assert.Equal(4, CountOccurrences(entriesBlock, "Sts1EventPhase.Canary"));
    }

    [Fact]
    public void DivineFountainRequiresEveryPlayerToHaveACurseAndUsesDrinkOption()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1DivineFountain.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "public override bool IsAllowed(IRunState runState)",
            "foreach (var player in runState.Players)",
            "if (!HasCurse(player))",
            "return runState.Players.Count > 0;",
            "private static bool HasCurse(Player player)",
            "card.Type == CardType.Curse",
            "new EventOption(this, Drink, InitialOptionKey(\"DRINK\"))",
            "private async Task Drink()",
            "STS1_DIVINE_FOUNTAIN.pages.DRINK.description");

        AssertSourceContains(engLoc,
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.title\": \"Drink\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.description\": \"Remove all Curses from your deck.\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.DRINK.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.title\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.description\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.DRINK.description\"");

        Assert.DoesNotContain("InitialOptionKey(\"PRAY\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_DIVINE_FOUNTAIN.pages.PRAY", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_DIVINE_FOUNTAIN.pages.PRAY", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void BigFishUsesBoxOptionName()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1BigFish.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Box, InitialOptionKey(\"BOX\"))",
            "private async Task Box()",
            "STS1_BIG_FISH.pages.BOX.description");

        AssertSourceContains(engLoc,
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.title\"",
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.description\"",
            "\"STS1_BIG_FISH.pages.BOX.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.title\"",
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.description\"",
            "\"STS1_BIG_FISH.pages.BOX.description\"");

        Assert.DoesNotContain("SHOE", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Shoe", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.INITIAL.options.SHOE", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.SHOE.description", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.INITIAL.options.SHOE", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.SHOE.description", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void GoldenIdolTrapOptionsUseWikiBranchNames()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1GoldenIdol.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Outrun,",
            "OptionKey(\"TRAP\", \"OUTRUN\")",
            "new EventOption(this, () => Smash(smashDamage),",
            "OptionKey(\"TRAP\", \"SMASH\")",
            "new EventOption(this, () => Hide(hideMaxHpLoss),",
            "OptionKey(\"TRAP\", \"HIDE\")",
            "private const decimal HideMaxHpPctNormal = 0.08m;",
            "private const decimal HideMaxHpPctA15 = 0.10m;");

        AssertSourceContains(engLoc,
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.title\": \"Outrun\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title\": \"Smash\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.title\": \"Hide\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.title\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.title\"");

        Assert.DoesNotContain("OptionKey(\"TRAP\", \"JUMP\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OptionKey(\"TRAP\", \"DESTROY\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void GoldenShrineUsesWikiGoldAndRegretOptions()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1GoldenShrine.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "private const int PrayGoldNormal = 100;",
            "private const int PrayGoldA15 = 50;",
            "private const int DesecrateGold = 275;",
            "new EventOption(this, Pray, InitialOptionKey(\"PRAY\"))",
            "new EventOption(this, Desecrate, InitialOptionKey(\"DESECRATE\"))",
            "await PlayerCmd.GainGold(PrayGoldAmount, owner);",
            "await PlayerCmd.GainGold(DesecrateGold, owner);",
            "ModelDb.Card<Regret>()",
            "STS1_GOLDEN_SHRINE.pages.PRAY.description");

        AssertSourceContains(engLoc,
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.title\": \"Pray\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.description\": \"Gain 100 gold. (50 on Ascension 15+.)\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.DESECRATE.description\": \"Gain 275 gold. Obtain Regret.\"",
            "\"STS1_GOLDEN_SHRINE.pages.PRAY.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.title\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.description\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.DESECRATE.description\"",
            "\"STS1_GOLDEN_SHRINE.pages.PRAY.description\"");

        Assert.DoesNotContain("TAKE_GOLD", source, StringComparison.Ordinal);
        Assert.DoesNotContain("TAKE_GOLD", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("TAKE_GOLD", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("RemoveAllCurses", source, StringComparison.Ordinal);
        Assert.DoesNotContain("hasCurses", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GoldNormal = 250", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GoldA15 = 100", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TheClericUsesA15PurifyCostAndGoldEligibility()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1TheCleric.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "private const int HealCost = 35;",
            "private const int PurifyCostNormal = 50;",
            "private const int PurifyCostA15 = 75;",
            "private int PurifyCost => HasA15 ? PurifyCostA15 : PurifyCostNormal;",
            "public override bool IsAllowed(IRunState runState)",
            "if (player.Gold < HealCost)",
            "return runState.Players.Count > 0;",
            "var canHeal = (Owner?.Gold ?? 0) >= HealCost;",
            "var canPurify = (Owner?.Gold ?? 0) >= PurifyCost;",
            "await PlayerCmd.LoseGold(HealCost, owner, GoldLossType.Spent);",
            "await PlayerCmd.LoseGold(PurifyCost, owner, GoldLossType.Spent);");

        AssertSourceContains(engLoc,
            "\"STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title\": \"Heal (35 gold)\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title\": \"Purify (50/75 gold)\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.description\": \"Pay 50 gold. (75 on Ascension 15+.) Remove a card from your deck.\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.description\"");

        Assert.DoesNotContain("private const int PurifyCost = 50;", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TheLabHasOnlyOpenOption()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1TheLab.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Open, InitialOptionKey(\"OPEN\"))",
            "int count = HasA15 ? 2 : 3;",
            "SetEventFinished(L10NLookup(\"STS1_THE_LAB.pages.OPEN.description\"))");

        Assert.Equal(1, CountOccurrences(source, "new EventOption(this,"));
        AssertSourceContains(engLoc, "\"STS1_THE_LAB.pages.INITIAL.options.OPEN.description\": \"Obtain 3 random potions. (2 on Ascension 15+.)\"");
        Assert.DoesNotContain("InitialOptionKey(\"LEAVE\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_THE_LAB.pages.INITIAL.options.LEAVE", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_THE_LAB.pages.INITIAL.options.LEAVE", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void OldBeggarOfferRequiresEnoughGold()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1OldBeggar.cs");

        AssertSourceContains(source,
            "private const int GoldCost = 75;",
            "var canOfferGold = (Owner?.Gold ?? 0) >= GoldCost;",
            "new EventOption(this, canOfferGold ? OfferGold : null, InitialOptionKey(\"OFFER_GOLD\"))",
            "await PlayerCmd.LoseGold(GoldCost, owner, GoldLossType.Spent);",
            "await Sts1EventHelpers.OpenCardRemoval(owner);");
    }

    [Fact]
    public void ShiningLightUpgradesRandomCardsWithoutManualSelection()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Act1", "Sts1ShiningLight.cs");
        var helpers = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventHelpers.cs");

        AssertSourceContains(source,
            "await Sts1EventHelpers.UpgradeRandomCards(owner, Rng, count: 2);");
        Assert.DoesNotContain("OpenCardUpgrade(owner, count: 2)", source, StringComparison.Ordinal);

        AssertSourceContains(helpers,
            "public static Task UpgradeRandomCards(Player owner, Rng rng, int count = 1)",
            ".Where(card => card.IsUpgradable)",
            ".TakeRandom(count, rng)",
            "CardCmd.Upgrade(cards, CardPreviewStyle.EventLayout);");
    }

    [Fact]
    public void AdditiveBatch1SimpleBatchSpecsExist()
    {
        foreach (var fileName in new[]
        {
            "purifier.md",
            "upgrade-shrine.md",
            "golden-shrine.md",
            "the-cleric.md",
            "old-beggar.md",
            "shining-light.md"
        })
        {
            var path = Path.Combine(Root, "docs", "features", "sts1-events", "event-specs", fileName);
            Assert.True(File.Exists(path), $"Missing AdditiveBatch1 simple-batch event spec: {fileName}");
        }
    }

    [Fact]
    public void RegisterAllSharedEventCountIs14()
    {
        // 14 SharedEvent calls in RegisterAll after Big Fish, Golden Idol, and The Cleric moved to Act1:
        // Golden Wing, Living Wall, Old Beggar, Purifier, Golden Shrine, Bonfire Spirits, Divine Fountain,
        // Fountain of Cleansing, The Lab, Face Trader, The Mausoleum, Designer, The Woman in Blue,
        // Wheel of Change. Sts1Duplicator is excluded from compilation.
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationService.cs");
        var registerAllMethod = SliceBetween(source, "public static void RegisterAll(string modId)", "content.Apply();");
        var sharedEventCount = CountOccurrences(registerAllMethod, "content.SharedEvent<");
        Assert.Equal(14, sharedEventCount);
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
    public void RegisterAllTotalRegistrationCallsIs57()
    {
        // 14 shared x 1 + 10 Act1 x 2 + 14 Act2 x 1 + 9 Act3 x 1 = 57 total registration calls.
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventRegistrationService.cs");
        var registerAllMethod = SliceBetween(source, "public static void RegisterAll(string modId)", "content.Apply();");
        var totalRegistrations = CountOccurrences(registerAllMethod, "content.ActEvent<") + CountOccurrences(registerAllMethod, "content.SharedEvent<");
        Assert.Equal(57, totalRegistrations);
    }

    [Fact]
    public void CanaryOnlyEventsAreHardcodedNotDynamic()
    {
        // CanaryOnly must use hardcoded registration calls, not a loop or dynamic list.
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
