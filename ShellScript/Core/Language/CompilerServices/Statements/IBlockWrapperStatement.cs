namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public interface IBlockWrapperStatement : IStatement
    {
        IStatement Statement { get; }
    }
}