using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaFeatureModule : IFeatureModule
{
    public string Id => "Ancients.Lotha";
    public int InitOrder => FeatureOrders.AncientsLotha;

    public FeatureGateResult EvaluateGate() =>
        FeatureGateResult.EnabledByDefault("default-on; Lotha runtime gates remain in LothaFeatureGate.");

    public void Initialize() => LothaInitializer.Initialize();
}
