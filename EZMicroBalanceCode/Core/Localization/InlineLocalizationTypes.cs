namespace EZMicroBalance.EZMicroBalanceCode.Core.Localization;

internal sealed class PowerLoc : List<(string, string)>
{
    public PowerLoc(string title, string description, string smartDescription, params (string, string)[] extras)
        : base(MakeEntries(title, description, smartDescription, extras))
    {
    }

    private static IEnumerable<(string, string)> MakeEntries(
        string title,
        string description,
        string smartDescription,
        IEnumerable<(string, string)> extras)
    {
        yield return ("title", title);
        yield return ("description", description);
        yield return ("smartDescription", smartDescription);

        foreach (var extra in extras)
        {
            yield return extra;
        }
    }
}

internal sealed class RelicLoc : List<(string, string)>
{
    public RelicLoc(string title, string description, string flavor, params (string, string)[] extras)
        : base(MakeEntries(title, description, flavor, extras))
    {
    }

    private static IEnumerable<(string, string)> MakeEntries(
        string title,
        string description,
        string flavor,
        IEnumerable<(string, string)> extras)
    {
        yield return ("title", title);
        yield return ("description", description);
        yield return ("flavor", flavor);

        foreach (var extra in extras)
        {
            yield return extra;
        }
    }
}

internal sealed class CardModifierLoc : List<(string, string)>
{
    public CardModifierLoc(string title, string description, string extraCardText, params (string, string)[] extras)
        : base(MakeEntries(title, description, extraCardText, extras))
    {
    }

    private static IEnumerable<(string, string)> MakeEntries(
        string title,
        string description,
        string extraCardText,
        IEnumerable<(string, string)> extras)
    {
        yield return ("title", title);
        yield return ("description", description);
        yield return ("extraCardText", extraCardText);

        foreach (var extra in extras)
        {
            yield return extra;
        }
    }
}
