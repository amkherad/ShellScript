namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class AssignmentOperator : IOperator
    {
        public int Order { get; }
        public StatementInfo Info { get; }
        public OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        public bool CanBeEmbedded => false;
        
        public IStatement[] TraversableChildren => new IStatement[0];
        
        
        public AssignmentOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "=";
    }
}