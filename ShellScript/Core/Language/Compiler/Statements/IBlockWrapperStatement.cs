namespace ShellScript.Core.Language.Compiler.Statements
{
    public interface IBlockWrapperStatement : IStatement
    {
        IStatement Statement { get; }
    }
}