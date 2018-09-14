using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableDefinitionStatement : DefinitionStatement
    {
        public override bool CanBeEmbedded => true;
        
        public bool IsConstant { get; }
        
        public VariableDefinitionStatement(
            DataTypes dataType,
            string name,
            bool isConstant,
            EvaluationStatement value,
            StatementInfo info)
            : base(dataType, name, value, value != null, info)
        {
            IsConstant = isConstant;
        }
    }
}