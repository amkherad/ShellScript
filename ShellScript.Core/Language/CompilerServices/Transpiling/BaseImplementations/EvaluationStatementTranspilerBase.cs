using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class EvaluationStatementTranspilerBase : IPlatformEvaluationStatementTranspiler
    {
        public virtual Type StatementType => typeof(EvaluationStatement);


        public static bool CanInlineEvaluation(Context context, Scope scope, EvaluationStatement statement)
        {
            return true;
            //return !statement.TreeContains<FunctionCallStatement>();
        }

        public virtual bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return true;
            //if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            //return !evalStt.TreeContains<FunctionCallStatement>();
        }

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            VariableAccessStatement variable = null;

            //check for all variables defined.
            if (evalStt.TreeAny(stt =>
            {
                if (stt is VariableAccessStatement varAccessStt)
                {
                    variable = varAccessStt;
                    return !scope.IsVariableExists(varAccessStt.VariableName);
                }

                return false;
            }))
            {
                message = IdentifierNotFoundCompilerException.CreateMessage(variable.VariableName, variable.Info);
                return false;
            }

            message = default;
            return true;
        }

        public abstract void WriteInline(Context context, Scope scope, TextWriter writer,
            TextWriter nonInlinePartWriter, IStatement statement);

        public abstract void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement);

        public abstract string PinEvaluationToInline(Context context, Scope scope, TextWriter pinCodeWriter,
            EvaluationStatement statement);


        public static string _createExpression(Context context, Scope scope, EvaluationStatement statement,
            string variableFormat, IDictionary<IStatement, string> replacements, StringWriter nonInlinePartWriter)
        {
            if (replacements != null && replacements.TryGetValue(statement, out var value))
            {
                return value;
            }

            switch (statement)
            {
                case ConstantValueStatement constant:
                {
                    if (constant.DataType == DataTypes.String)
                    {
                        //USE STRING CONCATENATION FOR ENTIRE EXPRESSION
                        
                        using (var strWriter = new StringWriter())
                        {
                            context.GetTranspiler<IPlatformStringTranspiler>()
                                .WriteStringInline(context, scope, strWriter, nonInlinePartWriter, constant.Value);

                            return strWriter.ToString();
                        }
                    }

                    return constant.Value;
                }
                case VariableAccessStatement variable:
                {
                    return string.Format(variableFormat, variable.VariableName);
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    //arithmeticEvaluationStatement.

                    break;
                }
            }

            return null;
        }

        public static string CreateExpression(Context context, Scope scope, EvaluationStatement statement,
            string variableFormat, IDictionary<IStatement, string> replacements, out StringBuilder nonInlinePart)
        {
            nonInlinePart = new StringBuilder(30);
            using (var textWriter = new StringWriter(nonInlinePart))
            {
                return _createExpression(context, scope, statement, variableFormat, replacements, textWriter);
            }
        }
    }
}