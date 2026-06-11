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

        // Should NOT contain the old wrong values
        Assert.DoesNotContain("Migrated 10 low-risk", migrationDoc, StringComparison.Ordinal);
        Assert.DoesNotContain("Total migrated:** 26 classes", migrationDoc, StringComparison.Ordinal);
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
