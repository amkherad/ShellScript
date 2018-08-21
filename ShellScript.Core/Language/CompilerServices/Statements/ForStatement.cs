namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForStatement : ConditionalBlockStatement
    {
        public IStatement PreLoopAssignment { get; }
        public IStatement AfterLoopEvaluations { get; }
        
        public ForStatement(IStatement preLoopAssignment , IStatement condition, IStatement afterLoopEvaluations, IStatement statement)
            : base(condition, statement)
        {
            PreLoopAssignment = preLoopAssignment;
            AfterLoopEvaluations = afterLoopEvaluations;
        }
    }
}