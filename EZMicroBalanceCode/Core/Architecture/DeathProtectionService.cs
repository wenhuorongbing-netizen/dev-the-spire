namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Represents a death protection request. Diagnostics-only stub — not wired into game logic.
/// </summary>
internal sealed record DeathProtectionRequest(
    string Player,
    string Source,
    int Damage,
    bool IsUnavoidable);

/// <summary>
/// Result of a death protection check. Diagnostics-only stub — not wired into game logic.
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
/// Diagnostics-only stub — not wired into game logic.
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
/// Diagnostics-only stub — not wired into game logic.
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

/// <summary>
/// Death protection service orchestrator. Currently diagnostics-only.
/// Registers providers and logs diagnostic information about death protection checks.
/// No actual death prevention occurs.
/// </summary>
internal static class DeathProtectionService
{
    private static readonly List<IDeathProtectionProvider> Providers = [];
    private static readonly object RegistrationLock = new();

    /// <summary>
    /// Register a provider during feature bootstrap.
    /// </summary>
    public static void Register(IDeathProtectionProvider provider)
    {
        lock (RegistrationLock)
        {
            Providers.Add(provider);
            Providers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }

    /// <summary>
    /// Diagnostics-only check. Logs which provider would protect (if any)
    /// but does NOT actually prevent death. Not wired into game logic.
    /// </summary>
    public static DeathProtectionResult CheckProtection(DeathProtectionRequest request) =>
        CheckProtectionDetailed(request).Result;

    /// <summary>
    /// Diagnostics-only check with provider attribution for tests and evidence.
    /// No actual death prevention occurs.
    /// </summary>
    internal static DeathProtectionCheck CheckProtectionDetailed(DeathProtectionRequest request)
    {
        List<IDeathProtectionProvider> snapshot;
        lock (RegistrationLock)
        {
            snapshot = [.. Providers];
        }

        if (request.IsUnavoidable)
        {
            return new DeathProtectionCheck(DeathProtectionResult.ForcedDeath, Provider: null);
        }

        foreach (var provider in snapshot)
        {
            if (provider.CanProtect(request))
            {
                return new DeathProtectionCheck(DeathProtectionResult.Protected, provider);
            }
        }

        return new DeathProtectionCheck(DeathProtectionResult.NotProtected, Provider: null);
    }

    /// <summary>
    /// Current registered provider count. For guard tests.
    /// </summary>
    public static int ProviderCount
    {
        get
        {
            lock (RegistrationLock)
            {
                return Providers.Count;
            }
        }
    }

    /// <summary>
    /// Get priority values in registration order. For guard tests.
    /// </summary>
    internal static IReadOnlyList<DeathProtectionPriority> RegisteredPriorities
    {
        get
        {
            lock (RegistrationLock)
            {
                return Providers.Select(p => p.Priority).ToArray();
            }
        }
    }

    /// <summary>
    /// Clear all providers. For test isolation only.
    /// </summary>
    internal static void ClearProviders()
    {
        lock (RegistrationLock)
        {
            Providers.Clear();
        }
    }
}
