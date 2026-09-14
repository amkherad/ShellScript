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
        private static string RequireArrayName(ExpressionBuilderParams p, EvaluationStatement array)
        {
            if (!(array is VariableAccessStatement variableAccess))
            {
                throw new InvalidStatementStructureCompilerException(array, array.Info);
            }

            if (!p.Scope.TryGetVariableInfo(variableAccess, out var variableInfo) ||
                !variableInfo.TypeDescriptor.IsArray())
            {
                throw new TypeMismatchCompilerException(variableInfo?.TypeDescriptor ?? TypeDescriptor.Any,
                    TypeDescriptor.Array, array.Info);
            }

            return variableInfo.AccessName;
        }

        public class BashIndexOf : IndexOf
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "local n=\"$1\" v=\"$2\" i=0 len\n" +
                    "eval \"len=\\${#\"$n\"[@]}\"\n" +
                    "while [ \"$i\" -lt \"$len\" ]; do\n" +
                    "  eval \"cur=\\${\"$n\"[$i]}\"\n" +
                    "  if [ \"$cur\" = \"$v\" ]; then echo \"$i\"; return; fi\n" +
                    "  i=$((i+1))\n" +
                    "done\n" +
                    "echo -1");
        }

        public class BashContains : Contains
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "local idx\n" +
                    "idx=$(Array_IndexOf \"$1\" \"$2\")\n" +
                    "if [ \"$idx\" -ge 0 ]; then echo 1; else echo 0; fi");
        }

        public class BashClear : Clear
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var name = RequireArrayName(p, call.Parameters[0]);
                return new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.Void,
                    $"{name}=()", call));
            }
        }

        public class BashReverse : Reverse
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "local n=\"$1\" i j tmp len\n" +
                    "eval \"len=\\${#\"$n\"[@]}\"\n" +
                    "i=0; j=$((len-1))\n" +
                    "while [ \"$i\" -lt \"$j\" ]; do\n" +
                    "  eval \"tmp=\\${\"$n\"[$i]}\"\n" +
                    "  eval \"${n}[$i]=\\${\"$n\"[$j]}\"\n" +
                    "  eval \"${n}[$j]=\\$tmp\"\n" +
                    "  i=$((i+1)); j=$((j-1))\n" +
                    "done");
        }

        public class BashAdd : Add
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var name = RequireArrayName(p, call.Parameters[0]);
                var valueTranspiler = p.Context.GetEvaluationTranspilerForStatement(call.Parameters[1]);
                var value = valueTranspiler.GetExpression(p, call.Parameters[1]);
                return new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.Void,
                    $"{name}+=(\"{value.Expression}\")", call));
            }
        }
    }
}
