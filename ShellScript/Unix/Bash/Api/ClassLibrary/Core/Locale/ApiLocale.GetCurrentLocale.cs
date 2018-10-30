using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Locale
{
    public partial class ApiLocale
    {
        public class GetCurrentLocale : EvaluationFunction
        {
            public override string Name => nameof(GetCurrentLocale);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

            public override FunctionParameterDefinitionStatement[] Parameters { get; }


            protected override ExpressionResult CreateEvaluationExpression(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return CreateVariableAccess(TypeDescriptor, "LANG", functionCallStatement.Info);
            }
        }
    }
}