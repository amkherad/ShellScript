using System;
using System.IO;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Lexing;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
{
    public abstract class VariableDefinitionStatementTranspilerBase : StatementTranspilerBase, IPlatformStatementTranspiler
    {
        public override Type StatementType => typeof(VariableDefinitionStatement);
        
        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.DefaultValue is EvaluationStatement defaultValue)
            {
                return EvaluationStatementTranspilerBase.CanInlineEvaluation(context, scope, defaultValue);
            }

            return true;
        }

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            var variableName = varDefStt.Name;
            
            if (scope.IsIdentifierExists(variableName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(variableName, varDefStt.Info);
                return false;
            }

            return base.Validate(context, scope, statement, out message);
        }
    }
}