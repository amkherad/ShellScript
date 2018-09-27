using System;
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

        public ValueTuple<DataTypes, string, string>[] CompilerConstants { get; } =
        {
            (DataTypes.Boolean, "Unix", "true"),
            (DataTypes.Boolean, "Bash", "true"),
        };

        public IApi Api { get; } = new UnixBashApi();

        public IPlatformMetaInfoTranspiler MetaInfoWriter { get; } = new BashPlatformMetaInfoTranspiler();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new BashAssignmentStatementTranspiler(),
            new BashBlockStatementTranspiler(),
            new BashFunctionCallStatementTranspiler(),
            new BashEchoStatementTranspiler(),
            new BashReturnStatementTranspiler(),
            new BashIfElseStatementTranspiler(),
            new BashSwitchCaseStatementTranspiler(),
            new BashVariableDefinitionStatementTranspiler(),
            new BashEvaluationStatementTranspiler(),
            new BashFunctionStatementTranspiler(),
        };
        
        public CompilerFlags ReviseFlags(CompilerFlags flags)
        {
            return flags;
        }
    }
}