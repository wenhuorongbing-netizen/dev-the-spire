using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviFeatureModule : IFeatureModule
{
    public string Id => "Ancients.Morvi";
    public int InitOrder => FeatureOrders.AncientsMorvi;
    public string DisplayName => "Morvi Ancient";
    public string Category => "Ancients";

    public FeatureGateResult EvaluateGate() =>
        FeatureGateResult.EnabledByDefault("default-on; Morvi runtime gates remain in MorviFeatureGate.");

    public void Initialize() => MorviInitializer.Initialize();
}
