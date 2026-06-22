using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class VelvetChokerVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "velvet-choker-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Velvet Choker canonical vars for Spire Plus soft-limit text";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), "CanonicalVars", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(7), new EnergyVar(1) };
        return false;
    }
}

internal sealed class VelvetChokerDisplayAmountPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "velvet-choker-display-amount";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show Velvet Choker's current cards-played soft-limit counter";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), "DisplayAmount", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref int __result)
    {
        __result = VelvetChokerSoftLimitTracker.HandPlayedThisTurn(__instance);
        return false;
    }
}

internal sealed class VelvetChokerShouldPlayPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "velvet-choker-should-play";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Keep Velvet Choker playable while soft-limit cost handles restriction";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))];

    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

internal sealed class VelvetChokerEnergyCostPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "velvet-choker-energy-cost";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add Velvet Choker's extra energy tax after the soft card-play limit";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))];

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
    static string IPatchMethod.PatchId => "velvet-choker-x-cost-can-play";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Require extra Velvet Choker energy before taxed X-cost cards can play";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))];

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
    static string IPatchMethod.PatchId => "velvet-choker-x-cost-spend";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Subtract Velvet Choker's soft-limit tax from captured X value after spending";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.SpendResources))];

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
    static string IPatchMethod.PatchId => "velvet-choker-after-card-played";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Track owner-played cards for Velvet Choker's soft limit";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))];

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
    static string IPatchMethod.PatchId => "velvet-choker-turn-reset";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Velvet Choker's soft-limit counter at owner turn start";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))];

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
    static string IPatchMethod.PatchId => "velvet-choker-room-reset";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Velvet Choker's soft-limit counter on combat room entry";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))];

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
    static string IPatchMethod.PatchId => "velvet-choker-combat-reset";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Velvet Choker's soft-limit counter after combat";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))];

    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref Task __result)
    {
        VelvetChokerSoftLimitTracker.Reset(__instance);
        __result = Task.CompletedTask;
        return false;
    }
}
