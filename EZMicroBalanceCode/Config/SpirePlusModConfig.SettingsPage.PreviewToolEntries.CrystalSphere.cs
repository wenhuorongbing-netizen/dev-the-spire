using System.Globalization;
using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Crystal Sphere entries stay together because the toggle and opacity
    // slider share the same local UI-only preview contract.
    private static void AddCrystalSpherePeekToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            EnableCrystalSpherePeekEntryId,
            Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.title", "Crystal Sphere peek"),
            Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
            Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.description", "Show the peek overlay for Crystal Sphere when supported."),
            () => true);

    private static void AddCrystalSphereMaskAlphaSlider(ModSettingsSectionBuilder section, string modId) =>
        section.AddSlider(
            CrystalSphereMaskAlphaEntryId,
            Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.title", "Crystal Sphere mask alpha"),
            Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value)),
            CrystalSphereMaskAlphaMin,
            CrystalSphereMaskAlphaMax,
            CrystalSphereMaskAlphaStep,
            value => value.ToString("0.00", CultureInfo.InvariantCulture),
            Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.description", "Opacity of the Crystal Sphere peek mask."));
}
