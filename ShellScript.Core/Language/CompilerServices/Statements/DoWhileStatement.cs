using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DoWhileStatement : ConditionalBlockStatement
    {
        public DoWhileStatement(IStatement condition, IStatement statement, ParserInfo parserInfo)
            : base(condition, statement, parserInfo)
        {
        }
    }
}