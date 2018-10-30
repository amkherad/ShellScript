using System.IO;

namespace ShellScript.Core.Language.Compiler.Transpiling
{
    public interface IPlatformMetaInfoTranspiler
    {
        void WritePrologue(Context context, TextWriter writer);
        void WriteEpilogue(Context context, TextWriter writer);

        void WriteComment(Context context, TextWriter writer, string comment);
        void WriteSeparator(Context context, TextWriter writer);
    }
}