using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal sealed partial class SpirePlusAncientLiveTestConsoleCmd
{
    private static async Task StartUnsavedAncientTestRun(AncientUiTarget target, bool forceFight)
    {
        if (NGame.Instance is null)
        {
            throw new InvalidOperationException("NGame is not initialized.");
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Starting unsaved live-test run for {target.Name} Ancient UI evidence.");

        IRunState? commandForceFightRunState = null;
        var commandForceFightArmed = false;
        try
        {
            await NGame.Instance.StartNewSingleplayerRun(
                ModelDb.Character<Ironclad>(),
                shouldSave: false,
                ActModel.GetDefaultList(),
                Array.Empty<ModifierModel>(),
                TestSeed,
                GameMode.Standard);

            if (forceFight)
            {
                commandForceFightRunState = RunManager.Instance.DebugOnlyGetState()
                    ?? throw new InvalidOperationException("RunState is not initialized.");
                VakuuFightFeatureGate.ArmCommandForceFight(commandForceFightRunState);
                commandForceFightArmed = true;
            }

            if (target.ActIndex > 0)
            {
                await RunManager.Instance.EnterAct(target.ActIndex);
            }

            await RunManager.Instance.EnterRoomDebug(
                RoomType.Event,
                MapPointType.Ancient,
                target.Ancient,
                showTransition: true);
            commandForceFightArmed = false;
        }
        catch
        {
            if (commandForceFightArmed && commandForceFightRunState != null)
            {
                VakuuFightFeatureGate.ClearCommandForceFight(commandForceFightRunState);
            }

            throw;
        }
    }

    private static AncientUiTarget? ResolveTarget(string value)
    {
        var normalized = value.Trim().ToUpperInvariant();
        return normalized switch
        {
            "URDA" or "EZMB_URDA" => new AncientUiTarget(
                "URDA",
                0,
                ModelDb.AncientEvent<EzmbUrda>()),
            "MORVI" or "EZMB_MORVI" => new AncientUiTarget(
                "MORVI",
                1,
                ModelDb.AncientEvent<EzmbMorvi>()),
            "LOTHA" or "EZMB_LOTHA" => new AncientUiTarget(
                "LOTHA",
                2,
                ModelDb.AncientEvent<EzmbLotha>()),
            "VAKUU" or "EZMB_VAKUU" => new AncientUiTarget(
                "VAKUU",
                2,
                ModelDb.AncientEvent<Vakuu>()),
            _ => null
        };
    }

    private sealed record AncientUiTarget(string Name, int ActIndex, AncientEventModel Ancient);
}
