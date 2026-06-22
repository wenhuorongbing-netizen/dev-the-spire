using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SovereignBladeJadeBoons
{
    public const decimal Amount = 3m;

    public static IEnumerable<IHoverTip> CreateHoverTips()
    {
        return
        [
            HoverTipFactory.FromPower<StrengthPower>((int)Amount),
            HoverTipFactory.FromPower<DexterityPower>((int)Amount),
            HoverTipFactory.FromPower<PlatingPower>((int)Amount),
            HoverTipFactory.FromPower<RegenPower>((int)Amount),
            HoverTipFactory.FromPower<VigorPower>((int)Amount)
        ];
    }

    public static async Task ApplyAfterOriginal(
        SovereignBlade blade,
        PlayerChoiceContext choiceContext,
        Task original)
    {
        await original;

        var owner = blade.Owner?.Creature;
        if (owner?.CombatState == null || owner.IsDead)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, owner, Amount, owner, blade);
        await PowerCmd.Apply<DexterityPower>(choiceContext, owner, Amount, owner, blade);
        await PowerCmd.Apply<PlatingPower>(choiceContext, owner, Amount, owner, blade);
        await PowerCmd.Apply<RegenPower>(choiceContext, owner, Amount, owner, blade);
        await PowerCmd.Apply<VigorPower>(choiceContext, owner, Amount, owner, blade);
    }
}

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

[HarmonyPatch(typeof(SovereignBlade), "OnPlay")]
internal static class SovereignBladeJadeBoonsOnPlayPatch
{
    [HarmonyPostfix]
    private static void Postfix(SovereignBlade __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = SovereignBladeJadeBoons.ApplyAfterOriginal(__instance, choiceContext, __result);
    }
}

internal sealed class SovereignBladeJadeBoonsHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sovereign-blade-jade-boons-hover-tips";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Expose Sovereign Blade's five jade boon power previews in card hover text";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is SovereignBlade)
        {
            __result = __result.Concat(SovereignBladeJadeBoons.CreateHoverTips()).Distinct().ToArray();
        }
    }
}
