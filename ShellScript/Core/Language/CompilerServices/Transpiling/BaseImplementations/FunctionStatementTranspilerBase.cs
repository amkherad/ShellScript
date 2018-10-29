using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class FunctionStatementTranspilerBase : BlockStatementTranspilerBase, IPlatformStatementTranspiler
    {
        public override Type StatementType => typeof(FunctionStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            if (funcDefStt.Parameters != null && funcDefStt.Parameters.Length > 0)
            {
                scope = new Scope(context, scope);
                foreach (var p in funcDefStt.Parameters)
                {
                    scope.ReserveNewVariable(p.TypeDescriptor, p.Name);
                }
            }

            var functionName = funcDefStt.Name;

            if (scope.IsIdentifierExists(functionName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(functionName, funcDefStt.Info);
                return false;
            }

            if (funcDefStt.TypeDescriptor.IsVoid())
            {
                if (!CheckVoidReturn(context, scope, funcDefStt))
                {
                    //throw new InvalidStatementStructureCompilerException("Non void return on a void function.", funcDefStt.Info);
                    message = "Non-void return inside a void function";
                    return false;
                }
            }
            else
            {
                var (assertion, assertTarget) = CheckReturnOnAllPaths(context, scope, funcDefStt.TypeDescriptor, funcDefStt);
                if (assertion != 0)
                {
                    //throw new InvalidStatementStructureCompilerException("Not all paths return a value.", funcDefStt.Info);
                    message = assertion == 2
                        ? "Void return inside a non-void function"
                        : "Not all paths return a value";
                    return false;
                }
            }

            return base.Validate(context, scope, statement, out message);
        }

        public static bool CheckVoidReturn(Context context, Scope scope, FunctionStatement functionStatement)
        {
            var anyNonVoidReturn = functionStatement.TreeAny(stt =>
            {
                if (stt is ReturnStatement returnStatement)
                {
                    if (returnStatement.Result != null)
                    {
                        return true;
                    }
                }

                return false;
            });

            return !anyNonVoidReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scope"></param>
        /// <param name="assertTypeDescriptor"></param>
        /// <param name="statement"></param>
        /// <returns>
        /// 0: success
        /// 1: fail
        /// 2: void return
        /// </returns>
        /// <exception cref="TypeMismatchCompilerException"></exception>
        public static (int, IStatement) CheckReturnOnAllPaths(Context context, Scope scope, TypeDescriptor assertTypeDescriptor,
            IStatement statement)
        {
            switch (statement)
            {
                case ThrowStatement _:
                {
                    return (0, null);
                }
                case ReturnStatement returnStatement:
                {
                    if (returnStatement.Result == null)
                    {
                        return (2, returnStatement);
                    }

                    var returnDataType = returnStatement.Result.GetDataType(context, scope);
                    if (!StatementHelpers.IsAssignableFrom(context, scope, assertTypeDescriptor, returnDataType))
                    {
                        throw new TypeMismatchCompilerException(returnDataType, assertTypeDescriptor,
                            returnStatement.Result.Info);
                    }

                    return (0, null);
                }

                case IBranchWrapperStatement branchWrapperStatement:
                {
                    foreach (var branch in branchWrapperStatement.Branches)
                    {
                        var result = CheckReturnOnAllPaths(context, scope, assertTypeDescriptor, branch);
                        if (result.Item1 != 0)
                        {
                            return result;
                        }
                    }

                    return (0, null);
                }
                case IBlockWrapperStatement blockWrapperStatement:
                {
                    return CheckReturnOnAllPaths(context, scope, assertTypeDescriptor, blockWrapperStatement.Statement);
                }
                case BlockStatement block:
                {
                    var isUnreachable = false;
                    foreach (var stt in block.Statements)
                    {
                        if (isUnreachable)
                        {
                            context.WarningWriter.WriteLine(
                                $"Statement at {stt.Info.GetPositionString()} is unreachable");
                            break;
                        }

                        var check = CheckReturnOnAllPaths(context, scope, assertTypeDescriptor, stt);
                        if (check.Item1 == 0)
                        {
                            isUnreachable = true;
                            continue;
                        }
                        else if (check.Item1 == 2)
                        {
                            return check;
                        }
                    }

                    return (isUnreachable ? 0 : 1, block);
                }
                default:
                    return (1, statement);
            }
        }

        public static bool TryGetInlinedStatement(Context context, Scope scope, FunctionStatement function,
            out IStatement inlinedStatement)
        {
            if (!context.Flags.UseInlining)
            {
                inlinedStatement = null;
                return false;
            }

            IStatement stt;

            if (function.Statement is BlockStatement blockStatement)
            {
                if (blockStatement.Statements.Length != 1)
                {
                    inlinedStatement = null;
                    return false;
                }

                stt = blockStatement.Statements.First();
            }
            else
            {
                stt = function.Statement;
            }

            switch (stt)
            {
                case FunctionCallStatement functionCallStatement:
                {
                    if (!context.Flags.InlineCascadingFunctionCalls)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    if (!scope.TryGetFunctionInfo(functionCallStatement, out var functionInfo))
                    {
                        inlinedStatement = functionInfo.InlinedStatement;
                        return inlinedStatement != null;
                    }

                    inlinedStatement = functionCallStatement;
                    return true;
                }
                case EvaluationStatement evaluationStatement:
                {
                    inlinedStatement = evaluationStatement;
                    return true;
                }
                case ReturnStatement returnStatement:
                {
                    inlinedStatement = returnStatement;
                    return true;
                }
                default:
                {
                    if (!context.Flags.InlineNonEvaluations)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    inlinedStatement = stt;
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EvaluationStatement GetSchemeParameterValueByIndex(Context context, Scope scope,
            FunctionInfo functionInfo, FunctionCallStatement functionCallStatement, int index)
        {
            return functionCallStatement.Parameters[index];
        }

        public static bool ReplaceEvaluation(IStatement statement,
            Predicate<string> predicate, Func<string, EvaluationStatement> replace, out IStatement result)
        {
            switch (statement)
            {
                case ConstantValueStatement _:
                {
                    result = statement;
                    return false;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (predicate(variableAccessStatement.VariableName))
                    {
                        var replaceValue = replace(variableAccessStatement.VariableName);
                        replaceValue.ParentStatement = variableAccessStatement.ParentStatement;

                        result = replaceValue;
                        return true;
                    }

                    result = variableAccessStatement;
                    return false;
                }
                case BitwiseEvaluationStatement bitwiseEvaluationStatement:
                {
                    if (bitwiseEvaluationStatement.Operator is BitwiseNotOperator bitwiseNotOperator)
                    {
                        if (ReplaceEvaluation(bitwiseEvaluationStatement.Right, predicate, replace,
                            out var right))
                        {
                            result = BitwiseEvaluationStatement.CreateNot(bitwiseNotOperator,
                                (EvaluationStatement) right, bitwiseEvaluationStatement.Info,
                                bitwiseEvaluationStatement.ParentStatement);
                            return true;
                        }
                    }
                    else
                    {
                        if (ReplaceEvaluation(bitwiseEvaluationStatement.Left, predicate, replace,
                                out var left) |
                            ReplaceEvaluation(bitwiseEvaluationStatement.Right, predicate, replace,
                                out var right))
                        {
                            result = new BitwiseEvaluationStatement((EvaluationStatement) left,
                                bitwiseEvaluationStatement.Operator, (EvaluationStatement) right,
                                bitwiseEvaluationStatement.Info, bitwiseEvaluationStatement.ParentStatement);
                            return true;
                        }
                    }

                    break;
                }
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (logicalEvaluationStatement.Operator is NotOperator logicalNotOperator)
                    {
                        if (ReplaceEvaluation(logicalEvaluationStatement.Right, predicate, replace,
                            out var right))
                        {
                            result = LogicalEvaluationStatement.CreateNot(logicalNotOperator,
                                (EvaluationStatement) right, logicalEvaluationStatement.Info,
                                logicalEvaluationStatement.ParentStatement);
                            return true;
                        }
                    }
                    else
                    {
                        if (ReplaceEvaluation(logicalEvaluationStatement.Left, predicate, replace,
                                out var left) |
                            ReplaceEvaluation(logicalEvaluationStatement.Right, predicate, replace,
                                out var right))
                        {
                            result = new LogicalEvaluationStatement((EvaluationStatement) left,
                                logicalEvaluationStatement.Operator, (EvaluationStatement) right,
                                logicalEvaluationStatement.Info, logicalEvaluationStatement.ParentStatement);
                            return true;
                        }
                    }

                    break;
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case NegativeNumberOperator negativeNumberOperator:
                        {
                            if (ReplaceEvaluation(arithmeticEvaluationStatement.Right, predicate, replace,
                                out var right))
                            {
                                result = ArithmeticEvaluationStatement.CreateNegate(negativeNumberOperator,
                                    (EvaluationStatement) right, arithmeticEvaluationStatement.Info,
                                    arithmeticEvaluationStatement.ParentStatement);
                                return true;
                            }

                            break;
                        }
                        case IncrementOperator _:
                        case DecrementOperator _:
                        {
                            EvaluationStatement operand;
                            bool isPostfix;
                            if (arithmeticEvaluationStatement.Left == null)
                            {
                                operand = arithmeticEvaluationStatement.Right;
                                isPostfix = false;
                            }
                            else if (arithmeticEvaluationStatement.Right == null)
                            {
                                operand = arithmeticEvaluationStatement.Left;
                                isPostfix = true;
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }

                            if (ReplaceEvaluation(operand, predicate, replace, out var replacedOperand))
                            {
                                result = isPostfix
                                    ? ArithmeticEvaluationStatement.CreatePostfix(
                                        arithmeticEvaluationStatement.Operator, (EvaluationStatement) replacedOperand,
                                        arithmeticEvaluationStatement.Info,
                                        arithmeticEvaluationStatement.ParentStatement)
                                    : ArithmeticEvaluationStatement.CreatePrefix(arithmeticEvaluationStatement.Operator,
                                        (EvaluationStatement) replacedOperand, arithmeticEvaluationStatement.Info,
                                        arithmeticEvaluationStatement.ParentStatement);
                                return true;
                            }

                            break;
                        }
                        default:
                        {
                            if (ReplaceEvaluation(arithmeticEvaluationStatement.Left, predicate, replace,
                                    out var left) |
                                ReplaceEvaluation(arithmeticEvaluationStatement.Right, predicate, replace,
                                    out var right))
                            {
                                result = new ArithmeticEvaluationStatement((EvaluationStatement) left,
                                    arithmeticEvaluationStatement.Operator, (EvaluationStatement) right,
                                    arithmeticEvaluationStatement.Info, arithmeticEvaluationStatement.ParentStatement);
                                return true;
                            }

                            break;
                        }
                    }

                    break;
                }
                case AssignmentStatement assignmentStatement:
                {
                    if (ReplaceEvaluation(assignmentStatement.LeftSide, predicate, replace, out var left) |
                        ReplaceEvaluation(assignmentStatement.RightSide, predicate, replace, out var right))
                    {
                        result = new AssignmentStatement((EvaluationStatement) left, (EvaluationStatement) right,
                            assignmentStatement.Info, assignmentStatement.ParentStatement);
                        return true;
                    }

                    break;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    var parameters = functionCallStatement.Parameters;
                    if (parameters != null)
                    {
                        var isReplaced = false;
                        var newParameters = new EvaluationStatement[parameters.Length];

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (ReplaceEvaluation(parameters[i], predicate, replace, out var newP))
                            {
                                newParameters[i] = (EvaluationStatement) newP;
                                isReplaced = true;
                            }
                        }

                        if (isReplaced)
                        {
                            result = new FunctionCallStatement(functionCallStatement.ObjectName,
                                functionCallStatement.FunctionName, functionCallStatement.TypeDescriptor, newParameters,
                                functionCallStatement.Info, functionCallStatement.ParentStatement);
                            return true;
                        }
                    }

                    break;
                }
                case EchoStatement echoStatement:
                {
                    var parameters = echoStatement.Parameters;
                    if (parameters != null)
                    {
                        var isReplaced = false;
                        var newParameters = new EvaluationStatement[parameters.Length];

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (ReplaceEvaluation(parameters[i], predicate, replace, out var newP))
                            {
                                newParameters[i] = (EvaluationStatement) newP;
                                isReplaced = true;
                            }
                        }

                        if (isReplaced)
                        {
                            result = new EchoStatement(newParameters, echoStatement.Info);
                            return true;
                        }
                    }

                    break;
                }
                case ForStatement forStatement:
                {
                    throw new NotImplementedException();
                }
                case ForEachStatement forEachStatement:
                {
                    throw new NotImplementedException();
                }
                case WhileStatement whileStatement:
                {
                    throw new NotImplementedException();
                }
                case DoWhileStatement doWhileStatement:
                {
                    throw new NotImplementedException();
                }
                case ConditionalBlockStatement conditionalBlockStatement:
                {
                    if (ReplaceEvaluation(conditionalBlockStatement.Condition, predicate, replace,
                            out var condition) |
                        ReplaceEvaluation(conditionalBlockStatement.Statement, predicate, replace,
                            out var stt))
                    {
                        result = new ConditionalBlockStatement((EvaluationStatement) condition, stt,
                            conditionalBlockStatement.Info);
                        return true;
                    }

                    break;
                }
                case IfElseStatement ifElseStatement:
                {
                    var isChanged = ReplaceEvaluation(ifElseStatement.MainIf, predicate, replace,
                        out var mainIf);

                    var elseIfs = new List<ConditionalBlockStatement>();

                    if (ifElseStatement.ElseIfs != null && ifElseStatement.ElseIfs.Length > 0)
                    {
                        foreach (var ei in ifElseStatement.ElseIfs)
                        {
                            isChanged |= ReplaceEvaluation(ei, predicate, replace,
                                out var newEi);

                            elseIfs.Add((ConditionalBlockStatement) newEi);
                        }
                    }

                    IStatement newElse = null;

                    if (ifElseStatement.Else != null)
                    {
                        isChanged |= ReplaceEvaluation(ifElseStatement.Else, predicate, replace,
                            out newElse);
                    }

                    if (isChanged)
                    {
                        result = new IfElseStatement((ConditionalBlockStatement) mainIf,
                            elseIfs.Count == 0 ? null : elseIfs.ToArray(), newElse, ifElseStatement.Info);
                        return true;
                    }

                    break;
                }
                case ReturnStatement returnStatement:
                {
                    if (ReplaceEvaluation(returnStatement.Result, predicate, replace, out var newResult))
                    {
                        result = new ReturnStatement((EvaluationStatement) newResult, returnStatement.Info);
                        return true;
                    }

                    break;
                }
                case VariableDefinitionStatement variableDefinitionStatement:
                {
                    if (variableDefinitionStatement.DefaultValue != null &&
                        ReplaceEvaluation(variableDefinitionStatement.DefaultValue, predicate, replace,
                            out var newResult))
                    {
                        result = new VariableDefinitionStatement(variableDefinitionStatement.TypeDescriptor,
                            variableDefinitionStatement.Name, variableDefinitionStatement.IsConstant,
                            (EvaluationStatement) newResult, variableDefinitionStatement.Info);
                        return true;
                    }

                    break;
                }
                case SwitchCaseStatement switchCaseStatement:
                {
                    var isChanged = ReplaceEvaluation(switchCaseStatement.SwitchTarget, predicate, replace,
                        out var switchTarget);

                    var cases = new List<ConditionalBlockStatement>();
                    if (switchCaseStatement.Cases != null && switchCaseStatement.Cases.Length > 0)
                    {
                        foreach (var sCase in switchCaseStatement.Cases)
                        {
                            isChanged |= ReplaceEvaluation(sCase, predicate, replace,
                                out var newCase);
                            cases.Add((ConditionalBlockStatement) newCase);
                        }
                    }

                    IStatement newDefaultCase = null;

                    if (switchCaseStatement.DefaultCase != null)
                    {
                        isChanged |= ReplaceEvaluation(switchCaseStatement.DefaultCase, predicate, replace,
                            out newDefaultCase);
                    }

                    if (isChanged)
                    {
                        result = new SwitchCaseStatement((EvaluationStatement) switchTarget,
                            cases.Count == 0 ? null : cases.ToArray(), newDefaultCase, switchCaseStatement.Info);
                        return true;
                    }

                    break;
                }
                case BlockStatement blockStatement:
                {
                    var isChanged = false;
                    var statements = new List<IStatement>();
                    foreach (var stt in blockStatement.Statements)
                    {
                        isChanged |= ReplaceEvaluation(stt, predicate, replace,
                            out var newStt);
                        statements.Add(newStt);
                    }

                    if (isChanged)
                    {
                        result = new BlockStatement(statements.ToArray(), blockStatement.Info);
                        return true;
                    }

                    break;
                }
            }

            result = statement;
            return false;
        }
    }
}