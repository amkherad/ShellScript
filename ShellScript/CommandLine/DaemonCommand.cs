using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ShellScript.CommandLine
{
    public class DaemonCommand : ICommand
    {
        public string Name => "Daemon";
        
        public Dictionary<string, string> SwitchesHelp { get; }
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("daemon");
        }

        public ResultCodes Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            SpinWait.SpinUntil(() =>
            {
                
                
                return false;
            });
            
            return ResultCodes.Successful;
        }
    }
}