using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForStatement : ConditionalBlockStatement
    {
        public IStatement PreLoopAssignment { get; }
        public IStatement AfterLoopEvaluations { get; }
        
        public ForStatement(IStatement preLoopAssignment , IStatement condition, IStatement afterLoopEvaluations, IStatement statement, ParserInfo parserInfo)
            : base(condition, statement, parserInfo)
        {
            PreLoopAssignment = preLoopAssignment;
            AfterLoopEvaluations = afterLoopEvaluations;
        }
    }
}