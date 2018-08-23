using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class EvaluationStatementTranspilerBase : IPlatformEvaluationStatementTranspiler
    {
        public virtual Type StatementType => typeof(EvaluationStatement);

        
        public static bool CanInlineEvaluation(Context context, Scope scope, EvaluationStatement statement)
        {
            return !statement.TraverseTreeContains<FunctionCallStatement>();
        }
        
        public virtual bool CanInline(Context context, Scope scope, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            return !evalStt.TraverseTreeContains<FunctionCallStatement>();
        }

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            message = default;
            return true;
        }

        public abstract void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement);

        public abstract void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement);

        public abstract string PinEvaluationToInline(Context context, Scope scope, TextWriter pinCodeWriter,
            EvaluationStatement statement);
    }
}