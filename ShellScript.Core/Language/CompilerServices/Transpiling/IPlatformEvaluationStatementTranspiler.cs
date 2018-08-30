using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public interface IPlatformEvaluationStatementTranspiler : IPlatformStatementTranspiler
    {
        string PinEvaluationToVariable(Context context, Scope scope, TextWriter pinCodeWriter,
            EvaluationStatement statement);
    }
}