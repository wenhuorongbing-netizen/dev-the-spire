namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class RootDeckService
{
    private const char PendingDowngradeSeparator = ';';
    private const char PendingDowngradePartSeparator = ':';

    private static void QueuePendingCombatDowngrade(Player player, int level, bool hasSplit)
    {
        var pending = ReadPendingCombatDowngrades(player);
        pending.Add(new RootblightCardToAdd(level, hasSplit));
        WritePendingCombatDowngrades(player, pending);
    }

    private static List<RootblightCardToAdd> ReadPendingCombatDowngrades(Player player)
    {
        var serialized = AscensionSavedStateFields.RootblightPendingCombatDowngrades[player];
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return [];
        }

        var pending = new List<RootblightCardToAdd>();
        foreach (var entry in serialized.Split(PendingDowngradeSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = entry.Split(PendingDowngradePartSeparator, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var level))
            {
                continue;
            }

            level = Math.Clamp(level, 1, MaxRootblightLevel);
            pending.Add(new RootblightCardToAdd(level, parts[1] == "1"));
        }

        return pending;
    }

    private static void WritePendingCombatDowngrades(Player player, IReadOnlyList<RootblightCardToAdd> pending)
    {
        AscensionSavedStateFields.RootblightPendingCombatDowngrades[player] = pending.Count == 0
            ? string.Empty
            : string.Join(
                PendingDowngradeSeparator,
                pending.Select(card => $"{Math.Clamp(card.Level, 1, MaxRootblightLevel)}{PendingDowngradePartSeparator}{(card.HasSplit ? 1 : 0)}"));
    }

    private static void ClearPendingCombatDowngrades(Player player)
    {
        AscensionSavedStateFields.RootblightPendingCombatDowngrades[player] = string.Empty;
    }

    private readonly record struct RootblightCardToAdd(int Level, bool HasSplit);
}
