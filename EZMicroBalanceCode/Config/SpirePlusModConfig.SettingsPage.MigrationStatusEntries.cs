using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Keep these entry IDs stable. The manual screenshot checklist and
    // future automation use them to prove the RitsuLib-only settings page.
    private static void AddRitsuLibSummaryParagraph(ModSettingsSectionBuilder section) =>
        section.AddParagraph(
            RitsuLibSummaryEntryId,
            Text("SPIREPLUS-RITSULIB_ONLY_SUMMARY.title", "RitsuLib-only mod surface"),
            Text(
                "SPIREPLUS-RITSULIB_ONLY_SUMMARY.description",
                "This page is registered through RitsuLib. Spire Plus uses RitsuLib for settings persistence, content registration, patch metadata, and saved marker state."));

    private static void AddRequiredRuntimeDependencyCard(ModSettingsSectionBuilder section) =>
        section.AddInfoCard(
            RequiredRuntimeDependencyEntryId,
            Text("SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.title", "Runtime dependency"),
            LiteralText(RequiredRuntimeDependency),
            Text(
                "SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.description",
                "Install the runtime pack under the game mods/STS2-RitsuLib folder before enabling Spire Plus."));

    private static void AddStableManifestIdCard(ModSettingsSectionBuilder section) =>
        section.AddInfoCard(
            StableManifestIdEntryId,
            Text("SPIREPLUS-STABLE_MANIFEST_ID.title", "Technical id"),
            LiteralText(StableTechnicalId),
            Text(
                "SPIREPLUS-STABLE_MANIFEST_ID.description",
                "This id remains only for the manifest, install folder, saves, and compatibility. Player-facing UI should say Spire Plus."));

    private static void AddProofBoundaryCard(ModSettingsSectionBuilder section) =>
        section.AddInfoCard(
            ProofBoundaryEntryId,
            Text("SPIREPLUS-PROOF_BOUNDARY.title", "Evidence boundary"),
            Text("SPIREPLUS-PROOF_BOUNDARY.subtitle", "Settings screenshots prove UI visibility only."),
            Text(
                "SPIREPLUS-PROOF_BOUNDARY.description",
                "Gameplay, clicked Ancient screens, save/load, co-op, and release readiness still need separate evidence."));
}
