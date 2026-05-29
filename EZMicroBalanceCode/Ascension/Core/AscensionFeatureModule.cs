using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionFeatureModule : IFeatureModule
{
    public string Id => "Ascension.A11A20";
    public int InitOrder => FeatureOrders.AscensionA11A20;
    public string DisplayName => "Ascension 11-20";
    public string Category => "Ascension";

    public FeatureGateResult EvaluateGate() =>
        FeatureGateResult.EnabledByDefault("default-on for single-player; co-op gameplay gates remain in AscensionFeatureGate.");

    public void Initialize() => AscensionInitializer.Initialize();
}
