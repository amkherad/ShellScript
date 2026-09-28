using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Windows.PowerShell.Api.ClassLibrary.Base
{
    internal static class PowerShellApiNative
    {
        public static IApiMethodBuilderResult Native(
            ApiBaseFunction func,
            ExpressionBuilderParams p,
            FunctionCallStatement call,
            string className,
            string body)
        {
            func.AssertParameters(p, call.Parameters);
            var info = new FunctionInfo(func.TypeDescriptor, func.Name, null, className, false, func.Parameters, null);
            return ApiBaseFunction.WriteNativeMethod(func, p, body, info, call.Parameters, call.Info);
        }
    }
}
