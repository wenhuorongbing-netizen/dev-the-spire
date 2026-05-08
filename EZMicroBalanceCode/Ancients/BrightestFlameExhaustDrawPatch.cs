using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

/// <summary>
/// ISSUE-2026-05-08-QUALITY-FLAME-CARD-EXHAUST-DRAW-ONE-MORE:
/// BrightestFlame gains Exhaust and draws 1 more card.
/// Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.
/// Modified: Gain Energy(2), Draw(3), LoseMaxHp(1); upgrade draws 4. Also Exhaust.
/// Card text is updated via BRIGHTEST_FLAME localization overrides.
/// Does not affect Pumpkin Candle relic vanilla behavior.
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]
internal static class BrightestFlameCanonicalKeywordsPatch
{
    [HarmonyPostfix]
    private static void AddVisibleExhaustKeyword(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (__instance is not BrightestFlame)
        {
            return;
        }

        if (!__result.Contains(CardKeyword.Exhaust))
        {
            __result = __result.Append(CardKeyword.Exhaust).ToArray();
        }
    }
}

[HarmonyPatch(typeof(BrightestFlame), "get_CanonicalVars")]
internal static class BrightestFlameCanonicalVarsPatch
{
    private const int ExtraDraw = 1;

    [HarmonyPostfix]
    private static void AddOneToBaseDraw(ref IEnumerable<DynamicVar> __result)
    {
        __result = __result
            .Select(dynamicVar => dynamicVar is CardsVar cards
                ? new CardsVar(cards.IntValue + ExtraDraw)
                : dynamicVar)
            .ToArray();
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal static class BrightestFlameExhaustOnPlayBackstopPatch
{
    private static void Prefix(CardModel __instance)
    {
        if (__instance is BrightestFlame brightestFlame)
        {
            brightestFlame.ExhaustOnNextPlay = true;
        }
    }
}
