namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Preview values are normalized before persistence and before RitsuLib
    // slider display so saved settings cannot drift outside the visible UI.
    private static double NormalizeCrystalSphereMaskAlpha(double value) =>
        Math.Clamp(value, CrystalSphereMaskAlphaMin, CrystalSphereMaskAlphaMax);
}
