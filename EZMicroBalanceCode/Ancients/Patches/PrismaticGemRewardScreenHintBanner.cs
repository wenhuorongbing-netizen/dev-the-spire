namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed partial class PrismaticGemRewardScreenHintPatch
{
    private const string BannerNodePath = "UI/Banner";

    private static bool BannerUnavailableLogged;

    private static void InfoOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Info(message);
    }

    private static void WarnOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Warn(message);
    }
}
