using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class TestInfrastructureGuardTests
{
    [Fact]
    public void LocalGameSourceGuardTestsAreExplicitlyOptIn()
    {
        var directLocalSourceReadNeedles = new[]
        {
            "ReadRepoText(\"" + "source code\"",
            "RepoPath(\"" + "source code\"",
            "AssertRepoFileExists(\"" + "source code\""
        };
        var localCoreReadNeedle = "ReadLocalCore" + "Text(";
        var directLocalSourceReads = new List<string>();
        var unguardedLocalCoreReads = new List<string>();

        foreach (var path in Directory
            .GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path => !Path.GetFileName(path).Equals("TestRepo.cs", StringComparison.Ordinal)))
        {
            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i++)
            {
                if (directLocalSourceReadNeedles.Any(needle => lines[i].Contains(needle, StringComparison.Ordinal)))
                {
                    directLocalSourceReads.Add($"{ToRepoRelativePath(path)}:{i + 1}");
                }

                if (!lines[i].Contains(localCoreReadNeedle, StringComparison.Ordinal))
                {
                    continue;
                }

                var methodLine = -1;
                var methodName = "(unknown)";
                for (var j = i; j >= 0; j--)
                {
                    var match = Regex.Match(lines[j], @"\bpublic\s+void\s+(?<name>[A-Za-z0-9_]+)\s*\(", RegexOptions.CultureInvariant);
                    if (match.Success)
                    {
                        methodLine = j;
                        methodName = match.Groups["name"].Value;
                        break;
                    }
                }

                var attributeStart = Math.Max(0, methodLine - 8);
                var hasLocalSourceFact = methodLine >= 0 &&
                    lines[attributeStart..methodLine].Any(line => line.Contains("[LocalSourceFact]", StringComparison.Ordinal));

                if (!hasLocalSourceFact)
                {
                    unguardedLocalCoreReads.Add($"{ToRepoRelativePath(path)}:{i + 1}:{methodName}");
                }
            }
        }

        Assert.True(
            directLocalSourceReads.Count == 0 && unguardedLocalCoreReads.Count == 0,
            "Tests that read ignored local game source must use LocalSourceFactAttribute and TestRepo.ReadLocalCoreText so normal test runs do not require `source code/**`." +
            Environment.NewLine +
            "Direct `source code` reads outside TestRepo.cs:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, directLocalSourceReads.OrderBy(offender => offender, StringComparer.Ordinal)) +
            Environment.NewLine +
            "ReadLocalCoreText calls missing [LocalSourceFact]:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, unguardedLocalCoreReads.OrderBy(offender => offender, StringComparer.Ordinal)));
    }

    [Fact]
    public void LocalGameSourceGuardTestsDoNotHideRepositoryCoverage()
    {
        var repositoryReadNeedles = new[]
        {
            "ReadRepo" + "Text(",
            "ReadSource" + "Tree(",
            "JsonString" + "Map(",
            "JsonString" + "Values(",
            "Json" + "Keys(",
            "ReadCurrentFacing" + "Docs(",
            "ReadShared" + "Text(",
            "AssertRepo",
            "Repo" + "Path(",
            "Game" + "Path(",
            "File.",
            "Directory.",
            "Sha" + "256(",
            "Manifest" + "Version(",
            "CurrentPackage",
            "ParseExport" + "Files(",
            "IsActiveExport" + "Resource(",
            "IsActiveRelease" + "Resource("
        };
        var offenders = new List<string>();

        foreach (var path in Directory
            .GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path =>
            {
                var fileName = Path.GetFileName(path);
                return !fileName.Equals("TestRepo.cs", StringComparison.Ordinal) &&
                       !fileName.Equals("TestInfrastructureGuardTests.cs", StringComparison.Ordinal);
            }))
        {
            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i++)
            {
                if (!Regex.IsMatch(lines[i], @"^\s*\[LocalSourceFact\]\s*$", RegexOptions.CultureInvariant))
                {
                    continue;
                }

                var methodLine = FindNextPublicVoidMethod(lines, i);
                if (methodLine < 0)
                {
                    continue;
                }

                var methodName = Regex.Match(lines[methodLine], @"\bpublic\s+void\s+(?<name>[A-Za-z0-9_]+)\s*\(", RegexOptions.CultureInvariant)
                    .Groups["name"]
                    .Value;
                var bodyLines = MethodBodyLines(lines, methodLine);
                var codeLines = bodyLines.Select(StripStringLiteralsAndLineComment).ToArray();
                var hasLocalCoreRead = codeLines.Any(line => line.Contains("ReadLocalCoreText(", StringComparison.Ordinal));
                var hits = bodyLines
                    .SelectMany((_, offset) => repositoryReadNeedles
                        .Where(needle => codeLines[offset].Contains(needle, StringComparison.Ordinal))
                        .Select(needle => $"{ToRepoRelativePath(path)}:{methodLine + offset + 1}:{methodName}:{needle}"))
                    .Concat(codeLines.SelectMany((line, offset) => Regex
                        .Matches(line, @"\bRead[A-Za-z0-9_]*\s*\(", RegexOptions.CultureInvariant)
                        .Where(match => !match.Value.StartsWith("ReadLocalCoreText", StringComparison.Ordinal))
                        .Select(match => $"{ToRepoRelativePath(path)}:{methodLine + offset + 1}:{methodName}:{match.Value}")))
                    .ToArray();
                if (!hasLocalCoreRead)
                {
                    offenders.Add($"{ToRepoRelativePath(path)}:{methodLine + 1}:{methodName}:missing ReadLocalCoreText");
                }

                offenders.AddRange(hits);
            }
        }

        Assert.True(
            offenders.Count == 0,
            "[LocalSourceFact] methods should only guard ignored local Core snapshot assumptions. " +
            "Keep repo, docs, localization, manifest, asset, and package assertions in normal [Fact] methods so normal test lanes retain that coverage:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders.OrderBy(offender => offender, StringComparer.Ordinal)));
    }

    private static int FindNextPublicVoidMethod(string[] lines, int start)
    {
        for (var i = start + 1; i < lines.Length; i++)
        {
            if (Regex.IsMatch(lines[i], @"\bpublic\s+void\s+[A-Za-z0-9_]+\s*\(", RegexOptions.CultureInvariant))
            {
                return i;
            }
        }

        return -1;
    }

    private static string[] MethodBodyLines(string[] lines, int methodLine)
    {
        var bodyStart = -1;
        for (var i = methodLine; i < lines.Length; i++)
        {
            if (StripStringLiteralsAndLineComment(lines[i]).IndexOf('{') >= 0)
            {
                bodyStart = i;
                break;
            }
        }

        if (bodyStart < 0)
        {
            return [];
        }

        var depth = 0;
        for (var i = bodyStart; i < lines.Length; i++)
        {
            var code = StripStringLiteralsAndLineComment(lines[i]);
            depth += code.Count(character => character == '{');
            depth -= code.Count(character => character == '}');
            if (i > bodyStart && depth == 0)
            {
                return lines[bodyStart..(i + 1)];
            }
        }

        return lines[bodyStart..];
    }

    private static string StripStringLiteralsAndLineComment(string line)
    {
        var result = new char[line.Length];
        var inString = false;
        var inVerbatimString = false;
        for (var i = 0; i < line.Length; i++)
        {
            var current = line[i];
            var next = i + 1 < line.Length ? line[i + 1] : '\0';

            if (!inString && current == '/' && next == '/')
            {
                break;
            }

            if (!inString && current == '@' && next == '"')
            {
                result[i] = ' ';
                result[i + 1] = ' ';
                inString = true;
                inVerbatimString = true;
                i++;
                continue;
            }

            if (!inString && current == '"')
            {
                inString = true;
                result[i] = ' ';
                continue;
            }

            if (inString)
            {
                if (inVerbatimString && current == '"' && next == '"')
                {
                    result[i] = ' ';
                    result[i + 1] = ' ';
                    i++;
                    continue;
                }

                if (current == '"' && (inVerbatimString || i == 0 || line[i - 1] != '\\'))
                {
                    inString = false;
                    inVerbatimString = false;
                }

                result[i] = ' ';
                continue;
            }

            result[i] = current;
        }

        return new string(result);
    }
}
