using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static DeepBranchPlan? FindExistingDeepBranch(ActMap map, int actIndex)
    {
        if (!IsDeepBranchAct(actIndex) ||
            map.GetColumnCount() <= 0)
        {
            return null;
        }

        foreach (var parent in map.GetAllMapPoints()
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col))
        {
            foreach (var firstBranchPoint in parent.Children
                .Where(child => child.coord.row == parent.coord.row + 1)
                .OrderBy(child => Math.Abs(child.coord.col - A11InsertedColumn))
                .ThenBy(child => child.coord.col))
            {
                for (var branchLength = DeepBranchMaxLength; branchLength >= DeepBranchMinLength; branchLength--)
                {
                    if (TryMatchExistingDeepBranch(parent, firstBranchPoint, branchLength, out var plan))
                    {
                        return plan;
                    }
                }
            }
        }

        return null;
    }

    private static bool TryMatchExistingDeepBranch(
        MapPoint parent,
        MapPoint firstBranchPoint,
        int branchLength,
        out DeepBranchPlan? plan)
    {
        plan = null;
        var branchColumn = firstBranchPoint.coord.col;
        var existingBranchPoints = new List<MapPoint>(branchLength);
        var current = firstBranchPoint;
        for (var index = 0; index < branchLength; index++)
        {
            if (current.coord.col != branchColumn ||
                current.coord.row != parent.coord.row + index + 1 ||
                current.PointType != GetDeepBranchPointType(index, branchLength) ||
                current.CanBeModified)
            {
                return false;
            }

            existingBranchPoints.Add(current);
            if (index == branchLength - 1)
            {
                continue;
            }

            var nextRow = parent.coord.row + index + 2;
            var next = current.Children
                .FirstOrDefault(child => child.coord.col == branchColumn && child.coord.row == nextRow);
            if (next == null)
            {
                return false;
            }

            current = next;
        }

        var reconnect = current.Children
            .FirstOrDefault(point => point.coord.row == parent.coord.row + branchLength + 1);
        if (reconnect == null ||
            !HasPathAvoiding(parent, reconnect, existingBranchPoints))
        {
            return false;
        }

        plan = new DeepBranchPlan(
            parent.coord,
            reconnect.coord,
            existingBranchPoints.Select(point => point.coord).ToList());
        return true;
    }
}
