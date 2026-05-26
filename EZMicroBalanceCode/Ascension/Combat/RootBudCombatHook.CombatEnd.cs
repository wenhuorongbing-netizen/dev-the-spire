using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
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
            .Where(bud => !bud.PlantedInSeedbed)
            .Where(bud => bud.Owner.IsActiveForHooks)
            .Where(bud => !tracker.DiedPlayers.Contains(bud.Owner))
            .ToList();

        foreach (var bud in budsWithGrowth)
        {
            var evidence = CreateBlightSproutEvidenceData(state);
            evidence["sproutRound"] = bud.SproutRound;
            evidence["wasPlayed"] = bud.WasPlayed;
            evidence["plantedInSeedbed"] = bud.PlantedInSeedbed;
            ReleaseEvidenceLog.Log("BlightSprout", "growth_rootblight_queued", bud.Owner, evidence);
            await RootDeckService.AddRootblightI(bud.Owner, "Blight Sprout");
        }

        if (budsWithGrowth.Count == 0)
        {
            var evidence = CreateBlightSproutEvidenceData(state);
            evidence["knownBuds"] = FindKnownBuds(state).Count;
            evidence["diedPlayers"] = tracker.DiedPlayers.Count;
            ReleaseEvidenceLog.Log("BlightSprout", "combat_end_no_growth", runState: state.RunState, data: evidence);
        }

        if (budsWithGrowth.Count > 0)
        {
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension Blight Sprout applied: added {budsWithGrowth.Count} Rootblight I card(s) from unplayed sprout(s).");
        }

        foreach (var bud in FindKnownBuds(state).Where(bud => bud.PlantedInSeedbed))
        {
            bud.PlantedInSeedbed = false;
        }

        Trackers.Remove(state);
        AscensionDiagnostics.LogCombatState(state, "after combat end after Rootblight sync");
    }
}
