using Godot;
using HarmonyLib;
using BaseLib.Config;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Config;
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
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
        LothaInitializer.Initialize();
        MorviInitializer.Initialize();
        UrdaInitializer.Initialize();
        VakuuFightInitializer.Initialize();
        AscensionInitializer.Initialize();
    }
}
