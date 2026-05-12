using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RootBudCombatHook : AbstractModel
{
    private static readonly ConditionalWeakTable<CombatState, AscensionCombatTracker> Trackers = new();

    public RootBudCombatHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeCombatStart()
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        AscensionDiagnostics.LogCombatState(state, "before combat start before root bud seed");

        var tracker = GetTracker(state);
        await AscensionCombatModifierService.BeforeCombatStart(state, tracker);
        if (AscensionFeatureGate.IsRootblightEnabled(state.RunState))
        {
            foreach (var player in state.Players.Where(player => player.IsActiveForHooks))
            {
                RootDeckService.MarkCombatStartRootblight(player);
            }
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        if (tracker.Seeded)
        {
            return;
        }

        tracker.Seeded = true;
        foreach (var player in state.Players.Where(player => player.IsActiveForHooks))
        {
            var targetBudCount = GetRootBudCountForCurrentRoom(state);
            var existingBuds = FindRootBudsInCombat(player).ToList();
            foreach (var duplicateBud in existingBuds.Skip(targetBudCount).ToList())
            {
                await CardPileCmd.RemoveFromCombat(duplicateBud, skipVisuals: true);
                tracker.Buds.Remove(duplicateBud);
            }

            if (existingBuds.Count > targetBudCount)
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension Blight Sprout normalized: removed {existingBuds.Count - targetBudCount} duplicate Blight Sprout card(s) for player {state.RunState.GetPlayerSlotIndex(player)}.");
                existingBuds = existingBuds.Take(targetBudCount).ToList();
            }

            if (existingBuds.Count >= targetBudCount)
            {
                foreach (var existingBud in existingBuds)
                {
                    tracker.Buds.Add(existingBud);
                }

                continue;
            }

            foreach (var existingBud in existingBuds)
            {
                tracker.Buds.Add(existingBud);
            }

            for (var i = existingBuds.Count; i < targetBudCount; i++)
            {
                var bud = state.CreateCard<RootBud>(player);
                bud.SproutRound = GetRootBudSproutRoundForCurrentRoom(state, i);
                tracker.Buds.Add(bud);
                await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom);
            }

            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension Blight Sprout applied: added {targetBudCount - existingBuds.Count} Blight Sprout card(s) to discard for player {state.RunState.GetPlayerSlotIndex(player)}.");
        }

        AscensionDiagnostics.LogCombatState(state, "before combat start after root bud seed");
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        var tracker = GetTracker(state);
        await AscensionCombatModifierService.AfterPlayerTurnStart(state, tracker);

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        foreach (var bud in FindKnownBuds(state)
                     .Where(bud => bud.Owner == player && ShouldSprout(state, bud))
                     .ToList())
        {
            bud.HasSprouted = true;
            await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top);
            MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout applied: sprouted to top of draw pile.");
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        if (card is RootBud bud)
        {
            GetTracker(state).Buds.Add(bud);
            if (bud.Pile?.Type == PileType.Hand)
            {
                MarkEnteredHand(state, bud);
            }
        }

        if (card.Pile?.Type == PileType.Hand)
        {
            await AscensionCombatModifierService.AfterCardEnteredHand(state, GetTracker(state), card);
        }

        return;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        var state = CurrentCombatState();
        if (state == null || !IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        if (card is RootBud bud)
        {
            GetTracker(state).Buds.Add(bud);
            MarkEnteredHand(state, bud);
        }

        await AscensionCombatModifierService.AfterCardEnteredHand(state, GetTracker(state), card);
        return;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        if (cardPlay.Card is RootBud bud)
        {
            GetTracker(state).Buds.Add(bud);
            bud.WasPlayed = true;
            MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout tracked: played before combat end.");
        }

        await AscensionCombatModifierService.AfterCardPlayed(state, GetTracker(state), cardPlay);
        return;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterDamageReceived(state, GetTracker(state), target, result, dealer, cardSource);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterCurrentHpChanged(state, GetTracker(state), creature, delta);
    }

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterShuffle(state, GetTracker(state), shuffler);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterSideTurnStart(state, GetTracker(state), side);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterTurnEnd(state, GetTracker(state), side);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        var state = CurrentCombatState();
        if (state == null || wasRemovalPrevented)
        {
            return;
        }

        await AscensionCombatModifierService.AfterDeath(state, GetTracker(state), creature, wasRemovalPrevented);

        if (creature.Player == null)
        {
            return;
        }

        GetTracker(state).DiedPlayers.Add(creature.Player);
        MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout tracked: player death clears combat-only Blight Sprout growth.");
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        AscensionDiagnostics.LogCombatState(state, "after combat end before root growth");
        var tracker = GetTracker(state);
        await AscensionCombatModifierService.AfterCombatEnd(state, tracker);

        var rootblightEnabled = AscensionFeatureGate.IsRootblightEnabled(state.RunState);
        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            if (rootblightEnabled)
            {
                await ResolveRootblightForCombatEnd(state);
            }

            Trackers.Remove(state);
            AscensionDiagnostics.LogCombatState(state, "after combat end without Blight Sprout growth");
            return;
        }

        if (rootblightEnabled)
        {
            await ResolveRootblightForCombatEnd(state);
        }

        var budsWithGrowth = FindKnownBuds(state)
            .Where(bud => bud.HasEnteredHand && !bud.WasPlayed)
            .Where(bud => bud.Owner.IsActiveForHooks)
            .Where(bud => !tracker.DiedPlayers.Contains(bud.Owner))
            .ToList();

        foreach (var bud in budsWithGrowth)
        {
            await RootDeckService.AddRootblightI(bud.Owner, "Blight Sprout");
        }

        if (budsWithGrowth.Count > 0)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension Blight Sprout applied: added {budsWithGrowth.Count} Rootblight I card(s) from unplayed sprout(s).");
        }

        Trackers.Remove(state);
        AscensionDiagnostics.LogCombatState(state, "after combat end after Rootblight sync");
    }

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

    private static int? RequiredAscensionLevelForCurrentRoom(CombatState state)
    {
        return state.RunState.CurrentRoom?.RoomType switch
        {
            RoomType.Boss when IsActTwoOrThree(state) && !IsSecondBossFight(state) => AscensionFeatureGate.BossRootBudLevel,
            RoomType.Elite when IsEligibleEliteSproutFight(state) => AscensionFeatureGate.EliteRootBudLevel,
            _ => null
        };
    }

    private static bool IsActTwoOrThree(CombatState state)
    {
        return state.RunState.CurrentActIndex is 1 or 2;
    }

    private static bool IsSecondBossFight(CombatState state)
    {
        return state.RunState.Map.SecondBossMapPoint != null &&
            state.RunState.CurrentMapCoord == state.RunState.Map.SecondBossMapPoint.coord;
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
