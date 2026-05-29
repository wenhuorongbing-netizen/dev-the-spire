using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class UrdaStateCodecGuardTests
{
    // --- Source-structure guard tests ---

    private static string ReadCodecSource()
    {
        return ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaStateCodec.cs");
    }

    [Fact]
    public void CodecHandlesNullInput()
    {
        var source = ReadCodecSource();

        // Decode must accept nullable input and return default state
        AssertSourceContains(source,
            "Decode(string? raw)");
    }

    [Fact]
    public void CodecHandlesEmptyInput()
    {
        var source = ReadCodecSource();

        // Must handle empty/null input gracefully
        AssertSourceContains(source,
            "string.IsNullOrEmpty(raw)");
    }

    [Fact]
    public void CodecHandlesLegacyFormat()
    {
        var source = ReadCodecSource();

        // Must handle legacy format with fewer parts (backward compat)
        AssertSourceContains(source,
            "GetPart(parts,",
            "parts.Length <");
    }

    [Fact]
    public void CodecEncodesAllFields()
    {
        var source = ReadCodecSource();

        // Encode must write all progress fields
        AssertSourceContains(source,
            "Encode(",
            "UrdaStateSnapshot",
            "string.Join(");
    }

    [Fact]
    public void CodecSanitizesDelimiterInFields()
    {
        var source = ReadCodecSource();

        // Must sanitize semicolons in field values to prevent wire format corruption
        AssertSourceContains(source,
            "SanitizeStateField");
    }

    [Fact]
    public void CodecUsesSemicolonDelimiter()
    {
        var source = ReadCodecSource();

        // Wire format uses semicolon separator (char literal)
        AssertSourceContains(source,
            "';'");
    }

    [Fact]
    public void CodecProvidesDefaultSnapshot()
    {
        var source = ReadCodecSource();

        // UrdaStateSnapshot must have a Default factory
        AssertSourceContains(source,
            "Default");
    }

    [Fact]
    public void CodecParseHelpersExist()
    {
        var source = ReadCodecSource();

        // Must have safe parse helpers for int and bool
        AssertSourceContains(source,
            "ParseInt",
            "ParseBool");
    }

    [Fact]
    public void WireFormatIsStable()
    {
        var source = ReadCodecSource();

        // The wire format must use positional fields separated by semicolons
        // This guard ensures we don't accidentally switch to a format that
        // breaks existing save data
        AssertSourceContains(source,
            "raw.Split(ProgressSeparator)");
    }

    [Fact]
    public void CodecDeclaresVersionedPartThresholds()
    {
        var source = ReadCodecSource();

        // Legacy and current format part-count thresholds must be declared as constants
        AssertSourceContains(source,
            "LegacyMinimumPartCount",
            "LegacyBaseIndex",
            "CurrentBaseIndex");
    }

    [Fact]
    public void CodecHandlesHumusCompletionPendingField()
    {
        var source = ReadCodecSource();

        // Current format includes HumusCompletionPending; legacy omits it
        AssertSourceContains(source,
            "hasHumusPendingField",
            "HumusCompletionPending");
    }

    [Fact]
    public void CodecGetPartIsBoundsSafe()
    {
        var source = ReadCodecSource();

        // GetPart must guard against out-of-bounds access
        AssertSourceContains(source,
            "index >= 0 && index < parts.Length");
    }

    [Fact]
    public void CodecParseIntClampsNegativeValues()
    {
        var source = ReadCodecSource();

        // ParseInt must clamp negative values to 0
        AssertSourceContains(source,
            "Math.Max(0, parsed)");
    }

    [Fact]
    public void CodecParseBoolAcceptsNumericOne()
    {
        var source = ReadCodecSource();

        // ParseBool must accept "1" as true for wire compat
        AssertSourceContains(source,
            "value == \"1\"");
    }

    [Fact]
    public void CodecSanitizeReplacesSemicolons()
    {
        var source = ReadCodecSource();

        // SanitizeStateField must replace the progress separator char
        AssertSourceContains(source,
            "Replace(ProgressSeparator, '_')");
    }

    [Fact]
    public void CodecEncodesProgressSeparately()
    {
        var source = ReadCodecSource();

        // The ProgressSeparator constant must be distinct from the delimiter used in string.Join
        // to allow easy format migration
        AssertSourceContains(source,
            "private const char ProgressSeparator");
    }

    [Fact]
    public void CodecUsesDefaultForShortState()
    {
        var source = ReadCodecSource();

        // When parts are too few, must return default progress
        AssertSourceContains(source,
            "UrdaProgress.Default");
    }

    [Fact]
    public void SnapshotDefaultFactoryIsStatic()
    {
        var source = ReadCodecSource();

        // Default must be a static property on the snapshot
        AssertSourceContains(source,
            "static UrdaStateSnapshot Default");
    }

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
