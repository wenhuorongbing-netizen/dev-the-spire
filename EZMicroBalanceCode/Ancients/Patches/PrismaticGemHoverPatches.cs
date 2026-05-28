using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class PrismaticGemHoverTipFactory
{
    public static MegaCrit.Sts2.Core.HoverTips.IHoverTip CreateCountHoverTip(PrismaticGem prismaticGem)
    {
        var count = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] % 2;
        var title = new LocString("relics", "PRISMATIC_GEM.countHint.title");
        title.Add("Count", (decimal)count);
        title.Add("Cycle", 2m);

        var descriptionKey = count == 0
            ? "PRISMATIC_GEM.countHint.nextNormal"
            : "PRISMATIC_GEM.countHint.nextOffColor";
        return new MegaCrit.Sts2.Core.HoverTips.HoverTip(title, new LocString("relics", descriptionKey));
    }
}

internal sealed class PrismaticGemHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-r-i-s-m-a-t-i-c-g-e-m-h-o-v-e-r-t-i-p-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_HoverTips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_HoverTips", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = new[]
        {
            __instance.HoverTip,
            PrismaticGemHoverTipFactory.CreateCountHoverTip(prismaticGem)
        };
        return false;
    }
}

internal sealed class PrismaticGemHoverTipsExcludingRelicPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-r-i-s-m-a-t-i-c-g-e-m-h-o-v-e-r-t-i-p-s-e-x-c-l-u-d-i-n-g-r-e-l-i-c-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_HoverTipsExcludingRelic";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_HoverTipsExcludingRelic", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = new[] { PrismaticGemHoverTipFactory.CreateCountHoverTip(prismaticGem) };
        return false;
    }
}


