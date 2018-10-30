using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders
{
    public readonly struct PinnedVariableResult
    {
        public TypeDescriptor TypeDescriptor { get; }
        public string Name { get; }
        public string Expression { get; }
        public EvaluationStatement Template { get; }

        public PinnedVariableResult(TypeDescriptor typeDescriptor, string name, string expression,
            EvaluationStatement template)
        {
            TypeDescriptor = typeDescriptor;
            Name = name;
            Expression = expression;
            Template = template;
        }

        public static implicit operator ExpressionResult(PinnedVariableResult result)
        {
            return new ExpressionResult(
                result.TypeDescriptor,
                result.Expression,
                result.Template
            );
        }
    }
}