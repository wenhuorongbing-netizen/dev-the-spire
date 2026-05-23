using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class BannerRoomMapHoverPatch
{
    internal static bool TryCreateHoverTip(MapPoint point, out HoverTip hoverTip)
    {
        hoverTip = default;
        if (!point.Quests.Any(quest => quest is BannerRoomMapQuestMarker))
        {
            return false;
        }

        var metadata = AscensionMapService.TryGetMetadata(point);
        if (metadata?.Banner == null)
        {
            return false;
        }

        hoverTip = CreateHoverTip(metadata.Banner.Value);
        return true;
    }

    private static HoverTip CreateHoverTip(BannerKind banner)
    {
        var locKey = banner switch
        {
            BannerKind.Vanguard => "BANNER_VANGUARD",
            BannerKind.Shieldwall => "BANNER_SHIELDWALL",
            BannerKind.BloodPrize => "BANNER_BLOOD_PRIZE",
            BannerKind.PressingLine => "BANNER_PRESSING_LINE",
            BannerKind.LastStand => "BANNER_LAST_STAND",
            _ => "BANNER_ROOM"
        };

        var description = new LocString("ascension", $"{locKey}.description");
        AddCurrentActBannerValues(description, banner);

        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            description);
    }

    private static bool RequiresKnownEnemyCount(BannerKind banner) =>
        banner is BannerKind.Shieldwall or BannerKind.LastStand;

    private static void AddCurrentActBannerValues(LocString description, BannerKind banner)
    {
        var actIndex = Math.Clamp(RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0, 0, 2);
        switch (banner)
        {
            case BannerKind.Vanguard:
                description.Add("Strength", ActValue(actIndex, 1m, 2m, 4m));
                break;
            case BannerKind.Shieldwall:
                description.Add("Block", ActValue(actIndex, 3m, 7m, 14m));
                description.Add("DeathBlock", ActValue(actIndex, 5m, 10m, 20m));
                break;
            case BannerKind.BloodPrize:
                description.Add("Gold", ActValue(actIndex, 15m, 30m, 55m));
                description.Add("Strength", ActValue(actIndex, 1m, 2m, 4m));
                description.Add("Artifact", ActValue(actIndex, 1m, 1m, 2m));
                break;
            case BannerKind.PressingLine:
                description.Add("PartialBlock", ActValue(actIndex, 4m, 8m, 16m));
                description.Add("FullBlock", ActValue(actIndex, 6m, 12m, 24m));
                description.Add("ExtraDamage", ActValue(actIndex, 1m, 2m, 4m));
                break;
            case BannerKind.LastStand:
                description.Add("Block", ActValue(actIndex, 6m, 12m, 24m));
                description.Add("Strength", ActValue(actIndex, 1m, 2m, 4m));
                break;
        }
    }

    private static decimal ActValue(int actIndex, decimal actOne, decimal actTwo, decimal actThree)
    {
        return actIndex switch
        {
            0 => actOne,
            1 => actTwo,
            _ => actThree
        };
    }
}
