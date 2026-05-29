namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

/// <summary>
/// Live status of a feature after initialization.
/// </summary>
internal enum FeatureLiveStatus
{
    /// <summary>
    /// Feature was enabled by gate and initialized successfully.
    /// </summary>
    Enabled = 0,

    /// <summary>
    /// Feature was disabled by gate; initialization was skipped.
    /// </summary>
    Disabled = 1,

    /// <summary>
    /// Feature was enabled by gate but initialization threw an exception.
    /// </summary>
    Failed = 2
}

/// <summary>
/// Records the bootstrap outcome for a single feature module.
/// Created during <see cref="FeatureRegistry.InitializeAll"/> and stored for later queries.
/// </summary>
internal sealed record FeatureBootstrapRecord(
    string Id,
    string DisplayName,
    string Category,
    FeatureGateResult Gate,
    FeatureLiveStatus LiveStatus,
    string? FailureMessage = null)
{
    /// <summary>
    /// Whether the feature is active (enabled gate + successful initialization).
    /// </summary>
    public bool IsActive => LiveStatus == FeatureLiveStatus.Enabled;
}
