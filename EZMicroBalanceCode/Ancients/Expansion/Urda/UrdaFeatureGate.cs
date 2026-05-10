using System;

using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaFeatureGate
{
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_URDA_BLESSING";

    public static string? ForcedBlessing =>
        Environment.GetEnvironmentVariable(ForceBlessingEnvironmentVariable)?.Trim();

    public static bool IsUrdaEnabled(UnlockState _unlockState) =>
        string.Equals(
            Environment.GetEnvironmentVariable(ForceAncientEnvironmentVariable)?.Trim(),
            "URDA",
            StringComparison.OrdinalIgnoreCase);
}
