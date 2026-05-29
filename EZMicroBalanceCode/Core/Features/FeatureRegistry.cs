namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal sealed class FeatureRegistry
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
        foreach (var module in modules
                     .OrderBy(module => module.InitOrder)
                     .ThenBy(module => module.Id, StringComparer.Ordinal))
        {
            var gate = module.EvaluateGate();
            logInfo($"[Spire Plus] Feature {module.DisplayName} ({module.Category}) bootstrap gate: {(gate.IsEnabled ? "enabled" : "disabled")} ({gate.Reason}).");

            if (!gate.IsEnabled)
            {
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Disabled));
                continue;
            }

            try
            {
                module.Initialize();
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Enabled));
            }
            catch (Exception exception)
            {
                logWarn($"[Spire Plus] Feature {module.DisplayName} initialization failed: {exception.GetType().Name}: {exception.Message}");
                bootstrapRecords.Add(new FeatureBootstrapRecord(
                    module.Id, module.DisplayName, module.Category,
                    gate, FeatureLiveStatus.Failed, $"{exception.GetType().Name}: {exception.Message}"));
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
            logInfo($"[Spire Plus] Feature {record.DisplayName}: bootstrap={(record.Gate.IsEnabled ? "enabled" : "disabled")}, live={record.LiveStatus}, reason={record.Gate.Reason}");
        }
    }
}
