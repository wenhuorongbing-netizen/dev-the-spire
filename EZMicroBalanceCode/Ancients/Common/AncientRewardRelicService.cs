namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Common;

internal static class AncientRewardRelicService
{
    public static async Task ObtainSelectionRelicIfMissing<T>(Player owner, string rewardId)
        where T : RelicModel
    {
        if (owner.GetRelic<T>() is not null)
        {
            MainFile.Logger.Info($"[EZMicroBalance] Ancient reward relic already present for {rewardId}: {typeof(T).Name}.");
            return;
        }

        var relic = ModelDb.Relic<T>().ToMutable();
        await RelicCmd.Obtain(relic, owner);
        MainFile.Logger.Info($"[EZMicroBalance] Ancient reward relic obtained for {rewardId}: {relic.Id}.");
    }
}
