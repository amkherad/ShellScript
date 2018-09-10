namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForStatement : ConditionalBlockStatement
    {
        public IStatement PreLoopAssignment { get; }
        public IStatement AfterLoopEvaluations { get; }
        
        public ForStatement(IStatement preLoopAssignment , EvaluationStatement condition, IStatement afterLoopEvaluations, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
            PreLoopAssignment = preLoopAssignment;
            AfterLoopEvaluations = afterLoopEvaluations;

            TraversableChildren =
                StatementHelpers.CreateChildren(preLoopAssignment, condition, afterLoopEvaluations, statement);
        }
    }
}