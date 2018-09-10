using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api;
using ShellScript.Unix.Bash.PlatformTranspiler;

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

        public IApi Api { get; } = new UnixBashApi();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new BashPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new BashBlockStatementTranspiler(),
            new BashFunctionCallStatementTranspiler(),
            new BashEchoStatementTranspiler(),
            new BashIfElseStatementTranspiler(),
            new BashSwitchCaseStatementTranspiler(),
            new BashVariableDefinitionStatementTranspiler(),
            new BashEvaluationStatementTranspiler(),
            new BashFunctionStatementTranspiler(),
        };
        
        public void ReviseFlags(CompilerFlags flags)
        {
            
        }
    }
}