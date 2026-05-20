using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuFightFeatureGate
{
    public const string EnableEnvironmentVariable = "EZMB_ENABLE_VAKUU_FIGHT";
    public const string SpirePlusEnableEnvironmentVariable = "SPIREPLUS_ENABLE_VAKUU_FIGHT";
    public const string DisableEnvironmentVariable = "EZMB_DISABLE_VAKUU_FIGHT";
    public const string SpirePlusDisableEnvironmentVariable = "SPIREPLUS_DISABLE_VAKUU_FIGHT";
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string SpirePlusForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string ForceFightEnvironmentVariable = "EZMB_FORCE_VAKUU_FIGHT";
    public const string SpirePlusForceFightEnvironmentVariable = "SPIREPLUS_FORCE_VAKUU_FIGHT";

    private static readonly object CommandForceFightLock = new();
    private static WeakReference<IRunState>? commandForcedFightRun;

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceAncientEnvironmentVariable, ForceAncientEnvironmentVariable);

    public static bool ShouldForceVakuu =>
        IsForcedAncient("VAKUU") || IsForcedAncient("EZMB_VAKUU");

    public static bool ShouldForceFight =>
        AncientFeatureGate.IsTruthyEnvironmentVariable(ForceFightEnvironmentVariable) ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusForceFightEnvironmentVariable);

    public static bool ShouldForceFightForRun(IRunState runState) =>
        ShouldForceFight || IsCommandForceFightArmedForRun(runState);

    public static bool ShouldEnableFight =>
        ShouldForceFight ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(EnableEnvironmentVariable) ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusEnableEnvironmentVariable);

    public static bool IsFightEnabled(UnlockState _) =>
        ShouldEnableFight &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusDisableEnvironmentVariable);

    public static bool IsFightEnabledForRun(IRunState runState) =>
        IsFightEnabledForRun(runState, ShouldForceFightForRun(runState));

    public static bool IsFightEnabledForRun(IRunState runState, bool forceFight) =>
        (forceFight || IsFightEnabled(runState.UnlockState)) && runState.Players.Count == 1;

    public static void ArmCommandForceFight(IRunState runState)
    {
        lock (CommandForceFightLock)
        {
            commandForcedFightRun = new WeakReference<IRunState>(runState);
        }
    }

    public static void ClearCommandForceFight(IRunState runState)
    {
        lock (CommandForceFightLock)
        {
            if (TryGetCommandForceFightRun(out var target) && ReferenceEquals(target, runState))
            {
                commandForcedFightRun = null;
            }
        }
    }

    public static void ConsumeCommandForceFightForRun(IRunState runState)
    {
        ClearCommandForceFight(runState);
    }

    public static bool HasCommandForceFightForRun(IRunState runState) =>
        IsCommandForceFightArmedForRun(runState);

    public static async Task ClearCommandForceFightWhenBeginEventCompletes(Task beginEventTask, IRunState runState)
    {
        try
        {
            await beginEventTask;
        }
        finally
        {
            ClearCommandForceFight(runState);
        }
    }

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);

    private static bool IsCommandForceFightArmedForRun(IRunState runState)
    {
        lock (CommandForceFightLock)
        {
            return TryGetCommandForceFightRun(out var target) && ReferenceEquals(target, runState);
        }
    }

    private static bool TryGetCommandForceFightRun(out IRunState? runState)
    {
        runState = null;
        if (commandForcedFightRun == null)
        {
            return false;
        }

        if (commandForcedFightRun.TryGetTarget(out runState))
        {
            return true;
        }

        commandForcedFightRun = null;
        return false;
    }
}
