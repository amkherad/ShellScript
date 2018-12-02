using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array
{
    public partial class BashArray
    {
        public class BashCopy : Copy
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var p0 = functionCallStatement.Parameters[0];
                var p1 = functionCallStatement.Parameters[1];

                if (!(p0 is VariableAccessStatement destination))
                {
                    throw new InvalidStatementStructureCompilerException(p0, p0.Info);
                }

                if (!p.Scope.TryGetVariableInfo(destination, out var dstInfo))
                {
                    throw new IdentifierNotFoundCompilerException(destination);
                }

                var dstVarName = destination.VariableName;


                if (p1 is ArrayStatement arrayStatement)
                {
                    var elements = arrayStatement.Elements;
                    var elementsResults = new List<string>(elements.Length);

                    foreach (var elem in elements)
                    {
                        var transpiler = p.Context.GetEvaluationTranspilerForStatement(elem);

                        var result = transpiler.GetExpression(p, elem);

                        elementsResults.Add(result.Expression);
                    }

                    if ((elementsResults.Sum(s => s.Length + 2) + dstVarName.Length + 3) <=
                        p.Context.Flags.ArrayManipulationColumnCount)
                    {
                        p.NonInlinePartWriter.Write(dstVarName);
                        p.NonInlinePartWriter.Write("=(");
                        p.NonInlinePartWriter.Write(string.Join(' ', elementsResults));
                        p.NonInlinePartWriter.WriteLine(")");
                    }
                    else
                    {
                        for (int i = 0; i < elementsResults.Count; i++)
                        {
                            p.NonInlinePartWriter.Write(dstVarName);
                            p.NonInlinePartWriter.Write('[');
                            p.NonInlinePartWriter.Write(i);
                            p.NonInlinePartWriter.Write("]=");
                            p.NonInlinePartWriter.WriteLine(elementsResults[i]);
                        }
                    }
                }
                else if (p1 is VariableAccessStatement source)
                {
                    if (!p.Scope.TryGetVariableInfo(source, out var srcInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(source);
                    }

                    var srcVarName = source.VariableName;

                    var isQuoteNeeded = srcInfo.TypeDescriptor.IsString();

                    //for i in ${!a[@]}; do
                    //    b[$i]="${a[$i]}"
                    //done

                    //for i in ${!a[@]}; do
                    p.NonInlinePartWriter.Write("for i in ${!");
                    p.NonInlinePartWriter.Write(srcVarName);
                    p.NonInlinePartWriter.WriteLine("[@]}; do");

                    //b[$i]="${a[$i]}"
                    p.NonInlinePartWriter.Write(dstVarName);
                    if (isQuoteNeeded)
                    {
                        p.NonInlinePartWriter.Write("[$i]=\"${");
                    }
                    else
                    {
                        p.NonInlinePartWriter.Write("[$i]=${");
                    }

                    p.NonInlinePartWriter.Write(srcVarName);
                    if (isQuoteNeeded)
                    {
                        p.NonInlinePartWriter.Write("[$i]}\"");
                    }
                    else
                    {
                        p.NonInlinePartWriter.Write("[$i]}");
                    }

                    p.NonInlinePartWriter.WriteLine();
                    p.NonInlinePartWriter.WriteLine("done");
                }
                else
                {
                    throw new InvalidStatementStructureCompilerException(p1, p1.Info);
                }

                return new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.Void, null, null,
                    new IExpressionNotice[] {new PinRequiredNotice()}));
            }
        }
    }
}