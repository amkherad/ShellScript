namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class AssignmentStatement : IStatement
    {
        public AssignmentStatement(IStatement leftSide, IStatement rightSide)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
        }

        public bool IsBlockStatement => false;
        
        public IStatement LeftSide { get; }
        public IStatement RightSide { get; }
    }
}