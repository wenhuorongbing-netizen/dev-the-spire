using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Phases of the reward pipeline. Handlers are invoked in ascending phase order.
/// </summary>
/// <remarks>
/// Skeleton only — diagnostics and contract enforcement, no behavior changes.
/// Future phases will coordinate reward modifications across features.
/// </remarks>
internal enum RewardPhase
{
    /// <summary>
    /// Before any reward generation begins. Handlers may log context.
    /// </summary>
    PreGeneration = 0,

    /// <summary>
    /// Card reward options are being assembled.
    /// </summary>
    CardOptions = 100,

    /// <summary>
    /// Room rewards (gold, relics, potions) are being assembled.
    /// </summary>
    RoomRewards = 200,

    /// <summary>
    /// Post-processing: transform, fission, enchantment applied.
    /// </summary>
    PostProcessing = 300,

    /// <summary>
    /// Pipeline complete. No mutations allowed.
    /// </summary>
    Finalized = 999
}

/// <summary>
/// A handler in the reward pipeline. Implementations are registered
/// with <see cref="RewardPipeline"/> and invoked in phase order.
/// </summary>
internal interface IRewardHandler
{
    /// <summary>
    /// The phase this handler runs in.
    /// </summary>
    RewardPhase Phase { get; }

    /// <summary>
    /// Lower values run first within the same phase.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Handle a reward pipeline event. Implementations must not mutate
    /// reward state in the skeleton phase — diagnostics only.
    /// </summary>
    void Handle(RewardPipelineContext context);
}

/// <summary>
/// Context passed through the reward pipeline.
/// </summary>
internal sealed record RewardPipelineContext
{
    public string Feature { get; init; } = string.Empty;
    public string EventName { get; init; } = string.Empty;
    public Dictionary<string, object?> Data { get; init; } = new();
    public string NetMode { get; init; } = "single";
}

/// <summary>
/// Reward pipeline orchestrator. Currently diagnostics-only.
/// Registers handlers, sorts by phase then priority, and invokes them
/// for diagnostic logging. No reward state mutation occurs.
/// </summary>
internal static class RewardPipeline
{
    private static readonly List<IRewardHandler> Handlers = [];
    private static readonly object RegistrationLock = new();

    /// <summary>
    /// Register a handler during feature bootstrap.
    /// </summary>
    public static void Register(IRewardHandler handler)
    {
        lock (RegistrationLock)
        {
            Handlers.Add(handler);
            Handlers.Sort((a, b) =>
            {
                var phaseCompare = a.Phase.CompareTo(b.Phase);
                return phaseCompare != 0 ? phaseCompare : a.Priority.CompareTo(b.Priority);
            });
        }
    }

    /// <summary>
    /// Run all registered handlers for diagnostics.
    /// No reward state mutation occurs.
    /// </summary>
    public static void Diagnose(RewardPipelineContext context)
    {
        List<IRewardHandler> snapshot;
        lock (RegistrationLock)
        {
            snapshot = [.. Handlers];
        }

        foreach (var handler in snapshot)
        {
            handler.Handle(context);
        }
    }

    /// <summary>
    /// Current registered handler count. For guard tests.
    /// </summary>
    public static int HandlerCount
    {
        get
        {
            lock (RegistrationLock)
            {
                return Handlers.Count;
            }
        }
    }

    /// <summary>
    /// Get phase names in registration order. For guard tests.
    /// </summary>
    internal static IReadOnlyList<RewardPhase> RegisteredPhases
    {
        get
        {
            lock (RegistrationLock)
            {
                return Handlers.Select(h => h.Phase).ToArray();
            }
        }
    }

    /// <summary>
    /// Clear all handlers. For test isolation only.
    /// </summary>
    internal static void ClearHandlers()
    {
        lock (RegistrationLock)
        {
            Handlers.Clear();
        }
    }
}
