using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Hooks;
using STS2RitsuLib.Patching.Models;
using System.Threading.Tasks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaSeedbedAfterCardDrawnPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-seedbed-after-card-drawn";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Skip recursive AfterCardDrawn hooks for cards Urda Seedbed just planted";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(Hook),
            nameof(Hook.AfterCardDrawn),
            [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool)])
    ];

    [HarmonyPrefix]
    private static bool Prefix(CardModel card, ref Task __result)
    {
        if (!UrdaBlessingService.WasPlantedBySeedbed(card))
        {
            return true;
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed skipped AfterCardDrawn hooks for planted card {card.Id.Entry}.");
        __result = Task.CompletedTask;
        return false;
    }
}
