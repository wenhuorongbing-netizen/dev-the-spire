using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static class LothaRewardSelectionService
{
    public static async Task SelectBlessing<T>(Player owner, string blessingId)
        where T : RelicModel
    {
        LothaBlessingService.SetSelectedBlessing(owner, blessingId);
        await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(owner, blessingId);
        if (blessingId == LothaBlessingIds.MirrorRebuttal)
        {
            await SelectMirrorRebuttalCard(owner);
        }
    }

    private static async Task SelectMirrorRebuttalCard(Player owner)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt"),
            1)
        {
            RequireManualConfirmation = true
        };

        var selected = (await CardSelectCmd.FromDeckGeneric(
                owner,
                prefs,
                LothaBlessingService.IsMirrorRebuttalDeckCardCandidate))
            .FirstOrDefault();
        if (selected == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Lotha Mirror Rebuttal selection skipped: no eligible non-Curse, non-Status deck card.");
            return;
        }

        LothaBlessingService.MarkMirrorRebuttalCard(owner, selected);
    }
}
