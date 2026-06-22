namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

using STS2RitsuLib.Patching.Models;

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
    static string IPatchMethod.PatchId => "prismatic-gem-hover-tips";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show Prismatic Gem reward-cycle hover text without changing reward state";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)];

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
    static string IPatchMethod.PatchId => "prismatic-gem-hover-tips-excluding-relic";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show only Prismatic Gem reward-cycle extra hover text in option surfaces";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTipsExcludingRelic), MethodType.Getter)];

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
