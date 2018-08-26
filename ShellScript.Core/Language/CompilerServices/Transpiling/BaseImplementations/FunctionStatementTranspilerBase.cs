using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class FunctionStatementTranspilerBase : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(FunctionStatement);
        
        public virtual bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public virtual bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            var functionName = funcDefStt.Name;
            
            if (scope.IsVariableExists(functionName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(functionName, funcDefStt.Info);
                return false;
            }
            

            //TODO: validate all paths must return a value
            message = null;
            return true;
        }

        public abstract void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter,
            IStatement statement);

        public abstract void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement);
    }
}