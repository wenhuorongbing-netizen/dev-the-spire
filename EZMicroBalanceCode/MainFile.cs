using Godot;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Core.Features;
using EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "EZMicroBalance"; // Stable technical manifest id; player-facing name is Spire Plus.
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        SpirePlusDebug.Log("Init", "Spire Plus initialization starting.");

        RitsuLibBootstrap.ApplyPatches(ModId);

        // RitsuLib SavedProperties closes registration before gameplay hooks run.
        // Touch every SavedAttachedState field while mod initialization is open.
        AncientSavedStateFields.EnsureRegistered();
        AscensionSavedStateFields.EnsureRegistered();
        SpirePlusDebug.Log("Init", "RitsuLib saved-state fields registered.");

        SpirePlusModConfig.Register(ModId);
        SpirePlusDebug.Log("Init", "RitsuLib mod settings registered.");

        SpirePlusContentRegistrationService.Register(ModId);
        SpirePlusDebug.Log("Init", "RitsuLib content registered.");

        var registry = SpirePlusFeatureRegistry.CreateDefault();
        registry.InitializeAll();
        registry.LogFeatureSummary();
        SpirePlusDebug.Log("Init", "Feature registry initialized.");
    }
}
