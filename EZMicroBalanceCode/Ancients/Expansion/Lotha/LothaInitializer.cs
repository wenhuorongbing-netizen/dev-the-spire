using EZMicroBalance.EZMicroBalanceCode.Ascension;
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

        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.Lotha.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[Spire Plus] Lotha hooks registered default-on; set {LothaFeatureGate.DisableEnvironmentVariable}=1 to disable. Legacy alias: {LothaFeatureGate.LegacyDisableEnvironmentVariable}=1.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        LothaFeatureGate.IsLothaEnabled(runState.UnlockState)
            ? [ModelDb.GetById<LothaRunHook>(ModelDb.GetId<LothaRunHook>())]
            : [];

    private static IEnumerable<AbstractModel> CreateCombatHookSubscribers(CombatState combatState)
    {
        if (!LothaFeatureGate.IsLothaEnabled(combatState.RunState.UnlockState))
        {
            return [];
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
                combatState.RunState,
                "LothaCombatHooks",
                "Lotha combat card, power, and death-prevention hooks still need two-client proof."))
        {
            return [];
        }

        return [ModelDb.GetById<LothaCombatHook>(ModelDb.GetId<LothaCombatHook>())];
    }
}
