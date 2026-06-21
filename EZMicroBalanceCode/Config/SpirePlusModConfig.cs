using System.Globalization;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static class SpirePlusModConfig
{
    private const string SettingsKey = "spire-plus-settings";
    private const string SettingsFileName = "spire-plus-settings.json";
    private const string SettingsLocalizationStem = "settings_ui";
    private const string SettingsLocalizationPckRoot = "res://EZMicroBalance/localization/settings_ui";
    private const string MigrationStatusSectionId = "migration_status";
    private const string PreviewToolsSectionId = "preview_tools";
    private const string RitsuLibSummaryEntryId = "ritsulib_only_summary";
    private const string RequiredRuntimeDependencyEntryId = "required_runtime_dependency";
    private const string StableManifestIdEntryId = "stable_manifest_id";
    private const string ProofBoundaryEntryId = "proof_boundary";
    // These RitsuLib setting-entry ids are persisted bindings and evidence anchors.
    // Rename them only with an explicit migration and package-version pass.
    private const string EnableCrystalSpherePeekEntryId = "enable_crystal_sphere_peek";
    private const string CrystalSphereMaskAlphaEntryId = "crystal_sphere_mask_alpha";
    private const string EnableTransformPredictionEntryId = "enable_transform_prediction";
    private const string TransformPredictionAlwaysOnEntryId = "transform_prediction_always_on";
    private const string ShowPreviewDebugLogsEntryId = "show_preview_debug_logs";
    private const string RequiredRuntimeDependency = "STS2-RitsuLib >= 0.4.31";
    private const string StableTechnicalId = "EZMicroBalance";
    private const double DefaultCrystalSphereMaskAlpha = 0.32;
    private const double CrystalSphereMaskAlphaMin = 0.05;
    private const double CrystalSphereMaskAlphaMax = 0.95;
    private const double CrystalSphereMaskAlphaStep = 0.05;

    // RitsuLib owns the persisted settings store. The fallback is only used while
    // the framework is unavailable during early startup or when a store read fails.
    private static readonly SettingsState FallbackState = new();
    private static I18N? settingsLocalization;
    private static string? registeredModId;

    public static bool EnableCrystalSpherePeek
    {
        get => State.EnableCrystalSpherePeek;
        set => UpdateState(state => state.EnableCrystalSpherePeek = value);
    }

    public static double CrystalSphereMaskAlpha
    {
        get => State.CrystalSphereMaskAlpha;
        set => UpdateState(state => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value));
    }

    public static bool EnableTransformPrediction
    {
        get => State.EnableTransformPrediction;
        set => UpdateState(state => state.EnableTransformPrediction = value);
    }

    public static bool TransformPredictionAlwaysOn
    {
        get => State.TransformPredictionAlwaysOn;
        set => UpdateState(state => state.TransformPredictionAlwaysOn = value);
    }

    public static bool ShowPreviewDebugLogs
    {
        get => State.ShowPreviewDebugLogs;
        set => UpdateState(state => state.ShowPreviewDebugLogs = value);
    }

    public static void Register(string modId)
    {
        registeredModId = modId;
        settingsLocalization = RitsuLibFramework.CreateModLocalization(
            modId,
            SettingsLocalizationStem,
            Array.Empty<string>(),
            Array.Empty<string>(),
            [SettingsLocalizationPckRoot],
            typeof(SpirePlusModConfig).Assembly);

        RegisterSettingsStore(modId);
        RegisterSettingsPage(modId);
    }

    private static void RegisterSettingsStore(string modId)
    {
        // RitsuLib setting controls bind to this data key, so the store must
        // exist before the page builder wires UI entries to persisted values.
        using (RitsuLibFramework.BeginModDataRegistration(modId))
        {
            var store = RitsuLibFramework.GetDataStore(modId);
            store.Register(SettingsKey, SettingsFileName, SaveScope.Global, () => new SettingsState(), true);
        }
    }

    private static void RegisterSettingsPage(string modId)
    {
        RitsuLibFramework.RegisterModSettings(modId, page =>
        {
            page.WithModDisplayName(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithTitle(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithDescription(Text(
                "SPIREPLUS.settings_page.description",
                "RitsuLib settings page for Spire Plus private beta testing."));

            AddMigrationStatusSection(page);
            AddPreviewToolsSection(page, modId);
        });
    }

    private static void AddMigrationStatusSection(ModSettingsPageBuilder page)
    {
        // Keep these entry IDs stable. The manual screenshot checklist and
        // future automation use them to prove the RitsuLib-only settings page.
        page.AddSection(MigrationStatusSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-MIGRATION_STATUS.title", "Migration Status"));
            section.WithDescription(Text(
                "SPIREPLUS-MIGRATION_STATUS.description",
                "Read-only status for the current RitsuLib-only settings surface."));
            section.AddParagraph(
                RitsuLibSummaryEntryId,
                Text("SPIREPLUS-RITSULIB_ONLY_SUMMARY.title", "RitsuLib-only mod surface"),
                Text(
                    "SPIREPLUS-RITSULIB_ONLY_SUMMARY.description",
                    "This page is registered through RitsuLib. Spire Plus uses RitsuLib for settings persistence, content registration, patch metadata, and saved marker state."));
            section.AddInfoCard(
                RequiredRuntimeDependencyEntryId,
                Text("SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.title", "Runtime dependency"),
                LiteralText(RequiredRuntimeDependency),
                Text(
                    "SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.description",
                    "Install the runtime pack under the game mods/STS2-RitsuLib folder before enabling Spire Plus."));
            section.AddInfoCard(
                StableManifestIdEntryId,
                Text("SPIREPLUS-STABLE_MANIFEST_ID.title", "Technical id"),
                LiteralText(StableTechnicalId),
                Text(
                    "SPIREPLUS-STABLE_MANIFEST_ID.description",
                    "This id remains only for the manifest, install folder, saves, and compatibility. Player-facing UI should say Spire Plus."));
            section.AddInfoCard(
                ProofBoundaryEntryId,
                Text("SPIREPLUS-PROOF_BOUNDARY.title", "Evidence boundary"),
                Text("SPIREPLUS-PROOF_BOUNDARY.subtitle", "Settings screenshots prove UI visibility only."),
                Text(
                    "SPIREPLUS-PROOF_BOUNDARY.description",
                    "Gameplay, clicked Ancient screens, save/load, co-op, and release readiness still need separate evidence."));
        });
    }

    private static void AddPreviewToolsSection(ModSettingsPageBuilder page, string modId)
    {
        page.AddSection(PreviewToolsSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-PREVIEW_TOOLS.title", "Preview Tools"));
            section.WithDescription(Text(
                "SPIREPLUS-PREVIEW_TOOLS.description",
                "Controls for Crystal Sphere peek and transform prediction."));
            section.AddToggle(
                EnableCrystalSpherePeekEntryId,
                Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.title", "Crystal Sphere peek"),
                Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
                Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.description", "Show the peek overlay for Crystal Sphere when supported."),
                () => true);
            section.AddSlider(
                CrystalSphereMaskAlphaEntryId,
                Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.title", "Crystal Sphere mask alpha"),
                Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value)),
                CrystalSphereMaskAlphaMin,
                CrystalSphereMaskAlphaMax,
                CrystalSphereMaskAlphaStep,
                value => value.ToString("0.00", CultureInfo.InvariantCulture),
                Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.description", "Opacity of the Crystal Sphere peek mask."));
            section.AddToggle(
                EnableTransformPredictionEntryId,
                Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.title", "Transform prediction"),
                Binding(modId, state => state.EnableTransformPrediction, (state, value) => state.EnableTransformPrediction = value),
                Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.description", "Show predicted transform outcomes when supported."),
                () => true);
            section.AddToggle(
                TransformPredictionAlwaysOnEntryId,
                Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.title", "Always show transform prediction"),
                Binding(modId, state => state.TransformPredictionAlwaysOn, (state, value) => state.TransformPredictionAlwaysOn = value),
                Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.description", "Show transform prediction without requiring a modifier key."),
                () => EnableTransformPrediction);
            section.AddToggle(
                ShowPreviewDebugLogsEntryId,
                Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", "Preview debug logs"),
                Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
                Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.description", "Emit extra preview-tool diagnostics to the log."),
                () => true);
        });
    }

    private static SettingsState State
    {
        get
        {
            if (registeredModId is null || !RitsuLibFramework.IsActive)
            {
                return FallbackState;
            }

            try
            {
                return Store.Get<SettingsState>(SettingsKey);
            }
            catch
            {
                return FallbackState;
            }
        }
    }

    private static ModDataStore Store => RitsuLibFramework.GetDataStore(registeredModId ?? MainFile.ModId);

    private static void UpdateState(Action<SettingsState> update)
    {
        if (registeredModId is null || !RitsuLibFramework.IsActive)
        {
            update(FallbackState);
            return;
        }

        try
        {
            Store.Modify(SettingsKey, update);
        }
        catch
        {
            update(FallbackState);
        }
    }

    private static IModSettingsValueBinding<TValue> Binding<TValue>(
        string modId,
        Func<SettingsState, TValue> getter,
        Action<SettingsState, TValue> setter) =>
        new ModSettingsValueBinding<SettingsState, TValue>(modId, SettingsKey, SaveScope.Global, getter, setter);

    private static double NormalizeCrystalSphereMaskAlpha(double value) =>
        Math.Clamp(value, CrystalSphereMaskAlphaMin, CrystalSphereMaskAlphaMax);

    private static ModSettingsText Text(string key, string fallback) =>
        settingsLocalization is { } i18n
            ? ModSettingsText.I18N(i18n, key, fallback)
            : LiteralText(fallback);

    private static ModSettingsText LiteralText(string value) => ModSettingsText.Literal(value);

    private sealed class SettingsState
    {
        public bool EnableCrystalSpherePeek { get; set; } = true;

        public double CrystalSphereMaskAlpha { get; set; } = DefaultCrystalSphereMaskAlpha;

        public bool EnableTransformPrediction { get; set; } = true;

        public bool TransformPredictionAlwaysOn { get; set; } = true;

        public bool ShowPreviewDebugLogs { get; set; }
    }
}
