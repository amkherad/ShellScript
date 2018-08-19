namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ConditionalBlockStatement : LogicalStatement
    {
        public override bool IsBlockStatement => true;
        
        public LogicalStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(LogicalStatement condition, IStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }
    }
}