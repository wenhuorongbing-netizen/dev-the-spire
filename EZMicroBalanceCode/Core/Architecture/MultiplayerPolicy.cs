namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Registry for multiplayer feature policies. Diagnostics-only stub.
/// Not wired into game logic.
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
