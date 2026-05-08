using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

/// <summary>
/// ISSUE-2026-05-08-QUALITY-FLAME-CARD-EXHAUST-DRAW-ONE-MORE:
/// BrightestFlame gains Exhaust and draws 1 more card.
/// Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.
/// Modified: Gain Energy(2), Draw(2+1 extra), LoseMaxHp(1); also Exhaust.
/// Card text is updated via localization overrides.
/// Does not affect Pumpkin Candle relic vanilla behavior.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal static class BrightestFlameExhaustPatch
{
    private static void Prefix(CardModel __instance)
    {
        if (__instance is BrightestFlame brightestFlame)
        {
            brightestFlame.ExhaustOnNextPlay = true;
        }
    }
}

[HarmonyPatch]
internal static class BrightestFlameExtraDrawPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.DeclaredMethod(
            typeof(BrightestFlame),
            "OnPlay",
            [typeof(PlayerChoiceContext), typeof(CardPlay)]);
    }

    private static void Postfix(BrightestFlame __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = DrawExtraAfterVanilla(__result, choiceContext, __instance);
    }

    private static async Task DrawExtraAfterVanilla(Task original, PlayerChoiceContext choiceContext, BrightestFlame card)
    {
        await original;
        await CardPileCmd.Draw(choiceContext, 1, card.Owner);
    }
}
