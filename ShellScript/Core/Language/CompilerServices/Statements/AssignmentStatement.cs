namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class AssignmentStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public EvaluationStatement LeftSide { get; }
        public EvaluationStatement RightSide { get; }


        public AssignmentStatement(EvaluationStatement leftSide, EvaluationStatement rightSide,
            StatementInfo info, IStatement parentStatement = null)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
            Info = info;
            ParentStatement = parentStatement;

            TraversableChildren = StatementHelpers.CreateChildren(leftSide, rightSide);
        }

        public override string ToString()
        {
            return $"{LeftSide} = {RightSide}";
        }
    }
}