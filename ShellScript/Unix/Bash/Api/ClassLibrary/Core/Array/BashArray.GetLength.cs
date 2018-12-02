using System.Globalization;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array
{
    public partial class BashArray
    {
        public class BashGetLength : GetLength
        {
            private const string ApiMathAbsBashMethodName = "GetArrayLength";

            private FunctionInfo _functionInfo;

            public BashGetLength()
            {
                _functionInfo = new FunctionInfo(TypeDescriptor.Integer, ApiMathAbsBashMethodName,
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
                                    $"${{#{varInfo.AccessName}}}",
                                    variableAccessStatement
                                ));
                            }

                            throw new TypeMismatchCompilerException(varInfo.TypeDescriptor, TypeDescriptor.String,
                                variableAccessStatement.Info);
                        }

                        if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                        {
                            return InlineConstant(constInfo.TypeDescriptor, constInfo.Value, variableAccessStatement);
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

            public static ApiMethodBuilderInlineResult InlineConstant(TypeDescriptor typeDescriptor, string value,
                IStatement statement)
            {
                if (typeDescriptor.IsString())
                {
                    return Inline(
                        new ConstantValueStatement(TypeDescriptor.Integer,
                            value.Length.ToString(NumberFormatInfo.InvariantInfo),
                            statement.Info)
                    );
                }

                throw new TypeMismatchCompilerException(typeDescriptor, TypeDescriptor.String, statement.Info);
            }
        }
    }
}