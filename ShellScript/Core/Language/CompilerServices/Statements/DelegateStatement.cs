using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DelegateDefinitionStatement : DefinitionStatement
    {
        public DelegateDefinitionStatement(string name, bool hasDefaultValue, StatementInfo info)
            : base(DataTypes.Delegate, name, defaultValue, hasDefaultValue, info)
        {
        }
    }
}