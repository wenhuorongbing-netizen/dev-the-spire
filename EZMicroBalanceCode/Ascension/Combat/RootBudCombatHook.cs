using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook : AbstractModel
{
    private static readonly ConditionalWeakTable<CombatState, AscensionCombatTracker> Trackers = new();

    public RootBudCombatHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => true;
}
