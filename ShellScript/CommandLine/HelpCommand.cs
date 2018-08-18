using System.IO;
using System.Linq;

namespace ShellScript.CommandLine
{
    public class HelpCommand : ICommand
    {
        public string Name => "Help";
        
        public bool CanHandle(CommandContext command)
        {
            if (command.IsCommand("help"))
            {
                return true;
            }

            if (command.NoCommand && command.Switches.Any(x => x.Name == "-h"))
            {
                return true;
            }

            return command.IsEmpty;
        }

        public void Execute(TextWriter writer, CommandContext context)
        {
            writer.WriteLine("ShellScript by Ali Mousavi Kherad");
        }
    }
}