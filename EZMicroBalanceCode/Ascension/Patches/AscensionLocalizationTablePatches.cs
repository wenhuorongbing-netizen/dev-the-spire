namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Patching.Models;

internal sealed class AscensionLocalizationLocStringRawTextPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-locstring-raw-text";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Resolve A11-A20 ascension LocString text before Core can show raw placeholder keys";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocString), nameof(LocString.GetRawText))];

    [HarmonyPrefix]
    private static bool Prefix(LocString __instance, ref string __result)
    {
        if (__instance.LocTable.Equals("ascension", StringComparison.Ordinal) &&
            AscensionLocalizationBridge.IsAscensionLevelKey(__instance.LocEntryKey) &&
            AscensionLocalizationBridge.TryGetText(__instance.LocEntryKey, out var text))
        {
            // The character-select Ascension panel formats a LocString very
            // early. Resolve A11-A20 before Core's table can leak placeholder
            // keys such as ascension.LEVEL_20.title into player-facing UI.
            __result = text;
            return false;
        }

        return true;
    }
}

internal sealed class AscensionLocalizationGetTablePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-get-table";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Merge Spire Plus A11-A20 entries into the Core ascension localization table";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocManager), nameof(LocManager.GetTable))];

    [HarmonyPostfix]
    private static void Postfix(string name, LocTable __result)
    {
        if (name.Equals("ascension", StringComparison.Ordinal))
        {
            AscensionLocalizationBridge.MergeIntoIfAscensionTable(__result);
        }
    }
}

internal sealed class AscensionLocalizationRawTextPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-raw-text";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Recover known A11-A20 ascension raw text after a Core localization miss";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetRawText))];

    [HarmonyFinalizer]
    private static Exception? Finalizer(LocTable __instance, string key, ref string __result, Exception? __exception)
    {
        if (__exception == null)
        {
            return null;
        }

        if (__exception is LocException &&
            AscensionLocalizationBridge.TryGetText(__instance, key, out var text))
        {
            __result = text;
            return null;
        }

        return __exception;
    }
}

internal sealed class AscensionLocalizationLocStringPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-loc-string";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Recover LocString handles for known A11-A20 ascension bridge keys";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.GetLocString))];

    [HarmonyFinalizer]
    private static Exception? Finalizer(LocTable __instance, string key, ref LocString? __result, Exception? __exception)
    {
        if (__exception == null)
        {
            return null;
        }

        if (__exception is LocException &&
            AscensionLocalizationBridge.TryGetText(__instance, key, out _))
        {
            __result = new LocString("ascension", key);
            return null;
        }

        return __exception;
    }
}

internal sealed class AscensionLocalizationHasEntryPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-has-entry";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Report known A11-A20 ascension bridge keys as present in the ascension table";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.HasEntry))];

    [HarmonyPostfix]
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && AscensionLocalizationBridge.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}

internal sealed class AscensionLocalizationIsLocalKeyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-localization-is-local-key";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Keep known A11-A20 ascension bridge keys in the current locale rather than English fallback formatting";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(LocTable), nameof(LocTable.IsLocalKey))];

    [HarmonyPostfix]
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && AscensionLocalizationBridge.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}
