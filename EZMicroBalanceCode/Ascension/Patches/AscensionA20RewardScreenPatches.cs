using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Screens;
using STS2RitsuLib.Patching.Models;

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

internal sealed class AscensionA20RewardScreenReadyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-a20-reward-screen-ready";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Replace the A20 Boss 1 reward-screen header with the courtyard wording";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))];

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

internal sealed class AscensionA20RewardScreenStatePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-a20-reward-screen-state";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Keep the A20 Boss 1 terminal proceed button labeled for the courtyard transition";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NRewardsScreen), "UpdateScreenState")];

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
