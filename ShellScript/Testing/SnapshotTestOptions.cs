namespace ShellScript.Testing
{
    public sealed class SnapshotTestOptions
    {
        public string PlatformName { get; set; } = ShellScriptRunner.DefaultPlatformName;

        public bool UpdateSnapshots { get; set; }

        public bool RequireSnapshotFile { get; set; } = true;

        public bool IncludeScriptsWithoutSnapshot { get; set; }
    }
}
