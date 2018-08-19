using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ConditionalBlockStatement : LogicalStatement
    {
        public override bool IsBlockStatement => true;
        public override ParserInfo ParserInfo { get; }

        public LogicalStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(LogicalStatement condition, IStatement statement, ParserInfo parserInfo)
        {
            Condition = condition;
            Statement = statement;
            ParserInfo = parserInfo;
        }
    }
}