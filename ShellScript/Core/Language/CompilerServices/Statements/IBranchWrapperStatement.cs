namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public interface IBranchWrapperStatement : IStatement
    {
        IStatement[] Branches { get; }
    }
}