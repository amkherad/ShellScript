using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class WhileStatement : ConditionalBlockStatement
    {
        public WhileStatement(IStatement condition, IStatement statement, ParserInfo parserInfo)
            : base(condition, statement, parserInfo)
        {
        }
    }
}