namespace ShellScript.Core.Language.Compiler.Statements
{
    public interface IBranchWrapperStatement : IStatement
    {
        IStatement[] Branches { get; }
    }
}