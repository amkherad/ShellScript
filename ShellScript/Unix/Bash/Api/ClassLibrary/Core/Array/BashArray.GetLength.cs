using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array
{
    public partial class BashArray
    {
        public class BashGetLength : GetLength
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var array = functionCallStatement.Parameters[0] as VariableAccessStatement;
                if (array == null)
                    throw new InvalidStatementStructureCompilerException(functionCallStatement.Parameters[0],
                        functionCallStatement.Parameters[0].Info);

                if (!p.Scope.TryGetVariableInfo(array, out var variableInfo))
                    throw new IdentifierNotFoundCompilerException(array);

                if (!variableInfo.TypeDescriptor.IsArray())
                    throw new TypeMismatchCompilerException(variableInfo.TypeDescriptor, TypeDescriptor.Array,
                        array.Info);

                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor, $"${{#{variableInfo.AccessName}[@]}}", array));
            }
        }
    }
}
