using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]
internal static class VakuuForceAncientPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result)
    {
        if (!VakuuFightFeatureGate.ShouldForceVakuu)
        {
            return;
        }

        __result = [ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>()];
        MainFile.Logger.Info("[Spire Plus] Force Ancient gate selected Vakuu as the Act 3 Ancient.");
    }
}

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), "GenerateInitialOptions")]
internal static class VakuuFightOptionPatch
{
    [HarmonyPostfix]
    private static void AddFightOption(
        MegaCrit.Sts2.Core.Models.Events.Vakuu __instance,
        ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner?.RunState is not RunState runState ||
            runState.Players.Count != 1)
        {
            return;
        }

        var forceFight = VakuuFightFeatureGate.ShouldForceFightForRun(runState);
        if (!VakuuFightFeatureGate.IsFightEnabledForRun(runState, forceFight))
        {
            return;
        }

        var fightOption = VakuuFightService.CreateFightOption(__instance);
        if (forceFight)
        {
            VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState);
            __result = [fightOption];
            return;
        }

        __result = __result.Concat([fightOption]).ToList();
    }
}

[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]
internal static class VakuuFightCommandForceCleanupPatch
{
    [HarmonyPostfix]
    private static void ClearCommandForceFightWhenVakuuBeginEventCompletes(
        EventModel __instance,
        ref Task __result)
    {
        if (__instance is not MegaCrit.Sts2.Core.Models.Events.Vakuu ||
            __instance.Owner?.RunState is not RunState runState ||
            !VakuuFightFeatureGate.HasCommandForceFightForRun(runState))
        {
            return;
        }

        __result = VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState);
    }
}

[HarmonyPatch(typeof(EventModel), nameof(EventModel.Resume))]
internal static class VakuuFightResumePatch
{
    [HarmonyPrefix]
    private static bool ResumeVakuuFightVictory(EventModel __instance, AbstractRoom exitedRoom, ref Task __result)
    {
        if (__instance is not MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu ||
            exitedRoom is not CombatRoom { Encounter: EzmbVakuuTrialEncounter } combatRoom)
        {
            return true;
        }

        __result = VakuuFightService.ResumeAfterVictory(vakuu, combatRoom);
        return false;
    }
}

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]
internal static class VakuuFightPreFinishedSavePatch
{
    [HarmonyPostfix]
    private static void PreserveVakuuParentForPreFinishedSave(
        CombatRoom __instance,
        SerializableRoom __result) =>
        VakuuFightService.PreserveParentEventForPreFinishedSave(__instance, __result);
}

[HarmonyPatch(typeof(EventRoom), nameof(EventRoom.EnterInternal))]
internal static class VakuuFightPreFinishedParentRestorePatch
{
    [HarmonyPrefix]
    private static void ArmVakuuPreFinishedParentRestoreHealSkip(
        EventRoom __instance,
        IRunState? runState,
        bool isRestoringRoomStackBase) =>
        VakuuFightService.ArmPrefinishedParentRestoreHealSkip(
            __instance,
            runState,
            isRestoringRoomStackBase);
}

[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]
internal static class VakuuFightPreFinishedParentRestoreHealPatch
{
    [HarmonyPrefix]
    private static bool SkipDuplicateVakuuRestoreHeal(
        AncientEventModel __instance,
        bool isPreFinished,
        ref Task __result)
    {
        if (!VakuuFightService.ShouldSkipPrefinishedParentRestoreHeal(__instance, isPreFinished))
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]
internal static class VakuuFightNoRewardRestorePatch
{
    [HarmonyPrefix]
    private static bool SkipVakuuLoadedTerminalRewards(CombatRoom __instance, ref Task __result)
    {
        if (__instance.Encounter is not EzmbVakuuTrialEncounter)
        {
            return true;
        }

        __result = VakuuFightService.ProceedFromNoRewardVictory(__instance);
        return false;
    }
}
