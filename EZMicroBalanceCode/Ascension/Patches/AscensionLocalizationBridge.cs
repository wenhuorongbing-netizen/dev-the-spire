namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using System.Reflection;
using System.Text.Json;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

internal static class AscensionLocalizationBridge
{
    private static readonly FieldInfo? TableNameField = AccessTools.Field(typeof(LocTable), "_name");
    private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> CachedTables = new(StringComparer.Ordinal);
    private static readonly IReadOnlyDictionary<string, string> EmptyTable = new Dictionary<string, string>();

    public static bool TryGetText(LocTable table, string key, out string text)
    {
        text = string.Empty;

        if (!IsAscensionTable(table))
        {
            return false;
        }

        var language = LocManager.Instance?.Language;
        if (!string.IsNullOrWhiteSpace(language) &&
            TryGetTextForLanguage(language, key, out text))
        {
            return true;
        }

        return TryGetTextForLanguage("eng", key, out text);
    }

    public static bool TryGetText(string key, out string text)
    {
        text = string.Empty;

        var language = LocManager.Instance?.Language;
        if (!string.IsNullOrWhiteSpace(language) &&
            TryGetTextForLanguage(language, key, out text))
        {
            return true;
        }

        return TryGetTextForLanguage("eng", key, out text);
    }

    public static bool IsAscensionLevelKey(string key)
    {
        if (!key.StartsWith("LEVEL_", StringComparison.Ordinal))
        {
            return false;
        }

        return key.EndsWith(".title", StringComparison.Ordinal) ||
            key.EndsWith(".description", StringComparison.Ordinal);
    }

    public static void MergeIntoIfAscensionTable(LocTable table)
    {
        if (!IsAscensionTable(table))
        {
            return;
        }

        MergeLanguageInto(table, "eng");

        var language = LocManager.Instance?.Language;
        if (!string.IsNullOrWhiteSpace(language) &&
            !language.Equals("eng", StringComparison.Ordinal))
        {
            MergeLanguageInto(table, language);
        }
    }

    private static bool IsAscensionTable(LocTable table)
    {
        return TableNameField?.GetValue(table) is string tableName &&
            tableName.Equals("ascension", StringComparison.Ordinal);
    }

    private static bool TryGetTextForLanguage(string language, string key, out string text)
    {
        var table = GetTable(language);
        return table.TryGetValue(key, out text!);
    }

    private static IReadOnlyDictionary<string, string> GetTable(string language)
    {
        if (CachedTables.TryGetValue(language, out var table))
        {
            return table;
        }

        table = LoadTable(language);
        CachedTables[language] = table;
        return table;
    }

    private static IReadOnlyDictionary<string, string> LoadTable(string language)
    {
        var path = $"{MainFile.ResPath}/localization/{language}/ascension.json";

        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            MainFile.Logger.Warn($"[Spire Plus] Ascension localization bridge could not open {path}.");
            return EmptyTable;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(file.GetAsText()) ?? EmptyTable;
        }
        catch (Exception exception)
        {
            MainFile.Logger.Warn($"[Spire Plus] Ascension localization bridge could not parse {path}: {exception.Message}");
            return EmptyTable;
        }
    }

    private static void MergeLanguageInto(LocTable table, string language)
    {
        var localizedTable = GetTable(language);
        if (localizedTable.Count == 0)
        {
            return;
        }

        // Core loads base localization before mods are fully initialized on some
        // early UI paths. Merge the current Spire Plus ascension slice into the
        // already-live original table so character select never displays raw keys.
        table.MergeWith(new Dictionary<string, string>(localizedTable, StringComparer.Ordinal));
    }
}
