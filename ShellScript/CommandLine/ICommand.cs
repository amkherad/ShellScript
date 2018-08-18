using System.IO;

namespace ShellScript.CommandLine
{
    public interface ICommand
    {
        string Name { get; }

        bool CanHandle(CommandContext command);

        void Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            CommandContext context
        );
    }
}