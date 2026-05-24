using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static void MarkBannerRooms(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsBannerRoomEnabled(runState))
        {
            return;
        }

        var desiredCount = actIndex == 0 ? 1 : 2;
        var preferredMinimumRow = Math.Max(GetFirstRestSiteRow(map) + 1, 1);
        var candidates = map.GetAllMapPoints()
            .Where(point => point.PointType == MapPointType.Monster)
            .Where(point => point.CanBeModified)
            .Where(point => point.coord.row >= preferredMinimumRow)
            .Where(point => point.Quests.All(quest => quest is not FiremarkedEliteMapQuestMarker))
            .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .ToList();

        if (candidates.Count < desiredCount && actIndex > 0)
        {
            candidates = map.GetAllMapPoints()
                .Where(point => point.PointType == MapPointType.Monster)
                .Where(point => point.CanBeModified)
                .Where(point => point.Quests.All(quest => quest is not FiremarkedEliteMapQuestMarker))
                .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
                .OrderBy(point => point.coord.row)
                .ThenBy(point => point.coord.col)
                .ToList();
        }

        var markedCount = 0;
        foreach (var point in PickDistinctByStableOrder(
                     candidates,
                     desiredCount,
                     runState,
                     BannerMarkerFamily,
                     actIndex))
        {
            var kind = GetKindFromActOrder<BannerKind>(
                runState,
                BannerMarkerFamily,
                actIndex,
                point.coord,
                markedCount);
            GetOrCreateMetadata(point).Banner = kind;
            EnsureQuestMarker<BannerRoomMapQuestMarker>(point);
            markedCount++;

            MainFile.Logger.Info(
                $"[Spire Plus] Ascension A16 applied: marked {point} as banner room ({kind}).");
            LogMapAssignment(actIndex, point.coord, BannerMarkerFamily, kind);
        }

        if (markedCount < desiredCount)
        {
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension A16 gate active: marked {markedCount}/{desiredCount} optional banner rooms on actIndex={actIndex}.");
        }
    }
}
