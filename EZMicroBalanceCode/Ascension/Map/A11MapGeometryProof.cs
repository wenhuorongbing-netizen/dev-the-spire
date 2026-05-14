namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal readonly record struct A11MapCoord(int Col, int Row);

internal sealed class A11MapGeometryGraph
{
    public A11MapGeometryGraph(
        int width,
        int height,
        A11MapCoord start,
        A11MapCoord boss,
        IEnumerable<A11MapCoord> routePoints,
        IEnumerable<KeyValuePair<A11MapCoord, IEnumerable<A11MapCoord>>> children)
    {
        Width = width;
        Height = height;
        Start = start;
        Boss = boss;
        RoutePoints = routePoints.Distinct().ToArray();

        var childMap = children.ToDictionary(
            entry => entry.Key,
            entry => (IReadOnlyCollection<A11MapCoord>)entry.Value.Distinct().ToArray());

        foreach (var point in RoutePoints.Append(Start).Append(Boss))
        {
            childMap.TryAdd(point, Array.Empty<A11MapCoord>());
        }

        Children = childMap;
    }

    public int Width { get; }

    public int Height { get; }

    public A11MapCoord Start { get; }

    public A11MapCoord Boss { get; }

    public IReadOnlyCollection<A11MapCoord> RoutePoints { get; }

    public IReadOnlyDictionary<A11MapCoord, IReadOnlyCollection<A11MapCoord>> Children { get; }
}

internal readonly record struct A11MapGeometryEvidence(
    int InsertedColumn,
    bool HasBossRoute,
    bool HasInsertedColumnRouteChoice,
    bool HasStartToBossRouteAvoidingInsertedColumn,
    int InsertedColumnRouteChoiceCount);

internal static class A11MapGeometryProof
{
    public static A11MapGeometryEvidence Analyze(A11MapGeometryGraph graph, int insertedColumn)
    {
        var insertedColumnPoints = graph.RoutePoints
            .Where(point => point.Col == insertedColumn)
            .ToArray();

        var insertedColumnRouteChoiceCount = insertedColumnPoints.Count(point =>
            HasPath(graph.Start, point, graph.Children) &&
            HasPath(point, graph.Boss, graph.Children) &&
            HasPathAvoiding(graph.Start, graph.Boss, graph.Children, new HashSet<A11MapCoord> { point }));

        return new A11MapGeometryEvidence(
            insertedColumn,
            HasPath(graph.Start, graph.Boss, graph.Children),
            insertedColumnRouteChoiceCount > 0,
            HasPathAvoiding(graph.Start, graph.Boss, graph.Children, insertedColumnPoints.ToHashSet()),
            insertedColumnRouteChoiceCount);
    }

    private static bool HasPath(
        A11MapCoord start,
        A11MapCoord target,
        IReadOnlyDictionary<A11MapCoord, IReadOnlyCollection<A11MapCoord>> childrenByCoord)
    {
        return HasPathAvoiding(start, target, childrenByCoord, new HashSet<A11MapCoord>());
    }

    private static bool HasPathAvoiding(
        A11MapCoord start,
        A11MapCoord target,
        IReadOnlyDictionary<A11MapCoord, IReadOnlyCollection<A11MapCoord>> childrenByCoord,
        IReadOnlySet<A11MapCoord> excludedCoords)
    {
        if (excludedCoords.Contains(start) ||
            excludedCoords.Contains(target))
        {
            return false;
        }

        var visited = new HashSet<A11MapCoord>();
        var queue = new Queue<A11MapCoord>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var coord = queue.Dequeue();
            if (!visited.Add(coord))
            {
                continue;
            }

            if (coord == target)
            {
                return true;
            }

            if (!childrenByCoord.TryGetValue(coord, out var children))
            {
                continue;
            }

            foreach (var child in children.Where(child => !excludedCoords.Contains(child)))
            {
                queue.Enqueue(child);
            }
        }

        return false;
    }
}
