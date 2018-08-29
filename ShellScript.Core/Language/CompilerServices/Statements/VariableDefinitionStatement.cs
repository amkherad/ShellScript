using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableDefinitionStatement : DefinitionStatement
    {
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