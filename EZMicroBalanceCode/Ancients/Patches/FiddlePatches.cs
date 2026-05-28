using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class FiddleVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "fiddle-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Fiddle canonical vars to use 7-card hand limit";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Fiddle), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(AncientCardHelpers.FiddleHandLimit) };
        return false;
    }
}

internal sealed class FiddleHandDrawPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "fiddle-hand-draw";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Limit Fiddle hand draw to 7-card cap";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))];

    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, decimal count, ref decimal __result)
    {
        if (__instance.IsMelted)
        {
            __result = count;
            return false;
        }

        if (player != __instance.Owner)
        {
            return true;
        }

        var handCount = PileType.Hand.GetPile(player).Cards.Count;
        __result = Math.Max(0, AncientCardHelpers.FiddleHandLimit - handCount);
        return false;
    }
}

internal sealed class FiddleShouldDrawPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "fiddle-should-draw";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Allow Fiddle draw when melted or for owner";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ShouldDraw))];

    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, ref bool __result)
    {
        if (__instance.IsMelted)
        {
            __result = true;
            return false;
        }

        if (player != __instance.Owner)
        {
            return true;
        }

        __result = true;
        return false;
    }
}

internal sealed class FiddleDrawCapPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "fiddle-draw-cap";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Cap Fiddle draw count to prevent overflow above 7-card hand limit";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardPileCmd), nameof(CardPileCmd.Draw),
            [typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool)])];

    [HarmonyPrefix]
    private static bool Prefix(ref decimal count, Player player, bool fromHandDraw, ref Task<IEnumerable<CardModel>> __result)
    {
        if (fromHandDraw || player.GetRelic<Fiddle>() is not { IsMelted: false })
        {
            return true;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.CurrentSide != player.Creature.Side)
        {
            return true;
        }

        var remainingRoom = AncientCardHelpers.FiddleHandLimit - PileType.Hand.GetPile(player).Cards.Count;
        if (remainingRoom <= 0)
        {
            __result = Task.FromResult<IEnumerable<CardModel>>(Array.Empty<CardModel>());
            MainFile.Logger.Info("[Spire Plus] Fiddle applied: prevented draw above 7-card player-turn hand cap.");
            return false;
        }

        count = Math.Min(count, remainingRoom);
        return true;
    }
}
