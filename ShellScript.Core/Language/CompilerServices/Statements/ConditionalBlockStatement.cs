namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConditionalBlockStatement : LogicalStatement
    {
        public override bool IsBlockStatement => true;

        public IStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(IStatement condition, IStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }
    }
}