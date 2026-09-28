using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Windows.PowerShell.PlatformTranspiler
{
    public class PowerShellThrowStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(ThrowStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is ThrowStatement throwStatement))
            {
                throw new InvalidOperationException();
            }

            if (throwStatement.Exception != null)
            {
                if (scope.TryGetConstantInfo(throwStatement.Exception, out var constantInfo))
                {
                    writer.WriteLine($"echo \"{constantInfo.Value}\" >&2");
                }
                else if (scope.TryGetVariableInfo(throwStatement.Exception, out var variableInfo))
                {
                    writer.WriteLine($"echo \"${variableInfo.AccessName}\" >&2");
                }
                else
                {
                    writer.WriteLine($"echo \"{throwStatement.Exception.VariableName}\" >&2");
                }
            }

            writer.WriteLine("return 1");
            scope.IncrementStatements();
        }
    }
}
