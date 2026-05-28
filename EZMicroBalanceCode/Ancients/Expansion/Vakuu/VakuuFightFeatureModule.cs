using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuFightFeatureModule : IFeatureModule
{
    public string Id => "Ancients.VakuuFight";
    public int InitOrder => FeatureOrders.AncientsVakuuFight;

    public FeatureGateResult EvaluateGate() =>
        FeatureGateResult.EnabledByDefault("hooks registered; fight entry remains hidden by VakuuFightFeatureGate.");

    public void Initialize() => VakuuFightInitializer.Initialize();
}
