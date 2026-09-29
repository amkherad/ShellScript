using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.String;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashStringBuilder : ApiStringBuilder
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashCreate(),
            new BashAppend(),
            new BashAppendLine(),
            new BashClear(),
            new BashToString(),
            new BashGetLength(),
        };

        public class BashCreate : Create
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$1\"");
        }

        public class BashAppend : Append
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var builderVar = BashStringBuilderHelpers.RequireBuilderVariable(p, call.Parameters[0]);
                var valueExp = BashStringBuilderHelpers.GetParameterExpression(p, call, call.Parameters[1]);
                p.NonInlinePartWriter.Write("printf -v ");
                p.NonInlinePartWriter.Write(builderVar);
                p.NonInlinePartWriter.Write(" '%s%s' \"");
                p.NonInlinePartWriter.Write('$');
                p.NonInlinePartWriter.Write(builderVar);
                p.NonInlinePartWriter.Write("\" ");
                p.NonInlinePartWriter.WriteLine(valueExp);
                return BashStringBuilderHelpers.VoidMutationResult();
            }
        }

        public class BashAppendLine : AppendLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var builderVar = BashStringBuilderHelpers.RequireBuilderVariable(p, call.Parameters[0]);
                if (call.Parameters.Length > 1)
                {
                    var valueExp = BashStringBuilderHelpers.GetParameterExpression(p, call, call.Parameters[1]);
                    p.NonInlinePartWriter.Write("printf -v ");
                    p.NonInlinePartWriter.Write(builderVar);
                    p.NonInlinePartWriter.Write(" '%s%s\\n' \"");
                    p.NonInlinePartWriter.Write('$');
                    p.NonInlinePartWriter.Write(builderVar);
                    p.NonInlinePartWriter.Write("\" ");
                    p.NonInlinePartWriter.WriteLine(valueExp);
                }
                else
                {
                    p.NonInlinePartWriter.Write("printf -v ");
                    p.NonInlinePartWriter.Write(builderVar);
                    p.NonInlinePartWriter.Write(" '%s\\n' \"");
                    p.NonInlinePartWriter.Write('$');
                    p.NonInlinePartWriter.Write(builderVar);
                    p.NonInlinePartWriter.WriteLine("\"");
                }

                return BashStringBuilderHelpers.VoidMutationResult();
            }
        }

        public class BashClear : Clear
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var builderVar = BashStringBuilderHelpers.RequireBuilderVariable(p, call.Parameters[0]);
                p.NonInlinePartWriter.Write(builderVar);
                p.NonInlinePartWriter.WriteLine("=\"\"");
                return BashStringBuilderHelpers.VoidMutationResult();
            }
        }

        public class BashToString : ToString
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$1\"");
        }

        public class BashGetLength : GetLength
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "echo ${#1}");
        }
    }
}
