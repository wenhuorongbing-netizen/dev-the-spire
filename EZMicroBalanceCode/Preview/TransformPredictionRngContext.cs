using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Entities.Players;
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

        if (!MultiplayerFeaturePolicy.IsSingleplayer(player.RunState))
        {
            MultiplayerFeaturePolicy.LogCoopEvidence(
                "PreviewTransform",
                "coop_ui_only_rng_context_registered",
                player.RunState,
                new Dictionary<string, object?>
                {
                    ["reason"] = "Transform prediction uses a forked local RNG snapshot for the local preview card only.",
                    ["source"] = sourceName,
                    ["seed"] = source.Seed,
                    ["counter"] = source.Counter,
                    ["netMode"] = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState)
                });
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
