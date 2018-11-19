using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBaseFunction : IApiFunc
    {
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public abstract string ClassName { get; }
        public abstract TypeDescriptor TypeDescriptor { get; }
        public abstract bool IsStatic { get; }
        public abstract FunctionParameterDefinitionStatement[] Parameters { get; }

        public static Dictionary<string, string> UtilitiesLookupTestVariableName { get; }

        static ApiBaseFunction()
        {
            UtilitiesLookupTestVariableName = new Dictionary<string, string>();
        }

        public abstract IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMethodBuilderInlineResult Inline(EvaluationStatement statement)
        {
            return new ApiMethodBuilderInlineResult(statement);
        }


        public static IApiMethodBuilderResult CompileMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string methodBody, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            
            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.TypeDescriptor,
                parameters, statementInfo));
        }

        public static IApiMethodBuilderResult CompileResourceMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string resourceName, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
//            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
//            {
//                return Inline(new FunctionCallStatement(funcInfo.ObjectName, funcInfo.Name, funcInfo.DataType,
//                    parameters, statementInfo));
//            }
//
//            using (var file = Assembly.GetCallingAssembly()
//                .GetManifestResourceStream(typeof(TFunc), resourceName))
//            using (var reader = new StreamReader(file))
//            {
//                string line;
//                while ((line = reader.ReadLine()) != null)
//                {
//                    p.MetaWriter.WriteLine(line);
//                }
//
//                p.MetaWriter.WriteLine();
//            }
//
//            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.TypeDescriptor,
                parameters, statementInfo));
        }

        public static IApiMethodBuilderResult WriteNativeMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string methodBody, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
            {
                return Inline(new FunctionCallStatement(funcInfo.ClassName, funcInfo.Name, funcInfo.TypeDescriptor,
                    parameters, statementInfo));
            }

            using (var funcWriter = new StringWriter())
            {
                funcWriter.Write("function ");
                funcWriter.Write(functionInfo.Fqn);
                funcWriter.WriteLine("() {");

                funcWriter.WriteLine(methodBody);

                funcWriter.WriteLine("}");

                p.MetaWriter.Write(funcWriter);
            }

            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.TypeDescriptor,
                parameters, statementInfo));
        }

        public static IApiMethodBuilderResult UseNativeResourceMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string resourceName, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
            {
                return Inline(new FunctionCallStatement(funcInfo.ClassName, funcInfo.Name, funcInfo.TypeDescriptor,
                    parameters, statementInfo));
            }

            using (var file = Assembly.GetCallingAssembly()
                .GetManifestResourceStream(typeof(TFunc), resourceName))
            using (var reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    p.MetaWriter.WriteLine(line);
                }

                p.MetaWriter.WriteLine();
            }

            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.TypeDescriptor,
                parameters, statementInfo));
        }

        public static string GetUtilityLookupTestVariableName(Context context, TextWriter metaWriter,
            IThirdPartyUtility utility)
        {
            var utilName = utility.Name;
            if (UtilitiesLookupTestVariableName.TryGetValue(utilName, out var name))
            {
                return $"${name} -ne 0";
            }

            var condition = utility.WriteExistenceCondition(context, metaWriter);

            metaWriter.WriteLine($"if [ {condition} ]");
            metaWriter.WriteLine("then");

            name = context.GeneralScope.NewHelperVariable(TypeDescriptor.Boolean, $"{utilName}_existence");
            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(context, context.GeneralScope, metaWriter,
                name, "1");

            metaWriter.WriteLine("else");

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(context, context.GeneralScope, metaWriter,
                name, "0");

            metaWriter.WriteLine("fi");

            return $"${name} -ne 0";
        }

        public static IApiMethodBuilderResult CreateNativeMethodWithUtilityExpressionSelector<TFunc>(
            TFunc func, ExpressionBuilderParams p, FunctionInfo functionInfo,
            IDictionary<string, string> utilityCommands,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
            {
                return Inline(new FunctionCallStatement(funcInfo.ClassName, funcInfo.Name, funcInfo.TypeDescriptor,
                    parameters, statementInfo));
            }

            using (var funcWriter = new StringWriter())
            {
                funcWriter.Write("function ");
                funcWriter.Write(functionInfo.Fqn);
                funcWriter.WriteLine("() {");

                bool isFirst = true;
                var utilities = p.Context.Api.Utilities;
                foreach (var utility in utilityCommands)
                {
                    if (utilities.TryGetValue(utility.Key, out var util))
                    {
                        var condition = GetUtilityLookupTestVariableName(p.Context, p.MetaWriter, util);

                        if (isFirst)
                        {
                            funcWriter.Write("if [ ");

                            isFirst = false;
                        }
                        else
                        {
                            funcWriter.Write("elif [ ");
                        }

                        funcWriter.Write(condition);
                        funcWriter.WriteLine(" ]");
                        funcWriter.WriteLine("then");
                        funcWriter.WriteLine(utility.Value);
                    }

                    //Just ignoring the utility!
//                    else
//                    {
//                        throw new InvalidOperationException("Utility is not installed.");
//                    }
                }

                funcWriter.WriteLine("fi");
                funcWriter.WriteLine("}");

                p.MetaWriter.Write(funcWriter);
            }

            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.TypeDescriptor,
                parameters, statementInfo));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ExpressionResult CreateVariableAccess(TypeDescriptor typeDescriptor, string className,
            string name, StatementInfo info)
        {
            return new ExpressionResult(
                typeDescriptor,
                $"${name}",
                new VariableAccessStatement(className, name, info)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ExpressionResult CreateVariableAccess(TypeDescriptor typeDescriptor, string name,
            StatementInfo info)
        {
            return new ExpressionResult(
                typeDescriptor,
                $"${name}",
                new VariableAccessStatement(name, info)
            );
        }


        public void AssertParameters(ExpressionBuilderParams p, EvaluationStatement[] parameters)
        {
            var passedCount = parameters?.Length ?? 0;
            var expectedCount = Parameters?.Length ?? 0;

            if (passedCount != expectedCount)
            {
                throw new InvalidFunctionCallParametersCompilerException(expectedCount, passedCount, null);
            }

            if (parameters != null && Parameters != null)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var passed = parameters[i];
                    var scheme = Parameters[i];

                    var passedType = passed.GetDataType(p.Context, p.Scope);
                    if (!StatementHelpers.IsAssignableFrom(p.Context, p.Scope, scheme.TypeDescriptor, passedType))
                    {
                        if (scheme.DynamicType)
                        {
                            continue;
                        }
                        
                        throw new TypeMismatchCompilerException(passedType, scheme.TypeDescriptor, passed.Info);
                    }
                }
            }
        }

        public void AssertExpressionParameters(EvaluationStatement[] parameters)
        {
            //TODO: assert
        }

        protected static Exception ThrowInvalidParameterType(TypeDescriptor typeDescriptor, string name)
        {
            //TODO: return correct exception.
            return new Exception();
        }

        protected static Exception ThrowInvalidParameterType(ExpressionResult result)
        {
            //TODO: return correct exception.
            return new Exception();
        }
    }
}