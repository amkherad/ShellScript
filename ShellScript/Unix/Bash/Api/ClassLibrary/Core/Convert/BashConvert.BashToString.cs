using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    public partial class BashConvert
    {
        public class BashToString : ToString
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var param = functionCallStatement.Parameters[0];

                var dt = param.GetDataType(p.Context, p.Scope);

                EvaluationStatement exp;
                if (dt.IsBoolean() || dt.IsNumber())
                {
                    //create an string concatenation expression, ExpressionBuilder has this functionality inside.
                    exp = new ArithmeticEvaluationStatement(param, new AdditionOperator(param.Info),
                        new ConstantValueStatement(TypeDescriptor.String, "", param.Info), param.Info,
                        param.ParentStatement);

                    param.ParentStatement = exp;
                }
                else if (dt.IsString())
                {
                    exp = param;
                }
                else
                {
                    throw new MethodParameterMismatchCompilerException(Name, functionCallStatement.Info);
                }

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(exp);
                var result = transpiler.GetExpression(p, exp);

                return new ApiMethodBuilderRawResult(result);
            }
        }
    }
}