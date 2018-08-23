using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public interface IPlatformStatementTranspiler
    {
        Type StatementType { get; }

        bool CanInline(Context context, Scope scope, IStatement statement);

        bool Validate(Context context, Scope scope, IStatement statement, out string message);
        
        void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement);
        void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement);
    }
}