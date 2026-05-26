using System.Runtime.CompilerServices;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private sealed class SeedBankExtractionState
    {
        public bool InProgress { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, SeedBankExtractionState> SeedBankExtractionInProgress = new();

    public static async Task TryExtractSeedBankFromRelicClick(Player player)
    {
        var extractionState = SeedBankExtractionInProgress.GetOrCreateValue(player);
        if (extractionState.InProgress)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Seed Bank extraction ignored: a Seed Bank selection is already open.");
            return;
        }

        extractionState.InProgress = true;
        try
        {
            await TryExtractSeedBankFromRelicClickOnce(player);
        }
        finally
        {
            extractionState.InProgress = false;
        }
    }
}
