using MegaCrit.Sts2.Core.Nodes;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Patches NGame.StartNewMultiplayerRun to log player HP immediately after
/// RunState creation but before the run launches.
/// This catches the HP state before any mod hooks fire.
/// </summary>
[HarmonyPatch(typeof(NGame), "StartNewMultiplayerRun")]
internal static class NGameStartNewMultiplayerRunDiagPatch
{
    private static void Postfix(NGame __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(runState, "StartNewMultiplayerRun postfix");
        }
        else
        {
            MainFile.Logger.Info("[EZMicroBalance][MPDiag] StartNewMultiplayerRun postfix: RunState is null");
        }
    }
}

/// <summary>
/// Patches RunManager.EnterAct to log player HP before and after act entry.
/// </summary>
[HarmonyPatch(typeof(RunManager), "EnterAct")]
internal static class RunManagerEnterActDiagPatch
{
    private static void Prefix(RunManager __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var state = __instance.DebugOnlyGetState();
        if (state != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(state, "EnterAct prefix");
        }
    }

    private static void Postfix(RunManager __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var state = __instance.DebugOnlyGetState();
        if (state != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(state, "EnterAct postfix");
        }
    }
}

/// <summary>
/// Patches the Neow BeforeEventStarted to log player HP right before and after healing.
/// We patch AncientEventModel.BeforeEventStarted since Neow inherits it.
/// </summary>
[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]
internal static class AncientEventModelBeforeEventStartedDiagPatch
{
    private static void Prefix(AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] AncientEventModel.BeforeEventStarted prefix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }

    private static void Postfix(AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] AncientEventModel.BeforeEventStarted postfix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }
}
