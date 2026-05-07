using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EzDailyContent.EzDailyContentCode;

[HarmonyPatch(typeof(AncientEventModel), "GenerateInitialOptionsWrapper")]
internal static class AncientRewardNoopProbe
{
    private const string ProbeEnvironmentFlag = "EZ_MICRO_BALANCE_DEBUG_PROBES";

    private static readonly string[] ProbePropertyNames =
    [
        "Id",
        "Name",
        "Entry",
        "TextKey",
        "LocalizationKey",
        "Rarity",
        "Pool"
    ];

    [HarmonyPostfix]
    private static void Postfix(AncientEventModel __instance, IReadOnlyList<EventOption> __result)
    {
        if (Environment.GetEnvironmentVariable(ProbeEnvironmentFlag) != "1")
        {
            return;
        }

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
                string relicSummary = DescribeProbeProperties(option.Relic);

                MainFile.Logger.Info(
                    $"[AncientRewardNoopProbe] OptionIndex={i}; OptionType={optionType}; TextKey={textKey}; " +
                    $"RelicIsNull={relicIsNull}; IsLocked={option.IsLocked}; IsProceed={option.IsProceed}; " +
                    $"ShouldSaveChoiceToHistory={option.ShouldSaveChoiceToHistory}; Relic={relicSummary}");
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

    private static string DescribeProbeProperties(object? value)
    {
        if (value is null)
        {
            return "<null>";
        }

        Type type = value.GetType();
        List<string> parts = [$"Type={type.FullName ?? type.Name}"];

        foreach (string propertyName in ProbePropertyNames)
        {
            PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property?.GetMethod is null || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            try
            {
                object? propertyValue = property.GetValue(value);
                parts.Add($"{propertyName}={FormatProbeValue(propertyValue)}");
            }
            catch (Exception ex)
            {
                parts.Add($"{propertyName}=<error:{ex.GetType().Name}>");
            }
        }

        return string.Join(", ", parts);
    }

    private static string FormatProbeValue(object? value)
    {
        if (value is null)
        {
            return "<null>";
        }

        string text = value.ToString() ?? "<null>";
        return text.Length <= 160 ? text : $"{text[..160]}...";
    }
}
