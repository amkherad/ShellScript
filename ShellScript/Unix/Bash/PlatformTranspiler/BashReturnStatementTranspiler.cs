using System;
using System.IO;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashReturnStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(ReturnStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter,
            IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is ReturnStatement returnStatement)) throw new InvalidOperationException();

            var returnResult = returnStatement.Result;

            if (returnResult != null)
            {
                var transpiler = context.GetEvaluationTranspilerForStatement(returnResult);
                var result = transpiler.GetExpression(context, scope, metaWriter, writer, null, returnResult);

                writer.Write("echo ");
                writer.Write(result.Expression);
                writer.WriteLine();
            }

            if (scope.StatementCount > 1 && scope.Type != ScopeType.MethodRoot)
            {
                writer.Write("return ");
                writer.Write(context.Flags.SuccessStatusCode);
                writer.WriteLine();
            }

            scope.IncrementStatements();
        }
    }
}