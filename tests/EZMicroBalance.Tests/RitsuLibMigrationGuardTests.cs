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
        // Ancient reward patches - RitsuLib discovery migration
        "iron-club-vars",
        "brilliant-scarf-vars",
        "beautiful-bracelet-vars",
        "beautiful-bracelet-after-obtained",
        "music-box-before-card-played",
        "music-box-after-card-played",
        "music-box-turn-reset",
        "music-box-combat-reset",
        "velvet-choker-vars",
        "velvet-choker-display-amount",
        "velvet-choker-should-play",
        "velvet-choker-energy-cost",
        "velvet-choker-x-cost-can-play",
        "velvet-choker-x-cost-spend",
        "velvet-choker-after-card-played",
        "velvet-choker-turn-reset",
        "velvet-choker-room-reset",
        "velvet-choker-combat-reset",
        // Clicked UI patches - owner-approved targeted migration
        "neow-initial-option-reroll",
        "urda-overgrowth-ancient-unlock",
        "urda-underdocks-ancient-unlock",
        "urda-option-relic-click",
        "morvi-hive-ancient-unlock",
        "lotha-glory-ancient-unlock",
        "vakuu-force-ancient-unlock",
        "vakuu-fight-option",
        "vakuu-fight-command-force-cleanup",
        "vakuu-fight-victory-resume",
        "vakuu-fight-prefinished-parent-heal-skip",
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
        "transform-prediction-aroma-of-chaos-rng",
        "transform-prediction-endless-conveyor-rng",
        "transform-prediction-symbiote-rng",
        "transform-prediction-whispering-hollow-rng",
        "transform-prediction-morphic-grove-niche-rng",
        "transform-prediction-trial-niche-rng",
        "transform-prediction-new-leaf-niche-rng",
        "transform-prediction-astrolabe-niche-rng",
        "transform-prediction-selection-lifetime",
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
        "sovereign-blade-jade-boons-hover-tips",
        "prismatic-gem-reward-screen-hint",
        "ascension-a20-reward-screen-ready",
        "ascension-a20-reward-screen-state",
        "ascension-a20-courtyard-proceed",
        // Event visual UI patches
        "ascension-a20-courtyard-portrait",
        // Remaining UI/input patches
        "spire-plus-mod-info-localization",
        "combat-hand-input-safety",
        "meat-cleaver-cook-is-enabled",
        "meat-cleaver-cook-description",
        "meat-cleaver-cook-on-select",
        "ascension-selection-singleplayer-character-change",
        "ascension-selection-begin-run-locally",
        "ascension-selection-update-max-multiplayer",
        "ascension-selection-update-preferred",
        "ascension-selection-sync-warning",
        "ascension-selection-begin-run-for-all-warning",
        // Intent UI patches
        "aeonglass-laser-echo-intent-label",
        "aeonglass-laser-echo-intent-damage",
        // Enemy damage polish getters
        "decimillipede-writhe-damage-polish",
        "decimillipede-constrict-damage-polish",
        "decimillipede-bulk-damage-polish",
        "terror-eel-crash-damage-polish",
        "terror-eel-thrash-damage-polish",
        "phantasmal-gardener-bite-damage-polish",
        "phantasmal-gardener-lash-damage-polish",
        // Batch 4c localization fallback patches
        "ascension-localization-locstring-raw-text",
        "ascension-localization-get-table",
        "ascension-localization-raw-text",
        "ascension-localization-loc-string",
        "ascension-localization-has-entry",
        "ascension-localization-is-local-key",
        // Core inline localization fallback patches
        "spire-plus-inline-localization-raw-text",
        "spire-plus-inline-localization-loc-string",
        "spire-plus-inline-localization-has-entry",
        "spire-plus-inline-localization-is-local-key",
        // RitsuLib compatibility patches
        "ritsulib-mod-settings-button-selection-reticle"
    ];

    private const int ExpectedBatch4aCount = 9;
    private const int ExpectedBatch4bCount = 16;
    private const int ExpectedAncientRewardCount = 18;
    private const int ExpectedClickedUiCount = 50;
    private const int ExpectedVisualHoverUiCount = 13;
    private const int ExpectedEventVisualUiCount = 1;
    private const int ExpectedIntentUiCount = 2;
    private const int ExpectedEnemyDamagePolishCount = 7;
    private const int ExpectedBatch4cLocalizationCount = 6;
    private const int ExpectedInlineLocalizationCount = 4;
    private const int ExpectedRitsuLibCompatibilityCount = 1;
    private const int ExpectedTotalMigratedCount = 127;
    private const int ExpectedRawHarmonyPatchDeclarationCount = 43;

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
    /// The expected migrated patch count must be 127:
    /// 9 Batch 4a + 16 Batch 4b + 18 Ancient reward patches
    /// + 50 clicked/UI patches + 13 visual/hover UI patches
    /// + 1 event visual UI patch + 2 intent UI patches
    /// + 7 enemy damage polish getter patches
    /// + 6 Batch 4c localization patches + 4 inline localization patches
    /// + 1 RitsuLib compatibility patch.
    /// </summary>
    [Fact]
    public void MigratedPatchCountMatchesExpected()
    {
        Assert.Equal(ExpectedTotalMigratedCount, ExpectedMigratedPatchIds.Length);
        Assert.Equal(
            ExpectedBatch4aCount + ExpectedBatch4bCount + ExpectedAncientRewardCount + ExpectedClickedUiCount + ExpectedVisualHoverUiCount + ExpectedEventVisualUiCount + ExpectedIntentUiCount + ExpectedEnemyDamagePolishCount + ExpectedBatch4cLocalizationCount + ExpectedInlineLocalizationCount + ExpectedRitsuLibCompatibilityCount,
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
        var registry = ReadRitsuLibIntegrationSource();

        Assert.Contains("SpirePlusMigratedPatchRegistry.RegisterAll(patcher);", bootstrap, StringComparison.Ordinal);
        Assert.DoesNotContain(".RegisterPatch<", bootstrap, StringComparison.Ordinal);
        AssertSourceContains(
            bootstrap,
            "ApplyMigratedRitsuLibPatches();",
            "ApplyLegacyHarmonyFallbackPatches();",
            "AuditRitsuLibRuntimeState();",
            "RitsuLibFramework.ApplyRequiredPatcher(",
            "Required RitsuLib ModPatcher apply failed; Spire Plus bootstrap stopped before feature initialization.",
            "RitsuLib ModPatcher owns every migrated patch class",
            "fallback to another mod framework");
        Assert.DoesNotContain("patcher.PatchAll();", bootstrap, StringComparison.Ordinal);
        AssertSourceContains(
            registry,
            "internal static partial class SpirePlusMigratedPatchRegistry",
            "public static void RegisterAll(ModPatcher patcher)",
            "RegisterBatch4a(patcher);",
            "RegisterBatch4b(patcher);",
            "RegisterAncientRewardPatches(patcher);",
            "RegisterPatch<IronClubVarsPatch>();",
            "RegisterPatch<BrilliantScarfVarsPatch>();",
            "RegisterPatch<BeautifulBraceletVarsPatch>();",
            "RegisterPatch<BeautifulBraceletPatch>();",
            "RegisterPatch<MusicBoxBeforeCardPlayedPatch>();",
            "RegisterPatch<MusicBoxAfterCardPlayedPatch>();",
            "RegisterPatch<MusicBoxTurnResetPatch>();",
            "RegisterPatch<MusicBoxCombatResetPatch>();",
            "RegisterPatch<VelvetChokerVarsPatch>();",
            "RegisterPatch<VelvetChokerDisplayAmountPatch>();",
            "RegisterPatch<VelvetChokerShouldPlayPatch>();",
            "RegisterPatch<VelvetChokerEnergyCostPatch>();",
            "RegisterPatch<VelvetChokerXCostCanPlayPatch>();",
            "RegisterPatch<VelvetChokerXCostSpendPatch>();",
            "RegisterPatch<VelvetChokerAfterCardPlayedPatch>();",
            "RegisterPatch<VelvetChokerTurnResetPatch>();",
            "RegisterPatch<VelvetChokerRoomResetPatch>();",
            "RegisterPatch<VelvetChokerCombatResetPatch>();",
            "RegisterAncientEventUiPatches(patcher);",
            "RegisterPatch<NeowInitialOptionRerollPatch>();",
            "RegisterPatch<UrdaOvergrowthPatch>();",
            "RegisterPatch<UrdaUnderdocksPatch>();",
            "RegisterPatch<UrdaOptionRelicClickPatch>();",
            "RegisterPatch<MorviHivePatch>();",
            "RegisterPatch<LothaGloryPatch>();",
            "RegisterPatch<VakuuForceAncientPatch>();",
            "RegisterPatch<VakuuFightOptionPatch>();",
            "RegisterPatch<VakuuFightCommandForceCleanupPatch>();",
            "RegisterPatch<VakuuFightResumePatch>();",
            "RegisterPatch<VakuuFightPreFinishedParentRestoreHealPatch>();",
            "RegisterClickedUiPatches(patcher);",
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
            "RegisterPatch<TransformPredictionAromaOfChaosRngPatch>();",
            "RegisterPatch<TransformPredictionEndlessConveyorRngPatch>();",
            "RegisterPatch<TransformPredictionSymbioteRngPatch>();",
            "RegisterPatch<TransformPredictionWhisperingHollowRngPatch>();",
            "RegisterPatch<TransformPredictionMorphicGroveNicheRngPatch>();",
            "RegisterPatch<TransformPredictionTrialNicheRngPatch>();",
            "RegisterPatch<TransformPredictionNewLeafNicheRngPatch>();",
            "RegisterPatch<TransformPredictionAstrolabeNicheRngPatch>();",
            "RegisterPatch<TransformPredictionSelectionLifetimePatch>();",
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
            "RegisterPatch<AscensionA20CourtyardProceedPatch>();",
            "RegisterPatch<AscensionA20CourtyardPortraitPatch>();",
            "RegisterPatch<ModInfoLocalizationPatches>();",
            "RegisterPatch<CombatHandInputSafetyPatch>();",
            "RegisterPatch<MeatCleaverCookIsEnabledPatch>();",
            "RegisterPatch<MeatCleaverCookDescriptionPatch>();",
            "RegisterPatch<MeatCleaverCookPatch>();",
            "RegisterAscensionSelectionUiPatches(patcher);",
            "RegisterPatch<StartRunLobbySetSingleplayerAscensionPatch>();",
            "RegisterPatch<StartRunLobbyBeginRunLocallyPatch>();",
            "RegisterPatch<StartRunLobbyUpdateMaxMultiplayerAscensionPatch>();",
            "RegisterPatch<StartRunLobbyUpdatePreferredAscensionPatch>();",
            "RegisterPatch<StartRunLobbySyncAscensionChangeA20WarningPatch>();",
            "RegisterPatch<StartRunLobbyBeginRunForAllPlayersA20WarningPatch>();",
            "RegisterAscensionIntentUiPatches(patcher);",
            "RegisterPatch<AeonglassLaserEchoIntentLabelPatch>();",
            "RegisterPatch<AeonglassLaserEchoIntentDamagePatch>();",
            "RegisterEnemyDamagePolishPatches(patcher);",
            "RegisterPatch<DecimillipedeWritheDamagePolishPatch>();",
            "RegisterPatch<DecimillipedeConstrictDamagePolishPatch>();",
            "RegisterPatch<DecimillipedeBulkDamagePolishPatch>();",
            "RegisterPatch<TerrorEelCrashDamagePolishPatch>();",
            "RegisterPatch<TerrorEelThrashDamagePolishPatch>();",
            "RegisterPatch<PhantasmalGardenerBiteDamagePolishPatch>();",
            "RegisterPatch<PhantasmalGardenerLashDamagePolishPatch>();",
            "RegisterBatch4cLocalizationPatches(patcher);",
            "RegisterPatch<AscensionLocalizationLocStringRawTextPatch>();",
            "RegisterPatch<AscensionLocalizationGetTablePatch>();",
            "RegisterPatch<AscensionLocalizationRawTextPatch>();",
            "RegisterPatch<AscensionLocalizationLocStringPatch>();",
            "RegisterPatch<AscensionLocalizationHasEntryPatch>();",
            "RegisterPatch<AscensionLocalizationIsLocalKeyPatch>();",
            "RegisterInlineLocalizationPatches(patcher);",
            "RegisterPatch<SpirePlusInlineLocalizationRawTextPatch>();",
            "RegisterPatch<SpirePlusInlineLocalizationLocStringPatch>();",
            "RegisterPatch<SpirePlusInlineLocalizationHasEntryPatch>();",
            "RegisterPatch<SpirePlusInlineLocalizationIsLocalKeyPatch>();");
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
        Assert.Contains("Current boundary: Spire Plus is RitsuLib-only for beta.123", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Batch 4c localization fallback patches, the visual-hover UI getter batch", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Ancient reward getter/relic hook patches, Aeonglass intent UI patches", migrationDoc, StringComparison.Ordinal);
        Assert.Contains("Enemy Damage polish getter patches", migrationDoc, StringComparison.Ordinal);
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
        Assert.Contains("installed beta.123 package parity passed; current beta.123 clicked Ancient UI smoke applied all 127 migrated patch classes.", record, StringComparison.Ordinal);
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
            "2026-06-23: NuGet flat-container and `dotnet list package --outdated --include-transitive` show `STS2.RitsuLib` `0.4.34` as the latest package",
            "The flat-container index lists 165 versions and ends at `0.4.34`",
            "found no `STS2.RitsuLib` update",
            "Nexus files list the variant-pack main file as `0.4.34`",
            "GitHub release API now reports tag/name `v0.4.34` / `0.4.34`",
            "raw `main` `mod_manifest.json` reports version `0.4.34`",
            "official NuGet package via `RitsuLibDeployDir`",
            "primary dependency-floor evidence",
            "Keep Spire Plus on stable `0.4.34`, not a dev build");

        AssertSourceContains(
            integrationDoc,
            "## External Version Recheck",
            "2026-06-23 recheck",
            "`dotnet list EZMicroBalance.csproj package --outdated --include-transitive`",
            "found no `STS2.RitsuLib` update",
            "The NuGet flat-container index reports `STS2.RitsuLib` latest `0.4.34`",
            "across 165 listed versions",
            "GitHub release API now reports release tag/name `v0.4.34` / `0.4.34`",
            "raw `main` branch `mod_manifest.json`",
            "Keep NuGet plus the installed XML/runtime as the primary",
            "NuGet package",
            "Nexus files page",
            "GitHub as corroborating evidence",
            "Major Update #2",
            "`v0.107.1`",
            "Workshop and RNG-system changes are dependency-sensitive",
            "rerun the source-workspace checker",
            "RitsuLib variant check before claiming compatibility");
    }

    /// <summary>
    /// docs/patch-inventory.md must list the migrated patches section and
    /// state the correct total migrated count (127).
    /// </summary>
    [Fact]
    public void PatchInventoryDocListsMigratedPatches()
    {
        var inventory = ReadRepoText("docs", "patch-inventory.md");

        Assert.Contains("Migrated to RitsuLib ModPatcher | 127", inventory, StringComparison.Ordinal);
        Assert.Contains("Raw HarmonyPatch remaining | 43", inventory, StringComparison.Ordinal);
        Assert.Contains("## Migrated Patches (RitsuLib ModPatcher)", inventory, StringComparison.Ordinal);
        Assert.Contains("## Raw HarmonyPatch Declarations (Unmigrated)", inventory, StringComparison.Ordinal);
        AssertSourceContains(
            inventory,
            "`VakuRewardPatches.cs` | 8 | `iron-club-vars, brilliant-scarf-vars, beautiful-bracelet-vars, beautiful-bracelet-after-obtained, music-box-before-card-played, music-box-after-card-played, music-box-turn-reset, music-box-combat-reset` | ancient-reward |",
            "`VelvetChokerPatches.cs` | 10 | `velvet-choker-vars, velvet-choker-display-amount, velvet-choker-should-play, velvet-choker-energy-cost, velvet-choker-x-cost-can-play, velvet-choker-x-cost-spend, velvet-choker-after-card-played, velvet-choker-turn-reset, velvet-choker-room-reset, velvet-choker-combat-reset` | ancient-reward |",
            "`NeowInitialOptionRerollPatch.cs` | 1 | `neow-initial-option-reroll` | clicked-ui |",
            "`UrdaAct1AncientService.cs` | 2 | `urda-overgrowth-ancient-unlock, urda-underdocks-ancient-unlock` | clicked-ui |",
            "`UrdaOptionRelicClickPatch.cs` | 1 | `urda-option-relic-click` | clicked-ui |",
            "`MorviAct2AncientService.cs` | 1 | `morvi-hive-ancient-unlock` | clicked-ui |",
            "`LothaAct3AncientService.cs` | 1 | `lotha-glory-ancient-unlock` | clicked-ui |",
            "`VakuuFightPatch.cs` | 5 | `vakuu-force-ancient-unlock, vakuu-fight-option, vakuu-fight-command-force-cleanup, vakuu-fight-victory-resume, vakuu-fight-prefinished-parent-heal-skip` | clicked-ui |",
            "`UrdaMapUiPatches.cs` | 3 | `urda-root-sight-map-point-ready, urda-root-sight-map-refresh-state, urda-root-sight-map-quest-icon-refresh` | clicked-ui |",
            "`UrdaRootSightMapClickPatches.cs` | 3 | `urda-root-sight-map-point-click, urda-root-sight-disabled-map-point-click, urda-root-sight-map-close` | clicked-ui |",
            "`SpirePlusMapPointHoverComposer.cs` | 1 | `spire-plus-map-point-hover-composer` | clicked-ui |",
            "`AscensionMapIconPatches.cs` | 1 | `ascension-map-marker-icon-refresh` | clicked-ui |",
            "`AscensionMapBossSealHoverPatches.cs` | 1 | `ascension-boss-map-point-hover` | clicked-ui |",
            "`SereTalonVisualUiPatches.cs` | 2 | `sere-talon-event-option-button-ready, sere-talon-relic-node-reload` | clicked-ui |",
            "`CrystalSpherePeekPatch.cs` | 2 | `crystal-sphere-peek-ready, crystal-sphere-peek-finished` | clicked-ui |",
            "`TransformPreviewPatch.cs` | 2 | `transform-preview-initialize, transform-preview-cycle-display` | clicked-ui |",
            "`TransformPredictionEventRngSourcePatches.cs` | 4 | `transform-prediction-aroma-of-chaos-rng, transform-prediction-endless-conveyor-rng, transform-prediction-symbiote-rng, transform-prediction-whispering-hollow-rng` | clicked-ui |",
            "`TransformPredictionNicheRngSourcePatches.cs` | 4 | `transform-prediction-morphic-grove-niche-rng, transform-prediction-trial-niche-rng, transform-prediction-new-leaf-niche-rng, transform-prediction-astrolabe-niche-rng` | clicked-ui |",
            "`TransformPredictionSelectionLifetimePatch.cs` | 1 | `transform-prediction-selection-lifetime` | clicked-ui |",
            "`SereTalonVisualPatches.cs` | 7 | `sere-talon-icon-path, sere-talon-packed-icon-path, sere-talon-packed-icon-outline-path, sere-talon-big-icon-path, sere-talon-icon-texture, sere-talon-icon-outline-texture, sere-talon-big-icon-texture` | visual-hover-ui |",
            "`PrismaticGemHoverPatches.cs` | 2 | `prismatic-gem-hover-tips, prismatic-gem-hover-tips-excluding-relic` | visual-hover-ui |",
            "`JewelryBoxPatches.cs` | 3 | `jewelry-box-extra-hover-tips, jewelry-box-hover-tips, jewelry-box-hover-tips-excluding-relic` | visual-hover-ui |",
            "`SovereignBladeForgePatches.cs` | 1 | `sovereign-blade-jade-boons-hover-tips` | visual-hover-ui |",
            "`PrismaticGemRewardScreenHintPatch.cs` | 1 | `prismatic-gem-reward-screen-hint` | clicked-ui |",
            "`AscensionA20RewardScreenPatches.cs` | 2 | `ascension-a20-reward-screen-ready, ascension-a20-reward-screen-state` | clicked-ui |",
            "`AscensionA20Patches.cs` | 1 | `ascension-a20-courtyard-proceed` | clicked-ui |",
            "`A20Courtyard.cs` | 1 | `ascension-a20-courtyard-portrait` | event-visual-ui |",
            "`ModInfoLocalizationPatches.cs` | 1 | `spire-plus-mod-info-localization` | clicked-ui |",
            "`CombatHandInputSafetyPatches.cs` | 1 | `combat-hand-input-safety` | clicked-ui |",
            "`MeatCleaverCookPatches.cs` | 3 | `meat-cleaver-cook-is-enabled, meat-cleaver-cook-description, meat-cleaver-cook-on-select` | clicked-ui |",
            "`AscensionSelectionPatches.cs` | 1 | `ascension-selection-singleplayer-character-change` | clicked-ui |",
            "`AscensionSelectionRunStartPatches.cs` | 5 | `ascension-selection-begin-run-locally, ascension-selection-update-max-multiplayer, ascension-selection-update-preferred, ascension-selection-sync-warning, ascension-selection-begin-run-for-all-warning` | clicked-ui |",
            "`AeonglassIntentPatches.cs` | 2 | `aeonglass-laser-echo-intent-label, aeonglass-laser-echo-intent-damage` | intent-ui |",
            "`EnemyDamagePolishPatches.cs` | 7 | `decimillipede-writhe-damage-polish, decimillipede-constrict-damage-polish, decimillipede-bulk-damage-polish, terror-eel-crash-damage-polish, terror-eel-thrash-damage-polish, phantasmal-gardener-bite-damage-polish, phantasmal-gardener-lash-damage-polish` | enemy-damage-polish |",
            "`AscensionLocalizationTablePatches.cs` | 6 | `ascension-localization-locstring-raw-text, ascension-localization-get-table, ascension-localization-raw-text, ascension-localization-loc-string, ascension-localization-has-entry, ascension-localization-is-local-key` | 4c-localization |",
            "`SpirePlusInlineLocalizationPatches.cs` | 4 | `spire-plus-inline-localization-raw-text, spire-plus-inline-localization-loc-string, spire-plus-inline-localization-has-entry, spire-plus-inline-localization-is-local-key` | inline-localization |");
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
    public void MigratedPatchMethodsUseRitsuLibDiscoverableNames()
    {
        var sourceFiles = Directory.GetFiles(
            RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories);
        var classPattern = new Regex(@"\bclass\s+(?<name>\w+)\s*:\s*IPatchMethod\b");
        var methodPattern = new Regex(@"\bstatic\s+(?:async\s+)?[^(=;]+?\s+(?<name>[A-Za-z_]\w*)\s*\(");
        var offenders = new List<string>();

        foreach (var file in sourceFiles)
        {
            var lines = File.ReadAllLines(file);
            string? patchClass = null;

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var classMatch = classPattern.Match(line);
                if (classMatch.Success)
                {
                    patchClass = classMatch.Groups["name"].Value;
                }
                else if (Regex.IsMatch(line, @"^\s*internal\s+(?:sealed\s+|static\s+|partial\s+)*class\s+"))
                {
                    patchClass = null;
                }

                if (patchClass == null)
                {
                    continue;
                }

                var expectedMethodName = line.Contains("[HarmonyPrefix]", StringComparison.Ordinal)
                    ? "Prefix"
                    : line.Contains("[HarmonyPostfix]", StringComparison.Ordinal)
                        ? "Postfix"
                        : null;
                if (expectedMethodName == null)
                {
                    continue;
                }

                var methodLineIndex = FindNextMethodLine(lines, index + 1);
                if (methodLineIndex < 0)
                {
                    offenders.Add($"{ToRepoRelativePath(file)}:{index + 1}: {patchClass} has {line.Trim()} without a discoverable method signature.");
                    continue;
                }

                var methodMatch = methodPattern.Match(lines[methodLineIndex]);
                if (!methodMatch.Success || !string.Equals(methodMatch.Groups["name"].Value, expectedMethodName, StringComparison.Ordinal))
                {
                    offenders.Add($"{ToRepoRelativePath(file)}:{methodLineIndex + 1}: {patchClass} {line.Trim()} must name its method {expectedMethodName} for RitsuLib discovery.");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "RitsuLib IPatchMethod discovery expects canonical Prefix/Postfix method names:" +
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

    private static int FindNextMethodLine(string[] lines, int startIndex)
    {
        for (var index = startIndex; index < lines.Length; index++)
        {
            var trimmed = lines[index].Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith("[", StringComparison.Ordinal))
            {
                continue;
            }

            return index;
        }

        return -1;
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
