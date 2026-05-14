using EZMicroBalance.EZMicroBalanceCode.Ascension;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class A11MapGeometryProofTests
{
    private const int InsertedColumn = 4;

    [Fact]
    public void InsertedColumnRouteChoiceRequiresReachableOptionalBossRoute()
    {
        var graph = CreateGraph(
            width: 8,
            height: 6,
            routePoints:
            [
                new(3, 1),
                new(3, 2),
                new(3, 3),
                new(4, 3),
                new(3, 4),
                new(3, 5)
            ],
            edges:
            [
                Edge(new(3, 0), new(3, 1)),
                Edge(new(3, 1), new(3, 2)),
                Edge(new(3, 2), new(3, 3)),
                Edge(new(3, 2), new(4, 3)),
                Edge(new(3, 3), new(3, 4)),
                Edge(new(4, 3), new(3, 4)),
                Edge(new(3, 4), new(3, 5)),
                Edge(new(3, 5), new(3, 6))
            ]);

        var evidence = A11MapGeometryProof.Analyze(graph, InsertedColumn);

        Assert.True(evidence.HasBossRoute);
        Assert.True(evidence.HasInsertedColumnRouteChoice);
        Assert.True(evidence.HasStartToBossRouteAvoidingInsertedColumn);
        Assert.Equal(1, evidence.InsertedColumnRouteChoiceCount);
    }

    [Fact]
    public void InsertedColumnChokepointIsNotAValidRouteChoice()
    {
        var graph = CreateGraph(
            width: 8,
            height: 4,
            routePoints:
            [
                new(3, 1),
                new(4, 2),
                new(3, 3)
            ],
            edges:
            [
                Edge(new(3, 0), new(3, 1)),
                Edge(new(3, 1), new(4, 2)),
                Edge(new(4, 2), new(3, 3)),
                Edge(new(3, 3), new(3, 4))
            ]);

        var evidence = A11MapGeometryProof.Analyze(graph, InsertedColumn);

        Assert.True(evidence.HasBossRoute);
        Assert.False(evidence.HasInsertedColumnRouteChoice);
        Assert.False(evidence.HasStartToBossRouteAvoidingInsertedColumn);
        Assert.Equal(0, evidence.InsertedColumnRouteChoiceCount);
    }

    [Fact]
    public void EvidenceIsDeterministicForAlreadyTargetSizedMap()
    {
        var graph = CreateGraph(
            width: 8,
            height: 7,
            routePoints:
            [
                new(3, 1),
                new(3, 2),
                new(3, 3),
                new(4, 3),
                new(3, 4),
                new(3, 5),
                new(3, 6)
            ],
            edges:
            [
                Edge(new(3, 0), new(3, 1)),
                Edge(new(3, 1), new(3, 2)),
                Edge(new(3, 2), new(3, 3)),
                Edge(new(3, 2), new(4, 3)),
                Edge(new(3, 3), new(3, 4)),
                Edge(new(4, 3), new(3, 4)),
                Edge(new(3, 4), new(3, 5)),
                Edge(new(3, 5), new(3, 6)),
                Edge(new(3, 6), new(3, 7))
            ]);

        var first = A11MapGeometryProof.Analyze(graph, InsertedColumn);
        var second = A11MapGeometryProof.Analyze(graph, InsertedColumn);

        Assert.Equal(8, graph.Width);
        Assert.Equal(7, graph.Height);
        Assert.Equal(first, second);
        Assert.True(second.HasInsertedColumnRouteChoice);
        Assert.True(second.HasStartToBossRouteAvoidingInsertedColumn);
    }

    private static A11MapGeometryGraph CreateGraph(
        int width,
        int height,
        IEnumerable<A11MapCoord> routePoints,
        IEnumerable<(A11MapCoord Parent, A11MapCoord Child)> edges)
    {
        var children = edges
            .GroupBy(edge => edge.Parent)
            .Select(group => new KeyValuePair<A11MapCoord, IEnumerable<A11MapCoord>>(
                group.Key,
                group.Select(edge => edge.Child)));

        return new A11MapGeometryGraph(
            width,
            height,
            new A11MapCoord(3, 0),
            new A11MapCoord(3, height),
            routePoints,
            children);
    }

    private static (A11MapCoord Parent, A11MapCoord Child) Edge(A11MapCoord parent, A11MapCoord child)
    {
        return (parent, child);
    }
}
