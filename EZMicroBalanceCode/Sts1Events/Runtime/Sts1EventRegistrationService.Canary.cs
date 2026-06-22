using STS2RitsuLib;
using STS2RitsuLib.Content;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;
using MegaCrit.Sts2.Core.Models.Acts;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

internal static partial class Sts1EventRegistrationService
{
    /// <summary>
    /// Registers exactly 4 canary event types through RitsuLib.
    /// </summary>
    public static void RegisterCanaryOnly(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping canary event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering canary events (Big Fish, Golden Idol, Lab, Divine Fountain)...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        // Keep CanaryOnly explicit: these six RitsuLib calls produce exactly
        // four event types, with two Act 1 events registered in both Act 1 maps.
        content.ActEvent<Overgrowth, Sts1BigFish>();
        content.ActEvent<Underdocks, Sts1BigFish>();
        content.ActEvent<Overgrowth, Sts1GoldenIdol>();
        content.ActEvent<Underdocks, Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1DivineFountain>();

        content.Apply();

        logger.Info("[StS1 Events] Canary events registered successfully.");
    }
}
