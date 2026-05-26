using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
    private static CombatState? CurrentCombatState()
    {
        return CombatManager.Instance.DebugOnlyGetState();
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
            var evidence = CreateBlightSproutEvidenceData(state);
            evidence["sproutRound"] = bud.SproutRound;
            evidence["pile"] = bud.Pile?.Type.ToString() ?? "none";
            ReleaseEvidenceLog.Log("BlightSprout", "sprouted_to_draw_top", player, evidence);
            MainFile.Logger.Info("[Spire Plus] Ascension Blight Sprout applied: sprouted to top of draw pile before hand draw.");
        }
    }

    private static void MarkEnteredHand(CombatState state, RootBud bud)
    {
        if (bud.HasEnteredHand)
        {
            return;
        }

        bud.HasEnteredHand = true;
        var evidence = CreateBlightSproutEvidenceData(state);
        evidence["sproutRound"] = bud.SproutRound;
        evidence["plantedInSeedbed"] = bud.PlantedInSeedbed;
        ReleaseEvidenceLog.Log("BlightSprout", "entered_hand", bud.Owner, evidence);
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Blight Sprout tracked: entered hand for player {state.RunState.GetPlayerSlotIndex(bud.Owner)}.");
    }

    private static async Task ResolveRootblightForCombatEnd(CombatState state)
    {
        foreach (var player in state.Players.Where(player => player.IsActiveForHooks))
        {
            await RootDeckService.ResolveCombatEndRootblight(player);
        }
    }

    private static Dictionary<string, object?> CreateBlightSproutEvidenceData(CombatState state)
    {
        return new Dictionary<string, object?>
        {
            ["roomType"] = state.RunState.CurrentRoom?.RoomType.ToString() ?? "none",
            ["actIndex"] = state.RunState.CurrentActIndex,
            ["floor"] = state.RunState.ActFloor,
            ["round"] = state.RoundNumber,
            ["requiredLevel"] = RequiredAscensionLevelForCurrentRoom(state)
        };
    }
}
