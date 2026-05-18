namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static LocString UrdaLoc(string suffix) =>
        new("ancients", $"EZMB_URDA.pages.INITIAL.options.{suffix}");
}
