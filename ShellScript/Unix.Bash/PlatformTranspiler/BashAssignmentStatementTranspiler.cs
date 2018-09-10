using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashAssignmentStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(AssignmentStatement);
        
        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return true;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, TextWriter nonInlinePartWriter,
            IStatement statement)
        {
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, IStatement statement)
        {
            
        }
    }
}