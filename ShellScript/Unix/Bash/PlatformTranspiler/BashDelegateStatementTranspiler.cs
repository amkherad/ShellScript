using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashDelegateStatementTranspiler : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(DelegateStatement);

        public bool CanInline(Context context, Scope scope, IStatement statement)
            => false;

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            message = null;
            return true;
        }

        public void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter,
            IStatement statement)
        {
            throw new NotImplementedException();
        }

        public void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is DelegateStatement delegateStatement)) throw new InvalidOperationException();

            scope.ReserveNewPrototype(
                new FunctionInfo(
                    delegateStatement.ReturnTypeDescriptor,
                    delegateStatement.Name,
                    null,
                    null,
                    false,
                    delegateStatement.Parameters,
                    null,
                    false)
            );
            
            BashTranspilerHelpers.WriteComment(writer, $"delegate {delegateStatement.ReturnTypeDescriptor} {delegateStatement.Name}");
        }
    }
}