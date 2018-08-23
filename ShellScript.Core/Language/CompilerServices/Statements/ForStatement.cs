using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForStatement : ConditionalBlockStatement
    {
        public IStatement PreLoopAssignment { get; }
        public IStatement AfterLoopEvaluations { get; }
        
        public ForStatement(IStatement preLoopAssignment , IStatement condition, IStatement afterLoopEvaluations, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
            PreLoopAssignment = preLoopAssignment;
            AfterLoopEvaluations = afterLoopEvaluations;
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return PreLoopAssignment;
                yield return Condition;
                yield return AfterLoopEvaluations;
                yield return Statement;
            }
        }
    }
}