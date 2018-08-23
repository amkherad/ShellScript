using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class VariableDefinitionStatementTranspilerBase : IPlatformStatementTranspiler
    {
        public virtual Type StatementType => typeof(VariableDefinitionStatement);
        
        public virtual bool CanInline(Context context, Scope scope, IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.DefaultValue is EvaluationStatement defaultValue)
            {
                return EvaluationStatementTranspilerBase.CanInlineEvaluation(context, scope, defaultValue);
            }

            return true;
        }

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            var variableName = varDefStt.Name;
            
            if (scope.IsVariableExists(variableName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(variableName);
                return false;
            }

            var regex = Lexer.ValidIdentifierName.Match(variableName);
            if (!regex.Success) // Unlikely to happen.
            {
                message = InvalidIdentifierNameCompilerException.CreateMessage(variableName);
                return false;
            }

            message = default;
            return true;
        }

        public abstract void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement);

        public abstract void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement);
    }
}