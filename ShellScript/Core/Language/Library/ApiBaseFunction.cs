using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Unix.Bash.PlatformTranspiler;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

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

        private static IApiMethodBuilderResult InvokeRegisteredNativeFunction(ExpressionBuilderParams p,
            FunctionInfo registeredFunction, EvaluationStatement[] parameters, StatementInfo statementInfo)
        {
            var builder = BashDefaultExpressionBuilder.Instance;
            var call = new StringBuilder();
            call.Append('`');
            call.Append(registeredFunction.AccessName);

            var paramTemplates = new List<EvaluationStatement>();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var result = builder.CreateExpression(p, param);
                    call.Append(' ');
                    call.Append(builder.FormatFunctionCallParameterSubExpression(p, result));
                    paramTemplates.Add(result.Template);
                }
            }

            call.Append('`');

            var template = new FunctionCallStatement(
                registeredFunction.ClassName,
                registeredFunction.Name,
                registeredFunction.TypeDescriptor,
                paramTemplates.ToArray(),
                statementInfo);

            return new ApiMethodBuilderRawResult(new ExpressionResult(
                registeredFunction.TypeDescriptor,
                call.ToString(),
                template));
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
            if (p.Scope.TryGetNativeFunctionInfo(functionInfo.ClassName, functionInfo.Name, out var funcInfo))
            {
                return InvokeRegisteredNativeFunction(p, funcInfo, parameters, statementInfo);
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

            return InvokeRegisteredNativeFunction(p, functionInfo, parameters, statementInfo);
        }

        public static IApiMethodBuilderResult UseNativeResourceMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string resourceName, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetNativeFunctionInfo(functionInfo.ClassName, functionInfo.Name, out var funcInfo))
            {
                return InvokeRegisteredNativeFunction(p, funcInfo, parameters, statementInfo);
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

            return InvokeRegisteredNativeFunction(p, functionInfo, parameters, statementInfo);
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
            EvaluationStatement[] parameters, StatementInfo statementInfo,
            string pureBashFallbackBody = null)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetNativeFunctionInfo(functionInfo.ClassName, functionInfo.Name, out var funcInfo))
            {
                return InvokeRegisteredNativeFunction(p, funcInfo, parameters, statementInfo);
            }

            var orderedImplementations =
                ThirdPartyUtilitySettings.OrderImplementations(p.Context.Flags, utilityCommands).ToList();

            if (p.Context.Flags.BindThirdPartyUtilitiesAtInit && orderedImplementations.Count > 0)
            {
                p.Context.RegisterUtilityFunctionInitBinding(new UtilityFunctionInitBinding(
                    functionInfo.Fqn,
                    orderedImplementations,
                    pureBashFallbackBody));
                p.Context.GeneralScope.ReserveNewFunction(functionInfo);
                return InvokeRegisteredNativeFunction(p, functionInfo, parameters, statementInfo);
            }

            using (var funcWriter = new StringWriter())
            {
                funcWriter.Write("function ");
                funcWriter.Write(functionInfo.Fqn);
                funcWriter.WriteLine("() {");

                var isFirst = true;
                var wroteBranch = false;
                var utilities = p.Context.Api.Utilities;

                foreach (var utility in orderedImplementations)
                {
                    if (!utilities.TryGetValue(utility.Key, out var util))
                    {
                        continue;
                    }

                    var condition = GetUtilityLookupTestVariableName(p.Context, p.MetaWriter, util);

                    funcWriter.Write(isFirst ? "if [ " : "elif [ ");
                    isFirst = false;
                    wroteBranch = true;
                    funcWriter.Write(condition);
                    funcWriter.WriteLine(" ]");
                    funcWriter.WriteLine("then");
                    funcWriter.WriteLine(utility.Value);
                }

                if (!string.IsNullOrWhiteSpace(pureBashFallbackBody))
                {
                    funcWriter.WriteLine(wroteBranch ? "else" : "if true");
                    funcWriter.WriteLine("then");
                    funcWriter.WriteLine(pureBashFallbackBody);
                    wroteBranch = true;
                }

                if (wroteBranch)
                {
                    funcWriter.WriteLine("fi");
                }

                funcWriter.WriteLine("}");

                p.MetaWriter.Write(funcWriter);
            }

            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return InvokeRegisteredNativeFunction(p, functionInfo, parameters, statementInfo);
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
            if (!FunctionStatementTranspilerBase.ValidateParameters(p.Context, p.Scope, Parameters, parameters,
                out var exception))
            {
                throw exception;
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