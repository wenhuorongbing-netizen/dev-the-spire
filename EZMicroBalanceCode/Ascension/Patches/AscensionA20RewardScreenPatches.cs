using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionA20RewardScreenReflection
{
    public static readonly System.Reflection.FieldInfo? RunStateField =
        AccessTools.Field(typeof(NRewardsScreen), "_runState");

    public static readonly System.Reflection.FieldInfo? HeaderLabelField =
        AccessTools.Field(typeof(NRewardsScreen), "_headerLabel");

    public static readonly System.Reflection.FieldInfo? ProceedButtonField =
        AccessTools.Field(typeof(NRewardsScreen), "_proceedButton");

    private static bool _warned;

    public static bool TryGetFieldValue<T>(
        System.Reflection.FieldInfo? field,
        NRewardsScreen screen,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
        where T : class
    {
        value = null;
        if (field == null)
        {
            WarnOnce("[Spire Plus] Ascension A20 reward-screen wording disabled: expected NRewardsScreen field was not found.");
            return false;
        }

        try
        {
            value = field.GetValue(screen) as T;
            return value != null;
        }
        catch (Exception exception)
        {
            WarnOnce($"[Spire Plus] Ascension A20 reward-screen wording disabled after reflection failure: {exception.GetType().Name}.");
            return false;
        }
    }

    private static void WarnOnce(string message)
    {
        if (_warned)
        {
            return;
        }

        _warned = true;
        MainFile.Logger.Warn(message);
    }
}

[HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))]
internal static class AscensionA20RewardScreenReadyPatch
{
    [HarmonyPostfix]
    private static void Postfix(NRewardsScreen __instance)
    {
        if (!IsA20BossOneIntermission(__instance))
        {
            return;
        }

        if (AscensionA20RewardScreenReflection.TryGetFieldValue<MegaLabel>(
                AscensionA20RewardScreenReflection.HeaderLabelField,
                __instance,
                out var headerLabel))
        {
            headerLabel.SetTextAutoSize(new LocString("ascension", "A20_INTERMISSION_HEADER").GetFormattedText());
        }
    }

    internal static bool IsA20BossOneIntermission(NRewardsScreen screen)
    {
        if (!AscensionA20RewardScreenReflection.TryGetFieldValue<IRunState>(
                AscensionA20RewardScreenReflection.RunStateField,
                screen,
                out var runState))
        {
            return false;
        }

        return AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState) &&
            runState.CurrentRoom?.RoomType == RoomType.Boss &&
            runState.CurrentActIndex == runState.Acts.Count - 1 &&
            runState.Map.SecondBossMapPoint != null &&
            runState.CurrentMapCoord == runState.Map.BossMapPoint.coord;
    }
}

[HarmonyPatch(typeof(NRewardsScreen), "UpdateScreenState")]
internal static class AscensionA20RewardScreenStatePatch
{
    [HarmonyPostfix]
    private static void Postfix(NRewardsScreen __instance)
    {
        if (!AscensionA20RewardScreenReadyPatch.IsA20BossOneIntermission(__instance))
        {
            return;
        }

        if (AscensionA20RewardScreenReflection.TryGetFieldValue<NProceedButton>(
                AscensionA20RewardScreenReflection.ProceedButtonField,
                __instance,
                out var proceedButton) &&
            !proceedButton.IsSkip)
        {
            proceedButton.UpdateText(new LocString("ascension", "A20_INTERMISSION_PROCEED"));
        }
    }
}
