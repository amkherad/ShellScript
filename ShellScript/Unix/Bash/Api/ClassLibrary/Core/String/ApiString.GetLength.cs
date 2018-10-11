using System;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class ApiString
    {
        public class GetLength : BashFunction
        {
            public override string Name => nameof(GetLength);
            public override string Summary => "Returns the absolute value of the number.";
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Numeric;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter
            };

            public GetLength()
            {
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(functionCallStatement.Parameters);

                throw new NotImplementedException();
            }
        }
    }
}