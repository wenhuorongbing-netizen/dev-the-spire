using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class VelvetChokerVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-v-a-r-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.get_CanonicalVars";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(7), new EnergyVar(1) };
        return false;
    }
}

internal sealed class VelvetChokerDisplayAmountPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-d-i-s-p-l-a-y-a-m-o-u-n-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.get_DisplayAmount";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), "get_DisplayAmount", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref int __result)
    {
        __result = VelvetChokerSoftLimitTracker.HandPlayedThisTurn(__instance);
        return false;
    }
}

internal sealed class VelvetChokerShouldPlayPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-s-h-o-u-l-d-p-l-a-y-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.ShouldPlay";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))];
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

internal sealed class VelvetChokerEnergyCostPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-e-n-e-r-g-y-c-o-s-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch CardEnergyCost.GetWithModifiers";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))];
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

internal sealed class VelvetChokerXCostCanPlayPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-x-c-o-s-t-c-a-n-p-l-a-y-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch PlayerCombatState.HasEnoughResourcesFor";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))];
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

internal sealed class VelvetChokerXCostSpendPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-x-c-o-s-t-s-p-e-n-d-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch CardModel.SpendResources";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.SpendResources))];
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

internal sealed class VelvetChokerAfterCardPlayedPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-a-f-t-e-r-c-a-r-d-p-l-a-y-e-d-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.AfterCardPlayed";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))];
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

internal sealed class VelvetChokerTurnResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-t-u-r-n-r-e-s-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.BeforeSideTurnStart";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))];
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

internal sealed class VelvetChokerRoomResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-r-o-o-m-r-e-s-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.AfterRoomEntered";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))];
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

internal sealed class VelvetChokerCombatResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "v-e-l-v-e-t-c-h-o-k-e-r-c-o-m-b-a-t-r-e-s-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch VelvetChoker.AfterCombatEnd";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))];
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref Task __result)
    {
        VelvetChokerSoftLimitTracker.Reset(__instance);
        __result = Task.CompletedTask;
        return false;
    }
}


