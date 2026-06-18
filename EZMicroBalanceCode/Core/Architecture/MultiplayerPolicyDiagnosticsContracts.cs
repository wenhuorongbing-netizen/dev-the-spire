namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

/// <summary>
/// Multiplayer safety categories for Spire Plus features.
/// Diagnostics-only stub. Not wired into game logic.
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
/// Not wired into game logic.
/// </summary>
internal sealed record MultiplayerPolicyRecord(
    string FeatureId,
    MultiplayerFeatureCategory Category,
    string? EnvOverride,
    bool IsVerified);
