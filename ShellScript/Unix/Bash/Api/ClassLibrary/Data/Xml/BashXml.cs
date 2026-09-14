using System.Collections.Generic;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Data.Xml;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Data.Xml
{
    public partial class BashXml : ApiXml
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
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import sys,xml.etree.ElementTree as ET; r=ET.fromstring(sys.argv[1]); print(r.findtext(sys.argv[2]))' \"$1\" \"$2\""
                        },
                    }, call.Parameters, call.Info,
                    "python3 -c 'import sys,xml.etree.ElementTree as ET; print(ET.fromstring(sys.argv[1]).tag)' \"$1\"");
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
                        {
                            BashFunction.PythonUtilityName,
                            "python3 -c 'import xml.etree.ElementTree as ET,sys; ET.fromstring(sys.argv[1])' \"$1\" >/dev/null"
                        },
                    }, call.Parameters, call.Info,
                    "python3 -c 'import xml.etree.ElementTree as ET,sys; ET.fromstring(sys.argv[1])' \"$1\" >/dev/null && echo 1 || echo 0");
            }
        }
    }
}
