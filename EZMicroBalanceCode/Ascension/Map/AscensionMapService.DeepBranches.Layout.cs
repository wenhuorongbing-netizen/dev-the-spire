using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static bool TryGetDeepBranchCoords(
        ActMap map,
        int parentRow,
        int branchLength,
        int branchColumn,
        out List<MapCoord> branchCoords)
    {
        branchCoords = BuildDeepBranchCoords(parentRow, branchLength, branchColumn);
        return branchCoords.All(coord =>
            coord.col >= 0 &&
            coord.col < map.GetColumnCount() &&
            !map.HasPoint(coord));
    }

    private static List<MapCoord> BuildDeepBranchCoords(int parentRow, int branchLength, int branchColumn)
    {
        var coords = new List<MapCoord>(branchLength);
        for (var i = 0; i < branchLength; i++)
        {
            coords.Add(new MapCoord
            {
                col = branchColumn,
                row = parentRow + i + 1
            });
        }

        return coords;
    }

    private static IEnumerable<int> EnumerateDeepBranchColumns(ActMap map)
    {
        for (var offset = 0; offset < map.GetColumnCount(); offset++)
        {
            var right = A11InsertedColumn + offset;
            if (right >= 0 && right < map.GetColumnCount())
            {
                yield return right;
            }

            var left = A11InsertedColumn - offset;
            if (offset > 0 && left >= 0 && left < map.GetColumnCount())
            {
                yield return left;
            }
        }
    }

    private static bool IsDeepBranchAct(int actIndex)
    {
        return actIndex is 1 or 2;
    }

    private static MapPointType GetDeepBranchPointType(int index, int branchLength)
    {
        if (index == 0)
        {
            return MapPointType.Monster;
        }

        if (index == 1)
        {
            return MapPointType.Elite;
        }

        return IsDeepBranchRewardIndex(index, branchLength)
            ? MapPointType.Treasure
            : MapPointType.Shop;
    }

    private static bool IsDeepBranchRewardIndex(int index, int branchLength)
    {
        return index == branchLength - 1;
    }
}
