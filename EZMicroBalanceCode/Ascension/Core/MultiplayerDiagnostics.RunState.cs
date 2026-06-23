using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Patches NGame.StartNewMultiplayerRun to log player HP immediately after
/// RunState creation but before the run launches.
/// This catches the HP state before any mod hooks fire.
/// </summary>
internal sealed class NGameStartNewMultiplayerRunDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-start-new-run";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log multiplayer RunState HP after NGame.StartNewMultiplayerRun creates the run";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(NGame),
            "StartNewMultiplayerRun",
            [
                typeof(StartRunLobby),
                typeof(bool),
                typeof(IReadOnlyList<ActModel>),
                typeof(IReadOnlyList<ModifierModel>),
                typeof(string),
                typeof(int),
                typeof(DateTimeOffset?)
            ])
    ];

    [HarmonyPostfix]
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
            MainFile.Logger.Info("[Spire Plus][MPDiag] StartNewMultiplayerRun postfix: RunState is null");
        }
    }
}

/// <summary>
/// Patches RunManager.EnterAct to log player HP before and after act entry.
/// </summary>
internal sealed class RunManagerEnterActDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-enter-act";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log multiplayer RunState HP before and after RunManager.EnterAct";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RunManager), "EnterAct", [typeof(int), typeof(bool)])];

    [HarmonyPrefix]
    private static void Prefix(RunManager __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var state = __instance.DebugOnlyGetState();
        if (state != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(state, "EnterAct prefix");
        }
    }

    [HarmonyPostfix]
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
/// Logs Neow HP before and after event startup through the AncientEventModel boundary.
/// </summary>
internal sealed class AncientEventModelBeforeEventStartedDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-ancient-event-start";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log Ancient event owner HP around BeforeEventStarted when multiplayer diagnostics are enabled";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AncientEventModel), "BeforeEventStarted")];

    [HarmonyPrefix]
    private static void Prefix(AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] AncientEventModel.BeforeEventStarted prefix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }

    [HarmonyPostfix]
    private static void Postfix(AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] AncientEventModel.BeforeEventStarted postfix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }
}
