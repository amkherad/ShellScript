using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Cli;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Cli
{
    public partial class BashCli : ApiCli
    {
        private const string ScriptArgsInitKey = "_SS_SCRIPT_ARGS_INIT";

        private static void EnsureScriptArgsSnapshot(ExpressionBuilderParams p)
        {
            if (!p.Context.Includes.Add(ScriptArgsInitKey))
            {
                return;
            }

            p.MetaWriter.WriteLine("_SS_SCRIPT_ARGS=(\"$@\")");
        }

        public override IApiFunc[] Functions { get; } =
        {
            new BashGetArgumentCount(),
            new BashGetArgument(),
            new BashHasFlag(),
            new BashGetFlagValue(),
        };

        public class BashGetArgumentCount : GetArgumentCount
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                EnsureScriptArgsSnapshot(p);
                return new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.Integer,
                    "${#_SS_SCRIPT_ARGS[@]}", call));
            }
        }

        public class BashGetArgument : GetArgument
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                EnsureScriptArgsSnapshot(p);
                var index = call.Parameters[0];
                var transpiler = p.Context.GetEvaluationTranspilerForStatement(index);
                var idx = transpiler.GetExpression(p, index);
                return new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.String,
                    $"${{_SS_SCRIPT_ARGS[{idx.Expression}]}}", call));
            }
        }

        public class BashHasFlag : HasFlag
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                EnsureScriptArgsSnapshot(p);
                return BashApiNative.Native(this, p, call, ClassAccessName,
                    "local f=\"$1\" a\n" +
                    "for a in \"${_SS_SCRIPT_ARGS[@]}\"; do [ \"$a\" = \"$f\" ] && echo 1 && return; done\n" +
                    "echo 0");
            }
        }

        public class BashGetFlagValue : GetFlagValue
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                EnsureScriptArgsSnapshot(p);
                return BashApiNative.Native(this, p, call, ClassAccessName,
                    "local f=\"$1\" a i\n" +
                    "for i in \"${!_SS_SCRIPT_ARGS[@]}\"; do\n" +
                    "  a=\"${_SS_SCRIPT_ARGS[$i]}\"\n" +
                    "  case \"$a\" in \"$f\"=*) printf '%s' \"${a#*=}\"; return ;; \"$f\") printf '%s' \"${_SS_SCRIPT_ARGS[$((i+1))]:-}\"; return ;; esac\n" +
                    "done\n" +
                    "printf ''");
            }
        }
    }
}
