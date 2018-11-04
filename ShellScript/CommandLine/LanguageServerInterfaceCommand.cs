using System;
using System.Collections.Generic;
using System.IO;

namespace ShellScript.CommandLine
{
    public class LanguageServerInterfaceCommand : ICommand
    {
        public const string RequestSwitchName = "lsp-req";
        
        public string Name => "lsp";
        
        public Dictionary<string, string> SwitchesHelp { get; }
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("lsp");
        }

        public ResultCodes Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            var sw = context.GetSwitch(RequestSwitchName);
            if (sw == null)
            {
                throw new InvalidOperationException($"{RequestSwitchName} is required.");
            }

            switch (sw.Value)
            {
                case "docs.api.root": //documentation
                {
                    outputWriter.WriteLine("{");
                    outputWriter.WriteLine("ns: [\"Platform\", \"File\"]");
                    outputWriter.WriteLine("}");
                    break;
                }
                case "src.analyze": //source file
                {
                    var fileSwitch = context.GetSwitch("src.analyze");
                    if (fileSwitch == null || !fileSwitch.HaveValue)
                    {
                        throw new InvalidOperationException("src.analyze is required and must have a value.");
                    }

                    if (!File.Exists(fileSwitch.Value))
                    {
                        throw new InvalidOperationException($"File '{fileSwitch.Value}' does not exist.");
                    }
                    
                    var limitSwitch = context.GetSwitch("src.analyze.limit");
                    if (!int.TryParse(limitSwitch?.Value, out var limit))
                    {
                        limit = int.MaxValue;
                    }

                    outputWriter.WriteLine("OK");
                    break;
                }
            }
            
            return ResultCodes.Successful;
        }
    }
}