using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaStateCodecGuardTests
{
    // --- Additional edge-case behavioral tests ---

    [Fact]
    public void Roundtrip_SemicolonsInStringFields_AreSanitizedThenDecoded()
    {
        // String fields containing semicolons are sanitized to underscores during encode.
        // After roundtrip, the decoded value should contain underscores, not semicolons.
        var progress = new UrdaProgress(
            SeedbedChecks: 0, SeedbedAccepted: 0, SeedbedTransformed: false,
            HumusSkips: 0, HumusCompleted: false, HumusCompletionPending: false,
            MoltingActive: false, MossRoomMask: 0, TrialCombats: 0,
            TrialSuccessfulCombats: 0, TrialPlayedThisCombat: false, TrialSettled: false,
            ShallowRelicPending: false, ShallowRelicRooted: false,
            ShallowRelicId: "a;b;c", RootedRouteCoord: "x;y",
            RootedRouteResolved: false, RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: false, AfterRainCompensated: false,
            AfterRainTriggerCount: 0, RootSightEyes: 0,
            RootSightFirstPotionGranted: false, RootSightMarkedCoords: "1;2",
            SeedBankCardIds: "card_a;card_b", SeedBankSettled: false,
            RootSightPreviewRecords: "rec;x", SeedbedCombatSlots: 0);

        var original = new UrdaStateSnapshot("test", progress);
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        // Semicolons replaced by underscores during encode
        Assert.Equal("a_b_c", decoded.Progress.ShallowRelicId);
        Assert.Equal("x_y", decoded.Progress.RootedRouteCoord);
        Assert.Equal("1_2", decoded.Progress.RootSightMarkedCoords);
        Assert.Equal("card_a_card_b", decoded.Progress.SeedBankCardIds);
        Assert.Equal("rec_x", decoded.Progress.RootSightPreviewRecords);
    }

    [Fact]
    public void Decode_SinglePartInput_ParsesBlessingOnly()
    {
        // A single value with no semicolons: only blessing is parsed, progress is default
        var result = UrdaStateCodec.Decode("solo_blessing");

        Assert.Equal("solo_blessing", result.SelectedBlessing);
        Assert.Equal(UrdaProgress.Default, result.Progress);
    }

    [Fact]
    public void Roundtrip_AllBooleansTrue_PreservesAllTrue()
    {
        var progress = new UrdaProgress(
            SeedbedChecks: 1, SeedbedAccepted: 1, SeedbedTransformed: true,
            HumusSkips: 1, HumusCompleted: true, HumusCompletionPending: true,
            MoltingActive: true, MossRoomMask: 1, TrialCombats: 1,
            TrialSuccessfulCombats: 1, TrialPlayedThisCombat: true, TrialSettled: true,
            ShallowRelicPending: true, ShallowRelicRooted: true,
            ShallowRelicId: "r", RootedRouteCoord: "c",
            RootedRouteResolved: true, RootedRouteWithered: true,
            AfterRainTriggeredThisCombat: true, AfterRainCompensated: true,
            AfterRainTriggerCount: 1, RootSightEyes: 1,
            RootSightFirstPotionGranted: true, RootSightMarkedCoords: "m",
            SeedBankCardIds: "s", SeedBankSettled: true,
            RootSightPreviewRecords: "p", SeedbedCombatSlots: 1);

        var original = new UrdaStateSnapshot("all_true", progress);
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(original, decoded);
        Assert.True(decoded.Progress.SeedbedTransformed);
        Assert.True(decoded.Progress.HumusCompleted);
        Assert.True(decoded.Progress.HumusCompletionPending);
        Assert.True(decoded.Progress.MoltingActive);
        Assert.True(decoded.Progress.TrialPlayedThisCombat);
        Assert.True(decoded.Progress.TrialSettled);
        Assert.True(decoded.Progress.ShallowRelicPending);
        Assert.True(decoded.Progress.ShallowRelicRooted);
        Assert.True(decoded.Progress.RootedRouteResolved);
        Assert.True(decoded.Progress.RootedRouteWithered);
        Assert.True(decoded.Progress.AfterRainTriggeredThisCombat);
        Assert.True(decoded.Progress.AfterRainCompensated);
        Assert.True(decoded.Progress.RootSightFirstPotionGranted);
        Assert.True(decoded.Progress.SeedBankSettled);
    }

    [Fact]
    public void Roundtrip_AllBooleansFalse_PreservesAllFalse()
    {
        var progress = new UrdaProgress(
            SeedbedChecks: 0, SeedbedAccepted: 0, SeedbedTransformed: false,
            HumusSkips: 0, HumusCompleted: false, HumusCompletionPending: false,
            MoltingActive: false, MossRoomMask: 0, TrialCombats: 0,
            TrialSuccessfulCombats: 0, TrialPlayedThisCombat: false, TrialSettled: false,
            ShallowRelicPending: false, ShallowRelicRooted: false,
            ShallowRelicId: string.Empty, RootedRouteCoord: string.Empty,
            RootedRouteResolved: false, RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: false, AfterRainCompensated: false,
            AfterRainTriggerCount: 0, RootSightEyes: 0,
            RootSightFirstPotionGranted: false, RootSightMarkedCoords: string.Empty,
            SeedBankCardIds: string.Empty, SeedBankSettled: false,
            RootSightPreviewRecords: string.Empty, SeedbedCombatSlots: 0);

        var original = new UrdaStateSnapshot("all_false", progress);
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Decode_WhitespaceInput_ParsesAsBlessing()
    {
        // Whitespace is not null or empty, so it is treated as a single-part input
        var result = UrdaStateCodec.Decode("   ");

        Assert.Equal("   ", result.SelectedBlessing);
        Assert.Equal(UrdaProgress.Default, result.Progress);
    }

    [Fact]
    public void Encode_ProducesSemicolonSeparatedString()
    {
        var encoded = UrdaStateCodec.Encode(UrdaStateSnapshot.Default);

        Assert.Contains(";", encoded);
        Assert.StartsWith(";", encoded, StringComparison.Ordinal); // empty blessing -> first semicolon immediately
    }

    [Fact]
    public void Decode_ExtraTrailingParts_AreIgnoredGracefully()
    {
        // If the wire format has more parts than expected, it should not crash
        var input = string.Concat("blessing", new string(';', 100), "extra");
        var result = UrdaStateCodec.Decode(input);

        Assert.Equal("blessing", result.SelectedBlessing);
        // Should not throw -- extra parts beyond the expected count are handled by GetPart
    }

    [Fact]
    public void Roundtrip_LargeIntValues_PreservesValues()
    {
        var progress = new UrdaProgress(
            SeedbedChecks: 999999, SeedbedAccepted: 888888, SeedbedTransformed: false,
            HumusSkips: 777777, HumusCompleted: false, HumusCompletionPending: false,
            MoltingActive: false, MossRoomMask: 666666, TrialCombats: 555555,
            TrialSuccessfulCombats: 444444, TrialPlayedThisCombat: false, TrialSettled: false,
            ShallowRelicPending: false, ShallowRelicRooted: false,
            ShallowRelicId: string.Empty, RootedRouteCoord: string.Empty,
            RootedRouteResolved: false, RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: false, AfterRainCompensated: false,
            AfterRainTriggerCount: 333333, RootSightEyes: 222222,
            RootSightFirstPotionGranted: false, RootSightMarkedCoords: string.Empty,
            SeedBankCardIds: string.Empty, SeedBankSettled: false,
            RootSightPreviewRecords: string.Empty, SeedbedCombatSlots: 111111);

        var original = new UrdaStateSnapshot("large_vals", progress);
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(original, decoded);
        Assert.Equal(999999, decoded.Progress.SeedbedChecks);
        Assert.Equal(888888, decoded.Progress.SeedbedAccepted);
        Assert.Equal(777777, decoded.Progress.HumusSkips);
        Assert.Equal(666666, decoded.Progress.MossRoomMask);
        Assert.Equal(111111, decoded.Progress.SeedbedCombatSlots);
    }
}
