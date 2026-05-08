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
            return;
        }

        initialized = true;

        ModHelper.SubscribeForRunStateHooks(
            $"{MainFile.ModId}.Ascension.RunHooks",
            CreateRunHookSubscribers);

        ModHelper.SubscribeForCombatStateHooks(
            $"{MainFile.ModId}.Ascension.CombatHooks",
            CreateCombatHookSubscribers);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension hooks registered. A11-A20 selection is default-on for private-beta multiplayer testing; set {AscensionFeatureGate.DisablePublicSelectionEnvironmentVariable}=1 to restore vanilla A1-A10 selection, set {AscensionFeatureGate.DisableMultiplayerSelectionEnvironmentVariable}=1 to disable only host-multiplayer A11-A20 selection, set {AscensionFeatureGate.DiagnosticsEnvironmentVariable}=1 for internal diagnostics, set {AscensionFeatureGate.MultiplayerDiagnosticsEnvironmentVariable}=1 for multiplayer run-start/Neow/save-quit diagnostics.");
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
        return AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) ||
               AscensionFeatureGate.IsDiagnosticsEnabled
            ? new AbstractModel[] { ModelDb.GetById<RootBudCombatHook>(ModelDb.GetId<RootBudCombatHook>()) }
            : Array.Empty<AbstractModel>();
    }
}
