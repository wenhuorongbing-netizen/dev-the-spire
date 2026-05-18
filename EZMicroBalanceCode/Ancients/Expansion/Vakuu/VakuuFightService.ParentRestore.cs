using MegaCrit.Sts2.Core.Entities.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    private static int pendingVakuuPrefinishedParentRestoreHealSkips;

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

        pendingVakuuPrefinishedParentRestoreHealSkips = Math.Max(
            pendingVakuuPrefinishedParentRestoreHealSkips,
            Math.Max(1, runState?.Players.Count ?? 1));
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore armed duplicate Ancient heal skip for the parent event.");
    }

    public static bool ShouldSkipPrefinishedParentRestoreHeal(
        AncientEventModel ancient,
        bool isPreFinished)
    {
        if (isPreFinished ||
            pendingVakuuPrefinishedParentRestoreHealSkips <= 0 ||
            ancient is not MegaCrit.Sts2.Core.Models.Events.Vakuu)
        {
            return false;
        }

        pendingVakuuPrefinishedParentRestoreHealSkips--;
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore skipped duplicate Ancient heal on the reconstructed parent event.");
        return true;
    }
}
