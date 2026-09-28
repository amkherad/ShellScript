using System;

namespace ShellScript.Core.Language
{
    public static class PlatformDefaults
    {
        public const string UnixBashPlatformName = "Unix-Bash";
        public const string WindowsBatchPlatformName = "Windows-Batch";
        public const string WindowsPowerShellPlatformName = "Windows-PowerShell";

        /// <summary>
        /// Default compile/run target: Windows Batch on Windows, Unix Bash elsewhere.
        /// </summary>
        public static string DefaultRunPlatformName =>
            IsWindows() ? WindowsBatchPlatformName : UnixBashPlatformName;

        public static bool IsWindows()
        {
            if (OperatingSystem.IsWindows())
            {
                return true;
            }

            var platform = Environment.OSVersion.Platform;
            return platform == PlatformID.Win32NT || platform == PlatformID.Win32S ||
                   platform == PlatformID.Win32Windows || platform == PlatformID.WinCE;
        }
    }
}
