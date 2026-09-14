using System.Collections.Generic;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Data.Json;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Data.Json
{
    public partial class BashJson : ApiJson
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashIsValid(),
            new BashGetPath(),
            new BashPrettyPrint(),
        };

        private static readonly Dictionary<string, string> PrettyBodies = new Dictionary<string, string>
        {
            {BashFunction.JqUtilityName, "printf '%s' \"$1\" | jq ."},
            {
                BashFunction.PythonUtilityName,
                "python3 -c 'import json,sys; print(json.dumps(json.loads(sys.argv[1]), indent=2))' \"$1\""
            },
        };

        public class BashIsValid : IsValid
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.Boolean, nameof(IsValid), null, ClassAccessName, false,
                    Parameters, null);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, info,
                    new Dictionary<string, string>
                    {
                        {BashFunction.JqUtilityName, "printf '%s' \"$1\" | jq -e . >/dev/null"},
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import json,sys; json.loads(sys.argv[1])' \"$1\" >/dev/null"
                        },
                    }, call.Parameters, call.Info,
                    "python3 -c 'import json,sys; json.loads(sys.argv[1])' \"$1\" >/dev/null && echo 1 || echo 0");
            }
        }

        public class BashGetPath : GetPath
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.String, nameof(GetPath), null, ClassAccessName, false,
                    Parameters, null);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, info,
                    new Dictionary<string, string>
                    {
                        {
                            BashFunction.JqUtilityName,
                            "printf '%s' \"$1\" | jq -r --arg p \"$2\" '$p'"
                        },
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import json,sys; d=json.loads(sys.argv[1]); p=sys.argv[2].lstrip(\".\").split(\".\"); c=d\n" +
                            "for k in p: c=c[k] if k else c; print(c)' \"$1\" \"$2\""
                        },
                    }, call.Parameters, call.Info,
                    "printf '%s' \"$1\" | python3 -c 'import json,sys; d=json.loads(sys.stdin.read()); print(d)'");
            }
        }

        public class BashPrettyPrint : PrettyPrint
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.String, nameof(PrettyPrint), null, ClassAccessName, false,
                    Parameters, null);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, info, PrettyBodies, call.Parameters,
                    call.Info, PrettyBodies[BashFunction.PythonUtilityName]);
            }
        }
    }
}
