using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

/// <summary>
/// ISSUE-2026-05-08-BRIGHTEST-FLAME-CARD-EXHAUST-DRAW-ONE-MORE:
/// BrightestFlame gains Exhaust and draws 1 more card.
/// Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.
/// Modified: Gain Energy(2), Draw(3), LoseMaxHp(1); upgrade draws 4. Also Exhaust.
/// Card text is updated via BRIGHTEST_FLAME localization overrides.
/// Does not affect Pumpkin Candle relic vanilla behavior.
/// </summary>
internal sealed class BrightestFlameCanonicalKeywordsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "brightest-flame-keywords";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add Exhaust keyword to BrightestFlame";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), "get_CanonicalKeywords", HarmonyLib.MethodType.Getter)];
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

internal sealed class BrightestFlameCanonicalVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "brightest-flame-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Increase BrightestFlame draw by 1";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BrightestFlame), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
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

internal sealed class BrightestFlameExhaustOnPlayBackstopPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "brightest-flame-exhaust-backstop";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Ensure BrightestFlame exhausts on play";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))];
    private static void Prefix(CardModel __instance)
    {
        if (__instance is BrightestFlame brightestFlame)
        {
            brightestFlame.ExhaustOnNextPlay = true;
        }
    }
}
