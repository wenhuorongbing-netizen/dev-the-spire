using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[ModInitializer(nameof(Initialize))]
public static class AscensionInitializer
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            SpirePlusDebug.LogAscension("Already initialized, skipping.");
            return;
        }

        initialized = true;
        SpirePlusDebug.LogAscension("Registering run and combat hooks.");

        ModHelper.SubscribeForRunStateHooks(
            $"{MainFile.ModId}.Ascension.RunHooks",
            CreateRunHookSubscribers);

        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.Ascension.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension hooks registered. A11-A20 selection is default-on for single-player private-beta testing; multiplayer A11-A20 gameplay is fail-closed unless {MultiplayerFeaturePolicy.AllowUnverifiedCoopGameplayEnvironmentVariable}=1 is set for focused two-client debugging. Set {AscensionFeatureGate.DisablePublicSelectionEnvironmentVariable}=1 to restore vanilla A1-A10 selection, set {AscensionFeatureGate.DiagnosticsEnvironmentVariable}=1 for internal diagnostics, set {AscensionFeatureGate.MultiplayerDiagnosticsEnvironmentVariable}=1 for multiplayer run-start/Neow/save-quit diagnostics.");
        SpirePlusDebug.LogAscension("Hooks registered successfully.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState)
    {
        return AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||
               AscensionFeatureGate.IsDiagnosticsEnabled
            ? new AbstractModel[] { ModelDb.GetById<RootRunHook>(ModelDb.GetId<RootRunHook>()) }
            : Array.Empty<AbstractModel>();
    }

    private static IEnumerable<AbstractModel> CreateCombatHookSubscribers(CombatState combatState)
    {
        if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) &&
            !AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return Array.Empty<AbstractModel>();
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
                combatState.RunState,
                "AscensionCombatHooks",
                "A11-A20 combat modifiers, Blight Sprout, Rootblight, firemark, banner, and boss-seal combat hooks still need two-client proof."))
        {
            return Array.Empty<AbstractModel>();
        }

        return new AbstractModel[] { ModelDb.GetById<RootBudCombatHook>(ModelDb.GetId<RootBudCombatHook>()) };
    }
}
