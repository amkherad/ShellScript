using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.CommandLine;
using ShellScript.Core;
using ShellScript.Core.Language;
using ShellScript.Unix.Bash;

namespace ShellScript
{
    public enum ResultCodes
    {
        Successful = 0,
        Failure = 1,
    }

    public class Program
    {
        public const string CompatibleOutputSwitchName = "compatible";
        public const string DuplexErrorOutputSwitchName = "duplex-error";
        
        public static HelpCommand HelpCommand { get; } = new HelpCommand();

        public static ICollection<ICommand> AvailableCommands { get; } = new List<ICommand>
        {
            new CompileCommand(),
            new PlatformsCommand(),
            new ExecuteCommand(),
            new VersionInfoCommand(),
            new DaemonCommand(),
            new LanguageServerInterfaceCommand(),
            
            
            HelpCommand,
        };
        
        static int Main(string[] args)
        {
            var outputWriter = Console.Out; //new StreamWriter(Console.OpenStandardError());

            TextWriter errorWriter = new ColoredWriter(Console.Error, ConsoleColor.Red);
            
            var warningWriter = new ColoredWriter(Console.Out, ConsoleColor.Yellow);
            var logWriter = new ColoredWriter(Console.Out, ConsoleColor.White);

            CommandContext commandContext = null;

            try
            {
                commandContext = CommandContext.Parse(args);

                if (commandContext.AnySwitch(DuplexErrorOutputSwitchName))
                {
                    var errorStdOutWriter = new ColoredWriter(Console.Out, ConsoleColor.Red);
                    var errorErrOutWriter = Console.Error;
                    errorWriter = new MultiStreamWriter(errorStdOutWriter, errorErrOutWriter);
                }
                
                Platforms.AddPlatform(new UnixBashPlatform());

                foreach (var command in AvailableCommands)
                {
                    if (command.CanHandle(commandContext))
                    {
                        if (commandContext.AnySwitch("help") && command != HelpCommand)
                        {
                            HelpCommand.ShowHelp(outputWriter, command);

                            return (int) ResultCodes.Successful;
                        }
                        
                        return (int) command.Execute(outputWriter, errorWriter, warningWriter, logWriter,
                            commandContext);
                    }
                }
            }
            catch (Exception ex)
            {
                if (commandContext == null)
                {
                    errorWriter.WriteLine(ex.Message);
                    return (int) ResultCodes.Failure;
                }

                Action<TextWriter, TextWriter, TextWriter, TextWriter, Exception> writer = null;

                if (commandContext.AnySwitch("verbose"))
                {
                    writer = WriteCompilerDebugInfo;
                }
                else if (commandContext.AnySwitch(CompatibleOutputSwitchName))
                {
                    writer = WriteCleanErrorInfo;
                }
                
                if (writer == null)
                {
                    errorWriter.WriteLine(ex.Message);
                }
                else
                {
                    writer(outputWriter, errorWriter, warningWriter, logWriter, ex);
                }

                return (int) ResultCodes.Failure;
            }

            errorWriter.WriteLine(DesignGuidelines.ErrorOutputHead + " Invalid command passed. ({0})",
                args.Length > 0 ? args[0] : "null");
            return (int) ResultCodes.Failure;
        }

        static void WriteCompilerDebugInfo(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            Exception ex)
        {
            errorWriter.WriteLine(ex.ToString());
        }
        
        static void WriteCleanErrorInfo(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            Exception ex)
        {
            errorWriter.Write("Error: ");
            errorWriter.WriteLine(ex.Message.Replace(Environment.NewLine, "\\r\\n"));
        }
    }
}