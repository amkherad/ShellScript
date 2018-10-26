using System.Globalization;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
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
            private const string ApiMathAbsBashMethodName = "GetLocaleLength";

            public override string Name => nameof(GetLength);
            public override string Summary => "Returns the length of the string. (may vary depending on current locale)";
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Decimal;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter
            };

            private FunctionInfo _functionInfo;

            public GetLength()
            {
                _functionInfo = new FunctionInfo(DataTypes.Decimal, ApiMathAbsBashMethodName,
                    null, ClassAccessName, false, Parameters, null);
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var str = functionCallStatement.Parameters[0];

                switch (str)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        return InlineConstant(constantValueStatement.DataType, constantValueStatement.Value,
                            constantValueStatement);
                    }
                    case VariableAccessStatement variableAccessStatement:
                    {
                        if (p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                        {
                            if (varInfo.DataType.IsString())
                            {
                                return new ApiMethodBuilderRawResult(new ExpressionResult(
                                    DataType,
                                    $"${{#{varInfo.AccessName}}}",
                                    variableAccessStatement
                                ));
                            }

                            throw new TypeMismatchCompilerException(varInfo.DataType, DataTypes.String,
                                variableAccessStatement.Info);
                        }

                        if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                        {
                            return InlineConstant(constInfo.DataType, constInfo.Value, variableAccessStatement);
                        }

                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }
                    default:
                    {
                        return WriteNativeMethod(this, p, "echo ${#1}", _functionInfo,
                            functionCallStatement.Parameters, functionCallStatement.Info);
                    }
                }
            }

            public static ApiMethodBuilderInlineResult InlineConstant(DataTypes dataType, string value,
                IStatement statement)
            {
                if (dataType.IsString())
                {
                    return Inline(
                        new ConstantValueStatement(DataTypes.Decimal,
                            value.Length.ToString(NumberFormatInfo.InvariantInfo),
                            statement.Info)
                    );
                }

                throw new TypeMismatchCompilerException(dataType, DataTypes.String, statement.Info);
            }
        }
    }
}