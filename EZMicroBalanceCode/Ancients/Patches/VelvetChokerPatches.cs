namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(VelvetChoker), "get_CanonicalVars")]
internal static class VelvetChokerVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(7), new EnergyVar(1) };
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), "get_DisplayAmount")]
internal static class VelvetChokerDisplayAmountPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref int __result)
    {
        __result = VelvetChokerSoftLimitTracker.HandPlayedThisTurn(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]
internal static class VelvetChokerShouldPlayPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
internal static class VelvetChokerEnergyCostPatch
{
    [HarmonyPostfix]
    private static void AddSoftLimitCost(CardEnergyCost __instance, CostModifiers modifiers, ref int __result)
    {
        if (!modifiers.HasFlag(CostModifiers.Global) || __result < 0)
        {
            return;
        }

        var card = VelvetChokerSoftLimitTracker.GetCard(__instance);
        if (card is { EnergyCost.CostsX: false } && VelvetChokerSoftLimitTracker.ShouldTax(card))
        {
            __result += VelvetChokerSoftLimitTracker.ExtraEnergyCost;
        }
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class VelvetChokerXCostCanPlayPatch
{
    [HarmonyPostfix]
    private static void RequireExtraEnergyForTaxedXCost(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        if (!card.EnergyCost.CostsX ||
            !VelvetChokerSoftLimitTracker.ShouldTax(card) ||
            __instance.Energy >= VelvetChokerSoftLimitTracker.ExtraEnergyCost)
        {
            return;
        }

        reason |= UnplayableReason.EnergyCostTooHigh;
        __result = false;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class VelvetChokerXCostSpendPatch
{
    [HarmonyPostfix]
    private static void ReduceCapturedXBySoftLimitTax(CardModel __instance, ref Task<ValueTuple<int, int>> __result)
    {
        if (__instance.EnergyCost.CostsX && VelvetChokerSoftLimitTracker.ShouldTax(__instance))
        {
            __result = ReduceCapturedXBySoftLimitTax(__instance, __result);
        }
    }

    private static async Task<ValueTuple<int, int>> ReduceCapturedXBySoftLimitTax(
        CardModel card,
        Task<ValueTuple<int, int>> originalSpend)
    {
        var result = await originalSpend;
        card.EnergyCost.CapturedXValue = Math.Max(0, result.Item1 - VelvetChokerSoftLimitTracker.ExtraEnergyCost);
        return result;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]
internal static class VelvetChokerAfterCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, CardPlay cardPlay, ref Task __result)
    {
        if (!cardPlay.IsAutoPlay &&
            cardPlay.IsFirstInSeries &&
            !cardPlay.Card.IsClone &&
            cardPlay.Card.Owner == __instance.Owner)
        {
            VelvetChokerSoftLimitTracker.Increment(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]
internal static class VelvetChokerTurnResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, CombatSide side, ref Task __result)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            VelvetChokerSoftLimitTracker.Reset(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]
internal static class VelvetChokerRoomResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, AbstractRoom room, ref Task __result)
    {
        if (room is CombatRoom)
        {
            VelvetChokerSoftLimitTracker.Reset(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]
internal static class VelvetChokerCombatResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref Task __result)
    {
        VelvetChokerSoftLimitTracker.Reset(__instance);
        __result = Task.CompletedTask;
        return false;
    }
}
