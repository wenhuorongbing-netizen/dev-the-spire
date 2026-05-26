namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal sealed class FeatureRegistry
{
    private readonly List<IFeatureModule> modules = [];
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
            logInfo($"[Spire Plus] Feature {module.Id} bootstrap gate: {(gate.IsEnabled ? "enabled" : "disabled")} ({gate.Reason}).");

            if (!gate.IsEnabled)
            {
                continue;
            }

            try
            {
                module.Initialize();
            }
            catch (Exception exception)
            {
                logWarn($"[Spire Plus] Feature {module.Id} initialization failed: {exception.GetType().Name}: {exception.Message}");
                throw;
            }
        }
    }
}
