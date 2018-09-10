using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
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

            var regex = Lexer.ValidIdentifierName.Match(variableName);
            if (!regex.Success) // Unlikely to happen.
            {
                message = InvalidIdentifierNameCompilerException.CreateMessage(variableName, varDefStt.Info);
                return false;
            }

            return base.Validate(context, scope, statement, out message);
        }
    }
}