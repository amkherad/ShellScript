using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using LambdaExpression = System.Linq.Expressions.LambdaExpression;

namespace ShellScript.CommandLine
{
    public class CompileCommand : ICommand
    {
        public string Name => "Compile";

        public bool CanHandle(CommandContext command)
        {
            if (command.IsCommand("compile"))
            {
                return true;
            }

            return false;
        }

        public ResultCodes Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            CommandContext context)
        {
            var inputFile = context.GetToken(0);
            var outputFile = context.GetToken(1);
            var platform = context.GetToken(2);

            if (inputFile == null)
            {
                errorWriter.WriteLine("Source file is not specified.");
                return ResultCodes.Failure;
            }

            if (outputFile == null)
            {
                errorWriter.WriteLine("Output file is not specified.");
                return ResultCodes.Failure;
            }

            if (platform == null)
            {
                errorWriter.WriteLine("Platform is not specified.");
                return ResultCodes.Failure;
            }

            var compiler = new Compiler();

            var flags = CompilerFlags.CreateDefault();

            foreach (var sw in _switches)
            {
                _setFlag(context, flags, sw.Value.Item1, sw.Key);
            }

            var result = compiler.CompileFromSource(
                errorWriter,
                warningWriter,
                logWriter,
                inputFile,
                outputFile,
                platform,
                flags
            );

            if (result.Successful)
            {
                outputWriter.WriteLine("Compilation finished successfully.");
                return ResultCodes.Successful;
            }

            throw result.Exception;
        }

        private void _setFlag(CommandContext context, CompilerFlags flags,
            Expression<Func<CompilerFlags, object>> prop, string switchName)
        {
            Switch s;

            if ((s = context.GetSwitch(switchName)) != null)
            {
                s.AssertValue();

                var lambda = prop as LambdaExpression;
                var memberExpr = lambda.Body as MemberExpression;
                if (memberExpr == null) throw new InvalidOperationException();
                var propName = memberExpr.Member.Name;

                var propInfo = flags.GetType().GetProperty(propName);

                if (propInfo.PropertyType == typeof(bool))
                {
                    StatementHelpers.TryParseBooleanFromString(s.Value, out var value);
                    propInfo.SetValue(flags, value);
                }
                else
                {
                    propInfo.SetValue(flags, Convert.ChangeType(s.Value, propInfo.PropertyType));
                }
            }
        }

        private static Dictionary<string, (Expression<Func<CompilerFlags, object>>, string)> _switches =
            new Dictionary<string, (Expression<Func<CompilerFlags, object>>, string)>
            {
                {
                    "echo-dev",
                    (
                        x => x.ExplicitEchoStream,
                        "Specifies the explicit device for standard output, default is /dev/tty."
                    )
                },

                {
                    "default-echo-dev",
                    (
                        x => x.DefaultExplicitEchoStream,
                        "Specifies the default explicit device for standard output, if not changed, /dev/tty will be used."
                    )
                },

                {
                    "use-comment",
                    (
                        x => x.UseComments,
                        "Determines whether meta-comments should be used in output code."
                    )
                },
            };

        public Dictionary<string, string> SwitchesHelp { get; } =
            _switches.ToDictionary(kv => kv.Key, kv => kv.Value.Item2);
    }
}