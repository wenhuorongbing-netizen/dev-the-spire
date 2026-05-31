using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

internal static partial class CardPlayContextCanary
{
    static partial void LogSingleExtraPlay(
        string feature,
        string reason,
        ExtraPlayPolicy policy,
        int currentDepth,
        bool depthIncremented)
    {
        ReleaseEvidenceLog.Log(
            "CardPlayContextCanary",
            "single_extra_play_evaluated",
            data: new Dictionary<string, object?>
            {
                ["feature"] = feature,
                ["reason"] = reason,
                ["policy"] = policy,
                ["maxDepth"] = CardPlayContext.MaxDepth,
                ["currentDepth"] = currentDepth,
                ["depthIncremented"] = depthIncremented
            });
    }
}
