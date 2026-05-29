namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal interface IFeatureModule
{
    string Id { get; }

    int InitOrder { get; }

    FeatureGateResult EvaluateGate();

    void Initialize();

    /// <summary>
    /// Human-readable display name for logs and UI. Defaults to <see cref="Id"/>.
    /// </summary>
    string DisplayName => Id;

    /// <summary>
    /// Feature category for grouping (e.g., "Ancients", "Ascension", "Events"). Defaults to "General".
    /// </summary>
    string Category => "General";

    /// <summary>
    /// Environment variable keys that disable this feature when set to a truthy value.
    /// </summary>
    IReadOnlyList<string> DisableEnvKeys => [];

    /// <summary>
    /// Environment variable keys that force-enable this feature when set to a truthy value.
    /// </summary>
    IReadOnlyList<string> ForceEnvKeys => [];
}
