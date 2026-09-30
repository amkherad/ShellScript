using System;

namespace ShellScript.Testing
{
    /// <summary>
    /// Which .shellscript paths are intended as standalone compile/run targets (not fragments or build output).
    /// </summary>
    public static class ShellScriptSourceFilters
    {
        public static bool IsStandaloneEntry(string scriptPath)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                return false;
            }

            var normalized = scriptPath.Replace('\\', '/');

            if (normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (normalized.Contains("/parts/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (normalized.Contains("/ShellScriptResources/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (normalized.EndsWith("/ShellScript.Luncher/shellscript.shellscript",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
