using BaseLib.Config;
using EZFuturePeek.EZFuturePeekCode.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace EZFuturePeek.EZFuturePeekCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "EZFuturePeek";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new EZFuturePeekConfig());

        var harmony = new Harmony(ModId);
        harmony.PatchAll();

        Logger.Info("[EZFuturePeek] Initialized.");
    }
}
