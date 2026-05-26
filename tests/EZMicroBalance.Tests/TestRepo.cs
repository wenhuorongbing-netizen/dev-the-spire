global using static EZMicroBalance.Tests.TestRepo;

using System.Buffers.Binary;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

internal static class TestRepo
{
    private static readonly Lazy<string> LazyRoot = new(FindRepoRoot);

    internal static string Root => LazyRoot.Value;

    internal static readonly string[] CurrentFacingDocs =
    [
        "README.md",
        "docs/dev-environment.md",
        "docs/private-beta-verification-handoff.md",
        "docs/private-beta-release-completion-audit.md",
        "docs/test-plan.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    internal static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    internal static string ReadSharedText(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    internal static string ReadSourceTree(params string[] parts)
    {
        var root = RepoPath(parts);
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    internal static string ReadAllTestSource()
    {
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    internal static string ReadBossSealMarkerPowerSources()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "BossSealMarkerPower.cs",
                "BossSealMarkerPowers.HolyDaze.cs",
                "BossSealMarkerPowers.MartyrOath.cs",
                "BossSealMarkerPowers.InkReturn.cs",
                "BossSealMarkerPowers.StartledShell.cs",
                "BossSealMarkerPowers.SoulTide.cs",
                "BossSealMarkerPowers.BoilingCritical.cs",
                "BossSealMarkerPowers.MisalignedShell.cs",
                "BossSealMarkerPowers.MarginalNote.cs",
                "BossSealMarkerPowers.StruggleBait.cs",
                "BossSealMarkerPowers.AeonglassHourglass.cs",
                "BossSealMarkerPowers.ChosenDecree.cs",
                "BossSealMarkerPowers.ResidualSample.cs"
            }.Select(fileName => ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", fileName)));
    }

    internal static string ReadAeonglassHourglassCombatSources()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "AscensionCombatModifierService.BossSeals.AeonglassHourglass.cs",
                "AscensionCombatModifierService.BossSeals.AeonglassHourglass.LaserEcho.cs",
                "AscensionCombatModifierService.BossSeals.AeonglassHourglass.State.cs"
            }.Select(fileName => ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", fileName)));
    }

    internal static string ReadChosenDecreeCombatSources()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "AscensionCombatModifierService.BossSeals.ChosenDecree.cs",
                "AscensionCombatModifierService.BossSeals.ChosenDecree.Cards.cs"
            }.Select(fileName => ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", fileName)));
    }

    internal static string ReadResidualSampleCombatSources()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "AscensionCombatModifierService.BossSeals.ResidualSample.cs",
                "AscensionCombatModifierService.BossSeals.ResidualSample.Notice.cs"
            }.Select(fileName => ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", fileName)));
    }

    internal static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { Root }.Concat(parts).ToArray());
    }

    internal static string GamePath(params string[] parts)
    {
        var root = Environment.GetEnvironmentVariable("STS2_PATH");
        if (string.IsNullOrWhiteSpace(root))
        {
            root = @"D:\Steam\steamapps\common\Slay the Spire 2";
        }

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }

    internal static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(Root, path).Replace('\\', '/');
    }

    internal static string AssertRepoFileExists(params string[] parts)
    {
        var path = RepoPath(parts);
        Assert.True(File.Exists(path), $"Missing repository file: {ToRepoRelativePath(path)}");
        return path;
    }

    internal static string AssertRepoDirectoryExists(params string[] parts)
    {
        var path = RepoPath(parts);
        Assert.True(Directory.Exists(path), $"Missing repository directory: {ToRepoRelativePath(path)}");
        return path;
    }

    internal static void AssertRepoPathDoesNotExist(params string[] parts)
    {
        var path = RepoPath(parts);
        Assert.False(File.Exists(path) || Directory.Exists(path), $"Repository path should not exist: {ToRepoRelativePath(path)}");
    }

    internal static void AssertDirectoryContainsOnlyFiles(string directory, IEnumerable<string> expectedRelativeFiles)
    {
        Assert.True(Directory.Exists(directory), $"Missing directory: {directory}");
        var entries = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(directory, path).Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedRelativeFiles.OrderBy(file => file, StringComparer.Ordinal), entries);
    }

    internal static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }

    internal static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    internal static void AssertNoMojibake(string value, params string[] extraFragments)
    {
        foreach (var fragment in new[] { "\uFFFD" }.Concat(extraFragments))
        {
            Assert.DoesNotContain(fragment, value, StringComparison.Ordinal);
        }
    }

    internal static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.String, property.Value.ValueKind);
            map.Add(property.Name, property.Value.GetString() ?? string.Empty);
        }

        return map;
    }

    internal static void AssertLocalizedKeys(
        IEnumerable<string> keys,
        IReadOnlyDictionary<string, string> english,
        IReadOnlyDictionary<string, string> simplifiedChinese,
        string context,
        Action<string>? assertValue = null)
    {
        foreach (var key in keys)
        {
            Assert.True(english.TryGetValue(key, out var englishValue), $"Missing English {context}: {key}");
            Assert.True(simplifiedChinese.TryGetValue(key, out var zhsValue), $"Missing zhs {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(englishValue), $"Empty English {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs {context}: {key}");
            assertValue?.Invoke(englishValue);
            assertValue?.Invoke(zhsValue);
        }
    }

    internal static IEnumerable<(string key, string value)> JsonStringValues(JsonElement element, string keyPrefix = "")
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(keyPrefix)
                    ? property.Name
                    : $"{keyPrefix}.{property.Name}";

                foreach (var value in JsonStringValues(property.Value, key))
                {
                    yield return value;
                }
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                foreach (var value in JsonStringValues(item, $"{keyPrefix}[{index}]"))
                {
                    yield return value;
                }

                index++;
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            yield return (keyPrefix, element.GetString() ?? string.Empty);
        }
    }

    internal static string ManifestVersion()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        return document.RootElement.GetProperty("version").GetString() ?? throw new InvalidOperationException("Missing manifest version.");
    }

    internal static string CurrentPackageName()
    {
        return $"SpirePlus-{ManifestVersion()}";
    }

    internal static string CurrentPackageZipRelativePath()
    {
        return $"publish\\{CurrentPackageName()}.zip";
    }

    internal static string CurrentPackageZipPath()
    {
        return RepoPath("publish", $"{CurrentPackageName()}.zip");
    }

    internal static string CurrentPackageZipSha256()
    {
        return Sha256(CurrentPackageZipPath());
    }

    internal static string CurrentPackageArtifactRelativePath(string fileName)
    {
        return $"publish\\{CurrentPackageName()}\\EZMicroBalance\\{fileName}";
    }

    internal static IReadOnlyDictionary<string, string> CurrentPackageHashesFromIssues()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in Regex.Matches(
                     issues,
                     @"^\|\s*(?<name>ZIP|DLL|PCK|Manifest|README_INSTALL)\s*\|\s*`(?<hash>[A-F0-9]{64})`\s*\|",
                     RegexOptions.CultureInvariant | RegexOptions.Multiline))
        {
            hashes[match.Groups["name"].Value] = match.Groups["hash"].Value;
        }

        foreach (var required in new[] { "ZIP", "DLL", "PCK", "Manifest", "README_INSTALL" })
        {
            Assert.True(hashes.ContainsKey(required), $"Missing current package hash row in docs/issues.md: {required}");
        }

        return hashes;
    }

    internal static string ReadCurrentFacingDocs(IEnumerable<string> paths)
    {
        return string.Join(Environment.NewLine, paths.Select(path => ReadRepoText(path.Split('/'))));
    }

    internal static string ReadCurrentFacingDocs(params string[] paths)
    {
        return ReadCurrentFacingDocs((IEnumerable<string>)paths);
    }

    internal static string SliceFrom(string source, string startMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");
        return source[start..];
    }

    internal static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");

        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end >= 0, $"Missing end marker: {endMarker}");

        return source[start..end];
    }

    internal static void AssertBefore(string source, string first, string second)
    {
        var firstIndex = source.IndexOf(first, StringComparison.Ordinal);
        var secondIndex = source.IndexOf(second, StringComparison.Ordinal);

        Assert.True(firstIndex >= 0, $"Missing first marker: {first}");
        Assert.True(secondIndex >= 0, $"Missing second marker: {second}");
        Assert.True(firstIndex < secondIndex, $"Expected '{first}' to appear before '{second}'.");
    }

    internal static int CountOccurrences(string source, string snippet)
    {
        var count = 0;
        var index = 0;

        while ((index = source.IndexOf(snippet, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += snippet.Length;
        }

        return count;
    }

    internal static byte[] ReadZipBytes(ZipArchive archive, string entryName)
    {
        var entry = archive.Entries.FirstOrDefault(candidate =>
            candidate.FullName.Replace('\\', '/').Equals(entryName, StringComparison.Ordinal));
        Assert.NotNull(entry);

        using var stream = entry.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    internal static string ReadZipText(ZipArchive archive, string entryName)
    {
        return Encoding.UTF8.GetString(ReadZipBytes(archive, entryName));
    }

    internal static IReadOnlyList<string> ReadPckDirectory(string path)
    {
        Assert.True(File.Exists(path), $"Missing PCK: {path}");
        return ReadPckDirectory(File.ReadAllBytes(path));
    }

    internal static IReadOnlyList<string> ReadPckDirectory(byte[] bytes)
    {
        var directoryOffset = (int)BitConverter.ToUInt64(bytes, 0x20);
        var count = (int)BitConverter.ToUInt32(bytes, directoryOffset);
        var offset = directoryOffset + 4;
        var entries = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var length = (int)BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            entries.Add(Encoding.UTF8.GetString(bytes, offset, length).TrimEnd('\0'));
            offset += length;
            offset += 8 + 8 + 16 + 4;
        }

        return entries;
    }

    internal static string Sha256(string path)
    {
        Assert.True(File.Exists(path), $"Missing file to hash: {path}");
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    internal static string Sha256(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    internal static string NormalizeJson(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
        return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = false });
    }

    internal static byte[] ReadPngBytes(string path)
    {
        Assert.True(File.Exists(path), $"Missing PNG: {path}");
        var bytes = File.ReadAllBytes(path);
        Assert.True(bytes.Length >= 24, $"PNG file is too short: {path}");
        ReadOnlySpan<byte> pngSignature = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A];
        Assert.True(bytes.AsSpan(0, 8).SequenceEqual(pngSignature), $"Not a PNG file: {path}");
        return bytes;
    }

    internal static (int Width, int Height) ReadPngDimensions(string path)
    {
        var bytes = ReadPngBytes(path);
        return (
            BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4)),
            BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4)));
    }

    internal static void AssertSmallUiPngHasAlpha(string path, string message)
    {
        var bytes = ReadPngBytes(path);
        Assert.True(bytes.Length >= 33, $"PNG too small to contain IHDR: {path}");

        var (width, height) = ReadPngDimensions(path);
        var colorType = bytes[25];

        Assert.True(width >= 96 && height >= 96, message);
        Assert.Equal(6, colorType);
    }

    internal static Exception? Unwrap(Exception? exception)
    {
        return exception is TargetInvocationException targetInvocationException
            ? targetInvocationException.InnerException ?? targetInvocationException
            : exception;
    }

    internal static string[] ParseExportFiles(string exportPreset)
    {
        var match = Regex.Match(exportPreset, @"export_files=PackedStringArray\((?<files>[^)]*)\)");
        Assert.True(match.Success, "Could not find export_files in export_presets.cfg.");

        return Regex.Matches(match.Groups["files"].Value, @"""(?<path>[^""]+)""")
            .Cast<Match>()
            .Select(match => match.Groups["path"].Value)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    internal static bool IsActiveExportResource(string path)
    {
        var relativePath = NormalizeRepoRelativePath(path);
        return IsActiveReleaseResource(relativePath) &&
            (Path.GetExtension(relativePath) is ".json" or ".png" or ".txt" or ".tscn");
    }

    internal static bool IsActiveReleaseResource(string path)
    {
        var relativePath = NormalizeRepoRelativePath(path);
        return relativePath.StartsWith("EZMicroBalance/", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png.import", StringComparison.Ordinal);
    }

    private static string NormalizeRepoRelativePath(string path)
    {
        var relativePath = Path.IsPathRooted(path)
            ? ToRepoRelativePath(path)
            : path;

        return relativePath.Replace('\\', '/');
    }
}
