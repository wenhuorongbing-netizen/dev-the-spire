using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromDeckForTransformation))]
internal static class TransformPredictionSelectionLifetimePatch
{
    private static void Postfix(Player player, ref Task<IEnumerable<CardModel>> __result)
    {
        __result = ClearContextWhenSelectionCompletes(player, __result);
    }

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
