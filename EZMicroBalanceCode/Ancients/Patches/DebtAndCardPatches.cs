namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterCreated))]
internal static class DebtAfterCreatedPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance)
    {
        if (__instance is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.FromSerializable))]
internal static class DebtFromSavePatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __result)
    {
        if (__result is Debt debt)
        {
            DebtCardPatch.ConfigureDebt(debt);
        }
    }
}

[HarmonyPatch(typeof(Debt), "get_CanonicalKeywords")]
internal static class DebtKeywordsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new CardKeyword[] { CardKeyword.Exhaust };
        return false;
    }
}

[HarmonyPatch(typeof(Debt), "get_CanonicalVars")]
internal static class DebtVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new GoldVar(5) };
        return false;
    }
}

[HarmonyPatch(typeof(Debt), "get_HasTurnEndInHandEffect")]
internal static class DebtTurnEndEffectPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(Debt), nameof(Debt.OnTurnEndInHand))]
internal static class DebtTurnEndInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "OnPlay")]
internal static class CardModelOnPlayPatch
{
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
        MainFile.Logger.Info("[EZMicroBalance] Debt applied: will exhaust after play.");
        return Task.CompletedTask;
    }

    private static async Task PlayEnthralled(Enthralled enthralled, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m, ValueProp.Move, cardPlay);
        MainFile.Logger.Info("[EZMicroBalance] Enthralled applied: gained 10 block.");
    }
}

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]
internal static class DebtExhaustPatch
{
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

        MainFile.Logger.Info($"[EZMicroBalance] Debt applied: lost {goldToLose} gold on exhaust.");
    }
}
