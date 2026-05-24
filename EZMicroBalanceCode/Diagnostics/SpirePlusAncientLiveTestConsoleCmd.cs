using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
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

internal sealed class SpirePlusAncientLiveTestConsoleCmd : AbstractConsoleCmd
{
    private const string ConfirmationToken = "confirm";
    private const string FightToken = "fight";
    private const string TestSeed = "SPIREPLUS-ANCIENT-UI-TEST";

    public override string CmdName => "spireplus_test_ancient";

    public override string Args => "<URDA|MORVI|LOTHA|VAKUU> confirm [fight]";

    public override string Description =>
        "Starts an unsaved single-player Spire Plus Ancient UI test run.";

    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (args.Length < 2)
        {
            return UsageResult();
        }

        if (!args[1].Equals(ConfirmationToken, StringComparison.OrdinalIgnoreCase))
        {
            return UsageResult();
        }

        if (RunManager.Instance.IsInProgress)
        {
            return new CmdResult(
                success: false,
                "This command starts a fresh unsaved test run. Return to the main menu before using it.");
        }

        var target = ResolveTarget(args[0]);
        if (target is null)
        {
            return new CmdResult(success: false, $"Unknown Spire Plus Ancient '{args[0]}'.");
        }

        var forceFight = args.Skip(2).Any(arg => arg.Equals(FightToken, StringComparison.OrdinalIgnoreCase));
        if (forceFight && !target.Name.Equals("VAKUU", StringComparison.Ordinal))
        {
            return new CmdResult(success: false, "The optional fight token is valid only for Vakuu.");
        }

        return new CmdResult(
            StartUnsavedAncientTestRun(target, forceFight),
            success: true,
            forceFight
                ? "Starting an unsaved Spire Plus Vakuu fight UI test run."
                : $"Starting an unsaved Spire Plus {target.Name} UI test run.");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length <= 1)
        {
            return CompleteArgument(["URDA", "MORVI", "LOTHA", "VAKUU"], [], args.FirstOrDefault() ?? string.Empty);
        }

        if (args.Length == 2)
        {
            return CompleteArgument([ConfirmationToken], [args[0]], args[1]);
        }

        if (args.Length == 3 && args[0].Equals("VAKUU", StringComparison.OrdinalIgnoreCase))
        {
            return CompleteArgument([FightToken], [args[0], args[1]], args[2]);
        }

        return new CompletionResult
        {
            Type = CompletionType.Argument,
            ArgumentContext = CmdName
        };
    }

    private static CmdResult UsageResult() =>
        new(
            success: false,
            "Usage: spireplus_test_ancient <URDA|MORVI|LOTHA|VAKUU> confirm [fight]");

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
