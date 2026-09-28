namespace ShellScript.Testing
{
    public sealed class ScriptRunResult
    {
        public int ExitCode { get; }
        public string StandardOutput { get; }
        public string StandardError { get; }

        public ScriptRunResult(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput ?? string.Empty;
            StandardError = standardError ?? string.Empty;
        }
    }
}
