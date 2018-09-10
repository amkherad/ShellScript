using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ShellScript.Core;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashEvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        private const string NumericFormat = "0.#############################";

        private class ExpressionTypes
        {
            public List<DataTypes> Types { get; set; }
            public bool ContainsFunctionCall { get; set; }
            public List<VariableAccessStatement> NonExistenceVariables { get; set; }

            public HashSet<Type> OperatorTypes { get; set; }
        }

        private static ExpressionTypes TraverseTreeAndGetExpressionTypes(Context context, Scope scope,
            IStatement statement)
        {
            var result = new ExpressionTypes();
            var types = new List<DataTypes>();
            var nonExistenceVariables = new List<VariableAccessStatement>();
            var operators = new HashSet<Type>();

            result.Types = types;
            result.NonExistenceVariables = nonExistenceVariables;
            result.OperatorTypes = operators;

            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    types.Add(constantValueStatement.DataType);
                    break;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out VariableInfo varInfo))
                    {
                        types.Add(varInfo.DataType);
                    }
                    else
                    {
                        nonExistenceVariables.Add(variableAccessStatement);
                    }

                    break;
                }
                case IOperator op:
                {
                    operators.Add(op.GetType());
                    break;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    types.Add(functionCallStatement.DataType);
                    break;
                }
                default:
                {
                    foreach (var child in statement.TraversableChildren)
                    {
                        if (child is IOperator)
                        {
                            operators.Add(child.GetType());
                            continue;
                        }

                        var childResult = TraverseTreeAndGetExpressionTypes(context, scope, child);

                        foreach (var t in childResult.Types)
                            types.Add(t);

                        foreach (var nev in childResult.NonExistenceVariables)
                            nonExistenceVariables.Add(nev);

                        foreach (var opType in childResult.OperatorTypes)
                            operators.Add(opType);

                        result.ContainsFunctionCall = result.ContainsFunctionCall || childResult.ContainsFunctionCall;
                    }

                    break;
                }
            }

            return result;
        }

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

        private static string PinExpressionToVariable(Context context, Scope scope, TextWriter writer,
            DataTypes dataTypes, string nameHint, string expression)
        {
            var variableName = scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"${{{expression}}}";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                context,
                scope,
                writer,
                variableName,
                expression
            );

            return $"${{{variableName}}}";
        }

        private static string PinFloatingPointExpressionToVariable(Context context, Scope scope, TextWriter writer,
            DataTypes dataTypes, string nameHint, string expression)
        {
            var variableName = scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"`awk \"BEGIN {{print {expression}}}\"`";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                context,
                scope,
                writer,
                variableName,
                expression
            );

            return $"${{{variableName}}}";
        }

        public interface IExpressionBuilder
        {
            (DataTypes, string) CreateExpression(Context context, Scope scope, TextWriter nonInlinePartWriter,
                IStatement statement);
        }

        public class ExpressionBuilder : IExpressionBuilder
        {
            public virtual (DataTypes, string) CreateExpression(Context context, Scope scope, TextWriter nonInlinePartWriter,
                IStatement statement)
            {
                switch (statement)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        if (constantValueStatement.IsNumeric())
                        {
                            return (constantValueStatement.DataType, constantValueStatement.Value);
                        }

                        if (constantValueStatement.IsString())
                        {
                            return (DataTypes.String,
                                BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true));
                        }

                        return (constantValueStatement.DataType, constantValueStatement.Value);
                    }
                    case VariableAccessStatement variableAccessStatement:
                    {
                        if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out var varInfo))
                        {
                            return (varInfo.DataType, $"${{{varInfo.Name}}}");
                        }

                        if (scope.TryGetConstantInfo(variableAccessStatement.VariableName, out var constInfo))
                        {
                            return (constInfo.DataType, $"${{{constInfo.Name}}}");
                        }

                        throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                            variableAccessStatement.Info);
                    }
                    case BitwiseEvaluationStatement bitwiseEvaluationStatement: //~ & |
                    {
                        if (bitwiseEvaluationStatement.Operator is BitwiseNotOperator)
                        {
                            var (dataType, exp) = CreateExpression(context, scope, nonInlinePartWriter,
                                bitwiseEvaluationStatement.Right);

                            if (!dataType.IsDecimal())
                            {
                                throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                    bitwiseEvaluationStatement.Info);
                            }

                            //can't have constant as the operand.
                            return (DataTypes.Decimal, $"~{exp}");
                        }

                        var left = bitwiseEvaluationStatement.Left;
                        var (leftDataType, leftExp) = CreateExpression(context, scope, nonInlinePartWriter, left);

                        if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
                        {
                            throw new InvalidStatementCompilerException(left, left.Info);
                        }

                        var right = bitwiseEvaluationStatement.Right;
                        var (rightDataType, rightExp) = CreateExpression(context, scope, nonInlinePartWriter, right);

                        if (!(rightDataType.IsDecimal() || rightDataType.IsBoolean()))
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        if (leftDataType != rightDataType)
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        if (leftDataType.IsNumericOrFloat() || rightDataType.IsNumericOrFloat())
                        {
                            return (leftDataType, PinFloatingPointExpressionToVariable(context, scope,
                                nonInlinePartWriter, leftDataType,
                                "bitwise",
                                $"{leftExp}{bitwiseEvaluationStatement.Operator}{rightExp}"));
                        }

                        return (leftDataType, $"{leftExp}{bitwiseEvaluationStatement.Operator}{rightExp}");
                    }
                    case LogicalEvaluationStatement logicalEvaluationStatement:
                    {
                        if (logicalEvaluationStatement.Operator is NotOperator)
                        {
                            var operand = logicalEvaluationStatement.Right;
                            var (dataType, exp) = CreateExpression(context, scope, nonInlinePartWriter, operand);

                            if (dataType.IsString())
                            {
                                throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                    logicalEvaluationStatement.Info);
                            }

                            if (dataType.IsNumericOrFloat())
                            {
                                return (dataType, PinFloatingPointExpressionToVariable(context, scope,
                                    nonInlinePartWriter, dataType, null, $"!{exp}"));
                            }

                            return (DataTypes.Boolean, $"!{exp}");
                        }

                        var left = logicalEvaluationStatement.Left;
                        var (leftDataType, leftExp) = CreateExpression(context, scope, nonInlinePartWriter, left);

                        if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
                        {
                            throw new InvalidStatementCompilerException(left, left.Info);
                        }

                        var right = logicalEvaluationStatement.Right;
                        var (rightDataType, rightExp) = CreateExpression(context, scope, nonInlinePartWriter, right);

                        if (!(rightDataType.IsDecimal() || rightDataType.IsBoolean()))
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        if (leftDataType != rightDataType)
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        if (leftDataType.IsNumericOrFloat() || rightDataType.IsNumericOrFloat())
                        {
                            return (DataTypes.Boolean, PinFloatingPointExpressionToVariable(context, scope,
                                nonInlinePartWriter, DataTypes.Boolean,
                                "logical",
                                $"{leftExp}{logicalEvaluationStatement.Operator}{rightExp}"));
                        }

                        return (DataTypes.Boolean, $"{leftExp}{logicalEvaluationStatement.Operator}{rightExp}");
                    }
                    case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                    {
                        var op = arithmeticEvaluationStatement.Operator;
                        if (op is IncrementOperator)
                        {
                            var operand = arithmeticEvaluationStatement.Left ?? arithmeticEvaluationStatement.Right;
                            if (!(operand is VariableAccessStatement))
                            {
                                var isError = true;
                                if (operand is FunctionCallStatement functionCallStatement)
                                {
                                    if (scope.TryGetFunctionInfo(functionCallStatement.FunctionName, out var funcInfo))
                                    {
                                        var inline = FunctionInfo.UnWrapInlinedStatement(context, scope, funcInfo);
                                        if (inline is EvaluationStatement evalStatement)
                                        {
                                            operand = evalStatement;
                                            isError = false;
                                        }
                                    }
                                }

                                if (isError)
                                {
                                    throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                        arithmeticEvaluationStatement.Info);
                                }
                            }

                            var (dt, exp) = CreateExpression(context, scope, nonInlinePartWriter, operand);

                            if (!dt.IsNumeric())
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }

                            exp = arithmeticEvaluationStatement.Left == null
                                ? $"++{exp}"
                                : $"{exp}++";
                            
                            if (dt.IsNumericOrFloat())
                            {
                                return (dt, PinFloatingPointExpressionToVariable(context, scope,
                                    nonInlinePartWriter, dt, null, exp));
                            }
                            
                            return (dt, exp);
                        }

                        if (op is DecrementOperator)
                        {
                            var operand = arithmeticEvaluationStatement.Left ?? arithmeticEvaluationStatement.Right;
                            if (!(operand is VariableAccessStatement))
                            {
                                var isError = true;
                                if (operand is FunctionCallStatement functionCallStatement)
                                {
                                    if (scope.TryGetFunctionInfo(functionCallStatement.FunctionName, out var funcInfo))
                                    {
                                        var inline = FunctionInfo.UnWrapInlinedStatement(context, scope, funcInfo);
                                        if (inline is EvaluationStatement evalStatement)
                                        {
                                            operand = evalStatement;
                                            isError = false;
                                        }
                                    }
                                }

                                if (isError)
                                {
                                    throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                        arithmeticEvaluationStatement.Info);
                                }
                            }

                            var (dt, exp) = CreateExpression(context, scope, nonInlinePartWriter, operand);

                            if (!dt.IsNumeric())
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }

                            exp = arithmeticEvaluationStatement.Left == null
                                ? $"--{exp}"
                                : $"{exp}--";
                            
                            if (dt.IsNumericOrFloat())
                            {
                                return (dt, PinFloatingPointExpressionToVariable(context, scope,
                                    nonInlinePartWriter, dt, null, exp));
                            }

                            return (dt, exp);
                        }

                        if (op is NegativeNumberOperator)
                        {
                            var operand = arithmeticEvaluationStatement.Right;

                            var (dt, exp) = CreateExpression(context, scope, nonInlinePartWriter, operand);

                            if (!dt.IsNumeric())
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }

                            exp = $"-{exp}";
                            
                            if (dt.IsNumericOrFloat())
                            {
                                return (dt, PinFloatingPointExpressionToVariable(context, scope,
                                    nonInlinePartWriter, dt, null, exp));
                            }
                            
                            return (dt, exp);
                        }

                        var left = arithmeticEvaluationStatement.Left;
                        var (leftDataType, leftExp) = CreateExpression(context, scope, nonInlinePartWriter, left);

                        var right = arithmeticEvaluationStatement.Right;
                        var (rightDataType, rightExp) = CreateExpression(context, scope, nonInlinePartWriter, right);

                        DataTypes dataType;
                        if (!(leftDataType.IsNumeric() && rightDataType.IsNumeric()))
                        {
                            /*var oneString = leftDataType.IsString() || rightDataType.IsString();
                            var oneDecimal = leftDataType.IsDecimal() || rightDataType.IsDecimal();
                            if (oneString && oneDecimal)
                            {
                                
                            }
                            else*/
                            if (leftDataType != rightDataType)
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }

                            if (leftDataType.IsString())
                            {
                                if (!(arithmeticEvaluationStatement.Operator is AdditionOperator))
                                {
                                    throw new InvalidOperatorForTypeCompilerException(
                                        arithmeticEvaluationStatement.Operator.GetType(),
                                        DataTypes.String,
                                        arithmeticEvaluationStatement.Info);
                                }
                            }

                            dataType = leftDataType;
                        }
                        else if (leftDataType.IsDecimal() && rightDataType.IsDecimal())
                        {
                            dataType = DataTypes.Decimal;
                        }
                        else
                        {
                            dataType = DataTypes.Numeric;
                        }

                        if (leftDataType.IsNumericOrFloat() || rightDataType.IsNumericOrFloat())
                        {
                            return (dataType, PinFloatingPointExpressionToVariable(context, scope,
                                nonInlinePartWriter, dataType,
                                "arithmetic",
                                $"{leftExp}{arithmeticEvaluationStatement.Operator}{rightExp}"));
                        }
                        
                        return (dataType, $"{leftExp}{arithmeticEvaluationStatement.Operator}{rightExp}");
                    }
                    case FunctionCallStatement functionCallStatement: //functions are always not-inlined.
                    {
                        var call = new StringBuilder(20); //`myfunc 0 "test"`
                        call.Append('`');
                        call.Append(functionCallStatement.FunctionName);

                        foreach (var param in functionCallStatement.Parameters)
                        {
                            var (dataType, exp) = CreateExpression(context, scope, nonInlinePartWriter, param);

                            call.Append(' ');
                            call.Append(exp);
                        }

                        call.Append('`');

                        var varName = PinExpressionToVariable(context, scope, nonInlinePartWriter,
                            functionCallStatement.DataType,
                            $"{functionCallStatement.ObjectName}_{functionCallStatement.FunctionName}",
                            call.ToString());

                        return (functionCallStatement.DataType, varName);
                    }

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public class StringConcatenationExpressionBuilder : ExpressionBuilder
        {
            public override (DataTypes, string) CreateExpression(Context context, Scope scope, TextWriter nonInlinePartWriter,
                IStatement statement)
            {
                switch (statement)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        if (constantValueStatement.DataType == DataTypes.String)
                        {
                            return (DataTypes.String, BashTranspilerHelpers.StandardizeString(constantValueStatement.Value, true));
                        }

                        return (DataTypes.String, constantValueStatement.Value);
                    }
                    
                    default:
                        return base.CreateExpression(context, scope, nonInlinePartWriter, statement);
                }
            }
        }

        

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            var stringConcatenation = structure.Types.Contains(DataTypes.String);
            
            if (stringConcatenation)
            {
                var expressionBuilder = new StringConcatenationExpressionBuilder();

                var (dataType, expression) =
                    expressionBuilder.CreateExpression(context, scope, nonInlinePartWriter, evalStt);
                
                writer.Write('"');
                writer.Write(expression);
                writer.Write('"');
            }
            else
            {
                var expressionBuilder = new ExpressionBuilder();
                
                var (dataType, expression) =
                    expressionBuilder.CreateExpression(context, scope, nonInlinePartWriter, evalStt);
                
                writer.Write(expression);
            }
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