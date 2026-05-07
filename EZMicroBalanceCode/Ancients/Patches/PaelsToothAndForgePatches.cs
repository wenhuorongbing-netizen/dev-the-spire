namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]
internal static class PaelsToothPickupPatch
{
    [HarmonyPostfix]
    private static void Postfix(PaelsTooth __instance, ref Task __result)
    {
        __result = ResetCounterAfterPickup(__instance, __result);
    }

    private static async Task ResetCounterAfterPickup(PaelsTooth paelsTooth, Task original)
    {
        await original;
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
        MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: stored {paelsTooth.SerializableCards.Count} removed card(s) and reset combat counter.");
    }
}

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]
internal static class PaelsToothCombatPatch
{
    private static readonly System.Reflection.MethodInfo UpdateCardListMethod =
        AccessTools.Method(typeof(PaelsTooth), "UpdateCardList");

    [HarmonyPrefix]
    private static bool Prefix(PaelsTooth __instance, CombatRoom room, ref Task __result)
    {
        __result = AfterCombatEnd(__instance, room);
        return false;
    }

    private static async Task AfterCombatEnd(PaelsTooth paelsTooth, CombatRoom room)
    {
        if (paelsTooth.Owner.Creature.IsDead)
        {
            return;
        }

        if (paelsTooth.SerializableCards.Count == 0)
        {
            AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
            RefreshStoredCardDisplay(paelsTooth);
            return;
        }

        if (room.RoomType == RoomType.Boss)
        {
            ClearStoredCards(paelsTooth, "act boss combat ended");
            return;
        }

        var counter = AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] + 1;
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = counter;
        if (counter < 2)
        {
            MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: counted non-boss combat {counter}/2 before next upgraded return.");
            return;
        }

        var returnedCard = await ChooseAndReturnStoredCard(paelsTooth);
        if (returnedCard != null)
        {
            AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
            MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: returned upgraded stored card {returnedCard.Id.Entry}; {paelsTooth.SerializableCards.Count} stored card(s) remain.");
        }
    }

    private static async Task<CardModel?> ChooseAndReturnStoredCard(PaelsTooth paelsTooth)
    {
        var previews = new List<(SerializableCard Saved, CardModel Card)>();
        foreach (var savedCard in paelsTooth.SerializableCards)
        {
            var card = CardModel.FromSerializable(savedCard);
            if (!paelsTooth.Owner.RunState.ContainsCard(card))
            {
                paelsTooth.Owner.RunState.AddCard(card, paelsTooth.Owner);
            }

            previews.Add((savedCard, card));
        }

        var selected = (await CardSelectCmd.FromChooseABundleScreen(
                paelsTooth.Owner,
                previews.Select(preview => (IReadOnlyList<CardModel>)new[] { preview.Card }).ToList()))
            .FirstOrDefault();

        foreach (var preview in previews.Where(preview => preview.Card != selected))
        {
            if (paelsTooth.Owner.RunState.ContainsCard(preview.Card))
            {
                paelsTooth.Owner.RunState.RemoveCard(preview.Card);
            }
        }

        var selectedPreview = previews.FirstOrDefault(preview => preview.Card == selected);
        if (selected == null || selectedPreview.Saved == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] PaelsTooth skipped: stored-card choice returned no selection.");
            return null;
        }

        paelsTooth.Flash();
        if (selected.IsUpgradable)
        {
            CardCmd.Upgrade(selected, CardPreviewStyle.MessyLayout);
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (!addResult.success)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
            RefreshStoredCardDisplay(paelsTooth);
            MainFile.Logger.Warn($"[EZMicroBalance] PaelsTooth skipped: selected stored card {selected.Id.Entry} could not be returned to the deck.");
            return null;
        }

        CardCmd.PreviewCardPileAdd(addResult);
        paelsTooth.SerializableCards.Remove(selectedPreview.Saved);
        RefreshStoredCardDisplay(paelsTooth);
        return selected;
    }

    public static void ClearStoredCards(PaelsTooth paelsTooth, string reason)
    {
        var remaining = paelsTooth.SerializableCards.Count;
        paelsTooth.SerializableCards.Clear();
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
        RefreshStoredCardDisplay(paelsTooth);
        MainFile.Logger.Info($"[EZMicroBalance] PaelsTooth applied: cleared {remaining} stored card(s) after {reason}.");
    }

    private static void RefreshStoredCardDisplay(PaelsTooth paelsTooth)
    {
        if (UpdateCardListMethod == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] PaelsTooth skipped display refresh: UpdateCardList method was not found.");
            return;
        }

        UpdateCardListMethod.Invoke(paelsTooth, Array.Empty<object>());
    }
}

[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]
internal static class SovereignBladeForgeExhaustPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task<IEnumerable<SovereignBlade>> __result)
    {
        __result = AddExhaustToForgedBlades(player, __result);
    }

    private static async Task<IEnumerable<SovereignBlade>> AddExhaustToForgedBlades(
        Player player,
        Task<IEnumerable<SovereignBlade>> original)
    {
        var blades = (await original).ToList();
        var modifiedCount = 0;
        foreach (var blade in blades.Where(blade =>
                     blade.Owner == player &&
                     blade.CreatedThroughForge &&
                     !blade.Keywords.Contains(CardKeyword.Exhaust)))
        {
            CardCmd.ApplyKeyword(blade, CardKeyword.Exhaust);
            modifiedCount++;
        }

        if (modifiedCount > 0)
        {
            MainFile.Logger.Info($"[EZMicroBalance] SovereignBlade applied: added Exhaust to {modifiedCount} forged temporary blade(s).");
        }

        return blades;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]
internal static class PaelsToothActTransitionPatch
{
    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, ref Task __result)
    {
        if (__instance is not PaelsTooth paelsTooth)
        {
            return;
        }

        __result = ClearStoredCardsAfterOriginal(paelsTooth, __result);
    }

    private static async Task ClearStoredCardsAfterOriginal(PaelsTooth paelsTooth, Task original)
    {
        await original;
        if (paelsTooth.SerializableCards.Count > 0)
        {
            PaelsToothCombatPatch.ClearStoredCards(paelsTooth, "act transition");
        }
    }
}

