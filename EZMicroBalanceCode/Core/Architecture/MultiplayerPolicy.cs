namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Multiplayer safety categories for Spire Plus features.
/// Diagnostics-only stub — Not wired into game logic.
/// </summary>
internal enum MultiplayerFeatureCategory
{
    /// <summary>
    /// Changes only local UI presentation. No gameplay state is read or mutated.
    /// Co-op safe: always.
    /// </summary>
    LocalUiOnly = 0,

    /// <summary>
    /// Mutates only the local player's state. Does not affect other players.
    /// Co-op safe: yes for host. Requires verification for client.
    /// </summary>
    LocalPlayerOnly = 1,

    /// <summary>
    /// Mutates shared state but only from the host path.
    /// Co-op safe: yes, when host-only execution is enforced.
    /// </summary>
    HostAuthoritative = 2,

    /// <summary>
    /// Reads or mutates state shared across all players. Requires synchronization.
    /// Co-op safe: requires explicit two-client proof before enabling.
    /// </summary>
    SharedRunState = 3,

    /// <summary>
    /// Affects combat state through the game's combat command system.
    /// Co-op safe: likely yes (commands are replicated), but needs two-client proof.
    /// </summary>
    CombatCommandReplicated = 4,

    /// <summary>
    /// Cannot safely run in multiplayer under any current design.
    /// Must be disabled or redesigned.
    /// </summary>
    UnsafeInMultiplayer = 5
}

/// <summary>
/// Policy record for a classified multiplayer feature. Diagnostics-only stub.
/// </summary>
internal sealed record MultiplayerPolicyRecord(
    string FeatureId,
    MultiplayerFeatureCategory Category,
    string? EnvOverride,
    bool IsVerified);

/// <summary>
/// Registry for multiplayer feature policies. Diagnostics-only stub — not wired into game logic.
/// Provides lookup and diagnostic information about feature multiplayer safety.
/// No actual gating or enforcement occurs from this stub.
/// </summary>
internal static class MultiplayerPolicyRegistry
{
    private static readonly List<MultiplayerPolicyRecord> Policies = [];
    private static readonly object RegistrationLock = new();

    /// <summary>
    /// Register a feature policy during bootstrap.
    /// </summary>
    public static void Register(MultiplayerPolicyRecord policy)
    {
        lock (RegistrationLock)
        {
            Policies.Add(policy);
        }
    }

    /// <summary>
    /// Diagnostics-only lookup. Returns the policy for a feature if registered,
    /// or null if not found. Does NOT gate or enforce anything.
    /// </summary>
    public static MultiplayerPolicyRecord? Lookup(string featureId)
    {
        lock (RegistrationLock)
        {
            return Policies.FirstOrDefault(p =>
                string.Equals(p.FeatureId, featureId, StringComparison.Ordinal));
        }
    }

    /// <summary>
    /// Get all registered policies. For guard tests.
    /// </summary>
    public static IReadOnlyList<MultiplayerPolicyRecord> AllPolicies
    {
        get
        {
            lock (RegistrationLock)
            {
                return [.. Policies];
            }
        }
    }

    /// <summary>
    /// Current registered policy count. For guard tests.
    /// </summary>
    public static int PolicyCount
    {
        get
        {
            lock (RegistrationLock)
            {
                return Policies.Count;
            }
        }
    }

    /// <summary>
    /// Get all feature IDs in a specific category. For guard tests.
    /// </summary>
    internal static IReadOnlyList<string> FeaturesInCategory(MultiplayerFeatureCategory category)
    {
        lock (RegistrationLock)
        {
            return Policies
                .Where(p => p.Category == category)
                .Select(p => p.FeatureId)
                .ToArray();
        }
    }

    /// <summary>
    /// Clear all policies. For test isolation only.
    /// </summary>
    internal static void ClearPolicies()
    {
        lock (RegistrationLock)
        {
            Policies.Clear();
        }
    }
}
