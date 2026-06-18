namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private sealed partial class LothaCombatState
    {
        public bool DeathReprieveActive { get; set; }

        public bool DeathReprievePendingStart { get; set; }

        public bool DeathReprieveStarted { get; set; }
    }
}
