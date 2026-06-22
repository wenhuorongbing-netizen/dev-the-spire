namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // The RitsuLib slider and fallback normalization share these bounds so
    // persisted values cannot drift outside the visible UI contract.
    private const double DefaultCrystalSphereMaskAlpha = 0.32;
    private const double CrystalSphereMaskAlphaMin = 0.05;
    private const double CrystalSphereMaskAlphaMax = 0.95;
    private const double CrystalSphereMaskAlphaStep = 0.05;
}
