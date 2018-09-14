namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public abstract class LogicalOperator : IOperator
    {
        public bool CanBeEmbedded => false;
        public abstract StatementInfo Info { get; }
        public abstract OperatorAssociativity Associativity { get; }
        public abstract int Order { get; }
        
        public IStatement[] TraversableChildren => new IStatement[0];
    }
}