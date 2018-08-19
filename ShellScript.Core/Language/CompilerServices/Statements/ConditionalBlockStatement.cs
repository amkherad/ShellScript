using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConditionalBlockStatement : LogicalStatement
    {
        public override bool IsBlockStatement => true;
        public override ParserInfo ParserInfo { get; }

        public IStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(IStatement condition, IStatement statement, ParserInfo parserInfo)
        {
            Condition = condition;
            Statement = statement;
            ParserInfo = parserInfo;
        }
    }
}