using System;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
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