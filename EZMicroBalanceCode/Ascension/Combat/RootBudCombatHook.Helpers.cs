using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
    private static CombatState? CurrentCombatState()
    {
        return CombatManager.Instance.DebugOnlyGetState();
    }

    private static bool IsGameplayEnabledForCurrentRoom(CombatState state)
    {
        var requiredLevel = RequiredAscensionLevelForCurrentRoom(state);
        return requiredLevel switch
        {
            AscensionFeatureGate.BossRootBudLevel => AscensionFeatureGate.IsBossBlightSproutEnabled(state.RunState),
            AscensionFeatureGate.EliteRootBudLevel => AscensionFeatureGate.IsEliteBlightSproutEnabled(state.RunState),
            _ => false
        };
    }

    private static int GetRootBudCountForCurrentRoom(CombatState state)
    {
        return state.RunState.CurrentRoom?.RoomType == RoomType.Boss
            ? 2
            : 1;
    }

    private static int GetRootBudSproutRoundForCurrentRoom(CombatState state, int budIndex)
    {
        return state.RunState.CurrentRoom?.RoomType == RoomType.Boss && budIndex == 1
            ? RootBud.BossSecondSproutRound
            : RootBud.DefaultSproutRound;
    }

    private static void NormalizeExistingRootBudRounds(CombatState state, IReadOnlyList<RootBud> existingBuds)
    {
        var targetRounds = Enumerable.Range(0, existingBuds.Count)
            .Select(i => GetRootBudSproutRoundForCurrentRoom(state, i))
            .ToList();
        for (var i = 0; i < existingBuds.Count; i++)
        {
            existingBuds[i].SproutRound = targetRounds[i];
        }
    }

    private static int? RequiredAscensionLevelForCurrentRoom(CombatState state)
    {
        return state.RunState.CurrentRoom?.RoomType switch
        {
            RoomType.Boss when IsActTwoOrThree(state) => AscensionFeatureGate.BossRootBudLevel,
            RoomType.Elite when IsEligibleEliteSproutFight(state) => AscensionFeatureGate.EliteRootBudLevel,
            _ => null
        };
    }

    private static bool IsActTwoOrThree(CombatState state)
    {
        return state.RunState.CurrentActIndex is 1 or 2;
    }

    private static bool IsEligibleEliteSproutFight(CombatState state)
    {
        if (!IsActTwoOrThree(state))
        {
            return false;
        }

        var currentRow = state.RunState.CurrentMapPoint?.coord.row ?? state.RunState.ActFloor - 1;
        return currentRow >= 3;
    }

    private static AscensionCombatTracker GetTracker(CombatState state)
    {
        return Trackers.GetValue(state, _ => new AscensionCombatTracker());
    }

    private static IReadOnlyList<RootBud> FindKnownBuds(CombatState state)
    {
        var tracker = GetTracker(state);
        foreach (var bud in state.Players
                     .SelectMany(player => player.Piles)
                     .SelectMany(pile => pile.Cards)
                     .OfType<RootBud>())
        {
            tracker.Buds.Add(bud);
        }

        return tracker.Buds.ToList();
    }

    private static IReadOnlyList<RootBud> FindRootBudsInCombat(Player player)
    {
        return player.Piles
            .SelectMany(pile => pile.Cards)
            .OfType<RootBud>()
            .ToList();
    }

    private static bool ShouldSprout(CombatState state, RootBud bud)
    {
        return !bud.HasEnteredHand &&
            !bud.WasPlayed &&
            !bud.HasSprouted &&
            state.RoundNumber >= bud.SproutRound &&
            bud.Pile?.Type is PileType.Draw or PileType.Discard;
    }

    private static async Task SproutDueBudsBeforeHandDraw(CombatState state, Player player)
    {
        foreach (var bud in FindKnownBuds(state)
                     .Where(bud => bud.Owner == player && ShouldSprout(state, bud))
                     .ToList())
        {
            bud.HasSprouted = true;
            await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top);
            MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout applied: sprouted to top of draw pile before hand draw.");
        }
    }

    private static void MarkEnteredHand(CombatState state, RootBud bud)
    {
        if (bud.HasEnteredHand)
        {
            return;
        }

        bud.HasEnteredHand = true;
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension Blight Sprout tracked: entered hand for player {state.RunState.GetPlayerSlotIndex(bud.Owner)}.");
    }

    private static async Task ResolveRootblightForCombatEnd(CombatState state)
    {
        foreach (var player in state.Players.Where(player => player.IsActiveForHooks))
        {
            await RootDeckService.ResolveCombatEndRootblight(player);
        }
    }
}
