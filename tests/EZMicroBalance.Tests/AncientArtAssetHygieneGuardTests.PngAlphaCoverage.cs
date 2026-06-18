using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientArtAssetHygieneGuardTests
{
    [Fact]
    public void ActiveSmallUiPngsKeepTransparentPadding()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var manifestTargets = document.RootElement
            .GetProperty("assets")
            .EnumerateArray()
            .Where(asset => RequiredString(asset, "source_status") != "missing")
            .Where(asset =>
            {
                var role = RequiredString(asset, "role");
                return role is "map_icon" or "map_icon_outline" or "run_history_icon" or "run_history_outline" or "option_relic" or "ascension_ui_icon";
            })
            .Select(asset => RequiredString(asset, "target_path"));

        var requiredTransparentTargets = manifestTargets
            .Concat(new[]
            {
                "EZMicroBalance/images/powers/lotha_verdict.png",
                "EZMicroBalance/images/ascension/firemarked_elite_indicator.png",
                "EZMicroBalance/images/ascension/firemark_might_indicator.png",
                "EZMicroBalance/images/ascension/firemark_giant_indicator.png",
                "EZMicroBalance/images/ascension/firemark_forge_armor_indicator.png",
                "EZMicroBalance/images/ascension/firemark_constant_heal_indicator.png",
                "EZMicroBalance/images/ascension/banner_room_indicator.png",
                "EZMicroBalance/images/ascension/banner_vanguard_indicator.png",
                "EZMicroBalance/images/ascension/banner_shield_formation_indicator.png",
                "EZMicroBalance/images/ascension/banner_bounty_indicator.png",
                "EZMicroBalance/images/ascension/boss_seal_indicator.png"
            })
            .Distinct(StringComparer.Ordinal);

        foreach (var targetPath in requiredTransparentTargets)
        {
            var (hasTransparentPixel, hasVisiblePixel) = ReadPngAlphaCoverage(RepoPath(targetPath.Split('/')));
            Assert.True(hasTransparentPixel, $"Small UI art must not ship with an opaque square background: {targetPath}");
            Assert.True(hasVisiblePixel, $"Small UI art appears fully transparent: {targetPath}");
        }
    }

    private static (bool HasTransparentPixel, bool HasVisiblePixel) ReadPngAlphaCoverage(string path)
    {
        var bytes = ReadPngBytes(path);
        Assert.True(bytes.Length >= 33, $"PNG too small to contain IHDR: {path}");

        var (width, height) = ReadPngDimensions(path);
        var bitDepth = bytes[24];
        var colorType = bytes[25];
        var interlace = bytes[28];
        Assert.Equal(8, bitDepth);
        Assert.Equal(6, colorType);
        Assert.Equal(0, interlace);

        using var compressed = new MemoryStream();
        var offset = 8;
        while (offset < bytes.Length)
        {
            var length = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(offset, 4));
            var type = Encoding.ASCII.GetString(bytes, offset + 4, 4);
            if (type == "IDAT")
            {
                compressed.Write(bytes, offset + 8, length);
            }

            offset += 12 + length;
        }

        compressed.Position = 0;
        using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
        using var raw = new MemoryStream();
        zlib.CopyTo(raw);
        var data = raw.ToArray();
        var stride = width * 4;
        var previous = new byte[stride];
        var current = new byte[stride];
        var sourceOffset = 0;
        var hasTransparentPixel = false;
        var hasVisiblePixel = false;

        for (var y = 0; y < height; y++)
        {
            var filter = data[sourceOffset++];
            Array.Copy(data, sourceOffset, current, 0, stride);
            sourceOffset += stride;
            UnfilterRow(current, previous, filter, bytesPerPixel: 4);

            for (var x = 3; x < current.Length; x += 4)
            {
                if (current[x] == 0)
                {
                    hasTransparentPixel = true;
                }
                else
                {
                    hasVisiblePixel = true;
                }
            }

            (previous, current) = (current, previous);
        }

        return (hasTransparentPixel, hasVisiblePixel);
    }

    private static void UnfilterRow(byte[] row, byte[] previous, int filter, int bytesPerPixel)
    {
        for (var i = 0; i < row.Length; i++)
        {
            var left = i >= bytesPerPixel ? row[i - bytesPerPixel] : 0;
            var up = previous[i];
            var upLeft = i >= bytesPerPixel ? previous[i - bytesPerPixel] : 0;
            var predictor = filter switch
            {
                0 => 0,
                1 => left,
                2 => up,
                3 => (left + up) / 2,
                4 => Paeth(left, up, upLeft),
                _ => throw new InvalidDataException($"Unsupported PNG filter: {filter}")
            };

            row[i] = unchecked((byte)(row[i] + predictor));
        }
    }

    private static int Paeth(int left, int up, int upLeft)
    {
        var p = left + up - upLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - up);
        var pc = Math.Abs(p - upLeft);
        return pa <= pb && pa <= pc ? left : pb <= pc ? up : upLeft;
    }
}
