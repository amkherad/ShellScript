using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Locale;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Locale
{
    public partial class BashLocale
    {
        public class BashGetCurrentLocale : GetCurrentLocale
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                
                var result = CreateVariableAccess(TypeDescriptor, "LANG", functionCallStatement.Info);

                return new ApiMethodBuilderRawResult(result);
            }
        }
    }
}