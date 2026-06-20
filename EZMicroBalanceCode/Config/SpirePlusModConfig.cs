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

        var store = RitsuLibFramework.GetDataStore(modId);
        store.Register(SettingsKey, SettingsFileName, SaveScope.Global, () => new SettingsState(), true);
        store.InitializeGlobal();

        RitsuLibFramework.RegisterModSettings(modId, page =>
        {
            page.WithModDisplayName(Text("Spire Plus"));
            page.WithTitle(Text("Spire Plus"));
            page.WithDescription(Text("Preview tools and diagnostics for Spire Plus private beta testing."));

            page.AddSection("preview_tools", section =>
            {
                section.WithTitle(Text("Preview Tools"));
                section.WithDescription(Text("Controls for Crystal Sphere peek and transform prediction."));
                section.AddToggle(
                    "enable_crystal_sphere_peek",
                    Text("Crystal Sphere peek"),
                    Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
                    Text("Show the peek overlay for Crystal Sphere when supported."),
                    () => true);
                section.AddSlider(
                    "crystal_sphere_mask_alpha",
                    Text("Crystal Sphere mask alpha"),
                    Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = Math.Clamp(value, 0.05, 0.95)),
                    0.05,
                    0.95,
                    0.05,
                    value => value.ToString("0.00", CultureInfo.InvariantCulture),
                    Text("Opacity of the Crystal Sphere peek mask."));
                section.AddToggle(
                    "enable_transform_prediction",
                    Text("Transform prediction"),
                    Binding(modId, state => state.EnableTransformPrediction, (state, value) => state.EnableTransformPrediction = value),
                    Text("Show predicted transform outcomes when supported."),
                    () => true);
                section.AddToggle(
                    "transform_prediction_always_on",
                    Text("Always show transform prediction"),
                    Binding(modId, state => state.TransformPredictionAlwaysOn, (state, value) => state.TransformPredictionAlwaysOn = value),
                    Text("Show transform prediction without requiring a modifier key."),
                    () => EnableTransformPrediction);
                section.AddToggle(
                    "show_preview_debug_logs",
                    Text("Preview debug logs"),
                    Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
                    Text("Emit extra preview-tool diagnostics to the log."),
                    () => true);
            });
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
