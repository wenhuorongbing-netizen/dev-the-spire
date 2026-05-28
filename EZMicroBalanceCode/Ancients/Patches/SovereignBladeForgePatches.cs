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

internal sealed class SovereignBladeForgeExhaustPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-o-v-e-r-e-i-g-n-b-l-a-d-e-f-o-r-g-e-e-x-h-a-u-s-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch ForgeCmd.Forge";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(ForgeCmd), nameof(ForgeCmd.Forge))];
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

internal sealed class SovereignBladeJadeBoonsOnPlayPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-o-v-e-r-e-i-g-n-b-l-a-d-e-j-a-d-e-b-o-o-n-s-o-n-p-l-a-y-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch SovereignBlade.OnPlay";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(SovereignBlade), nameof(SovereignBlade.OnPlay))];
{
    [HarmonyPostfix]
    private static void Postfix(SovereignBlade __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = SovereignBladeJadeBoons.ApplyAfterOriginal(__instance, choiceContext, __result);
    }
}

internal sealed class SovereignBladeJadeBoonsHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-o-v-e-r-e-i-g-n-b-l-a-d-e-j-a-d-e-b-o-o-n-s-h-o-v-e-r-t-i-p-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch CardModel.get_HoverTips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), "get_HoverTips", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is SovereignBlade)
        {
            __result = __result.Concat(SovereignBladeJadeBoons.CreateHoverTips()).Distinct().ToArray();
        }
    }
}


