using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
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
            SpirePlusDebug.LogAncient("Urda", "Already initialized, skipping.");
            return;
        }

        initialized = true;
        SpirePlusDebug.LogAncient("Urda", "Registering run and combat hooks.");

        ModHelper.SubscribeForRunStateHooks(
            $"{MainFile.ModId}.Urda.RunHooks",
            CreateRunHookSubscribers);

        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.Urda.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[Spire Plus] Urda hooks registered. Urda is default-on for private-beta testing; set {UrdaFeatureGate.DisableAncientEnvironmentVariable}=1 to hide it for comparison. Legacy alias: {UrdaFeatureGate.LegacyDisableAncientEnvironmentVariable}=1.");
        SpirePlusDebug.LogAncient("Urda", "Hooks registered successfully.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        UrdaFeatureGate.IsUrdaEnabled(runState.UnlockState) &&
        !MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
            runState,
            "UrdaRunHooks",
            "Urda reward, map, room, Seed Bank, Root Sight, and relic-state mutations are disabled in co-op until host-authoritative sync is proven.")
            ? [ModelDb.GetById<UrdaRunHook>(ModelDb.GetId<UrdaRunHook>())]
            : [];

    private static IEnumerable<AbstractModel> CreateCombatHookSubscribers(CombatState combatState)
    {
        if (!UrdaFeatureGate.IsUrdaEnabled(combatState.RunState.UnlockState))
        {
            return [];
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
                combatState.RunState,
                "UrdaCombatHooks",
                "Urda combat card, Seedbed, Seed Bank, Root Sight, and Rooted Route hooks still need two-client proof."))
        {
            return [];
        }

        return [ModelDb.GetById<UrdaCombatHook>(ModelDb.GetId<UrdaCombatHook>())];
    }
}
