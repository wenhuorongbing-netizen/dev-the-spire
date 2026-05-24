using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    public static ActMap ApplyA11MapGeometryAtCreateMapBoundary(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsMapGeometryEnabled(runState))
        {
            return map;
        }

        var adjustedMap = TryApplyA11MapShape(runState, map, actIndex);
        if (adjustedMap != map)
        {
            var evidence = GetA11GeometryEvidence(adjustedMap);
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension A11 source-boundary applied: ActModel.CreateMap returned adjusted map; actIndex={actIndex}; columns={adjustedMap.GetColumnCount()}/{GetA11TargetColumnCount()}; rows={adjustedMap.GetRowCount()}/{GetA11TargetRowCount(runState, actIndex)}; insertedColumnRoute={evidence.HasInsertedColumnRouteChoice}; originalRoutePreserved={evidence.HasStartToBossRouteAvoidingInsertedColumn}; insertedColumnRouteChoices={evidence.InsertedColumnRouteChoiceCount}.");
            return adjustedMap;
        }

        LogA11GeometryOutcome("ActModel.CreateMap", runState, map, actIndex);
        return map;
    }

    private static ActMap ApplyMapGeometry(IRunState runState, ActMap map, int actIndex)
    {
        if (AscensionFeatureGate.IsMapGeometryEnabled(runState))
        {
            var adjustedMap = TryApplyA11MapShape(runState, map, actIndex);
            if (adjustedMap != map)
            {
                var evidence = GetA11GeometryEvidence(adjustedMap);
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension A11 applied: expanded map width by {AscensionFeatureGate.A11ExtraMapColumns} column with a reachable optional route and inserted {GetA11ExtraRouteRowsForAct(actIndex)} late route row(s); actIndex={actIndex}; columns={adjustedMap.GetColumnCount()}/{GetA11TargetColumnCount()}; rows={adjustedMap.GetRowCount()}/{GetA11TargetRowCount(runState, actIndex)}; insertedColumnRoute={evidence.HasInsertedColumnRouteChoice}; originalRoutePreserved={evidence.HasStartToBossRouteAvoidingInsertedColumn}; insertedColumnRouteChoices={evidence.InsertedColumnRouteChoiceCount}.");
                map = adjustedMap;
            }
            else
            {
                LogA11GeometryOutcome("run map hook", runState, map, actIndex);
            }
        }

        if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))
        {
            var branchedMap = TryInsertDeepBranch(runState, map, actIndex);
            if (branchedMap != map)
            {
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension A17 applied: inserted one optional {DeepBranchMinLength}-{DeepBranchMaxLength} node Deep Branch with safe-route reconnect; actIndex={actIndex}; columns={branchedMap.GetColumnCount()}; rows={branchedMap.GetRowCount()}.");
                map = branchedMap;
            }
            else
            {
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension A17 gate active: Deep Branch already present or unsupported for safe insertion; actIndex={actIndex}; columns={map.GetColumnCount()}; rows={map.GetRowCount()}.");
            }
        }

        return map;
    }

    private static ActMap TryApplyA11MapShape(IRunState runState, ActMap map, int actIndex)
    {
        if (map is not StandardActMap and not SavedActMap)
        {
            return map;
        }

        var vanillaRows = runState.Act.GetNumberOfRooms(runState.Players.Count > 1) + 1;
        var targetRows = GetA11TargetRowCount(runState, actIndex);
        var targetColumns = GetA11TargetColumnCount();
        SerializableActMap? saved = null;
        if (map.GetColumnCount() == targetColumns &&
            map.GetRowCount() == targetRows)
        {
            saved = SerializableActMap.FromActMap(map);
            if (HasA11InsertedColumnRouteChoice(saved, BuildSerializableLookup(saved)) &&
                HasA11OriginalRoutePreserved(saved))
            {
                return map;
            }
        }

        if ((map.GetColumnCount() != VanillaMapColumns && map.GetColumnCount() != targetColumns) ||
            map.GetRowCount() < vanillaRows ||
            map.GetRowCount() > targetRows ||
            map.GetRowCount() < 4)
        {
            return map;
        }

        saved ??= SerializableActMap.FromActMap(map);
        if (saved.GridWidth == VanillaMapColumns)
        {
            ExpandA11MapWidth(saved);
        }

        var missingRows = targetRows - saved.GridHeight;
        if (missingRows > 0 && !TryInsertRouteRowsBeforeBossRest(saved, missingRows))
        {
            return map;
        }

        if (!TryInsertA11WidthChoice(saved))
        {
            return map;
        }

        if (!HasA11InsertedColumnRouteChoice(saved, BuildSerializableLookup(saved)) ||
            !HasA11OriginalRoutePreserved(saved))
        {
            return map;
        }

        return new SavedActMap(saved);
    }
}
