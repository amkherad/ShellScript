using System;
using System.IO;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashSwitchCaseStatementTranspiler : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(SwitchCaseStatement);
        
        public bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            throw new NotImplementedException();
        }

        public void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, TextWriter nonInlinePartWriter,
            IStatement statement)
        {
            throw new NotSupportedException();
        }

        public void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, IStatement statement)
        {
            throw new NotImplementedException();
            
            scope.IncrementStatements();
        }
    }
}