using EZMicroBalance.EZMicroBalanceCode.Core.Features;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events;

/// <summary>
/// Feature module for StS1 event port. Default Off — zero registrations unless
/// explicitly enabled through the StS1 event mode selector.
/// </summary>
internal sealed class Sts1EventsFeatureModule : IFeatureModule
{
    public string Id => "Sts1Events";
    public int InitOrder => FeatureOrders.Sts1Events;
    public string DisplayName => "StS1 Event Port";
    public string Category => "Events";

    public FeatureGateResult EvaluateGate() => Sts1EventFeatureGate.EvaluateGate();

    public void Initialize()
    {
        var mode = Sts1EventFeatureGate.ResolveMode();
        Sts1EventRegistrationService.RegisterGated(MainFile.ModId, mode);
    }
}
