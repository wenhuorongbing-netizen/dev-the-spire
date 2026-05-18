namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static string GetPart(string[] parts, int index) =>
        index >= 0 && index < parts.Length ? parts[index] : string.Empty;

    private static string SanitizeStateField(string value) =>
        (value ?? string.Empty).Replace(ProgressSeparator, '_');

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;

    private static IEnumerable<string> SplitList(string value, char separator) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
