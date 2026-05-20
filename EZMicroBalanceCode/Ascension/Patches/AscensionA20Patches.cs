using HarmonyLib;
using EZMicroBalance.EZMicroBalanceCode.Ascension.Events;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]
internal static class AscensionA20GenerateRoomsPatch
{
    private static void Postfix(RunManager __instance)
    {
        var runState = __instance.DebugOnlyGetState();
        if (runState == null ||
            !AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState) ||
            runState.Acts.Count == 0)
        {
            return;
        }

        var finalAct = runState.Acts[^1];
        if (finalAct.HasSecondBoss)
        {
            return;
        }

        var secondBoss = runState.Rng.UpFront.NextItem(finalAct.AllBossEncounters
            .Where(encounter => encounter.Id != finalAct.BossEncounter.Id));
        if (secondBoss == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Ascension A20 could not choose a second boss encounter for the final act.");
            return;
        }

        finalAct.SetSecondBossEncounter(secondBoss);
        ReleaseEvidenceLog.Log(
            "A20KingBrand",
            "second_boss_set",
            runState: runState,
            data: new Dictionary<string, object?>
            {
                ["encounter"] = secondBoss.Id.ToString()
            });
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A20 applied: final act second boss set to {secondBoss.Id.Entry} through the vanilla double-boss map path.");
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))]
internal static class AscensionA20CourtyardProceedPatch
{
    private static bool Prefix(RunManager __instance, ref Task __result)
    {
        var runState = __instance.DebugOnlyGetState();
        if (runState == null || !AscensionA20CourtyardService.ShouldEnterCourtyard(runState))
        {
            return true;
        }

        __result = AscensionA20CourtyardService.EnterCourtyard(__instance, runState);
        return false;
    }
}
