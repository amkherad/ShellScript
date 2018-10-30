using System.IO;

namespace ShellScript.Core.Language.Compiler.Transpiling
{
    public interface IPlatformStringTranspiler : IPlatformStatementTranspiler
    {
        void WriteStringInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, string constantString);
        void WriteStringBlock(Context context, Scope scope, TextWriter writer, string constantString);
    }
}