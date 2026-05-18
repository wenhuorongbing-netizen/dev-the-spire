using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static void MarkFiremarkedElite(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))
        {
            return;
        }

        var candidates = map.GetAllMapPoints()
            .Where(point => point.PointType == MapPointType.Elite)
            .Where(point => point.CanBeModified)
            .Where(point => IsAfterActOneFirstRestSite(map, point, actIndex))
            .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .ToList();

        var desiredCount = GetFiremarkedEliteTargetCount(actIndex);
        var markedCount = 0;
        foreach (var point in PickFiremarkedElitesByAct(
                     candidates,
                     desiredCount,
                     runState,
                     FiremarkMarkerFamily,
                     actIndex,
                     map.StartingMapPoint,
                     map.BossMapPoint))
        {
            var kind = GetKindFromActOrder<FiremarkKind>(
                runState,
                FiremarkMarkerFamily,
                actIndex,
                point.coord,
                markedCount);
            GetOrCreateMetadata(point).Firemark = kind;
            EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point);
            markedCount++;

            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 applied: marked {point} as firemarked elite ({kind}).");
            LogMapAssignment(actIndex, point.coord, FiremarkMarkerFamily, kind);
        }

        if (markedCount == 0)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 gate active: no optional elite node was safe to firemark on actIndex={actIndex}.");
            return;
        }

        if (markedCount < desiredCount)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 gate active: marked {markedCount}/{desiredCount} firemarked elites on actIndex={actIndex}; minimum fallback target is {MinimumFiremarkedEliteFallbackCount} when safe candidates exist.");
        }
    }

    private static int GetFiremarkedEliteTargetCount(int actIndex)
    {
        return actIndex == 0
            ? ActOneFiremarkedEliteTargetCount
            : LaterActFiremarkedEliteTargetCount;
    }
}
