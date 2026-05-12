using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaInitializer
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
            $"{MainFile.ModId}.Urda.RunHooks",
            CreateRunHookSubscribers);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda hooks registered. Urda is default-on for private-beta testing; set {UrdaFeatureGate.DisableAncientEnvironmentVariable}=1 to hide it for comparison.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        UrdaFeatureGate.IsUrdaEnabled(runState.UnlockState)
            ? [ModelDb.GetById<UrdaRunHook>(ModelDb.GetId<UrdaRunHook>())]
            : [];
}
