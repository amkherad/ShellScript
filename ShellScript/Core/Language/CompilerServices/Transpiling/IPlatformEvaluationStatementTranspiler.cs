using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public interface IPlatformEvaluationStatementTranspiler : IPlatformStatementTranspiler
    {
        string PinEvaluationToVariable(Context context, Scope scope, TextWriter metaWriter, TextWriter pinCodeWriter,
            EvaluationStatement statement);
        
        (DataTypes, string, EvaluationStatement) GetInline(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);
        
        (DataTypes, string, EvaluationStatement) GetInlineConditional(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);
    }
}