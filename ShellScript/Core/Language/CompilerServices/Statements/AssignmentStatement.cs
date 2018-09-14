namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class AssignmentStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public IStatement LeftSide { get; }
        public IStatement RightSide { get; }
        
        
        public AssignmentStatement(IStatement leftSide, IStatement rightSide, StatementInfo info)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(leftSide, rightSide);
        }
    }
}