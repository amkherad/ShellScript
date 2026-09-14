using System.Collections.Generic;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Data.Yaml;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Data.Yaml
{
    public partial class BashYaml : ApiYaml
    {
        public override IApiFunc[] Functions { get; } = {new BashGetPath(), new BashIsValid()};

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
                        {BashFunction.YqUtilityName, "printf '%s' \"$1\" | yq -r \"$2\""},
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import sys,yaml; d=yaml.safe_load(sys.argv[1]); p=sys.argv[2].split(\".\"); c=d\n" +
                            "for k in p:\n" +
                            "  c=c[k]\n" +
                            "print(c)' \"$1\" \"$2\""
                        },
                    }, call.Parameters, call.Info,
                    "python3 -c 'import sys,yaml; print(yaml.safe_load(sys.argv[1]))' \"$1\"");
            }
        }

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
                        {BashFunction.YqUtilityName, "printf '%s' \"$1\" | yq -e . >/dev/null"},
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import yaml,sys; yaml.safe_load(sys.argv[1])' \"$1\" >/dev/null"
                        },
                    }, call.Parameters, call.Info,
                    "python3 -c 'import yaml,sys; yaml.safe_load(sys.argv[1])' \"$1\" >/dev/null && echo 1 || echo 0");
            }
        }
    }
}
