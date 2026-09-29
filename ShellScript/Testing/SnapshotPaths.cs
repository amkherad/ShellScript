using System;
using System.IO;

namespace ShellScript.Testing
{
    public static class SnapshotPaths
    {
        public const string DefaultSnapshotPlatformSlug = "bash";

        public static string GetSnapshotPlatformSlug(string platformName)
        {
            if (string.IsNullOrWhiteSpace(platformName))
            {
                return DefaultSnapshotPlatformSlug;
            }

            if (platformName.Equals("Unix-Bash", StringComparison.OrdinalIgnoreCase))
            {
                return "bash";
            }

            if (platformName.Equals("Windows-Batch", StringComparison.OrdinalIgnoreCase))
            {
                return "batch";
            }

            if (platformName.Equals("Windows-PowerShell", StringComparison.OrdinalIgnoreCase))
            {
                return "powershell";
            }

            return platformName.Replace(' ', '-').ToLowerInvariant();
        }

        public static string GetOutputSnapshotFileName(string scriptBaseName, string platformName)
        {
            var slug = GetSnapshotPlatformSlug(platformName);
            return $"{scriptBaseName}.output.{slug}.txt";
        }

        public static string GetOutputSnapshotPath(string scriptPath, string platformName = null)
        {
            platformName = platformName ?? ShellScriptRunner.DefaultPlatformName;
            var directory = Path.GetDirectoryName(scriptPath) ?? string.Empty;
            var baseName = Path.GetFileNameWithoutExtension(scriptPath);
            return Path.Combine(directory, GetOutputSnapshotFileName(baseName, platformName));
        }

        public static bool HasOutputSnapshot(string scriptPath, string platformName = null) =>
            File.Exists(GetOutputSnapshotPath(scriptPath, platformName));
    }
}
