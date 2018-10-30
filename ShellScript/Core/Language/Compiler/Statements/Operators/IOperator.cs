namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public interface IOperator : IStatement
    {
        OperatorAssociativity Associativity { get; }
        int Order { get; }

        string ToString();
    }
}