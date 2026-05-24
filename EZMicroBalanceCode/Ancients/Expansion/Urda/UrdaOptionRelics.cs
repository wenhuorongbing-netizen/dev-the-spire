namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

internal abstract class UrdaOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaSeedbedOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.SeedbedOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaHumusPactOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.HumusPactOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaMoltingOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.MoltingOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaMossMapOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.MossMapOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaTrialBranchOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.TrialBranchOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaShallowRootRelicOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.ShallowRootRelicOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaEliteRootOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.EliteRootOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaRootedRouteOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootedRouteOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaAfterRainOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.AfterRainOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaRootSightOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootSightOptionIcon;

    public override bool IsUsedUp =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.GetRootSightEyes(Owner) <= 0;

    public override bool ShowCounter =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.GetRootSightEyes(Owner) > 0;

    public override int DisplayAmount =>
        IsMutable && Owner != null
            ? UrdaBlessingService.GetRootSightEyes(Owner)
            : 0;

    public void RefreshRootSightDisplay() => InvokeDisplayAmountChanged();
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaSeedBankOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.SeedBankOptionIcon;

    public override bool IsUsedUp =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.IsSeedBankSettled(Owner);

    public override bool ShowCounter =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.GetSeedBankStoredCount(Owner) > 0 &&
        !UrdaBlessingService.IsSeedBankSettled(Owner);

    public override int DisplayAmount =>
        IsMutable && Owner != null
            ? UrdaBlessingService.GetSeedBankStoredCount(Owner)
            : 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (!IsMutable || Owner == null)
            {
                return [];
            }

            ReleaseEvidenceLog.Log(
                "UrdaSeedBank",
                "relic_hover_count",
                Owner,
                new Dictionary<string, object?>
                {
                    ["stored"] = UrdaBlessingService.GetSeedBankStoredCount(Owner)
                });
            return UrdaBlessingService
                .GetSeedBankStoredCards(Owner)
                .SelectMany(card => new[] { HoverTipFactory.FromCard(card) }.Concat(card.HoverTips))
                .ToArray();
        }
    }

    public void RefreshStoredSeedDisplay() => InvokeDisplayAmountChanged();
}

[HarmonyPatch(typeof(NRelicInventory), "OnRelicClicked")]
internal static class UrdaSeedBankRelicClickPatch
{
    [HarmonyPrefix]
    private static bool ExtractStoredSeedInsteadOfInspecting(RelicModel model)
    {
        if (model is UrdaRootSightOptionRelic rootSight && rootSight.Owner != null)
        {
            return !UrdaBlessingService.TryBeginRootSightSelection(rootSight.Owner);
        }

        if (model is not UrdaSeedBankOptionRelic seedBank ||
            seedBank.Owner == null ||
            seedBank.IsUsedUp ||
            UrdaBlessingService.GetSeedBankStoredCount(seedBank.Owner) == 0)
        {
            return true;
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
            seedBank.Owner.RunState,
            "UrdaSeedBank",
            "Seed Bank relic extraction opens unsynced shared reward selection"))
        {
            return true;
        }

        _ = TaskHelper.RunSafely(UrdaBlessingService.TryExtractSeedBankFromRelicClick(seedBank.Owner));
        return false;
    }
}
