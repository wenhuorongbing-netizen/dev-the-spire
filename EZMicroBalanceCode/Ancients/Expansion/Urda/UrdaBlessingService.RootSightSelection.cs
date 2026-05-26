using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static Player? RootSightSelectionPlayer;

    private static void ResetRootSightTransientState()
    {
        RootSightSelectionPlayer = null;
        ResetRootSightCommittedEntryKeys();
    }

    internal static int GetRootSightEyes(Player player) =>
        GetProgress(player).RootSightEyes;

    internal static bool IsRootSightSelectionActive =>
        GetActiveRootSightSelectionPlayer() != null;

    internal static void CancelRootSightSelection()
    {
        if (RootSightSelectionPlayer == null)
        {
            return;
        }

        var player = RootSightSelectionPlayer;
        RootSightSelectionPlayer = null;
        NMapScreen.Instance?.RefreshAllPointVisuals();
        ReleaseEvidenceLog.Log("UrdaRootEyes", "selection_cancelled", player);
        MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection cancelled.");
    }

    internal static bool TryBeginRootSightSelection(Player player)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0)
        {
            return false;
        }

        var hasMultiplayerRunState = player.RunState.Players.Count > 1;
        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
            player.RunState,
            "UrdaRootEyes",
            "Root Eyes shared map preview mutation is pending host-authoritative sync proof") ||
            hasMultiplayerRunState)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Root Eyes preview is single-player only until host-authoritative map preview sync is implemented.");
            return false;
        }

        var mapScreen = NMapScreen.Instance;
        if (mapScreen == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Root Eyes selection could not start because the map screen is not available.");
            return false;
        }

        RootSightSelectionPlayer = player;
        RefreshRootSightRelicStatus(player);
        mapScreen.Open(isOpenedFromTopBar: true);
        mapScreen.RefreshAllPointVisuals();
        ReleaseEvidenceLog.Log("UrdaRootEyes", "selection_opened", player);
        MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection started; choose a future reachable Monster, Unknown, or Elite map node.");
        return true;
    }

    private static Player? GetActiveRootSightSelectionPlayer()
    {
        var player = RootSightSelectionPlayer;
        if (player == null)
        {
            return null;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null ||
            !ReferenceEquals(player.RunState, runState) ||
            !runState.Players.Contains(player))
        {
            RootSightSelectionPlayer = null;
            ReleaseEvidenceLog.Log(
                "UrdaRootEyes",
                "selection_cleared_context_changed",
                player,
                new Dictionary<string, object?>
                {
                    ["hasRunState"] = runState != null
                });
            MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection cleared after run context changed.");
            return null;
        }

        return player;
    }
}
