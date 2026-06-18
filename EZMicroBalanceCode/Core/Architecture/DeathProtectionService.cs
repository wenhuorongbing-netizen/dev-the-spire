namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

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
