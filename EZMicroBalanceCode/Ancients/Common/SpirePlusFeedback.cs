using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SpirePlusFeedback
{
    private const float DefaultPreviewSeconds = 2f;
    private const string DeckAddSfx = "event:/sfx/ui/cards/card_movement_B_into_deck";
    private const string RelicTriggerSfx = "event:/sfx/ui/relic_activate_general";

    public static void PreviewDeckAdds(CardPileAddResult result, float seconds = DefaultPreviewSeconds)
    {
        PreviewDeckAdds([result], sourceRelic: null, seconds);
    }

    public static void PreviewDeckAdds(CardPileAddResult result, RelicModel? sourceRelic, float seconds = DefaultPreviewSeconds)
    {
        PreviewDeckAdds([result], sourceRelic, seconds);
    }

    public static void PreviewDeckAdds(IEnumerable<CardPileAddResult> results, float seconds = DefaultPreviewSeconds)
    {
        PreviewDeckAdds(results, sourceRelic: null, seconds);
    }

    public static void PreviewDeckAdds(IEnumerable<CardPileAddResult> results, RelicModel? sourceRelic, float seconds = DefaultPreviewSeconds)
    {
        var successfulAdds = results.Where(result => result.success).ToList();
        if (successfulAdds.Count == 0)
        {
            return;
        }

        FlashSourceRelic(sourceRelic);
        successfulAdds = AttachSourceRelic(successfulAdds, sourceRelic);
        CardCmd.PreviewCardPileAdd(successfulAdds, seconds);
        PlayDeckGainCue();
    }

    public static void ConfirmChoiceRefresh()
    {
        SfxCmd.Play(RelicTriggerSfx);
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
    }

    public static void ConfirmRelicPayoff(RelicModel? sourceRelic)
    {
        FlashSourceRelic(sourceRelic);
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
    }

    private static List<CardPileAddResult> AttachSourceRelic(
        IReadOnlyList<CardPileAddResult> results,
        RelicModel? sourceRelic)
    {
        if (sourceRelic == null)
        {
            return results.ToList();
        }

        var attached = new List<CardPileAddResult>(results.Count);
        foreach (var result in results)
        {
            var copy = result;
            var models = copy.modifyingModels == null
                ? new List<AbstractModel>()
                : new List<AbstractModel>(copy.modifyingModels);
            if (!models.Contains(sourceRelic))
            {
                models.Insert(0, sourceRelic);
            }

            copy.modifyingModels = models;
            attached.Add(copy);
        }

        return attached;
    }

    private static void FlashSourceRelic(RelicModel? sourceRelic)
    {
        if (sourceRelic == null)
        {
            return;
        }

        sourceRelic.Flash();
        SfxCmd.Play(RelicTriggerSfx);
        var flashVfx = NRelicFlashVfx.Create(sourceRelic);
        if (flashVfx != null)
        {
            NRun.Instance?.GlobalUi.AboveTopBarVfxContainer.AddChildSafely(flashVfx);
        }
    }

    private static void PlayDeckGainCue()
    {
        SfxCmd.Play(DeckAddSfx);
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
    }
}
