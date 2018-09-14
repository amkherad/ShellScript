using System;
using System.Linq;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
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

            var functionName = funcDefStt.Name;

            if (scope.IsIdentifierExists(functionName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(functionName, funcDefStt.Info);
                return false;
            }

            if (funcDefStt.DataType == DataTypes.Void)
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
                var (assertion, assertTarget) = CheckReturnOnAllPaths(context, scope, funcDefStt.DataType, funcDefStt);
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
        /// <param name="assertDataType"></param>
        /// <param name="statement"></param>
        /// <returns>
        /// 0: success
        /// 1: fail
        /// 2: void return
        /// </returns>
        /// <exception cref="TypeMismatchCompilerException"></exception>
        public static (int, IStatement) CheckReturnOnAllPaths(Context context, Scope scope, DataTypes assertDataType,
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
                    if (!StatementHelpers.IsAssignableFrom(assertDataType, returnDataType))
                    {
                        throw new TypeMismatchCompilerException(returnDataType, assertDataType,
                            returnStatement.Result.Info);
                    }

                    return (0, null);
                }

                case IBranchWrapperStatement branchWrapperStatement:
                {
                    foreach (var branch in branchWrapperStatement.Branches)
                    {
                        var result = CheckReturnOnAllPaths(context, scope, assertDataType, branch);
                        if (result.Item1 != 0)
                        {
                            return result;
                        }
                    }

                    return (0, null);
                }
                case IBlockWrapperStatement blockWrapperStatement:
                {
                    return CheckReturnOnAllPaths(context, scope, assertDataType, blockWrapperStatement.Statement);
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

                        var check = CheckReturnOnAllPaths(context, scope, assertDataType, stt);
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
                case ReturnStatement returnStatement:
                {
                    inlinedStatement = returnStatement.Result;
                    return true;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    if (!context.Flags.InlineCascadingFunctionCalls)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    inlinedStatement = functionCallStatement;
                    return true;
                }
                default:
                {
                    if (!context.Flags.InlineNonEvaluations)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    inlinedStatement = null;
                    return false;
                }
            }
        }
    }
}