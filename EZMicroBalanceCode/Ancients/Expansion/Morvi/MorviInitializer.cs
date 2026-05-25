using EZMicroBalance.EZMicroBalanceCode.Ascension;
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

        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.Morvi.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[Spire Plus] Morvi v2.2 hooks registered default-on; set {MorviFeatureGate.DisableEnvironmentVariable}=1 to disable. Legacy alias: {MorviFeatureGate.LegacyDisableEnvironmentVariable}=1.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        MorviFeatureGate.IsMorviEnabled(runState.UnlockState)
            ? [ModelDb.GetById<MorviRunHook>(ModelDb.GetId<MorviRunHook>())]
            : [];

    private static IEnumerable<AbstractModel> CreateCombatHookSubscribers(CombatState combatState)
    {
        if (!MorviFeatureGate.IsMorviEnabled(combatState.RunState.UnlockState))
        {
            return [];
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
                combatState.RunState,
                "MorviCombatHooks",
                "Morvi combat card, pile, and power hooks still need two-client proof."))
        {
            return [];
        }

        return [ModelDb.GetById<MorviCombatHook>(ModelDb.GetId<MorviCombatHook>())];
    }
}
