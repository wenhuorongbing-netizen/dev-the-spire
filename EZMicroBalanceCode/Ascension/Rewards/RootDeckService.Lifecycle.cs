namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class RootDeckService
{
    public static async Task EnsureStartingRoot(RunState runState)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(runState))
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            await TrimRootblightDeckToCap(player, "run-state sync");
            if (!HasRootBeginsApplied(player))
            {
                var addedStartingRoot = false;
                if (FindRootFamilyCards(player).Count == 0)
                {
                    addedStartingRoot = await AddRootblightCard(player, 1);
                }

                SetDiagnosticLevelFromDeck(player);
                if (addedStartingRoot)
                {
                    MarkRootBeginsApplied(player);
                    ReleaseEvidenceLog.Log(
                        "Rootblight",
                        "rootblight_added",
                        player,
                        new Dictionary<string, object?>
                        {
                            ["level"] = 1,
                            ["source"] = "Root Begins"
                        });
                }
                else if (FindRootFamilyCards(player).Count > 0)
                {
                    MarkRootBeginsApplied(player);
                }
                else
                {
                    MainFile.Logger.Warn(
                        $"[Spire Plus] Ascension A14 delayed: Rootblight I could not be added for player {runState.GetPlayerSlotIndex(player)}; the next room/act hook will retry.");
                    continue;
                }

                MainFile.Logger.Info(
                    addedStartingRoot
                        ? $"[Spire Plus] Ascension A14 applied: Rootblight I added for player {runState.GetPlayerSlotIndex(player)}."
                        : $"[Spire Plus] Ascension A14 applied: starting Rootblight already present for player {runState.GetPlayerSlotIndex(player)}; no duplicate added.");
                continue;
            }

            SetDiagnosticLevelFromDeck(player);
        }
    }

    public static async Task AddRootblightI(Player player, string source)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        await TrimRootblightDeckToCap(player, $"{source} add");
        var hadRootblightBeforeAdd = FindRootFamilyCards(player).Count > 0;
        if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))
        {
            if (hadRootblightBeforeAdd || FindRootFamilyCards(player).Count > 0)
            {
                MarkRootBeginsApplied(player);
            }

            ShowRootSystemFull(player);
            ReleaseEvidenceLog.Log(
                "Rootblight",
                "deck_cap_enforced",
                player,
                new Dictionary<string, object?>
                {
                    ["source"] = source,
                    ["cap"] = MaxRootblightCards
                });
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension Rootblight capped: skipped Rootblight I from {source} because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
            return;
        }

        MarkRootBeginsApplied(player);
        SetDiagnosticLevelFromDeck(player);

        ReleaseEvidenceLog.Log(
            "Rootblight",
            "rootblight_added",
            player,
            new Dictionary<string, object?>
            {
                ["level"] = 1,
                ["source"] = source
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight applied: added Rootblight I from {source} for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static void MarkCombatStartRootblight(Player player)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var marked = 0;
        foreach (var card in FindRootFamilyCards(player))
        {
            card.WasPresentAtCombatStart = true;
            marked++;
        }

        ReleaseEvidenceLog.Log(
            "Rootblight",
            "combat_start_marked",
            player,
            new Dictionary<string, object?>
            {
                ["cards"] = marked
            });
    }

    public static async Task ResolveCombatEndRootblight(Player player)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        await TrimRootblightDeckToCap(player, "combat-end sync");
        var existingRootblight = FindRootFamilyCards(player)
            .Where(card => card.WasPresentAtCombatStart)
            .ToList();

        foreach (var card in existingRootblight)
        {
            card.WasPresentAtCombatStart = false;
            if (card.PlantedInSeedbed)
            {
                card.PlantedInSeedbed = false;
                ReleaseEvidenceLog.Log(
                    "Rootblight",
                    "sprout_buried",
                    player,
                    new Dictionary<string, object?>
                    {
                        ["level"] = card.RootblightLevel
                    });
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension Rootblight held by Seedbed: skipped level {card.RootblightLevel} growth for player {player.RunState.GetPlayerSlotIndex(player)}.");
                continue;
            }

            if (card.RootblightLevel >= MaxRootblightLevel)
            {
                if (!card.HasSplit)
                {
                    if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))
                    {
                        ShowRootSystemFull(player);
                        MainFile.Logger.Info(
                            $"[Spire Plus] Ascension Rootblight capped: skipped Rootblight I from ignored Rootblight III because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                    }
                    else
                    {
                        card.HasSplit = true;
                        MainFile.Logger.Info(
                            $"[Spire Plus] Ascension Rootblight applied: ignored Rootblight III split once and added Rootblight I for player {player.RunState.GetPlayerSlotIndex(player)}.");
                    }
                }
                else
                {
                    MainFile.Logger.Info(
                        $"[Spire Plus] Ascension Rootblight applied: ignored Rootblight III already split once; no Rootblight IV for player {player.RunState.GetPlayerSlotIndex(player)}.");
                }

                continue;
            }

            await ReplaceRootblightCard(player, card, card.RootblightLevel + 1, card.HasSplit);
        }

        var pendingDowngrades = ReadPendingCombatDowngrades(player);
        if (pendingDowngrades.Count > 0)
        {
            foreach (var cardToAdd in pendingDowngrades)
            {
                if (!await AddRootblightCard(player, cardToAdd.Level, cardToAdd.HasSplit, preferOverlayNotice: true))
                {
                    ShowRootSystemFull(player);
                    MainFile.Logger.Info(
                        $"[Spire Plus] Ascension Rootblight capped: skipped queued level {cardToAdd.Level} downgrade because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                }
            }

            ClearPendingCombatDowngrades(player);
        }

        SetDiagnosticLevelFromDeck(player);
        ReleaseEvidenceLog.Log("Rootblight", "combat_end_notice_queued", player);
    }
}
