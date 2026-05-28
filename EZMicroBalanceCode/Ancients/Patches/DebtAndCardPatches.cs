using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class DebtAfterCreatedPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-after-created";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Configure Debt card when created";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.AfterCreated))];
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance)
    {
        if (__instance is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

internal sealed class DebtFromSavePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-from-save";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Configure Debt card when loaded from save";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.FromSerializable))];
    [HarmonyPostfix]
    private static void Postfix(CardModel __result)
    {
        if (__result is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

internal sealed class DebtKeywordsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-keywords";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Debt keywords to Exhaust only";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Debt), "get_CanonicalKeywords", HarmonyLib.MethodType.Getter)];
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new CardKeyword[] { CardKeyword.Exhaust };
        return false;
    }
}

internal sealed class DebtVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Debt vars to 5 gold";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Debt), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new GoldVar(5) };
        return false;
    }
}

internal sealed class DebtTurnEndEffectPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-turn-end-effect";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Disable Debt turn-end-in-hand effect";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Debt), "get_HasTurnEndInHandEffect", HarmonyLib.MethodType.Getter)];
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}

internal sealed class DebtTurnEndInHandPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-turn-end-in-hand";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Suppress Debt turn-end-in-hand behavior";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Debt), "OnTurnEndInHand")];
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

internal sealed class CardModelOnPlayPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "card-model-on-play";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Handle Debt and Enthralled card play effects";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), "OnPlay")];
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, CardPlay cardPlay, ref Task __result)
    {
        switch (__instance)
        {
            case Debt debt:
                __result = PlayDebt(debt);
                return false;
            case Enthralled enthralled:
                __result = PlayEnthralled(enthralled, cardPlay);
                return false;
            default:
                return true;
        }
    }

    private static Task PlayDebt(Debt debt)
    {
        debt.ExhaustOnNextPlay = true;
        MainFile.Logger.Info("[Spire Plus] Debt applied: will exhaust after play.");
        return Task.CompletedTask;
    }

    private static async Task PlayEnthralled(Enthralled enthralled, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m, ValueProp.Move, cardPlay);
        MainFile.Logger.Info("[Spire Plus] Enthralled applied: gained 10 block.");
    }
}

internal sealed class DebtExhaustPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "debt-exhaust";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Lose gold when Debt card is exhausted";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardCmd), nameof(CardCmd.Exhaust))];
    [HarmonyPostfix]
    private static void Postfix(CardModel card, ref Task __result)
    {
        if (card is Debt debt)
        {
            __result = DebtCardPatch.LoseGoldForDebt(debt, __result);
        }
    }
}

internal static class DebtCardPatch
{
    public static void ConfigureDebt(Debt debt)
    {
        AncientCardHelpers.EnsureKeywordsInitialized(debt);
        if (debt.Keywords.Contains(CardKeyword.Unplayable))
        {
            debt.RemoveKeyword(CardKeyword.Unplayable);
        }

        if (!debt.Keywords.Contains(CardKeyword.Exhaust))
        {
            debt.AddKeyword(CardKeyword.Exhaust);
        }

        if (!debt.EnergyCost.CostsX)
        {
            debt.EnergyCost.SetCustomBaseCost(1);
        }
    }

    public static async Task LoseGoldForDebt(Debt debt, Task originalExhaust)
    {
        await originalExhaust;
        var goldToLose = Math.Min(5, debt.Owner.Gold);
        if (goldToLose > 0)
        {
            await PlayerCmd.LoseGold(goldToLose, debt.Owner);
        }

        MainFile.Logger.Info($"[Spire Plus] Debt applied: lost {goldToLose} gold on exhaust.");
    }
}
