using Godot;
using BaseLib.Config;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Core.Features;
using EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;
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
        RitsuLibBootstrap.ApplyPatches(ModId);

        ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
        SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
    }
}
