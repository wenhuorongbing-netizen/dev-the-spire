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

[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]
internal static class PrismaticGemHoverTipsPatch
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

[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]
internal static class PrismaticGemHoverTipsExcludingRelicPatch
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
