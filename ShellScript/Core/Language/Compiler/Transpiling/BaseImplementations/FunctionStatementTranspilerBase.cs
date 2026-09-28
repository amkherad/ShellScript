using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
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
            var skipNameCheck = !string.IsNullOrEmpty(funcDefStt.ClassName) &&
                                funcDefStt.IsConstructor &&
                                functionName == funcDefStt.ClassName;

            if (!skipNameCheck && scope.IsIdentifierExists(functionName))
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
                var (assertion, assertTarget) =
                    CheckReturnOnAllPaths(context, scope, funcDefStt.TypeDescriptor, funcDefStt);
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
        public static (int, IStatement) CheckReturnOnAllPaths(Context context, Scope scope,
            TypeDescriptor assertTypeDescriptor, IStatement statement)
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
                    StatementHelpers.AssertAssignableFrom(context, scope, assertTypeDescriptor, returnDataType,
                        returnStatement.Result.Info);
//                    if (!StatementHelpers.IsAssignableFrom(context, scope, assertTypeDescriptor, returnDataType))
//                    {
//                        throw new TypeMismatchCompilerException(returnDataType, assertTypeDescriptor,
//                            returnStatement.Result.Info);
//                    }

                    return (0, null);
                }

                case IfElseStatement ifElseStatement:
                {
                    if (ifElseStatement.Else == null &&
                        (ifElseStatement.ElseIfs == null || ifElseStatement.ElseIfs.Length == 0))
                    {
                        return (1, ifElseStatement);
                    }

                    foreach (var branch in ifElseStatement.Branches)
                    {
                        var result = CheckReturnOnAllPaths(context, scope, assertTypeDescriptor, branch);
                        if (result.Item1 != 0)
                        {
                            return result;
                        }
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
                                $"Statement {stt.Info} is unreachable");
                            break;
                        }

                        var check = CheckReturnOnAllPaths(context, scope, assertTypeDescriptor, stt);
                        if (StatementAlwaysTerminates(context, scope, assertTypeDescriptor, stt))
                        {
                            isUnreachable = true;
                            if (check.Item1 == 2)
                            {
                                return check;
                            }

                            continue;
                        }

                        if (check.Item1 == 2)
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

        private static bool StatementAlwaysTerminates(Context context, Scope scope,
            TypeDescriptor assertTypeDescriptor, IStatement statement)
        {
            switch (statement)
            {
                case ReturnStatement _:
                case ThrowStatement _:
                    return true;
                case IfElseStatement ifElseStatement:
                {
                    if (ifElseStatement.Else == null)
                    {
                        return false;
                    }

                    return StatementAlwaysTerminates(context, scope, assertTypeDescriptor,
                               ifElseStatement.MainIf.Statement) &&
                           StatementAlwaysTerminates(context, scope, assertTypeDescriptor, ifElseStatement.Else);
                }
                case BlockStatement blockStatement:
                {
                    foreach (var child in blockStatement.Statements)
                    {
                        if (StatementAlwaysTerminates(context, scope, assertTypeDescriptor, child))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                default:
                    return false;
            }
        }

        private static bool TryGetStrongInlineStatement(BlockStatement block, out IStatement statement)
        {
            statement = null;
            var statements = block.Statements;
            if (statements == null || statements.Length == 0)
            {
                return false;
            }

            var last = statements[statements.Length - 1];
            if (!(last is ReturnStatement returnStatement) || returnStatement.Result == null)
            {
                return false;
            }

            for (var i = 0; i < statements.Length - 1; i++)
            {
                if (!(statements[i] is EchoStatement))
                {
                    return false;
                }
            }

            statement = returnStatement;
            return true;
        }

        public static bool TryGetInlinedStatement(Context context, Scope scope, FunctionStatement function,
            out IStatement inlinedStatement)
        {
            if (!context.Flags.UseInlining)
            {
                inlinedStatement = null;
                return false;
            }

            IStatement stt = null;

            if (function.Statement is BlockStatement blockStatement)
            {
                if (blockStatement.Statements.Length == 1)
                {
                    stt = blockStatement.Statements[0];
                }
                else if (context.Flags.UseStrongInlining &&
                         blockStatement.Statements.Length <= context.Flags.StrongInliningMaxStatements &&
                         TryGetStrongInlineStatement(blockStatement, out stt))
                {
                }
                else
                {
                    inlinedStatement = null;
                    return false;
                }
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

                    if (!scope.TryGetFunctionInfo(functionCallStatement, out var functionInfo) ||
                        functionInfo.InlinedStatement == null)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    inlinedStatement = functionInfo.InlinedStatement;
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
            var passed = functionCallStatement.Parameters;
            if (passed != null && index < passed.Length)
            {
                return passed[index];
            }

            var scheme = functionInfo.Parameters;
            if (scheme != null && index < scheme.Length && scheme[index].HasDefaultValue)
            {
                return scheme[index].DefaultValue;
            }

            throw new InvalidFunctionCallParametersCompilerException(scheme?.Length ?? 0, passed?.Length ?? 0,
                functionCallStatement.Info);
        }

        public static int GetRequiredParameterCount(FunctionParameterDefinitionStatement[] schemeParameters)
        {
            if (schemeParameters == null || schemeParameters.Length == 0)
            {
                return 0;
            }

            var count = 0;
            foreach (var parameter in schemeParameters)
            {
                if (!parameter.HasDefaultValue)
                {
                    count++;
                }
            }

            return count;
        }

        public static FunctionInfo GetFunctionInfoFromFunctionCall(Context context, Scope scope,
            FunctionCallStatement functionCallStatement, out ILanguageObjectInfo sourceObjectInfo)
        {
            if (!scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo) &&
                !scope.TryGetPrototypeInfo(functionCallStatement, out funcInfo))
            {
                if (functionCallStatement.ClassName != null &&
                    scope.TryGetVariableInfo(functionCallStatement.ClassName, out var instanceVar) &&
                    ObjectModelHelpers.IsUserClassType(instanceVar.TypeDescriptor))
                {
                    var userClassName = ObjectModelHelpers.GetUserClassName(instanceVar.TypeDescriptor);
                    if (scope.TryGetFunctionInfo(userClassName, functionCallStatement.FunctionName, out funcInfo) ||
                        scope.TryGetPrototypeInfo(userClassName, functionCallStatement.FunctionName, out funcInfo))
                    {
                        sourceObjectInfo = instanceVar;
                        return PreferNativeFunctionImplementation(scope, functionCallStatement, funcInfo,
                            ref sourceObjectInfo);
                    }
                }

                if (!scope.TryGetVariableInfo(functionCallStatement, out var variableInfo))
                {
                    throw new IdentifierNotFoundCompilerException(functionCallStatement.Fqn,
                        functionCallStatement.Info);
                }

                sourceObjectInfo = variableInfo;

                var type = variableInfo.TypeDescriptor;

                if (type.DataType != DataTypes.Delegate &&
                    type.DataType != DataTypes.Lookup ||
                    type.Lookup == null)
                {
                    throw new InvalidStatementStructureCompilerException(functionCallStatement.Fqn,
                        functionCallStatement.Info);
                }

                var lookup = type.Lookup.Value;

                if (!scope.TryGetFunctionInfo(lookup.ClassName, lookup.Name, out funcInfo) &&
                    !scope.TryGetPrototypeInfo(lookup.ClassName, lookup.Name, out funcInfo))
                {
                    throw new InvalidStatementStructureCompilerException(functionCallStatement.Fqn,
                        functionCallStatement.Info);
                }
            }
            else
            {
                sourceObjectInfo = funcInfo;
            }

            return PreferNativeFunctionImplementation(scope, functionCallStatement, funcInfo, ref sourceObjectInfo);
        }

        private static FunctionInfo PreferNativeFunctionImplementation(Scope scope,
            FunctionCallStatement functionCallStatement, FunctionInfo funcInfo,
            ref ILanguageObjectInfo sourceObjectInfo)
        {
            if (funcInfo is ApiFunctionInfo && functionCallStatement.ClassName != null &&
                scope.TryGetNativeFunctionInfo(functionCallStatement.ClassName, functionCallStatement.FunctionName,
                    out var nativeFunctionInfo))
            {
                sourceObjectInfo = nativeFunctionInfo;
                return nativeFunctionInfo;
            }

            return funcInfo;
        }

        /// <summary>
        /// Validates the function call by using a schema (<param name="funcInfo">funcInfo</param>). 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scope"></param>
        /// <param name="funcInfo"></param>
        /// <param name="functionCallStatement"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool ValidateFunctionCall(Context context, Scope scope, FunctionInfo funcInfo,
            FunctionCallStatement functionCallStatement, out Exception exception)
        {
            return ValidateParameters(context, scope, funcInfo.Parameters, functionCallStatement.Parameters,
                out exception);
        }

        public static bool ValidateParameters(Context context, Scope scope,
            FunctionParameterDefinitionStatement[] schemeParameters, EvaluationStatement[] parameters,
            out Exception exception
        )
        {
            var passedCount = parameters?.Length ?? 0;
            var requiredCount = GetRequiredParameterCount(schemeParameters);
            var maxCount = schemeParameters?.Length ?? 0;

            if (passedCount < requiredCount || passedCount > maxCount)
            {
                exception = new InvalidFunctionCallParametersCompilerException(maxCount, passedCount, null);
                return false;
            }

            if (parameters != null && schemeParameters != null)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var passed = parameters[i];
                    var scheme = schemeParameters[i];

                    var passedType = passed.GetDataType(context, scope);
                    if (!StatementHelpers.IsAssignableFrom(context, scope, scheme.TypeDescriptor, passedType))
                    {
                        if (scheme.DynamicType)
                        {
                            continue;
                        }

                        exception = new TypeMismatchCompilerException(passedType, scheme.TypeDescriptor, passed.Info);
                        return false;
                    }
                }
            }

            exception = null;
            return true;
        }
    }
}