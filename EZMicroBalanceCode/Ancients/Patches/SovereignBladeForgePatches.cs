namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]
internal static class SovereignBladeForgeExhaustPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task<IEnumerable<SovereignBlade>> __result)
    {
        __result = AddExhaustToForgedBlades(player, __result);
    }

    private static async Task<IEnumerable<SovereignBlade>> AddExhaustToForgedBlades(
        Player player,
        Task<IEnumerable<SovereignBlade>> original)
    {
        var blades = (await original).ToList();
        var modifiedCount = 0;
        foreach (var blade in blades.Where(blade =>
                     blade.Owner == player &&
                     blade.CreatedThroughForge &&
                     !blade.Keywords.Contains(CardKeyword.Exhaust)))
        {
            CardCmd.ApplyKeyword(blade, CardKeyword.Exhaust);
            modifiedCount++;
        }

        if (modifiedCount > 0)
        {
            MainFile.Logger.Info($"[Spire Plus] SovereignBlade applied: added Exhaust to {modifiedCount} forged temporary blade(s).");
        }

        return blades;
    }
}
