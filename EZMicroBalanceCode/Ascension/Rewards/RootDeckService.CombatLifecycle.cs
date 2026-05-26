namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class RootDeckService
{
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
