using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private sealed class SeedBankExtractionState
    {
        public bool InProgress { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, SeedBankExtractionState> SeedBankExtractionInProgress = new();
}
