using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBaseFunction : IApiFunc
    {
        public abstract string Name { get; }
        public abstract bool IsStatic { get; }
        public abstract bool AllowDynamicParams { get; }
        public abstract IApiParameter[] Parameters { get; }


        public abstract IApiMethodBuilderResult BuildGeneral(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters);

        public virtual IApiMethodBuilderResult BuildIfCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters) =>
            ApiMethodBuilderFallbackToGeneralResult.Instance;

        public virtual IApiMethodBuilderResult BuildForCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters) =>
            ApiMethodBuilderFallbackToGeneralResult.Instance;

        public virtual IApiMethodBuilderResult BuildWhileCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters) =>
            ApiMethodBuilderFallbackToGeneralResult.Instance;

        public virtual IApiMethodBuilderResult BuildDoWhileCondition(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, EvaluationStatement[] parameters) =>
            ApiMethodBuilderFallbackToGeneralResult.Instance;

        public virtual IApiMethodBuilderResult BuildUnaryCondition(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, EvaluationStatement[] parameters) =>
            ApiMethodBuilderFallbackToGeneralResult.Instance;


        public void AssertParameters(EvaluationStatement[] parameters)
        {
        }
    }
}