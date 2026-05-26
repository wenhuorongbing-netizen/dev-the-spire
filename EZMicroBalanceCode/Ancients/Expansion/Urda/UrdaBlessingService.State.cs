using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        SetState(player, blessingId, UrdaProgress.Default);
    }

    public static string GetSelectedBlessing(Player player)
    {
        return ReadState(player).SelectedBlessing;
    }

    public static void SyncPersistentState(Player? player)
    {
        if (player == null)
        {
            return;
        }

        AncientPlayerState.SyncDeck(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
        if (GetSelectedBlessing(player) == UrdaBlessingIds.TrialBranch)
        {
            RefreshTrialBranchEnchantment(player);
        }
    }

    private static UrdaProgress GetProgress(Player player)
    {
        return ReadState(player).Progress;
    }

    private static void SetProgress(Player player, UrdaProgress progress)
    {
        var selectedBlessing = GetSelectedBlessing(player);
        if (!string.IsNullOrWhiteSpace(selectedBlessing))
        {
            SetState(player, selectedBlessing, progress);
        }
    }

    private static void SetState(Player player, string blessingId, UrdaProgress progress)
    {
        AncientPlayerState.Set(
            player,
            UrdaStateCodec.Encode(new UrdaStateSnapshot(blessingId, progress)),
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
    }

    private static UrdaStateSnapshot ReadState(Player player)
    {
        return UrdaStateCodec.Decode(AncientPlayerState.Get(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey));
    }
}
