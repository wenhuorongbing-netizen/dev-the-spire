using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static class MorviInitializer
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
            $"{MainFile.ModId}.Morvi.RunHooks",
            CreateRunHookSubscribers);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Morvi v2.2 hooks registered default-on; set {MorviFeatureGate.DisableEnvironmentVariable}=1 or {MorviFeatureGate.SpirePlusDisableEnvironmentVariable}=1 to disable.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        MorviFeatureGate.IsMorviEnabled(runState.UnlockState)
            ? [ModelDb.GetById<MorviRunHook>(ModelDb.GetId<MorviRunHook>())]
            : [];
}
