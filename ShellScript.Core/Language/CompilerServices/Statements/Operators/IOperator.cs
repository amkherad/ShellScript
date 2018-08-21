namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public interface IOperator : IStatement
    {
        OperatorAssociativity Associativity { get; }
        int Order { get; }
    }
}