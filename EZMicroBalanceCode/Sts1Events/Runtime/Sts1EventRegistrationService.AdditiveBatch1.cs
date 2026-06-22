using STS2RitsuLib;
using STS2RitsuLib.Content;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;
using MegaCrit.Sts2.Core.Models.Acts;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

internal static partial class Sts1EventRegistrationService
{
    /// <summary>
    /// Registers exactly 10 verified-scope prototype events: four canary events plus six simple batch events.
    /// </summary>
    public static void RegisterAdditiveBatch1(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping AdditiveBatch1 event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering AdditiveBatch1 events (4 canary + 6 simple)...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        // Keep this verified batch separate from draft registration so release-safe
        // modes cannot accidentally inherit draft or blocked events.
        content.ActEvent<Overgrowth, Sts1BigFish>();
        content.ActEvent<Underdocks, Sts1BigFish>();
        content.ActEvent<Overgrowth, Sts1GoldenIdol>();
        content.ActEvent<Underdocks, Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1DivineFountain>();
        content.SharedEvent<Sts1Purifier>();
        content.ActEvent<Glory, Sts1UpgradeShrine>();
        content.SharedEvent<Sts1GoldenShrine>();
        content.ActEvent<Overgrowth, Sts1TheCleric>();
        content.ActEvent<Underdocks, Sts1TheCleric>();
        content.SharedEvent<Sts1OldBeggar>();
        content.ActEvent<Overgrowth, Sts1ShiningLight>();
        content.ActEvent<Underdocks, Sts1ShiningLight>();

        content.Apply();

        logger.Info("[StS1 Events] AdditiveBatch1 events registered successfully.");
    }
}
