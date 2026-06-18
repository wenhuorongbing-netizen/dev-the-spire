using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaStateCodecGuardTests
{
    // --- Behavioral tests ---

    [Fact]
    public void Decode_NullInput_ReturnsDefaultSnapshot()
    {
        var result = UrdaStateCodec.Decode(null);

        Assert.Equal(UrdaStateSnapshot.Default.SelectedBlessing, result.SelectedBlessing);
        Assert.Equal(UrdaStateSnapshot.Default.Progress, result.Progress);
    }

    [Fact]
    public void Decode_EmptyInput_ReturnsDefaultSnapshot()
    {
        var result = UrdaStateCodec.Decode(string.Empty);

        Assert.Equal(UrdaStateSnapshot.Default, result);
    }

    [Fact]
    public void Decode_ShortInput_ReturnsDefaultProgressWithParsedBlessing()
    {
        // Fewer than LegacyMinimumPartCount (8) parts: blessing is parsed, progress is default
        var result = UrdaStateCodec.Decode("my_blessing;1;2;3");

        Assert.Equal("my_blessing", result.SelectedBlessing);
        Assert.Equal(UrdaProgress.Default, result.Progress);
    }

    [Fact]
    public void Decode_LegacyFormatEightParts_OmitsHumusCompletionPending()
    {
        // Legacy format: 8 parts, no HumusCompletionPending field
        // Layout: blessing;SeedbedChecks;SeedbedAccepted;SeedbedTransformed;HumusSkips;HumusCompleted;MoltingActive;MossRoomMask
        var result = UrdaStateCodec.Decode("legacy_blessing;10;5;1;3;1;1;7");

        Assert.Equal("legacy_blessing", result.SelectedBlessing);
        Assert.Equal(10, result.Progress.SeedbedChecks);
        Assert.Equal(5, result.Progress.SeedbedAccepted);
        Assert.True(result.Progress.SeedbedTransformed);
        Assert.Equal(3, result.Progress.HumusSkips);
        Assert.True(result.Progress.HumusCompleted);
        Assert.False(result.Progress.HumusCompletionPending); // Not in legacy format
        Assert.True(result.Progress.MoltingActive);           // parts[6] = MoltingActive in legacy
        Assert.Equal(7, result.Progress.MossRoomMask);         // parts[7]
    }

    [Fact]
    public void Decode_CurrentFormatNinePlusParts_IncludesHumusCompletionPending()
    {
        // Current format: >= 9 parts, HumusCompletionPending is at index 6
        // Layout: blessing;SeedbedChecks;SeedbedAccepted;SeedbedTransformed;HumusSkips;HumusCompleted;HumusCompletionPending;MoltingActive;MossRoomMask;...
        var result = UrdaStateCodec.Decode("current_blessing;10;5;1;3;1;1;1;7;20");

        Assert.Equal("current_blessing", result.SelectedBlessing);
        Assert.Equal(10, result.Progress.SeedbedChecks);
        Assert.Equal(5, result.Progress.SeedbedAccepted);
        Assert.True(result.Progress.SeedbedTransformed);
        Assert.Equal(3, result.Progress.HumusSkips);
        Assert.True(result.Progress.HumusCompleted);
        Assert.True(result.Progress.HumusCompletionPending); // parts[6] = HumusCompletionPending in current
        Assert.True(result.Progress.MoltingActive);           // parts[7] = MoltingActive in current
        Assert.Equal(7, result.Progress.MossRoomMask);         // parts[8]
    }

    [Fact]
    public void Roundtrip_EncodeDecode_PreservesAllFields()
    {
        var progress = new UrdaProgress(
            SeedbedChecks: 5,
            SeedbedAccepted: 3,
            SeedbedTransformed: true,
            HumusSkips: 2,
            HumusCompleted: true,
            HumusCompletionPending: true,
            MoltingActive: false,
            MossRoomMask: 7,
            TrialCombats: 4,
            TrialSuccessfulCombats: 2,
            TrialPlayedThisCombat: true,
            TrialSettled: false,
            ShallowRelicPending: true,
            ShallowRelicRooted: false,
            ShallowRelicId: "relic_123",
            RootedRouteCoord: "coord_abc",
            RootedRouteResolved: true,
            RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: true,
            AfterRainCompensated: false,
            AfterRainTriggerCount: 3,
            RootSightEyes: 2,
            RootSightFirstPotionGranted: true,
            RootSightMarkedCoords: "4,5",
            SeedBankCardIds: "card1,card2",
            SeedBankSettled: true,
            RootSightPreviewRecords: "record1",
            SeedbedCombatSlots: 6);

        var original = new UrdaStateSnapshot("test_blessing", progress);
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(original.SelectedBlessing, decoded.SelectedBlessing);
        Assert.Equal(original.Progress, decoded.Progress);
    }

    [Fact]
    public void Decode_CurrentFullWireFormat_MapsEveryFieldByPosition()
    {
        var input = string.Join(
            ';',
            "full_blessing",
            1,
            2,
            1,
            3,
            1,
            1,
            0,
            4,
            5,
            6,
            1,
            0,
            1,
            0,
            "shallow_relic",
            "rooted_coord",
            1,
            0,
            1,
            0,
            7,
            8,
            1,
            "marked_coords",
            "seed_bank_cards",
            1,
            "preview_records",
            9);

        var result = UrdaStateCodec.Decode(input);

        Assert.Equal("full_blessing", result.SelectedBlessing);
        Assert.Equal(1, result.Progress.SeedbedChecks);
        Assert.Equal(2, result.Progress.SeedbedAccepted);
        Assert.True(result.Progress.SeedbedTransformed);
        Assert.Equal(3, result.Progress.HumusSkips);
        Assert.True(result.Progress.HumusCompleted);
        Assert.True(result.Progress.HumusCompletionPending);
        Assert.False(result.Progress.MoltingActive);
        Assert.Equal(4, result.Progress.MossRoomMask);
        Assert.Equal(5, result.Progress.TrialCombats);
        Assert.Equal(6, result.Progress.TrialSuccessfulCombats);
        Assert.True(result.Progress.TrialPlayedThisCombat);
        Assert.False(result.Progress.TrialSettled);
        Assert.True(result.Progress.ShallowRelicPending);
        Assert.False(result.Progress.ShallowRelicRooted);
        Assert.Equal("shallow_relic", result.Progress.ShallowRelicId);
        Assert.Equal("rooted_coord", result.Progress.RootedRouteCoord);
        Assert.True(result.Progress.RootedRouteResolved);
        Assert.False(result.Progress.RootedRouteWithered);
        Assert.True(result.Progress.AfterRainTriggeredThisCombat);
        Assert.False(result.Progress.AfterRainCompensated);
        Assert.Equal(7, result.Progress.AfterRainTriggerCount);
        Assert.Equal(8, result.Progress.RootSightEyes);
        Assert.True(result.Progress.RootSightFirstPotionGranted);
        Assert.Equal("marked_coords", result.Progress.RootSightMarkedCoords);
        Assert.Equal("seed_bank_cards", result.Progress.SeedBankCardIds);
        Assert.True(result.Progress.SeedBankSettled);
        Assert.Equal("preview_records", result.Progress.RootSightPreviewRecords);
        Assert.Equal(9, result.Progress.SeedbedCombatSlots);
    }

    [Fact]
    public void Decode_LegacyFullWireFormat_MapsTrailingFieldsFromLegacyBaseIndex()
    {
        var input = string.Join(
            ';',
            "legacy_full",
            1,
            2,
            1,
            3,
            1,
            0,
            4,
            5,
            6,
            1,
            0,
            1,
            0,
            "shallow_relic",
            "rooted_coord",
            1,
            0,
            1,
            0,
            7,
            8,
            1,
            "marked_coords",
            "seed_bank_cards",
            1,
            "preview_records",
            9);

        var result = UrdaStateCodec.Decode(input);

        Assert.Equal("legacy_full", result.SelectedBlessing);
        Assert.False(result.Progress.HumusCompletionPending);
        Assert.False(result.Progress.MoltingActive);
        Assert.Equal(4, result.Progress.MossRoomMask);
        Assert.Equal(5, result.Progress.TrialCombats);
        Assert.Equal(6, result.Progress.TrialSuccessfulCombats);
        Assert.True(result.Progress.TrialPlayedThisCombat);
        Assert.False(result.Progress.TrialSettled);
        Assert.True(result.Progress.ShallowRelicPending);
        Assert.False(result.Progress.ShallowRelicRooted);
        Assert.Equal("shallow_relic", result.Progress.ShallowRelicId);
        Assert.Equal("rooted_coord", result.Progress.RootedRouteCoord);
        Assert.True(result.Progress.RootedRouteResolved);
        Assert.False(result.Progress.RootedRouteWithered);
        Assert.True(result.Progress.AfterRainTriggeredThisCombat);
        Assert.False(result.Progress.AfterRainCompensated);
        Assert.Equal(7, result.Progress.AfterRainTriggerCount);
        Assert.Equal(8, result.Progress.RootSightEyes);
        Assert.True(result.Progress.RootSightFirstPotionGranted);
        Assert.Equal("marked_coords", result.Progress.RootSightMarkedCoords);
        Assert.Equal("seed_bank_cards", result.Progress.SeedBankCardIds);
        Assert.True(result.Progress.SeedBankSettled);
        Assert.Equal("preview_records", result.Progress.RootSightPreviewRecords);
        Assert.Equal(9, result.Progress.SeedbedCombatSlots);
    }

    [Fact]
    public void Roundtrip_EncodeDecode_DefaultSnapshot()
    {
        var original = UrdaStateSnapshot.Default;
        var encoded = UrdaStateCodec.Encode(original);
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Encode_ProducesExpectedPartCount()
    {
        // Encode should produce exactly 29 semicolon-separated parts
        var encoded = UrdaStateCodec.Encode(UrdaStateSnapshot.Default);
        var partCount = encoded.Split(';').Length;

        Assert.Equal(29, partCount);
    }

    [Fact]
    public void ParseBool_NumericOneInWireFormat_IsTrue()
    {
        // Test that "1" in wire format is decoded as true
        var result = UrdaStateCodec.Decode("blessing;0;0;1;0;0;0;0;0");

        // parts[3] = "1" should be parsed as true for SeedbedTransformed
        Assert.True(result.Progress.SeedbedTransformed);
    }

    [Fact]
    public void ParseBool_ZeroInWireFormat_IsFalse()
    {
        var result = UrdaStateCodec.Decode("blessing;0;0;0;0;0;0;0;0");

        Assert.False(result.Progress.SeedbedTransformed);
    }

    [Fact]
    public void ParseInt_NegativeValue_ClampsToZero()
    {
        // Negative values in wire format should be clamped to 0
        var result = UrdaStateCodec.Decode("blessing;-5;0;0;0;0;0;0;0");

        Assert.Equal(0, result.Progress.SeedbedChecks);
    }

    [Fact]
    public void Encode_SanitizesSemicolonsInStringFields()
    {
        // String fields containing semicolons should be sanitized to underscores
        var progress = new UrdaProgress(
            SeedbedChecks: 0,
            SeedbedAccepted: 0,
            SeedbedTransformed: false,
            HumusSkips: 0,
            HumusCompleted: false,
            HumusCompletionPending: false,
            MoltingActive: false,
            MossRoomMask: 0,
            TrialCombats: 0,
            TrialSuccessfulCombats: 0,
            TrialPlayedThisCombat: false,
            TrialSettled: false,
            ShallowRelicPending: false,
            ShallowRelicRooted: false,
            ShallowRelicId: "abc;def",
            RootedRouteCoord: "x;y;z",
            RootedRouteResolved: false,
            RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: false,
            AfterRainCompensated: false,
            AfterRainTriggerCount: 0,
            RootSightEyes: 0,
            RootSightFirstPotionGranted: false,
            RootSightMarkedCoords: "1;2",
            SeedBankCardIds: "a;b;c",
            SeedBankSettled: false,
            RootSightPreviewRecords: "r;s",
            SeedbedCombatSlots: 0);

        var encoded = UrdaStateCodec.Encode(new UrdaStateSnapshot("test", progress));

        // After encoding, semicolons in field values should be underscores
        Assert.DoesNotContain("abc;def", encoded, StringComparison.Ordinal);
        Assert.Contains("abc_def", encoded, StringComparison.Ordinal);
    }

    [Fact]
    public void Encode_NullStringFields_SerializesAsEmptyFields()
    {
        var progress = new UrdaProgress(
            SeedbedChecks: 0,
            SeedbedAccepted: 0,
            SeedbedTransformed: false,
            HumusSkips: 0,
            HumusCompleted: false,
            HumusCompletionPending: false,
            MoltingActive: false,
            MossRoomMask: 0,
            TrialCombats: 0,
            TrialSuccessfulCombats: 0,
            TrialPlayedThisCombat: false,
            TrialSettled: false,
            ShallowRelicPending: false,
            ShallowRelicRooted: false,
            ShallowRelicId: null!,
            RootedRouteCoord: null!,
            RootedRouteResolved: false,
            RootedRouteWithered: false,
            AfterRainTriggeredThisCombat: false,
            AfterRainCompensated: false,
            AfterRainTriggerCount: 0,
            RootSightEyes: 0,
            RootSightFirstPotionGranted: false,
            RootSightMarkedCoords: null!,
            SeedBankCardIds: null!,
            SeedBankSettled: false,
            RootSightPreviewRecords: null!,
            SeedbedCombatSlots: 0);

        var encoded = UrdaStateCodec.Encode(new UrdaStateSnapshot("test", progress));
        var decoded = UrdaStateCodec.Decode(encoded);

        Assert.Equal(string.Empty, decoded.Progress.ShallowRelicId);
        Assert.Equal(string.Empty, decoded.Progress.RootedRouteCoord);
        Assert.Equal(string.Empty, decoded.Progress.RootSightMarkedCoords);
        Assert.Equal(string.Empty, decoded.Progress.SeedBankCardIds);
        Assert.Equal(string.Empty, decoded.Progress.RootSightPreviewRecords);
    }

    [Fact]
    public void Decode_MalformedNonNumericFields_FallsBackToSafeDefaults()
    {
        // Fields that can't be parsed should fall back to safe defaults (0 for int, false for bool)
        var result = UrdaStateCodec.Decode("blessing;abc;xyz;maybe;NaN;true;false;not_bool;no_int");

        Assert.Equal(0, result.Progress.SeedbedChecks);           // "abc" -> 0
        Assert.Equal(0, result.Progress.SeedbedAccepted);          // "xyz" -> 0
        Assert.False(result.Progress.SeedbedTransformed);          // "maybe" -> not "1", bool.TryParse fails -> false
        Assert.Equal(0, result.Progress.HumusSkips);              // "NaN" -> 0
        Assert.True(result.Progress.HumusCompleted);               // "true" -> bool.TryParse("true") -> true
        Assert.False(result.Progress.HumusCompletionPending);     // "false" -> false
        Assert.False(result.Progress.MoltingActive);              // "not_bool" -> false
        Assert.Equal(0, result.Progress.MossRoomMask);            // "no_int" -> 0
    }

    [Fact]
    public void Decode_TrailingFieldsOutOfBounds_DefaultToSafeValues()
    {
        // If wire format has exactly 9 parts (current minimum with HumusPending),
        // fields beyond index 8 should fall back to safe defaults via GetPart
        var result = UrdaStateCodec.Decode("blessing;1;2;0;3;0;0;1;4");

        Assert.Equal(1, result.Progress.SeedbedChecks);
        Assert.Equal(4, result.Progress.MossRoomMask);
        // Fields from baseIndex (9) onwards should be empty/default
        Assert.Equal(0, result.Progress.TrialCombats);         // GetPart returns ""
        Assert.Equal(0, result.Progress.TrialSuccessfulCombats);
        Assert.False(result.Progress.TrialPlayedThisCombat);
        Assert.Equal(string.Empty, result.Progress.ShallowRelicId);
        Assert.Equal(string.Empty, result.Progress.RootedRouteCoord);
    }

    [Fact]
    public void SnapshotDefault_HasExpectedValues()
    {
        var d = UrdaStateSnapshot.Default;

        Assert.Equal(string.Empty, d.SelectedBlessing);
        Assert.Equal(0, d.Progress.SeedbedChecks);
        Assert.Equal(0, d.Progress.SeedbedAccepted);
        Assert.False(d.Progress.SeedbedTransformed);
        Assert.Equal(0, d.Progress.HumusSkips);
        Assert.False(d.Progress.HumusCompleted);
        Assert.False(d.Progress.HumusCompletionPending);
        Assert.False(d.Progress.MoltingActive);
        Assert.Equal(0, d.Progress.MossRoomMask);
        Assert.Equal(0, d.Progress.TrialCombats);
        Assert.Equal(0, d.Progress.TrialSuccessfulCombats);
        Assert.False(d.Progress.TrialPlayedThisCombat);
        Assert.False(d.Progress.TrialSettled);
        Assert.False(d.Progress.ShallowRelicPending);
        Assert.False(d.Progress.ShallowRelicRooted);
        Assert.Equal(string.Empty, d.Progress.ShallowRelicId);
        Assert.Equal(string.Empty, d.Progress.RootedRouteCoord);
        Assert.False(d.Progress.RootedRouteResolved);
        Assert.False(d.Progress.RootedRouteWithered);
        Assert.False(d.Progress.AfterRainTriggeredThisCombat);
        Assert.False(d.Progress.AfterRainCompensated);
        Assert.Equal(0, d.Progress.AfterRainTriggerCount);
        Assert.Equal(0, d.Progress.RootSightEyes);
        Assert.False(d.Progress.RootSightFirstPotionGranted);
        Assert.Equal(string.Empty, d.Progress.RootSightMarkedCoords);
        Assert.Equal(string.Empty, d.Progress.SeedBankCardIds);
        Assert.False(d.Progress.SeedBankSettled);
        Assert.Equal(string.Empty, d.Progress.RootSightPreviewRecords);
        Assert.Equal(0, d.Progress.SeedbedCombatSlots);
    }

}
