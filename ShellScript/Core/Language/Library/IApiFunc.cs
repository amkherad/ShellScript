using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public interface IApiFunc : IApiObject
    {
        bool IsStatic { get; }
        bool AllowDynamicParams { get; }

        IApiParameter[] Parameters { get; }


        IApiMethodBuilderResult BuildGeneral(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        IApiMethodBuilderResult BuildIfCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        IApiMethodBuilderResult BuildForCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        IApiMethodBuilderResult BuildWhileCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        IApiMethodBuilderResult BuildDoWhileCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        IApiMethodBuilderResult BuildUnaryCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);
    }
}