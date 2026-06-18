using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal sealed partial class FeatureRegistry
{
    private readonly List<IFeatureModule> modules = [];
    private readonly List<FeatureBootstrapRecord> bootstrapRecords = [];
    private readonly Action<string> logInfo;
    private readonly Action<string> logWarn;

    public FeatureRegistry(Action<string> logInfo, Action<string> logWarn)
    {
        this.logInfo = logInfo;
        this.logWarn = logWarn;
    }

    public FeatureRegistry Register(IFeatureModule module)
    {
        modules.Add(module);
        return this;
    }

    public void InitializeAll()
    {
        bootstrapRecords.Clear();

        foreach (var module in modules
                     .OrderBy(module => module.InitOrder)
                     .ThenBy(module => module.Id, StringComparer.Ordinal))
        {
            var gate = ApplyEnvironmentOverrides(module, module.EvaluateGate());
            logInfo($"[Spire Plus] Feature {module.DisplayName} ({module.Category}) bootstrap gate: {(gate.IsEnabled ? "enabled" : "disabled")} ({gate.Reason}).");
            RewardPipeline.Diagnose(CreateBootstrapContext(module, gate, "FeatureBootstrapGate"));

            if (!gate.IsEnabled)
            {
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Disabled));
                RewardPipeline.Diagnose(CreateBootstrapContext(module, gate, "FeatureBootstrapDisabled", FeatureLiveStatus.Disabled));
                continue;
            }

            try
            {
                module.Initialize();
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Enabled));
                RewardPipeline.Diagnose(CreateBootstrapContext(module, gate, "FeatureBootstrapInitialized", FeatureLiveStatus.Enabled));
            }
            catch (Exception exception)
            {
                logWarn($"[Spire Plus] Feature {module.DisplayName} initialization failed: {exception.GetType().Name}: {exception.Message}");
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Failed, $"{exception.GetType().Name}: {exception.Message}"));
                RewardPipeline.Diagnose(CreateBootstrapContext(module, gate, "FeatureBootstrapFailed", FeatureLiveStatus.Failed, exception.Message));
                throw;
            }
        }
    }

    /// <summary>
    /// Bootstrap records from the last <see cref="InitializeAll"/> call.
    /// </summary>
    public IReadOnlyList<FeatureBootstrapRecord> BootstrapRecords => bootstrapRecords;

    /// <summary>
    /// Query bootstrap status for a feature by ID.
    /// </summary>
    public FeatureBootstrapRecord? GetBootstrapRecord(string id) =>
        bootstrapRecords.FirstOrDefault(r => r.Id == id);

    /// <summary>
    /// Log a summary of all feature statuses. Call after InitializeAll.
    /// </summary>
    public void LogFeatureSummary()
    {
        foreach (var record in bootstrapRecords)
        {
            logInfo($"[Spire Plus] Feature {record.Id} ({record.DisplayName}, {record.Category}): bootstrap={(record.Gate.IsEnabled ? "enabled" : "disabled")}, live={record.LiveStatus}, reason={record.Gate.Reason}");
        }
    }

    private static RewardPipelineContext CreateBootstrapContext(
        IFeatureModule module,
        FeatureGateResult gate,
        string eventName,
        FeatureLiveStatus? liveStatus = null,
        string? failureMessage = null) =>
        new()
        {
            Feature = "FeatureRegistry",
            EventName = eventName,
            Data = new Dictionary<string, object?>
            {
                ["id"] = module.Id,
                ["displayName"] = module.DisplayName,
                ["category"] = module.Category,
                ["bootstrapEnabled"] = gate.IsEnabled,
                ["reason"] = gate.Reason,
                ["liveStatus"] = liveStatus?.ToString(),
                ["failureMessage"] = failureMessage
            }
        };
}
