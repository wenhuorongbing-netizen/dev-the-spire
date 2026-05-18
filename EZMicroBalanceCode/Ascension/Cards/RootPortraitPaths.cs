using Godot;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class RootPortraitPaths
{
    private const string GenericPortrait = $"{MainFile.ResPath}/images/card_portraits/card.png";
    private const string GenericBigPortrait = $"{MainFile.ResPath}/images/card_portraits/big/card.png";

    public static string BlightSprout => OptionalPortrait("blight_sprout.png", GenericPortrait);

    public static string BigBlightSprout => OptionalPortrait("big/blight_sprout.png", GenericBigPortrait);

    public static string Rootblight(int level) =>
        OptionalPortrait($"{RootblightFileName(level)}.png", GenericPortrait);

    public static string BigRootblight(int level) =>
        OptionalPortrait($"big/{RootblightFileName(level)}.png", GenericBigPortrait);

    private static string RootblightFileName(int level) => level switch
    {
        1 => "rootblight_i",
        2 => "rootblight_ii",
        3 => "rootblight_iii",
        _ => "rootblight_i",
    };

    private static string OptionalPortrait(string relativePath, string fallback)
    {
        var candidate = $"{MainFile.ResPath}/images/card_portraits/{relativePath}";

        try
        {
            return ResourceLoader.Exists(candidate) ? candidate : fallback;
        }
        catch
        {
            return fallback;
        }
    }
}
