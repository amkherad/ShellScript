using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class AssignmentStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        public override StatementInfo Info { get; }

        public IStatement LeftSide { get; }
        public IStatement RightSide { get; }
        
        public AssignmentStatement(IStatement leftSide, IStatement rightSide, StatementInfo info)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
            Info = info;
        }



        public override IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return LeftSide;
                yield return RightSide;
            }
        }
    }
}