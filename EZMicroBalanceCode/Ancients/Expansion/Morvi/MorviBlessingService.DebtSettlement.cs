using MegaCrit.Sts2.Core.Entities.Gold;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int DebtSettlementImmediateGold = 220;
    private const int DebtSettlementStartingDebt = 320;
    private const int DebtSettlementCombatDue = 40;
    private const int DebtSettlementHpPerTenShortfall = 3;

    private static async Task ResolveDebtSettlementPickup(Player player)
    {
        await PlayerCmd.GainGold(DebtSettlementImmediateGold, player);

        var removalPrefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.removeSelectionPrompt"),
            0,
            2);
        var removals = (await CardSelectCmd.FromDeckForRemoval(player, removalPrefs)).ToList();
        if (removals.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(removals);
        }

        var upgradePrefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.upgradeSelectionPrompt"),
            0,
            2);
        var upgrades = (await CardSelectCmd.FromDeckForUpgrade(player, upgradePrefs)).ToList();
        if (upgrades.Count > 0)
        {
            CardCmd.Upgrade(upgrades, CardPreviewStyle.EventLayout);
        }

        SetProgress(player, new Progress(DebtSettlementStartingDebt, string.Empty, false));
        ReleaseEvidenceLog.Log(
            "MorviState",
            "debt_created",
            player,
            new Dictionary<string, object?>
            {
                ["debt"] = DebtSettlementStartingDebt
            });
        MainFile.Logger.Info("[Spire Plus] Morvi Debt Settlement granted 220 Gold, resolved optional removal/upgrade selections, and set Debt to 320.");
    }

    private static async Task PayDebtSettlementDue(Player player)
    {
        var progress = GetProgress(player);
        if (progress.DebtRemaining <= 0)
        {
            return;
        }

        var due = Math.Min(DebtSettlementCombatDue, progress.DebtRemaining);
        var goldPaid = Math.Min(player.Gold, due);
        if (goldPaid > 0)
        {
            await PlayerCmd.LoseGold(goldPaid, player, GoldLossType.Spent);
        }

        var shortfall = due - goldPaid;
        if (shortfall > 0)
        {
            var calculatedHpLoss = (int)Math.Ceiling(shortfall / 10m) * DebtSettlementHpPerTenShortfall;
            await DamagePlayerNonlethal(player, calculatedHpLoss);
            ReleaseEvidenceLog.Log(
                "MorviState",
                "debt_unpaid_fallback",
                player,
                new Dictionary<string, object?>
                {
                    ["shortfall"] = shortfall,
                    ["hpLoss"] = calculatedHpLoss
                });
        }

        var nextProgress = progress with { DebtRemaining = Math.Max(0, progress.DebtRemaining - due) };
        SetProgress(player, nextProgress);
        await SetCounterPower<MorviDebtPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            nextProgress.DebtRemaining);
        ReleaseEvidenceLog.Log(
            "MorviState",
            "debt_paid",
            player,
            new Dictionary<string, object?>
            {
                ["due"] = due,
                ["goldPaid"] = goldPaid,
                ["remaining"] = nextProgress.DebtRemaining
            });
        MainFile.Logger.Info($"[Spire Plus] Morvi Debt Settlement paid due={due}; debt remaining={nextProgress.DebtRemaining}.");
    }
}
