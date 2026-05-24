using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static int GetA11TargetColumnCount()
    {
        return VanillaMapColumns + AscensionFeatureGate.A11ExtraMapColumns;
    }

    private static int GetA11TargetRowCount(IRunState runState, int actIndex)
    {
        return runState.Act.GetNumberOfRooms(runState.Players.Count > 1) + 1 + GetA11ExtraRouteRowsForAct(actIndex);
    }

    private static int GetA11ExtraRouteRowsForAct(int actIndex)
    {
        return actIndex switch
        {
            0 => AscensionFeatureGate.A11ActOneExtraMapRows,
            1 => AscensionFeatureGate.A11ActTwoExtraMapRows,
            2 => AscensionFeatureGate.A11ActThreeExtraMapRows,
            _ => AscensionFeatureGate.A11ActTwoExtraMapRows
        };
    }

    private static void LogA11GeometryOutcome(string boundary, IRunState runState, ActMap map, int actIndex)
    {
        var targetColumns = GetA11TargetColumnCount();
        var targetRows = GetA11TargetRowCount(runState, actIndex);
        var evidenceAvailable = TryGetA11GeometryEvidence(map, out var evidence);
        var isVisibleShape = map.GetColumnCount() == targetColumns &&
            map.GetRowCount() == targetRows &&
            evidenceAvailable &&
            evidence.HasInsertedColumnRouteChoice &&
            evidence.HasStartToBossRouteAvoidingInsertedColumn;

        var message =
            $"[Spire Plus] Ascension A11 source-boundary check: boundary={boundary}; actIndex={actIndex}; columns={map.GetColumnCount()}/{targetColumns}; rows={map.GetRowCount()}/{targetRows}; insertedColumnRoute={evidence.HasInsertedColumnRouteChoice}; originalRoutePreserved={evidence.HasStartToBossRouteAvoidingInsertedColumn}; insertedColumnRouteChoices={evidence.InsertedColumnRouteChoiceCount}.";

        if (!evidenceAvailable)
        {
            MainFile.Logger.Warn(
                $"{message} A11 map geometry diagnostic failed closed; geometry may be unsupported or overwritten before the map UI reads it.");
            return;
        }

        if (isVisibleShape)
        {
            MainFile.Logger.Info(message);
            return;
        }

        MainFile.Logger.Warn(
            $"{message} A11 map shape is not visibly applied at this boundary; geometry may be unsupported or overwritten before the map UI reads it.");
    }

    private static A11MapGeometryEvidence GetA11GeometryEvidence(ActMap map)
    {
        return GetA11GeometryEvidence(SerializableActMap.FromActMap(map));
    }

    private static bool TryGetA11GeometryEvidence(ActMap map, out A11MapGeometryEvidence evidence)
    {
        try
        {
            evidence = GetA11GeometryEvidence(map);
            return true;
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"[Spire Plus] Ascension A11 geometry diagnostic failed: {ex.Message}");
            evidence = new A11MapGeometryEvidence(A11InsertedColumn, false, false, false, 0);
            return false;
        }
    }

    private static A11MapGeometryEvidence GetA11GeometryEvidence(SerializableActMap saved)
    {
        return A11MapGeometryProof.Analyze(ToA11MapGeometryGraph(saved), A11InsertedColumn);
    }

    private static bool HasA11InsertedColumnRouteChoice(
        SerializableActMap saved,
        IReadOnlyDictionary<MapCoord, SerializableMapPoint> pointsByCoord)
    {
        _ = pointsByCoord;
        return GetA11GeometryEvidence(saved).HasInsertedColumnRouteChoice;
    }

    private static bool HasA11OriginalRoutePreserved(SerializableActMap saved)
    {
        return GetA11GeometryEvidence(saved).HasStartToBossRouteAvoidingInsertedColumn;
    }
}
