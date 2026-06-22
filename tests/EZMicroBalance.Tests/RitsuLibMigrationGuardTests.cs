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
        "ancient-pickup-balance",
        // Clicked UI patches - owner-approved targeted migration
        "urda-option-relic-click",
        "urda-root-sight-map-point-ready",
        "urda-root-sight-map-refresh-state",
        "urda-root-sight-map-quest-icon-refresh",
        "urda-root-sight-map-point-click",
        "urda-root-sight-disabled-map-point-click",
        "urda-root-sight-map-close",
        "spire-plus-map-point-hover-composer",
        "ascension-map-marker-icon-refresh",
        "ascension-boss-map-point-hover",
        "sere-talon-event-option-button-ready",
        "sere-talon-relic-node-reload",
        "crystal-sphere-peek-ready",
        "crystal-sphere-peek-finished",
        "transform-preview-initialize",
        "transform-preview-cycle-display",
        // Visual/hover UI patches - getter-only presentation migration
        "sere-talon-icon-path",
        "sere-talon-packed-icon-path",
        "sere-talon-packed-icon-outline-path",
        "sere-talon-big-icon-path",
        "sere-talon-icon-texture",
        "sere-talon-icon-outline-texture",
        "sere-talon-big-icon-texture",
        "prismatic-gem-hover-tips",
        "prismatic-gem-hover-tips-excluding-relic",
        "jewelry-box-extra-hover-tips",
        "jewelry-box-hover-tips",
        "jewelry-box-hover-tips-excluding-relic",
        "prismatic-gem-reward-screen-hint",
        "ascension-a20-reward-screen-ready",
        "ascension-a20-reward-screen-state",
        "spire-plus-mod-info-localization",
        "combat-hand-input-safety",
        // Batch 4c localization fallback patches
        "ascension-localization-locstring-raw-text",
        "ascension-localization-get-table",
        "ascension-localization-raw-text",
        "ascension-localization-loc-string",
        "ascension-localization-has-entry",
        "ascension-localization-is-local-key"
    ];

    private const int ExpectedBatch4aCount = 9;
    private const int ExpectedBatch4bCount = 16;
    private const int ExpectedClickedUiCount = 21;
    private const int ExpectedVisualHoverUiCount = 12;
    private const int ExpectedBatch4cLocalizationCount = 6;
    private const int ExpectedTotalMigratedCount = 64;
    private const int ExpectedRawHarmonyPatchDeclarationCount = 107;

    private static readonly string[] ExpectedBatch4cLocalizationPatchClasses =
    [
        "AscensionLocalizationLocStringRawTextPatch",
        "AscensionLocalizationGetTablePatch",
        "AscensionLocalizationRawTextPatch",
        "AscensionLocalizationLocStringPatch",
        "AscensionLocalizationHasEntryPatch",
        "AscensionLocalizationIsLocalKeyPatch"
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
    /// The expected migrated patch count must be 64:
    /// 9 Batch 4a + 16 Batch 4b + 21 clicked/UI patches
    /// + 12 visual/hover UI patches + 6 Batch 4c localization patches.
    /// </summary>
    [Fact]
    public void MigratedPatchCountMatchesExpected()
    {
        Assert.Equal(ExpectedTotalMigratedCount, ExpectedMigratedPatchIds.Length);
        Assert.Equal(
            ExpectedBatch4aCount + ExpectedBatch4bCount + ExpectedClickedUiCount + ExpectedVisualHoverUiCount + ExpectedBatch4cLocalizationCount,
            ExpectedTotalMigratedCount);
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
                // Check only the attribute block directly attached to the
                // migrated class. Some files still contain raw Harmony classes
                // earlier in the same file while lower-risk classes migrate.
                var classPattern = new Regex(
                    @"(?m)(?<attributes>(?:^\s*\[[^\r\n]+\]\s*\r?\n)+)?^\s*(?:internal|public|private)?\s*(?:sealed\s+)?(?:static\s+)?(?:partial\s+)?class\s+" +
                    Regex.Escape(className) +
                    @"\b");
                var classMatch = classPattern.Match(source);

                if (!classMatch.Success)
                {
                    continue;
                }

                Assert.DoesNotContain(
                    "[HarmonyPatch",
                    classMatch.Groups["attributes"].Value,
                    StringComparison.Ordinal);
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
    /// the expected total. This guards against source drift.
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
            bootstrap,
            "ApplyMigratedRitsuLibPatches();",
            "ApplyLegacyHarmonyFallbackPatches();",
            "AuditRitsuLibRuntimeState();",
            "RitsuLib ModPatcher owns every migrated patch class",
            "fallback to another mod framework");
        AssertSourceContains(
            registry,
            "internal static class SpirePlusMigratedPatchRegistry",
            "public static void RegisterAll(ModPatcher patcher)",
            "RegisterBatch4a(patcher);",
            "RegisterBatch4b(patcher);",
            "RegisterClickedUiPatches(patcher);",
            "RegisterPatch<UrdaOptionRelicClickPatch>();",
            "RegisterPatch<UrdaRootSightMapQuestIconInputPatch>();",
            "RegisterPatch<UrdaRootSightMapPreviewIconPatch>();",
            "RegisterPatch<UrdaRootSightMapQuestIconPatch>();",
            "RegisterPatch<UrdaRootSightMapPointClickPatch>();",
            "RegisterPatch<UrdaRootSightDisabledMapPointClickPatch>();",
            "RegisterPatch<UrdaRootSightMapClosePatch>();",
            "RegisterMapUiPatches(patcher);",
            "RegisterPatch<SpirePlusMapPointHoverComposer>();",
            "RegisterPatch<FiremarkedEliteMapIconPatch>();",
            "RegisterPatch<BossMapPointHoverPatch>();",
            "RegisterSereTalonUiPatches(patcher);",
            "RegisterPatch<SereTalonAncientEventOptionButtonPatch>();",
            "RegisterPatch<SereTalonRelicNodeReloadPatch>();",
            "RegisterPreviewUiPatches(patcher);",
            "RegisterPatch<CrystalSpherePeekPatch>();",
            "RegisterPatch<CrystalSpherePeekFinishedPatch>();",
            "RegisterPatch<TransformPreviewInitializePatch>();",
            "RegisterPatch<TransformPreviewCyclePatch>();",
            "RegisterRelicVisualHoverPatches(patcher);",
            "RegisterPatch<SereTalonIconPathPatch>();",
            "RegisterPatch<SereTalonPackedIconPathPatch>();",
            "RegisterPatch<SereTalonPackedIconOutlinePathPatch>();",
            "RegisterPatch<SereTalonBigIconPathPatch>();",
            "RegisterPatch<SereTalonIconTexturePatch>();",
            "RegisterPatch<SereTalonIconOutlineTexturePatch>();",
            "RegisterPatch<SereTalonBigIconTexturePatch>();",
            "RegisterPatch<PrismaticGemHoverTipsPatch>();",
            "RegisterPatch<PrismaticGemHoverTipsExcludingRelicPatch>();",
            "RegisterPatch<JewelryBoxExtraHoverTipsPatch>();",
            "RegisterPatch<JewelryBoxHoverTipsPatch>();",
            "RegisterPatch<JewelryBoxHoverTipsExcludingRelicPatch>();",
            "RegisterRemainingUiPatches(patcher);",
            "RegisterPatch<PrismaticGemRewardScreenHintPatch>();",
            "RegisterPatch<AscensionA20RewardScreenReadyPatch>();",
            "RegisterPatch<AscensionA20RewardScreenStatePatch>();",
            "RegisterPatch<ModInfoLocalizationPatches>();",
            "RegisterPatch<CombatHandInputSafetyPatch>();",
            "RegisterBatch4cLocalizationPatches(patcher);",
            "RegisterPatch<AscensionLocalizationLocStringRawTextPatch>();",
            "RegisterPatch<AscensionLocalizationGetTablePatch>();",
            "RegisterPatch<AscensionLocalizationRawTextPatch>();",
            "RegisterPatch<AscensionLocalizationLocStringPatch>();",
            "RegisterPatch<AscensionLocalizationHasEntryPatch>();",
            "RegisterPatch<AscensionLocalizationIsLocalKeyPatch>();");
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
        Assert.Contains("Current boundary: Spire Plus is RitsuLib-only for beta.108", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Batch 4c localization fallback patches and the visual-hover UI getter batch", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Any higher-risk patch migration remains", migrationDoc, StringComparison.Ordinal);
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
    public void Batch4cLocalizationPatchesAreMigratedAndDocumented()
    {
        var record = ReadRepoText("docs", "features", "ritsulib-migration", "batch-4c-candidates.md");
        var migrationReadme = ReadRepoText("docs", "features", "ritsulib-migration", "README.md");
        var inventory = ReadRepoText("docs", "patch-inventory.md");
        var registrationSource = ReadRitsuLibIntegrationSource();

        Assert.Contains("Status: migrated localization fallback batch.", record, StringComparison.Ordinal);
        Assert.Contains("Migrated candidate count is 6", record, StringComparison.Ordinal);
        Assert.Contains("Owner decision recorded: 2026-06-22 continuation goal approved migrating the remaining six localization fallback candidates.", record, StringComparison.Ordinal);
        Assert.Contains("This migration is source/registration work only; it is not gameplay, save-load, co-op, release, or handoff proof.", record, StringComparison.Ordinal);
        Assert.Contains("Batch 4c localization fallback patches have moved to RitsuLib", migrationReadme, StringComparison.Ordinal);
        Assert.Contains("Do not migrate high-risk run/map/reward/save/multiplayer patches without explicit owner approval.", migrationReadme, StringComparison.Ordinal);

        var candidateSectionStart = record.IndexOf("## Migrated Candidates", StringComparison.Ordinal);
        var candidateSectionEnd = record.IndexOf("## Per-Candidate Evidence", StringComparison.Ordinal);
        var candidateSection = record[candidateSectionStart..candidateSectionEnd];
        var candidateRows = Regex.Matches(candidateSection, @"^\| \d+ \|", RegexOptions.Multiline);
        Assert.Equal(ExpectedBatch4cLocalizationPatchClasses.Length, candidateRows.Count);

        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);
        var allSource = string.Join(
            Environment.NewLine,
            sourceFiles.Select(path => File.ReadAllText(path)));

        foreach (var patchClass in ExpectedBatch4cLocalizationPatchClasses)
        {
            Assert.Contains(patchClass, record, StringComparison.Ordinal);
            Assert.Contains($"RegisterPatch<{patchClass}>", registrationSource, StringComparison.Ordinal);
            Assert.Contains($"class {patchClass} : IPatchMethod", allSource, StringComparison.Ordinal);

            var classPattern = new Regex(
                @"\[HarmonyPatch[^\]]*\]\s*(?:\r?\n\s*\[[^\]]+\]\s*)*(?:internal\s+)?(?:static\s+)?(?:partial\s+)?class\s+" +
                Regex.Escape(patchClass) +
                @"\b");
            Assert.False(
                classPattern.IsMatch(allSource),
                $"Batch 4c migrated patch '{patchClass}' must not keep a class-level [HarmonyPatch] attribute.");
        }

        Assert.Contains(
            "| `AscensionLocalizationTablePatches.cs` | 6 | `ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key` | 4c-localization |",
            inventory,
            StringComparison.Ordinal);
        Assert.DoesNotContain("| Ascension patches | Low | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs`", inventory, StringComparison.Ordinal);
    }

    [Fact]
    public void Batch4cStaticReviewRecordsOwnerDecisionAndKeepsHighRiskClosed()
    {
        var record = ReadRepoText("docs", "features", "ritsulib-migration", "batch-4c-candidates.md");
        var nextRun = ReadRepoText("docs", "features", "ritsulib-migration", "next-overnight-run.md");
        var goal = ReadRepoText("docs", "goals", "migration.md");

        Assert.Contains("Static review recaptured: 2026-06-18", record, StringComparison.Ordinal);
        Assert.Contains("Dependency gate refreshed: 2026-06-22", record, StringComparison.Ordinal);
        Assert.Contains("Checked: 2026-06-18.", record, StringComparison.Ordinal);
        Assert.Contains("Dependency gate checked: 2026-06-22.", record, StringComparison.Ordinal);
        Assert.Contains(
            "The 2026-06-18 recapture was static governance only; the 2026-06-22 continuation records owner approval for exactly the six localization fallback candidates.",
            record,
            StringComparison.Ordinal);
        Assert.Contains("installed beta.108 package parity passed; previous beta.107 clicked Ancient UI smoke applied the previous 46 migrated patch classes.", record, StringComparison.Ordinal);
        Assert.DoesNotContain("installed beta.87 package parity passes", record, StringComparison.Ordinal);
        Assert.DoesNotContain("installed beta.86 package parity passes", record, StringComparison.Ordinal);
        Assert.Contains("Current accepted no-build test lanes pass with 0 failures.", record, StringComparison.Ordinal);
        Assert.Contains("use the documented split lanes instead of treating runner instability as a source failure", record, StringComparison.Ordinal);

        Assert.Contains(
            "the 2026-06-22 continuation migrated the remaining 6 low-risk localization fallback candidates through RitsuLib after owner approval.",
            nextRun,
            StringComparison.Ordinal);
        Assert.Contains("This is not current enabled-mode, gameplay, save-load, replacement, co-op, QA, release, or handoff proof.", nextRun, StringComparison.Ordinal);
        Assert.Contains("- [x] Batch 4c localization owner decision recorded and implemented for the six fallback localization patches.", nextRun, StringComparison.Ordinal);
        Assert.DoesNotContain("- [ ] Remaining Batch 4c owner decision recorded.", nextRun, StringComparison.Ordinal);

        Assert.Contains(
            "| Batch 4c migration | Completed for the six localization fallback candidates | 2026-06-22 continuation goal approved the exact low-risk localization list; source now registers those six classes through RitsuLib `IPatchMethod` / `ModPatcher`. |",
            goal,
            StringComparison.Ordinal);
        Assert.Contains(
            "Do not treat Batch 4c source migration as gameplay, save-load, co-op, QA, release, or handoff proof.",
            goal,
            StringComparison.Ordinal);

        foreach (var forbiddenCategory in ForbiddenBatch4cMigrationCategories)
        {
            Assert.Contains(forbiddenCategory, record, StringComparison.Ordinal);
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
    /// state the correct total migrated count (64).
    /// </summary>
    [Fact]
    public void PatchInventoryDocListsMigratedPatches()
    {
        var inventory = ReadRepoText("docs", "patch-inventory.md");

        Assert.Contains("Migrated to RitsuLib ModPatcher | 64", inventory, StringComparison.Ordinal);
        Assert.Contains("## Migrated Patches (RitsuLib ModPatcher)", inventory, StringComparison.Ordinal);
        Assert.Contains("## Raw HarmonyPatch Declarations (Unmigrated)", inventory, StringComparison.Ordinal);
        AssertSourceContains(
            inventory,
            "`UrdaOptionRelicClickPatch.cs` | 1 | `urda-option-relic-click` | clicked-ui |",
            "`UrdaMapUiPatches.cs` | 3 | `urda-root-sight-map-point-ready, urda-root-sight-map-refresh-state, urda-root-sight-map-quest-icon-refresh` | clicked-ui |",
            "`UrdaRootSightMapClickPatches.cs` | 3 | `urda-root-sight-map-point-click, urda-root-sight-disabled-map-point-click, urda-root-sight-map-close` | clicked-ui |",
            "`SpirePlusMapPointHoverComposer.cs` | 1 | `spire-plus-map-point-hover-composer` | clicked-ui |",
            "`AscensionMapIconPatches.cs` | 1 | `ascension-map-marker-icon-refresh` | clicked-ui |",
            "`AscensionMapBossSealHoverPatches.cs` | 1 | `ascension-boss-map-point-hover` | clicked-ui |",
            "`SereTalonVisualUiPatches.cs` | 2 | `sere-talon-event-option-button-ready, sere-talon-relic-node-reload` | clicked-ui |",
            "`CrystalSpherePeekPatch.cs` | 2 | `crystal-sphere-peek-ready, crystal-sphere-peek-finished` | clicked-ui |",
            "`TransformPreviewPatch.cs` | 2 | `transform-preview-initialize, transform-preview-cycle-display` | clicked-ui |",
            "`SereTalonVisualPatches.cs` | 7 | `sere-talon-icon-path, sere-talon-packed-icon-path, sere-talon-packed-icon-outline-path, sere-talon-big-icon-path, sere-talon-icon-texture, sere-talon-icon-outline-texture, sere-talon-big-icon-texture` | visual-hover-ui |",
            "`PrismaticGemHoverPatches.cs` | 2 | `prismatic-gem-hover-tips, prismatic-gem-hover-tips-excluding-relic` | visual-hover-ui |",
            "`JewelryBoxPatches.cs` | 3 | `jewelry-box-extra-hover-tips, jewelry-box-hover-tips, jewelry-box-hover-tips-excluding-relic` | visual-hover-ui |",
            "`PrismaticGemRewardScreenHintPatch.cs` | 1 | `prismatic-gem-reward-screen-hint` | clicked-ui |",
            "`AscensionA20RewardScreenPatches.cs` | 2 | `ascension-a20-reward-screen-ready, ascension-a20-reward-screen-state` | clicked-ui |",
            "`ModInfoLocalizationPatches.cs` | 1 | `spire-plus-mod-info-localization` | clicked-ui |",
            "`CombatHandInputSafetyPatches.cs` | 1 | `combat-hand-input-safety` | clicked-ui |",
            "`AscensionLocalizationTablePatches.cs` | 6 | `ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key` | 4c-localization |");
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
    /// All migrated PatchId strings from ExpectedMigratedPatchIds must appear
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

    [Fact]
    public void UrdaOptionRelicClickPatchUsesRitsuLibDiscoverablePrefix()
    {
        var source = ReadRepoText(
            "EZMicroBalanceCode",
            "Ancients",
            "Expansion",
            "Urda",
            "UrdaOptionRelicClickPatch.cs");

        AssertSourceContains(
            source,
            "UrdaOptionRelicClickPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"urda-option-relic-click\"",
            "[HarmonyPrefix]",
            "private static bool Prefix(RelicModel model)");
        Assert.DoesNotContain("ExtractStoredSeedInsteadOfInspecting", source, StringComparison.Ordinal);
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
