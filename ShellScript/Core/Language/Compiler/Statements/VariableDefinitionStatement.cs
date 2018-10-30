using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class VariableDefinitionStatement : DefinitionStatement
    {
        public override bool CanBeEmbedded => false;

        public bool IsConstant { get; }

        public VariableDefinitionStatement(
            TypeDescriptor typeDescriptor,
            string name,
            bool isConstant,
            EvaluationStatement value,
            StatementInfo info)
            : base(typeDescriptor, name, value, value != null, info)
        {
            IsConstant = isConstant;
        }
    }
}