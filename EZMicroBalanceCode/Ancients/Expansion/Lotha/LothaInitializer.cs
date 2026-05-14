using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static class LothaInitializer
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        ModHelper.SubscribeForRunStateHooks(
            $"{MainFile.ModId}.Lotha.RunHooks",
            CreateRunHookSubscribers);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Lotha hooks registered default-on; set {LothaFeatureGate.DisableEnvironmentVariable}=1 or {LothaFeatureGate.SpirePlusDisableEnvironmentVariable}=1 to disable.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        LothaFeatureGate.IsLothaEnabled(runState.UnlockState)
            ? [ModelDb.GetById<LothaRunHook>(ModelDb.GetId<LothaRunHook>())]
            : [];
}
