using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.System.OS;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.System.OS
{
    public partial class BashOS : ApiOS
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetHostName(),
            new BashGetKernelName(),
            new BashGetArchitecture(),
            new BashGetOsVersion(),
        };

        public class BashGetHostName : GetHostName
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "hostname");
        }

        public class BashGetKernelName : GetKernelName
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "uname -s");
        }

        public class BashGetArchitecture : GetArchitecture
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "uname -m");
        }

        public class BashGetOsVersion : GetOsVersion
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [ -r /etc/os-release ]; then grep PRETTY_NAME= /etc/os-release | cut -d= -f2- | tr -d '\"'; else uname -sr; fi");
        }
    }
}
