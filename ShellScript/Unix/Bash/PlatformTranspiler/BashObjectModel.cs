using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public static class BashObjectModel
    {
        public const string SelfNameref = "self";

        public static string FormatInstanceFieldRead(string instanceExpression, string fieldName)
        {
            return $"${{{instanceExpression}[{fieldName}]}}";
        }

        public static string FormatInstanceFieldWrite(string instanceExpression, string fieldName, string valueExpression)
        {
            return $"{instanceExpression}[{fieldName}]={valueExpression}";
        }

        public static string GetMethodFunctionName(string className, string methodName, bool isConstructor)
        {
            return isConstructor ? $"{className}__new" : $"{className}__{methodName}";
        }

        public static string GetInstanceStorageName(VariableInfo instanceInfo)
        {
            return instanceInfo.ReName ?? instanceInfo.Name;
        }

        public static string GetInstanceFieldReadExpression(VariableInfo instanceInfo, string fieldName,
            bool instanceUsesNameref)
        {
            var storage = GetInstanceStorageName(instanceInfo);
            if (instanceUsesNameref)
            {
                return $"${{{SelfNameref}[{fieldName}]}}";
            }

            return FormatInstanceFieldRead(storage, fieldName);
        }

        public static string GetInstanceFieldWriteTarget(VariableInfo instanceInfo, string fieldName,
            bool instanceUsesNameref)
        {
            var storage = GetInstanceStorageName(instanceInfo);
            if (instanceUsesNameref)
            {
                return $"{SelfNameref}[{fieldName}]";
            }

            return $"{storage}[{fieldName}]";
        }

        public static void WriteDeclareInstance(Context context, Scope scope, TextWriter writer, string instanceName)
        {
            if (scope.IsInsideMethod)
            {
                writer.WriteLine($"local -A {instanceName}");
            }
            else
            {
                writer.WriteLine($"declare -A {instanceName}");
            }
        }

        public static void WriteConstructorInvocation(Context context, Scope scope, TextWriter writer,
            TextWriter metaWriter, string className, string instanceName, EvaluationStatement[] arguments,
            StatementInfo info, IStatement usageContext)
        {
            var ctorName = GetMethodFunctionName(className, className, true);
            var p = new ExpressionBuilderParams(context, scope, metaWriter, writer, usageContext);
            var call = new System.Text.StringBuilder();
            call.Append(ctorName);
            call.Append(' ');
            call.Append(instanceName);

            if (arguments != null)
            {
                var transpiler = context.GetEvaluationTranspilerForStatement(arguments[0]);
                foreach (var arg in arguments)
                {
                    call.Append(' ');
                    var argResult = transpiler.GetExpression(context, scope, metaWriter, writer, usageContext, arg);
                    call.Append(argResult.Expression);
                }
            }

            writer.WriteLine(call.ToString());
        }
    }
}
