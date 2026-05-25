using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal static class TransformPredictionRngContext
{
    private static readonly ConditionalWeakTable<Player, Snapshot> SnapshotsByPlayer = new();

    public static void Register(
        Player? player,
        Rng? source,
        string sourceName,
        bool upgradeReplacementPreview = false)
    {
        if (player == null || source == null)
        {
            return;
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopPreviewTool(
                player.RunState,
                "PreviewTransform",
                "transform prediction previews use local UI and forked transform RNG contexts that still need two-client proof"))
        {
            Clear(player);
            return;
        }

        SnapshotsByPlayer.Remove(player);
        SnapshotsByPlayer.Add(player, new Snapshot(
            source,
            source.Seed,
            source.Counter,
            sourceName,
            upgradeReplacementPreview));
        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "rng_context_registered",
            player,
            new Dictionary<string, object?>
            {
                ["source"] = sourceName,
                ["seed"] = source.Seed,
                ["counter"] = source.Counter,
                ["upgradePreview"] = upgradeReplacementPreview
            });
        PreviewLog.Debug($"Registered transform prediction RNG source: {sourceName}.");
    }

    public static bool TryConsume(
        Player player,
        out Rng fork,
        out string sourceName,
        out bool upgradeReplacementPreview)
    {
        if (!SnapshotsByPlayer.TryGetValue(player, out var snapshot))
        {
            fork = null!;
            sourceName = string.Empty;
            upgradeReplacementPreview = false;
            return false;
        }

        if (snapshot.Source.Seed != snapshot.Seed || snapshot.Source.Counter != snapshot.Counter)
        {
            fork = null!;
            sourceName = string.Empty;
            upgradeReplacementPreview = false;
            Clear(player);
            ReleaseEvidenceLog.Log(
                "PreviewTransform",
                "rng_context_stale",
                player,
                new Dictionary<string, object?>
                {
                    ["source"] = snapshot.SourceName
                });
            PreviewLog.Debug($"Transform prediction skipped: stale RNG source {snapshot.SourceName}.");
            return false;
        }

        fork = new Rng(snapshot.Seed, snapshot.Counter);
        sourceName = snapshot.SourceName;
        upgradeReplacementPreview = snapshot.UpgradeReplacementPreview;
        return true;
    }

    public static void Clear(Player? player)
    {
        if (player != null)
        {
            SnapshotsByPlayer.Remove(player);
            ReleaseEvidenceLog.Log("PreviewTransform", "rng_context_cleared", player);
        }
    }

    private sealed record Snapshot(
        Rng Source,
        uint Seed,
        int Counter,
        string SourceName,
        bool UpgradeReplacementPreview);
}

[HarmonyPatch]
internal static class TransformPredictionEventRngSourcePatches
{
    [HarmonyPatch(typeof(AromaOfChaos), "LetGo")]
    [HarmonyPrefix]
    private static void AromaOfChaosLetGo(AromaOfChaos __instance) =>
        RegisterEventRng(__instance, nameof(AromaOfChaos));

    [HarmonyPatch(typeof(EndlessConveyor), "JellyLiver")]
    [HarmonyPrefix]
    private static void EndlessConveyorJellyLiver(EndlessConveyor __instance) =>
        RegisterEventRng(__instance, nameof(EndlessConveyor));

    [HarmonyPatch(typeof(Symbiote), "KillWithFire")]
    [HarmonyPrefix]
    private static void SymbioteKillWithFire(Symbiote __instance) =>
        RegisterEventRng(__instance, nameof(Symbiote));

    [HarmonyPatch(typeof(WhisperingHollow), "Hug")]
    [HarmonyPrefix]
    private static void WhisperingHollowHug(WhisperingHollow __instance) =>
        RegisterEventRng(__instance, nameof(WhisperingHollow));

    private static void RegisterEventRng(EventModel source, string sourceName) =>
        TransformPredictionRngContext.Register(source.Owner, source.Rng, $"{sourceName}.Rng");
}

[HarmonyPatch]
internal static class TransformPredictionNicheRngSourcePatches
{
    [HarmonyPatch(typeof(MorphicGrove), "Group")]
    [HarmonyPrefix]
    private static void MorphicGroveGroup(MorphicGrove __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(MorphicGrove));

    [HarmonyPatch(typeof(Trial), "NondescriptInnocent")]
    [HarmonyPrefix]
    private static void TrialNondescriptInnocent(Trial __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(Trial));

    [HarmonyPatch(typeof(NewLeaf), nameof(NewLeaf.AfterObtained))]
    [HarmonyPrefix]
    private static void NewLeafAfterObtained(NewLeaf __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(NewLeaf));

    [HarmonyPatch(typeof(Astrolabe), nameof(Astrolabe.AfterObtained))]
    [HarmonyPrefix]
    private static void AstrolabeAfterObtained(Astrolabe __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(Astrolabe), upgradeReplacementPreview: true);

    private static void RegisterNicheRng(
        Player? player,
        string sourceName,
        bool upgradeReplacementPreview = false) =>
        TransformPredictionRngContext.Register(
            player,
            player?.RunState.Rng.Niche,
            $"{sourceName}.RunState.Rng.Niche",
            upgradeReplacementPreview);
}

[HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromDeckForTransformation))]
internal static class TransformPredictionSelectionLifetimePatch
{
    private static void Postfix(Player player, ref Task<IEnumerable<CardModel>> __result)
    {
        __result = ClearContextWhenSelectionCompletes(player, __result);
    }

    private static async Task<IEnumerable<CardModel>> ClearContextWhenSelectionCompletes(
        Player player,
        Task<IEnumerable<CardModel>> selectionTask)
    {
        try
        {
            return await selectionTask;
        }
        finally
        {
            TransformPredictionRngContext.Clear(player);
        }
    }
}
