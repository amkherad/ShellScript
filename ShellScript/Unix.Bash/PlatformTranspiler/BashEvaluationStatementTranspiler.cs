using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashEvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        private const string NumericFormat = "0.#############################";

        private static string GetConstantAsString(ConstantValueStatement constantValueStatement)
        {
            if (constantValueStatement.DataType == DataTypes.String)
            {
                return BashTranspilerHelpers.StandardizeString(constantValueStatement.Value, true);
            }
            else
            {
                return constantValueStatement.Value;
            }
        }


        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashStringConcatenationExpressionBuilder.Instance
                : BashDefaultExpressionBuilder.Instance;
            
            var (dataType, expression) =
                expressionBuilder.CreateExpression(context, scope, nonInlinePartWriter, evalStt);
            
            writer.Write(expression);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, IStatement statement)
        {
        }

        public override string PinEvaluationToVariable(Context context, Scope scope, TextWriter pinCodeWriter, EvaluationStatement statement)
        {
            throw new NotImplementedException();
        }
    }
}