namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Policy for extra card plays beyond the initial play count.
/// </summary>
internal enum ExtraPlayPolicy
{
    /// <summary>
    /// Allow the extra play unconditionally.
    /// </summary>
    Allow = 0,

    /// <summary>
    /// Block the extra play. The card is not replayed.
    /// </summary>
    Block = 1,

    /// <summary>
    /// Fall back to power-based effect instead of a physical card replay.
    /// Used when direct replay would cause state corruption.
    /// </summary>
    FallbackToPower = 2
}

/// <summary>
/// Tracks card play depth and enforces recursion guards.
/// Prevents infinite loops from extra-play or power-fallback chains.
/// </summary>
/// <remarks>
/// Skeleton only — establishes the depth-guard contract.
/// Current Lotha extra-play implementations (Mirror Rebuttal, Mirror Hall Echo,
/// Deferred Verdict, Single Sentence) use per-blessing flags for recursion control.
/// This context provides a unified depth guard for future consolidated use.
/// </remarks>
internal sealed class CardPlayContext
{
    /// <summary>
    /// Maximum allowed depth for card play chains. Beyond this, all extra plays are blocked.
    /// </summary>
    public const int MaxDepth = 10;

    private int _currentDepth;

    /// <summary>
    /// Current depth of the card play chain. 0 = original play.
    /// </summary>
    public int CurrentDepth => _currentDepth;

    /// <summary>
    /// Whether the current chain has exceeded the depth guard.
    /// </summary>
    public bool IsDepthExceeded => _currentDepth > MaxDepth;

    /// <summary>
    /// The policy for the current extra-play request.
    /// </summary>
    public ExtraPlayPolicy Policy { get; set; } = ExtraPlayPolicy.Allow;

    /// <summary>
    /// Whether this play originated from a power fallback rather than a direct card replay.
    /// </summary>
    public bool IsPowerFallback { get; set; }

    /// <summary>
    /// Increment depth for a nested play. Returns false if the depth guard would be exceeded,
    /// in which case the caller must block the extra play.
    /// </summary>
    public bool TryIncrementDepth()
    {
        if (_currentDepth >= MaxDepth)
        {
            return false;
        }

        _currentDepth++;
        return true;
    }

    /// <summary>
    /// Decrement depth when a nested play completes.
    /// </summary>
    public void DecrementDepth()
    {
        if (_currentDepth > 0)
        {
            _currentDepth--;
        }
    }

    /// <summary>
    /// Reset context state for reuse.
    /// </summary>
    public void Reset()
    {
        _currentDepth = 0;
        Policy = ExtraPlayPolicy.Allow;
        IsPowerFallback = false;
    }
}
