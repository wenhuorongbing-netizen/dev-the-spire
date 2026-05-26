using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

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

            var storedCards = UrdaBlessingService.GetSeedBankStoredCards(Owner);
            ReleaseEvidenceLog.Log(
                "UrdaSeedBank",
                "relic_hover_count",
                Owner,
                new Dictionary<string, object?>
                {
                    ["stored"] = storedCards.Count
                });
            return storedCards.Count == 0
                ? []
                : [CreateStoredSeedsHoverTip(storedCards)];
        }
    }

    private static IHoverTip CreateStoredSeedsHoverTip(IReadOnlyList<CardModel> storedCards)
    {
        var storedNames = storedCards
            .Take(3)
            .Select(static card => card.Title)
            .Where(static title => !string.IsNullOrWhiteSpace(title))
            .Select(static title => "- " + title)
            .ToList();

        var description = new List<string>
        {
            new LocString("relics", "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionPrefix").GetFormattedText()
        };
        description.AddRange(storedNames);
        description.Add(new LocString("relics", "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionFooter").GetFormattedText());

        return new HoverTip(
            new LocString("relics", "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.title"),
            string.Join("\n", description.Where(static line => !string.IsNullOrWhiteSpace(line))));
    }

    public void RefreshStoredSeedDisplay() => InvokeDisplayAmountChanged();
}
