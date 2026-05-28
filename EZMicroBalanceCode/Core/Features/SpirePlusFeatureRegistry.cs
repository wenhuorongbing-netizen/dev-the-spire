using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal static class SpirePlusFeatureRegistry
{
    public static FeatureRegistry CreateDefault() =>
        new FeatureRegistry(
            message => MainFile.Logger.Info(message),
            message => MainFile.Logger.Warn(message))
            .Register(new DelegateFeatureModule(
                "Ancients.Lotha",
                100,
                () => FeatureGateResult.EnabledByDefault("default-on; Lotha runtime gates remain in LothaFeatureGate."),
                LothaInitializer.Initialize))
            .Register(new DelegateFeatureModule(
                "Ancients.Morvi",
                200,
                () => FeatureGateResult.EnabledByDefault("default-on; Morvi runtime gates remain in MorviFeatureGate."),
                MorviInitializer.Initialize))
            .Register(new DelegateFeatureModule(
                "Ancients.Urda",
                300,
                () => FeatureGateResult.EnabledByDefault("default-on; Urda runtime gates remain in UrdaFeatureGate."),
                UrdaInitializer.Initialize))
            .Register(new DelegateFeatureModule(
                "Ancients.VakuuFight",
                400,
                () => FeatureGateResult.EnabledByDefault("hooks registered; fight entry remains hidden by VakuuFightFeatureGate."),
                VakuuFightInitializer.Initialize))
            .Register(new DelegateFeatureModule(
                "Ascension.A11A20",
                500,
                () => FeatureGateResult.EnabledByDefault("default-on for single-player; co-op gameplay gates remain in AscensionFeatureGate."),
                AscensionInitializer.Initialize));

    private sealed class DelegateFeatureModule : IFeatureModule
    {
        private readonly Func<FeatureGateResult> evaluateGate;
        private readonly Action initialize;

        public DelegateFeatureModule(
            string id,
            int initOrder,
            Func<FeatureGateResult> evaluateGate,
            Action initialize)
        {
            Id = id;
            InitOrder = initOrder;
            this.evaluateGate = evaluateGate;
            this.initialize = initialize;
        }

        public string Id { get; }

        public int InitOrder { get; }

        public FeatureGateResult EvaluateGate() => evaluateGate();

        public void Initialize()
        {
            SpirePlusDebug.Log("Feature", $"Initializing {Id} (order={InitOrder}).");
            initialize();
            SpirePlusDebug.Log("Feature", $"{Id} initialized.");
        }
    }
}
