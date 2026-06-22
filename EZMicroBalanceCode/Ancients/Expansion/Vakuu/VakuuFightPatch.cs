using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuForceAncientPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "vakuu-force-ancient-unlock";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Force Vakuu into the Act 3 Ancient event offer list for test gates";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Glory), nameof(Glory.GetUnlockedAncients))];

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

internal sealed class VakuuFightOptionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "vakuu-fight-option";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Append or force the gated Vakuu fight option in the Ancient event UI";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), "GenerateInitialOptions")];

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

internal sealed class VakuuFightCommandForceCleanupPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "vakuu-fight-command-force-cleanup";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Clear the one-shot command-force Vakuu fight flag after the event begins";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(EventModel), nameof(EventModel.BeginEvent), [typeof(Player), typeof(bool)])];

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

internal sealed class VakuuFightResumePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "vakuu-fight-victory-resume";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Resume the parent Vakuu event after the gated fight victory room";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(EventModel), nameof(EventModel.Resume), [typeof(AbstractRoom)])];

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

internal sealed class VakuuFightPreFinishedParentRestoreHealPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "vakuu-fight-prefinished-parent-heal-skip";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Skip duplicate Ancient heal when restoring Vakuu's prefinished parent event";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AncientEventModel), "BeforeEventStarted", [typeof(bool)])];

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
