using System.Globalization;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array
{
    public partial class BashArray
    {
        public class BashInitialize : Initialize
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

                var lengthStatement = functionCallStatement.Parameters[1];
                var indexName = p.Scope.NewHelperVariable(TypeDescriptor.Integer, "array_index");
                var elementType = variableInfo.TypeDescriptor.DataType & ~DataTypes.Array;
                var defaultValue = p.Context.Platform.GetDefaultValue(elementType);
                var constantLength = lengthStatement as ConstantValueStatement;
                if (constantLength != null && long.TryParse(constantLength.Value,
                    NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var length))
                {
                    if (length < 0)
                        throw new InvalidStatementStructureCompilerException(lengthStatement, lengthStatement.Info);

                    p.NonInlinePartWriter.Write(variableInfo.AccessName);
                    p.NonInlinePartWriter.WriteLine("=()");
                    if (length > 0)
                    {
                        p.NonInlinePartWriter.Write("for ((" + indexName + "=0; " + indexName + "<");
                        p.NonInlinePartWriter.Write(length.ToString(NumberFormatInfo.InvariantInfo));
                        p.NonInlinePartWriter.WriteLine("; " + indexName + "++)); do");
                        p.NonInlinePartWriter.Write(variableInfo.AccessName);
                        p.NonInlinePartWriter.WriteLine("[" + indexName + "]=" + defaultValue);
                        p.NonInlinePartWriter.WriteLine("done");
                    }
                }
                else
                {
                    var transpiler = p.Context.GetEvaluationTranspilerForStatement(lengthStatement);
                    var lengthResult = transpiler.GetExpression(p.Context, p.Scope, p.MetaWriter,
                        p.NonInlinePartWriter, functionCallStatement, lengthStatement);

                    p.NonInlinePartWriter.Write(variableInfo.AccessName);
                    p.NonInlinePartWriter.WriteLine("=()");
                    p.NonInlinePartWriter.Write("for ((" + indexName + "=0; " + indexName + "<");
                    p.NonInlinePartWriter.Write(lengthResult.Expression);
                    p.NonInlinePartWriter.WriteLine("; " + indexName + "++)); do");
                    p.NonInlinePartWriter.Write(variableInfo.AccessName);
                    p.NonInlinePartWriter.WriteLine("[" + indexName + "]=" + defaultValue);
                    p.NonInlinePartWriter.WriteLine("done");
                }

                return new ApiMethodBuilderRawResult(ExpressionResult.EmptyResult);
            }
        }
    }
}
