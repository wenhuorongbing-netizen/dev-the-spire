namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

[HarmonyPatch(typeof(LocString), nameof(LocString.GetRawText))]
internal static class AscensionLocalizationLocStringRawTextPatch
{
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

[HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))]
internal static class AscensionLocalizationGetTablePatch
{
    private static void Postfix(string name, LocTable __result)
    {
        if (name.Equals("ascension", StringComparison.Ordinal))
        {
            AscensionLocalizationBridge.MergeIntoIfAscensionTable(__result);
        }
    }
}

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
internal static class AscensionLocalizationRawTextPatch
{
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

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))]
internal static class AscensionLocalizationLocStringPatch
{
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

[HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))]
internal static class AscensionLocalizationHasEntryPatch
{
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && AscensionLocalizationBridge.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))]
internal static class AscensionLocalizationIsLocalKeyPatch
{
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && AscensionLocalizationBridge.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}
