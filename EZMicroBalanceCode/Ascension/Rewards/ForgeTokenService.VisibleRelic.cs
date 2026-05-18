namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class ForgeTokenService
{
    public static async Task SyncVisibleTokens(IRunState runState)
    {
        if (!AscensionFeatureGate.IsForgeTokenEnabled(runState))
        {
            foreach (var player in runState.Players)
            {
                await RemoveTokenRelics(player);
            }

            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            if (AscensionSavedStateFields.ForgeTokenHeld[player])
            {
                await EnsureTokenRelic(player);
            }
            else if (player.GetRelic<ForgeTokenRelic>() != null)
            {
                AscensionSavedStateFields.ForgeTokenHeld[player] = true;
            }
        }
    }

    internal static bool HasToken(Player player)
    {
        return AscensionSavedStateFields.ForgeTokenHeld[player] ||
            player.GetRelic<ForgeTokenRelic>() != null;
    }

    private static async Task EnsureTokenRelic(Player player)
    {
        var tokens = player.Relics.OfType<ForgeTokenRelic>().ToList();
        if (tokens.Count > 0)
        {
            foreach (var duplicate in tokens.Skip(1))
            {
                await RelicCmd.Remove(duplicate);
            }

            return;
        }

        await RelicCmd.Obtain<ForgeTokenRelic>(player);
    }

    private static async Task RemoveTokenRelics(Player player)
    {
        foreach (var token in player.Relics.OfType<ForgeTokenRelic>().ToList())
        {
            await RelicCmd.Remove(token);
        }
    }

    private static void MarkTokenActive(Player player)
    {
        var token = player.GetRelic<ForgeTokenRelic>();
        if (token != null)
        {
            token.Status = RelicStatus.Active;
        }
    }
}
