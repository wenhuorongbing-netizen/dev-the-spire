using System.Globalization;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static class SpirePlusModConfig
{
    private const string SettingsKey = "spire-plus-settings";
    private const string SettingsFileName = "spire-plus-settings.json";
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

    // RitsuLib owns the persisted settings store. The fallback is only used while
    // the framework is unavailable during early startup or when a store read fails.
    private static readonly SettingsState FallbackState = new();
    private static string? registeredModId;

    public static bool EnableCrystalSpherePeek
    {
        get => State.EnableCrystalSpherePeek;
        set => UpdateState(state => state.EnableCrystalSpherePeek = value);
    }

    public static double CrystalSphereMaskAlpha
    {
        get => State.CrystalSphereMaskAlpha;
        set => UpdateState(state => state.CrystalSphereMaskAlpha = Math.Clamp(value, 0.05, 0.95));
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
            page.WithModDisplayName(Text("Spire Plus"));
            page.WithTitle(Text("Spire Plus"));
            page.WithDescription(Text("RitsuLib settings page for Spire Plus private beta testing."));

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
            section.WithTitle(Text("Migration Status"));
            section.WithDescription(Text("Read-only status for the current RitsuLib-only settings surface."));
            section.AddParagraph(
                RitsuLibSummaryEntryId,
                Text("RitsuLib-only mod surface"),
                Text("This page is registered through RitsuLib. Spire Plus uses RitsuLib for settings persistence, content registration, patch metadata, and saved marker state."));
            section.AddInfoCard(
                RequiredRuntimeDependencyEntryId,
                Text("Runtime dependency"),
                Text(RequiredRuntimeDependency),
                Text("Install the runtime pack under the game mods/STS2-RitsuLib folder before enabling Spire Plus."));
            section.AddInfoCard(
                StableManifestIdEntryId,
                Text("Technical id"),
                Text(StableTechnicalId),
                Text("This id remains only for the manifest, install folder, saves, and compatibility. Player-facing UI should say Spire Plus."));
            section.AddInfoCard(
                ProofBoundaryEntryId,
                Text("Evidence boundary"),
                Text("Settings screenshots prove UI visibility only."),
                Text("Gameplay, clicked Ancient screens, save/load, co-op, and release readiness still need separate evidence."));
        });
    }

    private static void AddPreviewToolsSection(ModSettingsPageBuilder page, string modId)
    {
        page.AddSection(PreviewToolsSectionId, section =>
        {
            section.WithTitle(Text("Preview Tools"));
            section.WithDescription(Text("Controls for Crystal Sphere peek and transform prediction."));
            section.AddToggle(
                EnableCrystalSpherePeekEntryId,
                Text("Crystal Sphere peek"),
                Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
                Text("Show the peek overlay for Crystal Sphere when supported."),
                () => true);
            section.AddSlider(
                CrystalSphereMaskAlphaEntryId,
                Text("Crystal Sphere mask alpha"),
                Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = Math.Clamp(value, 0.05, 0.95)),
                0.05,
                0.95,
                0.05,
                value => value.ToString("0.00", CultureInfo.InvariantCulture),
                Text("Opacity of the Crystal Sphere peek mask."));
            section.AddToggle(
                EnableTransformPredictionEntryId,
                Text("Transform prediction"),
                Binding(modId, state => state.EnableTransformPrediction, (state, value) => state.EnableTransformPrediction = value),
                Text("Show predicted transform outcomes when supported."),
                () => true);
            section.AddToggle(
                TransformPredictionAlwaysOnEntryId,
                Text("Always show transform prediction"),
                Binding(modId, state => state.TransformPredictionAlwaysOn, (state, value) => state.TransformPredictionAlwaysOn = value),
                Text("Show transform prediction without requiring a modifier key."),
                () => EnableTransformPrediction);
            section.AddToggle(
                ShowPreviewDebugLogsEntryId,
                Text("Preview debug logs"),
                Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
                Text("Emit extra preview-tool diagnostics to the log."),
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

    private static ModSettingsText Text(string value) => ModSettingsText.Literal(value);

    private sealed class SettingsState
    {
        public bool EnableCrystalSpherePeek { get; set; } = true;

        public double CrystalSphereMaskAlpha { get; set; } = 0.32;

        public bool EnableTransformPrediction { get; set; } = true;

        public bool TransformPredictionAlwaysOn { get; set; } = true;

        public bool ShowPreviewDebugLogs { get; set; }
    }
}
