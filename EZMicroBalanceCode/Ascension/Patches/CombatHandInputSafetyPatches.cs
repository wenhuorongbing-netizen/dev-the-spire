using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class CombatHandInputSafetyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "combat-hand-input-safety";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Ignore the observed stale combat hand shortcut input exception while preserving all other input exceptions";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NPlayerHand), nameof(NPlayerHand._UnhandledInput), [typeof(InputEvent)])];

    [HarmonyFinalizer]
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
