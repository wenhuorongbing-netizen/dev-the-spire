using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal sealed class TransformPredictionSelectionLifetimePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-selection-lifetime";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Clear transform prediction RNG context when the deck transform selector completes";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(CardSelectCmd),
            nameof(CardSelectCmd.FromDeckForTransformation),
            [typeof(Player), typeof(CardSelectorPrefs), typeof(Func<CardModel, CardTransformation>)])
    ];

    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task<IEnumerable<CardModel>> __result)
    {
        __result = ClearContextWhenSelectionCompletes(player, __result);
    }

    // Wrapping the task keeps context alive while the UI selector is open, then
    // clears it for success, cancel, and exception paths.
    private static async Task<IEnumerable<CardModel>> ClearContextWhenSelectionCompletes(
        Player player,
        Task<IEnumerable<CardModel>> selectionTask)
    {
        try
        {
            return await selectionTask;
        }
        finally
        {
            TransformPredictionRngContext.Clear(player);
        }
    }
}
