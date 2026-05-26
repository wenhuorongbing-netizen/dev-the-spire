namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal interface IFeatureModule
{
    string Id { get; }

    int InitOrder { get; }

    FeatureGateResult EvaluateGate();

    void Initialize();
}
