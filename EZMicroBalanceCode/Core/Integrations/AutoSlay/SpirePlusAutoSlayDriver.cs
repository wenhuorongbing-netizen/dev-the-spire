using System;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Helpers;
using STS2RitsuLib;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.AutoSlay;

/// <summary>
/// Headless game-native AutoSlay driver for Spire Plus smoke testing.
///
/// Re-enables the game's built-in <see cref="AutoSlayer"/> from the mod WITHOUT
/// modifying the game install. Default-OFF: gated by
/// <see cref="SpirePlusAutoSlayGate.EnableEnvironmentVariable"/>. When the gate is
/// unset, <see cref="TryArm"/> registers nothing and returns false, leaving zero
/// impact on normal play.
///
/// Approach (mod-driven, timing-safe): rather than patching
/// <c>NGame.IsReleaseGame()</c> (whose return is checked during NGame init,
/// possibly before mod patches apply), this subscribes to RitsuLib's
/// <see cref="MainMenuReadyEvent"/> lifecycle event and then calls
/// <c>new AutoSlayer().Start(seed, logFile)</c>. <see cref="MainMenuReadyEvent"/>
/// guarantees the main menu is loaded, which is exactly the precondition
/// <c>AutoSlayer.PlayMainMenuAsync</c> needs. RitsuLib replays the event on
/// subscribe (replayCurrentState: true) if it already fired, so arming is
/// timing-safe regardless of mod-init ordering.
/// </summary>
internal static class SpirePlusAutoSlayDriver
{
    private static IDisposable? _subscription;
    private static bool _started;

    /// <summary>Stable hook identifier recorded in launcher evidence.</summary>
    public const string HookId = "spireplus-autoslay-mainmenu-ready";

    /// <summary>
    /// Arms the headless AutoSlay driver if (and only if) the env gate is truthy.
    /// Safe to call once during mod initialization. Returns true when armed.
    /// </summary>
    public static bool TryArm()
    {
        // Default-OFF guard: do nothing, register nothing, when the gate is unset.
        if (!SpirePlusAutoSlayGate.IsEnabled)
        {
            return false;
        }

        var resolution = SpirePlusAutoSlayGate.Resolve(
            commandLineValue: CommandLineHelper.GetValue,
            randomSeedFactory: () => SeedHelper.GetRandomSeed());

        if (!resolution.Enabled)
        {
            return false;
        }

        MainFile.Logger.Info(
            $"[Spire Plus][AutoSlay] Gate enabled. Arming headless AutoSlay on MainMenuReadyEvent. " +
            $"seed={resolution.Seed}; logFile={resolution.LogFile ?? "<none>"}; hook={HookId}.");

        // Subscribe once. replayCurrentState: true makes this fire immediately if
        // the main menu already loaded before the mod subscribed (timing-safe).
        _subscription = RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent>(
            _ => StartAutoSlay(resolution),
            replayCurrentState: true);

        return true;
    }

    private static void StartAutoSlay(SpirePlusAutoSlayGate.Resolution resolution)
    {
        // SubscribeLifecycleOnce should fire a single time, but guard against any
        // double-dispatch so AutoSlayer.Start (which mutates global game state and
        // ultimately quits the game) is never invoked twice.
        if (_started)
        {
            return;
        }

        _started = true;

        try
        {
            MainFile.Logger.Info(
                $"[Spire Plus][AutoSlay] MainMenuReadyEvent received; calling AutoSlayer.Start(seed, logFile). " +
                $"seed={resolution.Seed}; logFile={resolution.LogFile ?? "<none>"}.");

            // This is the game-native runner: it plays a full run to floor 49,
            // logs via AutoSlayLog, then quits the game with an exit code.
            new AutoSlayer().Start(resolution.Seed, resolution.LogFile);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error($"[Spire Plus][AutoSlay] Failed to start AutoSlayer: {ex}");
        }
    }
}
