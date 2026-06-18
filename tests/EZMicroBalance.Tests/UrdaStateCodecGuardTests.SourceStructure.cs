using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaStateCodecGuardTests
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
}
