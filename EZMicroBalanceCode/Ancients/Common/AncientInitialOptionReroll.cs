using System.Reflection;

using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class AncientInitialOptionReroll
{
    public const string OptionId = "ezmb_reroll_initial_options";

    private const char KeySeparator = '|';

    private static readonly FieldInfo GeneratedOptionsField =
        typeof(AncientEventModel).GetField("_generatedOptions", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingFieldException(nameof(AncientEventModel), "_generatedOptions");

    public static bool CanOffer(AncientEventModel ancient, int availableOptionCount, int visibleOptionCount) =>
        ancient.Owner != null &&
        availableOptionCount > visibleOptionCount &&
        !HasSpent(ancient);

    public static EventOption CreateOption(AncientEventModel ancient, string textKey, Func<Task> onChosen)
    {
        var relic = ModelDb.Relic<AncientInitialRerollOptionRelic>().ToMutable();
        if (ancient.Owner != null)
        {
            relic.Owner = ancient.Owner;
        }

        return EventOption
            .FromRelic(relic, ancient, onChosen, textKey)
            .ThatWontSaveToChoiceHistory();
    }

    public static bool TrySpend(AncientEventModel ancient)
    {
        if (ancient.Owner == null || HasSpent(ancient))
        {
            return false;
        }

        var spentKeys = ReadSpentKeys(ancient.Owner);
        spentKeys.Add(BuildEventKey(ancient));
        AncientSavedStateFields.AncientInitialOptionRerollStateKey[ancient.Owner] =
            string.Join(KeySeparator, spentKeys.OrderBy(key => key, StringComparer.Ordinal));
        return true;
    }

    public static void ReplaceGeneratedOptionsForHistory(AncientEventModel ancient, IReadOnlyList<EventOption> options) =>
        GeneratedOptionsField.SetValue(ancient, options.ToList());

    private static bool HasSpent(AncientEventModel ancient)
    {
        if (ancient.Owner == null)
        {
            return true;
        }

        return ReadSpentKeys(ancient.Owner).Contains(BuildEventKey(ancient));
    }

    private static HashSet<string> ReadSpentKeys(Player owner)
    {
        var raw = AncientSavedStateFields.AncientInitialOptionRerollStateKey[owner] ?? string.Empty;
        return raw.Split(KeySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string BuildEventKey(AncientEventModel ancient)
    {
        var runState = ancient.Owner?.RunState;
        var coord = runState?.CurrentMapCoord;
        var coordText = coord.HasValue
            ? $"{coord.Value.col},{coord.Value.row}"
            : "none";
        var actIndex = runState?.CurrentActIndex ?? -1;
        var totalFloor = runState?.TotalFloor ?? -1;
        return $"{ancient.Id.Entry}:{actIndex}:{coordText}:{totalFloor}";
    }
}

internal static class AncientRerollAssetPaths
{
    public static string OptionIcon => $"{MainFile.ResPath}/images/ancients/common/ancient_reroll_die.png";
}

[Pool(typeof(SharedRelicPool))]
internal sealed class AncientInitialRerollOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    public override string PackedIconPath => AncientRerollAssetPaths.OptionIcon;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}
