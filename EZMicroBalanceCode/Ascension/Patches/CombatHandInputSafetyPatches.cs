using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand._UnhandledInput))]
internal static class CombatHandInputSafetyPatch
{
    private static Exception? Finalizer(Exception? __exception)
    {
        if (__exception is ArgumentOutOfRangeException ex)
        {
            MainFile.Logger.Warn(
                $"[Spire Plus] Ignored stale combat hand shortcut input after the hand changed during card play: {ex.Message}");
            return null;
        }

        return __exception;
    }
}
