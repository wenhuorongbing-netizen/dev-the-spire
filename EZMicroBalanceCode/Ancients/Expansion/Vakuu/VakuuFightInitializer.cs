using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuFightInitializer
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.VakuuFight.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[Spire Plus] Vakuu fight hooks registered but hidden by default; set {VakuuFightFeatureGate.EnableEnvironmentVariable}=1 to opt in, or {VakuuFightFeatureGate.ForceFightEnvironmentVariable}=1 for focused debugging. Legacy aliases: {VakuuFightFeatureGate.LegacyEnableEnvironmentVariable}=1 / {VakuuFightFeatureGate.LegacyForceFightEnvironmentVariable}=1.");
    }

    private static IEnumerable<AbstractModel> CreateCombatHookSubscribers(CombatState combatState)
    {
        if (!VakuuFightFeatureGate.IsFightEnabledForRun(combatState.RunState))
        {
            return [];
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
                combatState.RunState,
                "VakuuFightCombatHooks",
                "Vakuu child combat is still single-player only and needs victory, failure, save-load, and two-client proof."))
        {
            return [];
        }

        return [ModelDb.GetById<VakuuFightCombatHook>(ModelDb.GetId<VakuuFightCombatHook>())];
    }
}
