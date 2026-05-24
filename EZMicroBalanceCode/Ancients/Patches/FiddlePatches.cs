namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(Fiddle), "get_CanonicalVars")]
internal static class FiddleVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(AncientCardHelpers.FiddleHandLimit) };
        return false;
    }
}

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))]
internal static class FiddleHandDrawPatch
{
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

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]
internal static class FiddleShouldDrawPatch
{
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

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]
internal static class FiddleDrawCapPatch
{
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
