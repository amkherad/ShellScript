namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class AssignmentOperator : IOperator
    {
        public int Order => 20;
        public StatementInfo Info { get; }
        public OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
        public bool CanBeEmbedded => false;
        
        public IStatement[] TraversableChildren => StatementHelpers.EmptyStatements;
        
        
        public AssignmentOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "=";
    }
}