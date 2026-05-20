using MegaCrit.Sts2.Core.Entities.Ancients;
using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    private static readonly ConditionalWeakTable<IRunState, ParentRestoreHealSkipState> ParentRestoreHealSkips = new();

    public static void PreserveParentEventForPreFinishedSave(
        CombatRoom combatRoom,
        SerializableRoom serializableRoom)
    {
        if (combatRoom.Encounter is not EzmbVakuuTrialEncounter ||
            !combatRoom.IsPreFinished ||
            !combatRoom.ShouldResumeParentEventAfterCombat ||
            serializableRoom.ParentEventId is not null)
        {
            return;
        }

        serializableRoom.ParentEventId =
            ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>().Id;
        serializableRoom.ShouldResumeParentEvent = true;
        MainFile.Logger.Info("[EZMicroBalance] Vakuu pre-finished fight save records Vakuu as the resume parent event.");
    }

    public static void ArmPrefinishedParentRestoreHealSkip(
        EventRoom eventRoom,
        IRunState? runState,
        bool isRestoringRoomStackBase)
    {
        if (!isRestoringRoomStackBase ||
            eventRoom.CanonicalEvent is not MegaCrit.Sts2.Core.Models.Events.Vakuu)
        {
            return;
        }

        if (runState == null)
        {
            return;
        }

        var state = ParentRestoreHealSkips.GetOrCreateValue(runState);
        state.RemainingSkips = Math.Max(state.RemainingSkips, Math.Max(1, runState.Players.Count));
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore armed duplicate Ancient heal skip for the parent event.");
    }

    public static bool ShouldSkipPrefinishedParentRestoreHeal(
        AncientEventModel ancient,
        bool isPreFinished)
    {
        if (isPreFinished ||
            ancient is not MegaCrit.Sts2.Core.Models.Events.Vakuu ||
            ancient.Owner?.RunState is not { } runState ||
            !ParentRestoreHealSkips.TryGetValue(runState, out var state) ||
            state.RemainingSkips <= 0)
        {
            return false;
        }

        state.RemainingSkips--;
        if (state.RemainingSkips <= 0)
        {
            ParentRestoreHealSkips.Remove(runState);
        }

        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore skipped duplicate Ancient heal on the reconstructed parent event.");
        return true;
    }

    private sealed class ParentRestoreHealSkipState
    {
        public int RemainingSkips { get; set; }
    }
}
