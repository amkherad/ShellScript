namespace ShellScript.Testing
{
    public sealed class SnapshotTestFailure
    {
        public string ScriptPath { get; }
        public string Message { get; }

        public SnapshotTestFailure(string scriptPath, string message)
        {
            ScriptPath = scriptPath;
            Message = message;
        }

        public override string ToString() => $"{ScriptPath}: {Message}";
    }
}
