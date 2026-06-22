using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Localization;

internal sealed class SpirePlusInlineLocalizationRawTextPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "spire-plus-inline-localization-raw-text";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Recover Spire Plus inline raw localization text after a Core table miss";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetRawText))];

    private static Exception? Finalizer(LocTable __instance, string key, ref string __result, Exception? __exception)
    {
        if (__exception == null)
        {
            return null;
        }

        if (__exception is LocException &&
            SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out var text))
        {
            __result = text;
            return null;
        }

        return __exception;
    }
}

internal sealed class SpirePlusInlineLocalizationLocStringPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "spire-plus-inline-localization-loc-string";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Recover LocString handles for Spire Plus inline localization entries";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetLocString))];

    private static Exception? Finalizer(LocTable __instance, string key, ref LocString? __result, Exception? __exception)
    {
        if (__exception == null)
        {
            return null;
        }

        if (__exception is LocException &&
            SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out _) &&
            SpirePlusInlineLocalizationRegistry.TryGetTableName(__instance, out var tableName))
        {
            __result = new LocString(tableName, key);
            return null;
        }

        return __exception;
    }
}

internal sealed class SpirePlusInlineLocalizationHasEntryPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "spire-plus-inline-localization-has-entry";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Report Spire Plus inline localization keys as present when Core asks the table";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.HasEntry))];

    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}

internal sealed class SpirePlusInlineLocalizationIsLocalKeyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "spire-plus-inline-localization-is-local-key";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Keep Spire Plus inline localization keys on the active locale table";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.IsLocalKey))];

    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}
