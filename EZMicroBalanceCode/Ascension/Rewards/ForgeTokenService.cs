namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class ForgeTokenService
{
    private const decimal SmithHealAmount = 7m;
    private const decimal NoUpgradeFallbackHealAmount = 5m;
    private const decimal DuplicateTokenGoldAmount = 15m;

    public static async Task GrantAfterFiremarkedElite(CombatState combatState)
    {
        if (!AscensionFeatureGate.IsForgeTokenEnabled(combatState.RunState))
        {
            return;
        }

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            if (HasToken(player))
            {
                await PlayerCmd.GainGold(DuplicateTokenGoldAmount, player);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A12 applied: duplicate Forge Token converted to {DuplicateTokenGoldAmount} gold for player {combatState.RunState.GetPlayerSlotIndex(player)}.");
                continue;
            }

            AscensionSavedStateFields.ForgeTokenHeld[player] = true;
            await EnsureTokenRelic(player);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 applied: Forge Token granted to player {combatState.RunState.GetPlayerSlotIndex(player)}.");
        }
    }

    public static async Task SyncVisibleTokens(IRunState runState)
    {
        if (!AscensionFeatureGate.IsForgeTokenEnabled(runState))
        {
            foreach (var player in runState.Players)
            {
                await RemoveTokenRelics(player);
            }

            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            if (AscensionSavedStateFields.ForgeTokenHeld[player])
            {
                await EnsureTokenRelic(player);
            }
            else if (player.GetRelic<ForgeTokenRelic>() != null)
            {
                AscensionSavedStateFields.ForgeTokenHeld[player] = true;
            }
        }
    }

    public static async Task ApplyAfterRestSiteHeal(Player player)
    {
        if (!CanSpendToken(player))
        {
            return;
        }

        MarkTokenActive(player);
        AscensionSavedStateFields.ForgeTokenHeld[player] = false;
        var upgradeTarget = FindHealRestUpgradeTarget(player);
        if (upgradeTarget != null)
        {
            CardCmd.Upgrade(upgradeTarget, CardPreviewStyle.HorizontalLayout);
            await RemoveTokenRelics(player);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 applied: Forge Token upgraded {upgradeTarget.Id.Entry} after rest heal.");
            return;
        }

        await CreatureCmd.Heal(player.Creature, NoUpgradeFallbackHealAmount);
        await RemoveTokenRelics(player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A12 applied: Forge Token fallback healed {NoUpgradeFallbackHealAmount} after rest heal because no common/uncommon upgrade target existed.");
    }

    public static async Task ApplyAfterRestSiteSmith(Player player)
    {
        if (!CanSpendToken(player))
        {
            return;
        }

        MarkTokenActive(player);
        AscensionSavedStateFields.ForgeTokenHeld[player] = false;
        await CreatureCmd.Heal(player.Creature, SmithHealAmount);
        await RemoveTokenRelics(player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A12 applied: Forge Token healed {SmithHealAmount} after smith.");
    }

    public static IReadOnlyList<LocString> ModifyExtraRestSiteHealText(
        Player player,
        IReadOnlyList<LocString> currentExtraText)
    {
        if (!CanSpendToken(player))
        {
            return currentExtraText;
        }

        var tokenText = ModelDb.Relic<ForgeTokenRelic>().GetAdditionalRestSiteHealText();
        if (tokenText == null)
        {
            return currentExtraText;
        }

        return currentExtraText.Concat(new[] { tokenText }).ToList();
    }

    private static bool CanSpendToken(Player player)
    {
        return AscensionFeatureGate.IsForgeTokenEnabled(player.RunState) &&
            HasToken(player);
    }

    private static bool HasToken(Player player)
    {
        return AscensionSavedStateFields.ForgeTokenHeld[player] ||
            player.GetRelic<ForgeTokenRelic>() != null;
    }

    private static async Task EnsureTokenRelic(Player player)
    {
        var tokens = player.Relics.OfType<ForgeTokenRelic>().ToList();
        if (tokens.Count > 0)
        {
            foreach (var duplicate in tokens.Skip(1))
            {
                await RelicCmd.Remove(duplicate);
            }

            return;
        }

        await RelicCmd.Obtain<ForgeTokenRelic>(player);
    }

    private static async Task RemoveTokenRelics(Player player)
    {
        foreach (var token in player.Relics.OfType<ForgeTokenRelic>().ToList())
        {
            await RelicCmd.Remove(token);
        }
    }

    private static void MarkTokenActive(Player player)
    {
        var token = player.GetRelic<ForgeTokenRelic>();
        if (token != null)
        {
            token.Status = RelicStatus.Active;
        }
    }

    private static CardModel? FindHealRestUpgradeTarget(Player player)
    {
        var targets = player.Deck.Cards
            .Where(card => card.IsUpgradable)
            .Where(card => card.Rarity is CardRarity.Common or CardRarity.Uncommon)
            .ToList();

        return targets.Count == 0
            ? null
            : player.RunState.Rng.Niche.NextItem(targets);
    }

}
