using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using STS2RitsuLib.Patching.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaSeedbedCardPileDrawPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-seedbed-card-pile-draw";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Scope Urda Seedbed planting state around the async CardPileCmd.Draw path";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(CardPileCmd),
            nameof(CardPileCmd.Draw),
            [typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool)])
    ];

    [HarmonyPrefix]
    private static void Prefix(Player player)
    {
        UrdaBlessingService.BeginSeedbedDraw(player);
    }

    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task<IEnumerable<CardModel>> __result)
    {
        if (__result != null)
        {
            __result = WrapDrawTask(__result, player);
        }
        else
        {
            UrdaBlessingService.EndSeedbedDraw(player);
        }
    }

    [HarmonyFinalizer]
    private static void Finalizer(Player player, Exception __exception)
    {
        if (__exception != null)
        {
            UrdaBlessingService.EndSeedbedDraw(player);
        }
    }

    private static async Task<IEnumerable<CardModel>> WrapDrawTask(Task<IEnumerable<CardModel>> task, Player player)
    {
        try
        {
            return await task;
        }
        finally
        {
            UrdaBlessingService.EndSeedbedDraw(player);
        }
    }
}
