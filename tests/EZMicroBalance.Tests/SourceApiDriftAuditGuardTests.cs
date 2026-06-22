using Xunit;

namespace EZMicroBalance.Tests;

public sealed class SourceApiDriftAuditGuardTests
{
    [Fact]
    public void V0106SourceApiDriftAuditRecordsCurrentPrimaryEvidence()
    {
        var audit = ReadRepoText("docs", "audits", "v0.106-source-api-drift.md");
        var docsIndex = ReadRepoText("docs", "README.md");

        AssertSourceContains(
            audit,
            "v0.106.1",
            "`previous package` `v3.1.4`",
            "`source code/src/Core/**`",
            "`sourcecodeonlyaianalysis/**` is not present",
            "do not use `v0.105.x` notes as the sole implementation basis",
            "`source code/src/Core/Commands/CardPileCmd.cs`",
            "clonedBy",
            "`Hook.AfterCardChangedPiles`",
            "`source code/src/Core/Entities/CardRewardAlternatives/CardRewardAlternative.cs`",
            "throws if alternatives exceed two",
            "`source code/src/Core/Rewards/CardReward.cs`",
            "`CardReward.OnSkipped()`",
            "`source code/src/Core/Models/AncientEventModel.cs`",
            "`Hook.ShouldAllowAncient`",
            "`source code/src/Core/MonsterMoves/Intents/AttackIntent.cs`",
            "`Hook.ModifyDamage",
            "ModifyDamageHookType.All",
            "`source code/src/Core/Runs/RunManager.cs`",
            "`act.SetSecondBossEncounter",
            "`RunManager.ProceedFromTerminalRewardsScreen()`",
            "`source code/src/Core/Commands/CreatureCmd.cs`",
            "`Hook.ShouldDie`",
            "`Hook.AfterPreventingDeath`",
            "`LizardTail`",
            "`FairyInABottle`",
            "`source code/src/Core/Multiplayer/Game/JoinFlow.cs`",
            "`source code/src/Core/Multiplayer/Game/Lobby/StartRunLobby.cs`",
            "`SyncAscensionChange`",
            "`source code/src/Core/Debug/ReleaseInfoManager.cs`",
            "Historical audit-time code registered config through previous package `ModConfigRegistry`",
            "current code registers Spire Plus settings through `RitsuLibFramework.RegisterModSettings(...)`",
            "with no `previous package` package reference",
            "no-black-screen victory return remains live-only",
            "two-client proof");

        Assert.Contains("`audits/v0.106-source-api-drift.md`", docsIndex, StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void LocalCoreSnapshotStillExposesAuditedApiShapes()
    {
        var cardPileCmd = ReadLocalCoreText("Commands", "CardPileCmd.cs");
        var cardRewardAlternative = ReadLocalCoreText("Entities", "CardRewardAlternatives", "CardRewardAlternative.cs");
        var cardReward = ReadLocalCoreText("Rewards", "CardReward.cs");
        var ancientEventModel = ReadLocalCoreText("Models", "AncientEventModel.cs");
        var attackIntent = ReadLocalCoreText("MonsterMoves", "Intents", "AttackIntent.cs");
        var runManager = ReadLocalCoreText("Runs", "RunManager.cs");
        var creatureCmd = ReadLocalCoreText("Commands", "CreatureCmd.cs");
        var startRunLobby = ReadLocalCoreText("Multiplayer", "Game", "Lobby", "StartRunLobby.cs");
        var joinFlow = ReadLocalCoreText("Multiplayer", "Game", "JoinFlow.cs");

        AssertSourceContains(
            cardPileCmd,
            "AbstractModel? clonedBy = null",
            "Hook.AfterCardChangedPiles",
            "AddGeneratedCardToCombat",
            "Hook.AfterCardGeneratedForCombat");

        AssertSourceContains(
            cardRewardAlternative,
            "Hook.ModifyCardRewardAlternatives",
            "More than 2 card reward alternatives are not supported.");

        AssertSourceContains(
            cardReward,
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward",
            "public override void OnSkipped()");

        AssertSourceContains(
            ancientEventModel,
            "protected override async Task BeforeEventStarted",
            "RunManager.Instance.HasAscension(AscensionLevel.WearyTraveler)",
            "GeneratedOptions = GenerateInitialOptions().ToList()",
            "Hook.ShouldAllowAncient");

        AssertSourceContains(
            attackIntent,
            "Hook.ModifyDamage",
            "ModifyDamageHookType.All",
            "CardPreviewMode.None");

        AssertSourceContains(
            runManager,
            "AscensionLevel.DoubleBoss",
            "act.SetSecondBossEncounter",
            "ProceedFromTerminalRewardsScreen",
            "ShouldResumeParentEventAfterCombat");

        AssertSourceContains(
            creatureCmd,
            "Hook.ShouldDie",
            "Hook.AfterPreventingDeath",
            "force || creature.MaxHp <= 0",
            "recursion >= 10");

        AssertSourceContains(
            startRunLobby,
            "maxMultiplayerAscensionUnlocked",
            "PreferredMultiplayerAscension",
            "SyncAscensionChange");

        AssertSourceContains(
            joinFlow,
            "Version mismatch",
            "Mod mismatch",
            "Model ID hash");
    }

    [Fact]
    public void ProjectUsesCurrentRitsuLibPackageAndModConfigRegistration()
    {
        var project = ReadRepoText("EZMicroBalance.csproj");
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var settings = ReadSourceTree("EZMicroBalanceCode", "Config");

        AssertSourceContains(
            project,
            "STS2.RitsuLib\" Version=\"0.4.33",
            "Include=\"0Harmony\"",
            "Include=\"sts2\"");

        Assert.Contains("SpirePlusModConfig.Register(ModId)", mainFile, StringComparison.Ordinal);
        Assert.Contains("SpirePlusContentRegistrationService.Register(ModId)", mainFile, StringComparison.Ordinal);
        AssertSourceContains(
            settings,
            "RegisterSettingsStore(modId)",
            "RegisterSettingsPage(modId)",
            "AddMigrationStatusSection(page)",
            "AddPreviewToolsSection(page, modId)",
            "SettingsLocalizationStem",
            "SettingsLocalizationPckRoot",
            "res://EZMicroBalance/localization/settings_ui",
            "RitsuLibFramework.CreateModLocalization",
            "RitsuLibFramework.BeginModDataRegistration(modId)",
            "RitsuLibFramework.RegisterModSettings",
            "ModSettingsText.I18N(i18n, key, fallback)",
            "migration_status",
            "required_runtime_dependency",
            "proof_boundary",
            "preview_tools",
            "EnableCrystalSpherePeekEntryId",
            "CrystalSphereMaskAlphaEntryId",
            "EnableTransformPredictionEntryId",
            "TransformPredictionAlwaysOnEntryId",
            "ShowPreviewDebugLogsEntryId",
            "DefaultCrystalSphereMaskAlpha",
            "CrystalSphereMaskAlphaMin",
            "CrystalSphereMaskAlphaMax",
            "CrystalSphereMaskAlphaStep",
            "NormalizeCrystalSphereMaskAlpha",
            "RitsuLib setting controls bind to this data key",
            "Keep these entry IDs stable",
            "SPIREPLUS-MIGRATION_STATUS.title",
            "SPIREPLUS-PREVIEW_TOOLS.title",
            "RitsuLib-only mod surface",
            "STS2-RitsuLib >= 0.4.33",
            "Settings screenshots prove UI visibility only.");
        Assert.DoesNotContain("private static ModSettingsText Text(string value)", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("Math.Clamp(value, 0.05, 0.95)", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("store.InitializeGlobal();", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void RitsuLibModConfigKeepsStoreAndSettingsPageSplit()
    {
        var entry = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.cs");
        var constants = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.Constants.cs");
        var previewSettings = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.PreviewSettings.cs");
        var page = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsPage.cs");
        var migrationStatus = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsPage.MigrationStatus.cs");
        var migrationStatusEntries = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsPage.MigrationStatusEntries.cs");
        var previewTools = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsPage.PreviewTools.cs");
        var previewToolEntries = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsPage.PreviewToolEntries.cs");
        var store = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsStore.cs");
        var access = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsAccess.cs");
        var binding = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsBinding.cs");
        var state = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.SettingsState.cs");

        AssertSourceContains(
            entry,
            "internal static partial class SpirePlusModConfig",
            "RitsuLibFramework.CreateModLocalization",
            "RegisterSettingsStore(modId);",
            "RegisterSettingsPage(modId);");
        Assert.DoesNotContain("RegisterModSettings", entry, StringComparison.Ordinal);
        Assert.DoesNotContain("BeginModDataRegistration", entry, StringComparison.Ordinal);
        Assert.DoesNotContain("EnableCrystalSpherePeekEntryId", entry, StringComparison.Ordinal);
        Assert.DoesNotContain("EnableCrystalSpherePeek", entry, StringComparison.Ordinal);

        AssertSourceContains(
            constants,
            "private const string SettingsKey = \"spire-plus-settings\"",
            "private const string SettingsLocalizationPckRoot = \"res://EZMicroBalance/localization/settings_ui\"",
            "private const string MigrationStatusSectionId = \"migration_status\"",
            "private const string PreviewToolsSectionId = \"preview_tools\"",
            "These RitsuLib setting-entry ids are persisted bindings and evidence anchors.",
            "private const string EnableCrystalSpherePeekEntryId = \"enable_crystal_sphere_peek\"",
            "private const string RequiredRuntimeDependency = \"STS2-RitsuLib >= 0.4.33\"",
            "private const double CrystalSphereMaskAlphaStep = 0.05");

        AssertSourceContains(
            previewSettings,
            "Preview runtime code reads these accessors instead of reaching into the",
            "public static bool EnableCrystalSpherePeek",
            "public static double CrystalSphereMaskAlpha",
            "NormalizeCrystalSphereMaskAlpha(value)",
            "public static bool EnableTransformPrediction",
            "public static bool TransformPredictionAlwaysOn",
            "public static bool ShowPreviewDebugLogs");
        Assert.DoesNotContain("RegisterModSettings", previewSettings, StringComparison.Ordinal);
        Assert.DoesNotContain("BeginModDataRegistration", previewSettings, StringComparison.Ordinal);
        Assert.DoesNotContain("AddPreviewToolsSection", previewSettings, StringComparison.Ordinal);

        AssertSourceContains(
            page,
            "RitsuLibFramework.RegisterModSettings",
            "AddMigrationStatusSection(page)",
            "AddPreviewToolsSection(page, modId)",
            "ModSettingsText.I18N(i18n, key, fallback)");
        Assert.DoesNotContain("BeginModDataRegistration", page, StringComparison.Ordinal);
        Assert.DoesNotContain("store.Register", page, StringComparison.Ordinal);
        Assert.DoesNotContain("page.AddSection(", page, StringComparison.Ordinal);

        AssertSourceContains(
            migrationStatus,
            "private static void AddMigrationStatusSection(ModSettingsPageBuilder page)",
            "page.AddSection(MigrationStatusSectionId",
            "SPIREPLUS-MIGRATION_STATUS.title",
            "AddRitsuLibSummaryParagraph(section)",
            "AddRequiredRuntimeDependencyCard(section)",
            "AddStableManifestIdCard(section)",
            "AddProofBoundaryCard(section)");
        Assert.DoesNotContain("section.AddParagraph(", migrationStatus, StringComparison.Ordinal);
        Assert.DoesNotContain("section.AddInfoCard(", migrationStatus, StringComparison.Ordinal);
        Assert.DoesNotContain("AddPreviewToolsSection", migrationStatus, StringComparison.Ordinal);

        AssertSourceContains(
            migrationStatusEntries,
            "private static void AddRitsuLibSummaryParagraph(ModSettingsSectionBuilder section)",
            "private static void AddRequiredRuntimeDependencyCard(ModSettingsSectionBuilder section)",
            "private static void AddStableManifestIdCard(ModSettingsSectionBuilder section)",
            "private static void AddProofBoundaryCard(ModSettingsSectionBuilder section)",
            "RitsuLibSummaryEntryId",
            "RequiredRuntimeDependencyEntryId",
            "StableManifestIdEntryId",
            "ProofBoundaryEntryId",
            "RitsuLib-only mod surface",
            "Spire Plus uses RitsuLib for settings persistence, content registration, patch metadata, and saved marker state.",
            "LiteralText(RequiredRuntimeDependency)",
            "Settings screenshots prove UI visibility only.");
        Assert.DoesNotContain("page.AddSection", migrationStatusEntries, StringComparison.Ordinal);
        Assert.DoesNotContain("AddPreviewToolsSection", migrationStatusEntries, StringComparison.Ordinal);

        AssertSourceContains(
            previewTools,
            "private static void AddPreviewToolsSection(ModSettingsPageBuilder page, string modId)",
            "page.AddSection(PreviewToolsSectionId",
            "SPIREPLUS-PREVIEW_TOOLS.title",
            "AddCrystalSpherePeekToggle(section, modId)",
            "AddCrystalSphereMaskAlphaSlider(section, modId)",
            "AddTransformPredictionToggle(section, modId)",
            "AddTransformPredictionAlwaysOnToggle(section, modId)",
            "AddPreviewDebugLogsToggle(section, modId)");
        Assert.DoesNotContain("section.AddToggle(", previewTools, StringComparison.Ordinal);
        Assert.DoesNotContain("section.AddSlider(", previewTools, StringComparison.Ordinal);
        Assert.DoesNotContain("AddMigrationStatusSection", previewTools, StringComparison.Ordinal);

        AssertSourceContains(
            previewToolEntries,
            "private static void AddCrystalSpherePeekToggle(ModSettingsSectionBuilder section, string modId)",
            "private static void AddCrystalSphereMaskAlphaSlider(ModSettingsSectionBuilder section, string modId)",
            "private static void AddTransformPredictionToggle(ModSettingsSectionBuilder section, string modId)",
            "private static void AddTransformPredictionAlwaysOnToggle(ModSettingsSectionBuilder section, string modId)",
            "private static void AddPreviewDebugLogsToggle(ModSettingsSectionBuilder section, string modId)",
            "EnableCrystalSpherePeekEntryId",
            "CrystalSphereMaskAlphaEntryId",
            "EnableTransformPredictionEntryId",
            "TransformPredictionAlwaysOnEntryId",
            "ShowPreviewDebugLogsEntryId",
            "CultureInfo.InvariantCulture",
            "NormalizeCrystalSphereMaskAlpha(value)");
        Assert.DoesNotContain("page.AddSection", previewToolEntries, StringComparison.Ordinal);
        Assert.DoesNotContain("AddMigrationStatusSection", previewToolEntries, StringComparison.Ordinal);

        AssertSourceContains(
            store,
            "RitsuLibFramework.BeginModDataRegistration(modId)",
            "store.Register(SettingsKey, SettingsFileName, SaveScope.Global, () => new SettingsState(), true)");
        Assert.DoesNotContain("RegisterModSettings", store, StringComparison.Ordinal);
        Assert.DoesNotContain("ModSettingsValueBinding", store, StringComparison.Ordinal);
        Assert.DoesNotContain("Store.Modify", store, StringComparison.Ordinal);

        AssertSourceContains(
            access,
            "private static SettingsState State",
            "RitsuLibFramework.IsActive",
            "Store.Get<SettingsState>(SettingsKey)",
            "private static ModDataStore Store",
            "Store.Modify(SettingsKey, update)");
        Assert.DoesNotContain("RegisterSettingsStore", access, StringComparison.Ordinal);
        Assert.DoesNotContain("ModSettingsValueBinding", access, StringComparison.Ordinal);

        AssertSourceContains(
            binding,
            "private static IModSettingsValueBinding<TValue> Binding<TValue>",
            "ModSettingsValueBinding<SettingsState, TValue>",
            "SaveScope.Global",
            "private static double NormalizeCrystalSphereMaskAlpha(double value)",
            "Math.Clamp(value, CrystalSphereMaskAlphaMin, CrystalSphereMaskAlphaMax)");
        Assert.DoesNotContain("BeginModDataRegistration", binding, StringComparison.Ordinal);
        Assert.DoesNotContain("RegisterModSettings", binding, StringComparison.Ordinal);

        AssertSourceContains(
            state,
            "private sealed class SettingsState",
            "public bool EnableCrystalSpherePeek { get; set; } = true",
            "public double CrystalSphereMaskAlpha { get; set; } = DefaultCrystalSphereMaskAlpha",
            "public bool ShowPreviewDebugLogs { get; set; }");
        Assert.DoesNotContain("RitsuLibFramework", state, StringComparison.Ordinal);
    }

}
