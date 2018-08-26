using System.Linq.Expressions;
using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Sdk;
using ShellScript.Unix.Bash.PlatformTranspiler;
using ShellScript.Unix.Bash.Sdk;

namespace ShellScript.Unix.Bash
{
    public class UnixBashPlatform : IPlatform
    {
        public string Name => "Unix-Bash";

        public string[] CompilerConstants { get; } =
        {
            "unix",
            "bash",
        };

        public ISdk Sdk { get; } = new UnixBashSdk();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new BashPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            //new IfElseTranspiler()
            new BashVariableDefinitionStatementTranspiler(),
            new BashEvaluationStatementTranspiler(),
            new BashFunctionStatementTranspiler(),
        };
    }
}