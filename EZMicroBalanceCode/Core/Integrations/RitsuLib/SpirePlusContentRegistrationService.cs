using STS2RitsuLib;
using STS2RitsuLib.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    public static void Register(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[Spire Plus] RitsuLib not active; skipping Spire Plus content registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[Spire Plus] Registering native content with RitsuLib...");

        // RitsuLib content packs are the only supported path for Spire Plus
        // model registration. Keep this orchestration file small so each
        // content kind has an obvious home in the sibling partial files.
        var content = RitsuLibFramework.CreateContentPack(modId);

        RegisterAncients(content);
        RegisterVakuuEncounter(content);
        RegisterCards(content);
        RegisterRelics(content);
        RegisterPowers(content);
        RegisterEnchantments(content);

        content.Apply();

        SpirePlusInlineLocalizationRegistry.RegisterKnownProviders();

        logger.Info("[Spire Plus] Native content registered successfully.");
    }

    // Public-entry strings already include the Spire Plus namespace. Passing
    // them through RitsuLib preserves the public id used by localization,
    // saves, screenshots, and package evidence.
    private static ModelPublicEntryOptions FullEntry(string publicEntry) =>
        ModelPublicEntryOptions.FromFullPublicEntry(publicEntry);
}
