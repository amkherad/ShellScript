using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class EvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override string PinEvaluationToInline(Context context, Scope scope, TextWriter pinCodeWriter, EvaluationStatement statement)
        {
            throw new NotImplementedException();
        }
    }
}