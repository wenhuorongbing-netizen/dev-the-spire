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
                MarkRootBeginsApplied(player);
                var addedStartingRoot = false;
                if (FindRootFamilyCards(player).Count == 0)
                {
                    addedStartingRoot = await AddRootblightCard(player, 1);
                }

                SetDiagnosticLevelFromDeck(player);
                if (addedStartingRoot)
                {
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
                MainFile.Logger.Info(
                    addedStartingRoot
                        ? $"[EZMicroBalance] Ascension A14 applied: Rootblight I added for player {runState.GetPlayerSlotIndex(player)}."
                        : $"[EZMicroBalance] Ascension A14 applied: starting Rootblight already present for player {runState.GetPlayerSlotIndex(player)}; no duplicate added.");
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

        MarkRootBeginsApplied(player);
        await TrimRootblightDeckToCap(player, $"{source} add");
        if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))
        {
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
                $"[EZMicroBalance] Ascension Rootblight capped: skipped Rootblight I from {source} because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
            return;
        }

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
            $"[EZMicroBalance] Ascension Rootblight applied: added Rootblight I from {source} for player {player.RunState.GetPlayerSlotIndex(player)}.");
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
                    $"[EZMicroBalance] Ascension Rootblight held by Seedbed: skipped level {card.RootblightLevel} growth for player {player.RunState.GetPlayerSlotIndex(player)}.");
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
                            $"[EZMicroBalance] Ascension Rootblight capped: skipped Rootblight I from ignored Rootblight III because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                    }
                    else
                    {
                        card.HasSplit = true;
                        MainFile.Logger.Info(
                            $"[EZMicroBalance] Ascension Rootblight applied: ignored Rootblight III split once and added Rootblight I for player {player.RunState.GetPlayerSlotIndex(player)}.");
                    }
                }
                else
                {
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension Rootblight applied: ignored Rootblight III already split once; no Rootblight IV for player {player.RunState.GetPlayerSlotIndex(player)}.");
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
                        $"[EZMicroBalance] Ascension Rootblight capped: skipped queued level {cardToAdd.Level} downgrade because player {player.RunState.GetPlayerSlotIndex(player)} already has {MaxRootblightCards} Rootblight cards.");
                }
            }

            ClearPendingCombatDowngrades(player);
        }

        SetDiagnosticLevelFromDeck(player);
        ReleaseEvidenceLog.Log("Rootblight", "combat_end_notice_queued", player);
    }
}
