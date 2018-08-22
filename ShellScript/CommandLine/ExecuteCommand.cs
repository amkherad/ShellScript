using System.IO;

namespace ShellScript.CommandLine
{
    public class ExecuteCommand : ICommand
    {
        public string Name => "Execute";
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("exec", "--exec");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, CommandContext context)
        {
            errorWriter.WriteLine("Not implemented.");
            return Program.Successful;
        }
    }
}