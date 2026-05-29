using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaFeatureModule : IFeatureModule
{
    public string Id => "Ancients.Urda";
    public int InitOrder => FeatureOrders.AncientsUrda;
    public string DisplayName => "Urda Ancient";
    public string Category => "Ancients";

    public FeatureGateResult EvaluateGate() =>
        FeatureGateResult.EnabledByDefault("default-on; Urda runtime gates remain in UrdaFeatureGate.");

    public void Initialize() => UrdaInitializer.Initialize();
}
