using System.IO;

namespace ShellScript.Testing
{
    public static class SnapshotPaths
    {
        public const string OutputSnapshotExtension = ".output.txt";

        public static string GetOutputSnapshotPath(string scriptPath)
        {
            var directory = Path.GetDirectoryName(scriptPath) ?? string.Empty;
            var baseName = Path.GetFileNameWithoutExtension(scriptPath);
            return Path.Combine(directory, baseName + OutputSnapshotExtension);
        }

        public static bool HasOutputSnapshot(string scriptPath) =>
            File.Exists(GetOutputSnapshotPath(scriptPath));
    }
}
