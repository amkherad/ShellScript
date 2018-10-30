using System;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
{
    public abstract class IfElseStatementTranspilerBase : StatementTranspilerBase
    {
        public override Type StatementType => typeof(IfElseStatement);
        
        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }
    }
}