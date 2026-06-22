using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

/// <summary>
/// Guards for RitsuLib patch migration integrity:
/// - PatchId uniqueness
/// - No double-patching (migrated class must not have [HarmonyPatch])
/// - Raw HarmonyPatch classes must not be registered in the migrated patch registry
/// - Migration counts match docs
/// </summary>
public sealed class RitsuLibMigrationGuardTests
{
    // All PatchId strings registered in SpirePlusMigratedPatchRegistry.RegisterAll().
    // Keep this list synchronized with the source.
    private static readonly string[] ExpectedMigratedPatchIds =
    [
        // FiddlePatches (4) - Batch 4a
        "fiddle-vars",
        "fiddle-hand-draw",
        "fiddle-should-draw",
        "fiddle-draw-cap",
        // ChoicesParadoxPatches (1) - Batch 4a
        "choices-paradox-turn-start",
        // DistinguishedCapePatches (3) - Batch 4a
        "distinguished-cape-vars",
        "distinguished-cape-event-option",
        "distinguished-cape-pickup",
        // BlackStarCompensationPatches (1) - Batch 4a
        "black-star-obtain",
        // CrossbowPatches (2) - Batch 4b
        "crossbow-offer",
        "crossbow-vanilla-after-turn",
        // BrightestFlameExhaustDrawPatch (3) - Batch 4b
        "brightest-flame-keywords",
        "brightest-flame-vars",
        "brightest-flame-exhaust-backstop",
        // DebtAndCardPatches (8) - Batch 4b
        "debt-after-created",
        "debt-from-save",
        "debt-keywords",
        "debt-vars",
        "debt-turn-end-effect",
        "debt-turn-end-in-hand",
        "card-model-on-play",
        "debt-exhaust",
        // SealOfGoldPatches (2) - Batch 4b
        "seal-of-gold-max-energy",
        "seal-of-gold-turn",
        // PickupRewardPatches (1) - Batch 4b
        "ancient-pickup-balance"
    ];

    private const int ExpectedBatch4aCount = 9;
    private const int ExpectedBatch4bCount = 16;
    private const int ExpectedTotalMigratedCount = 25;
    private const int ExpectedRawHarmonyPatchDeclarationCount = 146;

    private static readonly string[] ExpectedBatch4cCandidateClasses =
    [
        "AscensionLocalizationLocStringRawTextPatch",
        "AscensionLocalizationGetTablePatch",
        "AscensionLocalizationRawTextPatch",
        "AscensionLocalizationLocStringPatch",
        "AscensionLocalizationHasEntryPatch",
        "AscensionLocalizationIsLocalKeyPatch",
        "SereTalonAncientEventOptionButtonPatch",
        "SereTalonRelicNodeReloadPatch",
        "CombatHandInputSafetyPatch",
        "CrystalSpherePeekFinishedPatch"
    ];

    private static readonly string[] ExpectedBatch4cInventoryRows =
    [
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 6 | `[HarmonyPatch(typeof(LocString), nameof(LocString.GetRawText))]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 26 | `[HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 38 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 59 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 80 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | 92 | `[HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))]` |",
        "| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 71 | `[HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]` |",
        "| Ancient reward rebalance | Low | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | 83 | `[HarmonyPatch(typeof(NRelic), \"Reload\")]` |",
        "| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/CombatHandInputSafetyPatches.cs` | 6 | `[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand._UnhandledInput))]` |",
        "| Preview tools | Low | `EZMicroBalanceCode/Preview/CrystalSpherePeekPatch.cs` | 98 | `[HarmonyPatch]` |"
    ];

    private static readonly string[] ForbiddenBatch4cMigrationCategories =
    [
        "run lifecycle",
        "save/load",
        "map generation",
        "multiplayer/lobby",
        "death",
        "A20 boss-flow",
        "reward-state"
    ];

    /// <summary>
    /// All PatchId values registered in SpirePlusMigratedPatchRegistry must be unique.
    /// </summary>
    [Fact]
    public void MigratedPatchIdsAreUnique()
    {
        var duplicates = ExpectedMigratedPatchIds
            .GroupBy(id => id, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        Assert.True(duplicates.Length == 0,
            $"Duplicate PatchId values found: {string.Join(", ", duplicates)}");
    }

    /// <summary>
    /// The expected migrated patch count must be 25 (9 Batch 4a + 16 Batch 4b).
    /// </summary>
    [Fact]
    public void MigratedPatchCountMatchesExpected()
    {
        Assert.Equal(ExpectedTotalMigratedCount, ExpectedMigratedPatchIds.Length);
        Assert.Equal(ExpectedBatch4aCount + ExpectedBatch4bCount, ExpectedTotalMigratedCount);
    }

    /// <summary>
    /// Patch classes registered in the migrated patch registry must NOT have
    /// class-level [HarmonyPatch] attributes. If they did, Harmony.PatchAll()
    /// would pick them up again, causing double-patching.
    /// </summary>
    [Fact]
    public void MigratedPatchClassesHaveNoHarmonyPatchAttribute()
    {
        var migratedClassNames = ReadMigratedPatchClassNames();

        Assert.True(migratedClassNames.Length == ExpectedTotalMigratedCount,
            $"Expected {ExpectedTotalMigratedCount} RegisterPatch calls, found {migratedClassNames.Length}");

        // Scan all source files for these class names with [HarmonyPatch]
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);

        foreach (var sourceFile in sourceFiles)
        {
            var source = File.ReadAllText(sourceFile);
            var relativePath = ToRepoRelativePath(sourceFile);

            foreach (var className in migratedClassNames)
            {
                // Check for [HarmonyPatch] on the same class declaration
                // Pattern: [HarmonyPatch...] ... class ClassName
                var classPattern = new Regex(
                    @"\[HarmonyPatch[^\]]*\][\s\S]*?class\s+" + Regex.Escape(className) + @"\b");

                Assert.False(classPattern.IsMatch(source),
                    $"Migrated class '{className}' in '{relativePath}' has [HarmonyPatch] attribute - would cause double-patching.");
            }
        }
    }

    /// <summary>
    /// Classes with [HarmonyPatch] attributes must NOT be registered in
    /// SpirePlusMigratedPatchRegistry. This is the inverse of the above check.
    /// </summary>
    [Fact]
    public void RawHarmonyPatchClassesAreNotMigrated()
    {
        var migratedClassNames = new HashSet<string>(
            ReadMigratedPatchClassNames(),
            StringComparer.Ordinal);

        // Find all classes with [HarmonyPatch] attribute
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);

        var conflicts = new List<string>();

        foreach (var sourceFile in sourceFiles)
        {
            var source = File.ReadAllText(sourceFile);
            var relativePath = ToRepoRelativePath(sourceFile);

            // Match [HarmonyPatch] followed by class ClassName
            var classPattern = new Regex(
                @"\[HarmonyPatch[^\]]*\][\s\S]*?class\s+(\w+)");
            foreach (Match match in classPattern.Matches(source))
            {
                var className = match.Groups[1].Value;
                if (migratedClassNames.Contains(className))
                {
                    conflicts.Add($"{className} in {relativePath}");
                }
            }
        }

        Assert.True(conflicts.Count == 0,
            $"Classes with [HarmonyPatch] that are also registered in RegisterMigratedPatches (double-patch risk):{Environment.NewLine}" +
            string.Join(Environment.NewLine, conflicts));
    }

    /// <summary>
    /// The migrated patch registry call count must match
    /// the expected total (25). This guards against source drift.
    /// </summary>
    [Fact]
    public void MigratedPatchRegistryCallCountMatchesSource()
    {
        var registrationSource = ReadRitsuLibIntegrationSource();
        var callCount = CountOccurrences(registrationSource, ".RegisterPatch<");

        Assert.Equal(ExpectedTotalMigratedCount, callCount);
    }

    [Fact]
    public void RitsuLibBootstrapDelegatesMigratedPatchRegistration()
    {
        var bootstrap = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibBootstrap.cs");
        var registry = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "SpirePlusMigratedPatchRegistry.cs");

        Assert.Contains("SpirePlusMigratedPatchRegistry.RegisterAll(patcher);", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain(".RegisterPatch<", bootstrap, StringComparison.Ordinal);
        AssertSourceContains(
            registry,
            "internal static class SpirePlusMigratedPatchRegistry",
            "public static void RegisterAll(ModPatcher patcher)",
            "RegisterBatch4a(patcher);",
            "RegisterBatch4b(patcher);");
    }

    [Fact]
    public void RitsuLibContentRegistrationIsSplitByContentKind()
    {
        var orchestration = ReadRepoText(
            "EZMicroBalanceCode",
            "Core",
            "Integrations",
            "RitsuLib",
            "SpirePlusContentRegistrationService.cs");

        AssertSourceContains(
            orchestration,
            "internal static partial class SpirePlusContentRegistrationService",
            "RitsuLibFramework.CreateContentPack(modId)",
            "RegisterAncients(content);",
            "RegisterVakuuEncounter(content);",
            "RegisterCards(content);",
            "RegisterRelics(content);",
            "RegisterPowers(content);",
            "RegisterEnchantments(content);",
            "content.Apply();");
        Assert.DoesNotContain("content.Card<", orchestration, StringComparison.Ordinal);
        Assert.DoesNotContain("content.Relic<", orchestration, StringComparison.Ordinal);
        Assert.DoesNotContain("content.Power<", orchestration, StringComparison.Ordinal);
        Assert.DoesNotContain("content.Enchantment<", orchestration, StringComparison.Ordinal);

        foreach (var fileName in new[]
        {
            "SpirePlusContentRegistrationService.Ancients.cs",
            "SpirePlusContentRegistrationService.Cards.cs",
            "SpirePlusContentRegistrationService.Relics.cs",
            "SpirePlusContentRegistrationService.Powers.cs",
            "SpirePlusContentRegistrationService.Enchantments.cs"
        })
        {
            var partial = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", fileName);
            Assert.Contains("internal static partial class SpirePlusContentRegistrationService", partial, StringComparison.Ordinal);
            Assert.Contains("ModContentPackBuilder content", partial, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MigrationStatusStubRoutesInventoryToCanonicalDocs()
    {
        var migrationDoc = ReadRepoText("docs", "migration.md");

        Assert.Contains("# RitsuLib Migration Status Stub", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("This file stays only as a compatibility link", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Do not add migration tables", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("`docs/features/ritsulib-migration/README.md`", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("`docs/goals/migration.md`", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("`docs/integrations/ritsulib.md`", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("`docs/patch-inventory.md`", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Current boundary: Spire Plus is RitsuLib-only for beta.105", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Batch 4c and any higher-risk patch migration remain proposal-only", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("## Migrated Patch Inventory", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("| File | Classes | PatchIds |", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("| `DebtAndCardPatches.cs` |", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("Total migrated:**", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("**Remaining:**", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("Migrated 10 low-risk", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("Total migrated:** 26 classes", migrationDoc, StringComparison.Ordinal);
    }

    [Fact]
    public void RawHarmonyPatchInventoryCountMatchesMigrationDoc()
    {
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);
        var rawHarmonyPatchDeclarationCount = sourceFiles
            .SelectMany(File.ReadLines)
            .Count(line => line.Contains("[HarmonyPatch", StringComparison.Ordinal));

        Assert.Equal(ExpectedRawHarmonyPatchDeclarationCount, rawHarmonyPatchDeclarationCount);
    }

    [Fact]
    public void Batch4cCandidatesRemainProposalOnly()
    {
        var proposal = ReadRepoText("docs", "features", "ritsulib-migration", "batch-4c-candidates.md");
        var migrationReadme = ReadRepoText("docs", "features", "ritsulib-migration", "README.md");
        var inventory = ReadRepoText("docs", "patch-inventory.md");
        var registrationSource = ReadRitsuLibIntegrationSource();

        Assert.Contains("Status: proposal only. Do not migrate these patches without explicit owner approval.", proposal, StringComparison.Ordinal);
        Assert.Contains("Candidate count is 10", proposal, StringComparison.Ordinal);
        Assert.Contains("Before any Batch 4c source migration:", proposal, StringComparison.Ordinal);
        Assert.Contains("Owner accepts this exact candidate list or a smaller subset.", proposal, StringComparison.Ordinal);
        Assert.Contains("Previous `v0.107.1` beta.93 AdditiveBatch1 loader/registration proof is clean, but this proposal is not a substitute", proposal, StringComparison.Ordinal);
        Assert.Contains("retained current AdditiveBatch1 10 event types / 14 registration-line smoke with retained verifier reports and add the missing gameplay evidence", proposal, StringComparison.Ordinal);
        Assert.Contains("Do not migrate Batch 4c or high-risk run/map/reward/save/multiplayer patches", migrationReadme, StringComparison.Ordinal);

        var candidateSectionStart = proposal.IndexOf("## Candidates", StringComparison.Ordinal);
        var candidateSectionEnd = proposal.IndexOf("## Per-Candidate Evidence", StringComparison.Ordinal);
        var candidateSection = proposal[candidateSectionStart..candidateSectionEnd];
        var candidateRows = Regex.Matches(candidateSection, @"^\| \d+ \|", RegexOptions.Multiline);
        Assert.Equal(ExpectedBatch4cCandidateClasses.Length, candidateRows.Count);

        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);
        var allSource = string.Join(
            Environment.NewLine,
            sourceFiles.Select(path => File.ReadAllText(path)));

        foreach (var candidateClass in ExpectedBatch4cCandidateClasses)
        {
            Assert.Contains(candidateClass, proposal, StringComparison.Ordinal);
            Assert.DoesNotContain($"RegisterPatch<{candidateClass}>", registrationSource, StringComparison.Ordinal);

            var classPattern = new Regex(
                @"\[HarmonyPatch[^\]]*\]\s*(?:\r?\n\s*\[[^\]]+\]\s*)*(?:internal\s+)?(?:static\s+)?(?:partial\s+)?class\s+" +
                Regex.Escape(candidateClass) +
                @"\b");
            Assert.True(
                classPattern.IsMatch(allSource),
                $"Batch 4c candidate '{candidateClass}' must remain a raw HarmonyPatch until owner approval and runtime validation are complete.");
        }

        foreach (var inventoryRow in ExpectedBatch4cInventoryRows)
        {
            Assert.Contains(inventoryRow, inventory, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Batch4cStaticReviewKeepsOwnerDecisionOpen()
    {
        var proposal = ReadRepoText("docs", "features", "ritsulib-migration", "batch-4c-candidates.md");
        var nextRun = ReadRepoText("docs", "features", "ritsulib-migration", "next-overnight-run.md");
        var goal = ReadRepoText("docs", "goals", "migration.md");

        Assert.Contains("Static review recaptured: 2026-06-18", proposal, StringComparison.Ordinal);
        Assert.Contains("Dependency gate refreshed: 2026-06-21", proposal, StringComparison.Ordinal);
        Assert.Contains("Checked: 2026-06-18.", proposal, StringComparison.Ordinal);
        Assert.Contains("Dependency gate checked: 2026-06-21.", proposal, StringComparison.Ordinal);
        Assert.Contains(
            "This recapture was static governance only: no source migration, package refresh, loader smoke, gameplay proof, or owner approval was performed.",
            proposal,
            StringComparison.Ordinal);
        Assert.Contains("installed beta.105 package parity and clicked Ancient UI smoke pass", proposal, StringComparison.Ordinal);
        Assert.DoesNotContain("installed beta.87 package parity passes", proposal, StringComparison.Ordinal);
        Assert.DoesNotContain("installed beta.86 package parity passes", proposal, StringComparison.Ordinal);
        Assert.Contains("Current accepted no-build test lanes pass with 0 failures.", proposal, StringComparison.Ordinal);
        Assert.Contains("use the documented split lanes instead of treating runner instability as a source failure", proposal, StringComparison.Ordinal);

        Assert.Contains(
            "the 2026-06-18 static recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed.",
            nextRun,
            StringComparison.Ordinal);
        Assert.Contains("The current static recapture is not that decision.", nextRun, StringComparison.Ordinal);
        Assert.Contains("- [x] Batch 4c candidate list static review recaptured: 10 low-risk candidates, no forbidden high-risk categories, and no migration performed.", nextRun, StringComparison.Ordinal);
        Assert.Contains("- [ ] Batch 4c owner decision recorded.", nextRun, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Batch 4c owner decision recorded.", nextRun, StringComparison.Ordinal);

        Assert.Contains(
            "| Batch 4c migration | Proposal only / static review recaptured | 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration work. |",
            goal,
            StringComparison.Ordinal);
        Assert.Contains(
            "Record an owner decision for Batch 4c. The candidate list has static-review coverage; do not migrate unless the owner approves the scope.",
            goal,
            StringComparison.Ordinal);

        foreach (var forbiddenCategory in ForbiddenBatch4cMigrationCategories)
        {
            Assert.Contains(forbiddenCategory, proposal, StringComparison.Ordinal);
            Assert.Contains(forbiddenCategory, nextRun, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void UpstreamVersionResearchKeepsStableRitsuLibTarget()
    {
        var migrationGoal = ReadRepoText("docs", "goals", "migration.md");
        var integrationDoc = ReadRepoText("docs", "integrations", "ritsulib.md");

        AssertSourceContains(
            migrationGoal,
            "2026-06-22: NuGet flat-container and `dotnet list package --outdated --include-transitive` show `STS2.RitsuLib` `0.4.33` as the latest package",
            "The flat-container index lists 164 versions and ends at `0.4.33`",
            "found no `STS2.RitsuLib` update",
            "Nexus files list the variant-pack main file as `0.4.33`",
            "The GitHub release page/API can lag the NuGet/Nexus package version",
            "do not use a lagging GitHub release marker as the dependency-floor source",
            "official NuGet package via `RitsuLibDeployDir`",
            "dependency-floor source",
            "Keep Spire Plus on stable `0.4.33`, not a dev build");

        AssertSourceContains(
            integrationDoc,
            "## External Version Recheck",
            "2026-06-22 recheck",
            "`dotnet list EZMicroBalance.csproj package --outdated --include-transitive`",
            "found no `STS2.RitsuLib` update",
            "The NuGet flat-container index reports `STS2.RitsuLib` latest `0.4.33`",
            "across 164 listed versions",
            "GitHub releases can lag those package channels",
            "the main branch manifest is not the dependency-floor source",
            "NuGet package",
            "Nexus files page",
            "GitHub is not the dependency-floor source",
            "Major Update #2",
            "`v0.107.1`",
            "Workshop and RNG-system changes are dependency-sensitive",
            "rerun the source-workspace checker",
            "RitsuLib variant check before claiming compatibility");
    }

    /// <summary>
    /// docs/patch-inventory.md must list the migrated patches section and
    /// state the correct total migrated count (25).
    /// </summary>
    [Fact]
    public void PatchInventoryDocListsMigratedPatches()
    {
        var inventory = ReadRepoText("docs", "patch-inventory.md");

        Assert.Contains("Migrated to RitsuLib ModPatcher | 25", inventory, StringComparison.Ordinal);
        Assert.Contains("## Migrated Patches (RitsuLib ModPatcher)", inventory, StringComparison.Ordinal);
        Assert.Contains("## Raw HarmonyPatch Declarations (Unmigrated)", inventory, StringComparison.Ordinal);
    }

    [Fact]
    public void Sts1EventRegistrationCommentsStayRitsuLibOnlyAndReadable()
    {
        var registration = string.Join(
            Environment.NewLine,
            Directory.GetFiles(
                    RepoPath("EZMicroBalanceCode", "Sts1Events", "Runtime"),
                    "Sts1EventRegistrationService*.cs")
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path)));

        AssertSourceContains(
            registration,
            "Registers StS1 events through RitsuLib content packs.",
            "Keep this service on RitsuLib APIs only.",
            "new event batches should extend the mode-specific partial registration",
            "StS1 Act 1 events -> Overgrowth + Underdocks",
            "StS1 Act 2 events -> Hive",
            "StS1 Act 3 events -> Glory",
            "RitsuLibFramework.CreateContentPack(modId)",
            "content.Apply();");
        Assert.DoesNotContain(string.Concat("ModContent", "Registry"), registration, StringComparison.Ordinal);

        var nonAscii = registration
            .Select((ch, index) => new { Character = ch, Index = index })
            .Where(item => item.Character > 127)
            .ToArray();

        Assert.True(
            nonAscii.Length == 0,
            "StS1 RitsuLib registration comments/source should stay ASCII-readable; found non-ASCII code points at indexes: "
            + string.Join(", ", nonAscii.Take(10).Select(item => item.Index)));
    }

    /// <summary>
    /// All 25 migrated PatchId strings from ExpectedMigratedPatchIds must appear
    /// in RitsuLibBootstrap.cs as IPatchMethod.PatchId implementations.
    /// </summary>
    [Fact]
    public void AllExpectedPatchIdsAppearInSource()
    {
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);
        var allSource = string.Join(
            Environment.NewLine,
            sourceFiles.Select(f => File.ReadAllText(f)));

        foreach (var patchId in ExpectedMigratedPatchIds)
        {
            Assert.Contains($"\"{patchId}\"", allSource, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ModPatcherTargetsDoNotUseCompilerGetterNames()
    {
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);

        var offenders = sourceFiles
            .SelectMany(file => File.ReadLines(file)
                .Select((line, index) => new
                {
                    Path = ToRepoRelativePath(file),
                    LineNumber = index + 1,
                    Line = line.Trim()
                }))
            .Where(entry => entry.Line.Contains("ModPatchTarget(", StringComparison.Ordinal) &&
                            entry.Line.Contains("\"get_", StringComparison.Ordinal))
            .Select(entry => $"{entry.Path}:{entry.LineNumber}: {entry.Line}")
            .ToArray();

        Assert.True(offenders.Length == 0,
            "ModPatcher property getter targets must use the property name with MethodType.Getter, not compiler get_* names:" +
            Environment.NewLine + string.Join(Environment.NewLine, offenders));
    }

    private static string ReadRitsuLibIntegrationSource() =>
        ReadSourceTree("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib");

    private static string[] ReadMigratedPatchClassNames()
    {
        var registerPatchPattern = new Regex(@"\.RegisterPatch<(\w+)>\(\)");
        return registerPatchPattern.Matches(ReadRitsuLibIntegrationSource())
            .Select(m => m.Groups[1].Value)
            .ToArray();
    }
}
