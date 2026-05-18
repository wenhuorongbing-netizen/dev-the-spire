using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static DeepBranchPlan? CreateDeepBranchPlan(ActMap map, int actIndex)
    {
        if (!IsDeepBranchAct(actIndex) ||
            map.GetColumnCount() <= 0)
        {
            return null;
        }

        for (var branchLength = DeepBranchMaxLength; branchLength >= DeepBranchMinLength; branchLength--)
        {
            var firstParentRow = Math.Max(2, map.GetRowCount() / 3);
            var lastParentRow = map.GetRowCount() - branchLength - 2;
            for (var parentRow = firstParentRow; parentRow <= lastParentRow; parentRow++)
            {
                foreach (var branchColumn in EnumerateDeepBranchColumns(map))
                {
                    if (!TryGetDeepBranchCoords(map, parentRow, branchLength, branchColumn, out var branchCoords))
                    {
                        continue;
                    }

                    foreach (var parent in map.GetPointsInRow(parentRow)
                        .Where(point => Math.Abs(point.coord.col - branchColumn) <= 1)
                        .OrderBy(point => Math.Abs(point.coord.col - branchColumn))
                        .ThenBy(point => point.coord.col))
                    {
                        var reconnect = GetReachablePointsAtRow(parent, parentRow + branchLength + 1)
                            .Where(point => Math.Abs(point.coord.col - branchColumn) <= 1)
                            .OrderBy(point => Math.Abs(point.coord.col - branchColumn))
                            .ThenBy(point => point.coord.col)
                            .FirstOrDefault();
                        if (reconnect == null)
                        {
                            continue;
                        }

                        return new DeepBranchPlan(
                            parent.coord,
                            reconnect.coord,
                            branchCoords);
                    }
                }
            }
        }

        return null;
    }

    private sealed record DeepBranchPlan(
        MapCoord ParentCoord,
        MapCoord ReconnectCoord,
        List<MapCoord> BranchCoords);
}
