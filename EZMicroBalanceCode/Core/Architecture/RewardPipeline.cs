namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

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
            try
            {
                handler.Handle(context);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Spire Plus] RewardPipeline diagnostics handler {handler.GetType().Name} failed for {context.Feature}:{context.EventName}: {ex.GetType().Name}: {ex.Message}");
            }
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
