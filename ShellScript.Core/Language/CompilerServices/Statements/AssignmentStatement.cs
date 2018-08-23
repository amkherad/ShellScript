using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class AssignmentStatement : EvaluationStatement
    {
        public AssignmentStatement(IStatement leftSide, IStatement rightSide)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
        }

        public override bool IsBlockStatement => false;
        
        public IStatement LeftSide { get; }
        public IStatement RightSide { get; }
        


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