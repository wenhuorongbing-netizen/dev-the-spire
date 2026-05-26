using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
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
        var rootblightEnabled = AscensionFeatureGate.IsRootblightEnabled(state.RunState);
        if (rootblightEnabled)
        {
            // Run and room hooks normally add the starter Rootblight first. Combat start
            // is the last safe repair point before combat-end growth bookkeeping.
            await RootDeckService.EnsureStartingRoot(state.RunState);
            foreach (var player in state.Players.Where(player => player.IsActiveForHooks))
            {
                RootDeckService.MarkCombatStartRootblight(player);
            }
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            var evidence = CreateBlightSproutEvidenceData(state);
            evidence["rootblightEnabled"] = rootblightEnabled;
            evidence["bossSproutEnabled"] = AscensionFeatureGate.IsBossBlightSproutEnabled(state.RunState);
            evidence["eliteSproutEnabled"] = AscensionFeatureGate.IsEliteBlightSproutEnabled(state.RunState);
            ReleaseEvidenceLog.Log("BlightSprout", "gate_skipped", runState: state.RunState, data: evidence);
            return;
        }

        if (tracker.Seeded)
        {
            var evidence = CreateBlightSproutEvidenceData(state);
            evidence["trackedBuds"] = tracker.Buds.Count;
            ReleaseEvidenceLog.Log("BlightSprout", "seed_already_tracked", runState: state.RunState, data: evidence);
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
                var evidence = CreateBlightSproutEvidenceData(state);
                evidence["targetBudCount"] = targetBudCount;
                evidence["removed"] = existingBuds.Count - targetBudCount;
                ReleaseEvidenceLog.Log("BlightSprout", "duplicates_removed", player, evidence);
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension Blight Sprout normalized: removed {existingBuds.Count - targetBudCount} duplicate Blight Sprout card(s) for player {state.RunState.GetPlayerSlotIndex(player)}.");
                existingBuds = existingBuds.Take(targetBudCount).ToList();
            }

            NormalizeExistingRootBudRounds(state, existingBuds);
            if (existingBuds.Count >= targetBudCount)
            {
                foreach (var existingBud in existingBuds)
                {
                    tracker.Buds.Add(existingBud);
                }

                var evidence = CreateBlightSproutEvidenceData(state);
                evidence["targetBudCount"] = targetBudCount;
                evidence["existing"] = existingBuds.Count;
                ReleaseEvidenceLog.Log("BlightSprout", "existing_reused", player, evidence);
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

            var seedEvidence = CreateBlightSproutEvidenceData(state);
            seedEvidence["targetBudCount"] = targetBudCount;
            seedEvidence["existing"] = existingBuds.Count;
            seedEvidence["added"] = targetBudCount - existingBuds.Count;
            seedEvidence["sproutRounds"] = string.Join(
                "|",
                Enumerable.Range(existingBuds.Count, targetBudCount - existingBuds.Count)
                    .Select(i => GetRootBudSproutRoundForCurrentRoom(state, i)));
            ReleaseEvidenceLog.Log("BlightSprout", "seeded", player, seedEvidence);
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension Blight Sprout applied: added {targetBudCount - existingBuds.Count} Blight Sprout card(s) to discard for player {state.RunState.GetPlayerSlotIndex(player)}.");
        }

        AscensionDiagnostics.LogCombatState(state, "before combat start after root bud seed");
    }
}
