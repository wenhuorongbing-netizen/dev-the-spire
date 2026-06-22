using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Localization;

// Keep the LocTable fallback boundary separate from the provider registry. These
// patches are still a proposal-only RitsuLib migration area because they recover
// inline model text from Core localization misses rather than registering content.
[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetRawText))]
internal static class SpirePlusInlineLocalizationRawTextPatch
{
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

[HarmonyPatch(typeof(LocTable), nameof(LocTable.GetLocString))]
internal static class SpirePlusInlineLocalizationLocStringPatch
{
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

[HarmonyPatch(typeof(LocTable), nameof(LocTable.HasEntry))]
internal static class SpirePlusInlineLocalizationHasEntryPatch
{
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(LocTable), nameof(LocTable.IsLocalKey))]
internal static class SpirePlusInlineLocalizationIsLocalKeyPatch
{
    private static void Postfix(LocTable __instance, string key, ref bool __result)
    {
        if (!__result && SpirePlusInlineLocalizationRegistry.TryGetText(__instance, key, out _))
        {
            __result = true;
        }
    }
}
