using System;
using System.Collections.Generic;
using System.IO;

namespace ShellScript.Testing
{
    public static class PathGlob
    {
        public static IEnumerable<string> Expand(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                yield break;
            }

            pattern = pattern.Replace('/', Path.DirectorySeparatorChar);

            if (!pattern.Contains("*"))
            {
                var fullPath = Path.GetFullPath(pattern);
                if (File.Exists(fullPath))
                {
                    yield return fullPath;
                }

                yield break;
            }

            var recursiveIndex = pattern.IndexOf("**", StringComparison.Ordinal);
            if (recursiveIndex >= 0)
            {
                var root = pattern.Substring(0, recursiveIndex).TrimEnd(Path.DirectorySeparatorChar);
                if (string.IsNullOrEmpty(root))
                {
                    root = Environment.CurrentDirectory;
                }
                else if (!Path.IsPathRooted(root))
                {
                    root = Path.GetFullPath(root);
                }

                var remainder = pattern.Substring(recursiveIndex + 2).TrimStart(Path.DirectorySeparatorChar);
                var filePattern = string.IsNullOrEmpty(remainder) ? "*" : Path.GetFileName(remainder);
                if (!Directory.Exists(root))
                {
                    yield break;
                }

                foreach (var file in Directory.EnumerateFiles(root, filePattern, SearchOption.AllDirectories))
                {
                    yield return Path.GetFullPath(file);
                }

                yield break;
            }

            var directory = Path.GetDirectoryName(pattern);
            if (string.IsNullOrEmpty(directory))
            {
                directory = Environment.CurrentDirectory;
            }
            else if (!Path.IsPathRooted(directory))
            {
                directory = Path.GetFullPath(directory);
            }

            var searchPattern = Path.GetFileName(pattern);
            if (!Directory.Exists(directory))
            {
                yield break;
            }

            foreach (var file in Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly))
            {
                yield return Path.GetFullPath(file);
            }
        }
    }
}
