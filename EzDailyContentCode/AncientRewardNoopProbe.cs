using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EzDailyContent.EzDailyContentCode;

[HarmonyPatch(typeof(AncientEventModel), "GenerateInitialOptionsWrapper")]
internal static class AncientRewardNoopProbe
{
    [HarmonyPostfix]
    private static void Postfix(AncientEventModel __instance, IReadOnlyList<EventOption> __result)
    {
        try
        {
            string ancientType = __instance.GetType().FullName ?? __instance.GetType().Name;
            int optionCount = __result?.Count ?? 0;

            MainFile.Logger.Info($"[AncientRewardNoopProbe] AncientType={ancientType}; OptionCount={optionCount}");

            if (__result is null)
            {
                return;
            }

            for (int i = 0; i < __result.Count; i++)
            {
                EventOption option = __result[i];
                string optionType = option.GetType().FullName ?? option.GetType().Name;
                string textKey = option.TextKey ?? "<null>";
                bool relicIsNull = option.Relic is null;

                MainFile.Logger.Info(
                    $"[AncientRewardNoopProbe] OptionIndex={i}; OptionType={optionType}; TextKey={textKey}; " +
                    $"RelicIsNull={relicIsNull}; IsLocked={option.IsLocked}; IsProceed={option.IsProceed}; " +
                    $"ShouldSaveChoiceToHistory={option.ShouldSaveChoiceToHistory}");
            }
        }
        catch (Exception ex)
        {
            try
            {
                MainFile.Logger.Info($"[AncientRewardNoopProbe] Logging failed: {ex.GetType().FullName}: {ex.Message}");
            }
            catch
            {
                // Logging probe must not affect Ancient option generation.
            }
        }
    }
}
