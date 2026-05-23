using MegaCrit.Sts2.Core.Map;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    public static void AfterMapGenerated(ActMap map, int actIndex)
    {
        RestoreRootSightPreviewMarkers(map, actIndex);
    }

    public static async Task AfterActEntered()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null || runState.CurrentActIndex < 1)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            var progress = GetProgress(player);
            if (progress.MoltingActive)
            {
                var husks = PileType.Deck.GetPile(player).Cards.OfType<WitheredHusk>().Cast<CardModel>().ToList();
                foreach (var husk in husks)
                {
                    await CardPileCmd.RemoveFromDeck(husk, showPreview: false);
                }

                if (husks.Count > 0)
                {
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Urda Molting applied: removed {husks.Count} Withered Husk card(s) at Act {runState.CurrentActIndex + 1} start.");
                }

                SetProgress(player, progress with { MoltingActive = false });
            }

            if (selectedBlessing == UrdaBlessingIds.ShallowRootRelic)
            {
                await SettleUnrootedShallowRelicAtActTwo(player);
            }

            if (selectedBlessing == UrdaBlessingIds.AfterRain)
            {
                await CompensateAfterRainAtActTwo(player);
            }

            RefreshSeedBankRelicStatus(player);
            if (GetSeedBankStoredCount(player) > 0)
            {
                ReleaseEvidenceLog.Log(
                    "UrdaSeedBank",
                    "save_hydrate_storage_restored",
                    player,
                    new Dictionary<string, object?>
                    {
                        ["stored"] = GetSeedBankStoredCount(player),
                        ["settled"] = IsSeedBankSettled(player)
                    });
            }
            RefreshRootSightRelicStatus(player);
        }
    }

    public static async Task BeforeRoomEntered(AbstractRoom room)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        if (room is CombatRoom)
        {
            foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
            {
                ClearSeedbed(player);
                ResetAfterRainCombatTrigger(player);
            }
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            ClearUnreachableRootSightPreviews(player, runState);
        }

        if (runState.CurrentActIndex != 0)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {

            var selectedBlessing = GetSelectedBlessing(player);
            if (selectedBlessing == UrdaBlessingIds.RootedRoute)
            {
                await CheckRootedRouteBeforeRoom(player);
            }

            RefreshSeedBankRelicStatus(player);
        }
    }

    public static async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is not (RoomType.Monster or RoomType.Event or RoomType.Shop or RoomType.Elite or RoomType.RestSite))
        {
            return;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null || runState.CurrentActIndex != 0)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player =>
            player.IsActiveForHooks &&
            GetSelectedBlessing(player) == UrdaBlessingIds.MossMap))
        {
            var progress = GetProgress(player);
            var roomMask = GetRoomMask(room.RoomType);
            if ((progress.MossRoomMask & roomMask) != 0)
            {
                continue;
            }

            progress = progress with { MossRoomMask = progress.MossRoomMask | roomMask };
            SetProgress(player, progress);
            await ApplyMossMapRoomReward(player, room.RoomType);
        }
    }

    public static async Task AfterCombatVictory(CombatRoom room)
    {
        var runState = room.CombatState.RunState;

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            ClearSeedbed(player);
            var selectedBlessing = GetSelectedBlessing(player);
            if (selectedBlessing == UrdaBlessingIds.TrialBranch)
            {
                await ResolveTrialBranchCombat(player);
            }

            if (runState.CurrentActIndex != 0)
            {
                continue;
            }

            if (selectedBlessing == UrdaBlessingIds.ShallowRootRelic && room.RoomType == RoomType.Elite)
            {
                await RootShallowRelicFromElite(player);
            }

            if (selectedBlessing == UrdaBlessingIds.RootedRoute)
            {
                await TryResolveRootedRouteReward(player);
            }
        }
    }

    private static int GetRoomMask(RoomType roomType) => 1 << (int)roomType;
}
