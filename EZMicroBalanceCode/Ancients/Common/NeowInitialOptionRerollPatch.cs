using System.Reflection;

using MegaCrit.Sts2.Core.Events;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]
internal static class NeowInitialOptionRerollPatch
{
    private const int ExpectedNeowOptionCount = 3;
    private const string NeowRerollTextKey = $"NEOW.pages.INITIAL.options.{AncientInitialOptionReroll.OptionId}";

    private static readonly MethodInfo GenerateInitialOptionsMethod =
        typeof(Neow).GetMethod("GenerateInitialOptions", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(nameof(Neow), "GenerateInitialOptions");

    private static bool isRegeneratingOptions;

    [HarmonyPostfix]
    private static void AddRerollOption(Neow __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (isRegeneratingOptions ||
            __instance.Owner == null ||
            __instance.Owner.RunState.Modifiers.Count > 0 ||
            !AncientInitialOptionReroll.CanOffer(
                __instance,
                __instance.AllPossibleOptions.Count(),
                ExpectedNeowOptionCount))
        {
            return;
        }

        var options = __result.ToList();
        if (options.Any(option => option.TextKey == NeowRerollTextKey))
        {
            return;
        }

        options.Add(AncientInitialOptionReroll.CreateOption(
            __instance,
            NeowRerollTextKey,
            () => RerollNeowOptions(__instance)));
        __result = options;
    }

    private static Task RerollNeowOptions(Neow neow)
    {
        if (!AncientInitialOptionReroll.TrySpend(neow))
        {
            return Task.CompletedTask;
        }

        var previousChoices = neow.CurrentOptions
            .Where(option => option.TextKey != NeowRerollTextKey)
            .Select(option => option.TextKey)
            .ToHashSet(StringComparer.Ordinal);
        var rerolled = GenerateDifferentOptions(neow, previousChoices);
        AncientInitialOptionReroll.ReplaceGeneratedOptionsAndRefreshScreen(neow, neow.InitialDescription, rerolled);
        SpirePlusFeedback.ConfirmChoiceRefresh();
        MainFile.Logger.Info("[Spire Plus] Neow initial Ancient rewards rerolled once.");
        return Task.CompletedTask;
    }

    private static IReadOnlyList<EventOption> GenerateDifferentOptions(Neow neow, IReadOnlySet<string> previousChoices)
    {
        List<EventOption> latest = [];
        for (var attempt = 0; attempt < 4; attempt++)
        {
            latest = GenerateBaseOptions(neow).ToList();
            if (latest.Count == 0 ||
                !latest.Select(option => option.TextKey).ToHashSet(StringComparer.Ordinal).SetEquals(previousChoices))
            {
                return latest;
            }
        }

        return latest;
    }

    private static IReadOnlyList<EventOption> GenerateBaseOptions(Neow neow)
    {
        isRegeneratingOptions = true;
        try
        {
            return (IReadOnlyList<EventOption>)GenerateInitialOptionsMethod.Invoke(neow, null)!;
        }
        finally
        {
            isRegeneratingOptions = false;
        }
    }
}
