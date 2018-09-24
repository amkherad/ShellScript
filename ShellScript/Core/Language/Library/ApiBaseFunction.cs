using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBaseFunction : IApiFunc
    {
        public abstract string Name { get; }
        public abstract string ClassName { get; }
        public abstract DataTypes DataType { get; }
        public abstract bool IsStatic { get; }
        public abstract bool AllowDynamicParams { get; }
        public abstract FunctionParameterDefinitionStatement[] Parameters { get; }


        public abstract IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement functionCallStatement);

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMethodBuilderInlineResult Inline(EvaluationStatement statement)
        {
            return new ApiMethodBuilderInlineResult(statement);
        }
        
        public static IApiMethodBuilderResult UseNativeResourceMethod<TFunc>(
            TFunc func, ExpressionBuilderParams p, string resourceName, FunctionInfo functionInfo,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
            {
                return Inline(new FunctionCallStatement(funcInfo.ObjectName, funcInfo.Name, funcInfo.DataType,
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

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.DataType,
                parameters, statementInfo));
        }
        
        public static IApiMethodBuilderResult CreateNativeMethodWithUtilityExpressionSelector<TFunc>(
            TFunc func, ExpressionBuilderParams p, FunctionInfo functionInfo,
            IDictionary<string, string> utilityCommands,
            EvaluationStatement[] parameters, StatementInfo statementInfo)
            where TFunc : ApiBaseFunction
        {
            if (p.Scope.TryGetFunctionInfo(functionInfo, out var funcInfo))
            {
                return Inline(new FunctionCallStatement(funcInfo.ObjectName, funcInfo.Name, funcInfo.DataType,
                    parameters, statementInfo));
            }
            
            p.MetaWriter.Write("function ");
            p.MetaWriter.Write(functionInfo.Fqn);
            p.MetaWriter.WriteLine("() {");

            bool isFirst = true;
            var utilities = p.Context.Api.Utilities;
            foreach (var utility in utilityCommands)
            {
                if (utilities.TryGetValue(utility.Key, out var util))
                {
                    var condition = util.WriteExistenceCondition(p.Context, p.MetaWriter);
                    
                    if (isFirst)
                    {
                        p.MetaWriter.Write("if [ ");

                        isFirst = false;
                    }
                    else
                    {
                        p.MetaWriter.Write("elif [ ");
                    }
                    
                    p.MetaWriter.Write(condition);
                    p.MetaWriter.WriteLine(" ]");
                    p.MetaWriter.WriteLine("then");
                    p.MetaWriter.WriteLine(utility.Value);
                }
                else
                {
                    throw new InvalidOperationException("Utility is not installed.");
                }
            }
            
            p.MetaWriter.WriteLine("fi");
            p.MetaWriter.WriteLine("}");

            p.Context.GeneralScope.ReserveNewFunction(functionInfo);

            return Inline(new FunctionCallStatement(func.ClassName, functionInfo.Name, functionInfo.DataType,
                parameters, statementInfo));
        }
        

        public void AssertParameters(EvaluationStatement[] parameters)
        {
        }
    }
}