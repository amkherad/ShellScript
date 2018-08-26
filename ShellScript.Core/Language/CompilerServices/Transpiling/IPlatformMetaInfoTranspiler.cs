using System.IO;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public interface IPlatformMetaInfoTranspiler
    {
        void WritePrologue(Context context, TextWriter writer);
        void WriteEpilogue(Context context, TextWriter writer);
    }
}