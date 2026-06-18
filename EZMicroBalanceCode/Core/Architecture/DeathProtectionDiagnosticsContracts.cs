namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Represents a death protection request. Diagnostics-only stub, not wired into game logic.
/// </summary>
internal sealed record DeathProtectionRequest(
    string Player,
    string Source,
    int Damage,
    bool IsUnavoidable);

/// <summary>
/// Result of a death protection check. Diagnostics-only stub, not wired into game logic.
/// </summary>
internal enum DeathProtectionResult
{
    /// <summary>
    /// Death was prevented by a provider.
    /// </summary>
    Protected = 0,

    /// <summary>
    /// No provider prevented death. The creature dies normally.
    /// </summary>
    NotProtected = 1,

    /// <summary>
    /// Death is forced and cannot be prevented (e.g., reprieve turn ending with enemies alive).
    /// </summary>
    ForcedDeath = 2
}

/// <summary>
/// Diagnostics-only death protection check result with the provider that would have handled it.
/// Not wired into game logic.
/// </summary>
internal sealed record DeathProtectionCheck(
    DeathProtectionResult Result,
    IDeathProtectionProvider? Provider);

/// <summary>
/// Priority ordering for death protection providers. Lower values are checked first.
/// Diagnostics-only stub, not wired into game logic.
/// </summary>
internal enum DeathProtectionPriority
{
    /// <summary>
    /// Standard reprieve (e.g., Lotha Death Reprieve).
    /// </summary>
    Reprieve = 100,

    /// <summary>
    /// Sacrifice-based protection (e.g., relic or power that trades HP/resource for survival).
    /// </summary>
    Sacrifice = 200,

    /// <summary>
    /// Last-stand protection (e.g., final-HP-gated survival effects).
    /// </summary>
    LastStand = 300
}

/// <summary>
/// Provider interface for death protection. Implementations are registered with
/// <see cref="DeathProtectionService"/> and queried in priority order.
/// Diagnostics-only stub, not wired into game logic.
/// </summary>
internal interface IDeathProtectionProvider
{
    /// <summary>
    /// Whether this provider can protect against the given death event.
    /// Checked in priority order; first match wins.
    /// </summary>
    bool CanProtect(DeathProtectionRequest request);

    /// <summary>
    /// Priority for ordering providers. Lower values are checked first.
    /// </summary>
    DeathProtectionPriority Priority { get; }
}
