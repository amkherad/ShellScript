using System;
using System.Globalization;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class ApiString
    {
        public class IsNullOrWhiteSpace : BashFunction
        {
            private const string ApiMathAbsBashMethodName = "Abs_Bash";

            public override string Name => nameof(IsNullOrWhiteSpace);
            public override string Summary => "Checks whether a string is null or only contains spaces.";
            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter
            };

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var number = functionCallStatement.Parameters[0];

                switch (number)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        return InlineConstant(constantValueStatement.TypeDescriptor, constantValueStatement.Value,
                            constantValueStatement);
                    }
                    case VariableAccessStatement variableAccessStatement:
                    {
                        if (p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                        {
                            if (varInfo.TypeDescriptor.IsString())
                            {
                                return new ApiMethodBuilderRawResult(new ExpressionResult(
                                    TypeDescriptor,
                                    $"[[ -z ${{{varInfo.AccessName}// }} ]]",
                                    variableAccessStatement
                                ));
                            }
                        }
                        else if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                        {
                            return InlineConstant(constInfo.TypeDescriptor, constInfo.Value, variableAccessStatement);
                        }

                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }
                    default:
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public static ApiMethodBuilderInlineResult InlineConstant(TypeDescriptor typeDescriptor, string value,
                IStatement statement)
            {
                if (!typeDescriptor.IsString())
                {
                    throw new TypeMismatchCompilerException(typeDescriptor, TypeDescriptor.String, statement.Info);
                }

                return Inline(
                    new ConstantValueStatement(TypeDescriptor.String,
                        string.IsNullOrWhiteSpace(BashTranspilerHelpers.GetString(value))
                            .ToString(NumberFormatInfo.InvariantInfo),
                        statement.Info)
                );
            }
        }
    }
}