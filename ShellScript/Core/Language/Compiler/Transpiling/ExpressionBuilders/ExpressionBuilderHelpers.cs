using System;
using System.Collections.Generic;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;

namespace ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders
{
    public class ExpressionBuilderHelpers
    {
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
                case TypeCastStatement typeCastStatement:
                {
                    if (ReplaceEvaluation(typeCastStatement.Target, predicate, replace, out var right))
                    {
                        result = new TypeCastStatement(typeCastStatement.TypeDescriptor, (EvaluationStatement) right,
                            typeCastStatement.Info, typeCastStatement.ParentStatement);
                        return true;
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
                            result = new FunctionCallStatement(functionCallStatement.ClassName,
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