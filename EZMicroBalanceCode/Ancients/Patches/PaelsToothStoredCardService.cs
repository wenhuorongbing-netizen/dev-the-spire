namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class PaelsToothStoredCardService
{
    private static readonly System.Reflection.MethodInfo UpdateCardListMethod =
        AccessTools.Method(typeof(PaelsTooth), "UpdateCardList");

    public static async Task ResetCounterAfterPickup(PaelsTooth paelsTooth, Task original)
    {
        await original;
        AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
        MainFile.Logger.Info($"[Spire Plus] PaelsTooth applied: stored {paelsTooth.SerializableCards.Count} removed card(s) and reset combat counter.");
    }

    public static async Task AfterCombatEnd(PaelsTooth paelsTooth, CombatRoom room)
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
            MainFile.Logger.Info($"[Spire Plus] PaelsTooth applied: counted non-boss combat {counter}/2 before next upgraded return.");
            return;
        }

        var returnedCard = await ChooseAndReturnStoredCard(paelsTooth);
        if (returnedCard != null)
        {
            AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0;
            MainFile.Logger.Info($"[Spire Plus] PaelsTooth applied: returned upgraded stored card {returnedCard.Id.Entry}; {paelsTooth.SerializableCards.Count} stored card(s) remain.");
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
            MainFile.Logger.Warn("[Spire Plus] PaelsTooth skipped: stored-card choice returned no selection.");
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
            MainFile.Logger.Warn($"[Spire Plus] PaelsTooth skipped: selected stored card {selected.Id.Entry} could not be returned to the deck.");
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
        MainFile.Logger.Info($"[Spire Plus] PaelsTooth applied: cleared {remaining} stored card(s) after {reason}.");
    }

    private static void RefreshStoredCardDisplay(PaelsTooth paelsTooth)
    {
        if (UpdateCardListMethod == null)
        {
            MainFile.Logger.Warn("[Spire Plus] PaelsTooth skipped display refresh: UpdateCardList method was not found.");
            return;
        }

        UpdateCardListMethod.Invoke(paelsTooth, Array.Empty<object>());
    }
}
