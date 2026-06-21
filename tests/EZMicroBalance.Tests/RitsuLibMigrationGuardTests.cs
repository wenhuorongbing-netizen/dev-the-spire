using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

/// <summary>
/// Guards for RitsuLib patch migration integrity:
/// - PatchId uniqueness
/// - No double-patching (migrated class must not have [HarmonyPatch])
/// - Raw HarmonyPatch classes must not be registered in RegisterMigratedPatches
/// - Migration counts match docs
/// </summary>
public sealed class RitsuLibMigrationGuardTests
{
    // All PatchId strings registered in RitsuLibBootstrap.RegisterMigratedPatches().
    // Keep this list synchronized with the source.
    private static readonly string[] ExpectedMigratedPatchIds =
    [
        // FiddlePatches (4) — Batch 4a
        "fiddle-vars",
        "fiddle-hand-draw",
        "fiddle-should-draw",
        "fiddle-draw-cap",
        // ChoicesParadoxPatches (1) — Batch 4a
        "choices-paradox-turn-start",
        // DistinguishedCapePatches (3) — Batch 4a
        "distinguished-cape-vars",
        "distinguished-cape-event-option",
        "distinguished-cape-pickup",
        // BlackStarCompensationPatches (1) — Batch 4a
        "black-star-obtain",
        // CrossbowPatches (2) — Batch 4b
        "crossbow-offer",
        "crossbow-vanilla-after-turn",
        // BrightestFlameExhaustDrawPatch (3) — Batch 4b
        "brightest-flame-keywords",
        "brightest-flame-vars",
        "brightest-flame-exhaust-backstop",
        // DebtAndCardPatches (8) — Batch 4b
        "debt-after-created",
        "debt-from-save",
        "debt-keywords",
        "debt-vars",
        "debt-turn-end-effect",
        "debt-turn-end-in-hand",
        "card-model-on-play",
        "debt-exhaust",
        // SealOfGoldPatches (2) — Batch 4b
        "seal-of-gold-max-energy",
        "seal-of-gold-turn",
        // PickupRewardPatches (1) — Batch 4b
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
    /// All PatchId values registered in RegisterMigratedPatches must be unique.
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
    /// Patch classes registered in RegisterMigratedPatches must NOT have
    /// class-level [HarmonyPatch] attributes. If they did, Harmony.PatchAll()
    /// would pick them up again, causing double-patching.
    /// </summary>
    [Fact]
    public void MigratedPatchClassesHaveNoHarmonyPatchAttribute()
    {
        var bootstrap = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibBootstrap.cs");

        // Extract all RegisterPatch<T>() calls to get the class names
        var registerPatchPattern = new Regex(@"patcher\.RegisterPatch<(\w+)>\(\)");
        var migratedClassNames = registerPatchPattern.Matches(bootstrap)
            .Select(m => m.Groups[1].Value)
            .ToArray();

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
                    $"Migrated class '{className}' in '{relativePath}' has [HarmonyPatch] attribute — would cause double-patching.");
            }
        }
    }

    /// <summary>
    /// Classes with [HarmonyPatch] attributes must NOT be registered in
    /// RegisterMigratedPatches. This is the inverse of the above check.
    /// </summary>
    [Fact]
    public void RawHarmonyPatchClassesAreNotMigrated()
    {
        var bootstrap = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibBootstrap.cs");

        // Extract all RegisterPatch<T>() calls
        var registerPatchPattern = new Regex(@"patcher\.RegisterPatch<(\w+)>\(\)");
        var migratedClassNames = new HashSet<string>(
            registerPatchPattern.Matches(bootstrap).Select(m => m.Groups[1].Value),
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
    /// The RegisterMigratedPatches call count in RitsuLibBootstrap.cs must match
    /// the expected total (25). This guards against source drift.
    /// </summary>
    [Fact]
    public void RegisterMigratedPatchesCallCountMatchesSource()
    {
        var bootstrap = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibBootstrap.cs");
        var callCount = CountOccurrences(bootstrap, "patcher.RegisterPatch<");

        Assert.Equal(ExpectedTotalMigratedCount, callCount);
    }

    /// <summary>
    /// docs/migration.md must state the correct migrated counts:
    /// Batch 4a = 9, Batch 4b = 16, Total = 25.
    /// DebtAndCardPatches row must say 8 classes.
    /// </summary>
    [Fact]
    public void MigrationDocCountsMatchSource()
    {
        var migrationDoc = ReadRepoText("docs", "migration.md");

        // Batch 4a header should say 9
        Assert.Contains("Migrated 9 low-risk patch classes", migrationDoc, StringComparison.Ordinal);

        // DebtAndCardPatches row should say 8
        Assert.Contains("| `DebtAndCardPatches.cs` | 8 |", migrationDoc, StringComparison.Ordinal);

        // Total migrated line
        Assert.Contains("Total migrated:** 25 classes (9 from Batch 4a + 16 from Batch 4b)", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("**Remaining:** 146 `[HarmonyPatch]` declarations still on raw Harmony.", migrationDoc, StringComparison.Ordinal);
        Assert.Contains(
            "Inventory rechecked on 2026-06-20 against the current source tree: 25 migrated `IPatchMethod` classes, 146 raw `[HarmonyPatch]` declarations, and 171 tracked patch units.",
            migrationDoc,
            StringComparison.Ordinal);

        // Should NOT contain the old wrong values
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
        var migrationDoc = ReadRepoText("docs", "migration.md");
        var inventory = ReadRepoText("docs", "patch-inventory.md");
        var bootstrap = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "RitsuLibBootstrap.cs");

        Assert.Contains("Status: proposal only. Do not migrate these patches without explicit owner approval.", proposal, StringComparison.Ordinal);
        Assert.Contains("Candidate count is 10", proposal, StringComparison.Ordinal);
        Assert.Contains("Before any Batch 4c source migration:", proposal, StringComparison.Ordinal);
        Assert.Contains("Owner accepts this exact candidate list or a smaller subset.", proposal, StringComparison.Ordinal);
        Assert.Contains("Previous `v0.107.1` beta.93 AdditiveBatch1 loader/registration proof is clean, but this proposal is not a substitute", proposal, StringComparison.Ordinal);
        Assert.Contains("retained current AdditiveBatch1 10 event types / 14 registration-line smoke with retained verifier reports and add the missing gameplay evidence", proposal, StringComparison.Ordinal);
        Assert.Contains("Batch 4c may be reviewed as a low-risk candidate proposal only; do not migrate Batch 4c", migrationDoc, StringComparison.Ordinal);

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
            Assert.DoesNotContain($"RegisterPatch<{candidateClass}>", bootstrap, StringComparison.Ordinal);

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
        Assert.Contains("installed beta.96 package parity passes", proposal, StringComparison.Ordinal);
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
            "GitHub release `v0.4.31` is marked Latest",
            "NuGet flat-container includes `0.4.31`",
            "Nexus files now list the variant-pack main file as `0.4.31`",
            "the earlier Nexus `0.4.28` lag is historical only",
            "GitHub latest-release/tag, NuGet, Nexus, and installed variant pack",
            "`main` branch manifest can lag",
            "source of truth for the Spire Plus dependency floor",
            "Keep Spire Plus on stable `0.4.31`, not a dev build");

        AssertSourceContains(
            integrationDoc,
            "## External Version Recheck",
            "RitsuLib GitHub release `v0.4.31` is marked Latest",
            "GitHub latest-release API and the `v0.4.31` tag remain the GitHub version",
            "the main branch manifest is not the dependency-floor source",
            "NuGet package",
            "Nexus files page now lists the variant-pack main",
            "the earlier Nexus `0.4.28` lag is historical only",
            "Major Update #2",
            "`v0.107.1`",
            "Workshop and RNG-system changes are dependency-sensitive",
            "rerun the source-workspace checker",
            "variant check before claiming compatibility");
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
}
